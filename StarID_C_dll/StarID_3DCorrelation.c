#include "StarID_3DCorrelation.h"

#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "HSYMath.h"
#include "GlobalVariables.h"

#define MaxImgStarN 200

static int ID_buf1[MaxImgStarN];
static int ID_buf2[MaxImgStarN];
static CANDIDATEIDSCORE CanID[(MaxImgStarN*(MaxImgStarN - 1)) / 2];
static int DoubleIDCheck[MAX_STAR_ID_N][2];

// Define
static int ImgPairCnt[MaxImgStarN];
static PAIR_DATA ImgPair[(MaxImgStarN*(MaxImgStarN-1))/2];
static double ImgPairAngle[(MaxImgStarN*(MaxImgStarN - 1))/2];
static int TableForSch[MaxImgStarN][MaxImgStarN];

static int SearchPairIndex(double ImgPairAngle, double PairAngle[], int PairN, double Length_Std, int PairIndex[]);
static int SearchPairIndex_KVec(double ImgPairAngle, double Length_Std, int PairIndex[]);

static double CalculateScore(int CenterStarID, IMGSTARVECTOR ImgStar[], int ImgStarN, double dcm[][3], double num, int ID_Candidate[]);

static void UpdateIDScores(double Score, int ImgStarN, int ID_Candidate[], CANDIDATEIDSCORE* CanID);
static double ChooseMaximumCost(CANDIDATEIDSCORE* CanID, int ImgStarN, int ImgStarID[], double ImgStarScore[], int* Maxi);
static void FilterIDSolution(int ImgStarN, IMGSTARVECTOR ImgStar[], int IDSuc[], int ImgStarID[], double ImgStarScore[], double MaxScore, int Maxi);
// Functions

double StarID_3DCorrelation(IMGSTARVECTOR ImgStar[], int ImgStarN, STAR_ID_PAR* IDPar,
	int IDSuc[], int ImgStarID[], double ImgStarScore[])
{
	int i, j, k, ImgPairN;
	
	double den, num;
	double MinTwoStarLength_rad;
	double MaxTwoStarLength_rad;
	int PairN = g_PairHdr.PairN;
	int StarN = g_SCHdr.StarN;
	int Star_ID_N;

	double Star_Std_rad, TwoStarLength_Std_rad;
	double MaxScore;
	int Maxi;

	Star_Std_rad = IDPar->Star_Std_rad;
	TwoStarLength_Std_rad = IDPar->TwoStarLength_Std_rad;


	memset(CanID, 0, sizeof(CANDIDATEIDSCORE) * ImgStarN);

	den = Star_Std_rad*Star_Std_rad * 4;
	num = 1.0 / den;

	MinTwoStarLength_rad = g_PairAngle[0] - TwoStarLength_Std_rad;
	MaxTwoStarLength_rad = g_PairAngle[PairN - 1] + TwoStarLength_Std_rad;

	// Make Img Pairs
	// 최대한 몇개의 중심별?
	Star_ID_N = (ImgStarN < IDPar->MaxIDStar_N) ? ImgStarN : IDPar->MaxIDStar_N;

	memset(ImgPairCnt, 0, sizeof(ImgPairCnt));
	memset(TableForSch, 0, sizeof(TableForSch));
	ImgPairN = 0;
	for (i = 0; i < Star_ID_N; i++)
	{
		TableForSch[i][i] = 1;
		for (j = 0; j < ImgStarN; j++)
		{
			if (TableForSch[i][j])
				continue;

			ImgPairAngle[ImgPairN] = vector_3x1_BtwAngle_Small(ImgStar[i].XYZ_body, ImgStar[j].XYZ_body);
			if (ImgPairAngle[ImgPairN] < MinTwoStarLength_rad)
				continue;
			if (ImgPairAngle[ImgPairN] > MaxTwoStarLength_rad)
				continue;

			ImgPair[ImgPairN].ID[0] = i;
			ImgPair[ImgPairN].ID[1] = j;
			TableForSch[i][j] = TableForSch[j][i] = 1;
			ImgPairN++;
			ImgPairCnt[i]++;
			if (ImgPairCnt[i] == IDPar->MaxIDforaStar_N)
				break;
		}
	}

	// Star ID
	for (i = 0; i < ImgPairN; i++)
	{
		int status;
		int PairIndex[2];
		double MaxScore, Score;
		int A_1, B_1, A_2, B_2;

		// Search Pair Index
		//status = SearchPairIndex(ImgPairAngle[i], g_PairAngle, PairN, TwoStarLength_Std_rad, PairIndex);
		status = SearchPairIndex_KVec(ImgPairAngle[i], TwoStarLength_Std_rad, PairIndex);
		if (status)
			continue;

		// Find Maximum Score Pattern for the reference pair
		MaxScore = 0.0;
		for (j = PairIndex[0]; j <= PairIndex[1]; j++)
		{
			A_1 = ImgPair[i].ID[0];
			B_1 = ImgPair[i].ID[1];
			A_2 = g_PairID[j].ID[0];
			B_2 = g_PairID[j].ID[1];

			for (k = 0; k < 2; k++)
			{
				double dcm[3][3];

				triad_method(ImgStar[A_1].XYZ_body, ImgStar[B_1].XYZ_body, g_SC[A_2].XYZ, g_SC[B_2].XYZ, dcm);

				Score = CalculateScore(A_2, ImgStar, ImgStarN, dcm, num, ID_buf1);
				if (Score > MaxScore)
				{
					MaxScore = Score;
					memcpy(ID_buf2, ID_buf1, sizeof(int)*ImgStarN);
				}

				A_2 = g_PairID[j].ID[1];
				B_2 = g_PairID[j].ID[0];
			}
		}

		if (MaxScore == 0.0)
			continue;

		// Update Scores
		UpdateIDScores(MaxScore, ImgStarN, ID_buf2, CanID);
	}

	// Fine the Maximum Cost Function
	MaxScore = ChooseMaximumCost(CanID, ImgStarN, ImgStarID, ImgStarScore, &Maxi);

	// Filter the Solution
	FilterIDSolution(ImgStarN, ImgStar, IDSuc, ImgStarID, ImgStarScore, MaxScore, Maxi);
	
	return MaxScore;

}

