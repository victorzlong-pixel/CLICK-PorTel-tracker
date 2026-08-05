#include "HSYMath.h"

#include <stdlib.h>
#include <stdio.h>
#include <math.h>

const double d2r = 3.141592653589793 / 180.0;
const double r2d = 180.0 / 3.141592653589793;

#define BUFSIZEFORMERGE 10000
static int buf_1[BUFSIZEFORMERGE];
static int buf_2[BUFSIZEFORMERGE];


double vector_3x1_BtwAngle_Small(double A[], double B[])
{
	// Return : Angle (rad)

	double a[3], b[3], c[3], sin_angle, angle;

	vector_3x1_normalize(A, a);
	vector_3x1_normalize(B, b);

	vector_3x1_cross(a, b, c);

	sin_angle = sqrt(c[0] * c[0] + c[1] * c[1] + c[2] * c[2]);
	angle = asin(sin_angle);

	return angle;
}

void triad_method(double * A_1, double *B_1, double *A_2, double *B_2, double(*DCM_1_to_2)[3])
{
	// V_2 = DCM * V_1 이 성립하는 DCM 구하기
	// 
	// A_1, B_1 : V_1
	// A_2, B_2 : V_2

	double l_1[3], m_1[3], n_1[3], M_1[3][3];
	double l_2[3], m_2[3], n_2[3], M_2[3][3];

	l_1[0] = A_1[0];  l_1[1] = A_1[1];  l_1[2] = A_1[2];
	vector_3x1_normalize(l_1, l_1);
	vector_3x1_cross(l_1, B_1, m_1);
	vector_3x1_normalize(m_1, m_1);
	vector_3x1_cross(l_1, m_1, n_1);
	vector_3x1_normalize(n_1, n_1);

	l_2[0] = A_2[0];  l_2[1] = A_2[1];  l_2[2] = A_2[2];
	vector_3x1_normalize(l_2, l_2);
	vector_3x1_cross(l_2, B_2, m_2);
	vector_3x1_normalize(m_2, m_2);
	vector_3x1_cross(l_2, m_2, n_2);
	vector_3x1_normalize(n_2, n_2);

	M_1[0][0] = l_1[0]; M_1[0][1] = m_1[0]; M_1[0][2] = n_1[0];
	M_1[1][0] = l_1[1]; M_1[1][1] = m_1[1]; M_1[1][2] = n_1[1];
	M_1[2][0] = l_1[2]; M_1[2][1] = m_1[2]; M_1[2][2] = n_1[2];

	M_2[0][0] = l_2[0]; M_2[0][1] = m_2[0]; M_2[0][2] = n_2[0];
	M_2[1][0] = l_2[1]; M_2[1][1] = m_2[1]; M_2[1][2] = n_2[1];
	M_2[2][0] = l_2[2]; M_2[2][1] = m_2[2]; M_2[2][2] = n_2[2];

	matrix_3x3_transpose_product(M_2, M_1, DCM_1_to_2);
}

int binary_search(double e, double * Data, int N, int * Index)
{
	// 작은 순서로 정렬되어있는 Data Set (Data) 에서 e 가 낄 위치 탐색 알고리즘.
	//
	// input : e : 대상값
	//         Data : 정렬되어있는 데이터 세트
	//		   N : Data 사이즈
	//
	// output : Index : e가 끼어있는 Data 의 번지. Data[ Index[0] ] < x < Data[ Index[1] ]
	//

	int index = 0;
	int index_low, index_high, index_diff;


	index_low = 0;
	index_high = N - 1;

	while (1)
	{
		index_diff = index_high - index_low;
		if (index_diff == 1)
		{
			Index[0] = index_low;
			Index[1] = index_high;
			break;
		}
		else if (index_diff < 1)
		{
			// 에러가 난 상황
			Index[0] = 0xFFFFFFFF;
			Index[1] = 0xFFFFFFFF;
			//printf(" ERROR in binary search, 1\n");
			return -1;
			break;
		}
		else
		{
			index = index_diff / 2 + index_low;
			if (e > Data[index])
			{
				index_low = index;
			}
			else if (e < Data[index])
			{
				index_high = index;
			}
			else if (e == Data[index])
			{
				Index[0] = index_low;
				Index[1] = index_high;
				break;
			}
			else
			{
				// 에러가 난 상황
				Index[0] = 0xFFFFFFFF;
				Index[1] = 0xFFFFFFFF;
				//printf(" ERROR in binary search, 2\n");
				return -2;
				break;
			}
		}
	}

	return 1;
}

