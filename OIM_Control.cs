using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GS_Tracking_KR
{
    class OIM_Control_FT4222
    {
        /*[DllImport("OIM_Control_FT4222.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int initFT232H_SPI(byte[] _msg);

        [DllImport("OIM_Control_FT4222.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int closeFT232H_SPI();*/
        //C:\\Users\\NODE_GS\\PGrenfell\\Test1\\PorTeL\\Debug\\
        //C:\\Users\\NODE_GS\\PGrenfell\\Test1\\PorTeL\\Release\\
        [DllImport("OIM_Control_FT4222.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int setOIM_Vxy(double Vx, double Vy);

        [DllImport("OIM_Control_FT4222.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int initFT4222H_SPI(byte[] _msg);

        [DllImport("OIM_Control_FT4222.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int closeFT4222H_SPI();
    }
}
