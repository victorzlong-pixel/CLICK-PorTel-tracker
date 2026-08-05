using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;

namespace HSYLib.CS;

public class HSYSGP4
{
    public struct Tle
    {
        public string sat_name;

        public int yr;

        public double dn;

        public double i;

        public double raan;

        public double e;

        public double w;

        public double ma;

        public double mm;

        public double decy;

        public double decy6;

        public double bstar;

        public int revn;

        public int obj_no;

        public override string ToString()
        {
            string text = "";
            text = text + "yr: " + yr + "\n";
            text = text + "dn: " + dn.ToString("F15") + "\n";
            text = text + "i: " + i.ToString("F15") + "\n";
            text = text + "raan: " + raan.ToString("F15") + "\n";
            text = text + "e: " + e.ToString("F15") + "\n";
            text = text + "w: " + w.ToString("F15") + "\n";
            text = text + "ma: " + ma.ToString("F15") + "\n";
            text = text + "mm: " + mm.ToString("F15") + "\n";
            text = text + "decy: " + decy.ToString("F15") + "\n";
            text = text + "decy6: " + decy6.ToString("F15") + "\n";
            text = text + "bstar: " + bstar.ToString("F15") + "\n";
            text = text + "revn: " + revn + "\n";
            return text + "obj_no: " + obj_no + "\n";
        }
    }

    public class HSYSGP4EasyUsing
    {
        private HSYSGP4 m_Sgp4 = new HSYSGP4();

        private double m_StartTime;

        private double m_TLEEpoch;

        public double StartTime => m_StartTime;

        public double TLEEpoch => m_TLEEpoch;

        public HSYSGP4EasyUsing(Tle tle)
        {
            m_Sgp4.Initialize(tle);
            m_StartTime = HSYTime.DateTime_to_UnixTime(m_Sgp4.m_epoch);
            m_TLEEpoch = HSYTime.DateTime_to_UnixTime(m_Sgp4.m_epoch);
        }

        public HSYSGP4EasyUsing(DateTime Start_Time, Tle tle)
        {
            m_Sgp4.Initialize(tle);
            m_StartTime = HSYTime.DateTime_to_UnixTime(Start_Time);
            m_TLEEpoch = HSYTime.DateTime_to_UnixTime(m_Sgp4.m_epoch);
        }

        public HSYSGP4EasyUsing(DateTime Start_Time, HSYSGP4 Sgp4)
        {
            m_Sgp4 = Sgp4;
            m_StartTime = HSYTime.DateTime_to_UnixTime(Start_Time);
            m_TLEEpoch = HSYTime.DateTime_to_UnixTime(Sgp4.m_epoch);
        }

        public HSYSGP4EasyUsing(double Start_Time_UnixTime, string fn)
        {
            m_Sgp4.Initialize(fn);
            m_StartTime = Start_Time_UnixTime;
            m_TLEEpoch = HSYTime.DateTime_to_UnixTime(m_Sgp4.m_epoch);
        }

        public void PropagateOrbit(double ElapsedTime_sec, out DenseVector Pos_j2k, out DenseVector Vel_j2k, out DenseVector Pos_ecr, out DenseVector Vel_ecr)
        {
            PropagateOrbit(ElapsedTime_sec, out Pos_j2k, out Vel_j2k, out Pos_ecr, out Vel_ecr, out var _, out var _);
        }

        public void PropagateOrbit(double ElapsedTime_sec, out DenseVector Pos_j2k, out DenseVector Vel_j2k, out DenseVector Pos_ecr, out DenseVector Vel_ecr, out DenseMatrix mJ2k2Ecr, out DenseMatrix mJ2k2EcrDot)
        {
            double num = m_StartTime + ElapsedTime_sec;
            double t_since_min = (num - m_TLEEpoch) / 60.0;
            m_Sgp4.Propagate(t_since_min, out var OUTPUT_Pos_TEMED, out var OUTPUT_Vel_TEMED);
            double jD = HSYTime.UnixTime_to_JD(num);
            double jED = HSYTime.JD_to_JED(jD);
            double jD2 = HSYTime.UnixTime_to_JD1(num);
            HSYIERSc.TEMED2J2k2ECR(jD2, jED, out var mTemed2J2k, out mJ2k2Ecr, out mJ2k2EcrDot);
            Pos_j2k = mTemed2J2k * OUTPUT_Pos_TEMED;
            Vel_j2k = mTemed2J2k * OUTPUT_Vel_TEMED;
            Pos_ecr = mJ2k2Ecr * Pos_j2k;
            Vel_ecr = mJ2k2Ecr * Vel_j2k + mJ2k2EcrDot * Pos_j2k;
        }

