using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYFrame
{
    public const double PI = Math.PI;

    public const double DTR = Math.PI / 180.0;

    public const double RTD = 180.0 / Math.PI;

    public static DenseMatrix J2k2RPY(DenseVector Pos_j2k, DenseVector Vel_j2k)
    {
        DenseVector output = -Pos_j2k;
        HSYMath.vector_3x1_normalize(output, out output);
        DenseVector output2 = HSYMath.vector_3x1_cross(output, Vel_j2k);
        HSYMath.vector_3x1_normalize(output2, out output2);
        DenseVector denseVector = HSYMath.vector_3x1_cross(output2, output);
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        for (int i = 0; i < 3; i++)
        {
            denseMatrix[0, i] = denseVector[i];
            denseMatrix[1, i] = output2[i];
            denseMatrix[2, i] = output[i];
        }
        return denseMatrix;
    }

    public static DenseMatrix J2k2TF(DenseVector Pos_j2k, DenseVector Vel_j2k)
    {
        DenseVector denseVector = (DenseVector)Vel_j2k.Normalize(2.0);
        DenseVector denseVector2 = HSYMath.vector_3x1_cross(denseVector, Pos_j2k);
        denseVector2 = (DenseVector)denseVector2.Normalize(2.0);
        DenseVector denseVector3 = HSYMath.vector_3x1_cross(denseVector2, denseVector);
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        for (int i = 0; i < 3; i++)
        {
            denseMatrix[0, i] = denseVector3[i];
            denseMatrix[1, i] = denseVector2[i];
            denseMatrix[2, i] = denseVector[i];
        }
        return denseMatrix;
    }

    public static DenseMatrix J2k2MPT(DenseVector Pos_j2k, DenseVector Vel_j2k, DenseVector SunPos_j2k)
    {
        DenseVector output = HSYMath.vector_3x1_cross(Vel_j2k, Pos_j2k);
        HSYMath.vector_3x1_normalize(output, out output);
        DenseVector output2 = Pos_j2k - SunPos_j2k;
        HSYMath.vector_3x1_normalize(output2, out output2);
        DenseVector output3 = HSYMath.vector_3x1_cross(output, output2);
        HSYMath.vector_3x1_normalize(output3, out output3);
        output = HSYMath.vector_3x1_cross(output2, output3);
        DenseMatrix denseMatrix = new DenseMatrix(3, 3);
        for (int i = 0; i < 3; i++)
        {
            denseMatrix[0, i] = output3[i];
            denseMatrix[1, i] = output[i];
            denseMatrix[2, i] = output2[i];
        }
        return denseMatrix;
    }
}
