# CLICK-PorTel-tracker
CLICK PorTeL Optical Ground Station Software

STRUCTS Name, where defined, description StarImg, StarImg.cs OnePixel, StarImg.cs OneStar, StarImg.cs

StarID.cs - Does the actual star ID using external calls to StarID_C_dll, sets value in StarImg list of OneStars

StarImg.cs - StarImg struct - Defines image, implements thresholding, grouping, and centroiding

FSM Library:

Seems to prefer 32 bit compiler (PorTeL.sln) though might be possible to do 64 bit
requires LibFT4222.dll from FT4222 library to be in bin folder
iNova Library:

Seems to only work if the C# build platform target is "Prefer 32-bit" (PorTeL.sln)
Reference the i-NovaSDK.dll from C:\Windows\Microsoft.Net\assembly\GAC_MSIL\i-NovaSDK\v4.0_1.2.2.0__5f571db83fcccd6e\i-NovaSDK.dll
IR Camera:

Works with 32 bit (PorTeL.sln). Also seems to work with 64 bit (GS_Tracking_KR.sln); however, mixing 32 bit and 64 bit doesn't work at all
Needs CamConfig.cfx file in the bin folder
Needs Imperx/FrameExpress includes: VCECLB.dll, ippLib.dll, ipxdemosaicing.dll, IpxTrueSense.dll
For PorTeL.sln, "Take Img" freezes the program whereas "Start Grab" works fine
For GS_Tracking_KR.sln, "Take Img" works fine. "Start Grab" button runs, but "Stop Grab" doesn't come up...
**Current Best Working: PorTeL.sln - everything is seemingly working, but "Take Img" freezes program