        public void PropagateOrbit(DateTime CurrentTime, out DenseVector Pos_j2k, out DenseVector Vel_j2k, out DenseVector Pos_ecr, out DenseVector Vel_ecr, out DenseMatrix mJ2k2Ecr, out DenseMatrix mJ2k2EcrDot)
        {
            double elapsedTime_sec = HSYTime.DateTime_to_UnixTime(CurrentTime) - m_StartTime;
            PropagateOrbit(elapsedTime_sec, out Pos_j2k, out Vel_j2k, out Pos_ecr, out Vel_ecr, out mJ2k2Ecr, out mJ2k2EcrDot);
        }

        public void PropagateOrbit(double ElapsedTime_sec, out DenseVector Pos_j2k, out DenseVector Vel_j2k, out DenseVector Pos_ecr, out DenseVector Vel_ecr, out DenseVector Pos_temed, out DenseVector Vel_temed, out DenseMatrix mJ2k2Ecr, out DenseMatrix mJ2k2EcrDot)
        {
            double num = m_StartTime + ElapsedTime_sec;
            double t_since_min = (num - m_TLEEpoch) / 60.0;
            m_Sgp4.Propagate(t_since_min, out Pos_temed, out Vel_temed);
            double jD = HSYTime.UnixTime_to_JD(num);
            double jED = HSYTime.JD_to_JED(jD);
            double jD2 = HSYTime.UnixTime_to_JD1(num);
            HSYIERSc.TEMED2J2k2ECR(jD2, jED, out var mTemed2J2k, out mJ2k2Ecr, out mJ2k2EcrDot);
            Pos_j2k = mTemed2J2k * Pos_temed;
            Vel_j2k = mTemed2J2k * Vel_temed;
            Pos_ecr = mJ2k2Ecr * Pos_j2k;
            Vel_ecr = mJ2k2Ecr * Vel_j2k + mJ2k2EcrDot * Pos_j2k;
        }

        public void PropagateOrbit(DateTime CurrentTime, out DenseVector Pos_j2k, out DenseVector Vel_j2k, out DenseVector Pos_ecr, out DenseVector Vel_ecr, out DenseVector Pos_temed, out DenseVector Vel_temed, out DenseMatrix mJ2k2Ecr, out DenseMatrix mJ2k2EcrDot)
        {
            double elapsedTime_sec = HSYTime.DateTime_to_UnixTime(CurrentTime) - m_StartTime;
            PropagateOrbit(elapsedTime_sec, out Pos_j2k, out Vel_j2k, out Pos_ecr, out Vel_ecr, out Pos_temed, out Vel_temed, out mJ2k2Ecr, out mJ2k2EcrDot);
        }
    }

    public bool m_bInit;

    private const double dtr = Math.PI / 180.0;

    private const double pi2 = Math.PI * 2.0;

    private const double tothrd = 2.0 / 3.0;

    private const double ae = 1.0;

    private const double qo = 120.0;

    private const double so = 78.0;

    private const double xj2 = 0.0010826158;

    private const double xj3 = -2.53881E-06;

    private const double xj4 = -1.65597E-06;

    private const double xke = 0.0743669161;

    private const double xkmper = 6378.135;

    private const double Mnpda = 1440.0;

    private bool isimp;

    private double a1;

    private double cosio;

    private double theta2;

    private double x3thm1;

    private double eosq;

    private double betao2;

    private double betao;

    private double del1;

    private double ao;

    private double delo;

    private double xnodp;

    private double aodp;

    private double s4;

    private double qoms24;

    private double perige;

    private double pinvsq;

    private double tsi;

    private double eta;

    private double etasq;

    private double eeta;

    private double psisq;

    private double coef;

    private double coef1;

    private double c2;

    private double c1;

    private double sinio;

    private double a3ovk2;

    private double c3;

    private double x1mth2;

    private double c4;

    private double c5;

