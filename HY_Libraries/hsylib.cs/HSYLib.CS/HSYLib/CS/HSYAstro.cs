using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYAstro
{
    private const double PI = Math.PI;

    private const double DTR = Math.PI / 180.0;

    private const double RTD = 180.0 / Math.PI;

    private static double sind(double angle_deg)
    {
        return Math.Sin(angle_deg * (Math.PI / 180.0));
    }

    private static double cosd(double angle_deg)
    {
        return Math.Cos(angle_deg * (Math.PI / 180.0));
    }

    public static DenseVector Moon_Position(double jd)
    {
        double num = 0.91748206284057;
        double num2 = 0.39777715415268;
        double num3 = Math.PI / 180.0;
        double num4 = 6378.135;
        double num5 = (jd - 2451545.0) / 36525.0;
        double num6 = 218.32 + 481267.883 * num5 + 6.29 * sind(134.9 + 477198.85 * num5) - 1.27 * sind(259.2 - 413335.38 * num5) + 0.66 * sind(235.7 + 890534.23 * num5) + 0.21 * sind(269.9 + 954397.7 * num5) - 0.19 * sind(357.5 + 35999.05 * num5) - 0.11 * sind(186.6 + 966404.05 * num5);
        double num7 = 5.13 * sind(93.3 + 483202.03 * num5) + 0.28 * sind(228.2 + 960400.87 * num5) - 0.28 * sind(318.3 + 6003.18 * num5) - 0.17 * sind(217.6 - 407332.2 * num5);
        double angle_deg = 0.9508 + 0.0518 * cosd(134.9 + 477198.85 * num5) + 0.0095 * cosd(259.2 - 413335.38 * num5) + 0.0078 * cosd(235.7 + 890534.23 * num5) + 0.0028 * cosd(269.9 + 954397.7 * num5);
        num6 %= 360.0;
        if (num6 < 0.0)
        {
            num6 += 360.0;
        }
        double num8 = num4 / sind(angle_deg);
        num6 = num3 * num6;
        num7 = num3 * num7;
        double num9 = Math.Atan2(Math.Sin(num6) * num - Math.Tan(num7) * num2, Math.Cos(num6));
        double num10 = Math.Asin(Math.Sin(num7) * num + Math.Cos(num7) * num2 * Math.Sin(num6));
        DenseVector denseVector = new DenseVector(3);
        denseVector[2] = num8 * Math.Sin(num10);
        denseVector[0] = num8 * Math.Cos(num10);
        denseVector[1] = denseVector[0] * Math.Sin(num9);
        denseVector[0] *= Math.Cos(num9);
        return denseVector;
    }

    public static DenseVector Sun_Position(double jd)
    {
        double num = 9.93664E-05;
        double num2 = 0.91748206284057;
        double num3 = 0.39777715415268;
        double num4 = 149597870.0;
        double num5 = Math.PI * 2.0;
        double num6 = (jd - 2415020.0) / 36525.0;
        double num7 = (6.25658358 + num6 * 628.30194572) % num5;
        double num8 = (3.70795199 + num6 * 1021.32292286) % num5;
        double num9 = (5.57772322 + num6 * 334.0556174) % num5;
        double num10 = (3.98187774 + num6 * 52.96346477) % num5;
        double a = (6.12152394 + num6 * 7771.37719393) % num5;
        double a2 = (4.52360151 - num6 * 33.757146246) % num5;
        double num11 = -8.355E-05 * Math.Sin(a2);
        double num12 = 2.34553E-05 * Math.Cos(5.220309 + num8 - num7) + 2.67908E-05 * Math.Cos(2.588556 + 2.0 * (num8 - num7)) + 1.21058E-05 * Math.Cos(5.514251 + 2.0 * num8 - 3.0 * num7) + 9.9047E-06 * Math.Cos(6.001983 + 2.0 * (num7 - num9)) + 3.49454E-05 * Math.Cos(3.133418 + num7 - num10) + 1.26051E-05 * Math.Cos(4.593997 - num10) + 1.32402E-05 * Math.Cos(1.520967 + 2.0 * (num7 - num10)) + 3.10281E-05 * Math.Sin(4.035026 + num6 * 0.352556) + 3.12898E-05 * Math.Sin(a) + num11 - num;
        double num13 = Math.Sin(num7);
        double num14 = Math.Cos(num7);
        double num15 = 4.88162793 + num6 * (628.33195099 + num6 * 5.2844E-06) % num5 + num13 * (0.03349579 - 8.358E-05 * num6 + num14 * (0.00070141 - 3.5E-06 * num6 + num14 * 2.044E-05)) + num12;
        double num16 = 0.01675104 - 4.274E-05 * num6;
        double num17 = num4 * (1.0 - num16 * num16 - num14 * (num16 + num16 * num16 * num14));
        double num18 = Math.Atan2(Math.Sin(num15) * num2, Math.Cos(num15));
        double num19 = Math.Asin(num3 * Math.Sin(num15));
        DenseVector denseVector = new DenseVector(3);
        denseVector[2] = num17 * Math.Sin(num19);
        denseVector[0] = num17 * Math.Cos(num19);
        denseVector[1] = denseVector[0] * Math.Sin(num18);
        denseVector[0] *= Math.Cos(num18);
        return denseVector;
    }

    public static bool Check_Eclipse(DenseVector SatPos_j2k, DenseVector SunPos_j2k)
    {
        double[] array = new double[3];
        double[] array2 = new double[3];
        array[0] = SunPos_j2k[0] - SatPos_j2k[0];
        array[1] = SunPos_j2k[1] - SatPos_j2k[1];
        array[2] = SunPos_j2k[2] - SatPos_j2k[2];
        array2[0] = 0.0 - SatPos_j2k[0];
        array2[1] = 0.0 - SatPos_j2k[1];
        array2[2] = 0.0 - SatPos_j2k[2];
        double num = Math.Sqrt(array[0] * array[0] + array[1] * array[1] + array[2] * array[2]);
        double num2 = Math.Sqrt(array2[0] * array2[0] + array2[1] * array2[1] + array2[2] * array2[2]);
        double num3 = 6378.135 / num2;
        double num4 = Math.Sqrt(1.0 - num3 * num3);
        double num5 = (array2[0] * array[0] + array2[1] * array[1] + array2[2] * array[2]) / num / num2;
        if (num5 > num4)
        {
            return true;
        }
        return false;
    }
}
