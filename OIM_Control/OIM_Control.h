#ifndef __OIM_CONTROL
#define __OIM_CONTROL

#include <Windows.h>
#include "ftd2xx.h"
#include "libMPSSE_spi.h"
#include "PinConfig.h"

extern "C" _declspec(dllexport) int initFT232H_SPI(unsigned char* _msg);
extern "C" _declspec(dllexport) int closeFT232H_SPI();
int sendCmd(unsigned short value, uint8 pin);
int setOIM_V(double v, uint8 CSpin);
extern "C" _declspec(dllexport) int setOIM_Vxy(double Vx, double Vy);

#endif

