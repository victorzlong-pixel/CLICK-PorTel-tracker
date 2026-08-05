using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MathNet.Numerics.LinearAlgebra.Double;

namespace GS_Tracking_KR
{
    public class StarCatalog
    {
        public OneStarData[] masterCatStars; // stars in master star catalog
        public OneStarData[] redCatStars; // stars in reduced star catalog
        private const int nStars = 299453; // number of stars in catalog
        //private double RefYear_unixtime = 946684800;

        public class OneStarData
        {
            public uint index = 0; // index in master star catalog
            public uint ID_SKYMAP2000 = 0; // master star catalog ID
            public double RA = 0; // right ascension in radians
            public double DEC = 0; // declination in radians
            public double RA_prop = 0;
            public double DEC_prop = 0;
            public double[] Mag = new double[3]; // [0]: Observed, [1]: Derived, [2]: Pass3
            public DenseVector XYZj2k = new DenseVector(3);
            public double magRef = 0;
        }

        public StarCatalog()
        {
            // Read master star catalog.
            string fn = "SKY2000V5_299453.scb";
            FileInfo info = new FileInfo(fn);
            if (info.Exists == false)
                return;
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            ReadMasterCatData(br);
            br.Close();
            CalcXYZ(17); // get celestial coordinates of stars in 2017
            ChooseRefMag();

            // Read reduced star catalog.
            fn = "StarCat.bin";
            info = new FileInfo(fn);
            if (info.Exists == false)
                return;
            fs = new FileStream(fn, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs);
            ReadReducedCatData(br);
            br.Close();
        }

        private void ReadMasterCatData(BinaryReader br) // Reads data from SKYMAP2000 star catalog.
        {
            masterCatStars = new OneStarData[nStars];
            for (int i = 0; i < nStars; i++)
            {
                masterCatStars[i] = new OneStarData();
                masterCatStars[i].index = br.ReadUInt32();
                masterCatStars[i].ID_SKYMAP2000 = br.ReadUInt32();
                masterCatStars[i].RA = br.ReadDouble();
                masterCatStars[i].DEC = br.ReadDouble();
                masterCatStars[i].RA_prop = br.ReadDouble();
                masterCatStars[i].DEC_prop = br.ReadDouble();
                for (int j = 0; j < 3; j++)
                    masterCatStars[i].Mag[j] = br.ReadDouble();
            }
        }

        private void CalcXYZ(double year) // Converts RA, DEC to the location of the star in celestial coordinates.
        {
            double RA, DEC;
            double sr, cr, sd, cd;
            //foreach (OneStarData star in masterCatStars)
            for (int i = 0; i < masterCatStars.Length; i++)
            {
                DenseVector XYZ = new DenseVector(3);
                OneStarData star = masterCatStars[i];
                RA = star.RA + star.RA_prop * year; // propagate RA, DEC forward 
                DEC = star.DEC + star.DEC_prop * year;
                sr = Math.Sin(RA);
                cr = Math.Cos(RA);
                sd = Math.Sin(DEC);
                cd = Math.Cos(DEC);

                XYZ[0] = cr * cd;
                XYZ[1] = sr * cd;
                XYZ[2] = sd;
                masterCatStars[i].XYZj2k = XYZ;
            }
        }

        private void ChooseRefMag()
        {
            foreach (OneStarData star in masterCatStars)
            {
                if (star.Mag[2] != 0)
                    star.magRef = star.Mag[2];
                else if (star.Mag[0] != 0)
                    star.magRef = star.Mag[0];
                else star.magRef = star.Mag[1];
            }
        }

        private void ReadReducedCatData(BinaryReader br)
        {
            // Read header 
            int nStarsRed = br.ReadInt32();
            int pad = br.ReadInt32();
            double minMag = br.ReadDouble();
            double MinSepAng = br.ReadDouble(); // minimum separation angle in degrees

            // Write star data
            redCatStars = new OneStarData[nStarsRed];
            for (int i = 0; i < nStarsRed; i++)
            {
                redCatStars[i] = new OneStarData();
                redCatStars[i].index = br.ReadUInt32();
                redCatStars[i].ID_SKYMAP2000 = br.ReadUInt32();
                redCatStars[i].magRef = br.ReadDouble();
                for (int j = 0; j < 3; j++)
                    redCatStars[i].XYZj2k[j] = br.ReadDouble();
            }
        }
    }
}
