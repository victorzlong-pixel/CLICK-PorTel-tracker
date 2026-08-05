using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using HSYLib.CS;
using INovaSDK;


namespace GS_Tracking_KR
{
    public partial class TestCodeForm : Form // This form has lots of test functions.
    {
        private LogFileWriter LogFileWriter1 = null;
        private CelestronMount Mount = null;
        private Inclinometer Inclin = null;
        private string dataLog = null;
        private StreamWriter sw = null;
        private StarImg StarCamImg = null;
        private List<StarImg.OnePixel> pixels = null;
        private List<StarImg.OneStar> stars = null;
        private StarCatalog StarCatalog1 = null;

        public TestCodeForm(LogFileWriter lfw, CelestronMount cm, Inclinometer ic)
        {
            InitializeComponent();
            LogFileWriter1 = lfw;
            Mount = cm;
            Inclin = ic;

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
        }

        private void Send_Click(object sender, EventArgs e)
        {
            if (Mount == null)
                return;

            double AziDeg;
            double AltDeg;
            if (Double.TryParse(GoToAzi.Text, out AziDeg))
            {
                if (Double.TryParse(GoToAlt.Text, out AltDeg))
                {
                    double AziRad = AziDeg * HSYMath.DTR;
                    double AltRad = AltDeg * HSYMath.DTR;

                    Mount.SendAngleCommand(AziRad, AltRad);
                    PrintToLog("Mount commanded to Azi " + AziDeg + " (deg), Alt " + AltDeg + " (deg)");
                }
            }
        }

        private void ReadButton_Click(object sender, EventArgs e)
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

        private void SlewSendButton_Click(object sender, EventArgs e)
        {
            if (Mount == null)
                return;

            double AziDegS;
            double AltDegS;
            if (Double.TryParse(SlewAzi.Text, out AziDegS))
            {
                if (Double.TryParse(SlewAlt.Text, out AltDegS))
                {
                    double AziArcS = AziDegS * 3600;
                    double AltArcS = AltDegS * 3600;

                    Mount.SendSlewCommand(AziArcS, AltArcS);
                    PrintToLog("Mount slew rates commanded to Azi " + AziDegS + " (deg/s), Alt " + AltDegS + " (deg/s)");
                }
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            Mount.StopGoTo();
            Mount.SendSlewCommand(0, 0);
            PrintToLog("Stop command sent.");
        }

        private void PrintToLog(string text)
        {
            SessionLog.AppendText(text + "\n");
            SessionLog.ScrollToCaret();
            LogFileWriter1.WriteLogLine("Test form log: " + text);
        }

        private void LogDataButton_Click(object sender, EventArgs e) // Log mount angles continuously.
        {
            string startLog = "Start Log";
            string stopLog = "Stop Log";
            if (startLog.Equals(LogDataButton.Text, StringComparison.Ordinal))
            {
                LogDataButton.Text = stopLog;
                dataLog = "Log_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
                LogAnglesTimer.Enabled = true;
                sw = new StreamWriter(dataLog);
            }
            else
            {
                LogAnglesTimer.Enabled = false;
                LogDataButton.Text = startLog;
                if (sw != null)
                    sw.Close();
            }
        }

        private void TestCodeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogAnglesTimer.Enabled = false;
            if (sw != null)
                sw.Close();
        }

        private void LogAnglesTimer_Tick(object sender, EventArgs e)
        {
            double[] angles, angles2;
            DateTime t = DateTime.UtcNow;
            double tunix = HSYTime.DateTime_to_UnixTime(t);
            string str = tunix.ToString();
            angles = new double[2];//Mount.ReadAnglesDeg();
            angles2 = Inclin.ReadAnglesDegTemp();
            if (angles != null)
                str += ", " + angles[0].ToString() + ", " + angles[1].ToString() + ", " + angles2[0].ToString() + ", " + angles2[1].ToString() + ", " + angles2[2].ToString();
            sw.WriteLine(str);
        }

        private void OpenImgButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileUI = new OpenFileDialog();
            openFileUI.Filter = "Star Cam Image|*.pgm|All File|*.*";
            openFileUI.FilterIndex = 0;
            openFileUI.RestoreDirectory = true;

            try
            {
                if (openFileUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string fn = openFileUI.FileName;
                    PgmImage img = PgmImage.LoadImage_NotGeneral(fn);
                    StarCamImg = new StarImg(img, 9, 100);
                    DisplayImg();
                    ThresholdButton.Enabled = true;
                    StarIDButton.Enabled = true;
                }
            }
            catch (System.Exception exc)
            {
                MessageBox.Show("File open error: " + exc.Message);
            }
        }

