using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using HSYLib.CS;
using INovaSDK;
using MathNet.Numerics.LinearAlgebra.Double;

namespace GS_Tracking_KR
{
    public partial class MainForm : Form
    {
        /* Software */
        bool flag_Simulation = false;
        LogFileWriter lfw = null;
        TestCodeForm TestCodeForm1 = null;
        delegate void dVoidString(string str);
        dVoidString dPrintToLog;
        StreamWriter sw_FSMtrackLog = null;
        StreamWriter sw_TrackLog = null;
        StreamWriter sw_EKFLog = null;

        /* General */
        double lat = 42.610681; // latitude, deg, Wallace Astrophysical Observatory (Westford, MA)
        double lon = -71.484163; // longitude, deg, Wallace Astrophysical Observatory (Westford, MA)
        //double lat = 42.360689; // latitude, deg, MIT bldg 37
        //double lon = -71.093197; // longitude, deg, MIT bldg 37
        //double lat = 39.020090; // latitude, deg, GGAO
        //double lon = -76.827039; // longitude, deg, GGAO
        double alt = 0; // altitude, meters
        DenseVector rGS_ECEF = null;

        /* Star tracking */
        List<StarCatalog.OneStarData> brightStars;
        StarCatalog.OneStarData starToTrack;

        /* Satellite tracking */
        HSYSGP4.HSYSGP4EasyUsing SGP4;
        double tStart;
        EKF_satellite_tracking EKF = null;

        /* Mount */
        CelestronMount Mount = null;
        double[] AziAltRef = new double[2]; // degrees
        double[] AziAltDotRef = new double[2]; // degrees/s
        double[] AziAltDotCmd = new double[2]; // as/s
        int MntTrackMode; // 0 = GoTo, 1 = Auto Cal Schedule, 2 = Track Star, 3 = Track TLE
        List<double> aziSched = new List<double> { 360,  60,  90, 120, 135,  90,  30, 360,  60, 120, 135, 120,  30, 300, 225, 240, 300, 330, 270, 225, 240, 300 }; //Old: { 360,  60, 120, 180, 150,  90,  30, 360,  60, 120, 150, 120,  30, 300, 210, 240, 300, 330, 270, 210, 240, 300 }
        List<double> altSched = new List<double> {  45,  45,  65,  65,  65,  45,  45,  65,  65,  55,  55,  70,  70,  70,  70,  65,  65,  55,  55,  55,  45,  45 }; //Old: {  30,  30,  30,  30,  45,  45,  45,  60,  60,  60,  60,  75,  75,  75,  75,  60,  60,  45,  45,  45,  30,  30 }
        DenseMatrix MntFSMGain = null; // deg/V
        double[] FSMoffloadAziAlt = new double[2]; // degrees
        bool flag_OffloadFSM = false;
        double[] AziAltStart = new double[2]; // degrees

        /* Inclinometer */
        Inclinometer Inclin = null;

        /* Star camera */
        INovaCam StarCam = null; // wrapper for the iNovaCam functions
        INovaCamera iNova = null; // iNova SDK class for interfacing with camera
        List<StarCamImgDisplay> windowsList = new List<StarCamImgDisplay>(); // list of all open display windows
        StarCatalog StarCatalog1 = null;
        const int minPix = 4; // minimum number of pixels to be considered a star
        const int maxPix = 1000; // maximum number of pixels to be  considered a star
        const double starCamFeedbackMinMag = 5000;
        int[] LOStel_XY = new int[2];
        bool StarCamFeedbackEnabled = false;
        double ang_LastFeedback = 100;
        DenseVector LOStel_ST = new DenseVector(3);
        Quaternion dq_ST = new Quaternion();

        /* Tracking camera */
        bool flag_trackCamInit = false;
        Bitmap TrackCamDispMap;
        byte[] imgBufGrab = new byte[163840]; // Image buffer
        double[] centroid = new double[2]; // centroid xy
        double[] peakPowerCentroid = new double[2]; // stored value of peak power on tracking cam

        /* FSM */
        double[] FSMVxy = new double[2]; // FSM commanded voltage
        DenseMatrix FSMCamGain = null; // V/pixel
        double FSMmaxV = 10;
        int FSMtrackMode; // 0 = scan, 1 = track
        double thetadot = 2; // scan angular rate
        double rdot = 3.8 * Math.PI; // scan radial rate
        double r = 0; // scan radius
        double theta = 0; // scan angle

        /* Oscilloscope */
        RxMeasPwr pwrMeas = null;

        /* Pointing calibration */
        TelPointingModel TelPntModel = null; // pointing model associated with telescope
        int minNstarsCal = 6; // minimum stars to use measurement for calibration
        double minMaxScoreCal = 30; // minimum score to use measurement for calibration

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lfw = new LogFileWriter();

            // Initialize serial ports.
            string[] ports = SerialPort.GetPortNames();
            cb_SelectSerialPort.DataSource = ports;
            cb_SelectInclinometerPort.DataSource = ports;

            Initdgvs(); // Initialize display boxes
            TelPntModel = new TelPointingModel(lat, lon);

            // Initialize telescope LOS to center of star camera.
            LOStel_ST[2] = 1;
            LOStel_XY[0] = 640;
            LOStel_XY[1] = 480;

            // Load the master star catalog.
            StarCatalog1 = new StarCatalog();
            if (StarCatalog1.masterCatStars == null)
                PrintToLog("Failed to load SKY2000V5 star catalog.");
            else
                PrintToLog("SKY2000V5 star catalog loaded.");

            // Load the reduced star catalog and initialize star ID.
            if (StarID.CheckForStarCatFiles())
            {
                StarID.ExternInitializeStarID();
                PrintToLog("Star ID initialized.");
            }
            else
                PrintToLog("Star ID initialization failed: Missing catalog files.");
            dPrintToLog = PrintToLog;

            // Initialize peak power to be the center of the IR camera.
            peakPowerCentroid[0] = 0.5 * 320;
            peakPowerCentroid[1] = 0.5 * 256;

            rGS_ECEF = HSYEarth.LLH_to_XYZ(lat, lon, alt); // Initialize ground station location
            /* // Hard coded gain values for testing
            MntFSMGain = new DenseMatrix(2,2);
            MntFSMGain[0, 0] = 1e-4;
            MntFSMGain[0, 1] = 0;
            MntFSMGain[1, 0] = 0;
            MntFSMGain[1, 1] = 1e-4;*/
        }

        /* Prints text to .csv log file and to screen. */
        private void PrintToLog(string text)
        {
            SessionLog.AppendText(text + "\n");
            SessionLog.ScrollToCaret();
            lfw.WriteLogLine("Main form log, " + text);
        }
        private void PrintToLogi(string text)
        {
            this.Invoke(dPrintToLog, text);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) // If form is closed, close serial port and log.
        {
            if (sp_Mount.IsOpen)
                sp_Mount.Close();
            if (StarCam != null)
                StarCam.CloseINovaCam();
            if (flag_trackCamInit)
                SU320CSX_Control.closeSU320CSX();
            if (sw_TrackLog != null)
            {
                sw_TrackLog.Close();
                sw_TrackLog = null;
            }
            if (sw_FSMtrackLog != null)
            {
                sw_FSMtrackLog.Close();
                sw_FSMtrackLog = null;
            }
            if (sw_EKFLog != null)
            {
                sw_EKFLog.Close();
                sw_EKFLog = null;
            }
            lfw.CloseLog();
        }

        /* Initializes the DataGridView for bright stars and calibration measurements. */
        private void Initdgvs()
        {
            // Initialize dgv of bright stars
            dgv_BrightStars.ColumnCount = 6;
            dgv_BrightStars.AllowUserToAddRows = false;

            int n = 0;
            dgv_BrightStars.Columns[n++].Name = "Name";
            dgv_BrightStars.Columns[n++].Name = "Mv";
            dgv_BrightStars.Columns[n++].Name = "Azi (deg)";
            dgv_BrightStars.Columns[n++].Name = "El (deg)";
            dgv_BrightStars.Columns[n++].Name = "RA";
            dgv_BrightStars.Columns[n++].Name = "DEC";

            n = 0;
            dgv_BrightStars.Columns[n++].Width = 80;
            dgv_BrightStars.Columns[n++].Width = 80;
            dgv_BrightStars.Columns[n++].Width = 90;
            dgv_BrightStars.Columns[n++].Width = 90;
            dgv_BrightStars.Columns[n++].Width = 80;
            dgv_BrightStars.Columns[n++].Width = 80;

            for (int i = 0; i < dgv_BrightStars.ColumnCount; i++)
                dgv_BrightStars.Columns[i].ReadOnly = true;

            // Initialize dgv of calibration measurements
            dgv_CalMeas.ColumnCount = 7;
            dgv_CalMeas.AllowUserToAddRows = false;
            n = 0;
            dgv_CalMeas.Columns[n++].Name = "#";
            dgv_CalMeas.Columns[n++].Name = "n ID";
            dgv_CalMeas.Columns[n++].Name = "Max Score";
            dgv_CalMeas.Columns[n++].Name = "RMS (as)";
            dgv_CalMeas.Columns[n++].Name = "Azi (deg)";
            dgv_CalMeas.Columns[n++].Name = "Alt (deg)";
            dgv_CalMeas.Columns[n++].Name = "Time";

            n = 0;
            dgv_CalMeas.Columns[n++].Width = 30;
            dgv_CalMeas.Columns[n++].Width = 40;
            dgv_CalMeas.Columns[n++].Width = 100;
            dgv_CalMeas.Columns[n++].Width = 90;
            dgv_CalMeas.Columns[n++].Width = 90;
            dgv_CalMeas.Columns[n++].Width = 90;
            dgv_CalMeas.Columns[n++].Width = 80;
        }

        /* Opens Test Code Form, used for testing new functions. */
        private void button1_Click(object sender, EventArgs e)
        {
            if (TestCodeForm1 == null || TestCodeForm1.IsDisposed)
            {
                TestCodeForm1 = new TestCodeForm(lfw, Mount, Inclin);
                TestCodeForm1.Show();
            }
            else
            {
                TestCodeForm1.Activate();
            }
        }

