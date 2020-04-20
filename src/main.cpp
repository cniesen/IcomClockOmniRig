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

#include <sstream>
#include <time.h>
#include "ExitCodes.h"
#include "OmniRigV1.h"
#include "OmniRigV2.h"
#include "ProgramOptions.h"

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


int main(int argc, char* argv[])
{ 
	ProgramOptions options(argc, argv);
	OmniRigBase* omnirig;
	switch (options.getOmnirigVersion()) {
		case OmniRigVersion::OmniRigVersion1:
			omnirig = new OmniRigV1(options);
			break;
		case OmniRigVersion::OmniRigVersion2:
			omnirig = new OmniRigV2(options);
			break;
		default:
			exit(E_OPTION_OMNIRIG_VERSION);
	}

	
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

	HRESULT hr = omnirig->sendCustomCommand("FEFE94E01A0500951111FD");
	if (FAILED(hr))
		exit(E_INTERNAL_OMNIRIG_CUSTOMCOMMAND);

	std::cout << "Done\n";
	exit(E_SUCCESS);
}