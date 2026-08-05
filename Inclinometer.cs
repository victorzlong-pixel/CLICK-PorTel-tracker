using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using HSYLib.CS;

namespace GS_Tracking_KR
{
    public class Inclinometer
    {
        private SerialPort sp;

        public Inclinometer(SerialPort sp1)
        {
            sp = sp1;

            // Set default mode to respond to request only
            byte[] msg = new byte[6];
            msg[0] = 0x68;
            msg[1] = 0x05;
            msg[2] = 0x00;
            msg[3] = 0x0C;
            msg[4] = 0x00;
            msg[5] = 0x05 + 0x0C;
            sp.Write(msg, 0, msg.Length);
            DateTime t0 = DateTime.UtcNow;
            TimeSpan dt = DateTime.UtcNow - t0;
            while (dt.TotalMilliseconds < 1000) // dump existing data
            {
                while (sp.BytesToRead > 0)
                {
                    byte b = (byte)sp.ReadByte();
                }
                dt = DateTime.UtcNow - t0;
            }
        }

        public double[] ReadAnglesDegTemp()
        {
            // Read angles request
            byte[] msg = new byte[5];
            msg[0] = 0x68;
            msg[1] = 0x04;
            msg[2] = 0x00;
            msg[3] = 0x04;
            msg[4] = 0x08;

            byte[] rx = SendAndReceiveBytes(msg);
            if (rx.Length > 0)
            {
                double[] xyt = parseXYT(rx);
                return xyt;
            }
            return null;
        }

        private byte[] SendAndReceiveBytes(byte[] msg) // Handles all send and received data over serial.
        {
            sp.Write(msg, 0, msg.Length);

            DateTime t0 = DateTime.UtcNow;
            TimeSpan dt = DateTime.UtcNow - t0;
            List<byte> bl = new List<byte>();

            while (dt.TotalMilliseconds < 100) // timeout
            {
                while (sp.BytesToRead > 0)
                {
                    byte b = (byte)sp.ReadByte();
                    bl.Add(b);
                }
                dt = DateTime.UtcNow - t0;
            }
            return bl.ToArray();
        }

        private double[] parseXYT(byte[] rx)
        {
            double[] xyt = new double[3];

            double x = ((rx[4] & 0x0F) * 10);
            x += ((rx[5] & 0xF0) >> 4);
            x += ((rx[5] & 0x0F) / 10.0);
            x += (((rx[6] & 0xF0) >> 4) / 100.0);
            x += ((rx[6] & 0x0F) / 1000.0);
            if ((rx[4] & 0xF0) > 1)
                x = -x;
            xyt[0] = x;

            double y = ((rx[7] & 0x0F) * 10);
            y += ((rx[8] & 0xF0) >> 4);
            y += ((rx[8] & 0x0F) / 10.0);
            y += (((rx[9] & 0xF0) >> 4) / 100.0);
            y += ((rx[9] & 0x0F) / 1000.0);
            if ((rx[7] & 0xF0) > 1)
                y = -y;
            xyt[1] = y;

            double t = ((rx[10] & 0x0F) * 10);
            t += ((rx[11] & 0xF0) >> 4);
            t += ((rx[11] & 0x0F) / 10.0);
            t += (((rx[12] & 0xF0) >> 4) / 100.0);
            t += ((rx[12] & 0x0F) / 1000.0);
            if ((rx[10] & 0xF0) > 1)
                t = -t;
            xyt[2] = t;

            return xyt;
        }
    }
}