        /* Update DataGridView of calibration measurements. */
        private void Updatedgv_CalMeas()
        {
            dgv_CalMeas.Rows.Clear();
            List<TelCalMeasurement> CalMeas = TelPntModel.GetCalMeasList();

            for (int i = 0; i < CalMeas.Count; i++)
            {
                string[] row = new string[dgv_CalMeas.ColumnCount];
                int n = 0;
                TelCalMeasurement Meas = CalMeas[i];
                row[n++] = i.ToString();
                row[n++] = Meas.GetNstarsID().ToString();
                row[n++] = Meas.GetMaxScore().ToString("F2");
                row[n++] = (Meas.GetRMSE() * HSYMath.RTAS).ToString("F2");
                double[] AziAlt = Meas.GetAziAlt();
                row[n++] = (AziAlt[0] * HSYMath.RTD).ToString("F2");
                row[n++] = (AziAlt[1] * HSYMath.RTD).ToString("F2");
                row[n++] = Meas.GetTime().ToString("MM/dd HH:mm:ss");
                dgv_CalMeas.Rows.Add(row);
            }
        }

        /* Draw a plus in a display. */
        private void DrawPlus(Graphics g, Color c, double x, double y)
        {
            Pen pen = new Pen(c, 1);
            float d = 10f;
            float xd = (float)x; // resolution is reduced by factor of 2
            float yd = (float)y;
            float x1 = xd - d * 0.5f;
            float y1 = yd - d * 0.5f;
            float x2 = xd + d * 0.5f;
            float y2 = yd + d * 0.5f;
            g.DrawLine(pen, x1, yd, x2, yd);
            g.DrawLine(pen, xd, y1, xd, y2);
        }
/*MOUNT****************************************************************************************/
/* Connect to Celestron mount over serial port. */
        private void btn_MntConnect_Click(object sender, EventArgs e) // Connect to mount via serial port.
        {
            if (sp_Mount.IsOpen)
                sp_Mount.Close();
            if (cb_SelectSerialPort.SelectedIndex > -1)
            {
                string portName = cb_SelectSerialPort.SelectedItem.ToString();
                sp_Mount.PortName = portName;
                sp_Mount.BaudRate = 9600;
                sp_Mount.Open();
                PrintToLog(String.Format("Mount connected on port '{0}'", portName));
                Mount = new CelestronMount(sp_Mount);
            }
            else
            {
                MessageBox.Show("Please select a port first");
            }
        }

        /* Stops all mount motion. */
        private void btn_MntStop_Click(object sender, EventArgs e) 
        {
            tmr_CLMount.Enabled = false;
            if (sp_Mount.IsOpen)
            {
                Mount.StopGoTo();
                Mount.SendSlewCommand(0, 0);
                PrintToLog("Stop command sent.");
            }

            // Close all tracking logs.
            if (sw_EKFLog != null)
            {
                sw_EKFLog.Close();
                sw_EKFLog = null;
            }
            if (sw_TrackLog != null)
            {
                sw_TrackLog.Close();
                sw_TrackLog = null;
            }

            // Reset angle and rate references.
            AziAltRef[0] = 0;
            AziAltRef[1] = 0;
            AziAltDotRef[0] = 0;
            AziAltDotRef[1] = 0;

            // Reset dq.
            dq_ST[0] = 0;
            dq_ST[1] = 0;
            dq_ST[2] = 0;
            dq_ST[3] = 1;
            PrintToLog("dq cleared.");
            ang_LastFeedback = 100; // reset to very high

            // Reset autocalibration schedule.
            aziSched = new List<double> { 360, 60, 120, 180, 150, 90, 30, 360, 60, 120, 180, 120, 30, 300, 210, 240, 300, 330, 270, 210, 240, 300 };
            altSched = new List<double> { 30, 30, 30, 30, 45, 45, 45, 60, 60, 60, 60, 75, 75, 75, 75, 60, 60, 45, 45, 45, 30, 30 };
        }

        /* Send telescope to (Azi, Alt). */
        private void btn_GoTo_Click(object sender, EventArgs e)
        {
            if (Mount == null)
                return;

            double AziDeg;
            double AltDeg;
            if (Double.TryParse(rtb_GoToAzi.Text, out AziDeg))
            {
                if (Double.TryParse(rtb_GoToAlt.Text, out AltDeg))
                {
                    if (AziDeg < 0)
                        AziDeg += 360;
                    AziAltRef[0] = AziDeg;
                    AziAltRef[1] = AltDeg;
                    Mount.SendAngleCommand(HSYMath.DTR * AziAltRef[0], HSYMath.DTR * AziAltRef[1]);
                    PrintToLog("Mount commanded to Azi " + AziDeg + " deg, Alt " + AltDeg + " deg.");
                }
            }
        }

        /* Read (Azi, Alt) from mount. */
        private void btn_ReadMntAngles_Click(object sender, EventArgs e)
        {
            if (Mount == null)
                return;

            double[] angles = Mount.ReadAnglesDeg();

            if (angles != null)
            {
                AziRead.Text = angles[0].ToString("N2");
                AltRead.Text = angles[1].ToString("N2");
            }
        }

        /*STAR CAMERA**********************************************************************************/
        /* Initialize iNova star camera. */
        private void btn_InitStarCam_Click(object sender, EventArgs e) 
        {
            iNova = new INovaCamera();
            StarCam = new INovaCam(iNova);
            bool initialized = StarCam.InitINovaCam();
            if (initialized)
            {
                rtb_Exposure.Enabled = true;
                btn_TakeStarImg.Enabled = true;
                btn_StartGrabStarCam.Enabled = true;
            }
            PrintToLog("iNova camera initialized: " + initialized.ToString());
        }

        /* Take single image from star camera. */
        private void btn_TakeStarImg_Click(object sender, EventArgs e) 
        {
            DateTime t0 = DateTime.UtcNow;
            TakeStarCamImg();
            DateTime t1 = DateTime.UtcNow;
            TimeSpan dt = t1.Subtract(t0);
            PrintToLog("Capture time: " + dt.TotalSeconds + " s.");
        }

        /* Request star camera image. */
        private StarImg TakeStarCamImg()
        {
            if (StarCam == null)
                return null;
            DateTime t = DateTime.UtcNow; // Time of image capture
            PgmImage imgBuf = StarCam.TakeINovaPic(out t);
            double[] AziAlt = new double[2];
            if (Mount != null)
                AziAlt = Mount.ReadAnglesRad();
            if (imgBuf == null)
            {
                PrintToLog("Failed to take picture from star camera.");
                TakeStarCamImg();
                return null;
            }

            string fn = t.ToString("yyyyMMdd_HHmmss.ff");
            imgBuf.fn = fn;
            StarImg StarCamImg = new StarImg(imgBuf, minPix, maxPix);

            // If automatic star ID is on, do ID.
            if (chb_AutoIDStarsCheck.Checked)
            {
                StarID.PerformStarID(StarCamImg, null, StarCatalog1);
                // Add image to calibration set if it meets criteria.
                if (StarCamImg.GetMaxScore() > minMaxScoreCal && StarCamImg.GetNstarsID() > minNstarsCal)
                {
                    PrintToLog("Star camera image " + fn + " added for calibration.");
                    TelPntModel.addMeas(StarCamImg, t, AziAlt[0], AziAlt[1]);
                    Updatedgv_CalMeas();
                }
            }

            // Show star camera image.
            StarCamImgDisplay disp = new StarCamImgDisplay(this, StarCamImg, LOStel_XY, StarCamFeedbackEnabled, lfw); // calls up separate display to show star camera image
            disp.Show();

            this.BringToFront(); // Call up main form to front of display
            windowsList.Add(disp); // update list of open windows
            PrintToLog("Displaying image " + fn);

            // Save the image if check box is selected.
            if (SavePicsCheck.Checked)
            {
                FileStream ifs = new FileStream(fn + ".aa", FileMode.Create);
                BinaryWriter bw = new BinaryWriter(ifs);
                double ut = HSYTime.DateTime_to_UnixTime(t);
                bw.Write(AziAlt[0]);
                bw.Write(AziAlt[1]);
                bw.Write(ut);
                bw.Close();
                imgBuf.SaveAsFile();
            }
            return StarCamImg;
        }