        private void DisplayImg()
        {
            if (StarCamImg == null)
                return;
            PgmImage pgm = StarCamImg.GetPgm();
            StarCamDisplay.Width = pgm.width / 2; // resolution for display is reduced by factor of 2 per axis
            StarCamDisplay.Height = pgm.height / 2;

            Bitmap StarCamDispMap = new Bitmap(StarCamDisplay.Width, StarCamDisplay.Height);

            Graphics g = Graphics.FromImage(StarCamDispMap);

            double ratio = 255.0 / 4095.0; // scale 12-bit image to 8-bit 
            for (int i = 0; i < StarCamDisplay.Height; i++)
            {
                for (int j = 0; j < StarCamDisplay.Width; j++)
                {
                    int ipixel = 0;
                    for (int k = 0; k < 2; k++) // sum over two rows
                    {
                        for (int l = 0; l < 2; l++) // sum over two columns
                            ipixel += pgm.pixels[i * 2 + k][j * 2 + l]; // sum four original pixels per display pixel
                    }
                    ipixel = (int)(ratio * ipixel / 4); // scale to reduced resolution
                    if (ipixel > 255)
                        ipixel = 255;
                    StarCamDispMap.SetPixel(j, i, Color.FromArgb(255, ipixel, ipixel, ipixel)); // set pixel value
                }
            }
            StarCamDisplay.Image = StarCamDispMap;
        }

