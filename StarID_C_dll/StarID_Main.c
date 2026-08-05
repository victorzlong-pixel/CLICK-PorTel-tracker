#include "StarID_Main.h"
#include <string.h>
#include <stdlib.h>
#include <stdio.h>

#include "GlobalVariables.h"
#include "HSYMath.h"
#include "StarID_3DCorrelation.h"
#include "ImgProcessing.h"
#include "CatalogGeneration.h"

static double s_V_body_buf[MAX_STAR_ID_N][3];
static double s_V_eci_buf[MAX_STAR_ID_N][3];
static double s_w[MAX_STAR_ID_N];


void InitializeStarID(char SatID[])
{
	char calfn[100];

	// Prepare Catalogs
	ReadStarCat("StarCat.bin", &g_SCHdr, g_SC);
	ReadPairCat("PairCat.bin", &g_PairHdr, g_PairAngle, g_PairID);
	ReadFoiCat("FOICat.bin", &g_FoiHdr, g_FoiPtr, g_FoiID);
	ReadKVectorCat("KVectorCat.bin", &g_KHdr, g_KVec);
	sprintf(calfn, "CalData%s.bin", SatID);
	ReadCalPar(calfn, g_CalPar);

	// Prepare High-Pass Filtering
	MakeFilter(g_Filter);
	gen_w_r2(&g_wx[0][0], X_FFT * 2);
	gen_w_r2(&g_wy[0][0], Y_FFT * 2);

	bit_rev(&g_wx[0][0], X_FFT);
	bit_rev(&g_wy[0][0], Y_FFT);
}


int PerformStarIDC(char fn[], int Threshold, int MinPixelN, int MaxPixelN,
	int MaxMagStarN, int MaxIMGStarN, double Star_Std_arcsec,
	double TwoStarLength_Std_deg, int rIDSuc[], int rID[], double rScore[], double rU[], double rV[], int rMag[], int rPixelN[], double rQest[])
{
	STAR_ID_PAR par;
	int StarN = 0, i, j, StarIDN;
	double MaxScore;

	// Image processing first
	// 1. Load IMG File
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
	par.MaxIDStar_N = 12;
	par.MaxIDforaStar_N = 2;
	par.Star_Std_rad = Star_Std_arcsec / 3600.0 * d2r;
	par.TwoStarLength_Std_rad = TwoStarLength_Std_deg * d2r;

	MaxScore = StarID_3DCorrelation(g_SelStar.V, g_SelStar.N, &par, g_SelStar.IDsuc, g_SelStar.ID, g_SelStar.Score);
	//if (MaxScore < 10)
	//{
	//	for (i = 0; i < g_SelStar.N; i++)
	//	{
	//		g_SelStar.IDsuc[i] = g_SelStar.IDsuc[i] == 1 ? -9 : g_SelStar.IDsuc[i];
	//	}
	//}

	// Result output
	StarIDN = 0;
	for (i = 0; i < g_SelStar.N; i++)
	{
		j = g_SelStar.idx[i];
		g_Star.S[j].ID_result = g_SelStar.ID[i];
		g_Star.S[j].Score = g_SelStar.Score[i];
		g_Star.S[j].ID_suc = g_SelStar.IDsuc[i];

		rIDSuc[i] = g_Star.S[j].ID_suc;
		rID[i] = g_Star.S[j].ID_result;
		rU[i] = g_Star.S[j].U;
		rV[i] = g_Star.S[j].V;
		rMag[i] = g_Star.S[j].MagSum;
		rPixelN[i] = g_Star.S[j].PixelN;
		rScore[i] = g_Star.S[j].Score;

		if (rIDSuc[i] != 1)
			continue;

		memcpy(s_V_body_buf[StarIDN], g_SelStar.V[i].XYZ_body, 24);
		memcpy(s_V_eci_buf[StarIDN], g_SC[rID[i]].XYZ, 24);
		s_w[StarIDN] = 1;
		StarIDN++;
	}

	calAtt(StarIDN, s_V_body_buf, s_V_eci_buf, s_w, rQest);

	return g_SelStar.N;
}






int PerformStarIDCWithPrior(char fn[], double q_est[], double MaxAngleErr_deg, int Threshold, int MinPixelN, int MaxPixelN,
	int MaxMagStarN, int MaxIMGStarN, double Star_Std_arcsec,
	double TwoStarLength_Std_deg, int rIDSuc[], int rID[], double rScore[], double rU[], double rV[], int rMag[], int rPixelN[], double rQest[])
{
	STAR_ID_PAR par;
	int StarN = 0, i, j, StarIDN;
	double MaxScore;

	// Image processing first
	// 1. Load IMG File
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
	par.MaxIDStar_N = 12;
	par.MaxIDforaStar_N = 2;
	par.Star_Std_rad = Star_Std_arcsec / 3600.0 * d2r;
	par.TwoStarLength_Std_rad = TwoStarLength_Std_deg * d2r;

	MaxScore = StarID_3DCorrelation_WithPrior(g_SelStar.V, g_SelStar.N, q_est, MaxAngleErr_deg, &par, g_SelStar.IDsuc, g_SelStar.ID, g_SelStar.Score);
	if (MaxScore < 10)
	{
		for (i = 0; i < g_SelStar.N; i++)
		{
			g_SelStar.IDsuc[i] = g_SelStar.IDsuc[i] == 1 ? -9 : g_SelStar.IDsuc[i];
		}
	}

	// Result output
	StarIDN = 0;
	for (i = 0; i < g_SelStar.N; i++)
	{
		j = g_SelStar.idx[i];
		g_Star.S[j].ID_result = g_SelStar.ID[i];
		g_Star.S[j].Score = g_SelStar.Score[i];
		g_Star.S[j].ID_suc = g_SelStar.IDsuc[i];

		rIDSuc[i] = g_Star.S[j].ID_suc;
		rID[i] = g_Star.S[j].ID_result;
		rU[i] = g_Star.S[j].U;
		rV[i] = g_Star.S[j].V;
		rMag[i] = g_Star.S[j].MagSum;
		rPixelN[i] = g_Star.S[j].PixelN;
		rScore[i] = g_Star.S[j].Score;

		if (rIDSuc[i] != 1)
			continue;

		memcpy(s_V_body_buf[StarIDN], g_SelStar.V[i].XYZ_body, 24);
		memcpy(s_V_eci_buf[StarIDN], g_SC[rID[i]].XYZ, 24);
		s_w[StarIDN] = 1;
		StarIDN++;
	}

	calAtt(StarIDN, s_V_body_buf, s_V_eci_buf, s_w, rQest);

	return g_SelStar.N;
}
