#include "Structures.h"

extern STAR_HDR g_SCHdr;
extern STAR_DATA g_SC[STAR_N_CATALOG];

extern PAIR_HDR g_PairHdr;
extern double g_PairAngle[PAIR_BUF_SIZE];
extern PAIR_DATA g_PairID[PAIR_BUF_SIZE];

extern FOI_HDR g_FoiHdr;
extern FOI_PTR g_FoiPtr[FOI_BUF_SIZE];
extern int g_FoiID[FOI_BUF_SIZE];

extern K_HDR g_KHdr;
extern int g_KVec[PAIR_BUF_SIZE];

extern IMGSTARVECTOR g_ImgStarVec[1000];
extern int ImgStarID[1000];
extern int g_ID_True[1000];	// Just for the simulation

extern unsigned short g_PGM[HEIGHT][WIDTH];
extern STARPIXELS g_IMG;

extern STARS g_Star;
extern double g_CalPar[2][10];
extern STARSFORID g_SelStar;

extern float g_fIMG[X_FFT][Y_FFT][2];
extern float g_fIMG_buf[X_FFT][Y_FFT][2];
extern int g_Filter[X_FFT][Y_FFT];
extern float g_wx[X_FFT][2];
extern float g_wy[Y_FFT][2];

extern double PIXELSIZE; // pixel size (meter)
extern double FOCALLENGTH; // focal length (meter)