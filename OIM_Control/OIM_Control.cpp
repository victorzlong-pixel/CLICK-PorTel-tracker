#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <vector>
#include <string>

#include "ftd2xx.h"
#include "libMPSSE_spi.h"
#include "PinConfig.h"
#include "OIM_Control.h"
#include "GPIO_Control.h"

uint32 channels;
FT_HANDLE ftHandle = 0;
ChannelConfig channelConf;
static FT_STATUS s_sta;

/*extern "C" _declspec(dllexport) int initFT4222H_SPI(unsigned char* _msg)
{
	return 0;
}*/

// Open SPI connection.
extern "C" _declspec(dllexport) int initFT232H_SPI(unsigned char* _msg)
{
	FT_STATUS status;

	Init_libMPSSE();

	channelConf.ClockRate = 1000000;
	channelConf.LatencyTimer = 255;
	channelConf.configOptions = SPI_CONFIG_OPTION_MODE0 | SPI_CONFIG_OPTION_CS_DBUS3;
	channelConf.Pin = 0x00000000;

	status = SPI_GetNumChannels(&channels);
	if (channels == 0)
	{
		sprintf((char*)_msg, "Number of channels is 0. \n");
		return -1;
	}

	status = SPI_OpenChannel(0, &ftHandle);
	if (status != FT_OK)
	{
		sprintf((char*)_msg, "SPI open failed.\n");
		return -2;
	}

	status = SPI_InitChannel(ftHandle, &channelConf);

	initGPIO(ftHandle);
	setGPIOLow(ftHandle, PIN_CS_OIMX);
	setGPIOLow(ftHandle, PIN_CS_OIMY);
	setOIM_Vxy(0, 0);

	sprintf((char*)_msg, "FT232H initialized. \n");
	printf("FT232H initialized. \n");
	return 0;
}

// Close SPI connection.
extern "C" _declspec(dllexport) int closeFT232H_SPI()
{
	SPI_CloseChannel(ftHandle);
	return 0;
}

int sendCmd(unsigned short value, uint8 pin)
{
	uint32 sizeTransferred;
	unsigned char b[2];
	b[0] = (value >> 8) & 0xFF;
	b[1] = (value & 0xFF);
	s_sta = SPI_Write(ftHandle, b, 2, &sizeTransferred, SPI_TRANSFER_OPTIONS_SIZE_IN_BYTES);

	setGPIOHigh(ftHandle, pin);
	setGPIOLow(ftHandle, pin);
	return 1;
}

int setOIM_V(double v, uint8 CSpin)
{
	double v1;
	unsigned short us;

	if (v >= 10)
		us = 0xFFFF; // max voltage
	else if (v < -10)
		us = 0; // min voltage
	else
	{
		v1 = v + 10;
		us = (unsigned short)(v1 * 3276.8); // scale factor 
	}
	sendCmd(us, CSpin);
	return 1;
}

extern "C" _declspec(dllexport) int setOIM_Vxy(double Vx, double Vy)
{
	setOIM_V(Vx, PIN_CS_OIMX);
	setOIM_V(Vy, PIN_CS_OIMY);
	return 1;
}


