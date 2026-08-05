namespace GS_Tracking_KR
{
    partial class AlignCamerasForm
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
            this.ImgBoxCloseUp = new System.Windows.Forms.PictureBox();
            this.ImgBox = new System.Windows.Forms.PictureBox();
            this.rtb_Log = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_LOS = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImgBoxCloseUp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImgBox)).BeginInit();
            this.SuspendLayout();
            // 
            // ImgBoxCloseUp
            // 
            this.ImgBoxCloseUp.Location = new System.Drawing.Point(664, 29);
            this.ImgBoxCloseUp.Name = "ImgBoxCloseUp";
            this.ImgBoxCloseUp.Size = new System.Drawing.Size(420, 280);
            this.ImgBoxCloseUp.TabIndex = 8;
            this.ImgBoxCloseUp.TabStop = false;
            this.ImgBoxCloseUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImgBoxCloseUp_MouseDown);
            // 
            // ImgBox
            // 
            this.ImgBox.Location = new System.Drawing.Point(12, 29);
            this.ImgBox.Name = "ImgBox";
            this.ImgBox.Size = new System.Drawing.Size(626, 473);
            this.ImgBox.TabIndex = 7;
            this.ImgBox.TabStop = false;
            this.ImgBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImgBox_MouseDown);
            // 
            // rtb_Log
            // 
            this.rtb_Log.Location = new System.Drawing.Point(664, 401);
            this.rtb_Log.Name = "rtb_Log";
            this.rtb_Log.ReadOnly = true;
            this.rtb_Log.Size = new System.Drawing.Size(420, 99);
            this.rtb_Log.TabIndex = 6;
            this.rtb_Log.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(698, 312);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 17);
            this.label1.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(667, 319);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "LOS";
            // 
            // lbl_LOS
            // 
            this.lbl_LOS.AutoSize = true;
            this.lbl_LOS.Location = new System.Drawing.Point(667, 336);
            this.lbl_LOS.Name = "lbl_LOS";
            this.lbl_LOS.Size = new System.Drawing.Size(98, 17);
            this.lbl_LOS.TabIndex = 11;
            this.lbl_LOS.Text = "X: 640, Y: 480";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 17);
            this.label4.TabIndex = 12;
            this.label4.Text = "Star Cam Image:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(661, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Close Up (click to set LOS):";
            // 
            // AlignCamerasForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1120, 532);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbl_LOS);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ImgBoxCloseUp);
            this.Controls.Add(this.ImgBox);
            this.Controls.Add(this.rtb_Log);
            this.Name = "AlignCamerasForm";
            this.Text = "AlignCamerasForm";
            ((System.ComponentModel.ISupportInitialize)(this.ImgBoxCloseUp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImgBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImgBoxCloseUp;
        private System.Windows.Forms.PictureBox ImgBox;
        private System.Windows.Forms.RichTextBox rtb_Log;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_LOS;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}