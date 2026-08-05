#ifndef __GlobalHeader
#define __GlobalHeader

#define STAR_N_CATALOG 100000
#define PAIR_BUF_SIZE 2000000
#define FOI_BUF_SIZE 2000000
#define WIDTH 1280
#define HEIGHT 960
#define HALFWIDTH (WIDTH/2)
#define HALFHEIGHT (HEIGHT/2)
#define DEADPIXEL_W 2
#define DEADPIXEL_H 3
#define MAXSTARN 1000
#define MAXSTARNFORID 30
//#define PIXELSIZE (3.75e-6) // pixel size (meter)
//#define FOCALLENGTH (16.0e-3) // focal length (meter)
#define MAX_STAR_ID_N 30

#define X_FFT 1024
#define Y_FFT 2048
#define CUTOFF_N 90

typedef struct
{
	int StarN;
	int _pad;
	double MinMag;
	double MinSepAngle_deg;
}STAR_HDR;

typedef struct
{
	int Index;
	int ID_SKYMAP2000;
	double Mag;
	double XYZ[3];

}STAR_DATA;

typedef struct
{
	int PairN;
	int StarN;
	double MinMag;
	double MinAngle_deg;
	double MaxAngle_deg;
}PAIR_HDR;

typedef struct
{
	int ID[2];
}PAIR_DATA;

typedef struct
{
	int MaxFOIN;
	int StarN;
	double MinMag;
	double FOI_deg;
	double ThresholdMag;
	int FOI_TotalData_N;
	int _pad;

}FOI_HDR;

typedef struct
{
	// Write the start position and count for each foi information
	int i;
	int N;
}FOI_PTR;

typedef struct
{
	int PairN;
	int StarN;
	double MinMag;
	double m;
	double q;
}K_HDR;

typedef struct
{
	double XYZ_body[3];
}IMGSTARVECTOR;

typedef struct
{
	int ID[100];
	double Score[100];
	int cnt;
}CANDIDATEIDSCORE;

typedef struct
{
	double Star_Std_rad;
	double TwoStarLength_Std_rad;
	int MaxIDStar_N;		// 최대 몇개의 별에 대해서 ID를 수행할 것인지
	int MaxIDforaStar_N;		// 한개의 별에 대해서 최대 몇번의 ID를 수행할 것인지
}STAR_ID_PAR;

typedef struct
{
	int U;
	int V;
	int Mag;
	int ID;
}ONEPIXEL;

typedef struct
{
	ONEPIXEL PXL[HEIGHT * WIDTH];
	int N;	// # of pixels
}STARPIXELS;

typedef struct
{
	double U;
	double V;
	int MagSum;
	int PixelN;
	int MaxMag;
	int ID_result;
	int ID_suc;
	int _pad;
	double Score;
}ONESTAR;

typedef struct
{
	ONESTAR S[MAXSTARN];
	int N;

}STARS;

typedef struct
{
	IMGSTARVECTOR V[MAXSTARNFORID];
	double Score[MAXSTARNFORID];
	int ID[MAXSTARNFORID];	// ID results
	int IDsuc[MAXSTARNFORID];	// ID success
	int idx[MAXSTARNFORID];	// Index in STARS
	int N;
}STARSFORID;

#endif