void matrix_3x3_multiply_vector_3x1(double M[][3], double V[], double output[])
{
	output[0] = M[0][0] * V[0] + M[0][1] * V[1] + M[0][2] * V[2];
	output[1] = M[1][0] * V[0] + M[1][1] * V[1] + M[1][2] * V[2];
	output[2] = M[2][0] * V[0] + M[2][1] * V[1] + M[2][2] * V[2];

	return;
}


void vector_3x1_minus(double A[], double B[], double output[])
{
	output[0] = A[0] - B[0];
	output[1] = A[1] - B[1];
	output[2] = A[2] - B[2];
}

double vector_3x1_dot(double A[], double B[])
{
	return A[0] * B[0] + A[1] * B[1] + A[2] * B[2];
}

double quaternion_normalize(double q[], double output[])
{
	double norm;
	norm = sqrt(q[0] * q[0] + q[1] * q[1] + q[2] * q[2] + q[3] * q[3]);

	output[0] = q[0] / norm;
	output[1] = q[1] / norm;
	output[2] = q[2] / norm;
	output[3] = q[3] / norm;

	return norm;
}

double vector_3x1_normalize(double input[], double output[])
{
	double norm;
	norm = sqrt(input[0] * input[0] + input[1] * input[1] + input[2] * input[2]);
	output[0] = input[0] / norm; output[1] = input[1] / norm; output[2] = input[2] / norm;
	return norm;
}

void vector_3x1_cross(double A[], double B[], double output[])
{
	output[0] = A[1] * B[2] - A[2] * B[1];
	output[1] = A[2] * B[0] - A[0] * B[2];
	output[2] = A[0] * B[1] - A[1] * B[0];

	return;
}

void matrix_3x3_transpose(double A[][3], double output[][3])
{
	output[0][0] = A[0][0];	output[0][1] = A[1][0];	output[0][2] = A[2][0];
	output[1][0] = A[0][1];	output[1][1] = A[1][1];	output[1][2] = A[2][1];
	output[2][0] = A[0][2];	output[2][1] = A[1][2];	output[2][2] = A[2][2];
}

void matrix_3x3_product(double a[][3], double b[][3], double output[][3])
{
	output[0][0] = a[0][0] * b[0][0] + a[0][1] * b[1][0] + a[0][2] * b[2][0];
	output[0][1] = a[0][0] * b[0][1] + a[0][1] * b[1][1] + a[0][2] * b[2][1];
	output[0][2] = a[0][0] * b[0][2] + a[0][1] * b[1][2] + a[0][2] * b[2][2];

	output[1][0] = a[1][0] * b[0][0] + a[1][1] * b[1][0] + a[1][2] * b[2][0];
	output[1][1] = a[1][0] * b[0][1] + a[1][1] * b[1][1] + a[1][2] * b[2][1];
	output[1][2] = a[1][0] * b[0][2] + a[1][1] * b[1][2] + a[1][2] * b[2][2];

	output[2][0] = a[2][0] * b[0][0] + a[2][1] * b[1][0] + a[2][2] * b[2][0];
	output[2][1] = a[2][0] * b[0][1] + a[2][1] * b[1][1] + a[2][2] * b[2][1];
	output[2][2] = a[2][0] * b[0][2] + a[2][1] * b[1][2] + a[2][2] * b[2][2];

	return;
}

