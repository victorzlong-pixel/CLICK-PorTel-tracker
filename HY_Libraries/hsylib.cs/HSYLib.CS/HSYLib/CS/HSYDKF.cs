using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYDKF
{
    public DenseVector m_X;

    public DenseMatrix m_P;

    private DenseMatrix m_I;

    public bool m_MeasurementUpdate;

    public HSYDKF(int N_x)
    {
        m_X = new DenseVector(N_x);
        m_P = new DenseMatrix(N_x);
        m_I = DenseMatrix.CreateIdentity(N_x);
    }

    public void Initialize(DenseVector X_init, DenseMatrix P_init)
    {
        m_X = DenseVector.OfVector(X_init);
        m_P = DenseMatrix.OfMatrix(P_init);
        m_MeasurementUpdate = false;
    }

    public void Prediction(DenseMatrix F, DenseMatrix Q)
    {
        m_X = F * m_X;
        m_P = F * m_P * (DenseMatrix)F.Transpose() + Q;
        m_MeasurementUpdate = false;
    }

    public void Correction(DenseVector y, DenseMatrix H, DenseMatrix R)
    {
        DenseVector denseVector = y - H * m_X;
        DenseMatrix denseMatrix = m_P * (DenseMatrix)H.Transpose();
        DenseMatrix denseMatrix2 = denseMatrix * (DenseMatrix)(H * denseMatrix + R).Inverse();
        m_X += denseMatrix2 * denseVector;
        DenseMatrix denseMatrix3 = m_I - denseMatrix2 * H;
        m_P = denseMatrix3 * m_P * (DenseMatrix)denseMatrix3.Transpose() + denseMatrix2 * R * (DenseMatrix)denseMatrix2.Transpose();
        m_MeasurementUpdate = true;
    }
}
