#include "ImgProcessing.h"

#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include "GlobalVariables.h"
#include "HSYMath.h"


static int buf_V[WIDTH + 2];
static int buf_data_index[WIDTH + 2];

#define BUFSIZE 10000

static int buf_index[BUFSIZE];
static int buf_cnt[BUFSIZE];
static int index_table[BUFSIZE];
static int Group_index[HEIGHT * WIDTH];
static int idx_mag[MAXSTARN];
static int idx_r2[MAXSTARN];
static int Selected[MAXSTARN];
static double mag_buf[MAXSTARN];
static double r2_buf[MAXSTARN];

//static double Vm[BUFSIZE];
//static double Mag[BUFSIZE];

void LoadImgFile(char* _fn, unsigned short pgm[][WIDTH])
{
	char c;
	int i = 0, j;
	FILE* fp = fopen(_fn, "rb");
	unsigned char ucbuf, *ucp;
	memset(pgm, 0, sizeof(unsigned short) * WIDTH * HEIGHT);

	//while (1)
	for (j = 0; j < 1000; j++)
	{
		c = fgetc(fp);
		i++; 
		if ( (c == 10)|| (c == 13))
			break;
	}
	
	fread(pgm, 2, WIDTH*HEIGHT, fp);

	for (i = 0; i < WIDTH*HEIGHT; i++)
	{
		ucp = (unsigned char*)pgm + 2 * i;
		ucbuf = *ucp;
		*ucp = *(ucp + 1);
		*(ucp + 1) = ucbuf;
	}

	fclose(fp);
}

static int CalAutoThresholdValue(unsigned short _pgm[][WIDTH], int sigmamultiplier)
{
	double mean = 0, sigma = 0;
	double mean1 = 0, mean2 = 0;
	double sigma1 = 0, sigma2 = 0;
	double m, s;

	int i, thres, n;
	n = 0;
	for (i = 0; i < WIDTH; i+=3)
	{
		n++;
		mean += _pgm[480][i];
		sigma += _pgm[480][i] * _pgm[480][i];

		mean1 += _pgm[240][i];
		sigma1 += _pgm[240][i] * _pgm[240][i];

		mean2 += _pgm[720][i];
		sigma2 += _pgm[720][i] * _pgm[720][i];
	}
	mean /= (double)n;
	sigma /= (double)n;
	sigma = sigma - mean * mean;
	sigma = sqrt(sigma);

	mean1 /= (double)n;
	sigma1 /= (double)n;
	sigma1 = sigma1 - mean1 * mean1;
	sigma1 = sqrt(sigma1);

	mean2 /= (double)n;
	sigma2 /= (double)n;
	sigma2 = sigma2 - mean2 * mean2;
	sigma2 = sqrt(sigma2);

	//if (sigma1 < sigma)
	//{
	//	if (sigma < sigma2)
	//	{
	//		m = mean + mean1;
	//		s = sigma + sigma1;
	//	}
	//	else
	//	{
	//		m = mean2 + mean1;
	//		s = sigma2 + sigma1;
	//	}
	//}
	//else
	//{
	//	if (sigma1 < sigma2)
	//	{
	//		m = mean + mean1;
	//		s = sigma + sigma1;
	//	}
	//	else
	//	{
	//		m = mean + mean2;
	//		s = sigma + sigma2;
	//	}
	//}
	//thres = (int)(m + s * sigmamultiplier) / 2;

	if (sigma1 < sigma)
	{
		if (sigma1 < sigma2)
		{
			m = mean1; s = sigma1;
		}
		else
		{
			m = mean2; s = sigma2;
		}
	}
	else
	{
		if (sigma < sigma2)
		{
			m = mean; s = sigma;
		}
		else
		{
			m = mean2; s = sigma2;
		}
	}
	thres = (int)(m + s * sigmamultiplier);
	
	return thres;
}