    private double theta4;

    private double temp1;

    private double temp2;

    private double temp3;

    private double Mdot;

    private double x1m5th;

    private double omgdot;

    private double xhdot1;

    private double xnodot;

    private double omgcof;

    private double Mcof;

    private double xnodcf;

    private double t2cof;

    private double Lcof;

    private double aycof;

    private double delMo;

    private double sinMo;

    private double x7thm1;

    private double c1sq;

    private double d2;

    private double temp;

    private double d3;

    private double d4;

    private double t3cof;

    private double t4cof;

    private double t5cof;

    private double ck2;

    private double ck4;

    private double qoms2t;

    private double s;

    private double ae2;

    private double ae4;

    private double Mdf;

    private double omgadf;

    private double xnoddf;

    private double omega;

    private double Mp;

    private double tsq;

    private double xnode;

    private double tempa;

    private double tempe;

    private double templ;

    private double delomg;

    private double delm;

    private double tcube;

    private double tfour;

    private double a;

    private double e;

    private double L;

    private double beta;

    private double xn;

    private double axn;

    private double Ll;

    private double aynl;

    private double Lt;

    private double ayn;

    private double capu;

    private double sinepw;

    private double cosepw;

    private double temp4;

    private double temp5;

    private double temp6;

    private double epw;

    private double ecosE;

    private double esinE;

    private double elsq;

    private double pl;

    private double r;

    private double rdot;

    private double rfdot;

    private double betal;

    private double cosu;

    private double sinu;

    private double u;

    private double sin2u;

    private double cos2u;

    private double rk;

    private double uk;

    private double xnodek;

    private double xinck;

    private double rdotk;

    private double rfdotk;

    private double sinuk;

    private double cosuk;

    private double sinik;

    private double cosik;

    private double sinnok;

    private double cosnok;

    private double Mx;

    private double My;

    private double ux;

    private double uy;

    private double uz;

    private double vx;

    private double vy;

    private double vz;

    private double n0;

    private double M0;

    private double w0;

    private double raan0;

    private double incl0;

    private double e0;

    private double bstar;

    public DateTime m_epoch;

    public double m_EpochUnixTime;

    public HSYSGP4()
    {
        m_bInit = false;
    }

    public int Initialize(Tle tle)
    {
        Initialize(tle.yr, tle.dn, tle.i, tle.raan, tle.e, tle.w, tle.ma, tle.mm, tle.bstar);
        return 1;
    }

    public int Initialize(string _fn)
    {
        Tle tle = ReadTle(_fn);
        Initialize(tle);
        return 1;
    }

