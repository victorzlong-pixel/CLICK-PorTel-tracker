using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYKeplerOrbit
{
    public class OE
    {
        public double a;

        public double e;

        public double i;

        public double w;

        public double Omega;

        public double theta;

        public double i_deg
        {
            get
            {
                return i * (180.0 / Math.PI);
            }
            set
            {
                i = value * (Math.PI / 180.0);
            }
        }

        public double w_deg
        {
            get
            {
                return w * (180.0 / Math.PI);
            }
            set
            {
                w = value * (Math.PI / 180.0);
            }
        }

        public double Omega_deg
        {
            get
            {
                return Omega * (180.0 / Math.PI);
            }
            set
            {
                Omega = value * (Math.PI / 180.0);
            }
        }

        public double theta_deg
        {
            get
            {
                return theta * (180.0 / Math.PI);
            }
            set
            {
                theta = value * (Math.PI / 180.0);
            }
        }

        public override string ToString()
        {
            string text = "";
            text = text + "a: " + a + "\n";
            text = text + "e: " + e + "\n";
            text = text + "i: " + i + "\n";
            text = text + "w: " + w + "\n";
            text = text + "Omega: " + Omega + "\n";
            return text + "theta: " + theta + "\n";
        }
    }

    public static double mu = 398600.4418;

    public static OE RV2OE(DenseVector R, DenseVector V)
    {
        OE oE = new OE();
        double num = R.Norm(2.0);
        double num2 = V.Norm(2.0);
        double num3 = num2 * num2;
        double a = 1.0 / (2.0 / num - num3 / mu);
        DenseVector denseVector = HSYMath.vector_3x1_cross(R, V);
        denseVector = (DenseVector)denseVector.Normalize(2.0);
        oE.i = Math.Acos(denseVector[2]);
        DenseVector a2 = new DenseVector(new double[3] { 0.0, 0.0, 1.0 });
        DenseVector denseVector2 = HSYMath.vector_3x1_cross(a2, denseVector);
        double num4 = denseVector2.Norm(2.0);
        if (num4 == 0.0)
        {
            oE.Omega = 0.0;
        }
        else
        {
            denseVector2 /= num4;
            oE.Omega = Math.Atan2(denseVector2[1], denseVector2[0]);
        }
        DenseVector denseVector3 = 1.0 / mu * ((num3 - mu / num) * R - R.DotProduct(V) * V);
        double num5 = denseVector3.Norm(2.0);
        if (num5 == 0.0)
        {
            oE.w = 0.0;
        }
        else
        {
            double x = denseVector2.DotProduct(denseVector3) / num5;
            double y = HSYMath.vector_3x1_cross(denseVector2, denseVector3).DotProduct(denseVector) / num5;
            oE.w = Math.Atan2(y, x);
        }
        double x2 = denseVector3.DotProduct(R) / num5 / num;
        double y2 = HSYMath.vector_3x1_cross(denseVector3, R).DotProduct(denseVector) / num5 / num;
        oE.theta = Math.Atan2(y2, x2);
        oE.a = a;
        oE.e = num5;
        return oE;
    }

    public static OE RV2OE(DenseVector RV)
    {
        DenseVector r = (DenseVector)RV.SubVector(0, 3);
        DenseVector v = (DenseVector)RV.SubVector(3, 3);
        return RV2OE(r, v);
    }
}
