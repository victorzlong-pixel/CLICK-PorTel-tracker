#ifndef __StarID_3DCorrelation
#define __StarID_3DCorrelation

#include "Structures.h"

double StarID_3DCorrelation(IMGSTARVECTOR ImgStar[], int ImgStarN, STAR_ID_PAR* IDPar,
	int IDSuc[], int ImgStarID[], double ImgStarScore[]);

double StarID_3DCorrelation_WithPrior(IMGSTARVECTOR ImgStar[], int ImgStarN, double q_est[], double AngleErrBound_deg, STAR_ID_PAR* IDPar,
	int IDSuc[], int ImgStarID[], double ImgStarScore[]);


#endif