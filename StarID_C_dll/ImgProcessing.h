#ifndef __ImgProcessing
#define __ImgProcessing

#include "Structures.h"

void LoadImgFile(char* _fn, unsigned short pgm[][WIDTH]);
void Thresholding(int _thres, unsigned short _pgm[][WIDTH], STARPIXELS* _sp);
int Grouping(int Min_N, int Max_N, STARPIXELS *_sp);
int Centroiding(int StarN, STARPIXELS* _sp, STARS* _star);
int SelectStars(STARS* _star, STARSFORID * _sfi, int MaxMagStarN, int MaxIMGStarN);
void ConvertUV2XYZ(double U, double V, double d, double f, double CalPar[][10], double XYZ[]);

void DSPF_sp_cfftr2_dit(float *x, float*w, short n);
void DSPF_sp_icfftr2_dif(float* x, float* w, short n);

void bit_rev(float* x, int n);
void bit_rev_2d(float* data, int nx, int ny, float * buf);

void gen_w_r2(float * w, int n);

void fft2_bitrev_output(float * data, float * wx, float * wy, int nx, int ny, float * buf);
void divide(float* x, int n);
void ifft2_bitrev_input(float * data, float * wx, float * wy, int nx, int ny, float * buf);
void MakeFilter(int filter[][Y_FFT]);
void LoadImgAndHighPassFilter(char* _fn, unsigned short(*pgm)[WIDTH]);

#endif
