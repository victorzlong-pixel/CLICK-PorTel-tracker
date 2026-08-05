#include "GPIO_Control.h"

static uint8 s_GPIO = 0;

int initGPIO(FT_HANDLE ftHandle)
{
	FT_WriteGPIO(ftHandle, PIN_GPIO_ALL, 0x00);	// Set CS pins 0
	s_GPIO = 0;

	return 0;
}

int setGPIOHigh(FT_HANDLE fth, uint8 pin)
{
	s_GPIO = s_GPIO | pin;
	FT_WriteGPIO(fth, PIN_GPIO_ALL, s_GPIO);
	return 0;
}

int setGPIOLow(FT_HANDLE fth, uint8 pin)
{
	s_GPIO = s_GPIO & (~pin);
	FT_WriteGPIO(fth, PIN_GPIO_ALL, s_GPIO);
	return 0;
}