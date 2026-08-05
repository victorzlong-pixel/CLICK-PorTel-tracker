#include "ExternFT232H_SPI.h"

#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>

#include "ftd2xx.h"
#include "libMPSSE_spi.h"
#include "PinConfig.h"
#include "OIMFSM_Control.h"
#include "GPIO_Control.h"

uint32 channels;
FT_HANDLE ftHandle = 0;
ChannelConfig channelConf;


int initFT232H_SPI(char* _msg)
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
		sprintf(_msg, "Number of Channel is 0\n");
		return -1;
	}

	status = SPI_OpenChannel(0, &ftHandle);
	if (status != FT_OK)
	{
		sprintf(_msg, "SPI Open Failed.\n");
		return -2;
	}

	status = SPI_InitChannel(ftHandle, &channelConf);
	
	initGPIO(ftHandle);
	initOIMFSM(ftHandle);

	sprintf(_msg, "FT232H initialized\n");
	printf("FT232H initialized\n");
	return 0;
}

int closeFT232H_SPI()
{
	SPI_CloseChannel(ftHandle);

	return 0;
}

void setOIMFSM_Vxy(double Vx, double Vy)
{
	setOIMFSM_Vxy(ftHandle, Vx, Vy);
	return;
}

void setOIMFSM_Vx(double Vx)
{
	setOIMFSM_V(ftHandle, Vx, PIN_CS_OIMX);
	return;
}

void setOIMFSM_Vy(double Vy)
{
	setOIMFSM_V(ftHandle, Vy, PIN_CS_OIMY);
	return;
}

void setOIMFSM_Origin()
{
	setOIMFSM_Origin(ftHandle);
}
