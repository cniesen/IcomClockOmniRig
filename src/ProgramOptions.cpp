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
		} else if (arg == "-m") {
			if (++i >= argc) {
				std::cout << "ERROR: No Icom transceiver model specified\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_TRANSCEIVER_MODEL);
			}
			if (isValidTransceiverModel(argv[i])) {
				transceiverModel = argv[i];
			} else {
				std::cout << "ERROR: Invalid Icom transceiver model specified: " << argv[i] << "\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_TRANSCEIVER_MODEL);
			}
		} else if (arg == "-a") {
			if (++i >= argc) {
				std::cout << "ERROR: No Icom transceiver address specified\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_TRANSCEIVER_ADDRESS);
			}
			if (Utilities::isHex(argv[i])) {
				tranceiverAddress = argv[i];
			} else {
				std::cout << "ERROR: Invalid Icom transceiver address specified: " << argv[i] << "\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_TRANSCEIVER_ADDRESS);
			}
		} else if (arg == "-o") {
			if (++i >= argc) {
				std::cout << "ERROR: No OmniRig version specified\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_OMNIRIG_VERSION);
			}
			if (std::string(argv[i]) == "1") {
				omnirigVersion = OmniRigVersion::OmniRigVersion1;
			} else if (std::string(argv[i]) == "2") {
				omnirigVersion = OmniRigVersion::OmniRigVersion2;
			} else {
				std::cout << "ERROR: Invalid OmniRig version specified: " << argv[i] << "\n\n";
				printHelp(argv[0]);
				exit(E_OPTION_OMNIRIG_VERSION);
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
	if (!isQuiet()) {
		printProgramInfo();
		printOptions();
	}

}

void ProgramOptions::printProgramInfo() {
	std::cout
		<< " ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n"
		<< " ::                                                                          ::\n"
		<< " ::   IcomClockOmniRig 1.0  -  https://github.com/cniesen/IcomClockOmniRig   ::\n"
		<< " ::                                                                          ::\n"
		<< " ::    A program to set the Icom tranceiver clock to your computer's time    ::\n"
		<< " ::                                                                          ::\n"
		<< " ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n\n";
}

void ProgramOptions::printHelp(std::string programName) {
	printProgramInfo();
	std::cout
		<< "Usage: " << programName.substr(programName.find_last_of("/\\") + 1) << " [options]\n"
		<< "Options:\n"
		<< "\t-u\t\tReverse local and UTC time (show UTC as clock and local time as on UTC display)\n\n"
		<< "\t-r <number>\tThe selected rig in OmniRig (default: 1)\n\n"
		<< "\t-m <model>\tThe Icom transceiver model (default: IC-7300)\n"
		<< "\t\t\tValid models: " << listValidTransceiverModels() << "\n\n"
		<< "\t-a <hex>\tThe Icom transceiver address (default: 94)\n\n"
		<< "\t-o <number>\tOmniRig version number (default: 1)\n"
		<< "\t\t\t1 = original OmniRig by VE3NEA\n"
		<< "\t\t\t2 = updated OmniRig by HB9RYZ\n\n"
		<< "\t-q\t\tQuiet, don't output messages\n\n"
		<< "\t-h\t\tShow this help message\n\n";
}

void ProgramOptions::printOptions() {
	std::cout << "Program Options:\n"
		<< "    Using OmniRig version: " << ((omnirigVersion == OmniRigVersion::OmniRigVersion2) ? "2 (by HB9RYZ)" : "1 (by VE3NEA)") << "\n"
		<< "    Using OmniRig rig: " << rigNumber << "\n"
		<< "    Using Icom transceiver model: " << transceiverModel << "\n"
	    << "    Using Icom transceiver address: " << tranceiverAddress << "\n"
		<< "    Reverse clock and UTC time: " << (reversedTimeZone ? "yes" : "no") << "\n\n";
	
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

std::string ProgramOptions::getControllerAddress() {
	return controllerAddress;
}

std::string ProgramOptions::getTransceiverModel() {
	return transceiverModel;
}

OmniRigVersion ProgramOptions::getOmnirigVersion() {
	return omnirigVersion;
}

bool ProgramOptions::isQuiet() {
	return quiet;
}

bool ProgramOptions::isValidTransceiverModel(std::string transceiverModel) {
	auto transceiverCommands = commands.find(transceiverModel);
		if (transceiverCommands == commands.end()) {
			return false;
		} else {
			return true;
		}
}

std::string ProgramOptions::listValidTransceiverModels() {
    std::string transceiverModels = "";
	for (const auto& command : commands) {
		if (!transceiverModels.empty()) {
			transceiverModels.append(", ");
		}
		transceiverModels.append(command.first);
	}
	return transceiverModels;
}

std::string ProgramOptions::lookupCommand(const std::string command, const std::string data) {
	auto transceiverCommands = commands.find(getTransceiverModel());
	if (transceiverCommands == commands.end()) {
		exit(E_INTERNAL_COMMAND_MAP_TRANSCEIVER);
	}
	auto transceiverCommand = transceiverCommands->second.find(command);
	if (transceiverCommand == transceiverCommands->second.end()) {
		exit(E_INTERNAL_COMMAND_MAP_COMMAND);
	}
	return preamble + getTranceiverAddress() + getControllerAddress() + transceiverCommand->second + data + postamble;
}