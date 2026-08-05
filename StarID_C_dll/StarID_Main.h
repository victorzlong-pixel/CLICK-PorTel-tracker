#ifndef __STARID_MAIN
#define __STARID_MAIN

#include "Structures.h"

void InitializeStarID(char SatID[]);
int PerformStarIDC(char fn[], int Threshold, int MinPixelN, int MaxPixelN,
	int MaxMagStarN, int MaxIMGStarN, double Star_Std_arcsec,
	double TwoStarLength_Std_deg, int rIDSuc[], int rID[], double rScore[], double rU[], double rV[], int rMag[], int rPixelN[], double rQest[]);

int PerformStarIDCWithPrior(char fn[], double q_est[], double MaxAngleErr_deg, int Threshold, int MinPixelN, int MaxPixelN,
	int MaxMagStarN, int MaxIMGStarN, double Star_Std_arcsec,
	double TwoStarLength_Std_deg, int rIDSuc[], int rID[], double rScore[], double rU[], double rV[], int rMag[], int rPixelN[], double rQest[]);


#endif