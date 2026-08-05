namespace GS_Tracking_KR
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btn_ReadMntAngles = new System.Windows.Forms.Button();
            this.btn_MntStop = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_MntConnect = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cb_SelectSerialPort = new System.Windows.Forms.ComboBox();
            this.AziRead = new System.Windows.Forms.RichTextBox();
            this.btn_GoTo = new System.Windows.Forms.Button();
            this.AltRead = new System.Windows.Forms.RichTextBox();
            this.rtb_GoToAlt = new System.Windows.Forms.RichTextBox();
            this.rtb_GoToAzi = new System.Windows.Forms.RichTextBox();
            this.Azi = new System.Windows.Forms.Label();
            this.Alt = new System.Windows.Forms.Label();
            this.sp_Mount = new System.IO.Ports.SerialPort(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_StartGrabStarCam = new System.Windows.Forms.Button();
            this.chb_AutoIDStarsCheck = new System.Windows.Forms.CheckBox();
            this.btn_CloseAllStarImg = new System.Windows.Forms.Button();
            this.btn_OpenStarImg = new System.Windows.Forms.Button();
            this.SavePicsCheck = new System.Windows.Forms.CheckBox();
            this.btn_TakeStarImg = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.rtb_Exposure = new System.Windows.Forms.RichTextBox();
            this.btn_InitStarCam = new System.Windows.Forms.Button();
            this.SessionLog = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_TrackCamInit = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_StopTrackGrab = new System.Windows.Forms.Button();
            this.btn_FindPmax = new System.Windows.Forms.Button();
            this.btn_StartTrackGrab = new System.Windows.Forms.Button();
            this.btn_CloseTrackCam = new System.Windows.Forms.Button();
            this.SaveTrackPicsCheck = new System.Windows.Forms.CheckBox();
            this.btn_TakeTrackImg = new System.Windows.Forms.Button();
            this.TrackCamDisplay = new System.Windows.Forms.PictureBox();
            this.bgw_GrabThread = new System.ComponentModel.BackgroundWorker();
            this.tmr_Display = new System.Windows.Forms.Timer(this.components);
            this.btn_InitFSM = new System.Windows.Forms.Button();
            this.btn_CloseFSM = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btn_CalGainFSMMnt = new System.Windows.Forms.Button();
            this.btn_CalGainFSMCam = new System.Windows.Forms.Button();
            this.btn_CLFSMTrack = new System.Windows.Forms.Button();
            this.btn_FSMtoOrigin = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.rtb_Vy = new System.Windows.Forms.RichTextBox();
            this.rtb_Vx = new System.Windows.Forms.RichTextBox();
            this.btn_FSMsetV = new System.Windows.Forms.Button();
            this.btn_AutoCalPointingModel = new System.Windows.Forms.Button();
            this.bgw_RxMeasPwr = new System.ComponentModel.BackgroundWorker();
            this.tmr_CLFSM = new System.Windows.Forms.Timer(this.components);
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btn_ConnectInclinometer = new System.Windows.Forms.Button();
            this.cb_SelectInclinometerPort = new System.Windows.Forms.ComboBox();
            this.sp_Inclinometer = new System.IO.Ports.SerialPort(this.components);
            this.tmr_CLMount = new System.Windows.Forms.Timer(this.components);
            this.btn_TrackStars = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btn_StarCamFeedback = new System.Windows.Forms.Button();
            this.btn_LoadPointingModel = new System.Windows.Forms.Button();
            this.btn_StopTrackTLE = new System.Windows.Forms.Button();
            this.btn_ClearCalMeas = new System.Windows.Forms.Button();
            this.lbl_AltSlew = new System.Windows.Forms.Label();
            this.lbl_AziSlew = new System.Windows.Forms.Label();
            this.lbl_PassTime = new System.Windows.Forms.Label();
            this.btn_TrackTLE = new System.Windows.Forms.Button();
            this.btn_LoadTLE = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_Cleardq = new System.Windows.Forms.Button();
            this.dgv_CalMeas = new System.Windows.Forms.DataGridView();
            this.btn_OffloadFSM = new System.Windows.Forms.Button();
            this.dgv_BrightStars = new System.Windows.Forms.DataGridView();
            this.cb_CalibrateFromExisting = new System.Windows.Forms.CheckBox();
            this.sp_IRcam = new System.IO.Ports.SerialPort(this.components);
            this.cb_simulation = new System.Windows.Forms.CheckBox();
            this.tmr_TakePic = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackCamDisplay)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_CalMeas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BrightStars)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_ReadMntAngles);
            this.groupBox1.Controls.Add(this.btn_MntStop);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btn_MntConnect);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cb_SelectSerialPort);
            this.groupBox1.Controls.Add(this.AziRead);
            this.groupBox1.Controls.Add(this.btn_GoTo);
            this.groupBox1.Controls.Add(this.AltRead);
            this.groupBox1.Controls.Add(this.rtb_GoToAlt);
            this.groupBox1.Controls.Add(this.rtb_GoToAzi);
            this.groupBox1.Controls.Add(this.Azi);
            this.groupBox1.Controls.Add(this.Alt);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(221, 204);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mount";
            // 
            // btn_ReadMntAngles
            // 
            this.btn_ReadMntAngles.Location = new System.Drawing.Point(156, 140);
            this.btn_ReadMntAngles.Name = "btn_ReadMntAngles";
            this.btn_ReadMntAngles.Size = new System.Drawing.Size(57, 28);
            this.btn_ReadMntAngles.TabIndex = 28;
            this.btn_ReadMntAngles.Text = "Read";
            this.btn_ReadMntAngles.UseVisualStyleBackColor = true;
            this.btn_ReadMntAngles.Click += new System.EventHandler(this.btn_ReadMntAngles_Click);
            // 
            // btn_MntStop
            // 
            this.btn_MntStop.Location = new System.Drawing.Point(156, 86);
            this.btn_MntStop.Name = "btn_MntStop";
            this.btn_MntStop.Size = new System.Drawing.Size(57, 25);
            this.btn_MntStop.TabIndex = 2;
            this.btn_MntStop.Text = "STOP";
            this.btn_MntStop.UseVisualStyleBackColor = true;
            this.btn_MntStop.Click += new System.EventHandler(this.btn_MntStop_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 162);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 17);
            this.label6.TabIndex = 27;
            this.label6.Text = "Azi (deg)";
            // 
            // btn_MntConnect
            // 
            this.btn_MntConnect.Location = new System.Drawing.Point(95, 21);
            this.btn_MntConnect.Name = "btn_MntConnect";
            this.btn_MntConnect.Size = new System.Drawing.Size(75, 25);
            this.btn_MntConnect.TabIndex = 1;
            this.btn_MntConnect.Text = "Connect";
            this.btn_MntConnect.UseVisualStyleBackColor = true;
            this.btn_MntConnect.Click += new System.EventHandler(this.btn_MntConnect_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 17);
            this.label7.TabIndex = 26;
            this.label7.Text = "Alt (deg)";
            // 
            // cb_SelectSerialPort
            // 
            this.cb_SelectSerialPort.FormattingEnabled = true;
            this.cb_SelectSerialPort.Location = new System.Drawing.Point(6, 21);
            this.cb_SelectSerialPort.Name = "cb_SelectSerialPort";
            this.cb_SelectSerialPort.Size = new System.Drawing.Size(83, 24);
            this.cb_SelectSerialPort.TabIndex = 0;
            // 
            // AziRead
            // 
            this.AziRead.Location = new System.Drawing.Point(76, 159);
            this.AziRead.Name = "AziRead";
            this.AziRead.ReadOnly = true;
            this.AziRead.Size = new System.Drawing.Size(69, 24);
            this.AziRead.TabIndex = 25;
            this.AziRead.Text = "";
            // 
            // btn_GoTo
            // 
            this.btn_GoTo.Location = new System.Drawing.Point(156, 55);
            this.btn_GoTo.Name = "btn_GoTo";
            this.btn_GoTo.Size = new System.Drawing.Size(57, 28);
            this.btn_GoTo.TabIndex = 23;
            this.btn_GoTo.Text = "Send";
            this.btn_GoTo.UseVisualStyleBackColor = true;
            this.btn_GoTo.Click += new System.EventHandler(this.btn_GoTo_Click);
            // 
            // AltRead
            // 
            this.AltRead.Location = new System.Drawing.Point(76, 129);
            this.AltRead.Name = "AltRead";
            this.AltRead.ReadOnly = true;
            this.AltRead.Size = new System.Drawing.Size(69, 24);
            this.AltRead.TabIndex = 24;
            this.AltRead.Text = "";
            // 
            // rtb_GoToAlt
            // 
            this.rtb_GoToAlt.Location = new System.Drawing.Point(75, 59);
            this.rtb_GoToAlt.Name = "rtb_GoToAlt";
            this.rtb_GoToAlt.Size = new System.Drawing.Size(69, 24);
            this.rtb_GoToAlt.TabIndex = 19;
            this.rtb_GoToAlt.Text = "";
            // 
            // rtb_GoToAzi
            // 
            this.rtb_GoToAzi.Location = new System.Drawing.Point(75, 89);
            this.rtb_GoToAzi.Name = "rtb_GoToAzi";
            this.rtb_GoToAzi.Size = new System.Drawing.Size(69, 24);
            this.rtb_GoToAzi.TabIndex = 20;
            this.rtb_GoToAzi.Text = "";
            // 
            // Azi
            // 
            this.Azi.AutoSize = true;
            this.Azi.Location = new System.Drawing.Point(10, 92);
            this.Azi.Name = "Azi";
            this.Azi.Size = new System.Drawing.Size(65, 17);
            this.Azi.TabIndex = 22;
            this.Azi.Text = "Azi (deg)";
            // 
            // Alt
            // 
            this.Alt.AutoSize = true;
            this.Alt.Location = new System.Drawing.Point(10, 62);
            this.Alt.Name = "Alt";
            this.Alt.Size = new System.Drawing.Size(62, 17);
            this.Alt.TabIndex = 21;
            this.Alt.Text = "Alt (deg)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_StartGrabStarCam);
            this.groupBox2.Controls.Add(this.chb_AutoIDStarsCheck);
            this.groupBox2.Controls.Add(this.btn_CloseAllStarImg);
            this.groupBox2.Controls.Add(this.btn_OpenStarImg);
            this.groupBox2.Controls.Add(this.SavePicsCheck);
            this.groupBox2.Controls.Add(this.btn_TakeStarImg);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.rtb_Exposure);
            this.groupBox2.Controls.Add(this.btn_InitStarCam);
            this.groupBox2.Location = new System.Drawing.Point(527, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(497, 87);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Star Camera";
            // 
            // btn_StartGrabStarCam
            // 
            this.btn_StartGrabStarCam.Enabled = false;
            this.btn_StartGrabStarCam.Location = new System.Drawing.Point(192, 51);
            this.btn_StartGrabStarCam.Name = "btn_StartGrabStarCam";
            this.btn_StartGrabStarCam.Size = new System.Drawing.Size(85, 25);
            this.btn_StartGrabStarCam.TabIndex = 12;
            this.btn_StartGrabStarCam.Text = "Start Grab";
            this.btn_StartGrabStarCam.UseVisualStyleBackColor = true;
            this.btn_StartGrabStarCam.Click += new System.EventHandler(this.btn_StartGrabStarCam_Click);
            // 
            // chb_AutoIDStarsCheck
            // 
            this.chb_AutoIDStarsCheck.AutoSize = true;
            this.chb_AutoIDStarsCheck.Checked = true;
            this.chb_AutoIDStarsCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chb_AutoIDStarsCheck.Location = new System.Drawing.Point(374, 21);
            this.chb_AutoIDStarsCheck.Name = "chb_AutoIDStarsCheck";
            this.chb_AutoIDStarsCheck.Size = new System.Drawing.Size(119, 21);
            this.chb_AutoIDStarsCheck.TabIndex = 11;
            this.chb_AutoIDStarsCheck.Text = "Auto ID stars?";
            this.chb_AutoIDStarsCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.chb_AutoIDStarsCheck.UseVisualStyleBackColor = true;
            // 
            // btn_CloseAllStarImg
            // 
            this.btn_CloseAllStarImg.Location = new System.Drawing.Point(283, 21);
            this.btn_CloseAllStarImg.Name = "btn_CloseAllStarImg";
            this.btn_CloseAllStarImg.Size = new System.Drawing.Size(85, 25);
            this.btn_CloseAllStarImg.TabIndex = 10;
            this.btn_CloseAllStarImg.Text = "Close All";
            this.btn_CloseAllStarImg.UseVisualStyleBackColor = true;
            this.btn_CloseAllStarImg.Click += new System.EventHandler(this.btn_CloseAllStarImg_Click);
            // 
            // btn_OpenStarImg
            // 
            this.btn_OpenStarImg.Location = new System.Drawing.Point(192, 21);
            this.btn_OpenStarImg.Name = "btn_OpenStarImg";
            this.btn_OpenStarImg.Size = new System.Drawing.Size(85, 25);
            this.btn_OpenStarImg.TabIndex = 9;
            this.btn_OpenStarImg.Text = "Open Img";
            this.btn_OpenStarImg.UseVisualStyleBackColor = true;
            this.btn_OpenStarImg.Click += new System.EventHandler(this.btn_OpenStarImg_Click);
            // 
            // SavePicsCheck
            // 
            this.SavePicsCheck.AutoSize = true;
            this.SavePicsCheck.Checked = true;
            this.SavePicsCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SavePicsCheck.Location = new System.Drawing.Point(374, 48);
            this.SavePicsCheck.Name = "SavePicsCheck";
            this.SavePicsCheck.Size = new System.Drawing.Size(117, 21);
            this.SavePicsCheck.TabIndex = 8;
            this.SavePicsCheck.Text = "Save all pics?";
            this.SavePicsCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.SavePicsCheck.UseVisualStyleBackColor = true;
            // 
            // btn_TakeStarImg
            // 
            this.btn_TakeStarImg.Enabled = false;
            this.btn_TakeStarImg.Location = new System.Drawing.Point(101, 21);
            this.btn_TakeStarImg.Name = "btn_TakeStarImg";
            this.btn_TakeStarImg.Size = new System.Drawing.Size(85, 25);
            this.btn_TakeStarImg.TabIndex = 7;
            this.btn_TakeStarImg.Text = "Take Img";
            this.btn_TakeStarImg.UseVisualStyleBackColor = true;
            this.btn_TakeStarImg.Click += new System.EventHandler(this.btn_TakeStarImg_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Exposure (ms)";
            // 
            // rtb_Exposure
            // 
            this.rtb_Exposure.Enabled = false;
            this.rtb_Exposure.Location = new System.Drawing.Point(110, 52);
            this.rtb_Exposure.Multiline = false;
            this.rtb_Exposure.Name = "rtb_Exposure";
            this.rtb_Exposure.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rtb_Exposure.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtb_Exposure.Size = new System.Drawing.Size(69, 24);
            this.rtb_Exposure.TabIndex = 1;
            this.rtb_Exposure.Text = "400";
            this.rtb_Exposure.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.rtbExposure_KeyPress);
            this.rtb_Exposure.Leave += new System.EventHandler(this.rtb_Exposure_Leave);
            // 
            // btn_InitStarCam
            // 
            this.btn_InitStarCam.Location = new System.Drawing.Point(10, 21);
            this.btn_InitStarCam.Name = "btn_InitStarCam";
            this.btn_InitStarCam.Size = new System.Drawing.Size(85, 25);
            this.btn_InitStarCam.TabIndex = 0;
            this.btn_InitStarCam.Text = "Init Cam";
            this.btn_InitStarCam.UseVisualStyleBackColor = true;
            this.btn_InitStarCam.Click += new System.EventHandler(this.btn_InitStarCam_Click);
            // 
            // SessionLog
            // 
            this.SessionLog.Location = new System.Drawing.Point(527, 429);
            this.SessionLog.Name = "SessionLog";
            this.SessionLog.ReadOnly = true;
            this.SessionLog.Size = new System.Drawing.Size(497, 348);
            this.SessionLog.TabIndex = 3;
            this.SessionLog.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(524, 409);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Session Log";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(220, 826);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 27);
            this.button1.TabIndex = 5;
            this.button1.Text = "TestForm";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_TrackCamInit
            // 
            this.btn_TrackCamInit.Location = new System.Drawing.Point(18, 21);
            this.btn_TrackCamInit.Name = "btn_TrackCamInit";
            this.btn_TrackCamInit.Size = new System.Drawing.Size(85, 25);
            this.btn_TrackCamInit.TabIndex = 12;
            this.btn_TrackCamInit.Text = "Init Cam";
            this.btn_TrackCamInit.UseVisualStyleBackColor = true;
            this.btn_TrackCamInit.Click += new System.EventHandler(this.btn_TrackCamInit_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_StopTrackGrab);
            this.groupBox3.Controls.Add(this.btn_FindPmax);
            this.groupBox3.Controls.Add(this.btn_StartTrackGrab);
            this.groupBox3.Controls.Add(this.btn_CloseTrackCam);
            this.groupBox3.Controls.Add(this.SaveTrackPicsCheck);
            this.groupBox3.Controls.Add(this.btn_TakeTrackImg);
            this.groupBox3.Controls.Add(this.TrackCamDisplay);
            this.groupBox3.Controls.Add(this.btn_TrackCamInit);
            this.groupBox3.Location = new System.Drawing.Point(527, 105);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(497, 301);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tracking Camera";
            // 
            // btn_StopTrackGrab
            // 
            this.btn_StopTrackGrab.Enabled = false;
            this.btn_StopTrackGrab.Location = new System.Drawing.Point(18, 118);
            this.btn_StopTrackGrab.Name = "btn_StopTrackGrab";
            this.btn_StopTrackGrab.Size = new System.Drawing.Size(85, 25);
            this.btn_StopTrackGrab.TabIndex = 17;
            this.btn_StopTrackGrab.Text = "Stop Grab";
            this.btn_StopTrackGrab.UseVisualStyleBackColor = true;
            this.btn_StopTrackGrab.Click += new System.EventHandler(this.btn_StopTrackGrab_Click);
            // 
            // btn_FindPmax
            // 
            this.btn_FindPmax.Enabled = false;
            this.btn_FindPmax.Location = new System.Drawing.Point(18, 260);
            this.btn_FindPmax.Name = "btn_FindPmax";
            this.btn_FindPmax.Size = new System.Drawing.Size(87, 25);
            this.btn_FindPmax.TabIndex = 16;
            this.btn_FindPmax.Text = "Find Pmax";
            this.btn_FindPmax.UseVisualStyleBackColor = true;
            this.btn_FindPmax.Click += new System.EventHandler(this.btn_FindPmax_Click);
            // 
            // btn_StartTrackGrab
            // 
            this.btn_StartTrackGrab.Enabled = false;
            this.btn_StartTrackGrab.Location = new System.Drawing.Point(18, 86);
            this.btn_StartTrackGrab.Name = "btn_StartTrackGrab";
            this.btn_StartTrackGrab.Size = new System.Drawing.Size(85, 25);
            this.btn_StartTrackGrab.TabIndex = 16;
            this.btn_StartTrackGrab.Text = "Start Grab";
            this.btn_StartTrackGrab.UseVisualStyleBackColor = true;
            this.btn_StartTrackGrab.Click += new System.EventHandler(this.btn_StartTrackGrab_Click);
            // 
            // btn_CloseTrackCam
            // 
            this.btn_CloseTrackCam.Location = new System.Drawing.Point(18, 151);
            this.btn_CloseTrackCam.Name = "btn_CloseTrackCam";
            this.btn_CloseTrackCam.Size = new System.Drawing.Size(85, 25);
            this.btn_CloseTrackCam.TabIndex = 15;
            this.btn_CloseTrackCam.Text = "Close Cam";
            this.btn_CloseTrackCam.UseVisualStyleBackColor = true;
            this.btn_CloseTrackCam.Click += new System.EventHandler(this.btn_CloseTrackCam_Click);
            // 
            // SaveTrackPicsCheck
            // 
            this.SaveTrackPicsCheck.AutoSize = true;
            this.SaveTrackPicsCheck.Location = new System.Drawing.Point(18, 182);
            this.SaveTrackPicsCheck.Name = "SaveTrackPicsCheck";
            this.SaveTrackPicsCheck.Size = new System.Drawing.Size(117, 21);
            this.SaveTrackPicsCheck.TabIndex = 12;
            this.SaveTrackPicsCheck.Text = "Save all pics?";
            this.SaveTrackPicsCheck.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.SaveTrackPicsCheck.UseVisualStyleBackColor = true;
            // 
            // btn_TakeTrackImg
            // 
            this.btn_TakeTrackImg.Enabled = false;
            this.btn_TakeTrackImg.Location = new System.Drawing.Point(18, 54);
            this.btn_TakeTrackImg.Name = "btn_TakeTrackImg";
            this.btn_TakeTrackImg.Size = new System.Drawing.Size(85, 25);
            this.btn_TakeTrackImg.TabIndex = 14;
            this.btn_TakeTrackImg.Text = "Take Img\r\n";
            this.btn_TakeTrackImg.UseVisualStyleBackColor = true;
            this.btn_TakeTrackImg.Click += new System.EventHandler(this.btn_TakeTrackImg_Click);
            // 
            // TrackCamDisplay
            // 
            this.TrackCamDisplay.Location = new System.Drawing.Point(153, 21);
            this.TrackCamDisplay.Name = "TrackCamDisplay";
            this.TrackCamDisplay.Size = new System.Drawing.Size(309, 254);
            this.TrackCamDisplay.TabIndex = 13;
            this.TrackCamDisplay.TabStop = false;
            // 
            // bgw_GrabThread
            // 
            this.bgw_GrabThread.WorkerSupportsCancellation = true;
            this.bgw_GrabThread.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_GrabThread_DoWork);
            // 
            // tmr_Display
            // 
            this.tmr_Display.Tick += new System.EventHandler(this.tmr_Display_Tick);
            // 
            // btn_InitFSM
            // 
            this.btn_InitFSM.Location = new System.Drawing.Point(14, 26);
            this.btn_InitFSM.Name = "btn_InitFSM";
            this.btn_InitFSM.Size = new System.Drawing.Size(83, 25);
            this.btn_InitFSM.TabIndex = 3;
            this.btn_InitFSM.Text = "Init FSM";
            this.btn_InitFSM.UseVisualStyleBackColor = true;
            this.btn_InitFSM.Click += new System.EventHandler(this.btn_InitFSM_Click);
            // 
            // btn_CloseFSM
            // 
            this.btn_CloseFSM.Enabled = false;
            this.btn_CloseFSM.Location = new System.Drawing.Point(103, 26);
            this.btn_CloseFSM.Name = "btn_CloseFSM";
            this.btn_CloseFSM.Size = new System.Drawing.Size(83, 25);
            this.btn_CloseFSM.TabIndex = 14;
            this.btn_CloseFSM.Text = "Close FSM";
            this.btn_CloseFSM.UseVisualStyleBackColor = true;
            this.btn_CloseFSM.Click += new System.EventHandler(this.btn_CloseFSM_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btn_CalGainFSMMnt);
            this.groupBox4.Controls.Add(this.btn_CalGainFSMCam);
            this.groupBox4.Controls.Add(this.btn_CLFSMTrack);
            this.groupBox4.Controls.Add(this.btn_FSMtoOrigin);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.rtb_Vy);
            this.groupBox4.Controls.Add(this.rtb_Vx);
            this.groupBox4.Controls.Add(this.btn_FSMsetV);
            this.groupBox4.Controls.Add(this.btn_InitFSM);
            this.groupBox4.Controls.Add(this.btn_CloseFSM);
            this.groupBox4.Location = new System.Drawing.Point(239, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(282, 204);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "FSM";
            // 
            // btn_CalGainFSMMnt
            // 
            this.btn_CalGainFSMMnt.Enabled = false;
            this.btn_CalGainFSMMnt.Location = new System.Drawing.Point(141, 171);
            this.btn_CalGainFSMMnt.Name = "btn_CalGainFSMMnt";
            this.btn_CalGainFSMMnt.Size = new System.Drawing.Size(118, 25);
            this.btn_CalGainFSMMnt.TabIndex = 24;
            this.btn_CalGainFSMMnt.Text = "Cal. Gain Mount";
            this.btn_CalGainFSMMnt.UseVisualStyleBackColor = true;
            this.btn_CalGainFSMMnt.Click += new System.EventHandler(this.btn_CalGainFSMMnt_Click);
            // 
            // btn_CalGainFSMCam
            // 
            this.btn_CalGainFSMCam.Enabled = false;
            this.btn_CalGainFSMCam.Location = new System.Drawing.Point(141, 142);
            this.btn_CalGainFSMCam.Name = "btn_CalGainFSMCam";
            this.btn_CalGainFSMCam.Size = new System.Drawing.Size(118, 25);
            this.btn_CalGainFSMCam.TabIndex = 23;
            this.btn_CalGainFSMCam.Text = "Cal. Gain Cam";
            this.btn_CalGainFSMCam.UseVisualStyleBackColor = true;
            this.btn_CalGainFSMCam.Click += new System.EventHandler(this.btn_CalGainFSMCam_Click);
            // 
            // btn_CLFSMTrack
            // 
            this.btn_CLFSMTrack.Location = new System.Drawing.Point(14, 148);
            this.btn_CLFSMTrack.Name = "btn_CLFSMTrack";
            this.btn_CLFSMTrack.Size = new System.Drawing.Size(109, 40);
            this.btn_CLFSMTrack.TabIndex = 18;
            this.btn_CLFSMTrack.Text = "Start CL Track";
            this.btn_CLFSMTrack.UseVisualStyleBackColor = true;
            this.btn_CLFSMTrack.Click += new System.EventHandler(this.btn_CLFSMTrack_Click);
            // 
            // btn_FSMtoOrigin
            // 
            this.btn_FSMtoOrigin.Enabled = false;
            this.btn_FSMtoOrigin.Location = new System.Drawing.Point(102, 61);
            this.btn_FSMtoOrigin.Name = "btn_FSMtoOrigin";
            this.btn_FSMtoOrigin.Size = new System.Drawing.Size(83, 25);
            this.btn_FSMtoOrigin.TabIndex = 20;
            this.btn_FSMtoOrigin.Text = "To Origin";
            this.btn_FSMtoOrigin.UseVisualStyleBackColor = true;
            this.btn_FSMtoOrigin.Click += new System.EventHandler(this.btn_FSMtoOrigin_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 118);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 17);
            this.label5.TabIndex = 19;
            this.label5.Text = "Range: [-10,10]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Vy";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "Vx";
            // 
            // rtb_Vy
            // 
            this.rtb_Vy.Location = new System.Drawing.Point(32, 93);
            this.rtb_Vy.Multiline = false;
            this.rtb_Vy.Name = "rtb_Vy";
            this.rtb_Vy.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rtb_Vy.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtb_Vy.Size = new System.Drawing.Size(48, 24);
            this.rtb_Vy.TabIndex = 16;
            this.rtb_Vy.Text = "";
            // 
            // rtb_Vx
            // 
            this.rtb_Vx.Location = new System.Drawing.Point(32, 62);
            this.rtb_Vx.Multiline = false;
            this.rtb_Vx.Name = "rtb_Vx";
            this.rtb_Vx.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rtb_Vx.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtb_Vx.Size = new System.Drawing.Size(48, 24);
            this.rtb_Vx.TabIndex = 12;
            this.rtb_Vx.Text = "";
            // 
            // btn_FSMsetV
            // 
            this.btn_FSMsetV.Enabled = false;
            this.btn_FSMsetV.Location = new System.Drawing.Point(102, 93);
            this.btn_FSMsetV.Name = "btn_FSMsetV";
            this.btn_FSMsetV.Size = new System.Drawing.Size(83, 25);
            this.btn_FSMsetV.TabIndex = 15;
            this.btn_FSMsetV.Text = "Set V";
            this.btn_FSMsetV.UseVisualStyleBackColor = true;
            this.btn_FSMsetV.Click += new System.EventHandler(this.btn_FSMsetV_Click);
            // 
            // btn_AutoCalPointingModel
            // 
            this.btn_AutoCalPointingModel.Location = new System.Drawing.Point(10, 19);
            this.btn_AutoCalPointingModel.Name = "btn_AutoCalPointingModel";
            this.btn_AutoCalPointingModel.Size = new System.Drawing.Size(122, 56);
            this.btn_AutoCalPointingModel.TabIndex = 17;
            this.btn_AutoCalPointingModel.Text = "Calibrate Pointing Model";
            this.btn_AutoCalPointingModel.UseVisualStyleBackColor = true;
            this.btn_AutoCalPointingModel.Click += new System.EventHandler(this.btn_AutoCalPointingModel_Click);
            // 
            // bgw_RxMeasPwr
            // 
            this.bgw_RxMeasPwr.WorkerSupportsCancellation = true;
            this.bgw_RxMeasPwr.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_RxMeasPwr_DoWork);
            // 
            // tmr_CLFSM
            // 
            this.tmr_CLFSM.Interval = 20;
            this.tmr_CLFSM.Tick += new System.EventHandler(this.tmr_CLFSM_Tick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btn_ConnectInclinometer);
            this.groupBox5.Controls.Add(this.cb_SelectInclinometerPort);
            this.groupBox5.Location = new System.Drawing.Point(18, 790);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(196, 63);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Inclinometer";
            // 
            // btn_ConnectInclinometer
            // 
            this.btn_ConnectInclinometer.Location = new System.Drawing.Point(110, 21);
            this.btn_ConnectInclinometer.Name = "btn_ConnectInclinometer";
            this.btn_ConnectInclinometer.Size = new System.Drawing.Size(75, 25);
            this.btn_ConnectInclinometer.TabIndex = 1;
            this.btn_ConnectInclinometer.Text = "Connect";
            this.btn_ConnectInclinometer.UseVisualStyleBackColor = true;
            this.btn_ConnectInclinometer.Click += new System.EventHandler(this.btn_ConnectInclinometer_Click);
            // 
            // cb_SelectInclinometerPort
            // 
            this.cb_SelectInclinometerPort.FormattingEnabled = true;
            this.cb_SelectInclinometerPort.Location = new System.Drawing.Point(6, 21);
            this.cb_SelectInclinometerPort.Name = "cb_SelectInclinometerPort";
            this.cb_SelectInclinometerPort.Size = new System.Drawing.Size(83, 24);
            this.cb_SelectInclinometerPort.TabIndex = 0;
            // 
            // tmr_CLMount
            // 
            this.tmr_CLMount.Tick += new System.EventHandler(this.tmr_CLMount_Tick);
            // 
            // btn_TrackStars
            // 
            this.btn_TrackStars.Location = new System.Drawing.Point(401, 310);
            this.btn_TrackStars.Name = "btn_TrackStars";
            this.btn_TrackStars.Size = new System.Drawing.Size(89, 25);
            this.btn_TrackStars.TabIndex = 12;
            this.btn_TrackStars.Text = "Find Stars";
            this.btn_TrackStars.UseVisualStyleBackColor = true;
            this.btn_TrackStars.Click += new System.EventHandler(this.btn_FindStars_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btn_StarCamFeedback);
            this.groupBox6.Controls.Add(this.btn_LoadPointingModel);
            this.groupBox6.Controls.Add(this.btn_StopTrackTLE);
            this.groupBox6.Controls.Add(this.btn_ClearCalMeas);
            this.groupBox6.Controls.Add(this.lbl_AltSlew);
            this.groupBox6.Controls.Add(this.lbl_AziSlew);
            this.groupBox6.Controls.Add(this.lbl_PassTime);
            this.groupBox6.Controls.Add(this.btn_TrackTLE);
            this.groupBox6.Controls.Add(this.btn_LoadTLE);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Controls.Add(this.btn_Cleardq);
            this.groupBox6.Controls.Add(this.dgv_CalMeas);
            this.groupBox6.Controls.Add(this.btn_OffloadFSM);
            this.groupBox6.Controls.Add(this.dgv_BrightStars);
            this.groupBox6.Controls.Add(this.cb_CalibrateFromExisting);
            this.groupBox6.Controls.Add(this.btn_TrackStars);
            this.groupBox6.Controls.Add(this.btn_AutoCalPointingModel);
            this.groupBox6.Location = new System.Drawing.Point(14, 220);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(507, 557);
            this.groupBox6.TabIndex = 19;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Tracking";
            // 
            // btn_StarCamFeedback
            // 
            this.btn_StarCamFeedback.Location = new System.Drawing.Point(395, 19);
            this.btn_StarCamFeedback.Name = "btn_StarCamFeedback";
            this.btn_StarCamFeedback.Size = new System.Drawing.Size(95, 45);
            this.btn_StarCamFeedback.TabIndex = 36;
            this.btn_StarCamFeedback.Text = "Start Cam Feedback";
            this.btn_StarCamFeedback.UseVisualStyleBackColor = true;
            this.btn_StarCamFeedback.Click += new System.EventHandler(this.btn_StarCamFeedback_Click);
            // 
            // btn_LoadPointingModel
            // 
            this.btn_LoadPointingModel.Location = new System.Drawing.Point(10, 81);
            this.btn_LoadPointingModel.Name = "btn_LoadPointingModel";
            this.btn_LoadPointingModel.Size = new System.Drawing.Size(122, 25);
            this.btn_LoadPointingModel.TabIndex = 35;
            this.btn_LoadPointingModel.Text = "Load Model";
            this.btn_LoadPointingModel.UseVisualStyleBackColor = true;
            this.btn_LoadPointingModel.Click += new System.EventHandler(this.btn_LoadPointingModel_Click);
            // 
            // btn_StopTrackTLE
            // 
            this.btn_StopTrackTLE.Location = new System.Drawing.Point(12, 514);
            this.btn_StopTrackTLE.Name = "btn_StopTrackTLE";
            this.btn_StopTrackTLE.Size = new System.Drawing.Size(100, 25);
            this.btn_StopTrackTLE.TabIndex = 34;
            this.btn_StopTrackTLE.Text = "Stop Track";
            this.btn_StopTrackTLE.UseVisualStyleBackColor = true;
            this.btn_StopTrackTLE.Click += new System.EventHandler(this.btn_StopTrackTLE_Click);
            // 
            // btn_ClearCalMeas
            // 
            this.btn_ClearCalMeas.Location = new System.Drawing.Point(277, 50);
            this.btn_ClearCalMeas.Name = "btn_ClearCalMeas";
            this.btn_ClearCalMeas.Size = new System.Drawing.Size(112, 25);
            this.btn_ClearCalMeas.TabIndex = 33;
            this.btn_ClearCalMeas.Text = "Clear Cal Meas";
            this.btn_ClearCalMeas.UseVisualStyleBackColor = true;
            this.btn_ClearCalMeas.Click += new System.EventHandler(this.btn_ClearCalMeas_Click);
            // 
            // lbl_AltSlew
            // 
            this.lbl_AltSlew.AutoSize = true;
            this.lbl_AltSlew.Location = new System.Drawing.Point(118, 489);
            this.lbl_AltSlew.Name = "lbl_AltSlew";
            this.lbl_AltSlew.Size = new System.Drawing.Size(59, 17);
            this.lbl_AltSlew.TabIndex = 32;
            this.lbl_AltSlew.Text = "Alt slew:";
            // 
            // lbl_AziSlew
            // 
            this.lbl_AziSlew.AutoSize = true;
            this.lbl_AziSlew.Location = new System.Drawing.Point(118, 471);
            this.lbl_AziSlew.Name = "lbl_AziSlew";
            this.lbl_AziSlew.Size = new System.Drawing.Size(62, 17);
            this.lbl_AziSlew.TabIndex = 31;
            this.lbl_AziSlew.Text = "Azi slew:";
            // 
            // lbl_PassTime
            // 
            this.lbl_PassTime.AutoSize = true;
            this.lbl_PassTime.Location = new System.Drawing.Point(118, 453);
            this.lbl_PassTime.Name = "lbl_PassTime";
            this.lbl_PassTime.Size = new System.Drawing.Size(132, 17);
            this.lbl_PassTime.TabIndex = 30;
            this.lbl_PassTime.Text = "Countdown to pass:";
            // 
            // btn_TrackTLE
            // 
            this.btn_TrackTLE.Enabled = false;
            this.btn_TrackTLE.Location = new System.Drawing.Point(11, 482);
            this.btn_TrackTLE.Name = "btn_TrackTLE";
            this.btn_TrackTLE.Size = new System.Drawing.Size(101, 26);
            this.btn_TrackTLE.TabIndex = 29;
            this.btn_TrackTLE.Text = "Track TLE";
            this.btn_TrackTLE.UseVisualStyleBackColor = true;
            this.btn_TrackTLE.Click += new System.EventHandler(this.btn_TrackTLE_Click);
            // 
            // btn_LoadTLE
            // 
            this.btn_LoadTLE.Location = new System.Drawing.Point(11, 450);
            this.btn_LoadTLE.Name = "btn_LoadTLE";
            this.btn_LoadTLE.Size = new System.Drawing.Size(101, 26);
            this.btn_LoadTLE.TabIndex = 28;
            this.btn_LoadTLE.Text = "Load TLE";
            this.btn_LoadTLE.UseVisualStyleBackColor = true;
            this.btn_LoadTLE.Click += new System.EventHandler(this.btn_LoadTLE_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 120);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(432, 17);
            this.label9.TabIndex = 27;
            this.label9.Text = "Measurements for calibration (double click to delete measurement):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 318);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 17);
            this.label8.TabIndex = 26;
            this.label8.Text = "Double click on star to track:";
            // 
            // btn_Cleardq
            // 
            this.btn_Cleardq.Location = new System.Drawing.Point(277, 19);
            this.btn_Cleardq.Name = "btn_Cleardq";
            this.btn_Cleardq.Size = new System.Drawing.Size(112, 25);
            this.btn_Cleardq.TabIndex = 25;
            this.btn_Cleardq.Text = "Clear dq";
            this.btn_Cleardq.UseVisualStyleBackColor = true;
            this.btn_Cleardq.Click += new System.EventHandler(this.btn_Cleardq_Click);
            // 
            // dgv_CalMeas
            // 
            this.dgv_CalMeas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_CalMeas.Location = new System.Drawing.Point(11, 139);
            this.dgv_CalMeas.Margin = new System.Windows.Forms.Padding(4);
            this.dgv_CalMeas.Name = "dgv_CalMeas";
            this.dgv_CalMeas.Size = new System.Drawing.Size(478, 162);
            this.dgv_CalMeas.TabIndex = 24;
            this.dgv_CalMeas.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CalMeas_CellDoubleClick);
            // 
            // btn_OffloadFSM
            // 
            this.btn_OffloadFSM.Location = new System.Drawing.Point(394, 69);
            this.btn_OffloadFSM.Name = "btn_OffloadFSM";
            this.btn_OffloadFSM.Size = new System.Drawing.Size(95, 45);
            this.btn_OffloadFSM.TabIndex = 23;
            this.btn_OffloadFSM.Text = "Start Offload FSM";
            this.btn_OffloadFSM.UseVisualStyleBackColor = true;
            this.btn_OffloadFSM.Click += new System.EventHandler(this.btn_OffloadFSM_Click);
            // 
            // dgv_BrightStars
            // 
            this.dgv_BrightStars.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_BrightStars.Location = new System.Drawing.Point(12, 337);
            this.dgv_BrightStars.Margin = new System.Windows.Forms.Padding(4);
            this.dgv_BrightStars.Name = "dgv_BrightStars";
            this.dgv_BrightStars.Size = new System.Drawing.Size(478, 95);
            this.dgv_BrightStars.TabIndex = 21;
            this.dgv_BrightStars.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_BrightStars_CellDoubleClick);
            // 
            // cb_CalibrateFromExisting
            // 
            this.cb_CalibrateFromExisting.AutoSize = true;
            this.cb_CalibrateFromExisting.Location = new System.Drawing.Point(136, 29);
            this.cb_CalibrateFromExisting.Name = "cb_CalibrateFromExisting";
            this.cb_CalibrateFromExisting.Size = new System.Drawing.Size(150, 38);
            this.cb_CalibrateFromExisting.TabIndex = 18;
            this.cb_CalibrateFromExisting.Text = "Use existing data? \r\nElse, run auto-cal";
            this.cb_CalibrateFromExisting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cb_CalibrateFromExisting.UseVisualStyleBackColor = true;
            // 
            // sp_IRcam
            // 
            this.sp_IRcam.BaudRate = 57600;
            // 
            // cb_simulation
            // 
            this.cb_simulation.AutoSize = true;
            this.cb_simulation.Location = new System.Drawing.Point(219, 803);
            this.cb_simulation.Name = "cb_simulation";
            this.cb_simulation.Size = new System.Drawing.Size(103, 21);
            this.cb_simulation.TabIndex = 20;
            this.cb_simulation.Text = "Simulation?";
            this.cb_simulation.UseVisualStyleBackColor = true;
            this.cb_simulation.CheckedChanged += new System.EventHandler(this.cb_simulation_CheckedChanged);
            // 
            // tmr_TakePic
            // 
            this.tmr_TakePic.Interval = 2000;
            this.tmr_TakePic.Tick += new System.EventHandler(this.tmr_TakePic_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 880);
            this.Controls.Add(this.cb_simulation);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SessionLog);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "        GS Tracking Main Form";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackCamDisplay)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_CalMeas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BrightStars)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cb_SelectSerialPort;
        private System.Windows.Forms.Button btn_MntConnect;
        private System.IO.Ports.SerialPort sp_Mount;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox SessionLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_MntStop;
        private System.Windows.Forms.Button btn_InitStarCam;
        private System.Windows.Forms.RichTextBox rtb_Exposure;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_TakeStarImg;
        private System.Windows.Forms.CheckBox SavePicsCheck;
        private System.Windows.Forms.Button btn_OpenStarImg;
        private System.Windows.Forms.Button btn_CloseAllStarImg;
        private System.Windows.Forms.CheckBox chb_AutoIDStarsCheck;
        private System.Windows.Forms.Button btn_TrackCamInit;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_TakeTrackImg;
        private System.Windows.Forms.PictureBox TrackCamDisplay;
        private System.Windows.Forms.CheckBox SaveTrackPicsCheck;
        private System.Windows.Forms.Button btn_StopTrackGrab;
        private System.Windows.Forms.Button btn_StartTrackGrab;
        private System.ComponentModel.BackgroundWorker bgw_GrabThread;
        private System.Windows.Forms.Timer tmr_Display;
        private System.Windows.Forms.Button btn_InitFSM;
        private System.Windows.Forms.Button btn_CloseFSM;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btn_FSMsetV;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rtb_Vy;
        private System.Windows.Forms.RichTextBox rtb_Vx;
        private System.Windows.Forms.Button btn_FSMtoOrigin;
        private System.Windows.Forms.Button btn_CloseTrackCam;
        private System.Windows.Forms.Button btn_FindPmax;
        private System.Windows.Forms.Button btn_AutoCalPointingModel;
        private System.Windows.Forms.Button btn_CLFSMTrack;
        private System.ComponentModel.BackgroundWorker bgw_RxMeasPwr;
        private System.Windows.Forms.Timer tmr_CLFSM;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btn_ConnectInclinometer;
        private System.Windows.Forms.ComboBox cb_SelectInclinometerPort;
        private System.IO.Ports.SerialPort sp_Inclinometer;
        private System.Windows.Forms.Timer tmr_CLMount;
        private System.Windows.Forms.Button btn_ReadMntAngles;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RichTextBox AziRead;
        private System.Windows.Forms.Button btn_GoTo;
        private System.Windows.Forms.RichTextBox AltRead;
        private System.Windows.Forms.RichTextBox rtb_GoToAlt;
        private System.Windows.Forms.RichTextBox rtb_GoToAzi;
        private System.Windows.Forms.Label Azi;
        private System.Windows.Forms.Label Alt;
        private System.Windows.Forms.Button btn_TrackStars;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox cb_CalibrateFromExisting;
        private System.Windows.Forms.DataGridView dgv_BrightStars;
        private System.IO.Ports.SerialPort sp_IRcam;
        private System.Windows.Forms.Button btn_CalGainFSMCam;
        private System.Windows.Forms.Button btn_OffloadFSM;
        private System.Windows.Forms.Button btn_CalGainFSMMnt;
        private System.Windows.Forms.CheckBox cb_simulation;
        private System.Windows.Forms.DataGridView dgv_CalMeas;
        private System.Windows.Forms.Button btn_Cleardq;
        private System.Windows.Forms.Button btn_LoadTLE;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_TrackTLE;
        private System.Windows.Forms.Label lbl_PassTime;
        private System.Windows.Forms.Label lbl_AltSlew;
        private System.Windows.Forms.Label lbl_AziSlew;
        private System.Windows.Forms.Button btn_StartGrabStarCam;
        private System.Windows.Forms.Timer tmr_TakePic;
        private System.Windows.Forms.Button btn_ClearCalMeas;
        private System.Windows.Forms.Button btn_StopTrackTLE;
        private System.Windows.Forms.Button btn_LoadPointingModel;
        private System.Windows.Forms.Button btn_StarCamFeedback;
    }
}

