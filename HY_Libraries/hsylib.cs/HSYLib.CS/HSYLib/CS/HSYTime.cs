using System;

namespace HSYLib.CS;

public class HSYTime
{
    public const double TT_TAI = 32.184;

    public const double TAI_GPS = 19.0;

    public static double GPS_Leap_Sec = 17.0;

    public static double DUT1 = 0.0;

    private static DateTime m_Epoch = new DateTime(1970, 1, 1, 0, 0, 0);

    public static double JD_to_JED(double JD)
    {
        return JD + (GPS_Leap_Sec + 32.184 + 19.0) / 86400.0;
    }

    public static double UnixTime_to_JD(double UnixTime)
    {
        return UnixTime * 1.1574074074074073E-05 + 2440587.5;
    }

    public static double UnixTime_to_JD1(double UnixTime)
    {
        return (UnixTime + DUT1) * 1.1574074074074073E-05 + 2440587.5;
    }

    public static double DateTime_to_UnixTime(DateTime Date)
    {
        return (Date - m_Epoch).TotalSeconds;
    }

    public static DateTime UnixTime_to_DateTime(double UnixTime)
    {
        long ticks = (long)(UnixTime * 10000000.0);
        TimeSpan timeSpan = new TimeSpan(ticks);
        return m_Epoch + timeSpan;
    }

    public static double Dn_to_UnixTime(int yr, int mt, double dn)
    {
        if (yr < 1900 || yr > 2099)
        {
        }
        yr -= 1900;
        if (mt <= 2)
        {
            mt += 12;
            yr--;
        }
        double num = 15078.0 + dn + Math.Floor((double)yr * 1461.0 / 4.0) + Math.Floor(((double)mt * 153.0 - 457.0) / 5.0);
        return (num - 40587.0) * 86400.0;
    }

    public static double DateTime_to_JD(DateTime Date)
    {
        double unixTime = DateTime_to_UnixTime(Date);
        return UnixTime_to_JD(unixTime);
    }

    public static DateTime Dn_to_DateTime(int Year, int Mt, double Dn)
    {
        double unixTime = Dn_to_UnixTime(Year, Mt, Dn);
        return UnixTime_to_DateTime(unixTime);
    }

    public static double JD_to_mJD(double JD)
    {
        return JD - 2400000.5;
    }

    public static double mJD_to_JD(double mJD)
    {
        return mJD + 2400000.5;
    }

    public static double JD_to_UnixTime(double JD)
    {
        return (JD - 2440587.5) * 86400.0;
    }

    public static double mJD_to_UnixTime(double mJD)
    {
        return (mJD - 40587.0) * 86400.0;
    }
}
