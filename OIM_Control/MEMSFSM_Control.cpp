#include "MEMSFSM_Control.h"
#include "GPIO_Control.h"

static int PIN_ENABLE_STATUS = 0;
static FT_STATUS s_sta;

static void encodeData_MEMSFSM(unsigned char cmd, unsigned char add, double voltage, unsigned char code[])
{
	unsigned short v = (unsigned short)(26214.4 * voltage);
	code[0] = cmd | add;
	code[1] = (v >> 8) & 0xFF;
	code[2] = v & 0xFF;

	return;
}

static void sendMEMSFSMcmd(FT_HANDLE fth, unsigned char code[])
{
	uint32 sizeTransferred;
	setGPIOLow(fth, PIN_CS_MEMS);
	s_sta = SPI_Write(fth, (uint8*)(code), 3, &sizeTransferred, SPI_TRANSFER_OPTIONS_SIZE_IN_BYTES);
	setGPIOHigh(fth, PIN_CS_MEMS);
}

static void sendMEMSFSMcmd_ui(FT_HANDLE fth, unsigned int cmd)
{
	unsigned char code[3];
	code[0] = (cmd >> 16) & 0xFF;
	code[1] = (cmd >> 8) & 0xFF;
	code[2] = cmd & 0xFF;

	sendMEMSFSMcmd(fth, code);
}

void initMEMSFSM(FT_HANDLE fth)
{
	// IO pin setup
	setGPIOHigh(fth, PIN_CS_MEMS);
	setGPIOLow(fth, PIN_HIGH_V_ENABLE);
	PIN_ENABLE_STATUS = 0;

	// Send Initialize command
	sendMEMSFSMcmd_ui(fth, FULL_RESET);
	sendMEMSFSMcmd_ui(fth, ENABLE_INTERNAL_REFERENCE);
	sendMEMSFSMcmd_ui(fth, ENABLE_ALL_DAC_CHANNELS);
	sendMEMSFSMcmd_ui(fth, ENABLE_SOFTWARE_LDAC);
	setMEMSFSM_Origin(fth);

	return;
}

static void setMEMSFSMVoltage(FT_HANDLE fth, double V[])
{
	unsigned char code[3];
	encodeData_MEMSFSM(CMD_WRITE_INPUT_REG, ADDR_XP, V[0], code);
	sendMEMSFSMcmd(fth, code);

	encodeData_MEMSFSM(CMD_WRITE_INPUT_REG, ADDR_XM, V[1], code);
	sendMEMSFSMcmd(fth, code);

	encodeData_MEMSFSM(CMD_WRITE_INPUT_REG, ADDR_YP, V[2], code);
	sendMEMSFSMcmd(fth, code);

	encodeData_MEMSFSM(CMD_WRITE_INPUT_UPDATE_ALL, ADDR_YM, V[3], code);
	sendMEMSFSMcmd(fth, code);
	return;
}

static void setMEMSFSM_Voltage_X(FT_HANDLE fth, double V[])
{
	unsigned char code[3];
	encodeData_MEMSFSM(CMD_WRITE_INPUT_REG, ADDR_XP, V[0], code);
	sendMEMSFSMcmd(fth, code);

	encodeData_MEMSFSM(CMD_WRITE_INPUT_UPDATE_ALL, ADDR_XM, V[1], code);
	sendMEMSFSMcmd(fth, code);

	return;
}

static void setMEMSFSM_Voltage_Y(FT_HANDLE fth, double V[])
{
	unsigned char code[3];
	encodeData_MEMSFSM(CMD_WRITE_INPUT_REG, ADDR_YP, V[0], code);
	sendMEMSFSMcmd(fth, code);

	encodeData_MEMSFSM(CMD_WRITE_INPUT_UPDATE_ALL, ADDR_YM, V[1], code);
	sendMEMSFSMcmd(fth, code);

	return;
}


void setMEMSFSM_Origin(FT_HANDLE fth)
{
	double V[4];
	V[0] = V[1] = V[2] = V[3] = 1.25;
	setMEMSFSMVoltage(fth, V);
	return;
}

void enableMEMSFSM(FT_HANDLE fth)
{
	if (PIN_ENABLE_STATUS == 0)
	{
		setMEMSFSM_Origin(fth);
		setGPIOHigh(fth, PIN_HIGH_V_ENABLE);
		PIN_ENABLE_STATUS = 1;
	}
}

void disableMEMSFSM(FT_HANDLE fth)
{
	setMEMSFSM_Origin(fth);
	setGPIOLow(fth, PIN_HIGH_V_ENABLE);
	PIN_ENABLE_STATUS = 0;
}

void setMEMSFSM_VxyDiff_LOW(FT_HANDLE fth, double Vx, double Vy)
{
	double V[4];
	double Vx2, Vy2;
	Vx2 = Vx * 0.5;
	Vy2 = Vy * 0.5;
	V[0] = 1.25 + Vx2;
	V[1] = 1.25 - Vx2;
	V[2] = 1.25 + Vy2;
	V[3] = 1.25 - Vy2;
	setMEMSFSMVoltage(fth, V);
	return;
}

void setMEMSFSM_VxDiff_LOW(FT_HANDLE fth, double Vdiff)
{
	double V[2];
	double V2;
	V2 = Vdiff * 0.5f;

	V[0] = 1.25f + V2;
	V[1] = 1.25f - V2;

	setMEMSFSM_Voltage_X(fth, V);
	return;
}

void setMEMSFSM_VyDiff_LOW(FT_HANDLE fth, double Vdiff)
{
	double V[2];
	double V2;
	V2 = Vdiff * 0.5f;

	V[0] = 1.25f + V2;
	V[1] = 1.25f - V2;

	setMEMSFSM_Voltage_Y(fth, V);
	return;
}

