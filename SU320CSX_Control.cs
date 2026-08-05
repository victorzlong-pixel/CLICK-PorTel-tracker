using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GS_Tracking_KR
{
    class SU320CSX_Control
    {
        [DllImport("SU320CSX_Control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int initSU320CSX(byte[] _msg);

        [DllImport("SU320CSX_Control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void closeSU320CSX();

        [DllImport("SU320CSX_Control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int takeImg(byte[] _msg, byte[] img, double[] xy);

        [DllImport("SU320CSX_Control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int startGrab(byte[] _msg, byte[] img, double[] xy);

        [DllImport("SU320CSX_Control.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int stopGrab();
    }
}
