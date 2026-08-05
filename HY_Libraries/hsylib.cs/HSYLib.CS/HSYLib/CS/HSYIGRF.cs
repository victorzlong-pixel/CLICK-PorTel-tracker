using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYIGRF
{
    private int g_IGRF_year;

    private double[] g_IGRF_coef = new double[121];

    private double[] g_IGRF_sec = new double[121];

    private double[] g_G = new double[171];

    private double[] g_ghe = new double[121];

    public HSYIGRF(int epoch_year, double epoch_dn, int IGRF_year)
    {
        Initialize(epoch_year, epoch_dn, IGRF_year);
    }

    public int Initialize(int epoch_year, double epoch_dn, int IGRF_year)
    {
        int num = 0;
        double num2 = 0.0;
        double num3 = 0.0;
        int num4 = 0;
        int num5 = 0;
        int num6 = 0;
        int num7 = 0;
        double num8 = 0.0;
        double num9 = 0.0;
        double num10 = 0.0;
        if (IGRF_year == 2010)
        {
            g_IGRF_coef[0] = 0.0;
            g_IGRF_coef[1] = -29619.4;
            g_IGRF_coef[2] = -1728.2;
            g_IGRF_coef[3] = 5186.1;
            g_IGRF_coef[4] = -2267.7;
            g_IGRF_coef[5] = 3068.4;
            g_IGRF_coef[6] = -2481.6;
            g_IGRF_coef[7] = 1670.9;
            g_IGRF_coef[8] = -458.0;
            g_IGRF_coef[9] = 1339.6;
            g_IGRF_coef[10] = -2288.0;
            g_IGRF_coef[11] = -227.6;
            g_IGRF_coef[12] = 1252.1;
            g_IGRF_coef[13] = 293.4;
            g_IGRF_coef[14] = 714.5;
            g_IGRF_coef[15] = -491.1;
            g_IGRF_coef[16] = 932.3;
            g_IGRF_coef[17] = 786.8;
            g_IGRF_coef[18] = 272.6;
            g_IGRF_coef[19] = 250.0;
            g_IGRF_coef[20] = -231.9;
            g_IGRF_coef[21] = -403.0;
            g_IGRF_coef[22] = 119.8;
            g_IGRF_coef[23] = 111.3;
            g_IGRF_coef[24] = -303.8;
            g_IGRF_coef[25] = -218.8;
            g_IGRF_coef[26] = 351.4;
            g_IGRF_coef[27] = 43.8;
            g_IGRF_coef[28] = 222.3;
            g_IGRF_coef[29] = 171.9;
            g_IGRF_coef[30] = -130.4;
            g_IGRF_coef[31] = -133.1;
            g_IGRF_coef[32] = -168.6;
            g_IGRF_coef[33] = -39.3;
            g_IGRF_coef[34] = -12.9;
            g_IGRF_coef[35] = 106.3;
            g_IGRF_coef[36] = 72.3;
            g_IGRF_coef[37] = 68.2;
            g_IGRF_coef[38] = -17.4;
            g_IGRF_coef[39] = 74.2;
            g_IGRF_coef[40] = 63.7;
            g_IGRF_coef[41] = -160.9;
            g_IGRF_coef[42] = 65.1;
            g_IGRF_coef[43] = -5.9;
            g_IGRF_coef[44] = -61.2;
            g_IGRF_coef[45] = 16.9;
            g_IGRF_coef[46] = 0.7;
            g_IGRF_coef[47] = -90.4;
            g_IGRF_coef[48] = 43.8;
            g_IGRF_coef[49] = 79.0;
            g_IGRF_coef[50] = -74.0;
            g_IGRF_coef[51] = -64.6;
            g_IGRF_coef[52] = 0.0;
            g_IGRF_coef[53] = -24.2;
            g_IGRF_coef[54] = 33.3;
            g_IGRF_coef[55] = 6.2;
            g_IGRF_coef[56] = 9.1;
            g_IGRF_coef[57] = 24.0;
            g_IGRF_coef[58] = 6.9;
            g_IGRF_coef[59] = 14.8;
            g_IGRF_coef[60] = 7.3;
            g_IGRF_coef[61] = -25.4;
            g_IGRF_coef[62] = -1.2;
            g_IGRF_coef[63] = -5.8;
            g_IGRF_coef[64] = 24.4;
            g_IGRF_coef[65] = 6.6;
            g_IGRF_coef[66] = 11.9;
            g_IGRF_coef[67] = -9.2;
            g_IGRF_coef[68] = -21.5;
            g_IGRF_coef[69] = -7.9;
            g_IGRF_coef[70] = 8.5;
            g_IGRF_coef[71] = -16.6;
            g_IGRF_coef[72] = -21.5;
            g_IGRF_coef[73] = 9.1;
            g_IGRF_coef[74] = 15.5;
            g_IGRF_coef[75] = 7.0;
            g_IGRF_coef[76] = 8.9;
            g_IGRF_coef[77] = -7.9;
            g_IGRF_coef[78] = -14.9;
            g_IGRF_coef[79] = -7.0;
            g_IGRF_coef[80] = -2.1;
            g_IGRF_coef[81] = 5.0;
            g_IGRF_coef[82] = 9.4;
            g_IGRF_coef[83] = -19.7;
            g_IGRF_coef[84] = 3.0;
            g_IGRF_coef[85] = 13.4;
            g_IGRF_coef[86] = -8.4;
            g_IGRF_coef[87] = 12.5;
            g_IGRF_coef[88] = 6.3;
            g_IGRF_coef[89] = -6.2;
            g_IGRF_coef[90] = -8.9;
            g_IGRF_coef[91] = -8.4;
            g_IGRF_coef[92] = -1.5;
            g_IGRF_coef[93] = 8.4;
            g_IGRF_coef[94] = 9.3;
            g_IGRF_coef[95] = 3.8;
            g_IGRF_coef[96] = -4.3;
            g_IGRF_coef[97] = -8.2;
            g_IGRF_coef[98] = -8.2;
            g_IGRF_coef[99] = 4.8;
            g_IGRF_coef[100] = -2.6;
            g_IGRF_coef[101] = -6.0;
            g_IGRF_coef[102] = 1.7;
            g_IGRF_coef[103] = 1.7;
            g_IGRF_coef[104] = 0.0;
            g_IGRF_coef[105] = -3.1;
            g_IGRF_coef[106] = 4.0;
            g_IGRF_coef[107] = -0.5;
            g_IGRF_coef[108] = 4.9;
            g_IGRF_coef[109] = 3.7;
            g_IGRF_coef[110] = -5.9;
            g_IGRF_coef[111] = 1.0;
            g_IGRF_coef[112] = -1.2;
            g_IGRF_coef[113] = 2.0;
            g_IGRF_coef[114] = -2.9;
            g_IGRF_coef[115] = 4.2;
            g_IGRF_coef[116] = 0.2;
            g_IGRF_coef[117] = 0.3;
            g_IGRF_coef[118] = -2.2;
            g_IGRF_coef[119] = -1.1;
            g_IGRF_coef[120] = -7.4;
        }
        else
        {
            g_IGRF_coef[0] = 0.0;
            g_IGRF_coef[1] = -29556.8;
            g_IGRF_coef[2] = -1671.8;
            g_IGRF_coef[3] = 5080.0;
            g_IGRF_coef[4] = -2340.5;
            g_IGRF_coef[5] = 3047.0;
            g_IGRF_coef[6] = -2594.9;
            g_IGRF_coef[7] = 1656.9;
            g_IGRF_coef[8] = -516.7;
            g_IGRF_coef[9] = 1335.7;
            g_IGRF_coef[10] = -2305.3;
            g_IGRF_coef[11] = -200.4;
            g_IGRF_coef[12] = 1246.8;
            g_IGRF_coef[13] = 269.3;
            g_IGRF_coef[14] = 674.4;
            g_IGRF_coef[15] = -524.5;
            g_IGRF_coef[16] = 919.8;
            g_IGRF_coef[17] = 798.2;
            g_IGRF_coef[18] = 281.4;
            g_IGRF_coef[19] = 211.5;
            g_IGRF_coef[20] = -225.8;
            g_IGRF_coef[21] = -379.5;
            g_IGRF_coef[22] = 145.7;
            g_IGRF_coef[23] = 100.2;
            g_IGRF_coef[24] = -304.7;
            g_IGRF_coef[25] = -227.6;
            g_IGRF_coef[26] = 354.4;
            g_IGRF_coef[27] = 42.7;
            g_IGRF_coef[28] = 208.8;
            g_IGRF_coef[29] = 179.8;
            g_IGRF_coef[30] = -136.6;
            g_IGRF_coef[31] = -123.0;
            g_IGRF_coef[32] = -168.3;
            g_IGRF_coef[33] = -19.5;
            g_IGRF_coef[34] = -14.1;
            g_IGRF_coef[35] = 103.6;
            g_IGRF_coef[36] = 72.9;
            g_IGRF_coef[37] = 69.6;
            g_IGRF_coef[38] = -20.2;
            g_IGRF_coef[39] = 76.6;
            g_IGRF_coef[40] = 54.7;
            g_IGRF_coef[41] = -151.1;
            g_IGRF_coef[42] = 63.7;
            g_IGRF_coef[43] = -15.0;
            g_IGRF_coef[44] = -63.4;
            g_IGRF_coef[45] = 14.7;
            g_IGRF_coef[46] = 0.0;
            g_IGRF_coef[47] = -86.4;
            g_IGRF_coef[48] = 50.3;
            g_IGRF_coef[49] = 79.8;
            g_IGRF_coef[50] = -74.4;
            g_IGRF_coef[51] = -61.4;
            g_IGRF_coef[52] = -1.4;
            g_IGRF_coef[53] = -22.5;
            g_IGRF_coef[54] = 38.6;
            g_IGRF_coef[55] = 6.9;
            g_IGRF_coef[56] = 12.3;
            g_IGRF_coef[57] = 25.4;
            g_IGRF_coef[58] = 9.4;
            g_IGRF_coef[59] = 10.9;
            g_IGRF_coef[60] = 5.5;
            g_IGRF_coef[61] = -26.4;
            g_IGRF_coef[62] = 2.0;
            g_IGRF_coef[63] = -4.8;
            g_IGRF_coef[64] = 24.8;
            g_IGRF_coef[65] = 7.7;
            g_IGRF_coef[66] = 11.2;
            g_IGRF_coef[67] = -11.4;
            g_IGRF_coef[68] = -21.0;
            g_IGRF_coef[69] = -6.8;
            g_IGRF_coef[70] = 9.7;
            g_IGRF_coef[71] = -18.0;
            g_IGRF_coef[72] = -19.8;
            g_IGRF_coef[73] = 10.0;
            g_IGRF_coef[74] = 16.1;
            g_IGRF_coef[75] = 9.4;
            g_IGRF_coef[76] = 7.7;
            g_IGRF_coef[77] = -11.4;
            g_IGRF_coef[78] = -12.8;
            g_IGRF_coef[79] = -5.0;
            g_IGRF_coef[80] = -0.1;
            g_IGRF_coef[81] = 5.6;
            g_IGRF_coef[82] = 9.8;
            g_IGRF_coef[83] = -20.1;
            g_IGRF_coef[84] = 3.6;
            g_IGRF_coef[85] = 12.9;
            g_IGRF_coef[86] = -7.0;
            g_IGRF_coef[87] = 12.7;
            g_IGRF_coef[88] = 5.0;
            g_IGRF_coef[89] = -6.7;
            g_IGRF_coef[90] = -10.8;
            g_IGRF_coef[91] = -8.1;
            g_IGRF_coef[92] = -1.3;
            g_IGRF_coef[93] = 8.1;
            g_IGRF_coef[94] = 8.7;
            g_IGRF_coef[95] = 2.9;
            g_IGRF_coef[96] = -6.7;
            g_IGRF_coef[97] = -7.9;
            g_IGRF_coef[98] = -9.2;
            g_IGRF_coef[99] = 5.9;
            g_IGRF_coef[100] = -2.2;
            g_IGRF_coef[101] = -6.3;
            g_IGRF_coef[102] = 2.4;
            g_IGRF_coef[103] = 1.6;
            g_IGRF_coef[104] = 0.2;
            g_IGRF_coef[105] = -2.5;
            g_IGRF_coef[106] = 4.4;
            g_IGRF_coef[107] = -0.1;
            g_IGRF_coef[108] = 4.7;
            g_IGRF_coef[109] = 3.0;
            g_IGRF_coef[110] = -6.5;
            g_IGRF_coef[111] = 0.3;
            g_IGRF_coef[112] = -1.0;
            g_IGRF_coef[113] = 2.1;
            g_IGRF_coef[114] = -3.4;
            g_IGRF_coef[115] = 3.9;
            g_IGRF_coef[116] = -0.9;
            g_IGRF_coef[117] = -0.1;
            g_IGRF_coef[118] = -2.3;
            g_IGRF_coef[119] = -2.2;
            g_IGRF_coef[120] = -8.0;
        }
        g_IGRF_sec[0] = 0.0;
        g_IGRF_sec[1] = 8.8;
        g_IGRF_sec[2] = 10.8;
        g_IGRF_sec[3] = -21.3;
        g_IGRF_sec[4] = -15.0;
        g_IGRF_sec[5] = -6.9;
        g_IGRF_sec[6] = -23.3;
        g_IGRF_sec[7] = -1.0;
        g_IGRF_sec[8] = -14.0;
        g_IGRF_sec[9] = -0.3;
        g_IGRF_sec[10] = -3.1;
        g_IGRF_sec[11] = 5.4;
        g_IGRF_sec[12] = -0.9;
        g_IGRF_sec[13] = -6.5;
        g_IGRF_sec[14] = -6.8;
        g_IGRF_sec[15] = -2.0;
        g_IGRF_sec[16] = -2.5;
        g_IGRF_sec[17] = 2.8;
        g_IGRF_sec[18] = 2.0;
        g_IGRF_sec[19] = -7.1;
        g_IGRF_sec[20] = 1.8;
        g_IGRF_sec[21] = 5.9;
        g_IGRF_sec[22] = 5.6;
        g_IGRF_sec[23] = -3.2;
        g_IGRF_sec[24] = 0.0;
        g_IGRF_sec[25] = -2.6;
        g_IGRF_sec[26] = 0.4;
        g_IGRF_sec[27] = 0.1;
        g_IGRF_sec[28] = -3.0;
        g_IGRF_sec[29] = 1.8;
        g_IGRF_sec[30] = -1.2;
        g_IGRF_sec[31] = 2.0;
        g_IGRF_sec[32] = 0.2;
        g_IGRF_sec[33] = 4.5;
        g_IGRF_sec[34] = -0.6;
        g_IGRF_sec[35] = -1.0;
        g_IGRF_sec[36] = -0.8;
        g_IGRF_sec[37] = 0.2;
        g_IGRF_sec[38] = -0.4;
        g_IGRF_sec[39] = -0.2;
        g_IGRF_sec[40] = -1.9;
        g_IGRF_sec[41] = 2.1;
        g_IGRF_sec[42] = -0.4;
        g_IGRF_sec[43] = -2.1;
        g_IGRF_sec[44] = -0.4;
        g_IGRF_sec[45] = -0.4;
        g_IGRF_sec[46] = -0.2;
        g_IGRF_sec[47] = 1.3;
        g_IGRF_sec[48] = 0.9;
        g_IGRF_sec[49] = -0.4;
        g_IGRF_sec[50] = -0.0;
        g_IGRF_sec[51] = 0.8;
        g_IGRF_sec[52] = -0.2;
        g_IGRF_sec[53] = 0.4;
        g_IGRF_sec[54] = 1.1;
        g_IGRF_sec[55] = 0.1;
        g_IGRF_sec[56] = 0.6;
        g_IGRF_sec[57] = 0.2;
        g_IGRF_sec[58] = 0.4;
        g_IGRF_sec[59] = -0.9;
        g_IGRF_sec[60] = -0.5;
        g_IGRF_sec[61] = -0.3;
        g_IGRF_sec[62] = 0.9;
        g_IGRF_sec[63] = 0.3;
        g_IGRF_sec[64] = -0.2;
        g_IGRF_sec[65] = 0.2;
        g_IGRF_sec[66] = -0.2;
        g_IGRF_sec[67] = -0.2;
        g_IGRF_sec[68] = 0.2;
        g_IGRF_sec[69] = 0.2;
        g_IGRF_sec[70] = 0.2;
        g_IGRF_sec[71] = -0.2;
        g_IGRF_sec[72] = 0.4;
        g_IGRF_sec[73] = 0.2;
        g_IGRF_sec[74] = 0.2;
        g_IGRF_sec[75] = 0.5;
        g_IGRF_sec[76] = -0.3;
        g_IGRF_sec[77] = -0.7;
        g_IGRF_sec[78] = 0.5;
        g_IGRF_sec[79] = 0.5;
        g_IGRF_sec[80] = 0.4;
        for (num = 81; num < 121; num++)
        {
            g_IGRF_sec[num] = 0.0;
        }
        if (IGRF_year == 2010)
        {
            g_IGRF_year = 2010;
        }
        else
        {
            g_IGRF_year = 2005;
        }
        num2 = (double)epoch_year + epoch_dn / 365.25;
        g_ghe[0] = 0.0;
        num3 = num2 - (double)g_IGRF_year;
        for (num = 1; num <= 120; num++)
        {
            g_ghe[num] = g_IGRF_coef[num] + num3 * g_IGRF_sec[num];
        }
        g_G[0] = 0.0;
        num6 = 10;
        g_G[1] = 0.0;
        num4 = 2;
        num9 = -1E-05;
        for (num5 = 1; num5 <= num6; num5++)
        {
            num10 = (double)num5 / 2.0;
            num9 *= num10;
            num8 = num9 * 0.5;
            num8 *= Math.Sqrt(2.0);
            g_G[num4] = g_ghe[num4 - 1] * num9;
            num4++;
            for (num7 = 1; num7 <= num5; num7++)
            {
                num10 = (double)(num5 + num7) / ((double)(num5 - num7) + 1.0);
                num8 *= Math.Sqrt(num10);
                g_G[num4] = g_ghe[num4 - 1] * num8;
                g_G[num4 + 1] = g_ghe[num4] * num8;
                num4 += 2;
            }
        }
        for (num4 = 122; num4 <= 170; num4++)
        {
            g_G[num4] = 0.0;
        }
        return g_IGRF_year;
    }

    public DenseVector Calculate_B_ECR(DenseVector pos_ecr_km)
    {
        double[] array = new double[4];
        double[] array2 = new double[171];
        double num = 6371.2;
        int num2 = 10;
        double num3 = pos_ecr_km[0] / num;
        double num4 = pos_ecr_km[1] / num;
        double num5 = pos_ecr_km[2] / num;
        double num6 = 1.0 / (num3 * num3 + num4 * num4 + num5 * num5);
        array[1] = num3 * num6;
        array[2] = num4 * num6;
        array[3] = num5 * num6;
        int num7 = num2 * num2 + 1;
        int num8 = num7 + num2 + num2;
        int num9 = num2 + num2 - 1;
        for (int i = num7; i <= num8; i++)
        {
            array2[i] = g_G[i];
        }
        int num10 = 1;
        int num11 = 1;
        while (num11 == 1)
        {
            int i = num9;
            int num12 = num7;
            int num13 = 1;
            while (num13 == 1)
            {
                int num14 = num12 - i;
                double num15 = 2.0 / ((double)(i - num10) + 2.0);
                double num16 = array[1] * num15;
                double num17 = array[2] * num15;
                double num18 = array[3] * (num15 + num15);
                i -= 2;
                if (i - 1 > 0)
                {
                    int num19 = 3;
                    int num20 = 1;
                    while (num20 == 1)
                    {
                        array2[num14 + num19 + 1] = g_G[num14 + num19 + 1] + num18 * array2[num12 + num19 + 1] + num16 * (array2[num12 + num19 + 3] - array2[num12 + num19 - 1]) - num17 * (array2[num12 + num19 + 2] + array2[num12 + num19 - 2]);
                        array2[num14 + num19] = g_G[num14 + num19] + num18 * array2[num12 + num19] + num16 * (array2[num12 + num19 + 2] - array2[num12 + num19 - 2]) + num17 * (array2[num12 + num19 + 3] + array2[num12 + num19 - 1]);
                        num19 += 2;
                        if (num19 == i + 2)
                        {
                            num20 = 0;
                        }
                    }
                }
                if (i - 1 >= 0)
                {
                    array2[num14 + 2] = g_G[num14 + 2] + num18 * array2[num12 + 2] + num16 * array2[num12 + 4] - num17 * (array2[num12 + 3] + array2[num12]);
                    array2[num14 + 1] = g_G[num14 + 1] + num18 * array2[num12 + 1] + num17 * array2[num12 + 4] + num16 * (array2[num12 + 3] - array2[num12]);
                }
                array2[num14] = g_G[num14] + num18 * array2[num12] + 2.0 * (num16 * array2[num12 + 1] + num17 * array2[num12 + 2]);
                num12 = num14;
                if (i < num10)
                {
                    num13 = 0;
                }
            }
            num10 += 2;
            if (num10 == 5)
            {
                num11 = 0;
            }
        }
        double num21 = 0.5 * array2[1] + 2.0 * (array2[2] * array[3] + array2[3] * array[1] + array2[4] * array[2]);
        double num22 = (num6 + num6) * Math.Sqrt(num6);
        DenseVector denseVector = new DenseVector(3);
        denseVector[0] = 100.0 * num22 * (array2[3] - num21 * num3);
        denseVector[1] = 100.0 * num22 * (array2[4] - num21 * num4);
        denseVector[2] = 100.0 * num22 * (array2[2] - num21 * num5);
        return denseVector;
    }
}