static int SearchPairIndex(double ImgPairAngle, double PairAngle[], int PairN, double Length_Std, int PairIndex[])
{
	int indexbuf[2];
	double dVal;

	dVal = ImgPairAngle - Length_Std;
	if(dVal < PairAngle[0])
	{
		PairIndex[0] = 0;
	}
	else
	{
		binary_search(dVal, PairAngle, PairN, indexbuf);
		PairIndex[0] = indexbuf[0];
	}
		
	dVal = ImgPairAngle + Length_Std;
	if(dVal > PairAngle[PairN-1])
	{
		PairIndex[1] = PairN-1;
	}
	else
	{
		binary_search(dVal, PairAngle, PairN, indexbuf);
		PairIndex[1] = indexbuf[1];
	}

	if(PairIndex[0] >= PairIndex[1])
		return -1;
	
	return 0;
}

static int SearchPairIndex_KVec(double ImgPairAngle, double Length_Std, int PairIndex[])
{
	int j_b, j_t;
	int k_start, k_end;
	int i;
	int MaxIdx = g_KHdr.PairN - 1;

	double Data_start = ImgPairAngle - Length_Std;
	double Data_end = ImgPairAngle + Length_Std;

	//if (Data_end < g_PairAngle[0])
	//	return -1;
	//if (Data_start > g_PairAngle[MaxIdx])
	//	return -1;

	j_b = (int)((Data_start - g_KHdr.q) / g_KHdr.m);
	j_b = j_b < 0 ? 0 : j_b;
	j_b = j_b > MaxIdx ? MaxIdx : j_b;

	j_t = (int)ceil((double)(Data_end - g_KHdr.q) / g_KHdr.m);
	j_t = j_t < 0 ? 0 : j_t;
	j_t = j_t > MaxIdx ? MaxIdx : j_t;

	k_start = g_KVec[j_b] + 1;
	k_end = g_KVec[j_t];

	PairIndex[0] = -1;
	for (i = k_start; i < MaxIdx; i++)
	{
		if (Data_start < g_PairAngle[i])
		{
			PairIndex[0] = i;
			break;
		}
	}
	PairIndex[1] = -1;
	for (i = k_end; i > 0; i--)
	{
		if (Data_end > g_PairAngle[i])
		{
			PairIndex[1] = i;
			break;
		}
	}

	if (PairIndex[0] >= PairIndex[1])
		return -1;

	return 0;
}

static double CalculateScore(int CenterStarID, IMGSTARVECTOR ImgStar[], int ImgStarN, double dcm[][3], double num, int ID_Candidate[])
{
	double vec[3];
	double r_min, r[3], r_2, Score, R;
	int i, j, ID_min;
	
	Score = 0.0;
	for (i = 0; i < ImgStarN; i++)
	{
		// Convert Img Star Vector from body frame to J2000 frame
		matrix_3x3_multiply_vector_3x1(dcm, ImgStar[i].XYZ_body, vec);

		// Find the Closest Star
		r_min = 3.0*3.0;
		for (j = 0; j < g_FoiPtr[CenterStarID].N; j++)
		{
			int idx = g_FoiID[j + g_FoiPtr[CenterStarID].i];
			vector_3x1_minus(vec, g_SC[idx].XYZ, r);
			r_2 = r[0] * r[0] + r[1] * r[1] + r[2] * r[2];

			if (r_2 < r_min)
			{
				r_min = r_2;
				ID_min = idx;
			}
		}

		R = r_min * num;
		if (R < 5.0)
		{
			Score += exp(-R);
			ID_Candidate[i] = ID_min;
		}
		else
		{
			ID_Candidate[i] = -1;
		}
	}

	return Score;
}

