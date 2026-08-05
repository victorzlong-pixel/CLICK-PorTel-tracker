using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSYLib.CS;
using MathNet.Numerics.LinearAlgebra.Double;
using System.IO;

namespace GS_Tracking_KR
{
    public class TelCalMeasurement
    {
        Quaternion q;
        DateTime t;
        double UnixTime;
        double azi; // rad
        double alt; // rad
        double rms = 0; // as
        double MaxScore = 0;
        int nStarsID = 0;

        public TelCalMeasurement(Quaternion _q, DateTime _t, double _azi, double _alt, double _rms, double _MaxScore, int _nStarsID)
        {
            q = _q;
            t = _t;
            UnixTime = HSYTime.DateTime_to_UnixTime(t);
            azi = _azi;
            alt = _alt;
            rms = _rms;
            MaxScore = _MaxScore;
            nStarsID = _nStarsID;
        }

        public DateTime GetTime()
        {
            return t;
        }

        public int GetNstarsID()
        {
            return nStarsID;
        }

        public double GetMaxScore()
        {
            return MaxScore;
        }

        public double GetRMSE()
        {
            return rms;
        }

        public Quaternion GetQuat()
        {
            return q;
        }

        public double[] GetAziAlt()
        {
            double[] AziAlt = new double[2];
            AziAlt[0] = azi;
            AziAlt[1] = alt;
            return AziAlt;
        }

        public void WriteToFile(BinaryWriter bw)
        {
            for (int i = 0; i < 4; i++)
                bw.Write(q[i]);
            bw.Write(UnixTime);
            bw.Write(azi);
            bw.Write(alt);
            bw.Write(rms);
            bw.Write(MaxScore);
            bw.Write(nStarsID);
            return;
        }

        public TelCalMeasurement(BinaryReader br)
        {
            q = new Quaternion();
            for (int i = 0; i < 4; i++)
                q[i] = br.ReadDouble();
            UnixTime = br.ReadDouble();
            azi = br.ReadDouble();
            alt = br.ReadDouble();
            rms = br.ReadDouble();
            MaxScore = br.ReadDouble();
            nStarsID = br.ReadInt32();
            t = HSYTime.UnixTime_to_DateTime(UnixTime);
        }
    }

    public class TelPointingModel
    {
        private static double lat; // latitude, deg
        private static double lon;// longitude, deg
        private double[] ST_noise = new double[] { 20, 20, 200 }; // 400
        private List<TelCalMeasurement> CalMeasurements;
        private Quaternion q_ST_TOBS; // note: in current implementation this is not used, rather the vector of TOBS in star tracker frame is passed in
        private Quaternion q_TNOM_GIM_hat;
        private Quaternion q_ST_GIM_hat;
        private Quaternion q_MNT_ENU_hat;
        private static Quaternion q_ENU_ECEF;
        private Quaternion[] q_ST_J2K_meas;
        private Quaternion[] q_ST_J2K_meas_atmref;
        private Quaternion[] q_ENU_J2K_cal;
        private double[][] AziAlt;
        private double theta_NP;
        private double a_d;
        private double c;
        private double c2;
        private double c3;
        private double CalRMS;
        private bool calibrated;
        StreamWriter sw_CalResLog = null;

        public TelPointingModel(double _lat, double _lon)
        {
            lat = _lat;
            lon = _lon;
            CalMeasurements = new List<TelCalMeasurement>();
            calibrated = false;
        }

        public List<TelCalMeasurement> GetCalMeasList()
        {
            return CalMeasurements;
        }

        public void addMeas(StarImg StarImg1, DateTime t, double azi, double alt)
        {
            Quaternion q = StarImg1.GetQest();
            double rms = StarImg1.GetRMSE();
            double maxScore = StarImg1.GetMaxScore();
            int nStarsID = StarImg1.GetNstarsID();
            TelCalMeasurement meas = new TelCalMeasurement(q, t, azi, alt, rms, maxScore, nStarsID);
            CalMeasurements.Add(meas);
        }

        public void removeMeas(int index)
        {
            CalMeasurements.RemoveAt(index);
        }

        public void PerformCal()
        {
            if (CalMeasurements.Count < 2)
                return;
            AtmRef_Correction();
            CoarseCal();
            //FineCal_9term();
            //FineCal_new();
            FineCal_8term();
            //FineCal_6term();
            //FineCal_8term_tan();
            calibrated = true;
        }

        public double GetCalRMS()
        {
            return CalRMS;
        }

        public bool IsCalibrated()
        { 
            return calibrated;
        }

        public Quaternion Getq_ST_GIM()
        {
            return q_ST_GIM_hat;
        }

        public Quaternion Getq_MNT_ENU()
        {
            return q_MNT_ENU_hat;
        }

        public Quaternion Getq_ENU_ECEF()
        {
            return q_ENU_ECEF;
        }

        /* Returns AziAlt in radians. */
        public double[] CalcAziAltRef(DateTime t, DenseVector XYZ_J2K, DenseVector XYZ_ST)
        {
            double[] AziAltRef = new double[2];
            Quaternion q_ENU_J2K = Cal_q_ENU_J2K(t);
            DenseVector XYZ_MNT = HSYMath.quaternion_to_DCM(q_MNT_ENU_hat * q_ENU_J2K) * XYZ_J2K;
            DenseVector XYZ_GIM = HSYMath.quaternion_to_DCM(q_ST_GIM_hat.InverseQ()) * XYZ_ST;

            double XYZ_GIM1 = XYZ_GIM[0];
            double XYZ_GIM2 = XYZ_GIM[1];
            double XYZ_GIM3 = XYZ_GIM[2];
            double XYZ_MNT1 = XYZ_MNT[0];
            double XYZ_MNT2 = XYZ_MNT[1];
            double XYZ_MNT3 = XYZ_MNT[2];

            double[] cosAzi1, sinAzi1, cosAlt1, sinAlt1;
            double cosAzi, sinAzi, cosAlt, sinAlt;
            bool flag;

            flag = SolveAziAltEq(XYZ_GIM1, XYZ_MNT1, XYZ_MNT3, out cosAzi1, out sinAzi1);
            if (flag == false)
                return null;

            flag = SolveAziAltEq(XYZ_MNT2, XYZ_GIM2, XYZ_GIM3, out cosAlt1, out sinAlt1);
            if (flag == false)
                return null;

            DenseVector Dz = new DenseVector(3);
            DenseMatrix T = new DenseMatrix(3, 3);
            double minDz = 100;
            int ii = 0, jj = 0;
            List<int[]> idx = new List<int[]>();

            // There are 4 solutions to each set, enumerate 16 possibilities and see which works
            for (int i = 0; i < 4; i++)
            {
                cosAlt = cosAlt1[i];
                sinAlt = sinAlt1[i];
                for (int j = 0; j < 4; j++)
                {
                    cosAzi = cosAzi1[j];
                    sinAzi = sinAzi1[j];
                    T[0, 0] = cosAzi;
                    T[0, 1] = 0;
                    T[0, 2] = -sinAzi;
                    T[1, 0] = sinAlt * sinAzi;
                    T[1, 1] = cosAlt;
                    T[1, 2] = sinAlt * cosAzi;
                    T[2, 0] = cosAlt * sinAzi;
                    T[2, 1] = -sinAlt;
                    T[2, 2] = cosAlt * cosAzi;
                    Dz = T * XYZ_MNT - XYZ_GIM;
                    double dzNorm = Dz.L2Norm();
                    if (dzNorm < 1e-10)
                        idx.Add(new int[] { i, j });
                    if (dzNorm < minDz)
                    {
                        ii = i;
                        jj = j;
                        minDz = dzNorm;
                    }
                }
            }

            double[] AziAlt = new double[2];
            for (int i = 0; i < idx.Count; i++)
            {
                cosAzi = cosAzi1[idx[i][1]];
                sinAzi = sinAzi1[idx[i][1]];
                cosAlt = cosAlt1[idx[i][0]];
                sinAlt = sinAlt1[idx[i][0]];
                double AltTemp = Math.Atan2(sinAlt, cosAlt);
                if (AltTemp < 0 || AltTemp > (Math.PI / 2.0))
                    continue;
                AziAlt[0] = Math.Atan2(sinAzi, cosAzi);
                AziAlt[1] = AltTemp;
            }
            return AziAlt;
        }

