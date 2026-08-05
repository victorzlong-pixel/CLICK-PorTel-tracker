using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYFLSDKF
{
    public DenseVector[] m_X;

    public DenseMatrix[,] m_P;

    public bool[] m_Correction;

    public int m_N_lag;

    public HSYFLSDKF(int N_x, int N_lag)
    {
        m_N_lag = N_lag + 1;
        m_X = new DenseVector[m_N_lag];
        m_P = new DenseMatrix[m_N_lag, m_N_lag];
        m_Correction = new bool[m_N_lag];
        for (int i = 0; i < m_N_lag; i++)
        {
            m_X[i] = new DenseVector(N_x);
            for (int j = 0; j < m_N_lag; j++)
            {
                m_P[i, j] = new DenseMatrix(N_x);
            }
            m_Correction[i] = false;
        }
    }

    public void Initialize(DenseVector X_init, DenseMatrix P_init)
    {
        m_X[0] = DenseVector.OfVector(X_init);
        m_P[0, 0] = DenseMatrix.OfMatrix(P_init);
    }

    public void Prediction(DenseMatrix F, DenseMatrix Q)
    {
        for (int num = m_N_lag - 1; num > 0; num--)
        {
            m_X[num] = m_X[num - 1];
            m_Correction[num] = m_Correction[num - 1];
        }
        m_X[0] = F * m_X[0];
        m_Correction[0] = false;
        for (int num2 = m_N_lag - 1; num2 > 0; num2--)
        {
            for (int num3 = m_N_lag - 1; num3 > 0; num3--)
            {
                m_P[num2, num3] = m_P[num2 - 1, num3 - 1];
            }
        }
        DenseMatrix denseMatrix = (DenseMatrix)F.Transpose();
        m_P[0, 0] = F * m_P[1, 1] * denseMatrix + Q;
        for (int i = 1; i < m_N_lag; i++)
        {
            m_P[0, i] = F * m_P[1, i];
            m_P[i, 0] = (DenseMatrix)m_P[0, i].Transpose();
        }
    }

    public void Correction(int Lagi, DenseVector y, DenseMatrix H, DenseMatrix R)
    {
        DenseMatrix denseMatrix = (DenseMatrix)H.Transpose();
        DenseMatrix denseMatrix2 = denseMatrix * (DenseMatrix)(H * m_P[Lagi, Lagi] * denseMatrix + R).Inverse();
        DenseVector denseVector = y - H * m_X[Lagi];
        DenseMatrix[] array = new DenseMatrix[m_N_lag];
        DenseMatrix[,] array2 = new DenseMatrix[m_N_lag, m_N_lag];
        for (int i = 0; i < m_N_lag; i++)
        {
            array[i] = m_P[i, Lagi] * denseMatrix2;
            m_X[i] += array[i] * denseVector;
            DenseMatrix denseMatrix3 = array[i] * H;
            for (int j = 0; j < m_N_lag; j++)
            {
                array2[i, j] = m_P[i, j] - denseMatrix3 * m_P[Lagi, j];
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
        m_Correction[Lagi] = true;
    }
}