void matrix_3x3_transpose_product(double a[][3], double b[][3], double output[][3])
{
	// output = a * b'

	output[0][0] = a[0][0] * b[0][0] + a[0][1] * b[0][1] + a[0][2] * b[0][2];
	output[0][1] = a[0][0] * b[1][0] + a[0][1] * b[1][1] + a[0][2] * b[1][2];
	output[0][2] = a[0][0] * b[2][0] + a[0][1] * b[2][1] + a[0][2] * b[2][2];

	output[1][0] = a[1][0] * b[0][0] + a[1][1] * b[0][1] + a[1][2] * b[0][2];
	output[1][1] = a[1][0] * b[1][0] + a[1][1] * b[1][1] + a[1][2] * b[1][2];
	output[1][2] = a[1][0] * b[2][0] + a[1][1] * b[2][1] + a[1][2] * b[2][2];

	output[2][0] = a[2][0] * b[0][0] + a[2][1] * b[0][1] + a[2][2] * b[0][2];
	output[2][1] = a[2][0] * b[1][0] + a[2][1] * b[1][1] + a[2][2] * b[1][2];
	output[2][2] = a[2][0] * b[2][0] + a[2][1] * b[2][1] + a[2][2] * b[2][2];

	return;
}

void quaternion_to_DCM(double q[], double DCM[][3])
{
	// input : q : quaternion, output : DCM : Direction Cosine Matrix

	DCM[0][0] = q[0] * q[0] - q[1] * q[1] - q[2] * q[2] + q[3] * q[3];
	DCM[1][0] = 2 * (q[0] * q[1] - q[2] * q[3]);
	DCM[2][0] = 2 * (q[0] * q[2] + q[1] * q[3]);

	DCM[0][1] = 2 * (q[0] * q[1] + q[2] * q[3]);
	DCM[1][1] = -q[0] * q[0] + q[1] * q[1] - q[2] * q[2] + q[3] * q[3];
	DCM[2][1] = 2 * (q[1] * q[2] - q[0] * q[3]);

	DCM[0][2] = 2 * (q[0] * q[2] - q[1] * q[3]);
	DCM[1][2] = 2 * (q[1] * q[2] + q[0] * q[3]);
	DCM[2][2] = -q[0] * q[0] - q[1] * q[1] + q[2] * q[2] + q[3] * q[3];
}

double randn()
{
	// mean 0, Var 1 Gaussian random number generator
	double r;
	int i;

	r = -6.0;
	for (i = 0; i<12; i++)
	{
		r = r + (double)rand() / (double)(RAND_MAX + 1);
	}
	return r;
}

// Merge sorting에 사용되는 함수. 
static int merge(int *a, int N, int *b, int M, int*c, double * compared_data)
{
	int n = 0;
	int m = 0;

	int i, j;


	if (M < 1)
	{
		for (j = 0; j<(N + M); j++)
		{
			c[j] = a[n];
			n++;
		}
	}
	else
	{
		for (i = 0; i < (N + M); i++)
		{
			if (compared_data[a[n]] < compared_data[b[m]])
			{
				c[i] = a[n];
				n++;
				if (n == N)
				{
					for (j = (i + 1); j < (N + M); j++)
					{
						c[j] = b[m];
						m++;
					}
					return 0;
				}
			}
			else
			{
				c[i] = b[m];
				m++;
				if (m == M)
				{
					for (j = (i + 1); j < (N + M); j++)
					{
						c[j] = a[n];
						n++;
					}
					return 0;
				}
			}
		}
	}
	return 0;
}


// Merge sort 함수. 주어진 데이터를 기준으로 index를 merge sorting 시킨다.
void sort_merge(int Data_N, double *Data, int *Index)
{
	int step, flag;
	int start_1, start_2, times;
	int final_step;
	int i;

	if (Data_N > BUFSIZEFORMERGE)
	{
		printf(" The buffer is overflowed in Merge Sorting \n");
		return;
	}



	for (i = 0; i < Data_N; i++)
	{
		buf_1[i] = i;
	}

	step = 1; flag = 0;
	final_step = 0;
	i = 0;

	while (step < Data_N)
	{
		times = (int)ceil((double)Data_N / (double)(step * 2));

		start_1 = 0;
		start_2 = 0;

		for (i = 0; i < (times - 1); i++)
		{
			start_1 = i * step * 2;
			start_2 = start_1 + step;

			if (flag == 0)
			{
				merge(&buf_1[start_1], step, &buf_1[start_2], step, &buf_2[start_1], Data);
			}
			else
			{
				merge(&buf_2[start_1], step, &buf_2[start_2], step, &buf_1[start_1], Data);
			}
		}
		start_1 = i * step * 2;
		start_2 = start_1 + step;

		final_step = Data_N - start_2;

		if (flag == 0)
		{
			merge(&buf_1[start_1], step, &buf_1[start_2], final_step, &buf_2[start_1], Data);
		}
		else
		{
			merge(&buf_2[start_1], step, &buf_2[start_2], final_step, &buf_1[start_1], Data);
		}

		if (flag == 0){ flag = 1; }
		else{ flag = 0; }

		step = step * 2;
	}

	if (flag == 0)
	{
		for (i = 0; i < Data_N; i++)
			Index[i] = buf_1[i];
	}
	else
	{
		for (i = 0; i < Data_N; i++)
			Index[i] = buf_2[i];
	}
}