        /* Returns AziAltDot in radians/s */
        public double[] CalcAziAltDotRef(double AziRad, DenseVector v_ECEF, DenseVector vdot_ECEF)
        {
            double sinAzi = Math.Sin(AziRad);
            double cosAzi = Math.Cos(AziRad);

            DenseVector LOS_MNT = HSYMath.quaternion_to_DCM(q_MNT_ENU_hat * q_ENU_ECEF) * v_ECEF;
            DenseVector LOSdot_MNT = HSYMath.quaternion_to_DCM(q_MNT_ENU_hat * q_ENU_ECEF) * vdot_ECEF;
            double den = sinAzi * LOS_MNT[0] + cosAzi * LOS_MNT[2];
            double p = 1.0 / den;
            double[] AziAltDotRef = new double[2];
            AziAltDotRef[1] = -LOSdot_MNT[1] * p;
            AziAltDotRef[0] = (LOSdot_MNT[0] * cosAzi - LOSdot_MNT[2] * sinAzi) * p;
            return AziAltDotRef;
        }

        /* Returns AziAlt in radians. */
        public double[] CalcAziAltRef9term(DateTime t, DenseVector v_J2K, DenseVector v_ST)
        {
            double[] AziAltRef = new double[2];
            Quaternion q_ENU_J2K = Cal_q_ENU_J2K(t);
            DenseVector v_MNT = HSYMath.quaternion_to_DCM(q_MNT_ENU_hat * q_ENU_J2K) * v_J2K;
            DenseMatrix R = new DenseMatrix(3);
            R[0, 0] = Math.Cos(theta_NP);
            R[0, 1] = Math.Sin(theta_NP);
            R[1, 0] = -Math.Sin(theta_NP);
            R[1, 1] = Math.Cos(theta_NP);
            R[2, 2] = 1;
            DenseVector v_ENU = HSYMath.quaternion_to_DCM(q_ENU_J2K) * v_J2K;
            double c_d = Math.Sqrt(1 - v_ENU[2] * v_ENU[2]);
            DenseVector up = new DenseVector(new double[] { 0, 0, 1 });
            Quaternion q_ST_ENU = HSYMath.vector_3x1_3x1_to_quaternion(v_ENU, v_ST);
            DenseVector v_d = HSYMath.vector_3x1_cross(HSYMath.quaternion_to_DCM(q_ST_ENU) * up, v_ST);
            v_d = v_d / v_d.L2Norm();
            DenseMatrix v_d_matrix = new DenseMatrix(3, 1);
            v_d_matrix[0, 0] = v_d[0];
            v_d_matrix[1, 0] = v_d[1];
            v_d_matrix[2, 0] = v_d[2];
            Quaternion q_TOBS_TNOM_hat = Quaternion.OfRotationAxisAngle(v_d, a_d * c_d);
            DenseVector v_GIM = R * HSYMath.quaternion_to_DCM(q_TNOM_GIM_hat.InverseQ() * q_TOBS_TNOM_hat.InverseQ()) * v_ST;

            double z1 = v_GIM[0] - Math.Sin(theta_NP) * v_MNT[1];
            double vc = Math.Cos(theta_NP) * v_MNT[0];
            double vs = Math.Cos(theta_NP) * v_MNT[2];
            double[] cosAzi1, sinAzi1;
            double[] cosAlt1 = new double[4];
            double[] sinAlt1 = new double[4];
            double cosAzi, sinAzi, cosAlt, sinAlt;
            bool flag;

            flag = SolveAziAltEq(z1, vc, vs, out cosAzi1, out sinAzi1);
            if (flag == false)
                return null;

            for (int i = 0; i < 4; i++)
            {
                double cos1, sin1;
                z1 = -Math.Sin(theta_NP) * cosAzi1[i] * v_MNT[0] + Math.Cos(theta_NP) * v_MNT[1] + Math.Sin(theta_NP) * sinAzi1[i] * v_MNT[2];
                vc = v_GIM[1];
                vs = v_GIM[2];
                flag = SolveAltEq(z1, vc, vs, out cos1, out sin1);
                cosAlt1[i] = cos1;
                sinAlt1[i] = sin1;
                if (flag == false)
                    return null;
            }

            DenseVector Dz = new DenseVector(3);
            DenseMatrix T = new DenseMatrix(3, 3);
            List<int> idx = new List<int>();

            // There are 4 solutions to Azi set, 4 solutions to Alt set, enumerate possibilities
            for (int i = 0; i < 4; i++)
            {
                cosAlt = cosAlt1[i];
                sinAlt = sinAlt1[i];
                cosAzi = cosAzi1[i];
                sinAzi = sinAzi1[i];
                double cosTnp = Math.Cos(theta_NP);
                double sinTnp = Math.Sin(theta_NP);
                T[0, 0] = cosTnp * cosAzi;
                T[0, 1] = sinTnp;
                T[0, 2] = -cosTnp * sinAzi;
                T[1, 0] = -sinTnp * cosAlt * cosAzi + sinAlt * sinAzi;
                T[1, 1] = cosTnp * cosAlt;
                T[1, 2] = sinTnp * cosAlt * sinAzi + sinAlt * cosAzi;
                T[2, 0] = sinTnp * sinAlt * cosAzi + cosAlt * sinAzi;
                T[2, 1] = -cosTnp * sinAlt;
                T[2, 2] = -sinTnp * sinAlt * sinAzi + cosAlt * cosAzi;
                Dz = T * v_MNT - v_GIM;
                double dzNorm = Dz.L2Norm();
                if (dzNorm < 1e-10)
                    idx.Add(i);
            }

            double[] AziAlt = new double[2];
            for (int i = 0; i < idx.Count; i++)
            {
                cosAzi = cosAzi1[idx[i]];
                sinAzi = sinAzi1[idx[i]];
                cosAlt = cosAlt1[idx[i]];
                sinAlt = sinAlt1[idx[i]];
                double AltTemp = Math.Atan2(sinAlt, cosAlt);
                if (AltTemp < 0 || AltTemp > (Math.PI / 2.0))
                    continue;
                AziAlt[0] = Math.Atan2(sinAzi, cosAzi);
                AziAlt[1] = AltTemp;
            }
            return AziAlt;
        }

        /* Returns AziAltDot in radians/s. */
        public double[] CalcAziAltDotRef9term(double AziRad, DenseVector v_ECEF, DenseVector vdot_ECEF)
        {
            double sinAzi = Math.Sin(AziRad);
            double cosAzi = Math.Cos(AziRad);
            double sinTnp = Math.Sin(theta_NP);
            double cosTnp = Math.Cos(theta_NP);

            DenseVector LOS_MNT = HSYMath.quaternion_to_DCM(q_MNT_ENU_hat * q_ENU_ECEF) * v_ECEF;
            DenseVector LOSdot_MNT = HSYMath.quaternion_to_DCM(q_MNT_ENU_hat * q_ENU_ECEF) * vdot_ECEF;
            double den = cosTnp * sinAzi * LOS_MNT[0] + cosTnp * cosAzi * LOS_MNT[2];
            double p = 1.0 / den;
            double[] AziAltDotRef = new double[2];
            AziAltDotRef[0] = (cosTnp * cosAzi * LOSdot_MNT[0] + sinTnp * LOSdot_MNT[1] - cosTnp * sinAzi * LOSdot_MNT[2]) * p;
            den = sinAzi * LOS_MNT[0] + cosAzi * LOS_MNT[2];
            p = 1.0 / den;
            AziAltDotRef[1] = (-sinTnp * (sinAzi * AziAltDotRef[0] * LOS_MNT[0] + cosAzi * AziAltDotRef[0] * LOS_MNT[2] - cosAzi * LOSdot_MNT[0] + sinAzi * LOSdot_MNT[2]) - cosTnp * LOSdot_MNT[1]) * p;
            return AziAltDotRef;
        }

