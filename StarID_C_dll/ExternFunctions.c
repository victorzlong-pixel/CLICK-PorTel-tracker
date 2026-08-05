#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>
#include <time.h>

#include "Structures.h"
#include "HSYMath.h"
#include "StarID_3DCorrelation.h"
#include "CatalogGeneration.h"
#include "TestStarGen.h"
#include "GlobalVariables.h"
#include "ImgProcessing.h"
#include "StarID_Main.h"

extern _declspec(dllexport) void ExternInitializeStarID()
{
	InitializeStarID("Default");
}

extern _declspec(dllexport) void ExternPerformStarID(int ImgStarN, double ImgStarVec[], double Star_Std_arcsec, 
	double TwoStarLength_Std_deg, int IDSuc[], int ImgStarIndex_SKY2000Trimmed[], double ImgStarScore[], unsigned char msg[])
{
	int i;

	for(i = 0; i < ImgStarN; i++)
	{
		memcpy(g_ImgStarVec[i].XYZ_body, &ImgStarVec[i*3], sizeof(double)*3);
	}

	STAR_ID_PAR par;
	par.MaxIDStar_N = 12;
	par.MaxIDforaStar_N = 2;
	par.Star_Std_rad = Star_Std_arcsec / 3600.0 * d2r;
	par.TwoStarLength_Std_rad = TwoStarLength_Std_deg * d2r;

	StarID_3DCorrelation(g_ImgStarVec, ImgStarN, &par, IDSuc, ImgStarIndex_SKY2000Trimmed, ImgStarScore);

	msg[0] = 0;
}

extern _declspec(dllexport) void ExternPerformStarIDwithPrior(int ImgStarN, double ImgStarVec[], double Star_Std_arcsec,
	double TwoStarLength_Std_deg, double q_est[], double AngleErrBound_deg, int IDSuc[], int ImgStarIndex_SKY2000Trimmed[], double ImgStarScore[], unsigned char msg[])
{
	int i;

	for (i = 0; i < ImgStarN; i++)
	{
		memcpy(g_ImgStarVec[i].XYZ_body, &ImgStarVec[i * 3], sizeof(double) * 3);
	}

	STAR_ID_PAR par;
	par.MaxIDStar_N = 12;
	par.MaxIDforaStar_N = 2;
	par.Star_Std_rad = Star_Std_arcsec / 3600.0 * d2r;
	par.TwoStarLength_Std_rad = TwoStarLength_Std_deg * d2r;

	StarID_3DCorrelation_WithPrior(g_ImgStarVec, ImgStarN, q_est, AngleErrBound_deg, &par, IDSuc, ImgStarIndex_SKY2000Trimmed, ImgStarScore);

	msg[0] = 0;
}


extern _declspec(dllexport) void ExternSimulateID()
{
	int i;
	char buf[2000];
	double q[4];
	q[0] = 1;
	q[1] = 2;
	q[2] = 6;
	q[3] = -4;
	quaternion_normalize(q, q);

	// Input Image
	int ImgStarN = GenTestIMG(g_SC, g_SCHdr.StarN, 15, g_ImgStarVec, g_ID_True, q, 0);
	printf("Img Star N: %d\n", ImgStarN);

	// Star ID
	int* ImgIDSuc = (int*)malloc(sizeof(int) * ImgStarN);
	int* ImgStarID = (int*)malloc(sizeof(int) * ImgStarN);
	double* ImgStarScore = (double*)malloc(sizeof(double) * ImgStarN);

	STAR_ID_PAR par;
	par.MaxIDforaStar_N = 5;
	par.MaxIDStar_N = 5;
	par.Star_Std_rad = 150.0 / 3600.0 * d2r;
	par.TwoStarLength_Std_rad = 0.0001 * d2r;

	StarID_3DCorrelation(g_ImgStarVec, ImgStarN, &par, ImgIDSuc, ImgStarID, ImgStarScore);

	FILE* pf = fopen("SimResult.txt","w");

	// Print Result
	for (i = 0; i < ImgStarN; i++)
	{
		int Ok = 0;
		if ((int)g_ID_True[i] == ImgStarID[i])
		{
			Ok = 1;
		}
		sprintf(buf, "No: %d, ID_t: %d, ID_e: %d, Score: %f, Okay?: %d\n", i, g_ID_True[i], ImgStarID[i], ImgStarScore[i], Ok);
		//printf(buf);
		fputs(buf, pf);
	}

	fclose(pf);
	free(ImgIDSuc);
	free(ImgStarID);
	free(ImgStarScore);
}

