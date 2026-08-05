#ifndef __HSYMath
#define __HSYMath

extern const double d2r;
extern const double r2d;

double vector_3x1_BtwAngle_Small(double A[], double B[]);
void triad_method(double * A_1, double *B_1, double *A_2, double *B_2, double(*DCM_1_to_2)[3]);
int binary_search(double e, double * Data, int N, int * Index);
void matrix_3x3_multiply_vector_3x1(double M[][3], double V[], double output[]);
void vector_3x1_minus(double A[], double B[], double output[]);
double vector_3x1_dot(double A[], double B[]);
double quaternion_normalize(double q[], double output[]);
double vector_3x1_normalize(double input[], double output[]);
void vector_3x1_cross(double A[], double B[], double output[]);
void matrix_3x3_transpose(double A[][3], double output[][3]);
void matrix_3x3_product(double a[][3], double b[][3], double output[][3]);
void matrix_3x3_transpose_product(double a[][3], double b[][3], double output[][3]);
void quaternion_to_DCM(double q[], double DCM[][3]);
double randn();
void sort_merge(int Data_N, double *Data, int *Index);
void calAtt(int Data_N, double v_body[][3], double v_eci[][3], double w[], double q[]);

#endif