    public int Initialize(int tle_yr, double tle_dn, double tle_i_deg, double tle_raan_deg, double tle_e, double tle_w_deg, double tle_ma_deg, double tle_mm_rev_per_day, double tle_bstar)
    {
        DateTime epoch = new DateTime(tle_yr, 1, 1).AddDays(tle_dn - 1.0);
        m_epoch = epoch;
        m_EpochUnixTime = HSYTime.Dn_to_UnixTime(tle_yr, 1, tle_dn);
        ae2 = 1.0;
        ae4 = ae2 * ae2;
        ck2 = 0.0005413079 * ae2;
        ck4 = 6.209887499999999E-07 * ae4;
        double num = Math.Abs(0.006584997024992416);
        qoms2t = num * num * num * num;
        s = 1.0122292801892716;
        raan0 = tle_raan_deg * (Math.PI / 180.0);
        w0 = tle_w_deg * (Math.PI / 180.0);
        M0 = tle_ma_deg * (Math.PI / 180.0);
        incl0 = tle_i_deg * (Math.PI / 180.0);
        e0 = tle_e;
        temp = Math.PI / 720.0;
        n0 = tle_mm_rev_per_day * temp;
        bstar = tle_bstar;
        a1 = Math.Pow(0.0743669161 / n0, 2.0 / 3.0);
        cosio = Math.Cos(incl0);
        theta2 = cosio * cosio;
        x3thm1 = 3.0 * theta2 - 1.0;
        eosq = e0 * e0;
        betao2 = 1.0 - eosq;
        betao = Math.Sqrt(betao2);
        del1 = 1.5 * ck2 * x3thm1 / (a1 * a1 * betao * betao2);
        ao = a1 * (1.0 - del1 * (1.0 / 3.0 + del1 * (1.0 + 1.654320987654321 * del1)));
        delo = 1.5 * ck2 * x3thm1 / (ao * ao * betao * betao2);
        xnodp = n0 / (1.0 + delo);
        aodp = ao / (1.0 - delo);
        isimp = false;
        if (aodp * (1.0 - e0) / 1.0 < 1.034492841559484)
        {
            isimp = true;
        }
        s4 = s;
        qoms24 = qoms2t;
        perige = (aodp * (1.0 - e0) - 1.0) * 6378.135;
        if (perige <= 156.0)
        {
            s4 = perige - 78.0;
            if (perige <= 98.0)
            {
                s4 = 20.0;
            }
            num = (120.0 - s4) * 1.0 / 6378.135;
            qoms24 = num * num * num * num;
            s4 = s4 / 6378.135 + 1.0;
        }
        pinvsq = 1.0 / (aodp * aodp * betao2 * betao2);
        tsi = 1.0 / (aodp - s4);
        eta = aodp * e0 * tsi;
        etasq = eta * eta;
        eeta = e0 * eta;
        psisq = Math.Abs(1.0 - etasq);
        coef = qoms24 * tsi * tsi * tsi * tsi;
        coef1 = coef / Math.Pow(psisq, 3.5);
        c2 = coef1 * xnodp * (aodp * (1.0 + 1.5 * etasq + eeta * (4.0 + etasq)) + 0.75 * ck2 * tsi / psisq * x3thm1 * (8.0 + 3.0 * etasq * (8.0 + etasq)));
        c1 = bstar * c2;
        sinio = Math.Sin(incl0);
        a3ovk2 = (0.0 - -2.53881E-06 / ck2) * 1.0 * 1.0 * 1.0;
        c3 = coef * tsi * a3ovk2 * xnodp * 1.0 * sinio / e0;
        x1mth2 = 1.0 - theta2;
        c4 = 2.0 * xnodp * coef1 * aodp * betao2 * (eta * (2.0 + 0.5 * etasq) + e0 * (0.5 + 2.0 * etasq) - 2.0 * ck2 * tsi / (aodp * psisq) * (-3.0 * x3thm1 * (1.0 - 2.0 * eeta + etasq * (1.5 - 0.5 * eeta)) + 0.75 * x1mth2 * (2.0 * etasq - eeta * (1.0 + etasq)) * Math.Cos(2.0 * w0)));
        c5 = 2.0 * coef1 * aodp * betao2 * (1.0 + 2.75 * (etasq + eeta) + eeta * etasq);
        theta4 = theta2 * theta2;
        temp1 = 3.0 * ck2 * pinvsq * xnodp;
        temp2 = temp1 * ck2 * pinvsq;
        temp3 = 1.25 * ck4 * pinvsq * pinvsq * xnodp;
        Mdot = xnodp + 0.5 * temp1 * betao * x3thm1 + 0.0625 * temp2 * betao * (13.0 - 78.0 * theta2 + 137.0 * theta4);
        x1m5th = 1.0 - 5.0 * theta2;
        omgdot = -0.5 * temp1 * x1m5th + 0.0625 * temp2 * (7.0 - 114.0 * theta2 + 395.0 * theta4) + temp3 * (3.0 - 36.0 * theta2 + 49.0 * theta4);
        xhdot1 = (0.0 - temp1) * cosio;
        xnodot = xhdot1 + (0.5 * temp2 * (4.0 - 19.0 * theta2) + 2.0 * temp3 * (3.0 - 7.0 * theta2)) * cosio;
        omgcof = bstar * c3 * Math.Cos(w0);
        Mcof = -2.0 / 3.0 * coef * bstar * 1.0 / eeta;
        xnodcf = 3.5 * betao2 * xhdot1 * c1;
        t2cof = 1.5 * c1;
        Lcof = 0.125 * a3ovk2 * sinio * (3.0 + 5.0 * cosio) / (1.0 + cosio);
        aycof = 0.25 * a3ovk2 * sinio;
        num = 1.0 + eta * Math.Cos(M0);
        delMo = num * num * num;
        sinMo = Math.Sin(M0);
        x7thm1 = 7.0 * theta2 - 1.0;
        if (!isimp)
        {
            c1sq = c1 * c1;
            d2 = 4.0 * aodp * tsi * c1sq;
            temp = d2 * tsi * c1 / 3.0;
            d3 = (17.0 * aodp + s4) * temp;
            d4 = 0.5 * temp * aodp * tsi * (221.0 * aodp + 31.0 * s4) * c1;
            t3cof = d2 + 2.0 * c1sq;
            t4cof = 0.25 * (3.0 * d3 + c1 * (12.0 * d2 + 10.0 * c1sq));
            t5cof = 0.2 * (3.0 * d4 + 12.0 * c1 * d3 + 6.0 * d2 * d2 + 15.0 * c1sq * (2.0 * d2 + c1sq));
        }
        m_bInit = true;
        return 1;
    }

