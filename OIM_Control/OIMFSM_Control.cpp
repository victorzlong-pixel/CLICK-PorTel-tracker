#include "OIMFSM_Control.h"
#include "GPIO_Control.h"

static FT_STATUS s_sta;

void initOIMFSM(FT_HANDLE fth)
{
	setGPIOLow(fth, PIN_CS_OIMX);
	setGPIOLow(fth, PIN_CS_OIMY);
	setOIMFSM_Origin(fth);
	return;
}


int sendCmd_OIM(FT_HANDLE fth, unsigned short value, uint8 pin)
{
	uint32 sizeTransferred;
	s_sta = SPI_Write(fth, (uint8*)(&value), 2, &sizeTransferred, SPI_TRANSFER_OPTIONS_SIZE_IN_BYTES);
	
	setGPIOHigh(fth, pin);
	setGPIOLow(fth, pin);
	return 0;
}

unsigned short setOIMFSM_V(FT_HANDLE fth, double v, uint8 CSpin)
{
	double v1;
	unsigned short us;

	if (v >= 10)
		us = 0xFFFF;
	else if (v < -10)
		us = 0;
	else
	{
		v1 = v + 10;
		us = (unsigned short)(v1 * 3276.8);
	}
	sendCmd_OIM(fth, us, CSpin);
	return us;
}

void setOIMFSM_Vxy(FT_HANDLE fth, double Vx, double Vy)
{
	setOIMFSM_V(fth, Vx, PIN_CS_OIMX);
	setOIMFSM_V(fth, Vy, PIN_CS_OIMY);
}

void setOIMFSM_Origin(FT_HANDLE fth)
{
	setOIMFSM_V(fth, 0, PIN_CS_OIMX);
	setOIMFSM_V(fth, 0, PIN_CS_OIMY);
}
