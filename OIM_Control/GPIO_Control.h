#ifndef __GPIO_CONTROL
#define __GPIO_CONTROL

#include <Windows.h>
#include "ftd2xx.h"
#include "libMPSSE_spi.h"
#include "PinConfig.h"

int initGPIO(FT_HANDLE ftHandle);
int setGPIOHigh(FT_HANDLE fth, uint8 pin);
int setGPIOLow(FT_HANDLE fth, uint8 pin);

#endif