        /* Opens existing star camera image from UI file. */
        private void btn_OpenStarImg_Click(object sender, EventArgs e) 
        {
            OpenFileDialog openFileUI = new OpenFileDialog();
            openFileUI.Filter = "Star Cam Image|*.pgm|All File|*.*";
            openFileUI.FilterIndex = 0;
            openFileUI.RestoreDirectory = true;
            openFileUI.Multiselect = true;

            try
            {
                if (openFileUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] fileNames = openFileUI.FileNames;
                    Array.Sort(fileNames);
                    for (int i = 0; i < fileNames.Length; i++)
                    {
                        string fn = fileNames[i];
                        PgmImage img = PgmImage.LoadImage_NotGeneral(fn);
                        DateTime t = DateTime.UtcNow;
                        double[] AziAlt = new double[2];
                        bool dataLog = false;
                        string fnn = Path.GetFileNameWithoutExtension(fn);
                        string fna = Path.GetDirectoryName(fn) + "\\" + fnn + ".aa";
                        if (File.Exists(fna))
                        {
                            dataLog = true;
                            FileStream fs = new FileStream(fna, FileMode.Open);
                            BinaryReader br = new BinaryReader(fs);
                            AziAlt[0] = br.ReadDouble();
                            AziAlt[1] = br.ReadDouble();
                            double ut = br.ReadDouble();
                            t = HSYTime.UnixTime_to_DateTime(ut);
                            br.Close();
                        }
                        StarImg StarCamImg = new StarImg(img, minPix, maxPix);
                        if (chb_AutoIDStarsCheck.Checked)
                        {
                            StarID.PerformStarID(StarCamImg, null, StarCatalog1);
                            if (dataLog)
                            {
                                if (StarCamImg.GetMaxScore() > minMaxScoreCal && StarCamImg.GetNstarsID() > minNstarsCal)
                                {
                                    PrintToLog("Star camera image " + fn + " added for calibration.");
                                    TelPntModel.addMeas(StarCamImg, t, AziAlt[0], AziAlt[1]);
                                    Updatedgv_CalMeas();
                                }
                            }
                        }
                        StarCamImgDisplay disp = new StarCamImgDisplay(this, StarCamImg, LOStel_XY, StarCamFeedbackEnabled, lfw); // LOOK HERE
                        disp.Show();
                        windowsList.Add(disp);
                    }
                }
            }
            catch (System.Exception exc)
            {
                MessageBox.Show("File open error: " + exc.Message);
            }
        }

        /* Set the LOS of the telescope in the star camera. */
        public void SetLOS_ST(DenseVector _LOS_ST, int[] _LOStel_XY)
        {
            LOStel_ST = _LOS_ST;
            LOStel_XY = _LOStel_XY;
        }

        /* Close all star image displays. */
        private void btn_CloseAllStarImg_Click(object sender, EventArgs e) // Close all display forms open.
        {
            for (int i = 0; i < windowsList.Count; i++)
                windowsList[i].Close();
            windowsList.Clear();
        }

        /* Update star camera exposure. */
        private void rtb_Exposure_Leave(object sender, EventArgs e)
        {
            double exposure;
            if (Double.TryParse(rtb_Exposure.Text, out exposure))
            {
                StarCam.SetExposure(exposure);
                rtb_Exposure.Text = exposure.ToString();
                PrintToLog("Exposure set to " + exposure + " (ms)");
            }
        }

        /* Update star camera exposure. */
        private void rtbExposure_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                rtb_Exposure_Leave(sender, e);
        }

        /* Start continuous capture with star camera. */
        private void btn_StartGrabStarCam_Click(object sender, EventArgs e)
        {
            if (StarCam == null)
            {
                MessageBox.Show("Connect to star camera first!");
                return;
            }
            if (btn_StartGrabStarCam.Text == "Start Grab")
            {
                chb_AutoIDStarsCheck.Checked = false;
                tmr_TakePic.Enabled = true;
                btn_StartGrabStarCam.Text = "Stop Grab";
            }
            else
            {
                tmr_TakePic.Enabled = false;
                StarCamFeedbackEnabled = false;
                btn_StartGrabStarCam.Text = "Start Grab";
            }
        }

        /* Take images continuously from star camera for closed-loop feedback. */
        private StarImg TakeStarCamImgContinuous()
        {
            if (StarCam == null)
                return null;

            DateTime t0 = new DateTime();
            double[] AziAlt0 = new double[2];
            if (Mount != null)
            {
                t0 = DateTime.UtcNow;
                AziAlt0 = Mount.ReadAnglesRad();
            }

            DateTime t = DateTime.UtcNow;
            PgmImage imgBuf = StarCam.TakeINovaPic(out t);

            DateTime t1 = new DateTime();
            double[] AziAlt1 = new double[2];
            if (Mount != null)
            {
                t1 = DateTime.UtcNow;
                AziAlt1 = Mount.ReadAnglesRad();
            }

            // Interpolate mount angles
            TimeSpan dt01 = t1.Subtract(t0);
            TimeSpan dt0 = t.Subtract(t0);
            TimeSpan dt1 = t1.Subtract(t);
            double[] AziAlt = new double[2];
            AziAlt[0] = dt0.TotalSeconds / dt01.TotalSeconds * AziAlt1[0] + dt1.TotalSeconds / dt01.TotalSeconds * AziAlt0[0];
            AziAlt[1] = dt0.TotalSeconds / dt01.TotalSeconds * AziAlt1[1] + dt1.TotalSeconds / dt01.TotalSeconds * AziAlt0[1];

            if (imgBuf == null)
            {
                PrintToLogi("Failed to take picture from star camera.");
                return null;
            }

            string fn = t.ToString("yyyyMMdd_HHmmss.ff");
            imgBuf.fn = fn;
            StarImg StarCamImg = new StarImg(imgBuf, minPix, maxPix);

            for (int i = 0; i < windowsList.Count; i++)
                windowsList[i].Close();
            windowsList.Clear();
            StarCamImgDisplay disp = new StarCamImgDisplay(this, StarCamImg, LOStel_XY, StarCamFeedbackEnabled, lfw); // calls up separate display to show star camera image
            disp.Show();
            windowsList.Add(disp); // update list of open windows
            PrintToLogi("Displaying image " + fn);

            if (StarCamFeedbackEnabled == true) // automatically use feedback from image if object is bright enough
            {
                if (StarCamImg.GetBrightestStarIdx() < 0)
                {
                    PrintToLog("No star cam feedback: no stars detected.");
                    ang_LastFeedback = ang_LastFeedback * 2;
                }
                else
                {
                    if (StarCamImg.GetBrightestStarMagSum() > starCamFeedbackMinMag)
                    {
                        dqToBrightest(StarCamImg);
                        runEKF(t, AziAlt, StarCamImg.GetBrightestStarVector());
                    }
                    else
                    {
                        PrintToLog("No star cam feedback: object not bright enough.");
                        ang_LastFeedback = ang_LastFeedback * 2;
                    }
                }
            }

            if (SavePicsCheck.Checked)
            {
                FileStream ifs = new FileStream(fn + ".aa", FileMode.Create);
                BinaryWriter bw = new BinaryWriter(ifs);
                double ut = HSYTime.DateTime_to_UnixTime(t);
                bw.Write(AziAlt1[0]);
                bw.Write(AziAlt1[1]);
                bw.Write(ut);
                bw.Close();
                imgBuf.SaveAsFile();
            }
            return StarCamImg;
        }

        /* Initiate continuous feedback on captured images. */
        private void btn_StarCamFeedback_Click(object sender, EventArgs e)
        {
            if (btn_StarCamFeedback.Text == "Start Cam Feedback")
            {
                if (tmr_TakePic.Enabled == false)
                {
                    MessageBox.Show("Must start grab with star camera first!");
                    return;
                }
                btn_StarCamFeedback.Text = "Stop Cam Feedback";
                StarCamFeedbackEnabled = true;
            }
            else
            {
                btn_StarCamFeedback.Text = "Start Cam Feedback";
                StarCamFeedbackEnabled = false;
                if (sw_EKFLog != null)
                {
                    sw_EKFLog.Close();
                    sw_EKFLog = null;
                }
            }
        }

/*TRACKING CAMERA******************************************************************************/
        /* Initialize SU320CSX IR tracking camera. */
        private void btn_TrackCamInit_Click(object sender, EventArgs e)
        {
            byte[] msgb = new byte[200]; // Error message buffer
            int status = SU320CSX_Control.initSU320CSX(msgb);
            if (status < 0)
            {
                PrintToLog("Failed to initialize IR camera: " + System.Text.Encoding.UTF8.GetString(msgb, 0, msgb.Length).TrimEnd('\0'));
                flag_trackCamInit = false;
            }
            else
            {
                PrintToLog("Initialization success: IR camera.");
                flag_trackCamInit = true;
                btn_TakeTrackImg.Enabled = true;
                btn_StartTrackGrab.Enabled = true;
                btn_TrackCamInit.Enabled = false;
            }
        }

        /* Take an image with SU320CSX tracking camera. */
        private void btn_TakeTrackImg_Click(object sender, EventArgs e)
        {
            if (!flag_trackCamInit)
                return;

            byte[] msgb = new byte[200]; // Error message buffer
            int status = SU320CSX_Control.takeImg(msgb, imgBufGrab, centroid);

            if (status < 0)
                PrintToLog("Image request failed.");

            updateTrackCamDisplay(imgBufGrab, centroid);
        }

        /* Update main form display of SU320CSX images. */
        private void updateTrackCamDisplay(byte[] imgBuf, double[] xy)
        {
            TrackCamDisplay.Height = 256;
            TrackCamDisplay.Width = 320;
            ushort[][] img = new ushort[TrackCamDisplay.Height][];

            TrackCamDispMap = new Bitmap(TrackCamDisplay.Width, TrackCamDisplay.Height);

            Graphics g = Graphics.FromImage(TrackCamDispMap);

            double ratio = 255.0 / 4095.0; // scale 12-bit image to 8-bit 
            for (int i = 0; i < TrackCamDisplay.Height; i++)
            {
                img[i] = new ushort[TrackCamDisplay.Width];
                for (int j = 0; j < TrackCamDisplay.Width; j++)
                {
                    int ipixel = 0;
                    int index = 2 * (j + i * TrackCamDisplay.Width);
                    ipixel = imgBuf[index] | imgBuf[1 + index] << 8; // convert two bytes to pixel value
                    if (SaveTrackPicsCheck.Checked)
                        img[i][j] = (ushort)ipixel;
                    ipixel = (int)(ratio * ipixel); // scale to reduced resolution
                    if (ipixel > 255)
                        ipixel = 255;
                    TrackCamDispMap.SetPixel(j, i, Color.FromArgb(255, ipixel, ipixel, ipixel)); // set pixel value
                }
            }
            TrackCamDisplay.Image = TrackCamDispMap;
            DrawPlus(g, Color.Crimson, centroid[0], centroid[1]);
            if (peakPowerCentroid[0] != 0)
                DrawPlus(g, Color.Green, peakPowerCentroid[0], peakPowerCentroid[1]);

            if (SaveTrackPicsCheck.Checked)
            {
                PgmImage pgm = new PgmImage(320, 256, 4095, img); // 4095 corresponds with 12-bit max value
                string fn = "trackCam_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                pgm.fn = fn;
                pgm.SaveAsFile();
            }
        }

        /* Close SU320CSX tracking camera. */
        private void btn_CloseTrackCam_Click(object sender, EventArgs e)
        {
            if (!flag_trackCamInit)
                return;
            SU320CSX_Control.stopGrab();
            SU320CSX_Control.closeSU320CSX();
            PrintToLog("IR camera closed.");
            flag_trackCamInit = false;
            btn_TrackCamInit.Enabled = true;
            btn_StartTrackGrab.Enabled = false;
            btn_StopTrackGrab.Enabled = false;
        }

        /* Start continuous grab with SU320CSX tracking camera. */
        private void btn_StartTrackGrab_Click(object sender, EventArgs e)
        {
            if (!flag_trackCamInit)
                return;
            btn_StartTrackGrab.Enabled = false;
            btn_StopTrackGrab.Enabled = true;
            btn_TakeTrackImg.Enabled = false;
            bgw_GrabThread.RunWorkerAsync();
            tmr_Display.Start();
            PrintToLog("Start grab.");
        }

        /* Stop continuous grab with SU320CSX tracking camera. */
        private void btn_StopTrackGrab_Click(object sender, EventArgs e)
        {
            if (!flag_trackCamInit)
                return;
            SU320CSX_Control.stopGrab();
            tmr_Display.Stop();
            btn_StartTrackGrab.Enabled = true;
            btn_StopTrackGrab.Enabled = false;
            btn_TakeTrackImg.Enabled = true;
            PrintToLog("Stop grab.");
        }

        /* Starts continuous grab with SU320CSX on a separate thread in background. */
        private void bgw_GrabThread_DoWork(object sender, DoWorkEventArgs e)
        {
            byte[] msgb = new byte[200]; // Error message buffer
            int status = SU320CSX_Control.startGrab(msgb, imgBufGrab, centroid);

            if (status < 0)
                PrintToLog("Start grab request failed.");
            PrintToLog("Start grab.");
        }

        /* Update track camera display when timer goes off. */
        private void tmr_Display_Tick(object sender, EventArgs e)
        {
            updateTrackCamDisplay(imgBufGrab, centroid);
        }

