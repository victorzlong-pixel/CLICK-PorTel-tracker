using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class EKFBuf
{
    public DenseVector X;

    public DenseMatrix P;

    public bool Updated;

    public double t;

    public int i;

    public static EKFBuf CopyData(HSYEKF _ekf, int index)
    {
        EKFBuf eKFBuf = new EKFBuf();
        eKFBuf.X = DenseVector.OfVector(_ekf.m_X);
        eKFBuf.P = DenseMatrix.OfMatrix(_ekf.m_P);
        eKFBuf.Updated = _ekf.m_MeasurementUpdate;
        eKFBuf.t = _ekf.m_t;
        eKFBuf.i = index;
        return eKFBuf;
    }
}
