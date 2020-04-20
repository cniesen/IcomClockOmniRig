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

#include "ProgramOptions.h"

ProgramOptions::ProgramOptions(int argc, char* argv[]) {
	for (int i = 1; i < argc; ++i) {
		std::string arg = argv[i];
		if (arg == "-h") {
			printHelp(argv[0]);
			exit(E_SUCCESS);
		} else if (arg == "-u") {
			reversedTimeZone = true;
		} else if (arg == "-r") {
			if (++i >= argc) {
				std::cout << "ERROR: No rig number specified\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_RIG_NUMBER);
			}
			if (Utilities::isDigit(argv[i])) {
				rigNumber = *argv[i] - 48;
			} else {
				std::cout << "ERROR: Invalid rig number specified: " << argv[i] << "\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_RIG_NUMBER);
			}
		} else if (arg == "-a") {
			if (++i >= argc) {
				std::cout << "ERROR: No Icom tranceiver address specified\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_RIG_NUMBER);
			}
			if (Utilities::isHex(argv[i])) {
				tranceiverAddress = argv[i];
			} else {
				std::cout << "ERROR: Invalid Icom tranceiver address specified: " << argv[i] << "\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_RIG_NUMBER);
			}
		} else if (arg == "-o") {
			if (++i >= argc) {
				std::cout << "ERROR: No OmniRig version specified\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_RIG_NUMBER);
			}
			if (std::string(argv[i]) == "1") {
				omnirigVersion = OmniRigVersion::OmniRigVersion1;
			} else if (std::string(argv[i]) == "2") {
				omnirigVersion = OmniRigVersion::OmniRigVersion2;
			} else {
				std::cout << "ERROR: Invalid OmniRig version specified: " << argv[i] << "\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_RIG_NUMBER);
			}
		} else if (arg == "-q") {
			quiet = true;
		} else {
			std::cout << "ERROR: Invalid option specified: " << arg << "\n\n";
			printHelp(argv[0]);
			exit(E_OPTION_INVALID);
		}
	}

	switch (omnirigVersion) {
	case OmniRigVersion::OmniRigVersion1:
		if (rigNumber < 1 || rigNumber > 2) {
			std::cout << "ERROR: Invalid rig number specified: " << rigNumber << "\n\n";
			printHelp(argv[0]);
			exit(E_OPTION_RIG_NUMBER);
		}
		break;
	case OmniRigVersion::OmniRigVersion2:
		if (rigNumber < 1 || rigNumber > 4) {
			std::cout << "ERROR: Invalid rig number specified: " << rigNumber << "\n\n";
			printHelp(argv[0]);
			exit(E_OPTION_RIG_NUMBER);
		}
		break;
	default:
		exit(E_OPTION_OMNIRIG_VERSION);
	}
}

void ProgramOptions::printHelp(std::string programName) {
	std::cout 
		<< "IcomClockOmniRig version 1.0 - set the Icom clock to your computer's clock\n"
		<< "\thttps://github.com/cniesen/IcomClockOmniRig\n\n"
		<< "Usage: " << programName.substr(programName.find_last_of("/\\") + 1) << " [options]\n"
		<< "Options:\n"
		<< "\t-u\t\tReverse local and UTC time (show UTC as clock and local time as on UTC display)\n\n"
		<< "\t-r <number>\tThe selected rig in OmniRig (default: 1)\n\n"
		<< "\t-a <hex>\tThe Icom tranceiver address (default: 94)\n\n"
		<< "\t-o <number>\tOmniRig version number (default: 1)\n\n"
		<< "\t\t\t1 = original OmniRig by VE3NEA\n\n"
		<< "\t\t\t2 = updated OmniRig by HB9RYZ\n\n"
		<< "\t-q\t\tQuiet, don't output messages\n\n"
		<< "\t-h\t\tShow this help message\n\n";
}

bool ProgramOptions::isReversedTimeZone()
{
	return reversedTimeZone;
}

int ProgramOptions::getRigNumber() {
	return rigNumber;
}

std::string ProgramOptions::getTranceiverAddress() {
	return tranceiverAddress;
}

OmniRigVersion ProgramOptions::getOmnirigVersion() {
	return omnirigVersion;
}

bool ProgramOptions::isQuiet() {
	return quiet;
}