/*FSM******************************************************************************************/
        /* Initialize FSM. */
        private void btn_InitFSM_Click(object sender, EventArgs e)
        {
            byte[] msgb = new byte[200]; // Error message buffer
            //int status = OIM_Control.initFT232H_SPI(msgb);
            int status = OIM_Control_FT4222.initFT4222H_SPI(msgb);
            if (status < 0)
            {
                PrintToLog("FSM initialization failure.");
                return;
            }
            FSMVxy[0] = 0;
            FSMVxy[1] = 0;
            //OIM_Control.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
            OIM_Control_FT4222.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
            PrintToLog("Initialization success: FSM.");
            btn_CloseFSM.Enabled = true;
            btn_FSMsetV.Enabled = true;
            btn_FSMtoOrigin.Enabled = true;
            btn_FindPmax.Enabled = true;
            btn_CalGainFSMCam.Enabled = true;
            btn_CalGainFSMMnt.Enabled = true;

            // Load gain matrix between camera and FSM if it exists.
            string fn = "FSMCamGain.aa";
            FileInfo info = new FileInfo(fn);
            if (info.Exists == false)
                return;
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            FSMCamGain = new DenseMatrix(2, 2);
            try
            {
                FSMCamGain[0, 0] = br.ReadDouble();
                FSMCamGain[0, 1] = br.ReadDouble();
                FSMCamGain[1, 0] = br.ReadDouble();
                FSMCamGain[1, 1] = br.ReadDouble();
            }
            catch (EndOfStreamException ex)
            {
                Console.WriteLine("Could not read FSMCamGain.aa file: {0}.",
                    ex.GetType().Name);
            }
            br.Close();
        }

        /* Close FSM. */
        private void btn_CloseFSM_Click(object sender, EventArgs e)
        {
            FSMVxy[0] = 0;
            FSMVxy[1] = 0;
            OIM_Control_FT4222.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
            //int status = OIM_Control.closeFT232H_SPI();
            int status = OIM_Control_FT4222.closeFT4222H_SPI();
            PrintToLog("FSM connection closed.");
            btn_FSMsetV.Enabled = false;
            btn_CloseFSM.Enabled = false;
            btn_FSMtoOrigin.Enabled = false;
        }

        /* Set voltage of FSM. */
        private void btn_FSMsetV_Click(object sender, EventArgs e)
        {
            double Vx, Vy;
            if (Double.TryParse(rtb_Vx.Text, out Vx))
            {
                if (Double.TryParse(rtb_Vy.Text, out Vy))
                {
                    FSMVxy[0] = Vx;
                    FSMVxy[1] = Vy;
                    int status = OIM_Control_FT4222.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
                }
            }
        }

        /* Set FSM voltage to (0,0). */
        private void btn_FSMtoOrigin_Click(object sender, EventArgs e)
        {
            tmr_CLFSM.Enabled = false;
            FSMVxy[0] = 0;
            FSMVxy[1] = 0;
            int status = OIM_Control_FT4222.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
            rtb_Vx.Text = "0";
            rtb_Vy.Text = "0";
        }

        /* Initiates closed-loop tracking with FSM and tracking camera. */
        private void btn_CLFSMTrack_Click(object sender, EventArgs e)
        {
            if (!flag_trackCamInit)
            {
                MessageBox.Show("Initialize tracking camera first!");
                return;
            }
            if (btn_CLFSMTrack.Text == "Start CL Track")
            {
                string fn_FSMtrackLog = "FSM_CLtrack_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
                if (sw_FSMtrackLog != null)
                {
                    sw_FSMtrackLog.Close();
                    sw_FSMtrackLog = null;
                }
                sw_FSMtrackLog = new StreamWriter(fn_FSMtrackLog);
                PrintToLog("FSM closed loop tracking initiated."); FSMtrackMode = 0;
                tmr_CLFSM.Enabled = true;
                btn_CLFSMTrack.Text = "Stop CL Track";
            }
            else
            {
                if (sw_FSMtrackLog != null)
                {
                    sw_FSMtrackLog.Close();
                    sw_FSMtrackLog = null;
                }
                tmr_CLFSM.Enabled = false;
                btn_CLFSMTrack.Text = "Start CL Track";
            }
        }

        /* Print data to log during FSM CL track. */
        private void PrintFSMtrackLog(DateTime t, double dx, double dy, double[] FSMXY)
        {
            double tunix = HSYTime.DateTime_to_UnixTime(t);
            string str = tunix.ToString() + ", ";
            str += dx.ToString() + ", " + dy.ToString() + ", " + FSMXY[0].ToString() + ", " + FSMXY[1].ToString();
            sw_FSMtrackLog.WriteLine(str);
        }

        /* Calibrate gain matrix between FSM voltages and centroid displacement. */
        private void btn_CalGainFSMCam_Click(object sender, EventArgs e)
        {
            calibrateGainFSMCamOrigin();
        }

        /* Set FSM to a set of voltages, measure corresponding centroid, and use
        least squares to determine gain matrix between FSM voltage and centroid
        location. */
        private void calibrateGainFSMCamOrigin()
        {
            DenseVector y = new DenseVector(10);
            DenseMatrix H = new DenseMatrix(10, 4);
            double stepV = 1;

            y[0] = 0;
            y[1] = 0;
            OIM_Control_FT4222.setOIM_Vxy(0, 0);
            Thread.Sleep(500); // wait for motion to settle
            double x_c = centroid[0];
            double y_c = centroid[1];
            H[0, 0] = 0;
            H[0, 1] = 0;
            H[1, 2] = H[0, 0];
            H[1, 3] = H[0, 1];

            y[2] = -stepV;
            y[3] = -stepV;
            OIM_Control_FT4222.setOIM_Vxy(y[2], y[3]);
            Thread.Sleep(500); // wait for motion to settle
            H[2, 0] = centroid[0] - x_c;
            H[2, 1] = centroid[1] - y_c;
            H[3, 2] = H[2, 0];
            H[3, 3] = H[2, 1];

            y[4] = stepV;
            y[5] = -stepV;
            OIM_Control_FT4222.setOIM_Vxy(y[4], y[5]);
            Thread.Sleep(500); // wait for motion to settle
            H[4, 0] = centroid[0] - x_c;
            H[4, 1] = centroid[1] - y_c;
            H[5, 2] = H[4, 0];
            H[5, 3] = H[4, 1];

            y[6] = -stepV;
            y[7] = stepV;
            OIM_Control_FT4222.setOIM_Vxy(y[6], y[7]);
            Thread.Sleep(500); // wait for motion to settle
            H[6, 0] = centroid[0] - x_c;
            H[6, 1] = centroid[1] - y_c;
            H[7, 2] = H[6, 0];
            H[7, 3] = H[6, 1];

            y[8] = stepV;
            y[9] = stepV;
            OIM_Control_FT4222.setOIM_Vxy(y[8], y[9]);
            Thread.Sleep(500); // wait for motion to settle
            H[8, 0] = centroid[0] - x_c;
            H[8, 1] = centroid[1] - y_c;
            H[9, 2] = H[8, 0];
            H[9, 3] = H[8, 1];

            DenseVector gainV = (DenseVector)(H.TransposeThisAndMultiply(H).Inverse() * H.TransposeThisAndMultiply(y));
            FSMCamGain = new DenseMatrix(2, 2);
            FSMCamGain[0, 0] = gainV[0];
            FSMCamGain[0, 1] = gainV[1];
            FSMCamGain[1, 0] = gainV[2];
            FSMCamGain[1, 1] = gainV[3];

            FileStream ifs = new FileStream("FSMCamGain.aa", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(ifs);
            bw.Write(FSMCamGain[0, 0]);
            bw.Write(FSMCamGain[0, 1]);
            bw.Write(FSMCamGain[1, 0]);
            bw.Write(FSMCamGain[1, 1]);
            bw.Close();
        }

/*OSCILLOSCOPE*********************************************************************************/
        /* Initiate background thread to use DSO1004A oscilloscope to locate peak power and store 
        the corresponding location on tracking camera centroid. */
        private void btn_FindPmax_Click(object sender, EventArgs e)
        {
            bgw_RxMeasPwr.RunWorkerAsync();
        }

        /* Run peak power measurement procedure in background. Store peak power centroid in
        top level peakPowerCentroid variable. */
        private void bgw_RxMeasPwr_DoWork(object sender, DoWorkEventArgs e)
        {
            pwrMeas = new RxMeasPwr(centroid);
            List<RxMeasPwr.SingleMeas> pwrMeasList = pwrMeas.getMeasList();
            int iMax = pwrMeas.getMaxPindex();
            PrintToLogi("Power measurements:");
            for (int i = 0; i < pwrMeasList.Count; i++)
            {
                string s1 = string.Format("{0:N2}", pwrMeasList[i].FSM_Vx);
                string s2 = string.Format("{0:N2}", pwrMeasList[i].FSM_Vy);
                string s3 = string.Format("{0:N2}", pwrMeasList[i].APD_V);
                if (i == iMax)
                    PrintToLogi(s1 + ",     " + s2 + ",     " + s3 + "  <--MAX");
                else
                    PrintToLogi(s1 + ",     " + s2 + ",     " + s3);
            }

            FSMVxy[0] = pwrMeasList[iMax].FSM_Vx;
            FSMVxy[1] = pwrMeasList[iMax].FSM_Vy;
            int status = OIM_Control_FT4222.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
            Thread.Sleep(500);
            peakPowerCentroid[0] = centroid[0];
            peakPowerCentroid[1] = centroid[1];
        }

/*INCLINOMETER*********************************************************************************/
        /* Connect to inclinometer. */
        private void btn_ConnectInclinometer_Click(object sender, EventArgs e)
        {
            if (cb_SelectInclinometerPort.SelectedIndex > -1)
            {
                string portName = cb_SelectInclinometerPort.SelectedItem.ToString();
                if (sp_Mount.PortName == portName)
                {
                    MessageBox.Show("Port already in use");
                    return;
                }
                sp_Inclinometer.PortName = portName;
                if (!sp_Inclinometer.IsOpen)
                {
                    sp_Inclinometer.BaudRate = 9600;
                    sp_Inclinometer.Open();
                    PrintToLog(String.Format("Inclinometer connected on port '{0}'", portName));
                    Inclin = new Inclinometer(sp_Inclinometer);
                }
                else
                {
                    MessageBox.Show("Port already open");
                }
            }
            else
            {
                MessageBox.Show("Please select a port first");
            }
        }
        
/*MOUNT CALIBRATION & TRACKING*****************************************************************/
        /* Initiate autocalibration procedure. Takes ~15 minutes to execute. */
        private void btn_AutoCalPointingModel_Click(object sender, EventArgs e)
        {
            if (!cb_CalibrateFromExisting.Checked)
            {
                if (StarCam == null)
                {
                    MessageBox.Show("Connect to star camera!");
                    return;
                }
                if (Mount == null)
                {
                    MessageBox.Show("Connect to mount!");
                    return;
                }
                MntTrackMode = 1;
                RunCalSchedule();
            }
            else
            {
                TelPntModel.PerformCal();
                double CalRMS = TelPntModel.GetCalRMS();
                PrintToLog("Telescope pointing calibration RMSE: " + CalRMS + " arcseconds.");
                string fn = DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".acal";
                TelPntModel.WriteToFile(fn);
            }
        }

        /* Load existing calibration file from UI. */
        private void btn_LoadPointingModel_Click(object sender, EventArgs e)
        {
            string fn = GetCalFileNameFromUser();
            if (fn == null)
                return;
            TelPntModel.LoadFromFile(fn);
            PrintToLog("Loaded calibration file: " + fn);
        }

        /* UI dialog to select calibration file. */
        private string GetCalFileNameFromUser()
        {
            OpenFileDialog openFileUI = new OpenFileDialog();
            openFileUI.Filter = "Align Cal|*.acal|All File|*.*";
            openFileUI.FilterIndex = 0;
            openFileUI.RestoreDirectory = true;
            openFileUI.Multiselect = false;

            try
            {
                if (openFileUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fns = openFileUI.FileName;
                    return fns;
                }
                return null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("File Open Error: " + ex.Message);
            }
            return null;
        }

        /* Steps through each (Azi, Alt) point in autocalibration procedure, takes an image, and removes the point. */
        private void RunCalSchedule()
        {
            if (aziSched.Count == 0)
            {
                Mount.SendAngleCommand(0, 0);
                TelPntModel.PerformCal();
                if (!TelPntModel.IsCalibrated())
                {
                    MessageBox.Show("Need at least 2 star camera measurements!");
                    return;
                }
                double CalRMS = TelPntModel.GetCalRMS();
                PrintToLog("Telescope pointing calibration RMSE: " + CalRMS + " arcseconds.");
                string fn = DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".acal";
                TelPntModel.WriteToFile(fn);
                return;
            }
            AziAltRef[0] = aziSched[0];
            AziAltRef[1] = altSched[0];
            Mount.SendAngleCommand(HSYMath.DTR * AziAltRef[0], HSYMath.DTR * AziAltRef[1]);
            aziSched.RemoveAt(0);
            altSched.RemoveAt(0);
            MntTrackMode = 1;
            tmr_CLMount.Enabled = true;
        }

        /* Clear pointing model calibration measurements. */
        private void btn_ClearCalMeas_Click(object sender, EventArgs e)
        {
            List<TelCalMeasurement> meas = TelPntModel.GetCalMeasList();
            int cnt = meas.Count;
            for (int i = 0; i < cnt; i++)
                TelPntModel.removeMeas(0);
            Updatedgv_CalMeas();
        }

        /* Remove a calibration measurement if it is double clicked. */
        private void dgv_CalMeas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int n = e.RowIndex;
            TelPntModel.removeMeas(n);
            Updatedgv_CalMeas();
        }

        // NEEDS TO BE FIXED
        private void btn_CalGainFSMMnt_Click(object sender, EventArgs e)
        {
            if (Mount == null)
            {
                MessageBox.Show("Connect to mount first!");
                return;
            }
            calibrateGainMntFSM();
        }

        // NEEDS TO BE FIXED
        private void calibrateGainMntFSM()
        {
            DenseVector y = new DenseVector(10);
            DenseMatrix H = new DenseMatrix(10, 4);
            double[] MntAziAlt0 = Mount.ReadAnglesDeg();

            double x_c = centroid[0];
            double y_c = centroid[1];
            y[0] = 0;
            y[1] = 0;
            //H[0, 0] = FSMVxy[0];
            //H[0, 1] = FSMVxy[1];
            H[0, 0] = 0;
            H[0, 1] = 0;
            H[1, 2] = H[0, 0];
            H[1, 3] = H[0, 1];

            Mount.SendSlewCommand(50, 50);
            //AziAltRef[0] = MntAziAlt0[0] + 0.01;
            //AziAltRef[1] = MntAziAlt0[1] + 0.01;
            //Mount.SendAngleCommand(HSYMath.DTR * AziAltRef[0], HSYMath.DTR * AziAltRef[1]);
            Thread.Sleep(2000);
            Mount.SendSlewCommand(0, 0);
            Thread.Sleep(500);
            double[] MntAziAlt = Mount.ReadAnglesDeg();
            y[2] = MntAziAlt[0] - MntAziAlt0[0];
            y[3] = MntAziAlt[1] - MntAziAlt0[1];
            H[2, 0] = centroid[0] - x_c;
            H[2, 1] = centroid[1] - y_c;
            //H[2, 0] = FSMVxy[0];
            //H[2, 1] = FSMVxy[1];
            H[3, 2] = H[2, 0];
            H[3, 3] = H[2, 1];
            Mount.SendSlewCommand(-50, -50);
            Thread.Sleep(2000);
            Mount.SendSlewCommand(0, 0);

            Mount.SendSlewCommand(-50, 50);
            Thread.Sleep(2000);
            Mount.SendSlewCommand(0, 0);
            Thread.Sleep(500);
            MntAziAlt = Mount.ReadAnglesDeg();
            y[4] = MntAziAlt[0] - MntAziAlt0[0];
            y[5] = MntAziAlt[1] - MntAziAlt0[1];
            H[4, 0] = centroid[0] - x_c; //FSMVxy[0];
            H[4, 1] = centroid[1] - y_c; //FSMVxy[1];
            H[5, 2] = H[4, 0];
            H[5, 3] = H[4, 1];
            Mount.SendSlewCommand(50, -50);
            Thread.Sleep(2000);
            Mount.SendSlewCommand(0, 0);
            Thread.Sleep(500);

            Mount.SendSlewCommand(-50, -50);
            Thread.Sleep(2000);
            Mount.SendSlewCommand(0, 0);
            Thread.Sleep(500);
            MntAziAlt = Mount.ReadAnglesDeg();
            y[6] = MntAziAlt[0] - MntAziAlt0[0];
            y[7] = MntAziAlt[1] - MntAziAlt0[1];
            H[6, 0] = centroid[0] - x_c; //FSMVxy[0];
            H[6, 1] = centroid[1] - y_c; //FSMVxy[1];
            H[7, 2] = H[6, 0];
            H[7, 3] = H[6, 1];
            Mount.SendSlewCommand(50, 50);
            Thread.Sleep(2000);
            Mount.SendSlewCommand(0, 0);
            Thread.Sleep(500);

            Mount.SendSlewCommand(50, -50);
            Thread.Sleep(2000);
            Mount.SendSlewCommand(0, 0);
            Thread.Sleep(200);
            MntAziAlt = Mount.ReadAnglesDeg();
            y[8] = MntAziAlt[0] - MntAziAlt0[0];
            y[9] = MntAziAlt[1] - MntAziAlt0[1];
            H[8, 0] = centroid[0] - x_c; //FSMVxy[0];
            H[8, 1] = centroid[1] - y_c; //FSMVxy[1];
            H[9, 2] = H[8, 0];
            H[9, 3] = H[8, 1];
            Mount.SendSlewCommand(-50, 50);
            Thread.Sleep(2000);
            Mount.SendSlewCommand(0, 0);
            Thread.Sleep(500);

            DenseVector gainV = (DenseVector)(H.TransposeThisAndMultiply(H).Inverse() * H.TransposeThisAndMultiply(y));
            MntFSMGain = new DenseMatrix(2, 2);
            MntFSMGain[0, 0] = gainV[0];
            MntFSMGain[0, 1] = gainV[1];
            MntFSMGain[1, 0] = gainV[2];
            MntFSMGain[1, 1] = gainV[3];

            FileStream ifs = new FileStream("MntFSMGain.aa", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(ifs);
            bw.Write(MntFSMGain[0, 0]);
            bw.Write(MntFSMGain[0, 1]);
            bw.Write(MntFSMGain[1, 0]);
            bw.Write(MntFSMGain[1, 1]);
            bw.Close();
        }

        /* Find stars currently visible. */
        private void btn_FindStars_Click(object sender, EventArgs e)
        {
            List<StarCatalog.OneStarData> brightStars = FindStars();
        }

        /* Pull bright stars from catalog and determine if they are above the horizon,
        sort by magnitude, and display in DataGridView. */ 
        private List<StarCatalog.OneStarData> FindStars()
        {
            // Pull very bright stars from catalog
            List<StarCatalog.OneStarData> tempCat = new List<StarCatalog.OneStarData>();
            for (int i = 0; i < StarCatalog1.masterCatStars.Length; i++)
                if (StarCatalog1.masterCatStars[i].magRef < 3)
                    tempCat.Add(StarCatalog1.masterCatStars[i]);

            DateTime t = DateTime.UtcNow;

            // Determine zenith
            Quaternion q_ENU_J2K = TelPointingModel.Cal_q_ENU_J2K(t);
            DenseMatrix DCM_ENU_J2K = HSYMath.quaternion_to_DCM(q_ENU_J2K);
            DenseVector zen_ENU = new DenseVector(3);
            zen_ENU[2] = 1;
            DenseVector zen_J2K = (DenseMatrix)DCM_ENU_J2K.Transpose() * zen_ENU;
            double r_max = 2 * Math.Sin(HSYMath.DTR * 75 * 0.5);

            for (int i = (tempCat.Count - 1); i >= 0; i--)
            {
                DenseVector v = tempCat[i].XYZj2k - zen_J2K;
                double r = v.L2Norm();

                if (r > r_max)
                    tempCat.RemoveAt(i);
            }

            // Sort by magnitude
            brightStars = new List<StarCatalog.OneStarData>();
            double[] mag = new double[tempCat.Count];

            for (int i = 0; i < tempCat.Count; i++)
                mag[i] = tempCat[i].magRef;
            int[] indexSrt = HSYMath.sort_merge(mag);
            for (int i = 0; i < tempCat.Count; i++)
                brightStars.Add(tempCat[indexSrt[i]]);

            // Update display
            dgv_BrightStars.Rows.Clear();
            for (int i = 0; i < brightStars.Count; i++)
            {
                string[] row = new string[dgv_BrightStars.ColumnCount];
                int n = 0;
                StarCatalog.OneStarData star = brightStars[i];
                string name = CheckKnownStars(star);
                row[n++] = name;
                row[n++] = star.magRef.ToString();

                // calc azi, el
                DenseVector XYZ_ENU = DCM_ENU_J2K * star.XYZj2k;
                double azi = Math.Atan2(XYZ_ENU[0], XYZ_ENU[1]) * HSYMath.RTD;
                double el = Math.Asin(XYZ_ENU[2]) * HSYMath.RTD;

                row[n++] = azi.ToString();
                row[n++] = el.ToString();
                row[n++] = (star.RA * HSYMath.RTD).ToString();
                row[n++] = (star.DEC * HSYMath.RTD).ToString();
                dgv_BrightStars.Rows.Add(row);
            }
            return brightStars;
        }

        /* Label known stars with names, particularly for bright stars in IR. */
        private string CheckKnownStars(StarCatalog.OneStarData star)
        {
            string name = "";
            if (Math.Abs(HSYMath.RTD * star.RA - 213.915300292) < 0.01)
            {
                if (Math.Abs(HSYMath.RTD * star.DEC - 19.1824091667) < 0.01)
                    name = "Arcturus";
            }
            else if (Math.Abs(HSYMath.RTD * star.RA - 88.792939) < 0.01)
            {
                if (Math.Abs(HSYMath.RTD * star.DEC - 7.407064) < 0.01)
                    name = "Betelgeuse";
            }
            return name;
        }

        /* Start tracking a star if it is double clicked. */
        private void dgv_BrightStars_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int n = e.RowIndex;
            if (!TelPntModel.IsCalibrated())
            {
                MessageBox.Show("Must calibrate alignment first!");
                return;
            }
            starToTrack = brightStars[n];

            PrintToLog("Tracking star " + starToTrack.ID_SKYMAP2000.ToString() + ".");
            PrintToLog("RA: " + starToTrack.RA.ToString() + ", DEC: " + starToTrack.DEC.ToString());
            MntTrackMode = 2;
            tmr_CLMount.Enabled = true;
        }

        /* Update offset quaternion (top level dq_ST) using brightest star from star
        camera image. */
        public void dqToBrightest(StarImg StarCamImg)
        {
            if (StarCamImg.GetBrightestStarIdx() < 0)
            {
                PrintToLog("No star cam feedback: no stars identified.");
                return;
            }

            DenseVector XYZbrightest_ST = StarCamImg.GetBrightestStarVector();
            DenseVector axis = HSYMath.vector_3x1_cross(XYZbrightest_ST, LOStel_ST);
            double axisLen = axis.L2Norm();
            double ang = Math.Asin(axisLen);

            // No feedback if centroid is within 20 arcseconds.
            if (ang * HSYMath.RTAS < 20)
            {
                PrintToLog("No star cam feedback: brightest star distance " + (ang * HSYMath.RTAS).ToString("F3") + " arcsec.");
                return;
            }

            // No feedback if feedback angle is 5x last feedback (probably wrong object).
            if (ang > 5 * ang_LastFeedback)
            {
                PrintToLog("No star cam feedback: inconsistent location.");
                return;
            }
            axis = axis / axisLen;
            double s = Math.Sin(ang * 0.5);
            Quaternion ddq = new Quaternion();
            ddq[0] = axis[0] * s;
            ddq[1] = axis[1] * s;
            ddq[2] = axis[2] * s;
            ddq[3] = Math.Cos(ang * 0.5);
            dq_ST = ddq * dq_ST;
            PrintToLog("Feedback from brightest star: correction of " + (ang * HSYMath.RTAS).ToString("F3") + " arcsec.");

            ang_LastFeedback = ang;

            double dq_ang = HSYMath.RTD * Math.Acos(dq_ST[3]) * 2.0;
            if (dq_ang > 10) // If correction becomes too large (wrong object) clear the correction
            {
                dq_ST[0] = 0;
                dq_ST[1] = 0;
                dq_ST[2] = 0;
                dq_ST[3] = 1;
                ang_LastFeedback = 100; // reset to very high
                PrintToLog("dq cleared because correction exceeded 10 deg.");
            }
        }

        /* Sets flag to do continuous star camera feedback. */
        public void SetStarCamFeedback(bool enabled)
        {
            if (tmr_TakePic.Enabled == false)
            {
                MessageBox.Show("Must start grab with star camera first!");
                return;
            }
            StarCamFeedbackEnabled = enabled;
        }

        // OFFLOAD FSM NEEDS TO BE CORRECTED
        private void btn_OffloadFSM_Click(object sender, EventArgs e)
        {
            if (Mount == null)
            {
                MessageBox.Show("Connect to mount first!");
                return;
            }
            if (tmr_CLFSM.Enabled == false)
            {
                MessageBox.Show("Lock onto signal with FSM first!");
                return;
            }

            if (btn_OffloadFSM.Text == "Start Offload FSM")
            {
                PrintToLog("Offload FSM started.");
                btn_OffloadFSM.Text = "Stop Offload FSM";
                if (flag_Simulation) // For testing, set reference angle to current angle and attempt to offload.
                {
                    MntTrackMode = 3;
                    AziAltDotRef[0] = 0;
                    AziAltDotRef[1] = 0;
                    double[] MntAziAlt = Mount.ReadAnglesDeg();
                    AziAltRef[0] = MntAziAlt[0];
                    AziAltRef[1] = MntAziAlt[1];
                    tmr_CLMount.Enabled = true;
                }
                FSMoffloadAziAlt[0] = 0;
                FSMoffloadAziAlt[1] = 0;
                flag_OffloadFSM = true;
            }
            else
            {
                flag_OffloadFSM = false;
                FSMoffloadAziAlt[0] = 0;
                FSMoffloadAziAlt[1] = 0;
                btn_OffloadFSM.Text = "Start Offload FSM";
            }
        }

        /* Update simulation flag. */
        private void cb_simulation_CheckedChanged(object sender, EventArgs e)
        {
            flag_Simulation = cb_simulation.Checked;
        }

        /* Clear quaternion correction angle. */
        private void btn_Cleardq_Click(object sender, EventArgs e)
        {
            dq_ST[0] = 0;
            dq_ST[1] = 0;
            dq_ST[2] = 0;
            dq_ST[3] = 1;
            ang_LastFeedback = 100; // Reset to very high
            PrintToLog("dq cleared.");
        }

        /* Load TLE file from UI. */
        private void btn_LoadTLE_Click(object sender, EventArgs e)
        {
            if (!TelPntModel.IsCalibrated())
            {
                MessageBox.Show("Must calibrate alignment first!");
                return;
            }
            string fn = GetTLEFileNameFromUser();
            if (fn == null)
            {
                PrintToLog("Unable to load selected TLE.");
                return;
            }
            HSYSGP4.Tle TLE = HSYSGP4.ReadTle(fn);
            DateTime t0 = DateTime.UtcNow;
            DateTime t0Local = DateTime.Now;

            // For simulation purposes, select ISS from 7/05/17 and tracking will start immediately.
            if (flag_Simulation)
            {
                t0 = new DateTime(2017, 7, 7, 8, 44, 0);
                t0Local = new DateTime(2017, 7, 7, 4, 44, 0);
            }

            SGP4 = new HSYSGP4.HSYSGP4EasyUsing(t0, TLE);
            double t0_unix = HSYTime.DateTime_to_UnixTime(t0);
            int thours = 24;
            double T = 3600 * thours;
            double dt = 1;

            double telapsed = 0;
            DateTime t_DT;
            DenseVector v_ECEF = new DenseVector(3);
            DenseVector v_J2K;
            DenseMatrix DCM_ECEF_J2K;
            int n = (int)(T / dt);

            int startIndex = -1;

            // Propagate TLE until object is above the horizon.
            for (int i = 0; i < n; i++)
            {
                telapsed = dt * i;
                tStart = t0_unix + telapsed;
                t_DT = t0.AddSeconds(telapsed);

                DenseVector rSat_J2K, vdot_ECEF;
                CalcRelSatVector(t_DT, out rSat_J2K, out v_J2K, out v_ECEF, out vdot_ECEF, out DCM_ECEF_J2K);


                DenseVector LOSteldq_ST = HSYMath.quaternion_to_DCM(dq_ST.InverseQ()) * LOStel_ST;
                double[] AziAlt = TelPntModel.CalcAziAltRef(t_DT, v_J2K, LOSteldq_ST);
                if ((AziAlt[1] > 5 * HSYMath.DTR) && (AziAlt[1] < 90 * HSYMath.DTR))
                {
                    startIndex = i;
                    AziAltStart[0] = AziAlt[0] * HSYMath.RTD;
                    AziAltStart[1] = AziAlt[1] * HSYMath.RTD;

                }
                if (startIndex >= 0)
                    break;
            }
            if (startIndex < 0)
            {
                PrintToLog("There are no passes for this TLE in the next " + thours.ToString() + " hours!");
                return;
            }

            btn_TrackTLE.Enabled = true;

            DateTime tStartLocal = t0Local.AddSeconds(telapsed);
            PrintToLog(String.Format("Satellite appears in {0:f2} minutes", telapsed / 60.0));
            PrintToLog("The pass starts at " + tStartLocal.ToString("yyyy-MM-dd HH:mm:ss.ff") + " local time.");
            PrintToLog(String.Format("Mount Azi: {0:f3} deg, Mount Alt: {1:f3} deg", AziAltStart[0], AziAltStart[1]));

            DenseVector v_ENU = HSYEarth.DCM_ECR2ENU(lat, lon) * v_ECEF;
            double Azi_ENU = Math.Atan2(v_ENU[0], v_ENU[1]) * HSYMath.RTD;
            double Alt_ENU = Math.Asin(v_ENU[2]) * HSYMath.RTD;
            PrintToLog(String.Format("ENU Azi: {0:f3} deg, ENU Ele: {1:f3} deg", Azi_ENU, Alt_ENU));

            // Set the start position reference
            AziAltRef[0] = AziAltStart[0];
            AziAltRef[1] = AziAltStart[1];
            AziAltDotRef[0] = 0;
            AziAltDotRef[1] = 0;
        }

        /* Opens UI dialog to get TLE. */
        private string GetTLEFileNameFromUser()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "TLE|*.tle;*.txt|All File|*.*";
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;

            try
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return ofd.FileName;
                }
                return null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("File open error: " + ex.Message);
            }
            return null;
        }

        /* Propagate TLE to given time and determine the "look vector" based on the ground station location. */
        private void CalcRelSatVector(DateTime t, out DenseVector rSat_J2K, out DenseVector v_J2K,
            out DenseVector v_ECEF, out DenseVector vdot_ECEF, out DenseMatrix DCM_ECEF_J2K)
        {
            DenseVector rdotSat_J2K, rSat_ECEF, rdotSat_ECEF;
            DenseMatrix DCMdot_ECEF_J2K;
            SGP4.PropagateOrbit(t, out rSat_J2K, out rdotSat_J2K, out rSat_ECEF, out rdotSat_ECEF, out DCM_ECEF_J2K, out DCMdot_ECEF_J2K);
            DenseVector rGS_J2K = (DenseMatrix)DCM_ECEF_J2K.Transpose() * rGS_ECEF;
            DenseVector rRel_J2K = rSat_J2K - rGS_J2K;
            double rRelNorm = rRel_J2K.L2Norm();
            double p = 1.0 / rRelNorm;
            v_J2K = rRel_J2K * p;
            v_ECEF = DCM_ECEF_J2K * v_J2K;
            vdot_ECEF = (rdotSat_ECEF - v_ECEF * rdotSat_ECEF.DotProduct(v_ECEF)) * p;
        }

        /* Initiate tracking of TLE. */
        private void btn_TrackTLE_Click(object sender, EventArgs e)
        {
            if (!flag_Simulation && Mount == null)
            {
                MessageBox.Show("Connect to mount first!");
                return;
            }
            string fn_TrackLog = "TLE_track_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
            if (sw_TrackLog != null)
            {
                sw_TrackLog.Close();
                sw_TrackLog = null;
            }
            if (sw_EKFLog != null)
            {
                sw_EKFLog.Close();
                sw_EKFLog = null;
            }
            sw_TrackLog = new StreamWriter(fn_TrackLog);

            PrintToLog("TLE track initiated.");
            PrintToLog(String.Format("Mount commanded to Azi {0:F2} deg, Alt {1:F2} deg.", AziAltStart[0], AziAltStart[1]));
            MntTrackMode = 3;
            if (!flag_Simulation)
                Mount.SendAngleCommand(AziAltStart[0] * HSYMath.DTR, AziAltStart[1] * HSYMath.DTR);
            rtb_GoToAzi.Text = AziAltStart[0].ToString("F2");
            rtb_GoToAlt.Text = AziAltStart[1].ToString("F2");
            EKF = new EKF_satellite_tracking(TelPntModel);
            ang_LastFeedback = 100; // reset to very high
            tmr_CLMount.Enabled = true;
        }

        /* Run EKF in simulation. */
        private void testEKF(DateTime t, double[] MntAziAlt, DenseVector v_ECEF, DenseVector vdot_ECEF)
        {
            Quaternion q_ST_ECEF = TelPntModel.Getq_ST_GIM() * TelPntModel.AziAltToQuat(MntAziAlt[0], MntAziAlt[1]) * TelPntModel.Getq_MNT_ENU() * TelPntModel.Getq_ENU_ECEF();
            double t_offset = 1;
            DenseVector rSat_J2K_sim, v_J2K_sim, v_ECEF_sim, vdot_ECEF_sim;
            DenseMatrix DCM_ECEF_J2K_sim;
            CalcRelSatVector(t.AddSeconds(t_offset), out rSat_J2K_sim, out v_J2K_sim, out v_ECEF_sim, out vdot_ECEF_sim, out DCM_ECEF_J2K_sim);
            Quaternion qn = new Quaternion();
            Quaternion qerr = new Quaternion();
            qerr[0] = 1e-4;
            qerr[1] = 5e-5;
            qerr[2] = -2e-5;
            qerr = qerr.Normalize();
            Random rnd = new Random();
            qn[0] = 1e-3 * (rnd.NextDouble() - 0.5);
            qn[1] = 1e-3 * (rnd.NextDouble() - 0.5);
            qn[2] = 1e-3 * (rnd.NextDouble() - 0.5);
            qn = qn.Normalize();
            DenseVector v_ST = HSYMath.quaternion_to_DCM(q_ST_ECEF) * v_ECEF_sim;
            DenseVector LOSteldq_ST1 = HSYMath.quaternion_to_DCM(dq_ST.InverseQ()) * LOStel_ST;
            runEKF(tNow, MntAziAlt, v_ST);
            tLastFback = tNow;
            DenseVector axis = HSYMath.vector_3x1_cross(v_ST, LOStel_ST);
            double axisLen = axis.L2Norm();
            double ang = Math.Asin(axisLen);

            axis = axis / axisLen;
            double s = Math.Sin(ang * 0.5);
            Quaternion ddq = new Quaternion();
            ddq[0] = axis[0] * s;
            ddq[1] = axis[1] * s;
            ddq[2] = axis[2] * s;
            ddq[3] = Math.Cos(ang * 0.5);
            dq_ST = ddq * dq_ST;
        }

        /* Log mount data during TLE tracking. */
        private void PrintTrackLog(DateTime t, double[] MntAziAlt, double[] _AziAltRef, double[] _AziAltDotCmd, double[] _AziAltDotRef)
        {
            double tunix = HSYTime.DateTime_to_UnixTime(t);
            string str = tunix.ToString() + ", ";
            str += MntAziAlt[0].ToString() + ", " + MntAziAlt[1].ToString() + ", ";
            str += _AziAltRef[0].ToString() + ", " + _AziAltRef[1].ToString() + ", ";
            str += _AziAltDotCmd[0].ToString() + ", " + _AziAltDotCmd[1].ToString() + ", ";
            str += _AziAltDotRef[0].ToString() + ", " + _AziAltDotRef[1].ToString() + ", ";
            sw_TrackLog.WriteLine(str);
            return;
        }

        /* Stop TLE tracking. */
        private void btn_StopTrackTLE_Click(object sender, EventArgs e)
        {
            if (sw_TrackLog != null)
            {
                sw_TrackLog.Close();
                sw_TrackLog = null;
            }
            if (sw_EKFLog != null)
            {
                sw_EKFLog.Close();
                sw_EKFLog = null;
            }
            tmr_CLMount.Enabled = false;
            if (sp_Mount.IsOpen)
            {
                Mount.StopGoTo();
                Mount.SendSlewCommand(0, 0);
                PrintToLog("Tracking stopped.");
            }

            dq_ST[0] = 0;
            dq_ST[1] = 0;
            dq_ST[2] = 0;
            dq_ST[3] = 1;
            PrintToLog("dq cleared.");
            ang_LastFeedback = 100; // reset to very high

            AziAltRef[0] = 0;
            AziAltRef[1] = 0;
            AziAltDotRef[0] = 0;
            AziAltDotRef[1] = 0;
        }

        /* Run extended Kalman filter during tracking. */
        private void runEKF(DateTime t, double[] AziAlt, DenseVector v_ST)
        {
            DenseVector temp = new DenseVector(3);
            double t_offset_hat = EKF.GetToffset();
            DenseVector rSat_J2K, v_J2K, v_ECEF, vdot_ECEF;
            DenseMatrix DCM_ECEF_J2K;
            CalcRelSatVector(t.AddSeconds(t_offset_hat), out rSat_J2K, out v_J2K, out v_ECEF, out vdot_ECEF, out DCM_ECEF_J2K);
            EKF.EKFupdate1term(t, AziAlt[0], AziAlt[1], v_ST, temp, v_ECEF, vdot_ECEF);

            t_offset_hat = EKF.GetToffset();
            double P1 = EKF.GetP1();

            // Log EKF updates
            double tunix = HSYTime.DateTime_to_UnixTime(t);
            string str = tunix.ToString() + ", ";
            str += AziAlt[0].ToString() + ", " + AziAlt[1].ToString() + ", ";
            str += v_ST[0].ToString() + ", " + v_ST[1].ToString() + ", " + v_ST[2].ToString() + ", ";
            str += v_ECEF[0].ToString() + ", " + v_ECEF[1].ToString() + ", " + v_ECEF[2].ToString() + ", ";
            str += vdot_ECEF[0].ToString() + ", " + vdot_ECEF[1].ToString() + ", " + vdot_ECEF[2].ToString() + ", ";
            str += dq_ST[0].ToString() + ", " + dq_ST[1].ToString() + ", " + dq_ST[2].ToString() + ", " + dq_ST[3].ToString() + ", ";
            str += t_offset_hat.ToString() + ", " + P1.ToString() + ", ";
            if (sw_EKFLog == null)
            {
                string fn_EKFLog = "EKF_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
                sw_EKFLog = new StreamWriter(fn_EKFLog);
                Quaternion q1 = TelPntModel.Getq_ST_GIM();
                Quaternion q2 = TelPntModel.Getq_MNT_ENU();
                Quaternion q3 = TelPntModel.Getq_ENU_ECEF();
                string cal = q1[0].ToString() + ", " + q1[1].ToString() + ", " + q1[2].ToString() + ", " + q1[3].ToString();
                sw_EKFLog.WriteLine(cal);
                cal = q2[0].ToString() + ", " + q2[1].ToString() + ", " + q2[2].ToString() + ", " + q2[3].ToString();
                sw_EKFLog.WriteLine(cal);
                cal = q3[0].ToString() + ", " + q3[1].ToString() + ", " + q3[2].ToString() + ", " + q3[3].ToString();
                sw_EKFLog.WriteLine(cal);
            }
            sw_EKFLog.WriteLine(str);
        }

