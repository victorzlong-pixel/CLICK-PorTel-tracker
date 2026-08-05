using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public abstract class HSYEKF
{
    public DenseMatrix m_I;

    public DenseMatrix m_PHI;

    public DenseMatrix m_P;

    public DenseMatrix m_Q_IK;

    public DenseMatrix m_H;

    public DenseMatrix m_M;

    public DenseMatrix m_R;

    public DenseMatrix m_K;

    public DenseVector m_X;

    public DenseVector m_dX;

    public DenseVector m_Z;

    public DenseVector m_Z_hat;

    public double m_t;

    public double m_dt;

    public bool m_MeasurementUpdate;

    protected HSYEKF(int N_x, int N_dx)
    {
        m_I = DenseMatrix.CreateIdentity(N_dx);
        m_PHI = new DenseMatrix(N_dx, N_dx);
        m_P = new DenseMatrix(N_dx, N_dx);
        m_Q_IK = new DenseMatrix(N_dx, N_dx);
        m_X = new DenseVector(N_x);
        m_dX = new DenseVector(N_dx);
    }

    public void Initialize(double t_init, DenseVector X_init, DenseMatrix P_init)
    {
        m_t = t_init;
        m_X = DenseVector.OfVector(X_init);
        m_P = DenseMatrix.OfMatrix(P_init);
        m_MeasurementUpdate = false;
    }

    protected abstract void propagateX();

    protected abstract void predictZ();

    protected abstract void updateX();

    public void Prediction(double t)
    {
        m_dt = t - m_t;
        propagateX();
        m_P = m_PHI * m_P * (DenseMatrix)m_PHI.Transpose() + m_Q_IK;
        m_t = t;
        m_MeasurementUpdate = false;
    }

    public void Correction()
    {
        predictZ();
        DenseMatrix denseMatrix = m_P * (DenseMatrix)m_H.Transpose();
        DenseMatrix denseMatrix2 = m_M * m_R * (DenseMatrix)m_M.Transpose();
        m_K = denseMatrix * (DenseMatrix)(m_H * denseMatrix + denseMatrix2).Inverse();
        DenseVector denseVector = m_Z - m_Z_hat;
        m_dX = m_K * denseVector;
        updateX();
        DenseMatrix denseMatrix3 = m_I - m_K * m_H;
        m_P = denseMatrix3 * m_P * (DenseMatrix)denseMatrix3.Transpose() + m_K * denseMatrix2 * (DenseMatrix)m_K.Transpose();
        m_MeasurementUpdate = true;
    }
}
