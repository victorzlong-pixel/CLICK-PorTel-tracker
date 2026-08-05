//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Runtime.InteropServices;

//namespace GS_Tracking_KR
//{
//    class CallStarID_C_dll
//    {
//        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void ExternInitializeStarID();

//        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void ExternPerformStarID(int ImgStarN, double[] ImgStarVec, double Star_Std_arcsec,
//    double TwoStarLength_Std_deg, int[] IDSuc, int[] ImgStarIndex_SKY2000Trimmed, double[] ImgStarScore, byte[] msg);

//        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void ExternPerformStarIDwithPrior(int ImgStarN, double[] ImgStarVec, double Star_Std_arcsec,
//    double TwoStarLength_Std_deg, double[] q_est, double AngleErrBound_deg, int[] IDSuc, int[] ImgStarIndex_SKY2000Trimmed, double[] ImgStarScore, byte[] msg);

//        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
//        public static extern void ExternSimulateID();

//        [DllImport("StarID_C_dll.dll", CallingConvention = CallingConvention.Cdecl)]
//        public static extern int ExternPerformStarIDC(byte[] fn, int ApplyFiler, double f_length, double pixelSize,
//            byte[] SatID, int Threshold, int MinPixelN, int MaxPixelN,
//    int MaxMagStarN, int MaxIMGStarN, double Star_Std_arcsec,
//    double TwoStarLength_Std_deg, int[] rIDSuc, int[] rID, double[] rScore, double[] rU, double[] rV, int[] rMag, int[] rPixelN, double[] rCalPar);

//    }
//}