        private void ThresholdButton_Click(object sender, EventArgs e)
        {
            if (StarCamImg == null)
                return;
            ushort thresh;
            if (ushort.TryParse(threshold.Text, out thresh))
                pixels = StarCamImg.GetThreshPixels();
            else pixels = StarCamImg.GetThreshPixels();

            Bitmap StarCamDispMap = new Bitmap(StarCamDisplay.Width, StarCamDisplay.Height);
            Graphics g = Graphics.FromImage(StarCamDispMap);
            for (int i = 0; i < StarCamDisplay.Height; i++)
            {
                for (int j = 0; j < StarCamDisplay.Width; j++)
                    StarCamDispMap.SetPixel(j, i, Color.FromArgb(255, 0, 0, 0)); // set pixel black
            }
            for (int i = 0; i < pixels.Count; i++)
            {
                int x = (int)pixels[i].x / 2;
                int y = (int)pixels[i].y / 2;
                StarCamDispMap.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255)); // set pixel white
            }
            StarCamDisplay.Image = StarCamDispMap;
            GroupButton.Enabled = true;
        }

        private void GroupButton_Click(object sender, EventArgs e)
        {
            if (StarCamImg == null)
                return;

            stars = StarCamImg.GetStars();
            Bitmap StarCamDispMap = new Bitmap(StarCamDisplay.Width, StarCamDisplay.Height);
            Graphics g = Graphics.FromImage(StarCamDispMap);
            for (int i = 0; i < StarCamDisplay.Height; i++)
            {
                for (int j = 0; j < StarCamDisplay.Width; j++)
                    StarCamDispMap.SetPixel(j, i, Color.FromArgb(255, 0, 0, 0)); // set pixel black
            }
            int r, b, gr;
            for (int i = 0; i < pixels.Count; i++)
            {
                int x = (int)pixels[i].x / 2;
                int y = (int)pixels[i].y / 2;
                if (pixels[i].groupIndex != -1)
                {
                    r = 0;
                    if ((pixels[i].groupIndex % 3) == 1)
                        r = 255;
                    b = 0;
                    if ((pixels[i].groupIndex % 3) == 2)
                        b = 255;
                    gr = 0;
                    if ((pixels[i].groupIndex % 2) == 1)
                        gr = 255;
                    StarCamDispMap.SetPixel(x, y, Color.FromArgb(255, r, b, gr)); // set pixel white
                }
            }
            StarCamDisplay.Image = StarCamDispMap;
            CentroidButton.Enabled = true;
        }

        private void CentroidButton_Click(object sender, EventArgs e)
        {
            if (StarCamImg == null)
                return;

            Bitmap StarCamDispMap = new Bitmap(StarCamDisplay.Width, StarCamDisplay.Height);
            Graphics g = Graphics.FromImage(StarCamDispMap);
            for (int i = 0; i < StarCamDisplay.Height; i++)
            {
                for (int j = 0; j < StarCamDisplay.Width; j++)
                    StarCamDispMap.SetPixel(j, i, Color.FromArgb(255, 0, 0, 0)); // set pixel black
            }
            for (int i = 0; i < pixels.Count; i++)
            {
                int x = (int)pixels[i].x / 2;
                int y = (int)pixels[i].y / 2;
                if (pixels[i].groupIndex != -1)
                    StarCamDispMap.SetPixel(x, y, Color.FromArgb(255, 255, 255, 255)); // set pixel white
            }
            StarCamDisplay.Image = StarCamDispMap;
            for (int i = 0; i < stars.Count; i++)
                DrawPlus(g, Color.Green, stars[i].x, stars[i].y);

            DrawPlus(g, Color.Red, 0, 0);
        }

        private void DrawPlus(Graphics g, Color c, double x, double y)
        {
            Pen pen = new Pen(c, 1);
            float d = 10f;
            float xd = (float)x / 2; // resolution is reduced by factor of 2
            float yd = (float)y / 2;
            float x1 = xd - d * 0.5f;
            float y1 = yd - d * 0.5f;
            float x2 = xd + d * 0.5f;
            float y2 = yd + d * 0.5f;
            g.DrawLine(pen, x1, yd, x2, yd);
            g.DrawLine(pen, xd, y1, xd, y2);
        }

        private void StarIDButton_Click(object sender, EventArgs e)
        {
            if (StarCatalog1 == null)
            {
                StarCatalog1 = new StarCatalog();
                if (StarCatalog1 != null)
                    PrintToLog("Star catalog loaded.");
            }
            StarID.PerformStarID(StarCamImg, null, StarCatalog1);
            stars = StarCamImg.GetStars();
        }

        private void btn_ReadInclinometer_Click(object sender, EventArgs e)
        {
            /*
            if (!sp_inclinometer.IsOpen)
            {
                string portName = "COM5";
                sp_inclinometer.PortName = portName;
                sp_inclinometer.BaudRate = 9600;
                sp_inclinometer.Open();
            }

            byte[] msg = new byte[5];
            msg[0] = 0x68;
            msg[1] = 0x04;
            msg[2] = 0x00;
            msg[3] = 0x04;
            msg[4] = 0x08;

            //string rx = SendAndReceive(msg);
            //PrintToLog("text: " + rx); 
            SendAndReceiveBytes(msg);
            */
            double[] xyt = Inclin.ReadAnglesDegTemp();
            if (xyt != null)
            {
                rtb_xDeg.Text = xyt[0].ToString("N3");
                rtb_yDeg.Text = xyt[1].ToString("N3");
            }
        }

        private string SendAndReceive(byte[] msg) // Handles all send and received data over serial.
        {
            sp_inclinometer.Write(msg, 0, msg.Length);

            string rx = "";
            DateTime t0 = DateTime.UtcNow;
            TimeSpan dt = DateTime.UtcNow - t0;
            while (dt.TotalMilliseconds < 1000) // timeout
            {
                rx = rx + sp_inclinometer.ReadExisting();
                if (rx.Length > 14) // response length is a guess
                        return rx;
                dt = DateTime.UtcNow - t0;
            }
            return rx;
        }

        private byte[] SendAndReceiveBytes(byte[] msg) // Handles all send and received data over serial.
        {
            sp_inclinometer.Write(msg, 0, msg.Length);

            DateTime t0 = DateTime.UtcNow;
            TimeSpan dt = DateTime.UtcNow - t0;
            List<byte> bl = new List<byte>();

            while (dt.TotalMilliseconds < 1000) // timeout
            {
                while (sp_inclinometer.BytesToRead > 0)
                {
                    byte b = (byte)sp_inclinometer.ReadByte();
                    PrintToLog(String.Format("0x{0:X}", b));
                    bl.Add(b);
                }
                //rx = rx + sp_inclinometer.ReadExisting();
                //if (rx.Length > 14) // response length is a guess
                //    return rx;
                dt = DateTime.UtcNow - t0;
            }
            return bl.ToArray();
        }

        /*
        private void btnSetDefaultMode_Click(object sender, EventArgs e)
        {
            byte[] msg = new byte[6];
            msg[0] = 0x68;
            msg[1] = 0x05;
            msg[2] = 0x00;
            msg[3] = 0x0C;
            msg[4] = 0x00;
            msg[5] = 0x05 + 0x0C;

            if (!sp_inclinometer.IsOpen)
            {
                string portName = "COM5";
                sp_inclinometer.PortName = portName;
                sp_inclinometer.BaudRate = 9600;
                sp_inclinometer.Open();
            }
            sp_inclinometer.Write(msg, 0, msg.Length);

            string str = "rcv: 0x";
            while (sp_inclinometer.BytesToRead > 0)
            {
                byte b = (byte)sp_inclinometer.ReadByte();
                str += String.Format("{0:X} ", b);
            }
            PrintToLog(str);
        }*/
    }
}

