using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class Quaternion : DenseVector
{
    public Quaternion()
        : base(4)
    {
        base[3] = 1.0;
    }

    public static Quaternion OfQuaternion(Quaternion q)
    {
        Quaternion quaternion = new Quaternion();
        quaternion[0] = q[0];
        quaternion[1] = q[1];
        quaternion[2] = q[2];
        quaternion[3] = q[3];
        return quaternion;
    }

    public static Quaternion OfDoubleArray(double[] q)
    {
        Quaternion quaternion = new Quaternion();
        quaternion[0] = q[0];
        quaternion[1] = q[1];
        quaternion[2] = q[2];
        quaternion[3] = q[3];
        return quaternion;
    }

    public static Quaternion OfRotationAxisAngle(DenseVector Axis, double Angle_rad)
    {
        DenseVector denseVector = Axis / Axis.L2Norm();
        double num = Angle_rad * 0.5;
        double num2 = Math.Sin(num);
        double value = Math.Cos(num);
        Quaternion quaternion = new Quaternion();
        quaternion[0] = denseVector[0] * num2;
        quaternion[1] = denseVector[1] * num2;
        quaternion[2] = denseVector[2] * num2;
        quaternion[3] = value;
        return quaternion;
    }

    public static Quaternion operator *(Quaternion q1, Quaternion q2)
    {
        return quaternion_product(q1, q2);
    }

    public static Quaternion operator /(Quaternion q1, Quaternion q2)
    {
        return quaternion_error(q1, q2);
    }

    public void NormalizeSelf()
    {
        double d = base[0] * base[0] + base[1] * base[1] + base[2] * base[2] + base[3] * base[3];
        d = Math.Sqrt(d);
        for (int i = 0; i < 4; i++)
        {
            base[i] /= d;
        }
    }

    public Quaternion Normalize()
    {
        Quaternion quaternion = OfQuaternion(this);
        quaternion.NormalizeSelf();
        return quaternion;
    }

    public Quaternion InverseQ()
    {
        Quaternion quaternion = OfQuaternion(this);
        quaternion[3] = 0.0 - quaternion[3];
        return quaternion;
    }

    public void InverseSelf()
    {
        base[3] = 0.0 - base[3];
    }

    public void MakePlusQ4()
    {
        if (base[3] < 0.0)
        {
            for (int i = 0; i < 4; i++)
            {
                base[i] = 0.0 - base[i];
            }
        }
    }

    public DenseMatrix Cvt2DCM()
    {
        return HSYMath.quaternion_to_DCM(this);
    }

    private static Quaternion quaternion_product(Quaternion q1, Quaternion q2)
    {
        Quaternion quaternion = new Quaternion();
        quaternion[0] = q1[3] * q2[0] + q1[2] * q2[1] - q1[1] * q2[2] + q1[0] * q2[3];
        quaternion[1] = (0.0 - q1[2]) * q2[0] + q1[3] * q2[1] + q1[0] * q2[2] + q1[1] * q2[3];
        quaternion[2] = q1[1] * q2[0] - q1[0] * q2[1] + q1[3] * q2[2] + q1[2] * q2[3];
        quaternion[3] = (0.0 - q1[0]) * q2[0] - q1[1] * q2[1] - q1[2] * q2[2] + q1[3] * q2[3];
        return quaternion;
    }

    private static Quaternion quaternion_error(Quaternion q_ref, Quaternion q_current)
    {
        double[] array = new double[4];
        double[] array2 = new double[4];
        array[0] = q_ref[0];
        array[1] = q_ref[1];
        array[2] = q_ref[2];
        array[3] = q_ref[3];
        array2[0] = 0.0 - q_current[0];
        array2[1] = 0.0 - q_current[1];
        array2[2] = 0.0 - q_current[2];
        array2[3] = q_current[3];
        Quaternion quaternion = new Quaternion();
        quaternion[0] = array[3] * array2[0] + array[2] * array2[1] - array[1] * array2[2] + array[0] * array2[3];
        quaternion[1] = (0.0 - array[2]) * array2[0] + array[3] * array2[1] + array[0] * array2[2] + array[1] * array2[3];
        quaternion[2] = array[1] * array2[0] - array[0] * array2[1] + array[3] * array2[2] + array[2] * array2[3];
        quaternion[3] = (0.0 - array[0]) * array2[0] - array[1] * array2[1] - array[2] * array2[2] + array[3] * array2[3];
        quaternion.MakePlusQ4();
        return quaternion;
    }

    public static Quaternion RandomQ()
    {
        Quaternion quaternion = new Quaternion();
        quaternion[0] = HSYMath.randn();
        quaternion[1] = HSYMath.randn();
        quaternion[2] = HSYMath.randn();
        quaternion[3] = HSYMath.randn();
        quaternion.NormalizeSelf();
        return quaternion;
    }

    public static Quaternion RandomQ(Random _rnd)
    {
        Quaternion quaternion = new Quaternion();
        quaternion[0] = HSYMath.randn(_rnd);
        quaternion[1] = HSYMath.randn(_rnd);
        quaternion[2] = HSYMath.randn(_rnd);
        quaternion[3] = HSYMath.randn(_rnd);
        quaternion.NormalizeSelf();
        return quaternion;
    }
}
