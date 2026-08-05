using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;
using HSYLib.CS;

namespace GS_Tracking_KR
{
    class StarID
    {
        public static double OneStarSTD = 30; // standard deviation of star in arcsec
        public static double TwoStarLengthSTD = 0.02; // standard deviation of pair length in degrees
        public static int maxStars = 15; // maximum stars to ID in an image
        public static int magStarsSelect = 10; // number of stars to select based on magnitude

        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExternInitializeStarID();

        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExternPerformStarID(int ImgStarN, double[] ImgStarVec, double Star_Std_arcsec,
            double TwoStarLength_Std_deg, int[] IDSuc, int[] ImgStarIndex_SKY2000Trimmed, double[] ImgStarScore, byte[] msg);

        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExternPerformStarIDwithPrior(int ImgStarN, double[] ImgStarVec, double Star_Std_arcsec,
            double TwoStarLength_Std_deg, double[] q_est, double AngleErrBound_deg, int[] IDSuc, int[] ImgStarIndex_SKY2000Trimmed, double[] ImgStarScore, byte[] msg);

        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ExternSimulateID();

        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ExternPerformStarIDC(byte[] fn, int ApplyFiler, double f_length, double pixelSize,
            byte[] SatID, int Threshold, int MinPixelN, int MaxPixelN, int MaxMagStarN, int MaxIMGStarN, double Star_Std_arcsec,
            double TwoStarLength_Std_deg, int[] rIDSuc, int[] rID, double[] rScore, double[] rU, double[] rV, int[] rMag, int[] rPixelN, double[] rCalPar);

        public static bool CheckForStarCatFiles()
        {
            string[] fileNames = new string[] { "StarCat.bin", "PairCat.bin", "FOICat.bin", "KVectorCat.bin", "CalDataDefault.bin" };
            for (int i = 0; i < fileNames.Length; i++)
            {
                FileInfo info = new FileInfo(fileNames[i]);
                if (info.Exists == false)
                    return false;
            }
            return true;
        }

        public static void PerformStarID(StarImg StarImg1, double[] q_est, StarCatalog StarCatalog1)
        {
            List<StarImg.OneStar> stars = StarImg1.GetStars(); // get list of stars
            int nStars = stars.Count;

            // sort stars
            double[] MagSums = new double[nStars];
            double[] r2 = new double[nStars];
            DenseVector[] xyz_unsrt = new DenseVector[nStars];
            for (int i = 0; i < nStars; i++)
            {
                xyz_unsrt[i] = DenseVector.OfVector(stars[i].XYZ_ST);
                MagSums[i] = stars[i].magSum;
                r2[i] = stars[i].XYZ_ST[0] * stars[i].XYZ_ST[0] + stars[i].XYZ_ST[1] * stars[i].XYZ_ST[1];
            }

            int[] idxMag = HSYMath.sort_merge(MagSums); // indexes from lowest mag to highest mag
            int[] idxr2 = HSYMath.sort_merge(r2); // indexes from closest to furthest from center

            int nStarstoID = nStars;
            if (nStarstoID > maxStars)
                nStarstoID = maxStars; // limit number of stars
            if (nStarstoID < magStarsSelect)
                magStarsSelect = nStarstoID;

            int[] selected = new int[nStars];
            DenseVector[] xyz_srt = new DenseVector[nStarstoID];
            int[] idxOriginal = new int[nStarstoID];
            int r2starsSelect = nStarstoID - magStarsSelect;

            // Select stars with highest magnitudes
            for (int i = 0; i < magStarsSelect; i++)
            {
                int idx = idxMag[nStars - 1 - i]; // index of next highest mag star
                xyz_srt[i] = xyz_unsrt[idx];
                selected[idx] = 1;
                idxOriginal[i] = idx;
            }

            // Select stars with largest r2
            int n = 0;
            for (int i = 0; i < nStars; i++)
            {
                if (n == r2starsSelect)
                    break;
                if (selected[idxr2[i]] != 0) // if already selected by magnitude, skip
                    continue;
                xyz_srt[magStarsSelect + n] = xyz_unsrt[idxr2[i]];
                selected[idxr2[i]] = 1;
                idxOriginal[magStarsSelect + n] = idxr2[i];
                n++;
            }

            double[] ImgStarV = new double[3 * nStarstoID];
            for (int i = 0; i < nStarstoID; i++)
                for (int j = 0; j < 3; j++)
                    ImgStarV[3 * i + j] = xyz_srt[i][j];

            byte[] msg = new byte[100];
            int[] ID_srt = new int[nStarstoID];
            int[] IDsuccess_srt = new int[nStarstoID];
            double[] score_srt = new double[nStarstoID];

            if (q_est == null)
                ExternPerformStarID(nStarstoID, ImgStarV, OneStarSTD, TwoStarLengthSTD, IDsuccess_srt, ID_srt, score_srt, msg);
            else
                ExternPerformStarIDwithPrior(nStarstoID, ImgStarV, OneStarSTD, TwoStarLengthSTD, q_est, 1, IDsuccess_srt, ID_srt, score_srt, msg);

            // Get original indexes of ID'd stars
            int[] ID = new int[nStars];
            int[] IDsuccess = new int[nStars];
            double[] score = new double[nStars];
            for (int i = 0; i < nStars; i++)
                ID[i] = -100;
            for (int i = 0; i < nStarstoID; i++)
            {
                IDsuccess[idxOriginal[i]] = IDsuccess_srt[i];
                ID[idxOriginal[i]] = ID_srt[i];
                score[idxOriginal[i]] = score_srt[i];
            }

            StarImg1.SetStarsID(ID, IDsuccess, score);
            EstimateQuat(StarImg1, StarCatalog1);
        }

