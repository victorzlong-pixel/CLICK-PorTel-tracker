#ifndef __OIM_CONTROL
#define __OIM_CONTROL

#include <Windows.h>
#include "ftd2xx.h"
#include "LibFT4222.h"

extern "C" _declspec(dllexport) int initFT4222_SPI(unsigned char* _msg);
extern "C" _declspec(dllexport) int closeFT4222_SPI();
int sendCmd(unsigned short value, GPIO_Port port);
int setOIM_V(double v, GPIO_Port port);
extern "C" _declspec(dllexport) int setOIM_Vxy(double Vx, double Vy);

#endif