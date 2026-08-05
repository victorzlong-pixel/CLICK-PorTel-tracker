#include "TestStarGen.h"
#include "Structures.h"
#include "HSYMath.h"

#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

int GenTestIMG(STAR_DATA _sd[], int _sdN, double _fov_deg, IMGSTARVECTOR _sv[], unsigned int _sid[], double q[], double _noise_arcsec)
{
	int i, j, n;
	double fov, r_max, r_max_2, r[3], r_2, vec[3];
	double dcm[3][3], dcm_b2j[3][3], dcm_n[3][3];
	double qn[4], los[3];
	double z[3] = {0,0,1};
	double noise_rad;

	// FOV
	fov = _fov_deg * d2r;
	r_max = 2 * sin(fov / 4.0);
	r_max_2 = r_max * r_max;

	quaternion_normalize(q, qn);
	quaternion_to_DCM(qn, dcm);
	matrix_3x3_transpose(dcm, dcm_b2j);
	matrix_3x3_multiply_vector_3x1(dcm_b2j, z, los);
	
	// Gen Star IMG with noise
	noise_rad = _noise_arcsec / 3600.0 * d2r;
	n = 0;
	for(i = 0; i < _sdN; i++)
	{
		vector_3x1_minus(los, _sd[i].XYZ, r);
		r_2 = r[0]*r[0] + r[1]*r[1] + r[2]*r[2];
		if(r_2 > r_max_2)
			continue;

		_sid[n] = i;

		for(j = 0; j < 3; j++)
			qn[j] = 0.5 * randn() * noise_rad;
		qn[3] = 1.0;
		quaternion_normalize(qn, qn);
		quaternion_to_DCM(qn, dcm_n);

		matrix_3x3_multiply_vector_3x1(dcm, _sd[i].XYZ, vec);
		matrix_3x3_multiply_vector_3x1(dcm_n, vec, _sv[n].XYZ_body);	// Add Noise

		n++;
	}

	return n;
}