void Thresholding(int _thres, unsigned short _pgm[][WIDTH], STARPIXELS* _sp)
{
	int i, j, n;

	if (_thres < 0)
	{
		_thres = CalAutoThresholdValue(_pgm, 5);
		/*_thres = _thres > 200 ? 200 : _thres;
		_thres = _thres < 70 ? 70 : _thres;*/
	}

	n = 0;
	for (i = DEADPIXEL_H; i < (HEIGHT - DEADPIXEL_H); i++)
	{
		for (j = DEADPIXEL_W; j < (WIDTH - DEADPIXEL_W); j++)
		{
			if (_pgm[i][j] <= _thres)
				continue;
			_sp->PXL[n].Mag = (int)_pgm[i][j] - (int)_thres;
			_sp->PXL[n].U = j;
			_sp->PXL[n].V = i;
			_sp->PXL[n].ID = -1;
			n++;
		}
	}
	_sp->N = n;
}

// �׷��� ���ִ� �˰���
int Grouping(int Min_N, int Max_N, STARPIXELS *_sp)
{
	// Input :  Data_N : �� �ȼ� �������� ����
	//			Min_N : ���� �νĵǴ� �ּ� �ȼ� ����
	//			Max_N : ���� �νĵǴ� �ִ� �ȼ� ����
	//			IMG_U : �̹����� U ����
	//			IMG_V : �̹����� V ����
	//
	// output :	Group_index : �̹��� �׷� �ε���
	//
	// return: Group ����

	// grouping �Լ��� ���� ������ �ؾ� �Ѵ�. �ٸ� �� �� ������
	// �ѹ��� �Ȱ� �ε����� �����ֵ��� ���α׷��� �Ͽ���.

	// ���� ���� ����
	
	int U, V, U_1, V_1;

	int before_U, before_V, before_index;

	int candidate_index, next_index;
	int i, j, flag, flag_min[4], cnt;
	int dV, dU;
	
	// index_table �ʱ�ȭ
	for (i = 0; i < BUFSIZE; i++)
		index_table[i] = -1;


	// ������ 1��
	// ���� �ʱ�ȭ
	for (i = 0; i < (WIDTH + 2); i++)
	{
		buf_V[i] = -3;
		buf_data_index[i] = 0;
	}
	before_U = 0; before_V = -2; before_index = 0;
	next_index = 0;

	for (i = 0; i < _sp->N; i++)
	{
		U = _sp->PXL[i].U; V = _sp->PXL[i].V;
		U_1 = U - 1; V_1 = V - 1;

		candidate_index = next_index;
		flag = 0;
		flag_min[0] = 0; flag_min[1] = 0; flag_min[2] = 0; flag_min[3] = 0;

		// ���� ���� index ����

		if (buf_V[U] == V_1)
		{
			candidate_index = Group_index[buf_data_index[U]];
			flag = 1;
			flag_min[0] = 1;
		}

		if (buf_V[U + 1] == V_1)
		{
			flag_min[1] = 1;
			if (candidate_index > Group_index[buf_data_index[U + 1]])
			{
				candidate_index = Group_index[buf_data_index[U + 1]];
				flag = 1;
			}
		}

		if (buf_V[U + 2] == V_1)
		{
			flag_min[2] = 1;
			if (candidate_index > Group_index[buf_data_index[U + 2]])
			{
				candidate_index = Group_index[buf_data_index[U + 2]];
				flag = 1;
			}
		}

		dV = before_V - V;
		dV = dV < 0 ? -dV : dV;
		
		if (dV < 2)
		{
			dU= before_U - U;
			dU = dU < 0 ? -dU : dU;

			if (dU < 2)
			{
				flag_min[3] = 1;
				if (candidate_index > Group_index[before_index])
				{
					candidate_index = Group_index[before_index];
					flag = 1;
				}
			}
		}

		if (flag == 0)
		{
			next_index++;
			if (next_index >= BUFSIZE)
				return -next_index;	// Buffer Overflow!
		}

		// ���� ���� index�� �� ������Ʈ
		Group_index[i] = candidate_index;
		for (j = 0; j < 3; j++)
		{
			int ind;
			ind = Group_index[buf_data_index[U + j]];

			if (flag_min[j] > 0)
			{
				if (((index_table[ind] == -1) && (ind != candidate_index)) || (index_table[ind] > candidate_index))
					index_table[ind] = candidate_index;

				Group_index[buf_data_index[U + j]] = candidate_index;
			}
		}

		if (flag_min[3] > 0)
		{
			int ind;
			ind = Group_index[before_index];

			if (((index_table[ind] == -1) && (ind != candidate_index)) || (index_table[ind] > candidate_index))
				index_table[ind] = candidate_index;
			Group_index[before_index] = candidate_index;
		}

		buf_V[before_U + 1] = before_V;
		buf_data_index[before_U + 1] = before_index;

		before_U = U;
		before_V = V;
		before_index = i;
	}
		
	// index_table ���� �� ���� �ö󰡸鼭 index ������ ���ֱ�.

	for (i = 0; i < next_index; i++)
	{
		//printf(" i : %d, index table : %d\n",i, index_table[i]);

		if (index_table[i] > 0)
		{
			while (index_table[index_table[i]] > 0)
				index_table[i] = index_table[index_table[i]];
		}
	}

	// �׷� ��ȣ �Է�
	for (i = 0; i < _sp->N; i++)
	{
		int old_index = Group_index[i];

		if (index_table[old_index] > 0)
		{
			Group_index[i] = index_table[old_index];
		}
	}



	// �׷� �� ����.
	for (i = 0; i < next_index; i++)
	{
		buf_cnt[i] = 0;
		buf_index[i] = -1;
	}

	for (i = 0; i < _sp->N; i++)
	{
		buf_cnt[Group_index[i]] ++;
	}

	cnt = -1;
	for (i = 0; i < next_index; i++)	// �׷��� ũ�Ⱑ Min_N �� �̻��� ���� �׷� ��ȣ �ο�
	{
		if ((buf_cnt[i] >= Min_N) && (buf_cnt[i] <= Max_N))
		{
			cnt++;
			buf_index[i] = cnt;
		}
	}
	cnt++;

	// �׷� �ε��� �� �ο�

	for (i = 0; i < _sp->N; i++)
	{
		//Group_index[i] = buf_index[Group_index[i]];
		_sp->PXL[i].ID = buf_index[Group_index[i]];
	}

	return cnt;
}

