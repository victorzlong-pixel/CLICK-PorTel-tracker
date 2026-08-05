#ifndef __EXTERNFT232H_SPI
#define __EXTERNFT232H_SPI

extern "C" _declspec(dllexport) int initFT232H_SPI(char* _msg);
extern "C" _declspec(dllexport) int closeFT232H_SPI();

extern "C" _declspec(dllexport) void enableMEMSFSM(); 
extern "C" _declspec(dllexport) void setMEMSFSM_VxyDiff(double Vx, double Vy);
extern "C" _declspec(dllexport) void setMEMSFSM_VxDiff(double Vx);
extern "C" _declspec(dllexport) void setMEMSFSM_VyDiff(double Vy);
extern "C" _declspec(dllexport) void disableMEMSFSM();
extern "C" _declspec(dllexport) void setMEMSFSM_Origin();

extern "C" _declspec(dllexport) void setOIMFSM_Vxy(double Vx, double Vy);
extern "C" _declspec(dllexport) void setOIMFSM_Vx(double Vx);
extern "C" _declspec(dllexport) void setOIMFSM_Vy(double Vy);
extern "C" _declspec(dllexport) void setOIMFSM_Origin();

extern "C" _declspec(dllexport) void gotoFSMAll(double* V);
#endif