        private bool SolveAltEq(double z, double vc, double vs, out double cos1, out double sin1)
        {
            // Solves the equation z = vc * cos(x) - vs * sin(x) for cos(x) and sin(x)
            // Restricts solution to 0 < Alt < pi/2
            double vc2 = vc * vc;
            double vs2 = vs * vs;
            double z2 = z * z;
            cos1 = 0;
            sin1 = 0;
            double[] cos = new double[4];
            double[] sin = new double[4];

            double A = vc * z;
            double b = vc2 * z2 - (vc2 + vs2) * (z2 - vs2);
            if (b < 0)
            {
                if (b > -1e-7)
                    b = 0;
                else
                    return false;
            }
            double B = Math.Sqrt(b);
            double C = vc2 + vs2;
            cos[0] = (A + B) / C;
            cos[1] = cos[0];
            cos[2] = (A - B) / C;
            cos[3] = cos[2];

            for (int i = 0; i < 2; i++)
            {
                if ((cos[i * 2] > 1.0) || (cos[i * 2] < -1.0))
                    continue;
                sin[i * 2] = Math.Sqrt(1 - cos[i * 2] * cos[i * 2]);
                sin[i * 2 + 1] = -sin[i * 2];
            }
            for (int i = 0; i < 4; i++)
            {
                cos1 = cos[i];
                sin1 = sin[i];
                if (cos1 > 0 && cos1 < 1 && sin1 > 0 && sin1 < 1)
                    return true;
            }
            return false;
        }

        private bool SolveAziAltEq(double z, double vc, double vs, out double[] cos, out double[] sin)
        {
            // Solves the equation z = vc * cos(x) - vs * sin(x) for cos(x) and sin(x)
            double vc2 = vc * vc;
            double vs2 = vs * vs;
            double z2 = z * z;

            cos = new double[4];
            sin = new double[4];

            double A = vc * z;
            double b = vc2 * z2 - (vc2 + vs2) * (z2 - vs2);
            if (b < 0)
            {
                if (b > -1e-7)
                    b = 0;
                else
                    return false;
            }
            double B = Math.Sqrt(b);
            double C = vc2 + vs2;
            cos[0] = (A + B) / C;
            cos[1] = cos[0];
            cos[2] = (A - B) / C;
            cos[3] = cos[2];

            for (int i = 0; i < 2; i++)
            {
                if ((cos[i * 2] > 1.0) || (cos[i * 2] < -1.0))
                    continue;
                sin[i * 2] = Math.Sqrt(1 - cos[i * 2] * cos[i * 2]);
                sin[i * 2 + 1] = -sin[i * 2];
            }
            return true;
        }

        private void AtmRef_Correction()
        {
            DenseVector z_ST = new DenseVector(new double[] { 0, 0, 1 });
            Quaternion[] q_ST_ENU_meas = new Quaternion[CalMeasurements.Count];
            q_ENU_J2K_cal = new Quaternion[CalMeasurements.Count];
            q_ST_J2K_meas = new Quaternion[CalMeasurements.Count];
            q_ST_J2K_meas_atmref = new Quaternion[CalMeasurements.Count];

            for (int i = 0; i < CalMeasurements.Count; i++)
            {
                TelCalMeasurement meas = CalMeasurements[i];
                q_ST_J2K_meas[i] = meas.GetQuat();
                q_ENU_J2K_cal[i] = Cal_q_ENU_J2K(meas.GetTime());
                q_ST_ENU_meas[i] = meas.GetQuat() / q_ENU_J2K_cal[i];

                DenseMatrix DCM_ENU_ST = HSYMath.quaternion_to_DCM(q_ST_ENU_meas[i].InverseQ());
                DenseVector z_ENU = DCM_ENU_ST * z_ST;
                double h_a = HSYMath.RTD * Math.Asin(z_ENU[2]); // apparent altitude in degrees
                double theta_ref = HSYMath.DTR / 60 * 1 / Math.Tan((h_a + 7.31 / (h_a + 4.4)) * HSYMath.DTR); // refraction angle in rad
                DenseVector rotaxis_ENU = HSYMath.vector_3x1_cross(z_ST, z_ENU); // positive angle = down
                rotaxis_ENU = rotaxis_ENU / rotaxis_ENU.L2Norm();
                DenseVector rotaxis_J2K = HSYMath.quaternion_to_DCM(q_ENU_J2K_cal[i].InverseQ()) * rotaxis_ENU;
                Quaternion dq_atm = Quaternion.OfRotationAxisAngle(rotaxis_J2K, theta_ref);

                q_ST_J2K_meas_atmref[i] = q_ST_J2K_meas[i] * dq_atm;
            }
        }

        private void CoarseCal()
        {
            theta_NP = 0; // coarse calibration assumption
            a_d = 0;
            c = 0;
            AziAlt = new double[CalMeasurements.Count][];
            Quaternion[] q_GIM_MNT = new Quaternion[CalMeasurements.Count];
            Quaternion[] q_ST_ENU_meas = new Quaternion[CalMeasurements.Count];

            for (int i = 0; i < CalMeasurements.Count; i++)
            {
                // Calculate q_ST_ENU
                TelCalMeasurement meas = CalMeasurements[i];
                q_ENU_J2K_cal[i] = Cal_q_ENU_J2K(meas.GetTime());
                q_ST_ENU_meas[i] = meas.GetQuat() / q_ENU_J2K_cal[i];

                // Calculate q_GIM_MNT
                AziAlt[i] = new double[2];
                AziAlt[i] = meas.GetAziAlt();
                q_GIM_MNT[i] = AziAltToQuat(AziAlt[i][0], AziAlt[i][1], theta_NP);
            }

            List<Quaternion> q_ST_ENU_inter = new List<Quaternion>();
            List<Quaternion> q_ST_ENU_inter_inv = new List<Quaternion>();
            List<Quaternion> q_GIM_MNT_inter = new List<Quaternion>();
            List<Quaternion> q_GIM_MNT_inter_inv = new List<Quaternion>();

            for (int i = 0; i < CalMeasurements.Count; i++)
            {
                for (int j = i + 1; j < CalMeasurements.Count; j++)
                {
                    Quaternion q1 = q_GIM_MNT[i] / q_GIM_MNT[j];
                    q1.MakePlusQ4();

                    Quaternion q2 = q_GIM_MNT[i].InverseQ() * q_GIM_MNT[j];
                    q2.MakePlusQ4();

                    Quaternion q3 = q_ST_ENU_meas[i] / q_ST_ENU_meas[j];
                    q3.MakePlusQ4();

                    Quaternion q4 = q_ST_ENU_meas[i].InverseQ() * q_ST_ENU_meas[j];
                    q4.MakePlusQ4();

                    // Eliminate measurements with bad observability
                    if ((1 - q1[3]) < 1e-2 || q1[3] < 1e-2)
                        continue;
                    if ((1 - q2[3]) < 1e-2 || q2[3] < 1e-2)
                        continue;
                    if ((1 - q3[3]) < 1e-2 || q3[3] < 1e-2)
                        continue;
                    if ((1 - q4[3]) < 1e-2 || q4[3] < 1e-2)
                        continue;
                    q_GIM_MNT_inter.Add(q1);
                    q_GIM_MNT_inter_inv.Add(q2);
                    q_ST_ENU_inter.Add(q3);
                    q_ST_ENU_inter_inv.Add(q4);
                }
            }

            // Use QUEST to calculate q_ST_GIM and q_MNT_ENU
            DenseVector[] v_ST_ENU_inter = new DenseVector[q_ST_ENU_inter.Count];
            DenseVector[] v_ST_ENU_inter_inv = new DenseVector[q_ST_ENU_inter_inv.Count];
            DenseVector[] v_GIM_MNT_inter = new DenseVector[q_GIM_MNT_inter.Count];
            DenseVector[] v_GIM_MNT_inter_inv = new DenseVector[q_GIM_MNT_inter_inv.Count];
            double[] w = new double[q_ST_ENU_inter.Count];

            for (int i = 0; i < q_ST_ENU_inter.Count; i++)
            {
                v_ST_ENU_inter[i] = (DenseVector)((DenseVector)q_ST_ENU_inter[i].SubVector(0, 3)).Normalize(2);
                v_ST_ENU_inter_inv[i] = (DenseVector)((DenseVector)q_ST_ENU_inter_inv[i].SubVector(0, 3)).Normalize(2);
                v_GIM_MNT_inter[i] = (DenseVector)((DenseVector)q_GIM_MNT_inter[i].SubVector(0, 3)).Normalize(2);
                v_GIM_MNT_inter_inv[i] = (DenseVector)((DenseVector)q_GIM_MNT_inter_inv[i].SubVector(0, 3)).Normalize(2);
                w[i] = 1; // weighting for QUEST
            }

            q_ST_GIM_hat = HSYMath.QUEST(v_ST_ENU_inter, v_GIM_MNT_inter, w);
            Quaternion q_MNT_ENU_inv = HSYMath.QUEST(v_ST_ENU_inter_inv, v_GIM_MNT_inter_inv, w);
            q_MNT_ENU_hat = q_MNT_ENU_inv.InverseQ();

            if (q_ST_TOBS == null)
                q_ST_TOBS = new Quaternion();
            q_TNOM_GIM_hat = q_ST_TOBS.InverseQ() * q_ST_GIM_hat;

            Quaternion q_ECEF_ENU = HSYMath.DCM_to_quaternion(HSYEarth.DCM_ECR2ENU(lat, lon));
            Quaternion q_MNT_ECEF = q_MNT_ENU_hat / q_ECEF_ENU;
        }