int Centroiding(int StarN, STARPIXELS* _sp, STARS* _star)
{
	int i, id;
	
	memset(_star->S, 0, sizeof(_star->S));
	_star->N = StarN;

	for (i = 0; i < _sp->N; i++)
	{
		if (_sp->PXL[i].ID < 0)
			continue;

		id = _sp->PXL[i].ID;
		_star->S[id].MagSum += _sp->PXL[i].Mag;
		_star->S[id].U += _sp->PXL[i].U * _sp->PXL[i].Mag;
		_star->S[id].V += _sp->PXL[i].V * _sp->PXL[i].Mag;
		_star->S[id].PixelN++;
		_star->S[id].MaxMag = _star->S[id].MaxMag < _sp->PXL[i].Mag ? _sp->PXL[i].Mag : _star->S[id].MaxMag;
	}

	for (i = 0; i < StarN; i++)
	{
		_star->S[i].ID_result = -100;
		_star->S[i].U /= (double)_star->S[i].MagSum;
		_star->S[i].V /= (double)_star->S[i].MagSum;
	}

	return 1;
}

int SelectStars(STARS* _star, STARSFORID * _sfi, int MaxMagStarN, int MaxIMGStarN)
{
	int i, n;
	int r2StarN, IMGStarN, MagStarN, StarN;

	memset(Selected, 0, sizeof(Selected));

	StarN = _star->N;

	//IMGStarN = _star->N > MaxIMGStarN ? MaxIMGStarN : _star->N;

	for (i = 0; i < _star->N; i++)
	{
		double x, y;
		mag_buf[i] = _star->S[i].MagSum;
		x = _star->S[i].U - HALFWIDTH;
		y = _star->S[i].V - HALFHEIGHT;
		r2_buf[i] = x*x + y*y;

		if ((_star->S[i].MaxMag > 4090) || ((_star->S[i].U < 10) || (_star->S[i].U >(WIDTH - 10)) || (_star->S[i].V < 10) || (_star->S[i].V >(HEIGHT - 10))))
		{
			Selected[i] = -1;
			_star->S[i].ID_result = -101;
			_star->S[i].ID_suc = 0;
			StarN--;
			continue;
		}
	}

	sort_merge(_star->N, mag_buf, idx_mag);
	sort_merge(_star->N, r2_buf, idx_r2);

	IMGStarN = StarN > MaxIMGStarN ? MaxIMGStarN : StarN;

	MagStarN = IMGStarN > MaxMagStarN ? MaxMagStarN : IMGStarN;
	r2StarN = IMGStarN - MagStarN;

	n = 0;
	for (i = 0; i < _star->N; i++)
	{
		int idx = idx_mag[_star->N - 1 - i];
		if (Selected[idx] != 0)
			continue;

		_sfi->idx[n] = idx;
		Selected[idx] = 1;
		n++;
		if (n == MagStarN)
			break;
	}
	n = 0;
	for (i = 0; i < _star->N; i++)
	{
		if (n == r2StarN)
			break;
		if (Selected[idx_r2[i]] != 0)
			continue;
		_sfi->idx[MagStarN + n] = idx_r2[i];

		n++;
	}
	_sfi->N = IMGStarN;

	for (i = 0; i < _sfi->N; i++)
	{
		int idx = _sfi->idx[i];
		ConvertUV2XYZ(_star->S[idx].U, _star->S[idx].V, PIXELSIZE, FOCALLENGTH, g_CalPar, _sfi->V[i].XYZ_body);
	}

	return 1;
}