    public int Propagate(double t_since_min, out DenseVector OUTPUT_Pos_TEMED, out DenseVector OUTPUT_Vel_TEMED)
    {
        OUTPUT_Pos_TEMED = new DenseVector(3);
        OUTPUT_Vel_TEMED = new DenseVector(3);
        if (!m_bInit)
        {
            return -1;
        }
        Mdf = M0 + Mdot * t_since_min;
        omgadf = w0 + omgdot * t_since_min;
        xnoddf = raan0 + xnodot * t_since_min;
        omega = omgadf;
        Mp = Mdf;
        tsq = t_since_min * t_since_min;
        xnode = xnoddf + xnodcf * tsq;
        tempa = 1.0 - c1 * t_since_min;
        tempe = bstar * c4 * t_since_min;
        templ = t2cof * tsq;
        double num;
        if (!isimp)
        {
            delomg = omgcof * t_since_min;
            num = 1.0 + eta * Math.Cos(Mdf);
            delm = Mcof * (num * num * num - delMo);
            temp = delomg + delm;
            Mp = Mdf + temp;
            omega = omgadf - temp;
            tcube = tsq * t_since_min;
            tfour = t_since_min * tcube;
            tempa = tempa - d2 * tsq - d3 * tcube - d4 * tfour;
            tempe += bstar * c5 * (Math.Sin(Mp) - sinMo);
            templ = templ + t3cof * tcube + tfour * (t4cof + t_since_min * t5cof);
        }
        a = aodp * tempa * tempa;
        e = e0 - tempe;
        L = Mp + omega + xnode + xnodp * templ;
        beta = Math.Sqrt(1.0 - e * e);
        xn = 0.0743669161 / Math.Pow(a, 1.5);
        axn = e * Math.Cos(omega);
        temp = 1.0 / (a * beta * beta);
        Ll = temp * Lcof * axn;
        aynl = temp * aycof;
        Lt = L + Ll;
        ayn = e * Math.Sin(omega) + aynl;
        num = Lt - xnode;
        num %= Math.PI * 2.0;
        if (num < 0.0)
        {
            num += Math.PI * 2.0;
        }
        capu = num;
        temp2 = capu;
        for (int i = 0; i < 10; i++)
        {
            sinepw = Math.Sin(temp2);
            cosepw = Math.Cos(temp2);
            temp3 = axn * sinepw;
            temp4 = ayn * cosepw;
            temp5 = axn * cosepw;
            temp6 = ayn * sinepw;
            epw = (capu - temp4 + temp3 - temp2) / (1.0 - temp5 - temp6) + temp2;
            if (Math.Abs(epw - temp2) <= 1E-06)
            {
                break;
            }
            temp2 = epw;
        }
        ecosE = temp5 + temp6;
        esinE = temp3 - temp4;
        elsq = axn * axn + ayn * ayn;
        temp = 1.0 - elsq;
        pl = a * temp;
        r = a * (1.0 - ecosE);
        temp1 = 1.0 / r;
        rdot = 0.0743669161 * Math.Sqrt(a) * esinE * temp1;
        rfdot = 0.0743669161 * Math.Sqrt(pl) * temp1;
        temp2 = a * temp1;
        betal = Math.Sqrt(temp);
        temp3 = 1.0 / (1.0 + betal);
        cosu = temp2 * (cosepw - axn + ayn * esinE * temp3);
        sinu = temp2 * (sinepw - ayn - axn * esinE * temp3);
        u = Math.Atan2(sinu, cosu);
        sin2u = 2.0 * sinu * cosu;
        cos2u = 2.0 * cosu * cosu - 1.0;
        temp = 1.0 / pl;
        temp1 = ck2 * temp;
        temp2 = temp1 * temp;
        rk = r * (1.0 - 1.5 * temp2 * betal * x3thm1) + 0.5 * temp1 * x1mth2 * cos2u;
        uk = u - 0.25 * temp2 * x7thm1 * sin2u;
        xnodek = xnode + 1.5 * temp2 * cosio * sin2u;
        xinck = incl0 + 1.5 * temp2 * cosio * sinio * cos2u;
        rdotk = rdot - xn * temp1 * x1mth2 * sin2u;
        rfdotk = rfdot + xn * temp1 * (x1mth2 * cos2u + 1.5 * x3thm1);
        sinuk = Math.Sin(uk);
        cosuk = Math.Cos(uk);
        sinik = Math.Sin(xinck);
        cosik = Math.Cos(xinck);
        sinnok = Math.Sin(xnodek);
        cosnok = Math.Cos(xnodek);
        Mx = (0.0 - sinnok) * cosik;
        My = cosnok * cosik;
        ux = Mx * sinuk + cosnok * cosuk;
        uy = My * sinuk + sinnok * cosuk;
        uz = sinik * sinuk;
        vx = Mx * cosuk - cosnok * sinuk;
        vy = My * cosuk - sinnok * sinuk;
        vz = sinik * cosuk;
        rk *= 6378.135;
        OUTPUT_Pos_TEMED[0] = rk * ux;
        OUTPUT_Pos_TEMED[1] = rk * uy;
        OUTPUT_Pos_TEMED[2] = rk * uz;
        OUTPUT_Vel_TEMED[0] = (rdotk * ux + rfdotk * vx) * 6378.135 / 60.0;
        OUTPUT_Vel_TEMED[1] = (rdotk * uy + rfdotk * vy) * 6378.135 / 60.0;
        OUTPUT_Vel_TEMED[2] = (rdotk * uz + rfdotk * vz) * 6378.135 / 60.0;
        return 1;
    }

