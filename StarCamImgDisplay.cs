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
    public partial class StarCamImgDisplay : Form // This form is for displaying star camera images.
    {
        private LogFileWriter lfw = null;
        private PgmImage pgm; // Image passed in from star camera.
        private Bitmap StarCamDispMap; // Bitmap for graphics display.
        private StarImg StarCamImg;
        public dPrintToLog PrintToLogInvoke;
        public delegate void dPrintToLog(string _msg);
        private Bitmap bmpImgCloseUp;
        private int zoom = 20;
        private double ratio = 255.0 / 4096.0;
        private Pixels[,] m_pCloseUp = null;
        private int[] LOS_XY;
        //private DenseVector LOStel_ST;
        private MainForm MainFormRef = null;
        private bool FeedbackEnabled;

        private class Pixels
        {
            public int x = 0, y = 0, mag = 0;
        }

        public StarCamImgDisplay(MainForm _MainForm, StarImg Img, int[] _LOS_XY, bool _FeedbackEnabled, LogFileWriter LogFileWriter1)
        {
            InitializeComponent();
            lfw = LogFileWriter1;
            MainFormRef = _MainForm;
            PrintToLogInvoke = PrintToLog;
            pgm = Img.GetPgm();
            StarCamImg = Img;
            LOS_XY = _LOS_XY;
            FeedbackEnabled = _FeedbackEnabled;
        }

        void PrintToLog(string _str)
        {
            rtb_Log.AppendText(_str + "\n");
            rtb_Log.ScrollToCaret();
            lfw.WriteLogLine("Star Cam Display Form: " + _str);
        }

        void PrintToLogi(string _str)
        {
            this.Invoke(PrintToLogInvoke, _str);
        }

        private void StarCamImgDisplay_Load(object sender, EventArgs e)
        {
            StarCamDisplay.Width = pgm.width / 2; // resolution for display is reduced by factor of 2 per axis
            StarCamDisplay.Height = pgm.height / 2;

            lbl_LOS.Text = "X: " + LOS_XY[0] + ", Y: " + LOS_XY[1];

            StarCamDispMap = new Bitmap(StarCamDisplay.Width, StarCamDisplay.Height);
            bmpImgCloseUp = new Bitmap(ImgBoxCloseUp.Width, ImgBoxCloseUp.Height);

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
            ShowStars(g);
            DisplayCloseUpView(LOS_XY[0] / 2, LOS_XY[1] / 2);
            if (FeedbackEnabled)
                PrintToLog("Automated feedback enabled.");
        }

        private void ShowStars(Graphics g)
        {
            List<StarImg.OneStar> stars = StarCamImg.GetStars();
            int idxBrightest = StarCamImg.GetBrightestStarIdx();
            int count = 0;
            //foreach (StarImg.OneStar star in stars)
            for (int i = 0; i < stars.Count; i++)
            {
                StarImg.OneStar star = stars[i];
                if (star.IDsuccess == 1)
                {
                    DrawCircle(g, Color.ForestGreen, star.x, star.y);
                    count++;
                }
                else
                    DrawCircle(g, Color.Crimson, star.x, star.y);
                if (i == idxBrightest)
                    DrawCircle(g, Color.Yellow, star.x, star.y);
            }
            if (FeedbackEnabled == false)
            {
                PrintToLog("Identified " + count + " stars.");
                double rmse_arcs = StarCamImg.GetRMSE() * HSYMath.RTAS;
                PrintToLog("RMSE: " + rmse_arcs.ToString("N2") + " arcseconds");
                double maxScore = StarCamImg.GetMaxScore();
                PrintToLog("Max score: " + maxScore.ToString("N2"));
            }
        }

        private void DrawPlus(Graphics g, Color c, double x, double y)
        {
            Pen pen = new Pen(c, 2);
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

        private void DrawCircle(Graphics g, Color c, double x, double y)
        {
            Pen pen = new Pen(c, 2);
            float d = 10f;
            float x1 = (float)x / 2 - d * 0.5f;
            float y1 = (float)y / 2 - d * 0.5f;
            g.DrawArc(pen, x1, y1, d, d, 0, 360);
            return;
        }

        private void StarCamDisplay_MouseDown(object sender, MouseEventArgs e)
        {
            DisplayCloseUpView(e.X, e.Y);
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

                    if ((x < 0) || (y < 0) || (x >= pgm.width) || (y >= pgm.height))
                        continue;

                    int ipixel = (int)(ratio * (double)pgm.pixels[y][x]);
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
                            m_pCloseUp[xx, yy].mag = pgm.pixels[y][x];
                        }
                    }
                }
            }
            ImgBoxCloseUp.Image = bmpImgCloseUp;
        }

        private void ImgBoxCloseUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_pCloseUp == null)
                return;
            int i = e.X;
            int j = e.Y;

            lbl_LOS.Text = "X: " + m_pCloseUp[i, j].x + ", Y: " + m_pCloseUp[i, j].y;
            if (StarCamImg == null)
                return;
            DenseVector LOStel_ST = StarCamImg.CalcXYZ(m_pCloseUp[i, j].x, m_pCloseUp[i, j].y);
            int[] LOStel_XY = new int[2];
            LOStel_XY[0] = m_pCloseUp[i, j].x;
            LOStel_XY[1] = m_pCloseUp[i, j].y;
            MainFormRef.SetLOS_ST(LOStel_ST, LOStel_XY);
            PrintToLog("LOS set to X: " + m_pCloseUp[i, j].x + ", Y: " + m_pCloseUp[i, j].y);
        }

        private void btn_dqToBrightest_Click(object sender, EventArgs e)
        {
            MainFormRef.dqToBrightest(StarCamImg);
        }
    }
}