void ConvertUV2XYZ(double U, double V, double d, double f, double CalPar[][10], double XYZ[])
{
	double x, y, A[10], X, Y;
	int i;

	x = U - HALFWIDTH;
	y = V - HALFHEIGHT;
	A[0] = 10;
	A[1] = x;
	A[2] = x * x;
	A[3] = A[2] * x;
	A[4] = y;
	A[5] = y * y;
	A[6] = A[5] * y;
	A[7] = x * y;
	A[8] = A[7] * x;
	A[9] = A[7] * y;

	X = Y = 0;
	for (i = 0; i < 10; i++)
	{
		X += A[i] * CalPar[0][i];
		Y += A[i] * CalPar[1][i];
	}

	XYZ[0] = X * d;
	XYZ[1] = Y * d;
	XYZ[2] = f;

	vector_3x1_normalize(XYZ, XYZ);

	return;
}


void DSPF_sp_cfftr2_dit(float *x, float*w, short n)
{
	int n2, ie, ia, i, j, k, m;
	float rtemp, itemp, c, s;
	n2 = n;
	ie = 1;
	for (k = n; k > 1; k >>= 1)
	{
		n2 >>= 1;
		ia = 0;
		for (j = 0; j < ie; j++)
		{
			c = w[2 * j];
			s = w[2 * j + 1];
			for (i = 0; i < n2; i++)
			{
				m = ia + n2;
				rtemp = c*x[2 * m] + s*x[2 * m + 1];
				itemp = c*x[2 * m + 1] - s*x[2 * m];
				x[2 * m] = x[2 * ia] - rtemp;
				x[2 * m + 1] = x[2 * ia + 1] - itemp;
				x[2 * ia] = x[2 * ia] + rtemp;
				x[2 * ia + 1] = x[2 * ia + 1] + itemp;
				ia++;
			}
			ia += n2;
		}
		ie <<= 1;
	}
}

void DSPF_sp_icfftr2_dif(float* x, float* w, short n)
{
	int n2, ie, ia, i, j, k, m;
	float rtemp, itemp, c, s;
	n2 = 1;
	ie = n;
	for (k = n; k > 1; k >>= 1)
	{
		ie >>= 1;
		ia = 0;
		for (j = 0; j < ie; j++)
		{
			c = w[2 * j];
			s = w[2 * j + 1];
			for (i = 0; i < n2; i++)
			{
				m = ia + n2;
				rtemp = x[2 * ia] - x[2 * m];
				x[2 * ia] = x[2 * ia] + x[2 * m];
				itemp = x[2 * ia + 1] - x[2 * m + 1];
				x[2 * ia + 1] = x[2 * ia + 1] + x[2 * m + 1];
				x[2 * m] = c*rtemp - s*itemp;
				x[2 * m + 1] = c*itemp + s*rtemp;
				ia++;
			}
			ia += n2;
		}
		n2 <<= 1;
	}
}

