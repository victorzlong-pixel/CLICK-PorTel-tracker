using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYMath
{
    public const double PI = Math.PI;

    public const double DTR = Math.PI / 180.0;

    public const double RTD = 180.0 / Math.PI;

    public const double RTAS = 648000.0 / Math.PI;

    public const double ASTR = 4.84813681109536E-06;

    private static Random m_rnd = new Random();

    public static double sind(double angle_deg)
    {
        return Math.Sin(angle_deg * (Math.PI / 180.0));
    }

    public static double cosd(double angle_deg)
    {
        return Math.Cos(angle_deg * (Math.PI / 180.0));
    }

    public static DenseVector vector_3x1_cross(DenseVector A, DenseVector B)
    {
        DenseVector denseVector = new DenseVector(3);
        denseVector[0] = A[1] * B[2] - A[2] * B[1];
        denseVector[1] = A[2] * B[0] - A[0] * B[2];
        denseVector[2] = A[0] * B[1] - A[1] * B[0];
        return denseVector;
    }

    public static double vector_3x1_normalize(DenseVector A, out DenseVector output)
    {
        output = new DenseVector(3);
        double num = Math.Sqrt(A[0] * A[0] + A[1] * A[1] + A[2] * A[2]);
        double num2 = 1.0 / num;
        output[0] = A[0] * num2;
        output[1] = A[1] * num2;
        output[2] = A[2] * num2;
        return num;
    }

    public static double vector_3x1_interangle_small_rad(DenseVector A, DenseVector B)
    {
        vector_3x1_normalize(A, out var output);
        vector_3x1_normalize(B, out var output2);
        double d = vector_3x1_cross(output, output2).Norm(2.0);
        return Math.Asin(d);
    }

    public static DenseMatrix vector_3x1_to_skew_sym_matrix_3x3_shuster(DenseVector vec)
    {
        DenseMatrix denseMatrix = new DenseMatrix(3);
        denseMatrix[0, 0] = 0.0;
        denseMatrix[0, 1] = vec[2];
        denseMatrix[0, 2] = 0.0 - vec[1];
        denseMatrix[1, 0] = 0.0 - vec[2];
        denseMatrix[1, 1] = 0.0;
        denseMatrix[1, 2] = vec[0];
        denseMatrix[2, 0] = vec[1];
        denseMatrix[2, 1] = 0.0 - vec[0];
        denseMatrix[2, 2] = 0.0;
        return denseMatrix;
    }

    public static DenseMatrix vector_3x1_to_skew_sym_matrix_3x3_cross(DenseVector vec)
    {
        DenseMatrix denseMatrix = new DenseMatrix(3);
        denseMatrix[0, 0] = 0.0;
        denseMatrix[0, 1] = 0.0 - vec[2];
        denseMatrix[0, 2] = vec[1];
        denseMatrix[1, 0] = vec[2];
        denseMatrix[1, 1] = 0.0;
        denseMatrix[1, 2] = 0.0 - vec[0];
        denseMatrix[2, 0] = 0.0 - vec[1];
        denseMatrix[2, 1] = vec[0];
        denseMatrix[2, 2] = 0.0;
        return denseMatrix;
    }

    public static DenseMatrix vector_3x1_to_skew_sym_matrix_3x3_cross(DenseMatrix vec)
    {
        DenseMatrix denseMatrix = new DenseMatrix(3);
        denseMatrix[0, 0] = 0.0;
        denseMatrix[0, 1] = 0.0 - vec[0, 2];
        denseMatrix[0, 2] = vec[0, 1];
        denseMatrix[1, 0] = vec[0, 2];
        denseMatrix[1, 1] = 0.0;
        denseMatrix[1, 2] = 0.0 - vec[0, 0];
        denseMatrix[2, 0] = 0.0 - vec[0, 1];
        denseMatrix[2, 1] = vec[0, 0];
        denseMatrix[2, 2] = 0.0;
        return denseMatrix;
    }

    public static DenseMatrix Euler321deg_to_DCM(double roll_deg, double pitch_deg, double yaw_deg)
    {
        return Euler321rad_to_DCM(roll_deg * (Math.PI / 180.0), pitch_deg * (Math.PI / 180.0), yaw_deg * (Math.PI / 180.0));
    }

    public static DenseMatrix Euler321rad_to_DCM(double roll_rad, double pitch_rad, double yaw_rad)
    {
        double num = Math.Sin(roll_rad);
        double num2 = Math.Cos(roll_rad);
        double num3 = Math.Sin(pitch_rad);
        double num4 = Math.Cos(pitch_rad);
        double num5 = Math.Sin(yaw_rad);
        double num6 = Math.Cos(yaw_rad);
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        denseMatrix[0, 0] = num4 * num6;
        denseMatrix[0, 1] = num4 * num5;
        denseMatrix[0, 2] = 0.0 - num3;
        denseMatrix[1, 0] = (0.0 - num2) * num5 + num * num3 * num6;
        denseMatrix[1, 1] = num2 * num6 + num * num3 * num5;
        denseMatrix[1, 2] = num * num4;
        denseMatrix[2, 0] = num * num5 + num2 * num3 * num6;
        denseMatrix[2, 1] = (0.0 - num) * num6 + num2 * num3 * num5;
        denseMatrix[2, 2] = num2 * num4;
        return denseMatrix;
    }

    public static DenseMatrix Euler321deg_to_DCM(double[] EulerAngles_deg)
    {
        return Euler321deg_to_DCM(EulerAngles_deg[0], EulerAngles_deg[1], EulerAngles_deg[2]);
    }

    public static DenseMatrix Euler213deg_to_DCM(double[] Angle_deg)
    {
        return Euler213deg_to_DCM(Angle_deg[0], Angle_deg[1], Angle_deg[2]);
    }

    public static DenseMatrix Euler213deg_to_DCM(double roll_deg, double pitch_deg, double yaw_deg)
    {
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        double num = roll_deg * (Math.PI / 180.0);
        double num2 = pitch_deg * (Math.PI / 180.0);
        double num3 = yaw_deg * (Math.PI / 180.0);
        double num4 = Math.Sin(num);
        double num5 = Math.Cos(num);
        double num6 = Math.Sin(num2);
        double num7 = Math.Cos(num2);
        double num8 = Math.Sin(num3);
        double num9 = Math.Cos(num3);
        denseMatrix[0, 0] = num9 * num7 + num8 * num4 * num6;
        denseMatrix[0, 1] = num8 * num5;
        denseMatrix[0, 2] = (0.0 - num9) * num6 + num8 * num4 * num7;
        denseMatrix[1, 0] = (0.0 - num8) * num7 + num9 * num4 * num6;
        denseMatrix[1, 1] = num9 * num5;
        denseMatrix[1, 2] = num8 * num6 + num9 * num4 * num7;
        denseMatrix[2, 0] = num5 * num6;
        denseMatrix[2, 1] = 0.0 - num4;
        denseMatrix[2, 2] = num5 * num7;
        return denseMatrix;
    }

    public static void DCM_to_Euler321deg(double[,] DCM, out double roll_deg, out double pitch_deg, out double yaw_deg)
    {
        roll_deg = Math.Atan2(DCM[1, 2], DCM[2, 2]) * (180.0 / Math.PI);
        pitch_deg = (0.0 - Math.Asin(DCM[0, 2])) * (180.0 / Math.PI);
        yaw_deg = Math.Atan2(DCM[0, 1], DCM[0, 0]) * (180.0 / Math.PI);
    }

    public static void DCM_to_Euler321deg(DenseMatrix DCM, out double roll_deg, out double pitch_deg, out double yaw_deg)
    {
        double[,] array = new double[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                array[i, j] = DCM[i, j];
            }
        }
        DCM_to_Euler321deg(array, out roll_deg, out pitch_deg, out yaw_deg);
    }

    public static DenseVector DCM_to_Euler321deg(DenseMatrix DCM)
    {
        DenseVector denseVector = new DenseVector(3);
        DCM_to_Euler321deg(DCM, out var roll_deg, out var pitch_deg, out var yaw_deg);
        denseVector[0] = roll_deg;
        denseVector[1] = pitch_deg;
        denseVector[2] = yaw_deg;
        return denseVector;
    }

    public static void DCM_to_Euler213deg(double[,] DCM, out double roll_deg, out double pitch_deg, out double yaw_deg)
    {
        roll_deg = (0.0 - Math.Asin(DCM[2, 1])) * (180.0 / Math.PI);
        pitch_deg = Math.Atan(DCM[2, 0] / DCM[2, 2]) * (180.0 / Math.PI);
        yaw_deg = Math.Atan(DCM[0, 1] / DCM[1, 1]) * (180.0 / Math.PI);
    }

    public static void DCM_to_Euler213deg(DenseMatrix DCM, out double roll_deg, out double pitch_deg, out double yaw_deg)
    {
        double[,] array = new double[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                array[i, j] = DCM[i, j];
            }
        }
        DCM_to_Euler213deg(array, out roll_deg, out pitch_deg, out yaw_deg);
    }

    public static DenseVector DCM_to_Euler213deg(DenseMatrix DCM)
    {
        DenseVector denseVector = new DenseVector(3);
        DCM_to_Euler213deg(DCM, out var roll_deg, out var pitch_deg, out var yaw_deg);
        denseVector[0] = roll_deg;
        denseVector[1] = pitch_deg;
        denseVector[2] = yaw_deg;
        return denseVector;
    }

    public static void DCM_to_quaternion(double[,] DCM, double[] q)
    {
        double num = DCM[0, 0] + DCM[1, 1] + DCM[2, 2];
        if (num > 3.0)
        {
            num = 3.0;
        }
        else if (num < -1.0)
        {
            num = -1.0;
        }
        q[3] = 0.5 * Math.Sqrt(1.0 + num);
        if (num == -1.0)
        {
            q[0] = Math.Sqrt((1.0 + DCM[0, 0]) / 2.0);
            q[1] = Math.Sqrt((1.0 + DCM[1, 1]) / 2.0);
            q[2] = Math.Sqrt((1.0 + DCM[2, 2]) / 2.0);
            if (DCM[0, 1] > 0.0 && DCM[1, 2] < 0.0)
            {
                q[2] = 0.0 - q[2];
            }
            else if (DCM[0, 1] < 0.0 && DCM[1, 2] > 0.0)
            {
                q[0] = 0.0 - q[0];
            }
            else if (DCM[0, 1] < 0.0 && DCM[1, 2] < 0.0)
            {
                q[1] = 0.0 - q[1];
            }
            else if (DCM[0, 1] < 0.0 && DCM[1, 2] == 0.0)
            {
                q[1] = 0.0 - q[1];
            }
            else if (DCM[0, 1] == 0.0 && DCM[1, 2] < 0.0)
            {
                q[2] = 0.0 - q[2];
            }
            double num2 = Math.Sqrt(1.0 - q[3] * q[3]);
            q[0] *= num2;
            q[1] *= num2;
            q[2] *= num2;
        }
        else if (num == 3.0)
        {
            q[0] = (q[1] = (q[2] = 0.0));
        }
        else
        {
            q[0] = 0.25 * (DCM[1, 2] - DCM[2, 1]) / q[3];
            q[1] = 0.25 * (DCM[2, 0] - DCM[0, 2]) / q[3];
            q[2] = 0.25 * (DCM[0, 1] - DCM[1, 0]) / q[3];
        }
        double num3 = Math.Sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2] + q[3] * q[3]);
        q[0] /= num3;
        q[1] /= num3;
        q[2] /= num3;
        q[3] /= num3;
    }

    public static void DCM_to_quaternion(DenseMatrix DCM, out DenseVector q)
    {
        double[] array = new double[4];
        double[,] dCM = DCM.ToArray();
        DCM_to_quaternion(dCM, array);
        q = new DenseVector(4);
        q[0] = array[0];
        q[1] = array[1];
        q[2] = array[2];
        q[3] = array[3];
    }

    public static Quaternion DCM_to_quaternion(DenseMatrix DCM)
    {
        double[] array = new double[4];
        double[,] dCM = DCM.ToArray();
        DCM_to_quaternion(dCM, array);
        Quaternion quaternion = new Quaternion();
        quaternion[0] = array[0];
        quaternion[1] = array[1];
        quaternion[2] = array[2];
        quaternion[3] = array[3];
        return quaternion;
    }

    public static Quaternion DCM_to_quaternion(double[,] DCM)
    {
        double[] array = new double[4];
        DCM_to_quaternion(DCM, array);
        Quaternion quaternion = new Quaternion();
        quaternion[0] = array[0];
        quaternion[1] = array[1];
        quaternion[2] = array[2];
        quaternion[3] = array[3];
        return quaternion;
    }

    public static DenseMatrix quaternion_to_DCM(Quaternion q)
    {
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        denseMatrix[0, 0] = q[0] * q[0] - q[1] * q[1] - q[2] * q[2] + q[3] * q[3];
        denseMatrix[1, 0] = 2.0 * (q[0] * q[1] - q[2] * q[3]);
        denseMatrix[2, 0] = 2.0 * (q[0] * q[2] + q[1] * q[3]);
        denseMatrix[0, 1] = 2.0 * (q[0] * q[1] + q[2] * q[3]);
        denseMatrix[1, 1] = (0.0 - q[0]) * q[0] + q[1] * q[1] - q[2] * q[2] + q[3] * q[3];
        denseMatrix[2, 1] = 2.0 * (q[1] * q[2] - q[0] * q[3]);
        denseMatrix[0, 2] = 2.0 * (q[0] * q[2] - q[1] * q[3]);
        denseMatrix[1, 2] = 2.0 * (q[1] * q[2] + q[0] * q[3]);
        denseMatrix[2, 2] = (0.0 - q[0]) * q[0] - q[1] * q[1] + q[2] * q[2] + q[3] * q[3];
        return denseMatrix;
    }

    public static DenseVector quaternion_to_Euler321deg(Quaternion q)
    {
        DenseMatrix dCM = quaternion_to_DCM(q);
        return DCM_to_Euler321deg(dCM);
    }

    public static DenseVector quaternion_to_Euler213deg(Quaternion q)
    {
        DenseMatrix dCM = quaternion_to_DCM(q);
        return DCM_to_Euler213deg(dCM);
    }

    public static Quaternion Euler321deg_to_quaternion(double roll_deg, double pitch_deg, double yaw_deg)
    {
        DenseMatrix dCM = Euler321deg_to_DCM(roll_deg, pitch_deg, yaw_deg);
        return DCM_to_quaternion(dCM);
    }

    public static Quaternion Euler321deg_to_quaternion(double[] Angle_deg)
    {
        return Euler321deg_to_quaternion(Angle_deg[0], Angle_deg[1], Angle_deg[2]);
    }

    public static Quaternion smallangle_to_quaternion(double[] Angle_deg)
    {
        Quaternion quaternion = new Quaternion();
        for (int i = 0; i < 3; i++)
        {
            quaternion[i] = Angle_deg[i] * (Math.PI / 180.0) * 0.5;
        }
        quaternion.NormalizeSelf();
        return quaternion;
    }

    public static void randSet(int seed)
    {
        m_rnd = new Random(seed);
    }

    public static double randn()
    {
        return randn(m_rnd);
    }

    public static DenseVector randn(int Vec_dimension_N)
    {
        DenseVector denseVector = new DenseVector(Vec_dimension_N);
        for (int i = 0; i < Vec_dimension_N; i++)
        {
            denseVector[i] = randn();
        }
        return denseVector;
    }

    public static double randn(Random random)
    {
        double num = -6.0;
        for (int i = 0; i < 12; i++)
        {
            num += random.NextDouble();
        }
        return num;
    }

    public static double randu(Random random)
    {
        return random.NextDouble();
    }

    public static double randu()
    {
        return m_rnd.NextDouble();
    }

    public static double fmod2p(double x)
    {
        x %= Math.PI * 2.0;
        if (x < 0.0)
        {
            x += Math.PI * 2.0;
        }
        return x;
    }

    public static int binary_search(double e, double[] Data, int[] Index)
    {
        return binary_search(e, Data, Data.Length, Index);
    }

    public static int binary_search(double e, double[] Data, int N, int[] Index)
    {
        int num = 0;
        int num2 = 0;
        int num3 = N - 1;
        while (true)
        {
            int num4 = num3 - num2;
            if (num4 == 1)
            {
                Index[0] = num2;
                Index[1] = num3;
                break;
            }
            if (num4 < 1)
            {
                Index[0] = int.MaxValue;
                Index[1] = int.MaxValue;
                return -1;
            }
            num = num4 / 2 + num2;
            if (e > Data[num])
            {
                num2 = num;
                continue;
            }
            if (e < Data[num])
            {
                num3 = num;
                continue;
            }
            if (e == Data[num])
            {
                Index[0] = num;
                Index[1] = num + 1;
                break;
            }
            Index[0] = int.MaxValue;
            Index[1] = int.MaxValue;
            return -2;
        }
        return 1;
    }

    public static Quaternion quaternion_small_angle_propagate(Quaternion q_input, DenseVector w_rad_sec, double dt_sec)
    {
        Quaternion quaternion = new Quaternion();
        DenseVector output;
        double num = vector_3x1_normalize(w_rad_sec, out output);
        double num2 = num * dt_sec * 0.5;
        quaternion[3] = Math.Cos(num2);
        if (quaternion[3] == 1.0)
        {
            quaternion[0] = w_rad_sec[0] * dt_sec * 0.5;
            quaternion[1] = w_rad_sec[1] * dt_sec * 0.5;
            quaternion[2] = w_rad_sec[2] * dt_sec * 0.5;
        }
        else
        {
            double num3 = Math.Sin(num2);
            quaternion[0] = num3 * output[0];
            quaternion[1] = num3 * output[1];
            quaternion[2] = num3 * output[2];
        }
        Quaternion quaternion2 = quaternion * q_input;
        quaternion2.NormalizeSelf();
        return quaternion2;
    }

    public static double average(double[] x, int data_N)
    {
        double num = 0.0;
        for (int i = 0; i < data_N; i++)
        {
            num += x[i];
        }
        num /= (double)data_N;
        return data_N;
    }

    public static int average_standarddeviation(double[] x, int data_N, out double av, out double st)
    {
        if (data_N < 2)
        {
            av = x[0];
            st = 0.0;
            return -1;
        }
        av = 0.0;
        for (int i = 0; i < data_N; i++)
        {
            av += x[i];
        }
        av /= data_N;
        st = 0.0;
        for (int i = 0; i < data_N; i++)
        {
            double num = x[i] - av;
            st += num * num;
        }
        st /= data_N - 1;
        st = Math.Sqrt(st);
        return 1;
    }

    public static int average_standarddeviation(double[] x, out double av, out double st)
    {
        return average_standarddeviation(x, x.Length, out av, out st);
    }

    public static Quaternion vector_3x1_3x1_to_quaternion(DenseVector a, DenseVector z)
    {
        DenseVector denseVector = new DenseVector(3);
        vector_3x1_normalize(a, out var output);
        vector_3x1_normalize(z, out var output2);
        denseVector = vector_3x1_cross(output2, output);
        double num = output2.DotProduct(output);
        Quaternion quaternion = new Quaternion();
        quaternion[3] = Math.Sqrt((1.0 + num) * 0.5);
        quaternion[0] = denseVector[0] / (2.0 * quaternion[3]);
        quaternion[1] = denseVector[1] / (2.0 * quaternion[3]);
        quaternion[2] = denseVector[2] / (2.0 * quaternion[3]);
        return quaternion;
    }

    public static Quaternion QUEST(DenseVector[] v_body, DenseVector[] v_eci, double[] w)
    {
        return QUEST(v_body.Length, v_body, v_eci, w);
    }

    public static Quaternion QUEST(int Data_N, DenseVector[] v_body, DenseVector[] v_eci, double[] w)
    {
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        DenseVector denseVector = new DenseVector(3);
        DenseMatrix denseMatrix2 = new DenseMatrix(3, 3);
        DenseVector denseVector2 = new DenseVector(3);
        for (int i = 0; i < Data_N; i++)
        {
            denseMatrix += w[i] * (DenseMatrix)v_body[i].OuterProduct(v_eci[i]);
        }
        DenseMatrix denseMatrix3 = denseMatrix + (DenseMatrix)denseMatrix.Transpose();
        denseVector[0] = denseMatrix[1, 2] - denseMatrix[2, 1];
        denseVector[1] = denseMatrix[2, 0] - denseMatrix[0, 2];
        denseVector[2] = denseMatrix[0, 1] - denseMatrix[1, 0];
        double num = denseMatrix[0, 0] + denseMatrix[1, 1] + denseMatrix[2, 2];
        double num2 = 0.0;
        for (int j = 0; j < Data_N; j++)
        {
            num2 += w[j];
        }
        denseMatrix2[0, 0] = num2 + num;
        denseMatrix2[1, 1] = num2 + num;
        denseMatrix2[2, 2] = num2 + num;
        denseMatrix2 -= denseMatrix3;
        denseVector2 = (DenseMatrix)denseMatrix2.Inverse() * denseVector;
        Quaternion quaternion = new Quaternion();
        double num3 = Math.Sqrt(1.0 + denseVector2[0] * denseVector2[0] + denseVector2[1] * denseVector2[1] + denseVector2[2] * denseVector2[2]);
        quaternion[0] = denseVector2[0] / num3;
        quaternion[1] = denseVector2[1] / num3;
        quaternion[2] = denseVector2[2] / num3;
        quaternion[3] = 1.0 / num3;
        return quaternion;
    }

    public static DenseMatrix TRIAD(DenseVector[] V_r, DenseVector[] V_b)
    {
        vector_3x1_normalize(V_r[0], out var output);
        DenseVector output2 = vector_3x1_cross(output, V_r[1]);
        vector_3x1_normalize(output2, out output2);
        DenseVector denseVector = vector_3x1_cross(output, output2);
        vector_3x1_normalize(V_b[0], out var output3);
        DenseVector output4 = vector_3x1_cross(output3, V_b[1]);
        vector_3x1_normalize(output4, out output4);
        DenseVector denseVector2 = vector_3x1_cross(output3, output4);
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        DenseMatrix denseMatrix2 = new DenseMatrix(3, 3);
        for (int i = 0; i < 3; i++)
        {
            denseMatrix[0, i] = output[i];
            denseMatrix[1, i] = output2[i];
            denseMatrix[2, i] = denseVector[i];
            denseMatrix2[i, 0] = output3[i];
            denseMatrix2[i, 1] = output4[i];
            denseMatrix2[i, 2] = denseVector2[i];
        }
        return denseMatrix2 * denseMatrix;
    }

    private static int merge(int[] a, int N, int starta, int[] b, int M, int startb, int[] c, int startc, double[] compared_data)
    {
        int num = 0;
        int num2 = 0;
        if (M < 1)
        {
            for (int i = 0; i < N + M; i++)
            {
                c[i + startc] = a[num + starta];
                num++;
            }
        }
        else
        {
            for (int j = 0; j < N + M; j++)
            {
                if (compared_data[a[num + starta]] < compared_data[b[num2 + startb]])
                {
                    c[j + startc] = a[num + starta];
                    num++;
                    if (num == N)
                    {
                        for (int i = j + 1; i < N + M; i++)
                        {
                            c[i + startc] = b[num2 + startb];
                            num2++;
                        }
                        return 0;
                    }
                    continue;
                }
                c[j + startc] = b[num2 + startb];
                num2++;
                if (num2 == M)
                {
                    for (int i = j + 1; i < N + M; i++)
                    {
                        c[i + startc] = a[num + starta];
                        num++;
                    }
                    return 0;
                }
            }
        }
        return 0;
    }

    public static int[] sort_merge(int Data_N, double[] Data)
    {
        int[] array = new int[Data_N];
        int[] array2 = new int[Data_N];
        int i;
        for (i = 0; i < Data_N; i++)
        {
            array[i] = i;
        }
        int num = 1;
        int num2 = 0;
        int num3 = 0;
        i = 0;
        while (num < Data_N)
        {
            int num4 = (int)Math.Ceiling((double)Data_N / (double)(num * 2));
            int num5 = 0;
            int num6 = 0;
            for (i = 0; i < num4 - 1; i++)
            {
                num5 = i * num * 2;
                num6 = num5 + num;
                if (num2 == 0)
                {
                    merge(array, num, num5, array, num, num6, array2, num5, Data);
                }
                else
                {
                    merge(array2, num, num5, array2, num, num6, array, num5, Data);
                }
            }
            num5 = i * num * 2;
            num6 = num5 + num;
            num3 = Data_N - num6;
            if (num2 == 0)
            {
                merge(array, num, num5, array, num3, num6, array2, num5, Data);
            }
            else
            {
                merge(array2, num, num5, array2, num3, num6, array, num5, Data);
            }
            num2 = ((num2 == 0) ? 1 : 0);
            num *= 2;
        }
        int[] array3 = new int[Data_N];
        if (num2 == 0)
        {
            for (i = 0; i < Data_N; i++)
            {
                array3[i] = array[i];
            }
        }
        else
        {
            for (i = 0; i < Data_N; i++)
            {
                array3[i] = array2[i];
            }
        }
        return array3;
    }

    public static int[] sort_merge(double[] Data)
    {
        return sort_merge(Data.Length, Data);
    }

    public static int[] sort_merge(int[] Data)
    {
        double[] array = new double[Data.Length];
        for (int i = 0; i < Data.Length; i++)
        {
            array[i] = Data[i];
        }
        return sort_merge(array.Length, array);
    }

    public static int[] sort_merge(uint[] Data)
    {
        double[] array = new double[Data.Length];
        for (int i = 0; i < Data.Length; i++)
        {
            array[i] = Data[i];
        }
        return sort_merge(array.Length, array);
    }

    public static int[] sort_merge(float[] Data)
    {
        double[] array = new double[Data.Length];
        for (int i = 0; i < Data.Length; i++)
        {
            array[i] = Data[i];
        }
        return sort_merge(array.Length, array);
    }

    public static DenseVector RADEC_to_XYZ(double RA_deg, double DEC_deg)
    {
        double num = RA_deg * (Math.PI / 180.0);
        double num2 = DEC_deg * (Math.PI / 180.0);
        double num3 = Math.Cos(num);
        double num4 = Math.Sin(num);
        double num5 = Math.Cos(num2);
        double value = Math.Sin(num2);
        DenseVector denseVector = new DenseVector(3);
        denseVector[0] = num3 * num5;
        denseVector[1] = num4 * num5;
        denseVector[2] = value;
        return denseVector;
    }
}