static void UpdateIDScores(double Score, int ImgStarN, int ID_Candidate[], CANDIDATEIDSCORE* CanID)
{
	int i, j, flag;

	for(i = 0; i < ImgStarN; i++)
	{
		if(ID_Candidate[i] < 0)
			continue;

		flag = 1;
		for(j = 0; j < CanID[i].cnt; j++)
		{
			if(CanID[i].ID[j] == ID_Candidate[i])
			{
				flag = 0;
				CanID[i].Score[j] += Score;
				break;
			}
		}

		if(flag)
		{
			CanID[i].ID[j] = ID_Candidate[i];
			CanID[i].Score[j] = Score;
			CanID[i].cnt++;
		}
	}

	return;
}

static double ChooseMaximumCost(CANDIDATEIDSCORE* CanID, int ImgStarN, int ImgStarID[], double ImgStarScore[], int* Maxi)
{
	int i, j;
	double MaxScore = 0.0;
	*Maxi = -1;

	for(i = 0; i < ImgStarN; i++)
	{
		ImgStarScore[i] = 0.0;
		ImgStarID[i] = -1;
		for(j = 0; j < CanID[i].cnt; j++)
		{
			if(ImgStarScore[i] < CanID[i].Score[j])
			{
				ImgStarScore[i] = CanID[i].Score[j];
				ImgStarID[i] = CanID[i].ID[j];
			}
		}
		if (MaxScore < ImgStarScore[i])
		{
			MaxScore = ImgStarScore[i];
			*Maxi = i;
		}
	}

	return MaxScore;
}

static void FilterIDSolution(int ImgStarN, IMGSTARVECTOR ImgStar[], int IDSuc[], int ImgStarID[], double ImgStarScore[], double MaxScore, int Maxi)
{
	int i, j, n;
	double ScoreThreshold = MaxScore * 0.3;
	double AngleThreshold = 1.8 * d2r;
	double r2_threshold = AngleThreshold * AngleThreshold;
	double angle1, angle2, anglediff;

	memset(DoubleIDCheck, 0, sizeof(DoubleIDCheck));
	memset(IDSuc, 0, sizeof(int) * ImgStarN);
	if (MaxScore == 0.0)
		return;

	IDSuc[Maxi] = 1;
	DoubleIDCheck[0][0] = ImgStarID[Maxi];
	DoubleIDCheck[0][1] = Maxi;
	n = 1;
	for (i = 0; i < ImgStarN; i++)
	{
		if (ImgStarID[i] < 0)
		{
			IDSuc[i] = -1;
			continue;
		}

		if (ImgStarScore[i] < ScoreThreshold)
		{
			IDSuc[i] = -2;
			continue;
		}

		if (i == Maxi)
			continue;

		angle1 = acos(vector_3x1_dot(g_SC[ImgStarID[Maxi]].XYZ, g_SC[ImgStarID[i]].XYZ));
		angle2 = acos(vector_3x1_dot(ImgStar[Maxi].XYZ_body, ImgStar[i].XYZ_body));
		anglediff = angle1 - angle2;
		anglediff = (anglediff > 0) ? anglediff : -anglediff;
		if (anglediff > AngleThreshold)
		{
			IDSuc[i] = -3;
			continue;
		}

		for (j = 0; j < n; j++)	// Double ID check
		{
			if (DoubleIDCheck[j][0] == ImgStarID[i])
			{
				IDSuc[i] = -4;
				IDSuc[DoubleIDCheck[j][1]] = -4;
				break;
			}
		}
		if (IDSuc[i] == -4)
			continue;
		DoubleIDCheck[n][0] = ImgStarID[i];
		DoubleIDCheck[n][1] = i;
		n++;

		IDSuc[i] = 1;
	}
}

static int CheckAttitude(IMGSTARVECTOR ImgStar[], int ImgID[], int PairID[], double dcm_est[][3], double maxangle2, double dcm[][3])
{
	int A_1, B_1, A_2, B_2, k;
	double d[3][3], angle2;

	A_1 = ImgID[0];
	B_1 = ImgID[1];
	A_2 = PairID[0];
	B_2 = PairID[1];

	for (k = 0; k < 2; k++)
	{
		triad_method(ImgStar[A_1].XYZ_body, ImgStar[B_1].XYZ_body, g_SC[A_2].XYZ, g_SC[B_2].XYZ, dcm);
		matrix_3x3_product(dcm, dcm_est, d);
		angle2 = d[0][1] * d[0][1] + d[0][2] * d[0][2] + d[1][2] * d[1][2];

		if (angle2 < maxangle2)
			if ((d[0][0] >0) && (d[1][1] >0) && (d[2][2] >0))
				return A_2;

		A_2 = PairID[1];
		B_2 = PairID[0];
	}

	return -1;
}