    public int Propagate(DateTime _Time, out DenseVector OUTPUT_Pos_TEMED, out DenseVector OUTPUT_Vel_TEMED)
    {
        return Propagate((_Time - m_epoch).TotalMinutes, out OUTPUT_Pos_TEMED, out OUTPUT_Vel_TEMED);
    }

    public static int ReadTle(string fn, out Tle tle)
    {
        FileStream stream = new FileStream(fn, FileMode.Open);
        StreamReader streamReader = new StreamReader(stream);
        tle.sat_name = streamReader.ReadLine();
        string text = streamReader.ReadLine();
        string text2;
        if (tle.sat_name[0] == '1' && text[0] == '2')
        {
            text2 = text;
            text = tle.sat_name;
            tle.sat_name = "Unknown";
        }
        else
        {
            text2 = streamReader.ReadLine();
        }
        tle = default(Tle);
        tle.yr = int.Parse(text.Substring(18, 2));
        if (tle.yr > 50)
        {
            tle.yr += 1900;
        }
        else
        {
            tle.yr += 2000;
        }
        tle.dn = double.Parse(text.Substring(20, 10));
        tle.decy = double.Parse(text.Substring(33, 10));
        int num = int.Parse(text.Substring(50, 2));
        tle.decy6 = double.Parse(text.Substring(44, 5)) * Math.Pow(10.0, -5 + num);
        num = int.Parse(text.Substring(59, 2));
        tle.bstar = double.Parse(text.Substring(53, 6)) * Math.Pow(10.0, -5 + num);
        tle.i = double.Parse(text2.Substring(8, 8));
        tle.raan = double.Parse(text2.Substring(17, 8));
        tle.e = double.Parse(text2.Substring(26, 7)) * 1E-07;
        tle.w = double.Parse(text2.Substring(34, 8));
        tle.ma = double.Parse(text2.Substring(43, 8));
        tle.mm = double.Parse(text2.Substring(52, 11));
        streamReader.Close();
        return 1;
    }

    public static Tle ReadTle(string fn)
    {
        Tle tle = default(Tle);
        ReadTle(fn, out tle);
        return tle;
    }
}
