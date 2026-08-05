using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYIERSc
{
    private const double DAS2R = 4.84813681109536E-06;

    private const double DJC = 36525.0;

    private const double DJ00 = 2451545.0;

    private const double DD2R = 0.0174532925199433;

    private const double D2PI = 6.28318530717959;

    private const double DTR = Math.PI / 180.0;

    private const double RTD = 180.0 / Math.PI;

    public static DenseMatrix TEMED2J2k(double JED)
    {
        double num = (JED - 2451545.0) / 36525.0;
        double num2 = JED - 2451545.0;
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix2 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix3 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix4 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix5 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix6 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix7 = new DenseMatrix(3, 3);
        double num3 = (-2.650545 + (2306.077181 + (1.0927348 + (0.01826837 + (-2.8596E-05 + -2.904E-07 * num) * num) * num) * num) * num) * 4.84813681109536E-06;
        double num4 = (2004.191903 + (-0.4294934 + (-0.04182264 + (-7.089E-06 + -1.274E-07 * num) * num) * num) * num) * num * 4.84813681109536E-06;
        double num5 = (2.650545 + (2306.083227 + (0.2988499 + (0.01801828 + (-5.971E-06 + -3.173E-07 * num) * num) * num) * num) * num) * 4.84813681109536E-06;
        denseMatrix[0, 0] = Math.Cos(0.0 - num3);
        denseMatrix[0, 1] = Math.Sin(0.0 - num3);
        denseMatrix[0, 2] = 0.0;
        denseMatrix[1, 0] = 0.0 - Math.Sin(0.0 - num3);
        denseMatrix[1, 1] = Math.Cos(0.0 - num3);
        denseMatrix[1, 2] = 0.0;
        denseMatrix[2, 0] = 0.0;
        denseMatrix[2, 1] = 0.0;
        denseMatrix[2, 2] = 1.0;
        denseMatrix2[0, 0] = Math.Cos(num4);
        denseMatrix2[0, 1] = 0.0;
        denseMatrix2[0, 2] = 0.0 - Math.Sin(num4);
        denseMatrix2[1, 0] = 0.0;
        denseMatrix2[1, 1] = 1.0;
        denseMatrix2[1, 2] = 0.0;
        denseMatrix2[2, 0] = Math.Sin(num4);
        denseMatrix2[2, 1] = 0.0;
        denseMatrix2[2, 2] = Math.Cos(num4);
        denseMatrix3[0, 0] = Math.Cos(0.0 - num5);
        denseMatrix3[0, 1] = Math.Sin(0.0 - num5);
        denseMatrix3[0, 2] = 0.0;
        denseMatrix3[1, 0] = 0.0 - Math.Sin(0.0 - num5);
        denseMatrix3[1, 1] = Math.Cos(0.0 - num5);
        denseMatrix3[1, 2] = 0.0;
        denseMatrix3[2, 0] = 0.0;
        denseMatrix3[2, 1] = 0.0;
        denseMatrix3[2, 2] = 1.0;
        DenseMatrix denseMatrix8 = (DenseMatrix)(denseMatrix * denseMatrix2 * denseMatrix3).Transpose();
        double num6 = (125.0 - 0.05295 * num2) * 0.0174532925199433;
        double num7 = (200.9 + 1.97129 * num2) * 0.0174532925199433;
        double num8 = (-0.0048 * Math.Sin(num6) - 0.0004 * Math.Sin(num7)) * 0.0174532925199433;
        double num9 = (0.0026 * Math.Cos(num6) + 0.0002 * Math.Cos(num7)) * 0.0174532925199433;
        double num10 = 0.4090926006005829;
        double num11 = num10 + (-46.815 * num - 0.00059 * num * num + 0.001813 * num * num * num) * 4.84813681109536E-06;
        double num12 = num11 + num9;
        denseMatrix4[0, 0] = 1.0;
        denseMatrix4[0, 1] = 0.0;
        denseMatrix4[0, 2] = 0.0;
        denseMatrix4[1, 0] = 0.0;
        denseMatrix4[1, 1] = Math.Cos(0.0 - num12);
        denseMatrix4[1, 2] = Math.Sin(0.0 - num12);
        denseMatrix4[2, 0] = 0.0;
        denseMatrix4[2, 1] = 0.0 - Math.Sin(0.0 - num12);
        denseMatrix4[2, 2] = Math.Cos(0.0 - num12);
        denseMatrix5[0, 0] = Math.Cos(0.0 - num8);
        denseMatrix5[0, 1] = Math.Sin(0.0 - num8);
        denseMatrix5[0, 2] = 0.0;
        denseMatrix5[1, 0] = 0.0 - Math.Sin(0.0 - num8);
        denseMatrix5[1, 1] = Math.Cos(0.0 - num8);
        denseMatrix5[1, 2] = 0.0;
        denseMatrix5[2, 0] = 0.0;
        denseMatrix5[2, 1] = 0.0;
        denseMatrix5[2, 2] = 1.0;
        denseMatrix6[0, 0] = 1.0;
        denseMatrix6[0, 1] = 0.0;
        denseMatrix6[0, 2] = 0.0;
        denseMatrix6[1, 0] = 0.0;
        denseMatrix6[1, 1] = Math.Cos(num11);
        denseMatrix6[1, 2] = Math.Sin(num11);
        denseMatrix6[2, 0] = 0.0;
        denseMatrix6[2, 1] = 0.0 - Math.Sin(num11);
        denseMatrix6[2, 2] = Math.Cos(num11);
        DenseMatrix denseMatrix9 = (DenseMatrix)(denseMatrix4 * denseMatrix5 * denseMatrix6).Transpose();
        denseMatrix7[0, 0] = Math.Cos(num8 * Math.Cos(num11));
        denseMatrix7[0, 1] = Math.Sin(num8 * Math.Cos(num11));
        denseMatrix7[0, 2] = 0.0;
        denseMatrix7[1, 0] = 0.0 - Math.Sin(num8 * Math.Cos(num11));
        denseMatrix7[1, 1] = Math.Cos(num8 * Math.Cos(num11));
        denseMatrix7[1, 2] = 0.0;
        denseMatrix7[2, 0] = 0.0;
        denseMatrix7[2, 1] = 0.0;
        denseMatrix7[2, 2] = 1.0;
        DenseMatrix denseMatrix10 = (DenseMatrix)denseMatrix7.Transpose();
        return denseMatrix8 * denseMatrix9 * denseMatrix10;
    }

    public static void J2k2ECR(double JD, double JED, out DenseMatrix mJ2k2ECR, out DenseMatrix mJ2k2ECRdot)
    {
        double num = (JED - 2451545.0) / 36525.0;
        double num2 = JED - 2451545.0;
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix2 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix3 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix4 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix5 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix6 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix7 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix8 = new DenseMatrix(3, 3);
        double num3 = (-2.650545 + (2306.077181 + (1.0927348 + (0.01826837 + (-2.8596E-05 + -2.904E-07 * num) * num) * num) * num) * num) * 4.84813681109536E-06;
        double num4 = (2004.191903 + (-0.4294934 + (-0.04182264 + (-7.089E-06 + -1.274E-07 * num) * num) * num) * num) * num * 4.84813681109536E-06;
        double num5 = (2.650545 + (2306.083227 + (0.2988499 + (0.01801828 + (-5.971E-06 + -3.173E-07 * num) * num) * num) * num) * num) * 4.84813681109536E-06;
        denseMatrix[0, 0] = Math.Cos(0.0 - num3);
        denseMatrix[0, 1] = Math.Sin(0.0 - num3);
        denseMatrix[0, 2] = 0.0;
        denseMatrix[1, 0] = 0.0 - Math.Sin(0.0 - num3);
        denseMatrix[1, 1] = Math.Cos(0.0 - num3);
        denseMatrix[1, 2] = 0.0;
        denseMatrix[2, 0] = 0.0;
        denseMatrix[2, 1] = 0.0;
        denseMatrix[2, 2] = 1.0;
        denseMatrix2[0, 0] = Math.Cos(num4);
        denseMatrix2[0, 1] = 0.0;
        denseMatrix2[0, 2] = 0.0 - Math.Sin(num4);
        denseMatrix2[1, 0] = 0.0;
        denseMatrix2[1, 1] = 1.0;
        denseMatrix2[1, 2] = 0.0;
        denseMatrix2[2, 0] = Math.Sin(num4);
        denseMatrix2[2, 1] = 0.0;
        denseMatrix2[2, 2] = Math.Cos(num4);
        denseMatrix3[0, 0] = Math.Cos(0.0 - num5);
        denseMatrix3[0, 1] = Math.Sin(0.0 - num5);
        denseMatrix3[0, 2] = 0.0;
        denseMatrix3[1, 0] = 0.0 - Math.Sin(0.0 - num5);
        denseMatrix3[1, 1] = Math.Cos(0.0 - num5);
        denseMatrix3[1, 2] = 0.0;
        denseMatrix3[2, 0] = 0.0;
        denseMatrix3[2, 1] = 0.0;
        denseMatrix3[2, 2] = 1.0;
        DenseMatrix denseMatrix9 = denseMatrix * denseMatrix2 * denseMatrix3;
        DenseMatrix denseMatrix10 = (DenseMatrix)denseMatrix9.Transpose();
        double num6 = (125.0 - 0.05295 * num2) * 0.0174532925199433;
        double num7 = (200.9 + 1.97129 * num2) * 0.0174532925199433;
        double num8 = (-0.0048 * Math.Sin(num6) - 0.0004 * Math.Sin(num7)) * 0.0174532925199433;
        double num9 = (0.0026 * Math.Cos(num6) + 0.0002 * Math.Cos(num7)) * 0.0174532925199433;
        double num10 = 0.4090926006005829;
        double num11 = num10 + (-46.815 * num - 0.00059 * num * num + 0.001813 * num * num * num) * 4.84813681109536E-06;
        double num12 = num11 + num9;
        denseMatrix4[0, 0] = 1.0;
        denseMatrix4[0, 1] = 0.0;
        denseMatrix4[0, 2] = 0.0;
        denseMatrix4[1, 0] = 0.0;
        denseMatrix4[1, 1] = Math.Cos(0.0 - num12);
        denseMatrix4[1, 2] = Math.Sin(0.0 - num12);
        denseMatrix4[2, 0] = 0.0;
        denseMatrix4[2, 1] = 0.0 - Math.Sin(0.0 - num12);
        denseMatrix4[2, 2] = Math.Cos(0.0 - num12);
        denseMatrix5[0, 0] = Math.Cos(0.0 - num8);
        denseMatrix5[0, 1] = Math.Sin(0.0 - num8);
        denseMatrix5[0, 2] = 0.0;
        denseMatrix5[1, 0] = 0.0 - Math.Sin(0.0 - num8);
        denseMatrix5[1, 1] = Math.Cos(0.0 - num8);
        denseMatrix5[1, 2] = 0.0;
        denseMatrix5[2, 0] = 0.0;
        denseMatrix5[2, 1] = 0.0;
        denseMatrix5[2, 2] = 1.0;
        denseMatrix6[0, 0] = 1.0;
        denseMatrix6[0, 1] = 0.0;
        denseMatrix6[0, 2] = 0.0;
        denseMatrix6[1, 0] = 0.0;
        denseMatrix6[1, 1] = Math.Cos(num11);
        denseMatrix6[1, 2] = Math.Sin(num11);
        denseMatrix6[2, 0] = 0.0;
        denseMatrix6[2, 1] = 0.0 - Math.Sin(num11);
        denseMatrix6[2, 2] = Math.Cos(num11);
        DenseMatrix denseMatrix11 = denseMatrix4 * denseMatrix5 * denseMatrix6;
        DenseMatrix denseMatrix12 = (DenseMatrix)denseMatrix11.Transpose();
        double num13 = JD - Math.Floor(JD);
        double num14 = 6.28318530717959 * (num13 + 0.779057273264 + 0.00273781191135448 * (JD - 2451545.0));
        num14 -= Math.Floor(num14 / 6.28318530717959) * 6.28318530717959;
        if (num14 < 0.0)
        {
            num14 += 6.28318530717959;
        }
        double num15 = 0.014506 + (4612.156534 + (1.3915817 + (-4.4E-07 + (-2.9956E-05 + -3.68E-08 * num) * num) * num) * num) * num;
        double num16 = num14 + num15 * 4.84813681109536E-06;
        num16 -= Math.Floor(num16 / 6.28318530717959) * 6.28318530717959;
        if (num16 < 0.0)
        {
            num16 += 6.28318530717959;
        }
        double num17 = num16 + num8 * Math.Cos(num11);
        num17 -= Math.Floor(num17 / 6.28318530717959) * 6.28318530717959;
        if (num17 < 0.0)
        {
            num17 += 6.28318530717959;
        }
        denseMatrix7[0, 0] = Math.Cos(num17);
        denseMatrix7[0, 1] = Math.Sin(num17);
        denseMatrix7[0, 2] = 0.0;
        denseMatrix7[1, 0] = 0.0 - Math.Sin(num17);
        denseMatrix7[1, 1] = Math.Cos(num17);
        denseMatrix7[1, 2] = 0.0;
        denseMatrix7[2, 0] = 0.0;
        denseMatrix7[2, 1] = 0.0;
        denseMatrix7[2, 2] = 1.0;
        double num18 = 7.2921151467E-05;
        denseMatrix8[0, 0] = (0.0 - num18) * Math.Sin(num17);
        denseMatrix8[0, 1] = num18 * Math.Cos(num17);
        denseMatrix8[0, 2] = 0.0;
        denseMatrix8[1, 0] = (0.0 - num18) * Math.Cos(num17);
        denseMatrix8[1, 1] = (0.0 - num18) * Math.Sin(num17);
        denseMatrix8[1, 2] = 0.0;
        denseMatrix8[2, 0] = 0.0;
        denseMatrix8[2, 1] = 0.0;
        denseMatrix8[2, 2] = 0.0;
        DenseMatrix denseMatrix13 = denseMatrix11 * denseMatrix9;
        mJ2k2ECR = denseMatrix7 * denseMatrix13;
        mJ2k2ECRdot = denseMatrix8 * denseMatrix13;
    }

    public static void TEMED2J2k2ECR(double JD, double JED, out DenseMatrix mTemed2J2k, out DenseMatrix mJ2k2ECR, out DenseMatrix mJ2k2ECRdot)
    {
        double num = (JED - 2451545.0) / 36525.0;
        double num2 = JED - 2451545.0;
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix2 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix3 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix4 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix5 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix6 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix7 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix8 = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix9 = new DenseMatrix(3, 3);
        double num3 = (-2.650545 + (2306.077181 + (1.0927348 + (0.01826837 + (-2.8596E-05 + -2.904E-07 * num) * num) * num) * num) * num) * 4.84813681109536E-06;
        double num4 = (2004.191903 + (-0.4294934 + (-0.04182264 + (-7.089E-06 + -1.274E-07 * num) * num) * num) * num) * num * 4.84813681109536E-06;
        double num5 = (2.650545 + (2306.083227 + (0.2988499 + (0.01801828 + (-5.971E-06 + -3.173E-07 * num) * num) * num) * num) * num) * 4.84813681109536E-06;
        denseMatrix[0, 0] = Math.Cos(0.0 - num3);
        denseMatrix[0, 1] = Math.Sin(0.0 - num3);
        denseMatrix[0, 2] = 0.0;
        denseMatrix[1, 0] = 0.0 - Math.Sin(0.0 - num3);
        denseMatrix[1, 1] = Math.Cos(0.0 - num3);
        denseMatrix[1, 2] = 0.0;
        denseMatrix[2, 0] = 0.0;
        denseMatrix[2, 1] = 0.0;
        denseMatrix[2, 2] = 1.0;
        denseMatrix2[0, 0] = Math.Cos(num4);
        denseMatrix2[0, 1] = 0.0;
        denseMatrix2[0, 2] = 0.0 - Math.Sin(num4);
        denseMatrix2[1, 0] = 0.0;
        denseMatrix2[1, 1] = 1.0;
        denseMatrix2[1, 2] = 0.0;
        denseMatrix2[2, 0] = Math.Sin(num4);
        denseMatrix2[2, 1] = 0.0;
        denseMatrix2[2, 2] = Math.Cos(num4);
        denseMatrix3[0, 0] = Math.Cos(0.0 - num5);
        denseMatrix3[0, 1] = Math.Sin(0.0 - num5);
        denseMatrix3[0, 2] = 0.0;
        denseMatrix3[1, 0] = 0.0 - Math.Sin(0.0 - num5);
        denseMatrix3[1, 1] = Math.Cos(0.0 - num5);
        denseMatrix3[1, 2] = 0.0;
        denseMatrix3[2, 0] = 0.0;
        denseMatrix3[2, 1] = 0.0;
        denseMatrix3[2, 2] = 1.0;
        DenseMatrix denseMatrix10 = denseMatrix * denseMatrix2 * denseMatrix3;
        DenseMatrix denseMatrix11 = (DenseMatrix)denseMatrix10.Transpose();
        double num6 = (125.0 - 0.05295 * num2) * 0.0174532925199433;
        double num7 = (200.9 + 1.97129 * num2) * 0.0174532925199433;
        double num8 = (-0.0048 * Math.Sin(num6) - 0.0004 * Math.Sin(num7)) * 0.0174532925199433;
        double num9 = (0.0026 * Math.Cos(num6) + 0.0002 * Math.Cos(num7)) * 0.0174532925199433;
        double num10 = 0.4090926006005829;
        double num11 = num10 + (-46.815 * num - 0.00059 * num * num + 0.001813 * num * num * num) * 4.84813681109536E-06;
        double num12 = num11 + num9;
        denseMatrix4[0, 0] = 1.0;
        denseMatrix4[0, 1] = 0.0;
        denseMatrix4[0, 2] = 0.0;
        denseMatrix4[1, 0] = 0.0;
        denseMatrix4[1, 1] = Math.Cos(0.0 - num12);
        denseMatrix4[1, 2] = Math.Sin(0.0 - num12);
        denseMatrix4[2, 0] = 0.0;
        denseMatrix4[2, 1] = 0.0 - Math.Sin(0.0 - num12);
        denseMatrix4[2, 2] = Math.Cos(0.0 - num12);
        denseMatrix5[0, 0] = Math.Cos(0.0 - num8);
        denseMatrix5[0, 1] = Math.Sin(0.0 - num8);
        denseMatrix5[0, 2] = 0.0;
        denseMatrix5[1, 0] = 0.0 - Math.Sin(0.0 - num8);
        denseMatrix5[1, 1] = Math.Cos(0.0 - num8);
        denseMatrix5[1, 2] = 0.0;
        denseMatrix5[2, 0] = 0.0;
        denseMatrix5[2, 1] = 0.0;
        denseMatrix5[2, 2] = 1.0;
        denseMatrix6[0, 0] = 1.0;
        denseMatrix6[0, 1] = 0.0;
        denseMatrix6[0, 2] = 0.0;
        denseMatrix6[1, 0] = 0.0;
        denseMatrix6[1, 1] = Math.Cos(num11);
        denseMatrix6[1, 2] = Math.Sin(num11);
        denseMatrix6[2, 0] = 0.0;
        denseMatrix6[2, 1] = 0.0 - Math.Sin(num11);
        denseMatrix6[2, 2] = Math.Cos(num11);
        DenseMatrix denseMatrix12 = denseMatrix4 * denseMatrix5 * denseMatrix6;
        DenseMatrix denseMatrix13 = (DenseMatrix)denseMatrix12.Transpose();
        denseMatrix9[0, 0] = Math.Cos(num8 * Math.Cos(num11));
        denseMatrix9[0, 1] = Math.Sin(num8 * Math.Cos(num11));
        denseMatrix9[0, 2] = 0.0;
        denseMatrix9[1, 0] = 0.0 - Math.Sin(num8 * Math.Cos(num11));
        denseMatrix9[1, 1] = Math.Cos(num8 * Math.Cos(num11));
        denseMatrix9[1, 2] = 0.0;
        denseMatrix9[2, 0] = 0.0;
        denseMatrix9[2, 1] = 0.0;
        denseMatrix9[2, 2] = 1.0;
        DenseMatrix denseMatrix14 = (DenseMatrix)denseMatrix9.Transpose();
        double num13 = JD - Math.Floor(JD);
        double num14 = 6.28318530717959 * (num13 + 0.779057273264 + 0.00273781191135448 * (JD - 2451545.0));
        num14 -= Math.Floor(num14 / 6.28318530717959) * 6.28318530717959;
        if (num14 < 0.0)
        {
            num14 += 6.28318530717959;
        }
        double num15 = 0.014506 + (4612.156534 + (1.3915817 + (-4.4E-07 + (-2.9956E-05 + -3.68E-08 * num) * num) * num) * num) * num;
        double num16 = num14 + num15 * 4.84813681109536E-06;
        num16 -= Math.Floor(num16 / 6.28318530717959) * 6.28318530717959;
        if (num16 < 0.0)
        {
            num16 += 6.28318530717959;
        }
        double num17 = num16 + num8 * Math.Cos(num11);
        num17 -= Math.Floor(num17 / 6.28318530717959) * 6.28318530717959;
        if (num17 < 0.0)
        {
            num17 += 6.28318530717959;
        }
        denseMatrix7[0, 0] = Math.Cos(num17);
        denseMatrix7[0, 1] = Math.Sin(num17);
        denseMatrix7[0, 2] = 0.0;
        denseMatrix7[1, 0] = 0.0 - Math.Sin(num17);
        denseMatrix7[1, 1] = Math.Cos(num17);
        denseMatrix7[1, 2] = 0.0;
        denseMatrix7[2, 0] = 0.0;
        denseMatrix7[2, 1] = 0.0;
        denseMatrix7[2, 2] = 1.0;
        double num18 = 7.2921151467E-05;
        denseMatrix8[0, 0] = (0.0 - num18) * Math.Sin(num17);
        denseMatrix8[0, 1] = num18 * Math.Cos(num17);
        denseMatrix8[0, 2] = 0.0;
        denseMatrix8[1, 0] = (0.0 - num18) * Math.Cos(num17);
        denseMatrix8[1, 1] = (0.0 - num18) * Math.Sin(num17);
        denseMatrix8[1, 2] = 0.0;
        denseMatrix8[2, 0] = 0.0;
        denseMatrix8[2, 1] = 0.0;
        denseMatrix8[2, 2] = 0.0;
        mTemed2J2k = denseMatrix11 * denseMatrix13 * denseMatrix14;
        DenseMatrix denseMatrix15 = denseMatrix12 * denseMatrix10;
        mJ2k2ECR = denseMatrix7 * denseMatrix15;
        mJ2k2ECRdot = denseMatrix8 * denseMatrix15;
    }

    public static void SimpleCvt_ECR2J2k(double UnixSec, DenseVector R_ecr, DenseVector V_ecr, out DenseVector R_j2k, out DenseVector V_j2k)
    {
        double jD = HSYTime.UnixTime_to_JD(UnixSec);
        double jED = HSYTime.JD_to_JED(jD);
        J2k2ECR(jD, jED, out var mJ2k2ECR, out var mJ2k2ECRdot);
        DenseMatrix denseMatrix = (DenseMatrix)mJ2k2ECR.Transpose();
        DenseMatrix denseMatrix2 = (DenseMatrix)mJ2k2ECRdot.Transpose();
        R_j2k = denseMatrix * R_ecr;
        V_j2k = denseMatrix * V_ecr + denseMatrix2 * R_ecr;
    }
}