        private void FineCal_6term()
        {
            Quaternion dq_ST_GIM, dq_MNT_ENU;
            DenseVector res = new DenseVector(3 * CalMeasurements.Count);
            for (int i = 0; i < 100; i++)
            {
                NLS_iter_6term(out dq_ST_GIM, out dq_MNT_ENU, out res);
                q_ST_GIM_hat = dq_ST_GIM.Normalize() * q_ST_GIM_hat;
                q_MNT_ENU_hat = dq_MNT_ENU.Normalize() * q_MNT_ENU_hat;

                double dxMag = dq_ST_GIM.SubVector(0, 3).L2Norm() + dq_MNT_ENU.SubVector(0, 3).L2Norm();
                if (dxMag < 0.001 * HSYMath.ASTR)
                {
                    break;
                }
            }

            // Check against ECR
            Quaternion q_MNT_ECI = q_MNT_ENU_hat * q_ENU_ECEF;

            CalRMS = 2 * res.L2Norm() / Math.Sqrt(res.Count) * HSYMath.RTAS;

            string fn_CalResLog = "CalRes6term_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
            if (sw_CalResLog != null)
                sw_CalResLog.Close();
            sw_CalResLog = new StreamWriter(fn_CalResLog);
            for (int i=0; i < CalMeasurements.Count; i++)
            {
                double[] AziAlt = CalMeasurements[i].GetAziAlt();
                string str = AziAlt[0].ToString() + ", " + AziAlt[1].ToString() + ", ";
                for (int j = 0; j < 3; j++)
                    str += res[i * 3 + j].ToString() + ", ";
                sw_CalResLog.WriteLine(str);
            }
            sw_CalResLog.Close();
        }

        private void NLS_iter_6term(out Quaternion dq_ST_GIM, out Quaternion dq_MNT_ENU, out DenseVector res)
        {
            DenseMatrix Rinv = new DenseMatrix(3, 3); // noise covariance
            for (int i = 0; i < 3; i++)
            {
                double w = ST_noise[i] * HSYMath.ASTR;
                w = w * w;
                Rinv[i, i] = 1 / w;
            }
            int n = CalMeasurements.Count;
            DenseVector y = new DenseVector(3 * n);
            DenseMatrix H = new DenseMatrix(3 * n, 6);
            DenseMatrix W = new DenseMatrix(3 * n, 3 * n); // weighting matrix, inverse of covariance matrix

            for (int i = 0; i < n; i++)
            {
                Quaternion q_ST_MNT = q_ST_GIM_hat * AziAltToQuat(AziAlt[i][0], AziAlt[i][1], theta_NP);
                Quaternion q_ST_J2K_hat = q_ST_MNT * q_MNT_ENU_hat * q_ENU_J2K_cal[i];
                Quaternion dq_ST_J2K = q_ST_J2K_meas_atmref[i] / q_ST_J2K_hat;
                dq_ST_J2K.MakePlusQ4();
                DenseMatrix DCM_ST_MNT = HSYMath.quaternion_to_DCM(q_ST_MNT);

                for (int j = 0; j < 3; j++)
                {
                    y[3 * i + j] = dq_ST_J2K[j];
                    H[3 * i + j, j] = 1;
                    for (int k = 0; k < 3; k++)
                    {
                        H[3 * i + j, k + 3] = DCM_ST_MNT[j, k];
                        W[3 * i + j, 3 * i + k] = Rinv[j, k];
                    }
                }
            }

            DenseMatrix HT = (DenseMatrix)H.Transpose();
            DenseMatrix HTWHI = (DenseMatrix)(HT * W * H).Inverse();
            DenseVector x = HTWHI * HT * W * y;

            dq_ST_GIM = new Quaternion();
            dq_MNT_ENU = new Quaternion();
            for (int i = 0; i < 3; i++)
            {
                dq_ST_GIM[i] = x[i];
                dq_MNT_ENU[i] = x[i + 3];
            }

            res = y;
        }

        private void FineCal_8term()
        {
            Quaternion dq_TNOM_GIM, dq_MNT_ENU;
            double da_d, dtheta_NP;
            DenseVector res = new DenseVector(3 * CalMeasurements.Count);
            for (int i = 0; i < 100; i++)
            {
                NLS_iter_8term(out dq_TNOM_GIM, out dq_MNT_ENU, out dtheta_NP, out da_d, out res);
                dq_TNOM_GIM = dq_TNOM_GIM.Normalize();
                dq_MNT_ENU = dq_MNT_ENU.Normalize();
                q_TNOM_GIM_hat = dq_TNOM_GIM * q_TNOM_GIM_hat;
                q_MNT_ENU_hat = dq_MNT_ENU * q_MNT_ENU_hat;
                theta_NP = theta_NP + dtheta_NP;
                a_d = a_d + da_d;

                double dxMag = dq_TNOM_GIM.SubVector(0, 3).L2Norm() + dq_MNT_ENU.SubVector(0, 3).L2Norm() + Math.Abs(da_d) + Math.Abs(dtheta_NP);
                if (dxMag < 0.001 * HSYMath.ASTR)
                {
                    break;
                }
            }

            CalRMS = 2 * res.L2Norm() / Math.Sqrt(res.Count) * HSYMath.RTAS;
            DenseVector resxy = new DenseVector(2 * CalMeasurements.Count);
            for (int i = 0; i < CalMeasurements.Count; i++)
            {
                resxy[i * 2] = res[i * 3];
                resxy[i * 2 + 1] = res[i * 3 + 1];
            }
            double CalRMS2 = 2 * resxy.L2Norm() / Math.Sqrt(resxy.Count) * HSYMath.RTAS;

            string fn_CalResLog = "CalRes8term_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
            if (sw_CalResLog != null)
                sw_CalResLog.Close();
            sw_CalResLog = new StreamWriter(fn_CalResLog);
            for (int i = 0; i < CalMeasurements.Count; i++)
            {
                double[] AziAlt = CalMeasurements[i].GetAziAlt();
                string str = AziAlt[0].ToString() + ", " + AziAlt[1].ToString() + ", ";
                for (int j = 0; j < 3; j++)
                    str += res[i * 3 + j].ToString() + ", ";
                sw_CalResLog.WriteLine(str);
            }
            sw_CalResLog.Close();
        }