double StarID_3DCorrelation_WithPrior(IMGSTARVECTOR ImgStar[], int ImgStarN, double q_est[], double AngleErrBound_deg, STAR_ID_PAR* IDPar,
	int IDSuc[], int ImgStarID[], double ImgStarScore[])
{
	int i, j, ImgPairN;

	double den, num;
	double MinTwoStarLength_rad;
	double MaxTwoStarLength_rad;
	int PairN = g_PairHdr.PairN;
	int StarN = g_SCHdr.StarN;
	int Star_ID_N;

	double Star_Std_rad, TwoStarLength_Std_rad;
	double MaxScore;
	int Maxi;

	double dcm_est[3][3], dcm[3][3], maxangle2;

	quaternion_to_DCM(q_est, dcm_est);
	maxangle2 = AngleErrBound_deg * d2r;
	maxangle2 = maxangle2*maxangle2;

	Star_Std_rad = IDPar->Star_Std_rad;
	TwoStarLength_Std_rad = IDPar->TwoStarLength_Std_rad;


	memset(CanID, 0, sizeof(CANDIDATEIDSCORE) * ImgStarN);

	den = Star_Std_rad*Star_Std_rad * 4;
	num = 1.0 / den;

	MinTwoStarLength_rad = g_PairAngle[0] - TwoStarLength_Std_rad;
	MaxTwoStarLength_rad = g_PairAngle[PairN - 1] + TwoStarLength_Std_rad;

	// Make Img Pairs
	// 최대한 몇개의 중심별?
	Star_ID_N = (ImgStarN < IDPar->MaxIDStar_N) ? ImgStarN : IDPar->MaxIDStar_N;

	memset(ImgPairCnt, 0, sizeof(ImgPairCnt));
	memset(TableForSch, 0, sizeof(TableForSch));
	ImgPairN = 0;
	for (i = 0; i < Star_ID_N; i++)
	{
		TableForSch[i][i] = 1;
		for (j = 0; j < ImgStarN; j++)
		{
			if (TableForSch[i][j])
				continue;

			ImgPairAngle[ImgPairN] = vector_3x1_BtwAngle_Small(ImgStar[i].XYZ_body, ImgStar[j].XYZ_body);
			if (ImgPairAngle[ImgPairN] < MinTwoStarLength_rad)
				continue;
			if (ImgPairAngle[ImgPairN] > MaxTwoStarLength_rad)
				continue;

			ImgPair[ImgPairN].ID[0] = i;
			ImgPair[ImgPairN].ID[1] = j;
			TableForSch[i][j] = TableForSch[j][i] = 1;
			ImgPairN++;
			ImgPairCnt[i]++;
			if (ImgPairCnt[i] == IDPar->MaxIDforaStar_N)
				break;
		}
	}

	// Star ID
	for (i = 0; i < ImgPairN; i++)
	{
		int status;
		int PairIndex[2];
		double MaxScore, Score;
		int A_2;

		// Search Pair Index
		//status = SearchPairIndex(ImgPairAngle[i], g_PairAngle, PairN, TwoStarLength_Std_rad, PairIndex);
		status = SearchPairIndex_KVec(ImgPairAngle[i], TwoStarLength_Std_rad, PairIndex);
		if (status)
			continue;

		// Find Maximum Score Pattern for the reference pair
		MaxScore = 0.0;
		for (j = PairIndex[0]; j <= PairIndex[1]; j++)
		{

			A_2 = CheckAttitude(ImgStar, ImgPair[i].ID, g_PairID[j].ID, dcm_est, maxangle2, dcm);
			if (A_2 < 0)
				continue;
			
			Score = CalculateScore(A_2, ImgStar, ImgStarN, dcm, num, ID_buf1);
			if (Score > MaxScore)
			{
				MaxScore = Score;
				memcpy(ID_buf2, ID_buf1, sizeof(int)*ImgStarN);
			}

		}

		if (MaxScore == 0.0)
			continue;

		// Update Scores
		UpdateIDScores(MaxScore, ImgStarN, ID_buf2, CanID);
	}

	// Fine the Maximum Cost Function
	MaxScore = ChooseMaximumCost(CanID, ImgStarN, ImgStarID, ImgStarScore, &Maxi);

	// Filter the Solution
	FilterIDSolution(ImgStarN, ImgStar, IDSuc, ImgStarID, ImgStarScore, MaxScore, Maxi);

	return MaxScore;
}