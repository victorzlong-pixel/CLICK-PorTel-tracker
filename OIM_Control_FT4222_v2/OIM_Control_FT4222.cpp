#include <stdio.h>
#include <stdlib.h>
#include <Windows.h>
#include <vector>
#include <string>
#include "ftd2xx.h"
#include "LibFT4222.h"
//#include "OIM_Control.h"

FT_HANDLE ftHandle = NULL;
FT_STATUS ftStatus = 0;
std::vector< FT_DEVICE_LIST_INFO_NODE > g_FT4222DevList;

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
	}

	if (g_FT4222DevList.empty()) {
		printf("No FT4222 device is found!\n");
		return 0;
	}
	FT_STATUS ftStatus;
	ftStatus = FT_OpenEx((PVOID)g_FT4222DevList[0].LocId, FT_OPEN_BY_LOCATION, &ftHandle);
	if (FT_OK != ftStatus)
	{
		printf("Open a FT4222 device failed!\n");
		return 0;
	}
	printf("\n\n");
	printf("Init FT4222 as SPI master\n");
	ftStatus = FT4222_SPIMaster_Init(ftHandle, SPI_IO_SINGLE, CLK_DIV_4, CLK_IDLE_LOW, CLK_LEADING, 0x01);
	if (FT_OK != ftStatus)
	{
		printf("Init FT4222 as SPI master device failed!\n");
		return 0;
	}
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