        private void NLS_iter_8term(out Quaternion dq_TNOM_GIM, out Quaternion dq_MNT_ENU, out double dtheta_NP, out double da_d, out DenseVector res)
        {
            DenseMatrix Rinv = new DenseMatrix(3, 3); // noise covariance
            for (int i = 0; i < 3; i++)
            {
                double w = ST_noise[i] * HSYMath.ASTR;
                w = w * w;
                Rinv[i, i] = 1 / w;
            }
            int n = CalMeasurements.Count;
            DenseVector y = new DenseVector(3 * n);
            DenseMatrix H = new DenseMatrix(3 * n, 8);
            DenseMatrix W = new DenseMatrix(3 * n, 3 * n); // weighting matrix, inverse of covariance matrix

            DenseVector z = new DenseVector(new double[] { 0, 0, 1 });
            Quaternion q_alt_hat = new Quaternion();
            Quaternion q_GIM_MNT_hat = new Quaternion();
            Quaternion q_TNOM_ENU_hat = new Quaternion();
            Quaternion q_TOBS_TNOM_hat = new Quaternion();
            Quaternion q_TOBS_ENU_hat = new Quaternion();
            double c_d;
            DenseMatrix v_d_matrix = new DenseMatrix(3,1);
            DenseMatrix v_c = new DenseMatrix(3,1);
            DenseMatrix A, Ainv;
            DenseMatrix M = new DenseMatrix(3, 3);
            for (int i = 0; i < n; i++)
            {
                q_alt_hat[0] = Math.Cos(theta_NP) * Math.Sin(0.5 * AziAlt[i][1]);
                q_alt_hat[1] = Math.Sin(theta_NP) * Math.Sin(0.5 * AziAlt[i][1]);
                q_alt_hat[3] = Math.Cos(0.5 * AziAlt[i][1]);
                q_GIM_MNT_hat = AziAltToQuat(AziAlt[i][0], AziAlt[i][1], theta_NP);
                q_TNOM_ENU_hat = q_TNOM_GIM_hat * q_GIM_MNT_hat * q_MNT_ENU_hat;
                double temp = z.DotProduct(HSYMath.quaternion_to_DCM(q_TNOM_ENU_hat.InverseQ()) * z);
                c_d = Math.Min(Math.Sqrt(1 - temp * temp),0.71);
                DenseVector v_d = HSYMath.vector_3x1_cross(HSYMath.quaternion_to_DCM(q_TNOM_ENU_hat) * z, z);
                v_d = v_d / v_d.L2Norm();
                v_d_matrix[0, 0] = v_d[0];
                v_d_matrix[1, 0] = v_d[1];
                v_d_matrix[2, 0] = v_d[2];
                q_TOBS_TNOM_hat = Quaternion.OfRotationAxisAngle(v_d, a_d * c_d);
                q_TOBS_ENU_hat = q_TOBS_TNOM_hat * q_TNOM_ENU_hat;
                A = HSYMath.quaternion_to_DCM(q_TNOM_ENU_hat);
                Ainv = HSYMath.quaternion_to_DCM(q_TNOM_ENU_hat.InverseQ());
                M[0, 0] = 2 * A[2, 2] / Math.Sqrt(A[1, 2] * A[1, 2] + A[0, 2] * A[0, 2]) - 2 * A[2, 2] * A[1, 2] * A[1, 2] / Math.Pow(A[1, 2] * A[1, 2] + A[0, 2] * A[0, 2], 1.5);
                M[1, 0] = 2 * A[0, 2] * A[1, 2] * A[2, 2] / Math.Pow(A[0, 2] * A[0, 2] + A[1, 2] * A[1, 2], 1.5);
                M[0, 1] = M[1, 0];
                M[1, 1] = 2 * A[2, 2] / Math.Sqrt(A[1, 2] * A[1, 2] + A[0, 2] * A[0, 2]) + 2 * A[2, 2] * A[0, 2] * A[0, 2] / Math.Pow(A[1, 2] * A[1, 2] + A[0, 2] * A[0, 2], 1.5);
                M[0, 2] = -2 * A[0, 2] / Math.Sqrt(A[0, 2] * A[0, 2] + A[1, 2] * A[1, 2]);
                M[1, 2] = -2 * A[0, 2] / Math.Sqrt(A[1, 2] * A[0, 2] + A[1, 2] * A[1, 2]);
                v_c[0, 0] = 2 * Ainv[2, 1] * Ainv[2, 2] / Math.Sqrt(1 - Ainv[2, 2] * Ainv[2, 2]);
                v_c[1, 0] = -2 * Ainv[2, 0] * Ainv[2, 2] / Math.Sqrt(1 - Ainv[2, 2] * Ainv[2, 2]);
              
                Quaternion dq_TOBS_J2K = q_ST_TOBS.InverseQ() * q_ST_J2K_meas_atmref[i] / (q_TOBS_ENU_hat * q_ENU_J2K_cal[i]);
                dq_TOBS_J2K.MakePlusQ4();

                DenseMatrix identity = new DenseMatrix(3);
                identity[0, 0] = 1;
                identity[1, 1] = 1;
                identity[2, 2] = 1;
                DenseMatrix H1 = 0.5 * c_d * HSYMath.quaternion_to_DCM(q_ST_TOBS) * v_d_matrix;
                DenseMatrix temp1 = (DenseMatrix) (0.5 * a_d * v_d_matrix * v_c.Transpose() + (0.5 * Math.Sin(a_d * c_d) - Math.Sin(0.5 * a_d * c_d) * Math.Sin(0.5 * a_d * c_d) * HSYMath.vector_3x1_to_skew_sym_matrix_3x3_cross(v_d)) * M);
                DenseMatrix H2 = (DenseMatrix) (HSYMath.quaternion_to_DCM(q_ST_TOBS * q_TOBS_TNOM_hat) + HSYMath.quaternion_to_DCM(q_ST_TOBS) * temp1);
                //DenseVector H3 = (DenseVector) (-0.5 * (identity + HSYMath.quaternion_to_DCM(q_ST_TOBS) * temp1) * HSYMath.quaternion_to_DCM(q_ST_TOBS * q_TOBS_TNOM_hat * q_TNOM_GIM_hat) * (identity - HSYMath.quaternion_to_DCM(q_alt_hat)) * z); 
                //DenseMatrix H4 = (DenseMatrix) (identity + HSYMath.quaternion_to_DCM(q_ST_TOBS) * temp1) * HSYMath.quaternion_to_DCM(q_ST_TOBS * q_TOBS_TNOM_hat * q_TNOM_GIM_hat * q_GIM_MNT_hat);
                DenseVector H3 = -0.5 * (HSYMath.quaternion_to_DCM(q_ST_TOBS*q_TOBS_TNOM_hat*q_TNOM_GIM_hat) + HSYMath.quaternion_to_DCM(q_ST_TOBS) * temp1 * HSYMath.quaternion_to_DCM(q_TNOM_GIM_hat)) * (identity - HSYMath.quaternion_to_DCM(q_alt_hat)) * z;
                DenseMatrix H4 = HSYMath.quaternion_to_DCM(q_ST_TOBS * q_TOBS_TNOM_hat * q_TNOM_GIM_hat * q_GIM_MNT_hat) + HSYMath.quaternion_to_DCM(q_ST_TOBS) * temp1 * HSYMath.quaternion_to_DCM(q_TNOM_GIM_hat * q_GIM_MNT_hat);

                for (int j = 0; j < 3; j++)
                {
                    y[3 * i + j] = dq_TOBS_J2K[j];
                    H[3 * i + j, 0] = H1[j, 0];
                    H[3 * i + j, 4] = H3[j];
                    for (int k = 0; k < 3; k++)
                    {
                        H[3 * i + j, 1 + k] = H2[j, k];
                        H[3 * i + j, 5 + k] = H4[j, k]; 
                        W[3 * i + j, 3 * i + k] = Rinv[j, k];
                    }
                }
            }

            DenseMatrix HT = (DenseMatrix) H.Transpose();
            DenseMatrix HTWHI = (DenseMatrix) (HT * W * H).Inverse();
            DenseVector x = HTWHI * HT * W * y;

            dq_TNOM_GIM = new Quaternion();
            dq_MNT_ENU = new Quaternion();
            da_d = x[0];
            dtheta_NP = x[4];
            for (int i = 0; i < 3; i++)
            {
                dq_TNOM_GIM[i] = x[1 + i];
                dq_MNT_ENU[i] = x[5 + i];
            }
            res = y;
        }

        private void FineCal_8term_tan()
        {
            Quaternion dq_TNOM_GIM, dq_MNT_ENU;
            double da_d, dtheta_NP;
            DenseVector res = new DenseVector(3 * CalMeasurements.Count);
            for (int i = 0; i < 100; i++)
            {
                NLS_iter_8term_tan(out dq_TNOM_GIM, out dq_MNT_ENU, out dtheta_NP, out da_d, out res);
                dq_TNOM_GIM = dq_TNOM_GIM.Normalize();
                dq_MNT_ENU = dq_MNT_ENU.Normalize();
                q_TNOM_GIM_hat = dq_TNOM_GIM * q_TNOM_GIM_hat;
                q_MNT_ENU_hat = dq_MNT_ENU * q_MNT_ENU_hat;
                theta_NP = theta_NP + dtheta_NP;
                a_d = a_d + da_d;

                double dxMag = dq_TNOM_GIM.SubVector(0, 3).L2Norm() + dq_MNT_ENU.SubVector(0, 3).L2Norm() + Math.Abs(da_d) + Math.Abs(dtheta_NP);
                if (dxMag < 0.001 * HSYMath.ASTR)
                {
                    break;
                }
            }

            CalRMS = 2 * res.L2Norm() / Math.Sqrt(res.Count) * HSYMath.RTAS;

            string fn_CalResLog = "CalRes8term_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
            if (sw_CalResLog != null)
                sw_CalResLog.Close();
            sw_CalResLog = new StreamWriter(fn_CalResLog);
            for (int i = 0; i < CalMeasurements.Count; i++)
            {
                double[] AziAlt = CalMeasurements[i].GetAziAlt();
                string str = AziAlt[0].ToString() + ", " + AziAlt[1].ToString() + ", ";
                for (int j = 0; j < 3; j++)
                    str += res[i * 3 + j].ToString() + ", ";
                sw_CalResLog.WriteLine(str);
            }
            sw_CalResLog.Close();
        }

