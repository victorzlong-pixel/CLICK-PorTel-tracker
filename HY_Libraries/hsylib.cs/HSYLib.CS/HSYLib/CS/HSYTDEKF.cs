using System;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public abstract class HSYTDEKF
{
    public DenseMatrix m_I;

    public DenseMatrix m_PHI;

    public DenseMatrix[,] m_P;

    public DenseMatrix m_Q_IK;

    public DenseMatrix m_H;

    public DenseMatrix m_M;

    public DenseMatrix m_R;

    public DenseMatrix m_K;

    public DenseVector[] m_X;

    public DenseVector m_dX;

    public DenseVector m_Z;

    public DenseVector m_Z_hat;

    public double[] m_t;

    public double m_dt;

    public bool[] m_MeasurementUpdate;

    public int m_N_lag;

    protected HSYTDEKF(int N_x, int N_dx, int N_lag)
    {
        m_N_lag = N_lag + 1;
        if (m_N_lag < 2)
        {
            m_N_lag = 2;
        }
        m_X = new DenseVector[m_N_lag];
        m_P = new DenseMatrix[m_N_lag, m_N_lag];
        m_t = new double[m_N_lag];
        m_MeasurementUpdate = new bool[m_N_lag];
        for (int i = 0; i < m_N_lag; i++)
        {
            m_X[i] = new DenseVector(N_x);
            for (int j = 0; j < m_N_lag; j++)
            {
                m_P[i, j] = new DenseMatrix(N_dx);
            }
            m_MeasurementUpdate[i] = false;
        }
    }

    public void Initialize(double t_init, DenseVector X_init, DenseMatrix P_init)
    {
        m_t[0] = t_init;
        m_X[0] = DenseVector.OfVector(X_init);
        m_P[0, 0] = DenseMatrix.OfMatrix(P_init);
    }

    protected abstract DenseVector propagateX(double t, DenseVector X0, double t0);

    protected abstract DenseVector estimateXFQ(double t, int a, int b, out DenseMatrix aFj, out DenseMatrix jFb, out DenseMatrix aQb_i, out DenseMatrix jQb);

    protected abstract void predictZ(int iFit);

    protected abstract DenseVector updateX(DenseVector X, DenseVector dX);

    public void Prediction(double t)
    {
        m_dt = t - m_t[0];
        for (int num = m_N_lag - 1; num > 0; num--)
        {
            m_t[num] = m_t[num - 1];
            m_X[num] = m_X[num - 1];
            m_MeasurementUpdate[num] = m_MeasurementUpdate[num - 1];
        }
        m_t[0] = t;
        m_X[0] = propagateX(t, m_X[1], m_t[1]);
        m_MeasurementUpdate[0] = false;
        for (int num2 = m_N_lag - 1; num2 > 0; num2--)
        {
            for (int num3 = m_N_lag - 1; num3 > 0; num3--)
            {
                m_P[num2, num3] = m_P[num2 - 1, num3 - 1];
            }
        }
        m_P[0, 0] = m_PHI * m_P[1, 1] * (DenseMatrix)m_PHI.Transpose() + m_Q_IK;
        for (int i = 1; i < m_N_lag; i++)
        {
            m_P[0, i] = m_PHI * m_P[1, i];
            m_P[i, 0] = (DenseMatrix)m_P[0, i].Transpose();
        }
    }

    private int GenFitiByInterpolation(double t, double t_tol)
    {
        int num = -1;
        int num2 = -1;
        for (int i = 0; i < m_N_lag; i++)
        {
            double num3 = t - m_t[i];
            if (Math.Abs(num3) < t_tol)
            {
                return i;
            }
            if (num3 > 0.0)
            {
                num2 = i;
                num = i - 1;
                break;
            }
        }
        if (num2 < 0)
        {
            return -2;
        }
        DenseMatrix aFj;
        DenseMatrix jFb;
        DenseMatrix aQb_i;
        DenseMatrix jQb;
        DenseVector denseVector = estimateXFQ(t, num, num2, out aFj, out jFb, out aQb_i, out jQb);
        DenseMatrix denseMatrix = aFj * jFb;
        DenseMatrix denseMatrix2 = jQb * (DenseMatrix)aFj.Transpose() * aQb_i;
        DenseMatrix denseMatrix3 = jFb - denseMatrix2 * denseMatrix;
        DenseMatrix denseMatrix4 = (DenseMatrix)denseMatrix2.Transpose();
        DenseMatrix denseMatrix5 = (DenseMatrix)denseMatrix3.Transpose();
        DenseMatrix denseMatrix6 = m_P[num, num];
        DenseMatrix denseMatrix7 = m_P[num2, num2];
        DenseMatrix denseMatrix8 = m_P[num, num2];
        DenseMatrix denseMatrix9 = m_P[num2, num];
        DenseMatrix denseMatrix10 = denseMatrix3 * denseMatrix7 * denseMatrix5 + denseMatrix2 * denseMatrix6 * denseMatrix4 + denseMatrix3 * denseMatrix9 * denseMatrix4 + denseMatrix2 * denseMatrix8 * denseMatrix5 + jQb - denseMatrix2 * aFj * jQb;
        DenseMatrix[] array = new DenseMatrix[m_N_lag + 1];
        for (int j = 0; j < num2; j++)
        {
            array[j] = denseMatrix2 * m_P[num, j] + denseMatrix3 * m_P[num2, j];
        }
        array[num2] = denseMatrix10;
        for (int k = num2 + 1; k < m_N_lag + 1; k++)
        {
            array[k] = denseMatrix2 * m_P[num, k - 1] + denseMatrix3 * m_P[num2, k - 1];
        }
        for (int num4 = m_N_lag - 1; num4 > num2; num4--)
        {
            m_t[num4] = m_t[num4 - 1];
            m_X[num4] = m_X[num4 - 1];
            for (int num5 = m_N_lag - 1; num5 > num2; num5--)
            {
                m_P[num4, num5] = m_P[num4 - 1, num5 - 1];
            }
        }
        m_t[num2] = t;
        m_X[num2] = denseVector;
        for (int l = 0; l < m_N_lag; l++)
        {
            m_P[num2, l] = array[l];
            m_P[l, num2] = (DenseMatrix)m_P[num2, l].Transpose();
        }
        return num2;
    }

    public void Correction(double t, double t_tol)
    {
        int num = GenFitiByInterpolation(t, t_tol);
        if (num < 0)
        {
            return;
        }
        predictZ(num);
        DenseMatrix denseMatrix = (DenseMatrix)m_H.Transpose();
        DenseMatrix denseMatrix2 = m_M * m_R * (DenseMatrix)m_M.Transpose();
        DenseMatrix denseMatrix3 = denseMatrix * (DenseMatrix)(m_H * m_P[num, num] * denseMatrix + denseMatrix2).Inverse();
        DenseVector denseVector = m_Z - m_Z_hat;
        DenseMatrix[] array = new DenseMatrix[m_N_lag];
        DenseMatrix[,] array2 = new DenseMatrix[m_N_lag, m_N_lag];
        for (int i = 0; i < m_N_lag; i++)
        {
            array[i] = m_P[i, num] * denseMatrix3;
            m_dX = array[i] * denseVector;
            m_X[i] = updateX(m_X[i], m_dX);
            DenseMatrix denseMatrix4 = array[i] * m_H;
            for (int j = 0; j < m_N_lag; j++)
            {
                array2[i, j] = m_P[i, j] - denseMatrix4 * m_P[num, j];
            }
        }
        for (int k = 0; k < m_N_lag; k++)
        {
            for (int l = k; l < m_N_lag; l++)
            {
                m_P[k, l] = 0.5 * (array2[k, l] + (DenseMatrix)array2[l, k].Transpose());
                m_P[l, k] = (DenseMatrix)m_P[k, l].Transpose();
            }
        }
        m_MeasurementUpdate[num] = true;
    }
}
