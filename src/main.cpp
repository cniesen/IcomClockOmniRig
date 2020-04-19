/*
	IcomClockOmniRig
	Copyright (C) 2020  Claus Niesen

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

// OmniRig version to be supported:
//  1 = original VE3NEA version
//  2 = updated HB9RYZ version
#define OMNI_RIG_VERSION  1  

#include <iostream>
#include <sstream>
#include <string>
#include <time.h>
#include <wtypes.h>
#include "ExitCodes.h"

#include "OmniRigEventHandler2.h"
using namespace OmniRigEventHandlerNamespace;

#if OMNI_RIG_VERSION == 2
	//#import "C:\Program Files (x86)\Omni-Rig V2\omnirig2.exe"
	//using namespace OmniRig;
#else
	#import "C:\Program Files (x86)\Afreet\OmniRig\OmniRig.exe"
	using namespace OmniRig;
#endif

int offset(const SYSTEMTIME& utc, const SYSTEMTIME& local) {
	FILETIME uft, lft;

	SystemTimeToFileTime(&utc, &uft);
	SystemTimeToFileTime(&local, &lft);

	return ULARGE_INTEGER{ lft.dwLowDateTime, lft.dwHighDateTime }.QuadPart - ULARGE_INTEGER{ uft.dwLowDateTime, uft.dwHighDateTime }.QuadPart;
}

template<class T>
std::string toString(const T& value) {
	std::ostringstream os;
	os << value;
	return os.str();
}

byte char2byte(char input)
{
	if (input >= '0' && input <= '9')
		return input - '0';
	if (input >= 'A' && input <= 'F')
		return input - 'A' + 10;
	if (input >= 'a' && input <= 'f')
		return input - 'a' + 10;
	throw std::invalid_argument("Invalid input string");
}

// This function assumes src to be a zero terminated sanitized string with
// an even number of [0-9a-f] characters, and target to be sufficiently large
void hex2byte(const char* src, byte* target)
{
	while (*src && src[1])
	{
		*(target++) = char2byte(*src) * 16 + char2byte(src[1]);
		src += 2;
	}
}

HRESULT ParamsChange(long rigNumber, long params) {
	std::string strCommand, strReply;

	std::cout << params << " params have changed for rig " << rigNumber<<  "\n";

	return 0;
}

HRESULT customReply(long RigNumber, const _variant_t& Command, const _variant_t& Reply) {
	std::string strCommand, strReply;


	if (Command.vt == VT_BSTR)
		strCommand = _bstr_t(Command.bstrVal);
	else
		strCommand = "???";
	if (Reply.vt == VT_BSTR)
		strReply = _bstr_t(Reply.bstrVal);
	else
		strReply = "???";

	std::cout << "Custom command response for: " << strCommand << " is " << strReply << "\n";

	return 0;
}


int main()
{ 
	// Establish OmniRig COM connection
	HRESULT hr = CoInitialize(nullptr);
	if (FAILED(hr))
		exit(E_OMNIRIG_COM_INIT);
	
	IOmniRigX* pOmniRigX = nullptr;

	hr = CoCreateInstance(
		__uuidof(OmniRigX),
		nullptr,
		CLSCTX_LOCAL_SERVER,
		__uuidof(IOmniRigX),
		reinterpret_cast<void**>(&pOmniRigX)
	);
	if (FAILED(hr))
		exit(E_OMNIRIG_COM_CREATE);

	// Display OmniRig Version
	printf("OmniRig Software Version:  %d.%d\n", HIWORD(pOmniRigX->GetSoftwareVersion()), LOWORD(pOmniRigX->GetSoftwareVersion()));
	printf("OmniRig Interface Version: %d.%d\n", HIBYTE(pOmniRigX->GetInterfaceVersion()), LOBYTE(pOmniRigX->GetInterfaceVersion()));

	// Display Rig Status
	IRigXPtr pRig1 = pOmniRigX->GetRig1();
	printf("Rig 1\n");
	printf("    Rig Type: %s\n", _com_util::ConvertBSTRToString(pRig1->GetRigType()));
	printf("    Status:   %s\n", _com_util::ConvertBSTRToString(pRig1->GetStatusStr()));
	IRigXPtr pRig2 = pOmniRigX->GetRig2();
	printf("Rig 2\n");
	printf("    Rig Type: %s\n", _com_util::ConvertBSTRToString(pRig2->GetRigType()));
	printf("    Status:   %s\n", _com_util::ConvertBSTRToString(pRig2->GetStatusStr()));
#if OMNI_RIG_VERSION == 2
	IRigXPtr pRig3 = pOmniRigX->GetRig3();
	printf("Rig 3\n");
	printf("    Rig Type: %s\n", _com_util::ConvertBSTRToString(pRig3->GetRigType()));
	printf("    Status:   %s\n", _com_util::ConvertBSTRToString(pRig3->GetStatusStr()));
	IRigXPtr pRig4 = pOmniRigX->GetRig4();
	printf("Rig 4\n");
	printf("    Rig Type: %s\n", _com_util::ConvertBSTRToString(pRig4->GetRigType()));
	printf("    Status:   %s\n", _com_util::ConvertBSTRToString(pRig4->GetStatusStr()));
#endif

	// References:
	//     https://www.codeproject.com/Articles/9014/Understanding-COM-Event-Handling
	//    https://books.google.com/books?id=kfRWvKSePmAC&pg=PA362&lpg=PA362&dq=IConnectionPointContainer*+pCPC;IConnectionPoint*+pCP;&source=bl&ots=o9iWj6dOhq&sig=ACfU3U2qBmLy2RRrhv5rRnA1hr59p5XRoA&hl=en&sa=X&ved=2ahUKEwiJwJvUrenoAhVVXM0KHS4JAaYQ6AEwAnoECAsQKg#v=onepage&q=IConnectionPointContainer*%20pCPC%3BIConnectionPoint*%20pCP%3B&f=false

	// Establish COM connection for custom reply events
	IConnectionPointContainer* pCPC;
	IConnectionPoint* pCP;
	OmniRigEventHandler* pSink = new OmniRigEventHandler();
	DWORD dwAdvise;

	hr = pOmniRigX->QueryInterface(IID_IConnectionPointContainer, (void**)&pCPC);
	if (FAILED(hr))
		exit(E_OMNIRIG_COM_OBTRAIN_CALLBACK_CONTAINTER_POINTER);
	hr = pCPC->FindConnectionPoint(__uuidof(IOmniRigXEvents), &pCP);
	if (FAILED(hr))
		exit(E_OMNIRIG_COM_OBTAIN_CALLBACK_REGISTRATION_POINTER);
	pCPC->Release();
	hr = pCP->QueryInterface(IID_IUnknown, (void**)&pSink); //--wrong
		if (FAILED(hr))
			exit(E_OMNIRIG_OBTAIN_OUR_CALLBACK_FUNCTION_POINTER);
	hr = pCP->Advise(pSink, &dwAdvise);
	if (FAILED(hr))
		exit(E_OMNIRIG_COM_REGISTER_CALLBACK_FUNCTION);

	IRigXPtr pRig = pOmniRigX->GetRig1();
	RigStatusX rigStatus = pRig->GetStatus();
	switch (rigStatus) {
	    case ST_NOTCONFIGURED:
			exit(E_OMNIRIG_STATUS_NOTCONFIGURED);
		case ST_DISABLED:
			exit(E_OMNIRIG_STATUS_DISABLED);
		case ST_PORTBUSY:
			exit(E_OMNIRIG_STATUS_PORTBUSY);
		case ST_NOTRESPONDING:
			exit(E_OMNIRIG_STATUS_NOTRESPONDING);
		case ST_ONLINE:
			break;
		default:
			exit(E_OMNIRIG_STATUS_UNKNOWN);
	}
	
	// hack to let omnirig connect with radio
	Sleep(100);
	
	long frequency = pRig->GetFreqA();
    std::cout << "Frequency A: " << frequency << "\n";
	
	SYSTEMTIME st, lt;

	GetSystemTime(&st);
	GetLocalTime(&lt);

	printf("The system time is: %04d-%02d-%02d %02d:%02d:%02d\n", st.wYear, st.wMonth, st.wDay, st.wHour, st.wMinute, st.wSecond);
	printf(" The local time is: %04d-%02d-%02d %02d:%02d:%02d\n", lt.wYear, lt.wMonth, lt.wDay, lt.wHour, lt.wMinute, lt.wSecond);

	
	TIME_ZONE_INFORMATION timeZoneInformation;
	GetTimeZoneInformation(&timeZoneInformation);
	long offset = timeZoneInformation.Bias + timeZoneInformation.DaylightBias;
	std::string offsetData = "";
	

	if (offset < 0) {
		offset = offset * -1;
		long hours = offset / 60;
		long minutes = offset % 60;
		offsetData += std::string(2 - toString(hours).length(), '0') + toString(hours);
		offsetData += std::string(2 - toString(minutes).length(), '0') + toString(minutes);
		offsetData += "00";
	}
	else {
		long hours = offset / 60;
		long minutes = offset % 60;
		offsetData += std::string(2 - toString(hours).length(), '0') + toString(hours);
		offsetData += std::string(2 - toString(minutes).length(), '0') + toString(minutes);
		offsetData += "01";
	}
	std::cout << "Offset: " << offsetData << "\n";

	//IOmniRigXEvents event;
	byte command[11] = { 0 };
	byte response[6] = { 0 };
	hex2byte("FEFE94E01A0500951111FD", command);
	hex2byte("FEFEE094FBFD", response);

	hr = pRig->SendCustomCommand(command, 6, response);
	//hr = pRig->CustomReply()

	frequency = pRig->GetFreqA();
	std::cout << "Frequency A: " << frequency << "\n";

	// Terminate COM connection for custom reply events
	pCP->Unadvise(dwAdvise);
	pCP->Release();
	
	// Terminate OmniRig COM connection
	pOmniRigX->Release();
	CoUninitialize();

	std::cout << "Done\n";
	exit(E_SUCCESS);
}