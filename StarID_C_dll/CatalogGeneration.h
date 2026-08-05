#ifndef __CatalogGeneration
#define __CatalogGeneration

#include "Structures.h"

void ReadStarCat(char fn[], STAR_HDR* hdr, STAR_DATA sc[]);
void ReadPairCat(char fn[], PAIR_HDR* hdr, double Angle[], PAIR_DATA ID[]);
void ReadFoiCat(char fn[], FOI_HDR* hdr, FOI_PTR ptr[], int FoiID[]);
void ReadKVectorCat(char fn[], K_HDR* hdr, int KVec[]);
void ReadCalPar(char fn[], double cal[][10]);

#endif