        private void NLS_iter_8term_tan(out Quaternion dq_TNOM_GIM, out Quaternion dq_MNT_ENU, out double dtheta_NP, out double da_d, out DenseVector res)
        {
            DenseMatrix Rinv = new DenseMatrix(3, 3); // noise covariance
            for (int i = 0; i < 3; i++)
            {
                double w = ST_noise[i] * HSYMath.ASTR;
                w = w * w;
                Rinv[i, i] = 1 / w;
            }
            int n = CalMeasurements.Count;
            DenseVector y = new DenseVector(3 * n);
            DenseMatrix H = new DenseMatrix(3 * n, 8);
            DenseMatrix W = new DenseMatrix(3 * n, 3 * n); // weighting matrix, inverse of covariance matrix

            DenseVector z = new DenseVector(new double[] { 0, 0, 1 });
            Quaternion q_alt_hat = new Quaternion();
            Quaternion q_GIM_MNT_hat = new Quaternion();
            Quaternion q_TNOM_ENU_hat = new Quaternion();
            Quaternion q_TOBS_TNOM_hat = new Quaternion();
            Quaternion q_TOBS_ENU_hat = new Quaternion();
            DenseMatrix v_d_matrix = new DenseMatrix(3, 1);
            DenseMatrix A, Ainv;
            DenseMatrix M = new DenseMatrix(3, 3);
            for (int i = 0; i < n; i++)
            {
                q_alt_hat[0] = Math.Cos(theta_NP) * Math.Sin(0.5 * AziAlt[i][1]);
                q_alt_hat[1] = Math.Sin(theta_NP) * Math.Sin(0.5 * AziAlt[i][1]);
                q_alt_hat[3] = Math.Cos(0.5 * AziAlt[i][1]);
                q_GIM_MNT_hat = AziAltToQuat(AziAlt[i][0], AziAlt[i][1], theta_NP);
                q_TNOM_ENU_hat = q_TNOM_GIM_hat * q_GIM_MNT_hat * q_MNT_ENU_hat;
                DenseVector temp = HSYMath.quaternion_to_DCM(q_ENU_J2K_cal[i] * q_ST_J2K_meas_atmref[i].InverseQ()) * z;
                double tanEl = temp[2] / Math.Sqrt(temp[0] * temp[0] + temp[1] * temp[1]);
                DenseVector v_d = HSYMath.vector_3x1_cross(HSYMath.quaternion_to_DCM(q_TNOM_ENU_hat) * z, z);
                v_d = v_d / v_d.L2Norm();
                v_d_matrix[0, 0] = v_d[0];
                v_d_matrix[1, 0] = v_d[1];
                v_d_matrix[2, 0] = v_d[2];
                q_TOBS_TNOM_hat = Quaternion.OfRotationAxisAngle(v_d, a_d * tanEl);
                q_TOBS_ENU_hat = q_TOBS_TNOM_hat * q_TNOM_ENU_hat;
                A = HSYMath.quaternion_to_DCM(q_TNOM_ENU_hat);
                Ainv = HSYMath.quaternion_to_DCM(q_TNOM_ENU_hat.InverseQ());
                M[0, 0] = 2 * A[2, 2] / Math.Sqrt(A[1, 2] * A[1, 2] + A[0, 2] * A[0, 2]) - 2 * A[2, 2] * A[1, 2] * A[1, 2] / Math.Pow(A[1, 2] * A[1, 2] + A[0, 2] * A[0, 2], 1.5);
                M[1, 0] = 2 * A[0, 2] * A[1, 2] * A[2, 2] / Math.Pow(A[0, 2] * A[0, 2] + A[1, 2] * A[1, 2], 1.5);
                M[0, 1] = M[1, 0];
                M[1, 1] = 2 * A[2, 2] / Math.Sqrt(A[1, 2] * A[1, 2] + A[0, 2] * A[0, 2]) + 2 * A[2, 2] * A[0, 2] * A[0, 2] / Math.Pow(A[1, 2] * A[1, 2] + A[0, 2] * A[0, 2], 1.5);
                M[0, 2] = -2 * A[0, 2] / Math.Sqrt(A[0, 2] * A[0, 2] + A[1, 2] * A[1, 2]);
                M[1, 2] = -2 * A[0, 2] / Math.Sqrt(A[1, 2] * A[0, 2] + A[1, 2] * A[1, 2]);

                Quaternion dq_TOBS_J2K = q_ST_TOBS.InverseQ() * q_ST_J2K_meas_atmref[i] / (q_TOBS_ENU_hat * q_ENU_J2K_cal[i]);
                dq_TOBS_J2K.MakePlusQ4();

                DenseMatrix identity = new DenseMatrix(3);
                identity[0, 0] = 1;
                identity[1, 1] = 1;
                identity[2, 2] = 1;
                DenseMatrix H1 = 0.5 * tanEl * HSYMath.quaternion_to_DCM(q_ST_TOBS) * v_d_matrix;
                DenseMatrix temp1 = (DenseMatrix)(0.5 * Math.Sin(a_d * tanEl) - Math.Sin(0.5 * a_d * tanEl) * Math.Sin(0.5 * a_d * tanEl) * HSYMath.vector_3x1_to_skew_sym_matrix_3x3_cross(v_d) * M);
                DenseMatrix H2 = (DenseMatrix)(HSYMath.quaternion_to_DCM(q_ST_TOBS * q_TOBS_TNOM_hat) + HSYMath.quaternion_to_DCM(q_ST_TOBS) * temp1);
                DenseVector H3 = -0.5 * (HSYMath.quaternion_to_DCM(q_ST_TOBS * q_TOBS_TNOM_hat * q_TNOM_GIM_hat) + HSYMath.quaternion_to_DCM(q_ST_TOBS) * temp1 * HSYMath.quaternion_to_DCM(q_TNOM_GIM_hat)) * (identity - HSYMath.quaternion_to_DCM(q_alt_hat)) * z;
                DenseMatrix H4 = HSYMath.quaternion_to_DCM(q_ST_TOBS * q_TOBS_TNOM_hat * q_TNOM_GIM_hat * q_GIM_MNT_hat) + HSYMath.quaternion_to_DCM(q_ST_TOBS) * temp1 * HSYMath.quaternion_to_DCM(q_TNOM_GIM_hat * q_GIM_MNT_hat);

                for (int j = 0; j < 3; j++)
                {
                    y[3 * i + j] = dq_TOBS_J2K[j];
                    H[3 * i + j, 0] = H1[j, 0];
                    H[3 * i + j, 4] = H3[j];
                    for (int k = 0; k < 3; k++)
                    {
                        H[3 * i + j, 1 + k] = H2[j, k];
                        H[3 * i + j, 5 + k] = H4[j, k];
                        W[3 * i + j, 3 * i + k] = Rinv[j, k];
                    }
                }
            }

            DenseMatrix HT = (DenseMatrix)H.Transpose();
            DenseMatrix HTWHI = (DenseMatrix)(HT * W * H).Inverse();
            DenseVector x = HTWHI * HT * W * y;

            dq_TNOM_GIM = new Quaternion();
            dq_MNT_ENU = new Quaternion();
            da_d = x[0];
            dtheta_NP = x[4];
            for (int i = 0; i < 3; i++)
            {
                dq_TNOM_GIM[i] = x[1 + i];
                dq_MNT_ENU[i] = x[5 + i];
            }
            res = y;
        }


