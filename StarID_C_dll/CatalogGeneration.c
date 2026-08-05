#include "CatalogGeneration.h"
#include "HSYMath.h"
#include <string.h>
#include <math.h>
#include <stdlib.h>
#include <stdio.h>


void ReadStarCat(char fn[], STAR_HDR* hdr, STAR_DATA sc[])
{
	FILE* fp;
	fp = fopen(fn, "rb");

	fread(hdr, sizeof(STAR_HDR), 1, fp);
	fread(sc, sizeof(STAR_DATA), hdr->StarN, fp);
	fclose(fp);
}

void ReadPairCat(char fn[], PAIR_HDR* hdr, double Angle[], PAIR_DATA ID[])
{
	FILE* fp;
	fp = fopen(fn, "rb");

	fread(hdr, sizeof(PAIR_HDR), 1, fp);
	fread(Angle, sizeof(double), hdr->PairN, fp);
	fread(ID, sizeof(PAIR_DATA), hdr->PairN, fp);
	fclose(fp);
}

void ReadFoiCat(char fn[], FOI_HDR* hdr, FOI_PTR ptr[], int FoiID[])
{
	FILE* fp;
	fp = fopen(fn, "rb");

	fread(hdr, sizeof(FOI_HDR), 1, fp);
	fread(ptr, sizeof(FOI_PTR), hdr->StarN, fp);
	fread(FoiID, sizeof(int), hdr->FOI_TotalData_N, fp);
	fclose(fp);
}

void ReadKVectorCat(char fn[], K_HDR* hdr, int KVec[])
{
	FILE* fp;
	fp = fopen(fn, "rb");

	fread(hdr, sizeof(K_HDR), 1, fp);
	fread(KVec, sizeof(int), hdr->PairN, fp);
	fclose(fp);
}

void ReadCalPar(char fn[], double cal[][10])
{
	FILE* fp;
	fp = fopen(fn, "rb");
	if (fp == NULL)
	{
		memset(cal, 0, sizeof(double) * 20);
		cal[0][1] = cal[1][4] = 1;
		return;
	}
	fread(cal, 8, 20, fp);
	fclose(fp);
	return;
}