extern _declspec(dllexport) void TestSomething()
{
	LoadImgFile("20140815.130348_0.pgm", g_PGM);
}

extern _declspec(dllexport) int ExternPerformStarIDC(unsigned char fn[], int ApplyFilter, double f_length, double pixelSize,
	unsigned char SatID[], int Threshold, int MinPixelN, int MaxPixelN,
	int MaxMagStarN, int MaxIMGStarN, double Star_Std_arcsec,
	double TwoStarLength_Std_deg, int rIDSuc[], int rID[], double rScore[], double rU[], double rV[], int rMag[], int rPixelN[], double rCalPar[])
{
	STAR_ID_PAR par;
	int StarN, i, j;
	char buf[100];
	double MaxScore;

	FOCALLENGTH = f_length;
	PIXELSIZE = pixelSize;

	sprintf(buf, "CalData%s.bin", SatID);
	ReadCalPar(buf, g_CalPar);

		// Image processing first
	// 1. Load IMG File
	if (ApplyFilter)
		LoadImgAndHighPassFilter(fn, g_PGM);
	else
		LoadImgFile(fn, g_PGM);
	// 2. Threshold IMG
	Thresholding(Threshold, g_PGM, &g_IMG);
	// 3. Grouping
	StarN = Grouping(MinPixelN, MaxPixelN, &g_IMG);
	if ((StarN < 3) || (StarN > MAXSTARN))
		return StarN;
	
	// 4. Centroiding
	Centroiding(StarN, &g_IMG, &g_Star);
	// 5. Select stars for Star ID algorithm
	SelectStars(&g_Star, &g_SelStar, MaxMagStarN, MaxIMGStarN);

		// Star Identification
	//par.MaxIDStar_N = 12;
	//par.MaxIDforaStar_N = 2;
	par.MaxIDStar_N = MaxIMGStarN;
	par.MaxIDforaStar_N = 2;
	par.Star_Std_rad = Star_Std_arcsec / 3600.0 * d2r;
	par.TwoStarLength_Std_rad = TwoStarLength_Std_deg * d2r;

	MaxScore = StarID_3DCorrelation(g_SelStar.V, g_SelStar.N, &par, g_SelStar.IDsuc, g_SelStar.ID, g_SelStar.Score);
	if (MaxScore < 5)
	{
		for (i = 0; i < g_SelStar.N; i++)
		{
			g_SelStar.IDsuc[i] = g_SelStar.IDsuc[i] == 1 ? -9 : g_SelStar.IDsuc[i];
		}
	}

		// Result output
	for (i = 0; i < g_SelStar.N; i++)
	{
		g_Star.S[g_SelStar.idx[i]].ID_result = g_SelStar.ID[i];
		g_Star.S[g_SelStar.idx[i]].Score = g_SelStar.Score[i];
		g_Star.S[g_SelStar.idx[i]].ID_suc = g_SelStar.IDsuc[i];
	}

	for (i = 0; i < StarN; i++)
	{
		rIDSuc[i] = g_Star.S[i].ID_suc;
		rID[i] = g_Star.S[i].ID_result;
		rU[i] = g_Star.S[i].U;
		rV[i] = g_Star.S[i].V;
		rMag[i] = g_Star.S[i].MagSum;
		rPixelN[i] = g_Star.S[i].PixelN;
		rScore[i] = g_Star.S[i].Score;
	}

	for (i = 0; i < 2; i++)
		for (j = 0; j < 10; j++)
			rCalPar[i * 10 + j] = g_CalPar[i][j];

	return StarN;
}
