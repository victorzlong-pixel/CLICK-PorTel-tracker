namespace GS_Tracking_KR
{
    partial class TestCodeForm
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
            this.GoToAlt = new System.Windows.Forms.RichTextBox();
            this.GoToAzi = new System.Windows.Forms.RichTextBox();
            this.Alt = new System.Windows.Forms.Label();
            this.Azi = new System.Windows.Forms.Label();
            this.GoTo = new System.Windows.Forms.GroupBox();
            this.ReadButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AziRead = new System.Windows.Forms.RichTextBox();
            this.AltRead = new System.Windows.Forms.RichTextBox();
            this.GoToSendButton = new System.Windows.Forms.Button();
            this.SessionLog = new System.Windows.Forms.RichTextBox();
            this.StopButton = new System.Windows.Forms.Button();
            this.Slew = new System.Windows.Forms.GroupBox();
            this.SlewSendButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SlewAlt = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SlewAzi = new System.Windows.Forms.RichTextBox();
            this.LogDataButton = new System.Windows.Forms.Button();
            this.LogAnglesTimer = new System.Windows.Forms.Timer(this.components);
            this.OpenImgButton = new System.Windows.Forms.Button();
            this.StarCam = new System.Windows.Forms.GroupBox();
            this.StarIDButton = new System.Windows.Forms.Button();
            this.CentroidButton = new System.Windows.Forms.Button();
            this.GroupButton = new System.Windows.Forms.Button();
            this.threshold = new System.Windows.Forms.RichTextBox();
            this.ThresholdButton = new System.Windows.Forms.Button();
            this.StarCamDisplay = new System.Windows.Forms.PictureBox();
            this.sp_inclinometer = new System.IO.Ports.SerialPort(this.components);
            this.rtb_xDeg = new System.Windows.Forms.RichTextBox();
            this.btn_ReadInclinometer = new System.Windows.Forms.Button();
            this.rtb_yDeg = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.GoTo.SuspendLayout();
            this.Slew.SuspendLayout();
            this.StarCam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StarCamDisplay)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GoToAlt
            // 
            this.GoToAlt.Location = new System.Drawing.Point(80, 24);
            this.GoToAlt.Name = "GoToAlt";
            this.GoToAlt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.GoToAlt.Size = new System.Drawing.Size(69, 24);
            this.GoToAlt.TabIndex = 0;
            this.GoToAlt.Text = "";
            // 
            // GoToAzi
            // 
            this.GoToAzi.Location = new System.Drawing.Point(80, 54);
            this.GoToAzi.Name = "GoToAzi";
            this.GoToAzi.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.GoToAzi.Size = new System.Drawing.Size(69, 24);
            this.GoToAzi.TabIndex = 1;
            this.GoToAzi.Text = "";
            // 
            // Alt
            // 
            this.Alt.AutoSize = true;
            this.Alt.Location = new System.Drawing.Point(15, 27);
            this.Alt.Name = "Alt";
            this.Alt.Size = new System.Drawing.Size(62, 17);
            this.Alt.TabIndex = 2;
            this.Alt.Text = "Alt (deg)";
            // 
            // Azi
            // 
            this.Azi.AutoSize = true;
            this.Azi.Location = new System.Drawing.Point(15, 57);
            this.Azi.Name = "Azi";
            this.Azi.Size = new System.Drawing.Size(65, 17);
            this.Azi.TabIndex = 3;
            this.Azi.Text = "Azi (deg)";
            // 
            // GoTo
            // 
            this.GoTo.Controls.Add(this.ReadButton);
            this.GoTo.Controls.Add(this.label1);
            this.GoTo.Controls.Add(this.label2);
            this.GoTo.Controls.Add(this.AziRead);
            this.GoTo.Controls.Add(this.AltRead);
            this.GoTo.Controls.Add(this.GoToSendButton);
            this.GoTo.Controls.Add(this.Azi);
            this.GoTo.Controls.Add(this.Alt);
            this.GoTo.Controls.Add(this.GoToAzi);
            this.GoTo.Controls.Add(this.GoToAlt);
            this.GoTo.Location = new System.Drawing.Point(11, 10);
            this.GoTo.Name = "GoTo";
            this.GoTo.Size = new System.Drawing.Size(226, 167);
            this.GoTo.TabIndex = 4;
            this.GoTo.TabStop = false;
            this.GoTo.Text = "GoTo";
            // 
            // ReadButton
            // 
            this.ReadButton.Location = new System.Drawing.Point(161, 105);
            this.ReadButton.Name = "ReadButton";
            this.ReadButton.Size = new System.Drawing.Size(57, 28);
            this.ReadButton.TabIndex = 9;
            this.ReadButton.Text = "Read";
            this.ReadButton.UseVisualStyleBackColor = true;
            this.ReadButton.Click += new System.EventHandler(this.ReadButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Azi (deg)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Alt (deg)";
            // 
            // AziRead
            // 
            this.AziRead.Location = new System.Drawing.Point(81, 124);
            this.AziRead.Name = "AziRead";
            this.AziRead.ReadOnly = true;
            this.AziRead.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.AziRead.Size = new System.Drawing.Size(69, 24);
            this.AziRead.TabIndex = 6;
            this.AziRead.Text = "";
            // 
            // AltRead
            // 
            this.AltRead.Location = new System.Drawing.Point(81, 94);
            this.AltRead.Name = "AltRead";
            this.AltRead.ReadOnly = true;
            this.AltRead.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.AltRead.Size = new System.Drawing.Size(69, 24);
            this.AltRead.TabIndex = 5;
            this.AltRead.Text = "";
            // 
            // GoToSendButton
            // 
            this.GoToSendButton.Location = new System.Drawing.Point(160, 35);
            this.GoToSendButton.Name = "GoToSendButton";
            this.GoToSendButton.Size = new System.Drawing.Size(57, 28);
            this.GoToSendButton.TabIndex = 4;
            this.GoToSendButton.Text = "Send";
            this.GoToSendButton.UseVisualStyleBackColor = true;
            this.GoToSendButton.Click += new System.EventHandler(this.Send_Click);
            // 
            // SessionLog
            // 
            this.SessionLog.Location = new System.Drawing.Point(11, 705);
            this.SessionLog.Name = "SessionLog";
            this.SessionLog.Size = new System.Drawing.Size(1013, 149);
            this.SessionLog.TabIndex = 5;
            this.SessionLog.Text = "";
            // 
            // StopButton
            // 
            this.StopButton.Location = new System.Drawing.Point(927, 30);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(76, 24);
            this.StopButton.TabIndex = 6;
            this.StopButton.Text = "STOP!!!";
            this.StopButton.UseMnemonic = false;
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // Slew
            // 
            this.Slew.Controls.Add(this.SlewSendButton);
            this.Slew.Controls.Add(this.label3);
            this.Slew.Controls.Add(this.SlewAlt);
            this.Slew.Controls.Add(this.label4);
            this.Slew.Controls.Add(this.SlewAzi);
            this.Slew.Location = new System.Drawing.Point(243, 10);
            this.Slew.Name = "Slew";
            this.Slew.Size = new System.Drawing.Size(251, 98);
            this.Slew.TabIndex = 7;
            this.Slew.TabStop = false;
            this.Slew.Text = "Slew";
            // 
            // SlewSendButton
            // 
            this.SlewSendButton.Location = new System.Drawing.Point(181, 35);
            this.SlewSendButton.Name = "SlewSendButton";
            this.SlewSendButton.Size = new System.Drawing.Size(57, 28);
            this.SlewSendButton.TabIndex = 14;
            this.SlewSendButton.Text = "Send";
            this.SlewSendButton.UseVisualStyleBackColor = true;
            this.SlewSendButton.Click += new System.EventHandler(this.SlewSendButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Azi (deg/s)";
            // 
            // SlewAlt
            // 
            this.SlewAlt.Location = new System.Drawing.Point(101, 24);
            this.SlewAlt.Name = "SlewAlt";
            this.SlewAlt.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.SlewAlt.Size = new System.Drawing.Size(69, 24);
            this.SlewAlt.TabIndex = 10;
            this.SlewAlt.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "Alt (deg/s)";
            // 
            // SlewAzi
            // 
            this.SlewAzi.Location = new System.Drawing.Point(101, 54);
            this.SlewAzi.Name = "SlewAzi";
            this.SlewAzi.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.SlewAzi.Size = new System.Drawing.Size(69, 24);
            this.SlewAzi.TabIndex = 11;
            this.SlewAzi.Text = "";
            // 
            // LogDataButton
            // 
            this.LogDataButton.Location = new System.Drawing.Point(927, 61);
            this.LogDataButton.Name = "LogDataButton";
            this.LogDataButton.Size = new System.Drawing.Size(76, 28);
            this.LogDataButton.TabIndex = 8;
            this.LogDataButton.Text = "Start Log";
            this.LogDataButton.UseVisualStyleBackColor = true;
            this.LogDataButton.Click += new System.EventHandler(this.LogDataButton_Click);
            // 
            // LogAnglesTimer
            // 
            this.LogAnglesTimer.Interval = 1000;
            this.LogAnglesTimer.Tick += new System.EventHandler(this.LogAnglesTimer_Tick);
            // 
            // OpenImgButton
            // 
            this.OpenImgButton.Location = new System.Drawing.Point(667, 30);
            this.OpenImgButton.Name = "OpenImgButton";
            this.OpenImgButton.Size = new System.Drawing.Size(85, 25);
            this.OpenImgButton.TabIndex = 10;
            this.OpenImgButton.Text = "Open Img";
            this.OpenImgButton.UseVisualStyleBackColor = true;
            this.OpenImgButton.Click += new System.EventHandler(this.OpenImgButton_Click);
            // 
            // StarCam
            // 
            this.StarCam.Controls.Add(this.StarIDButton);
            this.StarCam.Controls.Add(this.CentroidButton);
            this.StarCam.Controls.Add(this.GroupButton);
            this.StarCam.Controls.Add(this.threshold);
            this.StarCam.Controls.Add(this.ThresholdButton);
            this.StarCam.Controls.Add(this.StarCamDisplay);
            this.StarCam.Controls.Add(this.OpenImgButton);
            this.StarCam.Location = new System.Drawing.Point(12, 183);
            this.StarCam.Name = "StarCam";
            this.StarCam.Size = new System.Drawing.Size(1012, 516);
            this.StarCam.TabIndex = 11;
            this.StarCam.TabStop = false;
            this.StarCam.Text = "Star Cam";
            // 
            // StarIDButton
            // 
            this.StarIDButton.Enabled = false;
            this.StarIDButton.Location = new System.Drawing.Point(667, 184);
            this.StarIDButton.Name = "StarIDButton";
            this.StarIDButton.Size = new System.Drawing.Size(140, 25);
            this.StarIDButton.TabIndex = 15;
            this.StarIDButton.Text = "Identify Stars";
            this.StarIDButton.UseVisualStyleBackColor = true;
            this.StarIDButton.Click += new System.EventHandler(this.StarIDButton_Click);
            // 
            // CentroidButton
            // 
            this.CentroidButton.Enabled = false;
            this.CentroidButton.Location = new System.Drawing.Point(667, 123);
            this.CentroidButton.Name = "CentroidButton";
            this.CentroidButton.Size = new System.Drawing.Size(85, 25);
            this.CentroidButton.TabIndex = 14;
            this.CentroidButton.Text = "Centroid ";
            this.CentroidButton.UseVisualStyleBackColor = true;
            this.CentroidButton.Click += new System.EventHandler(this.CentroidButton_Click);
            // 
            // GroupButton
            // 
            this.GroupButton.Enabled = false;
            this.GroupButton.Location = new System.Drawing.Point(667, 92);
            this.GroupButton.Name = "GroupButton";
            this.GroupButton.Size = new System.Drawing.Size(85, 25);
            this.GroupButton.TabIndex = 13;
            this.GroupButton.Text = "Group";
            this.GroupButton.UseVisualStyleBackColor = true;
            this.GroupButton.Click += new System.EventHandler(this.GroupButton_Click);
            // 
            // threshold
            // 
            this.threshold.Location = new System.Drawing.Point(758, 61);
            this.threshold.Name = "threshold";
            this.threshold.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.threshold.Size = new System.Drawing.Size(69, 24);
            this.threshold.TabIndex = 10;
            this.threshold.Text = "";
            // 
            // ThresholdButton
            // 
            this.ThresholdButton.Enabled = false;
            this.ThresholdButton.Location = new System.Drawing.Point(667, 61);
            this.ThresholdButton.Name = "ThresholdButton";
            this.ThresholdButton.Size = new System.Drawing.Size(85, 25);
            this.ThresholdButton.TabIndex = 12;
            this.ThresholdButton.Text = "Threshold";
            this.ThresholdButton.UseVisualStyleBackColor = true;
            this.ThresholdButton.Click += new System.EventHandler(this.ThresholdButton_Click);
            // 
            // StarCamDisplay
            // 
            this.StarCamDisplay.Location = new System.Drawing.Point(13, 23);
            this.StarCamDisplay.Name = "StarCamDisplay";
            this.StarCamDisplay.Size = new System.Drawing.Size(644, 466);
            this.StarCamDisplay.TabIndex = 11;
            this.StarCamDisplay.TabStop = false;
            // 
            // rtb_xDeg
            // 
            this.rtb_xDeg.Location = new System.Drawing.Point(13, 20);
            this.rtb_xDeg.Name = "rtb_xDeg";
            this.rtb_xDeg.ReadOnly = true;
            this.rtb_xDeg.Size = new System.Drawing.Size(57, 24);
            this.rtb_xDeg.TabIndex = 12;
            this.rtb_xDeg.Text = "";
            // 
            // btn_ReadInclinometer
            // 
            this.btn_ReadInclinometer.Location = new System.Drawing.Point(135, 32);
            this.btn_ReadInclinometer.Name = "btn_ReadInclinometer";
            this.btn_ReadInclinometer.Size = new System.Drawing.Size(57, 28);
            this.btn_ReadInclinometer.TabIndex = 10;
            this.btn_ReadInclinometer.Text = "Read";
            this.btn_ReadInclinometer.UseVisualStyleBackColor = true;
            this.btn_ReadInclinometer.Click += new System.EventHandler(this.btn_ReadInclinometer_Click);
            // 
            // rtb_yDeg
            // 
            this.rtb_yDeg.Location = new System.Drawing.Point(13, 51);
            this.rtb_yDeg.Name = "rtb_yDeg";
            this.rtb_yDeg.ReadOnly = true;
            this.rtb_yDeg.Size = new System.Drawing.Size(57, 24);
            this.rtb_yDeg.TabIndex = 14;
            this.rtb_yDeg.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.rtb_yDeg);
            this.groupBox1.Controls.Add(this.btn_ReadInclinometer);
            this.groupBox1.Controls.Add(this.rtb_xDeg);
            this.groupBox1.Location = new System.Drawing.Point(539, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 133);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Inclinometer";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(76, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "Y (deg)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(76, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "X (deg)";
            // 
            // TestCodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 866);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.StarCam);
            this.Controls.Add(this.LogDataButton);
            this.Controls.Add(this.Slew);
            this.Controls.Add(this.StopButton);
            this.Controls.Add(this.SessionLog);
            this.Controls.Add(this.GoTo);
            this.Name = "TestCodeForm";
            this.Text = "TestCode";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TestCodeForm_FormClosing);
            this.GoTo.ResumeLayout(false);
            this.GoTo.PerformLayout();
            this.Slew.ResumeLayout(false);
            this.Slew.PerformLayout();
            this.StarCam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.StarCamDisplay)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox GoToAlt;
        private System.Windows.Forms.RichTextBox GoToAzi;
        private System.Windows.Forms.Label Alt;
        private System.Windows.Forms.Label Azi;
        private System.Windows.Forms.GroupBox GoTo;
        private System.Windows.Forms.Button GoToSendButton;
        private System.Windows.Forms.RichTextBox SessionLog;
        private System.Windows.Forms.Button ReadButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox AziRead;
        private System.Windows.Forms.RichTextBox AltRead;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.GroupBox Slew;
        private System.Windows.Forms.Button SlewSendButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox SlewAlt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox SlewAzi;
        private System.Windows.Forms.Button LogDataButton;
        private System.Windows.Forms.Timer LogAnglesTimer;
        private System.Windows.Forms.Button OpenImgButton;
        private System.Windows.Forms.GroupBox StarCam;
        private System.Windows.Forms.PictureBox StarCamDisplay;
        private System.Windows.Forms.Button ThresholdButton;
        private System.Windows.Forms.RichTextBox threshold;
        private System.Windows.Forms.Button GroupButton;
        private System.Windows.Forms.Button CentroidButton;
        private System.Windows.Forms.Button StarIDButton;
        private System.IO.Ports.SerialPort sp_inclinometer;
        private System.Windows.Forms.RichTextBox rtb_xDeg;
        private System.Windows.Forms.Button btn_ReadInclinometer;
        private System.Windows.Forms.RichTextBox rtb_yDeg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
    }
}