using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace GS_Tracking_KR
{
    public class PgmImage // Class defining pgm image. Taken from internets.
    {
        public string fn;
        public int width;
        public int height;
        public int maxVal;
        public ushort[][] pixels;

        public PgmImage(int width, int height, int maxVal, ushort[][] pixels)
        {
            this.width = width;
            this.height = height;
            this.maxVal = maxVal;
            this.pixels = pixels;
        }

        public static PgmImage LoadImage_NotGeneral(string file)
        {
            FileStream ifs = new FileStream(file, FileMode.Open);
            BinaryReader br = new BinaryReader(ifs);

            string line = NextNonCommentLine(br);
            string[] tokens = line.Split(' ');

            int width = int.Parse(tokens[1]);
            int height = int.Parse(tokens[2]);
            int maxVal = int.Parse(tokens[3]);
            //listBox1.Items.Add("maxVal+ maxVal);

            // read width * height pixel values . . .
            ushort[][] pixels = new ushort[height][];
            for (int i = 0; i < height; ++i)
                pixels[i] = new ushort[width];

            byte[] raw = new byte[2];
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    raw[0] = br.ReadByte();
                    raw[1] = br.ReadByte();
                    pixels[i][j] = CvtUShort(raw, 0);
                }
            }

            br.Close(); ifs.Close();

            PgmImage result = new PgmImage(width, height, maxVal, pixels);
            //listBox1.Items.Add("imageed");
            result.fn = file;

            return result;
        }
        static string NextAnyLine(BinaryReader br)
        {
            string s = "";
            byte b = 0; // dummy
            while (b != 10) // newline
            {
                b = br.ReadByte();
                char c = (char)b;
                s += c;
            }
            return s.Trim();
        }

        static string NextNonCommentLine(BinaryReader br)
        {
            string s = NextAnyLine(br);
            while (s.StartsWith("#") || s == "")
                s = NextAnyLine(br);
            return s;
        }

        static ushort CvtUShort(byte[] raw, int starti)
        {
            byte[] d = new byte[2];
            for (int i = 0; i < 2; i++)
                d[i] = raw[starti + 1 - i];
            return BitConverter.ToUInt16(d, 0);
        }

        public void SaveAsFile()
        {
            FileStream ifs = new FileStream(fn + ".pgm", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(ifs);
            //string hdr = "P5 1280 960 4095\n";
            string hdr = String.Format("P5 {0} {1} {2}\n", width, height, maxVal);
            byte[] hdrb = ASCIIEncoding.ASCII.GetBytes(hdr);
            bw.Write(hdrb);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    byte[] b = new byte[2];
                    b[0] = (byte)((pixels[i][j] >> 8) & 0xFF);
                    b[1] = (byte)(pixels[i][j] & 0xFF);
                    bw.Write(b);
                }

            bw.Close();
            ifs.Close();
        }
    }
}
