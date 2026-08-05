using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSYLib.CS;
using MathNet.Numerics.LinearAlgebra.Double;

namespace GS_Tracking_KR
{
    public partial class AlignCamerasForm : Form
    {
        public delegate void dPrintLog(string _msg);
        public dPrintLog PrintLogInvoke;
        private LogFileWriter lfw;
        private PgmImage img;
        private StarImg StarImg1;
        private Bitmap bmpImgCloseUp;
        private int zoom = 20;
        private double ratio = 255.0 / 4096.0;
        private Pixels[,] m_pCloseUp = null;
        private DenseVector LOStel_ST;

        private class Pixels
        {
            public int x = 0, y = 0, mag = 0;
        }

        public AlignCamerasForm(LogFileWriter _lfw, PgmImage _img, DenseVector _LOStel_ST)
        {
            InitializeComponent();
            PrintLogInvoke = PrintLog;
            lfw = _lfw;
            img = _img;
            LOStel_ST = _LOStel_ST;

            StarImg1 = new StarImg(img, 0, 0);

            ImgBox.Width = img.width / 2; // resolution for display is quartered
            ImgBox.Height = img.height / 2;

            Bitmap DispMap = new Bitmap(ImgBox.Width, ImgBox.Height);
            bmpImgCloseUp = new Bitmap(ImgBoxCloseUp.Width, ImgBoxCloseUp.Height);

            for (int i = 0; i < ImgBox.Height; i++)
            {
                for (int j = 0; j < ImgBox.Width; j++)
                {
                    int ipixel = 0;
                    for (int k = 0; k < 2; k++) // sum over four rows
                    {
                        for (int l = 0; l < 2; l++) // sum over four columns
                            ipixel += img.pixels[i * 2 + k][j * 2 + l]; // sum four original pixels per display pixel
                    }
                    ipixel = (int)(ratio * ipixel / 4); // scale to reduced resolution
                    if (ipixel > 255)
                        ipixel = 255;
                    DispMap.SetPixel(j, i, Color.FromArgb(255, ipixel, ipixel, ipixel)); // set pixel value
                }
            }
            ImgBox.Image = DispMap;
        }

        void PrintLog(string _str)
        {
            rtb_Log.AppendText(_str + "\n");
            rtb_Log.ScrollToCaret();
            lfw.WriteLogLine("Align Cameras Form: " + _str);
        }

        void PrintLogi(string _str)
        {
            this.Invoke(PrintLogInvoke, _str);
        }

        private void DisplayCloseUpView(int Center_x, int Center_y)
        {
            Center_x *= 2;
            Center_y *= 2;

            Graphics g1 = Graphics.FromImage(bmpImgCloseUp);
            g1.FillRectangle(new SolidBrush(Color.Black), 0, 0, bmpImgCloseUp.Width, bmpImgCloseUp.Height);

            int LX = bmpImgCloseUp.Width / zoom;
            int LY = bmpImgCloseUp.Height / zoom;

            int StartX = Center_x - LX / 2;
            if (StartX < 0) StartX = 0;
            int StartY = Center_y - LY / 2;
            if (StartY < 0) StartY = 0;

            m_pCloseUp = new Pixels[bmpImgCloseUp.Width, bmpImgCloseUp.Height];
            for (int i = 0; i < bmpImgCloseUp.Width; i++)
            {
                for (int j = 0; j < bmpImgCloseUp.Height; j++)
                {
                    m_pCloseUp[i, j] = new Pixels();
                }
            }
            for (int i = 0; i < LX; i++)
            {
                for (int j = 0; j < LY; j++)
                {
                    int x = StartX + i;
                    int y = StartY + j;

                    if ((x < 0) || (y < 0) || (x >= img.width) || (y >= img.height))
                        continue;

                    int ipixel = (int)(ratio * (double)img.pixels[y][x]);
                    if (ipixel > 255)
                        ipixel = 255;
                    for (int ii = 0; ii < zoom; ii++)
                    {
                        int xx = i * zoom + ii;
                        if ((xx < 0) || (xx >= bmpImgCloseUp.Width))
                            continue;
                        for (int jj = 0; jj < zoom; jj++)
                        {
                            int yy = j * zoom + jj;
                            if ((yy < 0) || (yy >= bmpImgCloseUp.Height))
                                continue;

                            bmpImgCloseUp.SetPixel(xx, yy, Color.FromArgb(255, ipixel, ipixel, ipixel));
                            m_pCloseUp[xx, yy].x = x;
                            m_pCloseUp[xx, yy].y = y;
                            m_pCloseUp[xx, yy].mag = img.pixels[y][x];
                        }
                    }
                }
            }
            ImgBoxCloseUp.Image = bmpImgCloseUp;
        }

        private void ImgBox_MouseDown(object sender, MouseEventArgs e)
        {
            //LOStel_ST = StarImg1.CalcXYZ(e.X * 2, e.Y * 2);
            DisplayCloseUpView(e.X, e.Y);
        }

        private void ImgBoxCloseUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_pCloseUp == null)
                return;
            int i = e.X;
            int j = e.Y;

            lbl_LOS.Text = "X: " + m_pCloseUp[i, j].x + ", Y: " + m_pCloseUp[i, j].y;
            if (StarImg1 == null)
                return;
            LOStel_ST = StarImg1.CalcXYZ(m_pCloseUp[i, j].x, m_pCloseUp[i, j].y);
            PrintLog("LOS set to X: " + m_pCloseUp[i, j].x + ", Y: " + m_pCloseUp[i, j].y);
        }
    }
}