        private void FineCal_9term()
        {
            Quaternion dq_ST_GIM, dq_MNT_ENU;
            double da_d, dtheta_NP, dc;
            DenseVector res = new DenseVector(3 * CalMeasurements.Count);
            for (int i = 0; i < 100; i++)
            {
                NLS_iter_9term(out dq_ST_GIM, out dq_MNT_ENU, out da_d, out dtheta_NP, out dc, out res);
                q_ST_GIM_hat = dq_ST_GIM.Normalize() * q_ST_GIM_hat;
                q_MNT_ENU_hat = dq_MNT_ENU.Normalize() * q_MNT_ENU_hat;
                a_d += da_d;
                theta_NP += dtheta_NP;
                c += dc;

                double dxMag = dq_ST_GIM.SubVector(0, 3).L2Norm() + dq_MNT_ENU.SubVector(0, 3).L2Norm() + Math.Abs(da_d) + Math.Abs(dtheta_NP) + Math.Abs(dc);
                if (dxMag < 0.001 * HSYMath.ASTR)
                {
                    break;
                }
            }

            // Check against ECR
            Quaternion q_MNT_ECI = q_MNT_ENU_hat * q_ENU_ECEF;

            CalRMS = 2 * res.L2Norm() / Math.Sqrt(res.Count) * HSYMath.RTAS;

            string fn_CalResLog = "CalRes9term_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
            if (sw_CalResLog != null)
                sw_CalResLog.Close();
            sw_CalResLog = new StreamWriter(fn_CalResLog);
            for (int i = 0; i < CalMeasurements.Count; i++)
            {
                double[] AziAlt = CalMeasurements[i].GetAziAlt();
                string str = AziAlt[0].ToString() + ", " + AziAlt[1].ToString() + ", ";
                for (int j = 0; j < 3; j++)
                    str += res[i * 3 + j].ToString() + ", ";
                sw_CalResLog.WriteLine(str);
            }
            sw_CalResLog.Close();
        }

        private void NLS_iter_9term(out Quaternion dq_ST_GIM, out Quaternion dq_MNT_ENU, out double da_d, out double dtheta_NP, out double dc, out DenseVector res)
        {
            DenseMatrix Rinv = new DenseMatrix(3, 3); // noise covariance
            for (int i = 0; i < 3; i++)
            {
                double w = ST_noise[i] * HSYMath.ASTR;
                w = w * w;
                Rinv[i, i] = 1 / w;
            }
            int n = CalMeasurements.Count;
            DenseVector y = new DenseVector(3 * n);
            DenseMatrix H = new DenseMatrix(3 * n, 9);
            DenseMatrix W = new DenseMatrix(3 * n, 3 * n); // weighting matrix, inverse of covariance matrix
            // q_ST_GIM_hat, q_MNT_ENU_hat
            DenseVector z = new DenseVector(new double[] { 0, 0, 1 });
            Quaternion q_alt_hat = new Quaternion();
            Quaternion q_GIM1_GIM2_hat = new Quaternion();
            Quaternion q_GIM0_GIM1_hat = new Quaternion();
            Quaternion q_GIM_MNT_hat = new Quaternion();
            Quaternion q_ST_J2K_hat = new Quaternion();
            for (int i = 0; i < n; i++)
            {
                q_alt_hat[0] = Math.Cos(theta_NP) * Math.Sin(0.5 * AziAlt[i][1]);
                q_alt_hat[1] = Math.Sin(theta_NP) * Math.Sin(0.5 * AziAlt[i][1]);
                q_alt_hat[3] = Math.Cos(0.5 * AziAlt[i][1]);
                q_GIM0_GIM1_hat[2] = Math.Sin(c * Math.Cos(Math.PI + AziAlt[i][0]) / 2);
                q_GIM0_GIM1_hat[3] = Math.Cos(c * Math.Cos(Math.PI + AziAlt[i][0]) / 2);
                q_GIM1_GIM2_hat[0] = Math.Sin((a_d * Math.Cos(Math.Max(AziAlt[i][1], Math.PI / 4))) / 2);
                q_GIM1_GIM2_hat[3] = Math.Cos((a_d * Math.Cos(Math.Max(AziAlt[i][1], Math.PI / 4))) / 2);
                q_GIM_MNT_hat = AziAltToQuat(AziAlt[i][0], AziAlt[i][1], theta_NP);
                q_ST_J2K_hat = q_GIM0_GIM1_hat * q_GIM1_GIM2_hat * q_ST_GIM_hat * q_GIM_MNT_hat * q_MNT_ENU_hat * q_ENU_J2K_cal[i];

                Quaternion dq_ST_J2K = q_ST_J2K_meas_atmref[i] / (q_ST_J2K_hat);
                dq_ST_J2K.MakePlusQ4();

                DenseMatrix identity = new DenseMatrix(3);
                identity[0, 0] = 1;
                identity[1, 1] = 1;
                identity[2, 2] = 1;
                DenseMatrix H1 = HSYMath.quaternion_to_DCM(q_GIM0_GIM1_hat * q_GIM1_GIM2_hat);
                DenseVector v = new DenseVector(3);
                v[2] = 0.5 * Math.Cos(Math.PI + AziAlt[i][0]);
                // DenseVector H2 = HSYMath.quaternion_to_DCM(q_ST_GIM_hat) * v;
                DenseVector H2 = v;
                DenseVector v2 = new DenseVector(3);
                v2[0] = 0.5 * Math.Cos(Math.Max(AziAlt[i][1],Math.PI/4));
                //DenseVector H3 = HSYMath.quaternion_to_DCM(q_ST_GIM_hat * q_GIM0_GIM1_hat) * v2;
                DenseVector H3 = HSYMath.quaternion_to_DCM(q_GIM0_GIM1_hat) * v2;
                DenseVector H4 = 0.5 * HSYMath.quaternion_to_DCM(q_GIM0_GIM1_hat * q_GIM1_GIM2_hat * q_ST_GIM_hat) * (HSYMath.quaternion_to_DCM(q_alt_hat) - identity) * z;
                DenseMatrix H5 = HSYMath.quaternion_to_DCM(q_GIM0_GIM1_hat * q_GIM1_GIM2_hat * q_ST_GIM_hat * q_GIM_MNT_hat);

                for (int j = 0; j < 3; j++)
                {
                    y[3 * i + j] = dq_ST_J2K[j];
                    H[3 * i + j, 3] = H3[j];
                    H[3 * i + j, 4] = H2[j];
                    H[3 * i + j, 5] = H4[j];
                    for (int k = 0; k < 3; k++)
                    {
                        H[3 * i + j, k] = H1[j, k];
                        H[3 * i + j, 6 + k] = H5[j, k];
                        W[3 * i + j, 3 * i + k] = Rinv[j, k];
                    }
                }
            }

            DenseMatrix HT = (DenseMatrix)H.Transpose();
            DenseMatrix HTWHI = (DenseMatrix)(HT * W * H).Inverse();
            DenseVector x = HTWHI * HT * W * y;

            dq_ST_GIM = new Quaternion();
            dq_MNT_ENU = new Quaternion();
            da_d = x[3];
            dc = x[4];
            dtheta_NP = x[5];
            for (int i = 0; i < 3; i++)
            {
                dq_ST_GIM[i] = x[i];
                dq_MNT_ENU[i] = x[6 + i];
            }
            res = y;
        }

        private void FineCal_new()
        {
            Quaternion dq_ST_GIM, dq_MNT_ENU;
            double da_d, dtheta_NP, dc;
            DenseVector res = new DenseVector(3 * CalMeasurements.Count);
            for (int i = 0; i < 100; i++)
            {
                NLS_iter_new(out dq_ST_GIM, out dq_MNT_ENU, out dc, out res);
                q_ST_GIM_hat = dq_ST_GIM.Normalize() * q_ST_GIM_hat;
                q_MNT_ENU_hat = dq_MNT_ENU.Normalize() * q_MNT_ENU_hat;
                c += dc;

                double dxMag = dq_ST_GIM.SubVector(0, 3).L2Norm() + dq_MNT_ENU.SubVector(0, 3).L2Norm() + Math.Abs(dc);
                if (dxMag < 0.001 * HSYMath.ASTR)
                {
                    break;
                }
            }

            // Check against ECR
            Quaternion q_MNT_ECI = q_MNT_ENU_hat * q_ENU_ECEF;

            CalRMS = 2 * res.L2Norm() / Math.Sqrt(res.Count) * HSYMath.RTAS;

            string fn_CalResLog = "CalResNew_" + DateTime.Now.ToString("yyyyMMdd_HHmmss.ff") + ".csv";
            if (sw_CalResLog != null)
                sw_CalResLog.Close();
            sw_CalResLog = new StreamWriter(fn_CalResLog);
            for (int i = 0; i < CalMeasurements.Count; i++)
            {
                double[] AziAlt = CalMeasurements[i].GetAziAlt();
                string str = AziAlt[0].ToString() + ", " + AziAlt[1].ToString() + ", ";
                for (int j = 0; j < 3; j++)
                    str += res[i * 3 + j].ToString() + ", ";
                sw_CalResLog.WriteLine(str);
            }
            sw_CalResLog.Close();
        }

