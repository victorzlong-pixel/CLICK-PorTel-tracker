using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using HSYLib.CS;

namespace GS_Tracking_KR
{
    public class CelestronMount  // This class interfaces with the Celestron mount and sends commands in the appropriate format.
    {
        private SerialPort serialPort1;
        private const double oneRot = 4294967296; // maximum value representing one rotation around the axis
        private const int maxRate = 65500; // maximum value that can be sent to the mount

        public CelestronMount(SerialPort sp)
        {
            serialPort1 = sp;
        }

        public double[] ReadAnglesRad() // Read azi alt angles in radians.
        {
            string rx = SendAndReceive("z");
            if (rx == null)
                return null;
            string Azi = rx.Substring(0, 8); // first 8 characters are azimuth, next 8 altitude
            string Alt = rx.Substring(9, 8);

            double[] angles = new double[2];
            angles[0] = HexToRad(Azi);
            angles[1] = HexToRad(Alt);

            return angles;
        }

        public double[] ReadAnglesDeg() // Read azi alt angles in degrees.
        {
            double[] rad = ReadAnglesRad();
            if (rad == null)
                return null;
            double[] angles = new double[2];
            angles[0] = rad[0] * HSYMath.RTD;
            angles[1] = rad[1] * HSYMath.RTD;

            return angles;
        }

        public void SendAngleCommand(double AziRad, double AltRad) // Sends input command angles (in radians) to mount as a GoTo command.
        {
            string AziHex = RadToHex(AziRad);
            string AltHex = RadToHex(AltRad);

            string cmd = "b" + AziHex + "," + AltHex; // format for precision GoTo command
            SendAndReceive(cmd);
        }

        public void SendSlewCommand(double AziArcS, double AltArcS) // Sends slew command in arcsec/s.
        {
            bool AziPos = true;
            bool AltPos = true;

            int AziArcS4 = (int)(AziArcS * 4); // why does Celestron do this? nobody knows
            int AltArcS4 = (int)(AltArcS * 4);
            AziArcS4 = Math.Min(AziArcS4, maxRate); // saturates input command
            AltArcS4 = Math.Min(AltArcS4, maxRate);

            if (AziArcS4 < 0)
            {
                AziPos = false;
                AziArcS4 = -AziArcS4;
            }
            if (AltArcS4 < 0)
            {
                AltPos = false;
                AltArcS4 = -AltArcS4;
            }

            byte[] AziSlewCmd = new byte[8];
            AziSlewCmd[0] = 0x50; // "P"
            AziSlewCmd[1] = 3;
            AziSlewCmd[2] = 16; // corresponds to azimuth
            if (AziPos)
                AziSlewCmd[3] = 6;
            else
                AziSlewCmd[3] = 7;
            AziSlewCmd[4] = (byte)((AziArcS4 >> 8) & 0xFF); // high byte
            AziSlewCmd[5] = (byte)(AziArcS4 & 0xFF); // low byte
            AziSlewCmd[6] = 0;
            AziSlewCmd[7] = 0;

            byte[] AltSlewCmd = new byte[8];
            AltSlewCmd[0] = 0x50; // "P"
            AltSlewCmd[1] = 3;
            AltSlewCmd[2] = 17; // corresponds to altitude
            if (AltPos)
                AltSlewCmd[3] = 6;
            else
                AltSlewCmd[3] = 7;
            AltSlewCmd[4] = (byte)((AltArcS4 >> 8) & 0xFF); // high byte
            AltSlewCmd[5] = (byte)(AltArcS4 & 0xFF); // low byte
            AltSlewCmd[6] = 0;
            AltSlewCmd[7] = 0;

            SendAndReceive(AziSlewCmd);
            SendAndReceive(AltSlewCmd);
        }

        public void StopGoTo() // Tells mount to stop executing GoTo command.
        {
            SendAndReceive("M");
        }

        private string RadToHex(double rad) // Convert radians to Celestron's command format in hex.
        {
            double twopi = 2 * Math.PI;
            while (rad > twopi || rad < 0)
            {
                if (rad > twopi)
                    rad -= twopi;
                else if (rad < 0)
                    rad += twopi;
            }
            double frac = rad / twopi * oneRot; // fraction of a rotation
            uint val = (uint)Math.Round(frac);
            string hex = String.Format("{0:X8}", val); // convert fraction to hex

            return hex;
        }

        private double HexToRad(string hex) // Convert Celestron's return angle values from hex to double representation of radians.
        {
            uint a = Convert.ToUInt32(hex, 16); // convert hex to integer
            double val = (double)a; // count for current angle
            double rad = val / oneRot * 2 * Math.PI; // angle in radians

            return rad;
        }

        private string SendAndReceive(string msg) // Handles all send and received data over serial.
        {
            serialPort1.Write(msg);

            string rx = "";
            DateTime t0 = DateTime.UtcNow;
            TimeSpan dt = DateTime.UtcNow - t0;
            while (dt.TotalMilliseconds < 1000) // timeout
            {
                rx = rx + serialPort1.ReadExisting();
                if (rx.Length > 0)
                    if (rx[rx.Length - 1] == '#') // Celestron ends all commands with #
                        return rx;
                dt = DateTime.UtcNow - t0;
            }
            return null;
        }

        private string SendAndReceive(byte[] msg) // Handles all send and received data over serial.
        {
            serialPort1.Write(msg, 0, msg.Length);

            string rx = "";
            DateTime t0 = DateTime.UtcNow;
            TimeSpan dt = DateTime.UtcNow - t0;
            while (dt.TotalMilliseconds < 1000) // timeout
            {
                rx = rx + serialPort1.ReadExisting();
                if (rx.Length > 0)
                    if (rx[rx.Length - 1] == '#') // Celestron ends all commands with #
                        return rx;
                dt = DateTime.UtcNow - t0;
            }
            return null;
        }
    }
}
