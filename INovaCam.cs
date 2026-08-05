using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using INovaSDK;

namespace GS_Tracking_KR
{
    class INovaCam // This class interfaces with the iNova camera used for star tracking. It is a wrapper for the iNova SDK.
    {
        private bool initialized;
        private INovaCamera cam;
        private int imgCount = 0;
        public double focalLength = 35e-3; // focal length of camera in meters
        public double pixelPitch = 3.75e-6; // pixel pitch of detector in meters

        public INovaCam(INovaCamera iNova)
        {
            cam = iNova;
            initialized = false;
        }

        public bool InitINovaCam()
        {
            if (initialized)
                return true;
            try
            {
                int numCams = cam.MaxCamera(); // number of cameras available
                if (numCams == 0)
                    return false;
                string serialNum = cam.OpenCamera(1);
                if (string.IsNullOrEmpty(serialNum))
                    return false;
                if (serialNum.Equals("DETECTERROR", StringComparison.Ordinal))
                    return false;
                bool success = cam.InitCamera(ENUM_RESOLUTION.RESOLUTION_FULL);
                if (success)
                {
                    cam.PermanentPolling = true;
                    cam.SetFrameSpeed(ENUM_FRAME_SPEED.FRAME_SPEED_NORMAL); // note: this changes the number of bytes stored in a frame (12 vs. 10)
                    cam.SetBlackLevel(0); // 0 - 250
                    cam.SetAnalogGain(510); // 0 - 1023
                    cam.SetHB(384);
                    cam.SetVB(40);
                    cam.SetPixClock(25); // tuning util recommends 33, 25 works better empirically - make sure okay
                    cam.SetExpTime(400); // ms
                    cam.CancelLongExpTime();
                    cam.OpenVideo();
                    initialized = true;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            GetFrameSpeed();
            return initialized;
        }

        public int GetFrameSpeed()
        {
            int fps = (int) cam.GetFrameSpeed();
            return fps;
        }

        public void SetExposure(double ms) // Sets the exposure time of the camera.
        {
            if (!initialized)
                return;
            cam.SetExpTime(ms);
        }

        public PgmImage TakeINovaPic(out DateTime utctime) // Returns pgm image from camera and time of exposure.
        {
            utctime = new DateTime();
            if (!initialized)
                return null;
            DateTime t0 = DateTime.UtcNow;
            bool success = cam.GrabFrame();
            DateTime t1 = DateTime.UtcNow;
            TimeSpan dt = t1.Subtract(t0);
            if (!success)
                return null;
            if (imgCount == cam.ImgCount)
                return null;
            imgCount = cam.ImgCount;

            DateTime t = cam.GetLastFrameTime(); // time at end of frame
            utctime = t.AddMilliseconds(-cam.GetExpTime() * 0.5); // sets time at middle of exposure
            utctime = t;
            ushort[] rawData = (ushort[])cam.RawData; 
            if (rawData == null)
                return null;

            int imgHeight = cam.GetImageHeight();
            int imgWidth = cam.GetImageWidth();
            ushort[][] img = new ushort[imgHeight][];

            // converts 1-D array to 2-D image map
            int k = 0;
            for (int i = 0; i < imgHeight; i++)
            {
                img[i] = new ushort[imgWidth];
                for (int j=0; j < imgWidth; j++)
                {
                    byte[] b = BitConverter.GetBytes(rawData[k]);
                    k++;
                    byte temp = b[0]; // switch big endian to little endian
                    b[0] = b[1];
                    b[1] = temp;
                    ushort c = BitConverter.ToUInt16(b, 0);
                    img[i][j] = (ushort)(c >> 4); // after converting to little endian bitshift right by 4 because storage is 12-bit
                }
            }

            PgmImage pgm = new PgmImage(imgWidth, imgHeight, 4095, img); // 4095 corresponds with 12-bit max value
            DateTime t2 = DateTime.UtcNow;
            dt = t2.Subtract(t0);
            return pgm;
        }

        public void CloseINovaCam() // Close the camera.
        {
            if (initialized)
                cam.CloseCamera();
        }
    }
}
