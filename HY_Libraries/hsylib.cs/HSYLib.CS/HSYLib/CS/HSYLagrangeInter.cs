namespace HSYLib.CS;

public class HSYLagrangeInter
{
    private double[] m_Poly;

    private bool m_Init;

    private bool m_ReCal;

    private double m_X0;

    private double[] m_X;

    private double[] m_Y;

    private int m_OrderN;

    private int m_OrderN2;

    private int m_N;

    private int m_Starti;

    public HSYLagrangeInter(int _OrderN)
    {
        m_Poly = new double[_OrderN];
        m_OrderN = _OrderN;
        m_OrderN2 = m_OrderN / 2;
        m_Init = false;
        m_ReCal = true;
    }

    public bool Initialize(double[] x_time, double[] y_value)
    {
        if (x_time.Length != y_value.Length)
        {
            return false;
        }
        m_N = x_time.Length;
        for (int i = 1; i < m_N; i++)
        {
            if (x_time[i - 1] >= x_time[i])
            {
                return false;
            }
        }
        m_X = new double[m_N];
        m_Y = new double[m_N];
        m_X0 = x_time[0];
        for (int j = 0; j < m_N; j++)
        {
            m_X[j] = x_time[j] - m_X0;
            m_Y[j] = y_value[j];
        }
        m_ReCal = true;
        m_Init = true;
        return true;
    }

    private void CalcPolynomial(int starti)
    {
        for (int i = 0; i < m_OrderN; i++)
        {
            m_Poly[i] = 1.0;
            for (int j = 0; j < m_OrderN; j++)
            {
                if (i != j)
                {
                    m_Poly[i] *= m_X[i] - m_X[j];
                }
            }
            m_Poly[i] = 1.0 / m_Poly[i];
        }
        m_Starti = starti;
    }

    public double CalcValue(double x_time)
    {
        if (CalcValue(x_time, out var Output))
        {
            return Output;
        }
        return 0.0;
    }

    public bool CalcValue(double x_time, out double Output)
    {
        double num = x_time - m_X0;
        Output = 0.0;
        if (!m_Init)
        {
            return false;
        }
        if (num < m_X[0])
        {
            return false;
        }
        if (num > m_X[m_N - 1])
        {
            return false;
        }
        int[] array = new int[2];
        HSYMath.binary_search(num, m_X, m_N, array);
        int num2 = array[1] - m_OrderN2;
        if (num2 < 0)
        {
            num2 = 0;
        }
        if (num2 + m_OrderN > m_N)
        {
            num2 = m_N - m_OrderN;
        }
        if (m_ReCal || num2 != m_Starti)
        {
            CalcPolynomial(num2);
        }
        for (int i = 0; i < m_OrderN; i++)
        {
            double num3 = 1.0;
            for (int j = 0; j < m_OrderN; j++)
            {
                if (i != j)
                {
                    num3 *= num - m_X[j + num2];
                }
            }
            Output += num3 * m_Poly[i] * m_Y[i + num2];
        }
        return true;
    }
}