        private void NLS_iter_new(out Quaternion dq_ST_GIM, out Quaternion dq_MNT_ENU, out double dc, out DenseVector res)
        {
            DenseMatrix Rinv = new DenseMatrix(3, 3); // noise covariance
            for (int i = 0; i < 3; i++)
            {
                double w = ST_noise[i] * HSYMath.ASTR;
                w = w * w;
                Rinv[i, i] = 1 / w;
            }
            int n = CalMeasurements.Count;
            DenseVector y = new DenseVector(3 * n);
            DenseMatrix H = new DenseMatrix(3 * n, 7);
            DenseMatrix W = new DenseMatrix(3 * n, 3 * n); // weighting matrix, inverse of covariance matrix
            // q_ST_GIM_hat, q_MNT_ENU_hat
            DenseVector z = new DenseVector(new double[] { 0, 0, 1 });
            Quaternion q_GIM0_GIM1_hat = new Quaternion();
            Quaternion q_GIM_MNT_hat = new Quaternion();
            Quaternion q_ST_J2K_hat = new Quaternion();
            for (int i = 0; i < n; i++)
            {
                q_GIM0_GIM1_hat[2] = Math.Sin(c * Math.Cos(AziAlt[i][0]) / 2);
                q_GIM0_GIM1_hat[3] = Math.Cos(c * Math.Cos(AziAlt[i][0]) / 2);
                q_GIM_MNT_hat = AziAltToQuat(AziAlt[i][0], AziAlt[i][1], 0);
                q_ST_J2K_hat = q_ST_GIM_hat * q_GIM0_GIM1_hat * q_GIM_MNT_hat * q_MNT_ENU_hat * q_ENU_J2K_cal[i];

                Quaternion dq_ST_J2K = q_ST_J2K_meas_atmref[i] / (q_ST_J2K_hat);
                dq_ST_J2K.MakePlusQ4();

                DenseMatrix identity = new DenseMatrix(3);
                identity[0, 0] = 1;
                identity[1, 1] = 1;
                identity[2, 2] = 1;
                DenseVector v = new DenseVector(3);
                v[2] = 0.5 * Math.Cos(AziAlt[i][0]);
                DenseVector H2 = HSYMath.quaternion_to_DCM(q_ST_GIM_hat) * v;
                DenseMatrix H3 = HSYMath.quaternion_to_DCM(q_ST_GIM_hat * q_GIM0_GIM1_hat * q_GIM_MNT_hat);

                for (int j = 0; j < 3; j++)
                {
                    y[3 * i + j] = dq_ST_J2K[j];
                    H[3 * i + j, 3] = H2[j];
                    for (int k = 0; k < 3; k++)
                    {
                        H[3 * i + j, k] = identity[j, k];
                        H[3 * i + j, 4 + k] = H3[j, k];
                        W[3 * i + j, 3 * i + k] = Rinv[j, k];
                    }
                }
            }

            DenseMatrix HT = (DenseMatrix)H.Transpose();
            DenseMatrix HTWHI = (DenseMatrix)(HT * W * H).Inverse();
            DenseVector x = HTWHI * HT * W * y;

            dq_ST_GIM = new Quaternion();
            dq_MNT_ENU = new Quaternion();
            dc = x[3];
            for (int i = 0; i < 3; i++)
            {
                dq_ST_GIM[i] = x[i];
                dq_MNT_ENU[i] = x[4 + i];
            }
            res = y;
        }

        public static Quaternion Cal_q_ECEF_J2K(DateTime t)
        {
            double JD, JED;
            JD = HSYTime.DateTime_to_JD(t);
            JED = HSYTime.JD_to_JED(JD);

            // Calculate J2K to ECEF
            DenseMatrix DCM_ECEF_J2K, DCM_ECEF_J2K_dot;
            HSYIERSc.J2k2ECR(JD, JED, out DCM_ECEF_J2K, out DCM_ECEF_J2K_dot);

            Quaternion q_ECEF_J2K = HSYMath.DCM_to_quaternion(DCM_ECEF_J2K);
            return q_ECEF_J2K;
        }

        public static Quaternion Cal_q_ENU_J2K(DateTime t)
        {
            double JD, JED;
            JD = HSYTime.DateTime_to_JD(t);
            JED = HSYTime.JD_to_JED(JD);

            // Calculate J2K to ECEF
            DenseMatrix DCM_ECEF_J2K, DCM_ECEF_J2K_dot;
            HSYIERSc.J2k2ECR(JD, JED, out DCM_ECEF_J2K, out DCM_ECEF_J2K_dot);

            // Calculate ECEF to ENU
            DenseMatrix DCM_ENU_ECEF = HSYEarth.DCM_ECR2ENU(lat, lon);
            q_ENU_ECEF = HSYMath.DCM_to_quaternion(DCM_ENU_ECEF);
            Quaternion q_ECEF_J2K = HSYMath.DCM_to_quaternion(DCM_ECEF_J2K);

            // Calculate J2K to ENU
            return q_ENU_ECEF * q_ECEF_J2K;
        }

        public Quaternion AziAltToQuat(double Azi, double Alt, double theta_NP)
        {
            Quaternion q_azi = new Quaternion();
            q_azi[1] = Math.Sin(0.5 * Azi);
            q_azi[3] = Math.Cos(0.5 * Azi);

            Quaternion q_alt = new Quaternion();
            q_alt[0] = Math.Cos(theta_NP) * Math.Sin(0.5 * Alt);
            q_alt[1] = Math.Sin(theta_NP) * Math.Sin(0.5 * Alt);
            q_alt[3] = Math.Cos(0.5 * Alt);

            Quaternion q = q_alt * q_azi;
            q.MakePlusQ4();
            return q;
        }

        public Quaternion AziAltToQuat(double Azi, double Alt)
        {
            Quaternion q_azi = new Quaternion();
            q_azi[1] = Math.Sin(0.5 * Azi);
            q_azi[3] = Math.Cos(0.5 * Azi);

            Quaternion q_alt = new Quaternion();
            q_alt[0] = Math.Sin(0.5 * Alt);
            q_alt[3] = Math.Cos(0.5 * Alt);

            Quaternion q = q_alt * q_azi;
            q.MakePlusQ4();
            return q;
        }

        public void WriteToFile(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(CalMeasurements.Count);
            for (int i = 0; i < CalMeasurements.Count; i++)
                CalMeasurements[i].WriteToFile(bw);
            WriteQtoFile(bw, q_ST_TOBS);
            WriteQtoFile(bw, q_TNOM_GIM_hat);
            WriteQtoFile(bw, q_ST_GIM_hat);
            WriteQtoFile(bw, q_MNT_ENU_hat);
            WriteQtoFile(bw, q_ENU_ECEF);
            bw.Write(theta_NP);
            bw.Write(a_d);
            bw.Write(CalRMS);
            bw.Close();
        }

        private void WriteQtoFile(BinaryWriter bw, Quaternion q)
        {
            for (int i = 0; i < 4; i++)
                bw.Write(q[i]);
            return;
        }

        public void LoadFromFile(string fn)
        {
            FileStream fs = new FileStream(fn, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            CalMeasurements.Clear();
            int n = br.ReadInt32();
            for (int i = 0; i < n; i++)
            {
                TelCalMeasurement tcm = new TelCalMeasurement(br);
                CalMeasurements.Add(tcm);
            }
            q_ST_TOBS = ReadQfromFile(br);
            q_TNOM_GIM_hat = ReadQfromFile(br);
            q_ST_GIM_hat = ReadQfromFile(br);
            q_MNT_ENU_hat = ReadQfromFile(br);
            q_ENU_ECEF = ReadQfromFile(br);
            theta_NP = br.ReadDouble();
            a_d = br.ReadDouble();
            CalRMS = br.ReadDouble();

            br.Close();
            calibrated = true;
        }

        private Quaternion ReadQfromFile(BinaryReader br)
        {
            Quaternion q = new Quaternion();
            for (int i = 0; i < 4; i++)
                q[i] = br.ReadDouble();
            return q;
        }
    }
}
