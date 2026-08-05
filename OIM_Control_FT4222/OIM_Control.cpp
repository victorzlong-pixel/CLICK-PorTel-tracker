
#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <vector>
#include <string>
#include "ftd2xx.h"
#include "LibFT4222.h"
#include "OIM_Control.h"

FT_HANDLE ftHandle = NULL;
FT_HANDLE ftGPIOHandle = NULL;
FT_STATUS ftStatus = 0;
FT4222_STATUS ft4222Status = FT4222_OK;
std::vector< FT_DEVICE_LIST_INFO_NODE > g_FT4222DevList;
std::vector< FT_DEVICE_LIST_INFO_NODE > g_FT4222GPIODevList;

extern "C" _declspec(dllexport) int initFT4222H_SPI(unsigned char* _msg)
{
	DWORD numOfDevices = 0;
	ftStatus = FT_CreateDeviceInfoList(&numOfDevices);
	for (DWORD iDev = 0; iDev < numOfDevices; ++iDev)
	{
		FT_DEVICE_LIST_INFO_NODE devInfo;
		memset(&devInfo, 0, sizeof(devInfo));

		ftStatus = FT_GetDeviceInfoDetail(iDev, &devInfo.Flags, &devInfo.Type, &devInfo.ID, &devInfo.LocId,
			devInfo.SerialNumber,
			devInfo.Description,
			&devInfo.ftHandle);

		const std::string desc = devInfo.Description;
		if (desc == "FT4222" || desc == "FT4222 A")
		{
			g_FT4222DevList.push_back(devInfo);
		}
		if (desc == "FT4222 B")
		{
			g_FT4222GPIODevList.push_back(devInfo);
		}
	}

	if (g_FT4222DevList.empty()) {
		printf("No FT4222 device is found!\n");
		return -1;
	}
	ftStatus = FT_OpenEx((PVOID)g_FT4222DevList[0].LocId, FT_OPEN_BY_LOCATION, &ftHandle);
	ftStatus = FT_OpenEx((PVOID)g_FT4222GPIODevList[0].LocId, FT_OPEN_BY_LOCATION, &ftGPIOHandle);
	if (FT_OK != ftStatus)
	{
		printf("Open a FT4222 device failed!\n");
		return -1;
	}
	printf("\n\n");
	printf("Init FT4222 as SPI master\n");
	ftStatus = FT4222_SPIMaster_Init(ftHandle, SPI_IO_SINGLE, CLK_DIV_4, CLK_IDLE_LOW, CLK_LEADING, 0x01);
	if (FT_OK != ftStatus)
	{
		printf("Init FT4222 as SPI master device failed!\n");
		return -1;
	}
	GPIO_Dir dir[4];
	dir[0] = GPIO_OUTPUT;
	dir[1] = GPIO_OUTPUT;
	dir[2] = GPIO_OUTPUT;
	dir[3] = GPIO_OUTPUT;
	ft4222Status = FT4222_GPIO_Init(ftGPIOHandle, dir);
	ft4222Status = FT4222_GPIO_Write(ftGPIOHandle, GPIO_PORT0, false);
	ft4222Status = FT4222_GPIO_Write(ftGPIOHandle, GPIO_PORT1, false);
	return 0;
}

extern "C" _declspec(dllexport) int closeFT4222H_SPI()
{
	printf("UnInitialize FT4222\n");
	FT4222_UnInitialize(ftHandle);

	printf("Close FT device\n");
	FT_Close(ftHandle);
	return 0;
}

int sendCmd(unsigned short value, GPIO_Port port)
{
	uint16 sizeTransferred;
	unsigned char b[2];
	b[0] = (value >> 8) &0xFF;
	b[1] = (value & 0xFF);
	ft4222Status = FT4222_SPIMaster_SingleWrite(ftHandle, b, 2, &sizeTransferred, true);
	ft4222Status = FT4222_GPIO_Write(ftGPIOHandle, port, true); // true = high, false = low
	ft4222Status = FT4222_GPIO_Write(ftGPIOHandle, port, false);
	return 1;
}


int setOIM_V(double v, GPIO_Port port)
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
	sendCmd(us, port);
	return 1;
}

extern "C" _declspec(dllexport) int setOIM_Vxy(double Vx, double Vy)
{
	setOIM_V(Vx, GPIO_PORT0);
	setOIM_V(Vy, GPIO_PORT1);
	return 1;
}


