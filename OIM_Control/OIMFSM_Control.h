#ifndef __OIMFSM_CONTROL
#define __OIMFSM_CONTROL

#include <Windows.h>
#include "ftd2xx.h"
#include "libMPSSE_spi.h"
#include "PinConfig.h"

void initOIMFSM(FT_HANDLE fth);
int sendCmd_OIM(FT_HANDLE fth, unsigned short value, uint8 pin);
unsigned short setOIMFSM_V(FT_HANDLE fth, double v, uint8 CSpin);
void setOIMFSM_Vxy(FT_HANDLE fth, double Vx, double Vy);
void setOIMFSM_Origin(FT_HANDLE fth);

#endif