static void absAe(double a[][3], double b[][3])
{
	double s = 0.0;

	s = a[0][0] * (a[1][1] * a[2][2] - a[1][2] * a[2][1]);
	s += a[0][1] * (a[1][2] * a[2][0] - a[1][0] * a[2][2]);
	s += a[0][2] * (a[1][0] * a[2][1] - a[1][1] * a[2][0]);


	b[0][0] = (a[1][1] * a[2][2] - a[1][2] * a[2][1]) / s;
	b[0][1] = (a[0][2] * a[2][1] - a[0][1] * a[2][2]) / s;
	b[0][2] = (a[0][1] * a[1][2] - a[0][2] * a[1][1]) / s;
	b[1][0] = (a[1][2] * a[2][0] - a[1][0] * a[2][2]) / s;
	b[1][1] = (a[0][0] * a[2][2] - a[0][2] * a[2][0]) / s;
	b[1][2] = (a[0][2] * a[1][0] - a[0][0] * a[1][2]) / s;
	b[2][0] = (a[1][0] * a[2][1] - a[1][1] * a[2][0]) / s;
	b[2][1] = (a[0][1] * a[2][0] - a[0][0] * a[2][1]) / s;
	b[2][2] = (a[0][0] * a[1][1] - a[0][1] * a[1][0]) / s;

	return;
}

void calAtt(int Data_N, double v_body[][3], double v_eci[][3], double w[], double q[])
{
	double B[3][3], B_trans[3][3];
	double S[3][3];
	double Z[3];
	double sigma;
	double lambda_opt;

	double A[3][3], AA[3][3];
	double p[3];
	double dem;

	int i, j, k;

	if (Data_N < 2)
	{
		q[0] = q[1] = q[2] = 0; q[3] = 1;
		return;
	}

	for (i = 0; i < 3; i++)
		for (j = 0; j < 3; j++)
			B[i][j] = 0.0;


	for (k = 0; k < Data_N; k++)
	{
		for (i = 0; i < 3; i++)
		{
			for (j = 0; j < 3; j++)
			{
				B[i][j] = B[i][j] + w[k] * (v_body[k][i] * v_eci[k][j]);
			}
		}
	}

	matrix_3x3_transpose(B, B_trans);

	for (i = 0; i < 3; i++)	
		for (j = 0; j < 3; j++)
			S[i][j] = B[i][j] + B_trans[i][j];

	Z[0] = B[1][2] - B[2][1]; Z[1] = B[2][0] - B[0][2]; Z[2] = B[0][1] - B[1][0];

	sigma = B[0][0] + B[1][1] + B[2][2];

	lambda_opt = 0.0;
	for (k = 0; k < Data_N; k++)
		lambda_opt = lambda_opt + w[k];

	// A 구하기
	// 우선 A = (람다 + 시그마) * 아이로 하기
	for (i = 0; i < 3; i++)
		for (j = 0; j < 3; j++)
			A[i][j] = 0.0;
	A[0][0] = lambda_opt + sigma; A[1][1] = lambda_opt + sigma; A[2][2] = lambda_opt + sigma;
	
	for (i = 0; i < 3; i++)
		for (j = 0; j < 3; j++)
			A[i][j] = A[i][j] - S[i][j];

	absAe(A, AA);

	// 로드리게스 파라미터
	matrix_3x3_multiply_vector_3x1(AA, Z, p);

	// 쿼터니안 구하기
	dem = sqrt(1 + p[0] * p[0] + p[1] * p[1] + p[2] * p[2]);
	q[0] = p[0] / dem;
	q[1] = p[1] / dem;
	q[2] = p[2] / dem;
	q[3] = 1 / dem;
}
