// SU320CSX_Control.cpp : Defines the exported functions for the DLL application.
//
#define _CRT_SECURE_NO_DEPRECATE
#define WIDTH 320
#define HEIGHT 256

//#include "stdafx.h"
#include <string.h>
#include <string>
#include <conio.h>
#include <stdlib.h>
#include <malloc.h>

#include <windows.h>
#include <process.h>
#include <queue>

#include "VCECLB.h"

char Port = 0; // Camera port, by default set as the first
HANDLE DeviceID;
VCECLB_Configuration config;
std::wstring ConfigFileStr = L"CamConfig.cxf"; // Default config file name
const wchar_t *ConfigFile = ConfigFileStr.c_str();
unsigned char *externImg = NULL;
double *externXY = NULL;
bool imgReady = false;
bool stopGrabCall = false;
int threshMult = 5;
int minPix = 10;

struct SU320CSXimg
{
	SYSTEMTIME time;
	BYTE* Buffer = NULL;
	DWORD size;
} IRimg;

// Initializes IR camera, returns 0 if successful, -1 for failure.
extern "C" _declspec(dllexport) int initSU320CSX(char *_msg)
{	
	// Initialize FrameLink grabber
	DeviceID = VCECLB_Init();
	if (DeviceID == NULL)
	{
		sprintf(_msg, "No FrameLink Express card detected!");
		return -1;
	}
	
	// Acquire access to image acquisition on port
	if (VCECLB_GetDMAAccessEx(DeviceID, Port) != VCECLB_Err_Success)
	{
		sprintf(_msg, "Could not access port!");
		return -1;
	}
	
	ZeroMemory(&config, sizeof(config));

	TCHAR strManufacturer[MAX_PATH];
	ZeroMemory(strManufacturer, sizeof(strManufacturer));

	TCHAR strModel[MAX_PATH];
	ZeroMemory(strModel, sizeof(strModel));

	TCHAR strDescription[MAX_PATH];
	ZeroMemory(strDescription, sizeof(strDescription));

	TCHAR strAlias[MAX_PATH];
	ZeroMemory(strAlias, sizeof(strAlias));

	config.lpszManufacturer = strManufacturer;
	config.cchManufacturer = MAX_PATH;
	config.lpszModel = strModel;
	config.cchModel = MAX_PATH;
	config.lpszDescription = strDescription;
	config.cchDescription = MAX_PATH;
	config.lpszAlias = strAlias;
	config.cchAlias = MAX_PATH;
	
	
	// Load configuration file
	if (VCECLB_LoadConfig(ConfigFile, &config) != VCECLB_Err_Success)
	{
		sprintf(_msg, "Could not load configuration file!");
		return -1;
	}
	
	// Configure frame grabber with specified settings
	if (VCECLB_PrepareEx(DeviceID, Port, &config.pixelInfo.cameraData) != VCECLB_Err_Success)
	{
		sprintf(_msg, "Could not configure ExpressCard!");
		return -1;
	}
	/*
	unsigned long dwPortNumber = 6;
	if (VCECLB_UART_CreateSerialPortEx(DeviceID, Port, dwPortNumber) != VCECLB_Err_Success)
	{
		sprintf(_msg, "Could not create virtual serial port.");
	}
	*/
	return 0;
}

extern "C" _declspec(dllexport) void closeSU320CSX()
{
	if (DeviceID == NULL)
		return;

	// Release access to image acquisition on port
	VCECLB_ReleaseDMAAccessEx(DeviceID, Port);

	// Close handle to frame grabber
	VCECLB_Done(DeviceID);
	DeviceID = NULL;
}

static int CalThreshVal(unsigned short img[HEIGHT][WIDTH])
{
	double mean = 0, sigma = 0;
	int thresh = 0;

	for (int i = 0; i < HEIGHT; i++)
	{
		for (int j = 0; j < WIDTH; j++)
		{
			mean += img[i][j];
			sigma += img[i][j] * img[i][j];
		}
	}
	mean /= (double)HEIGHT*WIDTH;
	sigma /= (double)HEIGHT*WIDTH;
	sigma -= mean*mean;
	sigma = sqrt(sigma);

	thresh = (int) mean + threshMult * sigma;
	if (thresh > 4094)
		thresh = 4094; // one pixel before saturated
	return thresh;
}


static void FindCentroid(BYTE* imgBuf)
{
	// Convert buffer to a 2d pixel array
	unsigned short img[HEIGHT][WIDTH];
	for (int i = 0; i < HEIGHT; i++)
	{
		for (int j = 0; j < WIDTH; j++)
		{
			int ipixel = 0;
			int index = 2 * (j + i * WIDTH);
			ipixel = imgBuf[index] | imgBuf[1 + index] << 8; // convert two bytes to pixel value
			img[i][j] = (unsigned short)ipixel;
		}
	}

	// Get threshold value
	int thresh = CalThreshVal(img);
	int count = 0;
	double x = 0, y = 0, sum = 1e-6;
	for (int i = 0; i < HEIGHT; i++)
	{
		for (int j = 0; j < WIDTH; j++)
		{
			if (img[i][j] > thresh)
			{
				count++;
				y += (i + 0.5)*(img[i][j] - thresh);
				x += (j + 0.5)*(img[i][j] - thresh);
				sum += img[i][j] - thresh;
			}
		}
	}
	x /= sum;
	y /= sum;
	externXY[0] = x;
	externXY[1] = y;
	if (count < minPix)
	{
		externXY[0] = -1;
		externXY[1] = -1;
	}
}

static void WINAPI ContinuousGrab(LPVOID lpUserData, VCECLB_FrameInfoEx* pFrameInfo)
{
	if (pFrameInfo->dma_status != VCECLB_DMA_STATUS_OK)
		return;
	
	IRimg.Buffer = externImg;
	IRimg.size = pFrameInfo->bufferSize;
	memcpy(IRimg.Buffer, pFrameInfo->lpRawBuffer, pFrameInfo->bufferSize);
	GetLocalTime(&IRimg.time);
	FindCentroid(IRimg.Buffer);
	imgReady = true;
}


extern "C" _declspec(dllexport) int takeImg(char *_msg, unsigned char *img, double *xy)
{
	if (DeviceID == NULL)
		return -1;
	
	externImg = img;
	externXY = xy;
	if (VCECLB_StartGrabEx(DeviceID, Port, 0, (VCECLB_GrabFrame_CallbackEx)ContinuousGrab, NULL) != VCECLB_Err_Success)
	{
		sprintf(_msg, "Start grab failed!");
		return -1;
	}
	while (!imgReady)
	{ }
	VCECLB_StopGrabEx(DeviceID, Port);
	externImg = NULL;
	imgReady = false;
	return 0;
}

extern "C" _declspec(dllexport) int startGrab(char *_msg, unsigned char *img, double *xy)
{
	if (DeviceID == NULL)
		return -1;
	stopGrabCall = false;
	imgReady = true;
	externImg = img;
	externXY = xy;
	if (VCECLB_StartGrabEx(DeviceID, Port, 0, (VCECLB_GrabFrame_CallbackEx)ContinuousGrab, NULL) != VCECLB_Err_Success)
	{
		sprintf(_msg, "Start grab failed!");
		return -1;
	}

	while (!stopGrabCall)
	{ }
	VCECLB_StopGrabEx(DeviceID, Port);
	externImg = NULL;
	return 0;
}

extern "C" _declspec(dllexport) int stopGrab()
{
	stopGrabCall = true;
	return 0;
}