/* ======================================================================== */
/*                                                                          */
/*  TEXAS INSTRUMENTS, INC.                                                 */
/*                                                                          */
/*  NAME                                                                    */
/*      bit_rev                                                             */
/*                                                                          */
/*  USAGE                                                                   */
/*      This function has the prototype:                                    */
/*                                                                          */
/*      void bit_rev(float *x, int n);                                      */
/*                                                                          */
/*      x              : Array to be bit-reversed.                          */
/*      n              : Number of complex array elements to bit-reverse.   */
/*                                                                          */
/*  DESCRIPTION                                                             */
/*      This routine bit reverses the floating point array x which          */
/*      is considered to be an array of complex numbers with the even       */
/*      numbered elements being thr real parts of the complex numbers       */
/*      while the odd numbered elements being the imaginary parts of the    */
/*      complex numbers. This function is made use of in sp_icfftr2_dif     */
/*      to bit-reverse the twiddle factor array generated using             */
/*      tw_genr2fft.c.                                                      */
/* ======================================================================== */

void bit_rev(float* x, int n)
{
	int i, j, k;
	float rtemp, itemp;

	j = 0;
	for (i = 1; i < (n - 1); i++)
	{
		k = n >> 1;
		while (k <= j)
		{
			j -= k;
			k >>= 1;
		}
		j += k;
		if (i < j)
		{
			rtemp = x[j * 2];
			x[j * 2] = x[i * 2];
			x[i * 2] = rtemp;
			itemp = x[j * 2 + 1];
			x[j * 2 + 1] = x[i * 2 + 1];
			x[i * 2 + 1] = itemp;
		}
	}
}

void bit_rev_2d(float* data, int nx, int ny, float * buf)
{
	int i, j, offset;
	float * start_index;
	int r_1, r_2, i_1, i_2;

	for (i = 0; i < nx; i++)
	{
		offset = i * 2 * ny;

		start_index = data + offset;

		bit_rev(start_index, ny);
	}

	for (i = 0; i < nx; i++)
	{
		for (j = 0; j < ny; j++)
		{

			r_1 = (j * nx + i) * 2;
			r_2 = (i * ny + j) * 2;
			i_1 = r_1 + 1;
			i_2 = r_2 + 1;

			buf[r_1] = data[r_2];
			buf[i_1] = data[i_2];
		}
	}

	for (i = 0; i < ny; i++)
	{
		offset = i * 2 * nx;

		start_index = buf + offset;

		bit_rev(start_index, nx);
	}

	for (i = 0; i < ny; i++)
	{
		for (j = 0; j < nx; j++)
		{
			r_1 = (j * ny + i) * 2;
			r_2 = (i * nx + j) * 2;
			i_1 = r_1 + 1;
			i_2 = r_2 + 1;

			data[r_1] = buf[r_2];
			data[i_1] = buf[i_2];
		}
	}

}


void gen_w_r2(float * w, int n)
{
	// generate real and imaginary twiddle table of size n/2 complex numbers
	// 그래서 n = nx * 2
	// 이 함수로 생성한 w는 꼭 bitrev_index 함수로 뒤집어줘야 한다.
	int i;
	float pi = (float)(4.0 * atan(1.0));
	float e = pi * 2.0f / (float)n;
	for (i = 0; i < (n >> 1); i++)
	{
		w[2 * i] = (float)cos(i*e);
		w[2 * i + 1] = (float)sin(i*e);
	}

}

void fft2_bitrev_output(float * data, float * wx, float * wy, int nx, int ny, float * buf)
{
	int i, j, offset;
	float * start_index;
	int r_1, r_2, i_1, i_2;

	for (i = 0; i < nx; i++)
	{
		offset = i * 2 * ny;

		start_index = data + offset;

		DSPF_sp_cfftr2_dit(start_index, wy, ny);
		//bit_rev(start_index, ny);
	}

	for (i = 0; i < nx; i++)
	{
		for (j = 0; j < ny; j++)
		{

			r_1 = (j * nx + i) * 2;
			r_2 = (i * ny + j) * 2;
			i_1 = r_1 + 1;
			i_2 = r_2 + 1;

			buf[r_1] = data[r_2];
			buf[i_1] = data[i_2];
		}
	}

	for (i = 0; i < ny; i++)
	{
		offset = i * 2 * nx;

		start_index = buf + offset;

		DSPF_sp_cfftr2_dit(start_index, wx, nx);
		//bit_rev(start_index, nx);
	}

	for (i = 0; i < ny; i++)
	{
		for (j = 0; j < nx; j++)
		{
			r_1 = (j * ny + i) * 2;
			r_2 = (i * nx + j) * 2;
			i_1 = r_1 + 1;
			i_2 = r_2 + 1;

			data[r_1] = buf[r_2];
			data[i_1] = buf[i_2];
		}
	}
}

