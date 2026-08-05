using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public abstract class HSYRK4
{
    protected abstract DenseVector derivs(double t, DenseVector x);

    public DenseVector Integrate(double t, DenseVector x, double dt)
    {
        int count = x.Count;
        double num = dt * 0.5;
        double num2 = dt / 6.0;
        double t2 = t + num;
        DenseVector denseVector = derivs(t, x);
        DenseVector x2 = x + num * denseVector;
        DenseVector denseVector2 = derivs(t2, x2);
        x2 = x + num * denseVector2;
        DenseVector denseVector3 = derivs(t2, x2);
        x2 = x + dt * denseVector3;
        denseVector3 += denseVector2;
        denseVector2 = derivs(t + dt, x2);
        return x + num2 * (denseVector + denseVector2 + 2.0 * denseVector3);
    }
}
