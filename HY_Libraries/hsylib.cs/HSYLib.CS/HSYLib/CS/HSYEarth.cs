using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYEarth
{
    public const double PI = Math.PI;

    public const double DTR = Math.PI / 180.0;

    public const double RTD = 180.0 / Math.PI;

    private static void LLH_to_XYZ(double Lat_deg, double Lon_deg, double Height_meter, double[] XYZ_ecr)
    {
        double num = Lat_deg * (Math.PI / 180.0);
        double num2 = Lon_deg * (Math.PI / 180.0);
        double num3 = Height_meter * 0.001;
        double num4 = Math.Sin(num);
        double num5 = Math.Cos(num);
        double num6 = Math.Sin(num2);
        double num7 = Math.Cos(num2);
        double num8 = 6378.135 * (0.9983235934110516 + 0.0016764065889484571 * Math.Cos(2.0 * num)) + num3;
        double num9 = Math.Atan(0.993305615000412 * num4 / num5);
        double num10 = Math.Cos(num9);
        double num11 = Math.Sin(num9);
        XYZ_ecr[2] = num8 * num11;
        XYZ_ecr[0] = num8 * num10 * num7;
        XYZ_ecr[1] = num8 * num10 * num6;
    }

    public static DenseVector LLH_to_XYZ(double Lat_deg, double Lon_deg, double Height_meter)
    {
        DenseVector denseVector = new DenseVector(3);
        LLH_to_XYZ(Lat_deg, Lon_deg, Height_meter, (double[])denseVector);
        return denseVector;
    }

    private static void XYZ_to_LLH(double[] XYZ_ecr, double[] LLH_Geodetic)
    {
        double num = Math.Sqrt(XYZ_ecr[0] * XYZ_ecr[0] + XYZ_ecr[1] * XYZ_ecr[1] + XYZ_ecr[2] * XYZ_ecr[2]);
        double num2 = XYZ_ecr[2] / num;
        double num3 = num2 / Math.Sqrt(1.0 - num2 * num2);
        double d = num3 / 0.993305615000412;
        LLH_Geodetic[0] = Math.Atan(d);
        LLH_Geodetic[1] = Math.Atan2(XYZ_ecr[1], XYZ_ecr[0]) * (180.0 / Math.PI);
        double num4 = 6378.135 * (0.9983235934110516 + 0.0016764065889484571 * Math.Cos(2.0 * LLH_Geodetic[0]));
        LLH_Geodetic[0] *= 180.0 / Math.PI;
        LLH_Geodetic[2] = num - num4;
    }

    public static DenseVector LL_to_XYZ(double Lat_deg, double Lon_deg)
    {
        return LLH_to_XYZ(Lat_deg, Lon_deg, 0.0);
    }

    public static void XYZ_to_LL(double[] XYZ_ecr, double[] LL_Geodetic)
    {
        double num = Math.Sqrt(XYZ_ecr[0] * XYZ_ecr[0] + XYZ_ecr[1] * XYZ_ecr[1] + XYZ_ecr[2] * XYZ_ecr[2]);
        double num2 = XYZ_ecr[2] / num;
        double num3 = num2 / Math.Sqrt(1.0 - num2 * num2);
        double d = num3 / 0.993305615000412;
        LL_Geodetic[0] = Math.Atan(d) * (180.0 / Math.PI);
        LL_Geodetic[1] = Math.Atan2(XYZ_ecr[1], XYZ_ecr[0]) * (180.0 / Math.PI);
    }

    public static int get_R_p_on_earth(double[] R, double[] Gradient, double[] R_p)
    {
        double num = 0.0033528106647474805;
        double num2 = 6378.137;
        double num3 = num2 * (1.0 - num);
        double num4 = Gradient[0];
        double num5 = Gradient[1];
        double num6 = Gradient[2];
        double num7 = num4 * num4 + num5 * num5 + num2 * num2 / (num3 * num3) * num6 * num6;
        double num8 = num4 * R[0] + num5 * R[1] + num2 * num2 / (num3 * num3) * num6 * R[2];
        double num9 = R[0] * R[0] + R[1] * R[1] + num2 * num2 / (num3 * num3) * R[2] * R[2] - num2 * num2;
        double[] array = new double[2];
        double[] array2 = new double[2];
        double[] array3 = new double[2];
        double[] array4 = new double[2];
        double[] array5 = new double[2];
        double num10 = num8 * num8 - num7 * num9;
        if (num10 < 0.0)
        {
            return -1;
        }
        num10 = Math.Sqrt(num10);
        array[0] = (0.0 - num8 + num10) / num7;
        array[1] = (0.0 - num8 - num10) / num7;
        for (int i = 0; i < 2; i++)
        {
            array3[i] = array[i] * num4;
            array4[i] = array[i] * num5;
            array5[i] = array[i] * num6;
            array2[i] = array3[i] * array3[i] + array4[i] * array4[i] + array5[i] * array5[i];
        }
        int num11 = ((array2[0] > array2[1]) ? 1 : 0);
        R_p[0] = array3[num11] + R[0];
        R_p[1] = array4[num11] + R[1];
        R_p[2] = array5[num11] + R[2];
        return 1;
    }

    public static void DCM_ECR2ENU(double Lat_deg, double Lon_deg, double[,] mEcr2ENU)
    {
        double num = Math.Sin(Lat_deg * (Math.PI / 180.0));
        double num2 = Math.Cos(Lat_deg * (Math.PI / 180.0));
        double num3 = Math.Sin(Lon_deg * (Math.PI / 180.0));
        double num4 = Math.Cos(Lon_deg * (Math.PI / 180.0));
        mEcr2ENU[0, 0] = 0.0 - num3;
        mEcr2ENU[0, 1] = num4;
        mEcr2ENU[0, 2] = 0.0;
        mEcr2ENU[1, 0] = (0.0 - num) * num4;
        mEcr2ENU[1, 1] = (0.0 - num) * num3;
        mEcr2ENU[1, 2] = num2;
        mEcr2ENU[2, 0] = num2 * num4;
        mEcr2ENU[2, 1] = num2 * num3;
        mEcr2ENU[2, 2] = num;
    }

    public static DenseMatrix DCM_ECR2ENU(double Lat_deg, double Lon_deg)
    {
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        double num = Math.Sin(Lat_deg * (Math.PI / 180.0));
        double num2 = Math.Cos(Lat_deg * (Math.PI / 180.0));
        double num3 = Math.Sin(Lon_deg * (Math.PI / 180.0));
        double num4 = Math.Cos(Lon_deg * (Math.PI / 180.0));
        denseMatrix[0, 0] = 0.0 - num3;
        denseMatrix[0, 1] = num4;
        denseMatrix[0, 2] = 0.0;
        denseMatrix[1, 0] = (0.0 - num) * num4;
        denseMatrix[1, 1] = (0.0 - num) * num3;
        denseMatrix[1, 2] = num2;
        denseMatrix[2, 0] = num2 * num4;
        denseMatrix[2, 1] = num2 * num3;
        denseMatrix[2, 2] = num;
        return denseMatrix;
    }
}
