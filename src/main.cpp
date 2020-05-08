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

#include <time.h>
#include "ExitCodes.h"
#include "OmniRigV1.h"
#include "OmniRigV2.h"
#include "ProgramOptions.h"

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

	auto pGetTime = GetLocalTime;
	if (options.isReversedTimeZone()) {
		pGetTime = GetSystemTime;
	}
	
	if (!options.isQuiet()) {
		std::cout << "Waiting for the full minute to set the time" << std::endl;
	}
	SYSTEMTIME currentDatetime;
	pGetTime(&currentDatetime);
	while (currentDatetime.wSecond != 0) {
		Sleep(100);
		pGetTime(&currentDatetime);
	} 

	omnirig->setTime(currentDatetime);
	omnirig->setDate(currentDatetime);
	omnirig->setUtcOffset();

	exit(E_SUCCESS);
}