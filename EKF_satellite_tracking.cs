using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;
using HSYLib.CS;

namespace GS_Tracking_KR
{
    public class EKF_satellite_tracking
    {
        private TelPointingModel TelPntModel = null;
        private double t_offset = 0;
        private double P1 = 0.5;
        private double Q1 = 0.05;
        private double[] ST_noise = new double[] {20 * 35e-3 * HSYMath.ASTR, 20 * 35e-3 * HSYMath.ASTR, 20 * 35e-3 * HSYMath.ASTR};

        public EKF_satellite_tracking(TelPointingModel _TelPntModel)
        {
            TelPntModel = _TelPntModel;
        }

        public void EKFupdate1term(DateTime t, double azi, double alt, DenseVector v_ST, DenseVector LOSteldq_ST, DenseVector v_ECEF, DenseVector vdot_ECEF)
        {
            if (Math.Abs(t_offset) >= 10)
            {
                t_offset = 10;
                P1 = -1;
                return;
            }

            // propagate covariance
            P1 = P1 + Q1;

            // update covariance and state
            Quaternion q_ST_ECEF_hat = TelPntModel.Getq_ST_GIM() * TelPntModel.AziAltToQuat(azi, alt) * TelPntModel.Getq_MNT_ENU() * TelPntModel.Getq_ENU_ECEF();
            DenseVector v_ST_hat = HSYMath.quaternion_to_DCM(q_ST_ECEF_hat) * v_ECEF;
            DenseMatrix H = new DenseMatrix(3, 1);
            DenseVector temp = HSYMath.quaternion_to_DCM(q_ST_ECEF_hat) * vdot_ECEF;
            H[0, 0] = temp[0];
            H[1, 0] = temp[1];
            H[2, 0] = temp[2];
            DenseMatrix HT = (DenseMatrix) H.Transpose();
            DenseMatrix R = new DenseMatrix(3);
            for (int i = 0; i < 3; i++)
            {
                R[i, i] = ST_noise[i];
            }
            DenseMatrix K = (DenseMatrix) (P1 * HT * (H * P1 * HT + R).Inverse());
            DenseVector dt_offset = K * (v_ST - v_ST_hat);
            t_offset = t_offset + dt_offset[0];
            DenseMatrix KH = K * H;
            P1 = P1 - KH[0, 0]*P1;
        }

        public double GetToffset()
        {
            return t_offset;
        }

        public double GetP1()
        {
            return P1;
        }

        // Revisit this EKF at some point!
        /*public void EKFupdate4term(DateTime t, double azi, double alt, DenseVector v_ST, DenseVector v_ECEF, DenseVector rSat_ECEF, DenseVector rdotSat_ECEF, DenseVector rGS_ECEF, out double dt)
        {
            DenseMatrix P = new DenseMatrix(4);
            DenseMatrix R = new DenseMatrix(3);
            Quaternion q_ST_MNT_hat = q_ST_GIM_hat * AziAltToQuat(azi, alt, theta_NP);
            DenseVector v_MNT_hat = HSYMath.quaternion_to_DCM(q_MNT_ENU_hat * q_ENU_ECEF) * v_ECEF;
            DenseVector v_ST_hat = HSYMath.quaternion_to_DCM(q_ST_MNT_hat) * v_MNT_hat;
            DenseMatrix H1 = 2 * HSYMath.quaternion_to_DCM(q_ST_MNT_hat) * HSYMath.vector_3x1_to_skew_sym_matrix_3x3_cross(v_ST_hat);
            DenseMatrix temp = new DenseMatrix(3);
            double rNorm = (rSat_ECEF - rGS_ECEF).L2Norm();
            temp[0, 0] = ((rSat_ECEF[1] - rGS_ECEF[1]) * (rSat_ECEF[1] - rGS_ECEF[1]) + (rSat_ECEF[2] - rGS_ECEF[2]) * (rSat_ECEF[2] - rGS_ECEF[2])) / (rNorm * rNorm * rNorm);
            temp[0, 1] = -(rSat_ECEF[0] - rGS_ECEF[0]) * (rSat_ECEF[1] - rGS_ECEF[1]) / (rNorm * rNorm * rNorm);
            temp[0, 2] = -(rSat_ECEF[0] - rGS_ECEF[0]) * (rSat_ECEF[2] - rGS_ECEF[2]) / (rNorm * rNorm * rNorm);
            temp[1, 0] = temp[0, 1];
            temp[1, 1] = ((rSat_ECEF[0] - rGS_ECEF[0]) * (rSat_ECEF[0] - rGS_ECEF[0]) + (rSat_ECEF[2] - rGS_ECEF[2]) * (rSat_ECEF[2] - rGS_ECEF[2])) / (rNorm * rNorm * rNorm);
            temp[1, 2] = -(rSat_ECEF[1] - rGS_ECEF[1]) * (rSat_ECEF[2] - rGS_ECEF[2]) / (rNorm * rNorm * rNorm);
            temp[2, 0] = temp[0, 2];
            temp[2, 1] = temp[1, 2];
            temp[2, 2] = ((rSat_ECEF[0] - rGS_ECEF[0]) * (rSat_ECEF[0] - rGS_ECEF[0]) + (rSat_ECEF[1] - rGS_ECEF[1]) * (rSat_ECEF[1] - rGS_ECEF[1])) / (rNorm * rNorm * rNorm);
            DenseVector H2 = HSYMath.quaternion_to_DCM(q_ST_MNT_hat * q_MNT_ENU_hat * q_ENU_ECEF) * temp * rdotSat_ECEF;

            DenseMatrix H = new DenseMatrix(3, 4);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    H[i, j] = H1[i, j];
                H[3 + i, 3] = H2[i];
            }

            DenseMatrix HT = (DenseMatrix)H.Transpose();
            DenseMatrix K = (DenseMatrix)(P * HT * (H * P * HT + R).Inverse());
            DenseVector deltaX = K * (v_ST - v_ST_hat);
            P = P - K * H;

            dt = 0;
        }*/
    }
}