        private static void EstimateQuat(StarImg StarImg1, StarCatalog StarCatalog1)
        {
            Quaternion q_est;
            double RMSE = 0;
            List<StarImg.OneStar> stars = StarImg1.GetStars();
            StarCatalog.OneStarData[] redCatStars = StarCatalog1.redCatStars;
            StarCatalog.OneStarData[] masterCatStars = StarCatalog1.masterCatStars;

            List<DenseVector> R_stars_J2K = new List<DenseVector>(); // observed stars in J2K frame
            List<DenseVector> R_stars_ST = new List<DenseVector>(); // observed stars in camera frame
            List<double> w = new List<double>(); // weighting of measurements

            for (int i = 0; i < stars.Count; i++)
            {
                StarCatalog.OneStarData OneStarData1;
                if (stars[i].IDsuccess != 1)
                    OneStarData1 = new StarCatalog.OneStarData();
                else
                {
                    int starCatID = stars[i].RedStarCatID;
                    OneStarData1 = redCatStars[starCatID];
                    R_stars_ST.Add(stars[i].XYZ_ST);
                    R_stars_J2K.Add(OneStarData1.XYZj2k);
                    w.Add(1);
                }
            }
            if (R_stars_J2K.Count > 2)
                q_est = HSYMath.QUEST(R_stars_J2K.Count, R_stars_ST.ToArray(), R_stars_J2K.ToArray(), w.ToArray());
            else
                q_est = new Quaternion();

            // Calculate RMSE
            DenseMatrix dcm_est = HSYMath.quaternion_to_DCM(q_est);
            for (int i = 0; i < R_stars_ST.Count; i++)
            {
                DenseVector R_star_ST_est = dcm_est * R_stars_J2K[i];
                DenseVector delta_R = R_stars_ST[i] - R_star_ST_est;
                double err = delta_R.L2Norm();
                RMSE += err * err;
            }
            RMSE = Math.Sqrt(RMSE / (double) R_stars_ST.Count);

            StarImg1.SetQest(q_est, RMSE);
        }
    }
}
