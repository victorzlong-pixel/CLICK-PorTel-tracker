namespace GS_Tracking_KR
{
    partial class StarCamImgDisplay
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
            this.StarCamDisplay = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ImgBoxCloseUp = new System.Windows.Forms.PictureBox();
            this.lbl_LOS = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_dqToBrightest = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.StarCamDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImgBoxCloseUp)).BeginInit();
            this.SuspendLayout();
            // 
            // StarCamDisplay
            // 
            this.StarCamDisplay.Location = new System.Drawing.Point(15, 35);
            this.StarCamDisplay.Name = "StarCamDisplay";
            this.StarCamDisplay.Size = new System.Drawing.Size(652, 478);
            this.StarCamDisplay.TabIndex = 0;
            this.StarCamDisplay.TabStop = false;
            this.StarCamDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.StarCamDisplay_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(159, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Star Camera Full Image:";
            // 
            // rtb_Log
            // 
            this.rtb_Log.Location = new System.Drawing.Point(444, 628);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(362, 212);
            this.rtb_Log.TabIndex = 4;
            this.rtb_Log.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(440, 608);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Session Log";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 540);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 17);
            this.label3.TabIndex = 15;
            this.label3.Text = "Close Up (click to set LOS):";
            // 
            // ImgBoxCloseUp
            // 
            this.ImgBoxCloseUp.Location = new System.Drawing.Point(14, 560);
            this.ImgBoxCloseUp.Name = "ImgBoxCloseUp";
            this.ImgBoxCloseUp.Size = new System.Drawing.Size(420, 280);
            this.ImgBoxCloseUp.TabIndex = 14;
            this.ImgBoxCloseUp.TabStop = false;
            this.ImgBoxCloseUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImgBoxCloseUp_MouseDown);
            // 
            // lbl_LOS
            // 
            this.lbl_LOS.AutoSize = true;
            this.lbl_LOS.Location = new System.Drawing.Point(441, 577);
            this.lbl_LOS.Name = "lbl_LOS";
            this.lbl_LOS.Size = new System.Drawing.Size(98, 17);
            this.lbl_LOS.TabIndex = 16;
            this.lbl_LOS.Text = "X: 640, Y: 480";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(440, 560);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 17);
            this.label4.TabIndex = 17;
            this.label4.Text = "LOS :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 516);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(400, 17);
            this.label5.TabIndex = 18;
            this.label5.Text = "Green = ID success, Red = ID failure, Yellow = Brightest object";
            // 
            // btn_dqToBrightest
            // 
            this.btn_dqToBrightest.Location = new System.Drawing.Point(687, 35);
            this.btn_dqToBrightest.Name = "btn_dqToBrightest";
            this.btn_dqToBrightest.Size = new System.Drawing.Size(114, 25);
            this.btn_dqToBrightest.TabIndex = 23;
            this.btn_dqToBrightest.Text = "dq to Brightest";
            this.btn_dqToBrightest.UseVisualStyleBackColor = true;
            this.btn_dqToBrightest.Click += new System.EventHandler(this.btn_dqToBrightest_Click);
            // 
            // StarCamImgDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 880);
            this.Controls.Add(this.btn_dqToBrightest);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbl_LOS);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ImgBoxCloseUp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rtb_Log);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StarCamDisplay);
            this.Location = new System.Drawing.Point(1062, 0);
            this.Name = "StarCamImgDisplay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "StarCamImgDisplay";
            this.Load += new System.EventHandler(this.StarCamImgDisplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.StarCamDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImgBoxCloseUp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox StarCamDisplay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox ImgBoxCloseUp;
        private System.Windows.Forms.Label lbl_LOS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_dqToBrightest;
    }
}