/*TIMERS***************************************************************************************/
        /* Timer to provide closed-loop correction with FSM. */
        private void tmr_CLFSM_Tick(object sender, EventArgs e)
        {
            if (FSMtrackMode == 0) // 0 = scan
            {
                // Break if centroid is seen
                if (centroid[0] != -1)
                {
                    r = 0;
                    theta = 0;
                    FSMtrackMode = 1;
                    return;
                }

                theta = theta + thetadot * tmr_CLFSM.Interval / 1000.0;
                r = r + rdot * tmr_CLFSM.Interval / 1000.0;

                if (r > FSMmaxV || r < 0)
                    rdot = -rdot;

                FSMVxy[0] = r * Math.Cos(theta);
                FSMVxy[1] = r * Math.Sin(theta);
                if (FSMVxy[0] > FSMmaxV)
                    FSMVxy[0] = FSMmaxV;
                else if (FSMVxy[0] < -FSMmaxV)
                    FSMVxy[0] = -FSMmaxV;
                if (FSMVxy[1] > FSMmaxV)
                    FSMVxy[1] = FSMmaxV;
                else if (FSMVxy[1] < -FSMmaxV)
                    FSMVxy[1] = -FSMmaxV;
                rtb_Vx.Text = FSMVxy[0].ToString("N2");
                rtb_Vy.Text = FSMVxy[1].ToString("N2");
                int status = OIM_Control_FT4222.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
            }
            else if (FSMtrackMode == 1) // 1 = track
            {
                if (FSMCamGain == null)
                    calibrateGainFSMCamOrigin();

                // if no centroid, stop tracking and return to origin for search
                if (centroid[0] == -1)
                {
                    FSMVxy[0] = 0;
                    FSMVxy[1] = 0;
                    OIM_Control_FT4222.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
                    rtb_Vx.Text = FSMVxy[0].ToString("N2");
                    rtb_Vy.Text = FSMVxy[1].ToString("N2");
                    FSMtrackMode = 0;
                    return;
                }

                double dx = peakPowerCentroid[0] - centroid[0];
                double dy = peakPowerCentroid[1] - centroid[1];
                DenseVector dc = new DenseVector(2);
                dc[0] = dx;
                dc[1] = dy;
                PrintFSMtrackLog(DateTime.UtcNow, dx, dy, FSMVxy);
                DenseVector v = FSMCamGain * dc;
                FSMVxy[0] = FSMVxy[0] + v[0];
                FSMVxy[1] = FSMVxy[1] + v[1];
                if (FSMVxy[0] > FSMmaxV)
                    FSMVxy[0] = FSMmaxV;
                else if (FSMVxy[0] < -FSMmaxV)
                    FSMVxy[0] = -FSMmaxV;
                if (FSMVxy[1] > FSMmaxV)
                    FSMVxy[1] = FSMmaxV;
                else if (FSMVxy[1] < -FSMmaxV)
                    FSMVxy[1] = -FSMmaxV;
                rtb_Vx.Text = FSMVxy[0].ToString("N2");
                rtb_Vy.Text = FSMVxy[1].ToString("N2");
                int status = OIM_Control_FT4222.setOIM_Vxy(FSMVxy[0], FSMVxy[1]);
            }
        }

        /* Timer for closed-loop tracking with mount. */
        DateTime tNow = new DateTime(2017, 7, 7, 8, 44, 0); //used for testing
        DateTime tLastFback = new DateTime(2017, 7, 7, 8, 44, 0); //used for testing
        private void tmr_CLMount_Tick(object sender, EventArgs e)
        {
            // Read mount angles
            double[] MntAziAlt = new double[2];
            if (!flag_Simulation)
                MntAziAlt = Mount.ReadAnglesDeg();
            else
            {
                MntAziAlt[0] = AziAltRef[0] + AziAltDotCmd[0] * 0.1 / 3600.0;
                MntAziAlt[1] = AziAltRef[1] + AziAltDotCmd[1] * 0.1 / 3600.0;
            }
            if (MntAziAlt != null)
            {
                AziRead.Text = MntAziAlt[0].ToString("N2");
                AltRead.Text = MntAziAlt[1].ToString("N2");
            }

            // Slew command parameters
            double[] AziAltDotCmd_prev = new double[2];
            AziAltDotCmd_prev[0] = AziAltDotCmd[0];
            AziAltDotCmd_prev[1] = AziAltDotCmd[1];
            double Ts = 0.3; // controller gain
            double maxSlew = 3.4 * 3600; // 3.4 deg/s is telescope limitation

            if (!flag_Simulation)
                tNow = DateTime.UtcNow;
            else
                tNow = tNow.AddMilliseconds(100);

            // If tracking a star, update reference angles
            if (MntTrackMode == 2)
            {
                double dt = 0.1;
                DateTime t = DateTime.UtcNow.AddSeconds(dt);
                Quaternion q_ECEF_J2K = TelPointingModel.Cal_q_ECEF_J2K(t);
                DenseVector v_J2K = starToTrack.XYZj2k;
                v_J2K = v_J2K / v_J2K.L2Norm();
                DenseVector LOSteldq_ST = HSYMath.quaternion_to_DCM(dq_ST.InverseQ()) * LOStel_ST;
                double[] AziAltRefRad = TelPntModel.CalcAziAltRef(t, v_J2K, LOSteldq_ST);
                AziAltRef[0] = AziAltRefRad[0] * HSYMath.RTD;
                AziAltRef[1] = AziAltRefRad[1] * HSYMath.RTD;
            }

            // If tracking a TLE, update reference angles and rates
            if (MntTrackMode == 3)
            {
                double tunix = HSYTime.DateTime_to_UnixTime(tNow);
                if (tunix < tStart) // Wait for pass to start
                {
                    double tToStart = tStart - tunix;
                    lbl_PassTime.Text = String.Format("Countdown to pass: {0:F2} sec", tToStart);
                    return;
                }

                double tElapsed = tunix - tStart;
                lbl_PassTime.Text = String.Format("Time elapsed: {0:F2} sec", tElapsed);


                double t_offset_hat = 0;
                if (flag_Simulation)
                    t_offset_hat = EKF.GetToffset();
                DenseVector rSat_J2K, v_J2K, v_ECEF, vdot_ECEF;
                DenseMatrix DCM_ECEF_J2K;
                CalcRelSatVector(tNow.AddSeconds(t_offset_hat), out rSat_J2K, out v_J2K, out v_ECEF, out vdot_ECEF, out DCM_ECEF_J2K);

                // code to test EKF ******
                if (flag_Simulation && tNow.Subtract(tLastFback).TotalSeconds > 2)
                {
                    double[] MntAziAltRad = new double[2];
                    MntAziAltRad[0] = HSYMath.DTR * MntAziAlt[0];
                    MntAziAltRad[1] = HSYMath.DTR * MntAziAlt[1];
                    testEKF(tNow, MntAziAltRad, v_ECEF, vdot_ECEF);
                    t_offset_hat = EKF.GetToffset();
                }
                //************************

                DenseVector LOSteldq_ST = HSYMath.quaternion_to_DCM(dq_ST.InverseQ()) * LOStel_ST;
                double[] AziAltRefRad = TelPntModel.CalcAziAltRef(tNow, v_J2K, LOSteldq_ST);
                double[] AziAltRefRad2 = TelPntModel.CalcAziAltRef9term(tNow, v_J2K, LOSteldq_ST);
                double[] AziAltDotRefRad = TelPntModel.CalcAziAltDotRef(AziAltRefRad[0], v_ECEF, vdot_ECEF);
                double[] AziAltDotRefRad2 = TelPntModel.CalcAziAltDotRef9term(AziAltRefRad[0], v_ECEF, vdot_ECEF);

                // Convert radians to degrees
                AziAltRef[0] = AziAltRefRad[0] * HSYMath.RTD;
                AziAltRef[1] = AziAltRefRad[1] * HSYMath.RTD;
                AziAltDotRef[0] = AziAltDotRefRad[0] * HSYMath.RTD;
                AziAltDotRef[1] = AziAltDotRefRad[1] * HSYMath.RTD;
            }

            // Resolve angle ambiguity
            if ((MntAziAlt[0] - AziAltRef[0]) < -180.0)
                MntAziAlt[0] += 360;
            if ((MntAziAlt[0] - AziAltRef[0]) > 180.0)
                MntAziAlt[0] -= 360;
            if ((MntAziAlt[1] - AziAltRef[1]) < -180.0)
                MntAziAlt[1] += 360;
            if ((MntAziAlt[1] - AziAltRef[1]) > 180.0)
                MntAziAlt[1] -= 360;

            if (MntTrackMode == 2 || MntTrackMode == 3) // Generate rate commands
            {
                // OFFLOAD CODE NEEDS TO BE FIXED
                // Calculate FSM offload
                /*if (flag_OffloadFSM)
                {
                    if (centroid[0] != -1) // If there's a centroid, offload
                    {
                        DenseVector dAziAltOffload = -MntFSMGain * FSMVxy; // Drive mirror to (0,0)
                        FSMoffloadAziAlt[0] = FSMoffloadAziAlt[0] + dAziAltOffload[0];
                        FSMoffloadAziAlt[1] = FSMoffloadAziAlt[1] + dAziAltOffload[1];
                    }
                }*/

                // Calculate desired rates
                AziAltDotCmd[0] = HSYMath.DTR * HSYMath.RTAS * (AziAltDotRef[0] + 1.0 / Ts * (AziAltRef[0] - MntAziAlt[0] + FSMoffloadAziAlt[0]));
                AziAltDotCmd[1] = HSYMath.DTR * HSYMath.RTAS * (AziAltDotRef[1] + 1.0 / Ts * (AziAltRef[1] - MntAziAlt[1] + FSMoffloadAziAlt[1]));

                // Saturate command at max slew rate
                if (AziAltDotCmd[0] > maxSlew)
                    AziAltDotCmd[0] = maxSlew;
                else if (AziAltDotCmd[0] < -maxSlew)
                    AziAltDotCmd[0] = -maxSlew;
                if (AziAltDotCmd[1] > maxSlew)
                    AziAltDotCmd[1] = maxSlew;
                else if (AziAltDotCmd[1] < -maxSlew)
                    AziAltDotCmd[1] = -maxSlew;

                if (MntTrackMode == 3)
                {
                    PrintTrackLog(tNow, MntAziAlt, AziAltRef, AziAltDotCmd, AziAltDotRef);
                    lbl_AziSlew.Text = String.Format("Azi slew: {0:F2} deg/s", AziAltDotCmd[0] * HSYMath.ASTR * HSYMath.RTD);
                    lbl_AltSlew.Text = String.Format("Alt slew: {0:F2} deg/s", AziAltDotCmd[1] * HSYMath.ASTR * HSYMath.RTD);
                }
            }

            switch (MntTrackMode) // 1 = Auto Cal Schedule, 2 = Track Star, 3 = Track TLE
            {
                case (1):
                    if ((Math.Abs(MntAziAlt[0] - AziAltRef[0]) + Math.Abs(MntAziAlt[1] - AziAltRef[1])) < 1e-2)
                    {
                        tmr_CLMount.Enabled = false;
                        btn_TakeStarImg_Click(null, null);
                        RunCalSchedule();
                    }
                    break;
                case (2):
                    if (AziAltRef[1] < 5) // If elevation is below 5 deg, stop tracking
                    {
                        if (!flag_Simulation)
                            Mount.SendSlewCommand(0, 0);
                        tmr_CLMount.Enabled = false;
                        return;
                    }
                    if (!flag_Simulation)
                        Mount.SendSlewCommand(AziAltDotCmd[0], AziAltDotCmd[1]);
                    break;
                case (3):
                    if (AziAltRef[1] < 5) // If elevation is below 5 deg, stop tracking
                    {
                        if (!flag_Simulation)
                            Mount.SendSlewCommand(0, 0);
                        tmr_CLMount.Enabled = false;
                        sw_TrackLog.Close();
                        sw_TrackLog = null;
                        return;
                    }
                    if (!flag_Simulation)
                        Mount.SendSlewCommand(AziAltDotCmd[0], AziAltDotCmd[1]);
                    break;
            }
        }

        /* Timer to take images from star camera. */
        private void tmr_TakePic_Tick(object sender, EventArgs e)
        {
            TakeStarCamImgContinuous();
        }
    }
}
