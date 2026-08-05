using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MathNet.Numerics.LinearAlgebra.Double;

namespace GS_Tracking_KR
{
    public class RxMeasPwr
    {
        private List<SingleMeas> meas = null;
        private DSO1004A oScope = null;
        private int indexMaxP;
        private DenseMatrix FSMgain = null;
       // private double[] centroid;

        public RxMeasPwr(double[] centroid)
        {
            meas = new List<SingleMeas>();
            if (oScope == null)
            {
                oScope = new DSO1004A();
                oScope.initialize();
            }
            findPmax();
            findMaxPindex();
            calibrateGain(centroid);
        }

        public class SingleMeas
        {
            public double FSM_Vx;
            public double FSM_Vy;
            public double cam_x;
            public double cam_y;
            public double APD_V;
        }

        public List<SingleMeas> getMeasList()
        {
            return meas;
        }

        private void addMeas(double FSM_Vx, double FSM_Vy, double cam_x, double cam_y, double APD_v)
        {
            SingleMeas sm = new SingleMeas();
            sm.FSM_Vx = FSM_Vx;
            sm.FSM_Vy = FSM_Vy;
            sm.cam_x = cam_x;
            sm.cam_y = cam_y;
            sm.APD_V = APD_v;

            meas.Add(sm);
        }

        public int getMaxPindex()
        {
            return indexMaxP;
        }

        public DenseMatrix getFSMgain(double[] centroid)
        { 
            if (FSMgain == null)
                calibrateGain(centroid);
            return FSMgain;
        }

        private void findMaxPindex()
        {
            if (meas.Count == 0)
                return;

            int index = 0;
            double Pmax = meas[0].APD_V;
            for (int i = 1; i < meas.Count; i++)
            {
                double Pcur = meas[i].APD_V;
                if (Pcur > Pmax)
                {
                    index = i;
                    Pmax = Pcur;
                }
            }
            indexMaxP = index;
        }

        public void findPmax()
        {
            int n = 5;
            double delta_V = 1;
            double x = 0;
            double y = 0;
            double Pmax = 0;
            double Pmax_old = 0;

            while (((Pmax - Pmax_old) / (Pmax_old + 1e-16) > 5e-2) || delta_V > 0.1)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        // set FSM voltages
                        double FSM_Vx = (j - (n - 1) / 2) * delta_V + x;
                        double FSM_Vy = (i - (n - 1) / 2) * delta_V + y;
                        OIM_Control_FT4222.setOIM_Vxy(FSM_Vx, FSM_Vy);
                        Thread.Sleep(100); // give oscilliscope time to catch up

                        // take IR camera image


                        // measure oscilloscope output
                        double APD_V = oScope.getVavg();

                        // add combined measurements
                        addMeas(FSM_Vx, FSM_Vy, 0, 0, APD_V);
                    }
                }
                findMaxPindex();
                Pmax_old = Pmax;
                Pmax = meas[indexMaxP].APD_V;
                x = meas[indexMaxP].FSM_Vx;
                y = meas[indexMaxP].FSM_Vy;
                delta_V = delta_V / (n - 1);
            }
            oScope.close();
            oScope = null;
        }

        public void calibrateGain(double[] centroid)
        {
            DenseVector y = new DenseVector(10);
            DenseMatrix H = new DenseMatrix(10, 4);
            double stepV = 1;

            double Vx_max = 0, Vy_max = 0;
            if (meas.Count > 0)
            {
                Vx_max = meas[indexMaxP].FSM_Vx;
                Vy_max = meas[indexMaxP].FSM_Vy;
            }

            y[0] = 0;
            y[1] = 0;
            OIM_Control_FT4222.setOIM_Vxy(Vx_max, Vy_max);
            Thread.Sleep(500); // wait for motion to settle
            double x_c = centroid[0];
            double y_c = centroid[1];
            H[0, 0] = 0;
            H[0, 1] = 0;
            H[1, 2] = H[0, 0];
            H[1, 3] = H[0, 1];

            y[2] = -stepV;
            y[3] = -stepV;
            OIM_Control_FT4222.setOIM_Vxy(Vx_max + y[2], Vy_max + y[3]);
            Thread.Sleep(500); // wait for motion to settle
            H[2, 0] = centroid[0] - x_c;
            H[2, 1] = centroid[1] - y_c;
            H[3, 2] = H[2, 0];
            H[3, 3] = H[2, 1];

            y[4] = stepV;
            y[5] = -stepV;
            OIM_Control_FT4222.setOIM_Vxy(Vx_max + y[4], Vy_max + y[5]);
            Thread.Sleep(500); // wait for motion to settle
            H[4, 0] = centroid[0] - x_c;
            H[4, 1] = centroid[1] - y_c;
            H[5, 2] = H[4, 0];
            H[5, 3] = H[4, 1];

            y[6] = -stepV;
            y[7] = stepV;
            OIM_Control_FT4222.setOIM_Vxy(Vx_max + y[6], Vy_max + y[7]);
            Thread.Sleep(500); // wait for motion to settle
            H[6, 0] = centroid[0] - x_c;
            H[6, 1] = centroid[1] - y_c;
            H[7, 2] = H[6, 0];
            H[7, 3] = H[6, 1];

            y[8] = stepV;
            y[9] = stepV;
            OIM_Control_FT4222.setOIM_Vxy(Vx_max + y[8], Vy_max + y[9]);
            Thread.Sleep(500); // wait for motion to settle
            H[8, 0] = centroid[0] - x_c;
            H[8, 1] = centroid[1] - y_c;
            H[9, 2] = H[8, 0];
            H[9, 3] = H[8, 1];

            DenseVector gainV = (DenseVector)(H.TransposeThisAndMultiply(H).Inverse() * H.TransposeThisAndMultiply(y));
            FSMgain = new DenseMatrix(2, 2);
            FSMgain[0, 0] = gainV[0];
            FSMgain[0, 1] = gainV[1];
            FSMgain[1, 0] = gainV[2];
            FSMgain[1, 1] = gainV[3];
        }
    }
}
