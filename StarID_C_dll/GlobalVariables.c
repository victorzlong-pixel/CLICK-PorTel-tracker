#include "Structures.h"

STAR_HDR g_SCHdr;
STAR_DATA g_SC[STAR_N_CATALOG];

PAIR_HDR g_PairHdr;
double g_PairAngle[PAIR_BUF_SIZE];
PAIR_DATA g_PairID[PAIR_BUF_SIZE];

FOI_HDR g_FoiHdr;
FOI_PTR g_FoiPtr[FOI_BUF_SIZE];
int g_FoiID[FOI_BUF_SIZE];

K_HDR g_KHdr;
int g_KVec[PAIR_BUF_SIZE];

IMGSTARVECTOR g_ImgStarVec[1000];
int ImgStarID[1000];
int g_ID_True[1000];	// Just for the simulation

unsigned short g_PGM[HEIGHT][WIDTH];
STARPIXELS g_IMG;
int g_GroupIndex[HEIGHT * WIDTH];

STARS g_Star;
double g_CalPar[2][10];
STARSFORID g_SelStar;

float g_fIMG[X_FFT][Y_FFT][2];
float g_fIMG_buf[X_FFT][Y_FFT][2];
int g_Filter[X_FFT][Y_FFT];
float g_wx[X_FFT][2];
float g_wy[Y_FFT][2];

double PIXELSIZE = 3.75e-6; // pixel size (meter)
double FOCALLENGTH = 16.0e-3; // focal length (meter)