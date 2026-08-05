using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;
using HSYLib.CS;

namespace GS_Tracking_KR
{
    public class StarImg
    /* This class handles a star image and implements thresholding, blob identification, and
    centroiding. This class includes subclasses of OnePixel and OneStar, corresponding to each 
    pixel and each blob. */
    {
        private PgmImage pgm = null; // image within the StarImg object
        private List<OnePixel> threshPix = null; // list of pixels that meet the specified threshold
        private List<OneStar> stars = null; // list of stars in image
        private int idxBrightestStar = -1;
        private int cnt;
        private int nStarsID;
        private double maxScore;
        private Quaternion q_est;
        private double RMSE;
        private DenseMatrix CalMatrix;

        public double focalLength = 35e-3;
        public double pixelPitch = 3.75e-6;
        public double threshMultiplier = 3;

        public StarImg(PgmImage img, int minPix, int maxPix, ushort thresh)
        {
            pgm = img;
            ReadCalMatrix();
            Threshold(thresh);
            Group(minPix, maxPix);
            Centroid();
            Brightest();
        }

        public StarImg(PgmImage img, int minPix, int maxPix)
        {
            pgm = img;
            ReadCalMatrix();
            Threshold();
            Group(minPix, maxPix);
            Centroid();
            Brightest();
        }

        public class OnePixel // Defines a single pixel by location and value.
        {
            public int y; // pixel row
            public int x; // pixel col
            public double mag; // magnitude
            public int groupIndex; // group that pixel is in
        }

        public class OneStar // Defines a star as a grouping of pixels above a given threshold.
        {
            public int index; // group number in image
            public List<OnePixel> starPix = new List<OnePixel>();
            public double y; // centroid location
            public double x;
            public double magSum; // magnitude summed across all pixels
            public double maxMag; // maximum magnitude of all pixels
            public DenseVector XYZ_ST;
            public int RedStarCatID;
            public int IDsuccess;
            public double score;
        }

        public PgmImage GetPgm() // Returns image associated with StarImg object.
        {
            return pgm;
        }

        public List<OnePixel> GetThreshPixels()
        {
            return threshPix;
        }

        public List<OneStar> GetStars()
        {
            return stars;
        }

        public int GetStarCount()
        {
            return cnt;
        }

        public int GetNstarsID()
        {
            return nStarsID;
        }

        public double GetMaxScore()
        {
            return maxScore;
        }

        public double[] GetWidthHeight()
        {
            double[] wh = new double[2];
            wh[0] = pgm.width;
            wh[1] = pgm.height;
            return wh;
        }

        public Quaternion GetQest()
        {
            return q_est;
        }

        public double GetRMSE()
        {
            return RMSE;
        }

        public DenseVector GetBrightestStarVector()
        {
            if (idxBrightestStar < 0)
                return null;
            OneStar star = stars[idxBrightestStar];
            return star.XYZ_ST;
        }

        public int GetBrightestStarIdx()
        {
            return idxBrightestStar;
        }

        public double GetBrightestStarMagSum()
        {
            if (idxBrightestStar < 0)
                return -1;
            OneStar star = stars[idxBrightestStar];
            return star.magSum;
        }

        public void SetStarsID(int[] ID, int[] IDsuccess, double[] score)
        {
            maxScore = 0;
            nStarsID = 0;
            for (int i = 0; i < stars.Count; i++)
            {
                if (score[i] > maxScore)
                    maxScore = score[i];
                if (IDsuccess[i] == 1)
                    nStarsID++;
                stars[i].RedStarCatID = ID[i];
                stars[i].IDsuccess = IDsuccess[i];
                stars[i].score = score[i];
            }
        }

        public void SetQest(Quaternion _q_est, double _RMSE)
        {
            q_est = _q_est;
            RMSE = _RMSE;
        }

        private void ReadCalMatrix()
        {
            CalMatrix = new DenseMatrix(2, 10);
            // Try to locate camera calibration data
            string fn = "CalData35.bin";
            FileInfo info = new FileInfo(fn);
            if (info.Exists == true) // read calibration file
            {
                FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 10; j++)
                        CalMatrix[i, j] = br.ReadDouble();
                }
                br.Close();
            }
            else // if no camera calibration data exists, assume standard model
            {
                CalMatrix[0, 1] = 1.0;
                CalMatrix[1, 4] = 1.0;
            }
        }

        private void Threshold(ushort thresh) // Given a threshold, returns a list of pixels above threshold.
        {
            threshPix = new List<OnePixel>();
            for (int i = 0; i < pgm.height; i++)
            {
                for (int j = 0; j < pgm.width; j++)
                {
                    if (pgm.pixels[i][j] > thresh)
                    {
                        OnePixel pixel = new OnePixel();
                        pixel.y = i;
                        pixel.x = j;
                        pixel.mag = (double)(pgm.pixels[i][j] - thresh);
                        threshPix.Add(pixel);
                    }
                }
            }
        }

        private void Threshold() // Returns a list of pixels above a calculated threshold.
        {
            ushort thresh = CalculateThreshold();
            threshPix = new List<OnePixel>();
            for (int i = 0; i < pgm.height; i++)
            {
                for (int j = 0; j < pgm.width; j++)
                {
                    if (pgm.pixels[i][j] > thresh)
                    {
                        OnePixel pixel = new OnePixel();
                        pixel.y = i;
                        pixel.x = j;
                        pixel.mag = (double) (pgm.pixels[i][j] - thresh);
                        threshPix.Add(pixel);
                    }
                }
            }
        }

        private ushort CalculateThreshold() // Calculates threshold value for image based on n-sigma above mean.
        {
            double mean = 0;
            double sigma = 0;
            double var = 0;

            // calculate mean of pixels
            for (int i = 0; i < pgm.height; i++)
            {
                for (int j = 0; j < pgm.width; j++)
                {
                    mean += pgm.pixels[i][j];
                }
            }
            mean /= (pgm.height * pgm.width);

            // calculate variance and standard deviation of pixels
            for (int i = 0; i < pgm.height; i++)
            {
                for (int j = 0; j < pgm.width; j++)
                {
                    var += (pgm.pixels[i][j] - mean) * (pgm.pixels[i][j] - mean);
                }
            }
            var /= (pgm.height * pgm.width - 1);
            sigma = Math.Sqrt(var);

            ushort thresh = (ushort)(mean + threshMultiplier * sigma);
            return thresh;
        }

        private int Group(int minPix, int maxPix) // All groups of connected pixels above minPix and below maxPix are identified. A group count is returned.
        {
            int ymax = pgm.height;
            int xmax = pgm.width;
            int[] buf_y = new int[xmax + 2]; // buffer storing y index of previous pixel, note that index is shifted by 1
            int[] buf_data_i = new int[xmax + 2]; // maps array index to the list of pixels index, note that index is shifted by 1
            int X, Y, X_1, Y_1;
            int prev_X, prev_Y, prev_i;
            int candidate_index, next_index;
            int flag;
            int[] flag_min = new int[4];
            int[] index_table = new int[threshPix.Count];
            int[] group_index = new int[threshPix.Count]; // group index assigned to each thresholded pixel

            int[] buf_index = new int[threshPix.Count];
            int[] buf_cnt = new int[threshPix.Count];

            // initialize arrays
            for (int i = 0; i < threshPix.Count; i++)
            {
                index_table[i] = -1;
                group_index[i] = 0;
            }
            for (int i = 0; i < (xmax + 2); i++)
            {
                buf_y[i] = -3;
                buf_data_i[i] = 0;
            }
            prev_X = 0;
            prev_Y = -2;
            prev_i = 0;
            next_index = 0;

            // This for loop does initial group labelings ignoring minimum count.
            for (int i = 0; i < threshPix.Count; i++)
            {
                X = threshPix[i].x;
                Y = threshPix[i].y;
                X_1 = X - 1;
                Y_1 = Y - 1;

                candidate_index = next_index; // set candidate index for upcoming groups
                flag = 0; // reset flags
                flag_min[0] = 0; // top left is a hit
                flag_min[1] = 0; // top is a hit
                flag_min[2] = 0; // top right is a hit
                flag_min[3] = 0; // previous pixel is a hit

                if (buf_y[X] == Y_1) // if pixel top left is in buffer, flag and connect
                {
                    candidate_index = group_index[buf_data_i[X]];
                    flag = 1;
                    flag_min[0] = 1;
                }

                if (buf_y[X + 1] == Y_1) // if pixel above is in buffer, flag and connect
                {
                    flag_min[1] = 1;
                    if (candidate_index > group_index[buf_data_i[X + 1]])
                    {
                        candidate_index = group_index[buf_data_i[X + 1]];
                        flag = 1;
                    }
                }

                if (buf_y[X + 2] == Y_1) // if pixel top right is in buffer, flag and connect
                {
                    flag_min[2] = 1;
                    if (candidate_index > group_index[buf_data_i[X + 2]])
                    {
                        candidate_index = group_index[buf_data_i[X + 2]];
                        flag = 1;
                    }
                }

                if ((Math.Abs(prev_Y - Y) < 2) && (Math.Abs(prev_X - X) < 2)) // if previous pixel is adjacent, flag and connect
                {
                    flag_min[3] = 1;
                    if (candidate_index > group_index[prev_i])
                    {
                        candidate_index = group_index[prev_i];
                        flag = 1;
                    }
                }

                if (flag == 0)
                    next_index++; // if no adjacent pixels are found, increment next index of group

                group_index[i] = candidate_index; // set group index of current pixel

                for (int j = 0; j < 3; j++)
                {
                    int ind = group_index[buf_data_i[X + j]]; // pull index of top left, top, and top right pixels
                    if (flag_min[j] == 1) // if the flagged pixel has a differend index, connect it to lower index
                    {
                        if (((index_table[ind] == -1) && (ind != candidate_index)) || (index_table[ind] > candidate_index))
                            index_table[ind] = candidate_index;
                        group_index[buf_data_i[X + j]] = candidate_index;
                    }
                }

                if (flag_min[3] == 1)
                {
                    int ind = group_index[prev_i]; // get index of previous pixel
                    if (((index_table[ind] == -1) && (ind != candidate_index)) || (index_table[ind] > candidate_index))
                        index_table[ind] = candidate_index;
                    group_index[prev_i] = candidate_index;
                }

                buf_y[prev_X + 1] = prev_Y; // update buffer of y coordinate
                buf_data_i[prev_X + 1] = prev_i; // update map of array -> list transformation
                prev_X = X;
                prev_Y = Y;
                prev_i = i;
            }

            if (next_index >= threshPix.Count) // something has gone terribly wrong
                return -1;

            for (int i = 0; i < next_index; i++)
            {
                if (index_table[i] > 0)
                {
                    while (index_table[index_table[i]] > 0)
                        index_table[i] = index_table[index_table[i]];
                }
            }

            for (int i = 0; i < threshPix.Count; i++)
            {
                int old_index = group_index[i];
                if (index_table[old_index] > 0)
                {
                    group_index[i] = index_table[old_index];
                }
            }

            // initialize arrays
            for (int i = 0; i < next_index; i++)
            {
                buf_cnt[i] = 0;
                buf_index[i] = -1;
            }

            for (int i = 0; i < threshPix.Count; i++)
                buf_cnt[group_index[i]]++;

            cnt = -1; // group numbers are indexed from 0
            for (int i = 0; i < next_index; i++)
            {
                if ((buf_cnt[i] >= minPix) && (buf_cnt[i] <= maxPix))
                {
                    cnt++;
                    buf_index[i] = cnt;
                }
            }
            cnt++; // returned value is indexed from 1

            // eliminate the indices of groups under minimum pixel requirement
            for (int i = 0; i < threshPix.Count; i++)
                group_index[i] = buf_index[group_index[i]];

            for (int i = 0; i < threshPix.Count; i++)
                threshPix[i].groupIndex = group_index[i];

            return cnt;
        }

        private void Centroid() // Returns list of stars centroided by a center of mass calculation.
        {
            stars = new List<OneStar>();

            double[] mag = new double[cnt];
            double[] xm = new double[cnt];
            double[] ym = new double[cnt];

            // Create a new star for every pixel group.
            for (int i = 0; i < cnt; i++)
            {
                OneStar newStar = new OneStar();
                newStar.index = i;
                newStar.magSum = 0;
                newStar.starPix = new List<OnePixel>();
                newStar.x = 0;
                newStar.y = 0;

                stars.Add(newStar);

                xm[i] = 0;
                ym[i] = 0;
            }

            for (int i = 0; i < threshPix.Count; i++)
            {
                if ((threshPix[i].groupIndex != -1) && (threshPix[i].groupIndex < cnt))
                {
                    int index = threshPix[i].groupIndex;
                    stars[index].magSum += threshPix[i].mag;
                    xm[index] += threshPix[i].mag * (0.5 + threshPix[i].x);
                    ym[index] += threshPix[i].mag * (0.5 + threshPix[i].y);

                    stars[index].starPix.Add(threshPix[i]);
                    if (stars[index].maxMag < threshPix[i].mag)
                        stars[index].maxMag = threshPix[i].mag;
                }
            }
            for (int i = 0; i < cnt; i++)
            {
                stars[i].x = xm[i] / stars[i].magSum;
                stars[i].y = ym[i] / stars[i].magSum;
                stars[i].XYZ_ST = CalcXYZ(stars[i].x, stars[i].y);
            }
        }

        private void Brightest()
        {
            // Identify brightest star
            if (cnt > 0)
                idxBrightestStar = 0;
            for (int i = 1; i < cnt; i++)
            {
                if (stars[i].magSum > stars[idxBrightestStar].magSum)
                    idxBrightestStar = i;
            }
        }

        public DenseVector CalcXYZ(double xCentroid, double yCentroid)
        {
            double xCenter = pgm.width * 0.5;
            double yCenter = pgm.height * 0.5;
            double x = xCentroid - xCenter;
            double y = yCentroid - yCenter;
            DenseVector xyz = new DenseVector(3);
            DenseVector temp = new DenseVector(10);

            temp[0] = 10;
            temp[1] = x;
            temp[2] = x * x;
            temp[3] = x * x * x;
            temp[4] = y;
            temp[5] = y * y;
            temp[6] = y * y * y;
            temp[7] = x * y;
            temp[8] = x * x * y;
            temp[9] = x * y * y;

            DenseVector xy = CalMatrix * temp;

            xyz[0] = xy[0] * pixelPitch;
            xyz[1] = xy[1] * pixelPitch;
            xyz[2] = focalLength;
            xyz = (DenseVector)xyz.Normalize(2);
            return xyz;
        }
    }
}