void divide(float* x, int n)
{
	int i;
	float p = 1.0f / (float)n;

	for (i = 0; i < n; i++)
	{
		x[2 * i] = x[2 * i] * p;
		x[2 * i + 1] = x[2 * i + 1] * p;

	}
}

void ifft2_bitrev_input(float * data, float * wx, float * wy, int nx, int ny, float * buf)
{
	int i, j, offset;
	float * start_index;
	int r_1, r_2, i_1, i_2;

	for (i = 0; i < nx; i++)
	{
		offset = i * 2 * ny;

		start_index = data + offset;

		DSPF_sp_icfftr2_dif(start_index, wy, ny);
		//bit_rev(start_index, ny);
	}

	for (i = 0; i < nx; i++)
	{
		for (j = 0; j < ny; j++)
		{

			r_1 = (j * nx + i) * 2;
			r_2 = (i * ny + j) * 2;
			i_1 = r_1 + 1;
			i_2 = r_2 + 1;

			buf[r_1] = data[r_2];
			buf[i_1] = data[i_2];
		}
	}

	for (i = 0; i < ny; i++)
	{
		offset = i * 2 * nx;

		start_index = buf + offset;

		DSPF_sp_icfftr2_dif(start_index, wx, nx);
		//bit_rev(start_index, nx);
	}

	for (i = 0; i < ny; i++)
	{
		for (j = 0; j < nx; j++)
		{
			r_1 = (j * ny + i) * 2;
			r_2 = (i * nx + j) * 2;
			i_1 = r_1 + 1;
			i_2 = r_2 + 1;

			data[r_1] = buf[r_2];
			data[i_1] = buf[i_2];
		}
	}
}

void MakeFilter(int filter[][Y_FFT])
{
	int i, j;
	int Dx = X_FFT / CUTOFF_N;
	int Dy = Y_FFT / CUTOFF_N;

	for (i = 0; i < X_FFT; i++)
	{
		for (j = 0; j < Y_FFT; j++)
		{
			g_fIMG[i][j][0] = 1.0f;
		}
	}

	for (i = 0; i < Dx; i++)
	{
		for (j = 0; j < Dy; j++)
		{
			g_fIMG[i][j][0] = 0;
			g_fIMG[X_FFT - 1 - i][j][0] = 0;
			g_fIMG[i][Y_FFT - 1 - j][0] = 0;
			g_fIMG[X_FFT - 1 - i][Y_FFT - 1 - j][0] = 0;
		}
	}

	bit_rev_2d(&g_fIMG[0][0][0], X_FFT, Y_FFT, &g_fIMG_buf[0][0][0]);
	for (i = 0; i < X_FFT; i++)
		for (j = 0; j < Y_FFT; j++)
			filter[i][j] = (int)g_fIMG[i][j][0];

	return;
}

void LoadImgAndHighPassFilter(char* _fn, unsigned short(*pgm)[WIDTH])
{
	int i, j;

	LoadImgFile(_fn, pgm);

	memset(g_fIMG, 0, sizeof(g_fIMG));
	for (i = 0; i < HEIGHT; i++)
		for (j = 0; j < WIDTH; j++)
			g_fIMG[i][j][0] = (float)pgm[i][j];

	fft2_bitrev_output(&g_fIMG[0][0][0], &g_wx[0][0], &g_wy[0][0], X_FFT, Y_FFT, &g_fIMG_buf[0][0][0]);
	for (i = 0; i < X_FFT; i++)
	{
		for (j = 0; j < Y_FFT; j++)
		{
			g_fIMG[i][j][0] *= g_Filter[i][j];
			g_fIMG[i][j][1] *= g_Filter[i][j];
		}
	}
	ifft2_bitrev_input(&g_fIMG[0][0][0], &g_wx[0][0], &g_wy[0][0], X_FFT, Y_FFT, &g_fIMG_buf[0][0][0]);
	divide(&g_fIMG[0][0][0], X_FFT*Y_FFT);
	for (i = 0; i < HEIGHT; i++)
		for (j = 0; j < WIDTH; j++)
			pgm[i][j] = g_fIMG[i][j][0] > 0 ? (unsigned short)g_fIMG[i][j][0] : 0;

	return;
}