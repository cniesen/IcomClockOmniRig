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

#pragma once

#include <iostream>
#include <map>
#include "ExitCodes.h"
#include "Utilities.h"

enum class OmniRigVersion {
	OmniRigVersion1,  // original OmniRig by VE3NEA
	OmniRigVersion2   // updated OmniRig by HB9RYZ
};

class ProgramOptions {
public:
	ProgramOptions(int argc, char* argv[]);
	bool isReversedTimeZone();
	int getRigNumber();
	std::string getTranceiverAddress();
	std::string getControllerAddress();
    std::string getTransceiverModel();
	OmniRigVersion getOmnirigVersion();
	bool isQuiet();
	std::string lookupCommand(std::string command, std::string data);
private:
	bool reversedTimeZone = false;
	int rigNumber = 1;
	std::string tranceiverAddress = "94";
	std::string controllerAddress = "E0";
	std::string transceiverModel = "IC-7300";
	OmniRigVersion omnirigVersion = OmniRigVersion::OmniRigVersion1;
	bool quiet = false;
	void printProgramInfo();
	void printHelp(std::string programName);
	void printOptions();
	bool isValidTransceiverModel(std::string transceiverModel);
	std::string listValidTransceiverModels();
	const std::string preamble = "FEFE";
	const std::map<std::string, std::map<std::string, std::string>> commands = {
		{"IC-7100", {{"setDateCommand", "1A050120"}, {"setTimeCommand", "1A050121"}, {"setUtcOffsetCommand", "1A050123"}}},
		{"IC-7300", {{"setDateCommand", "1A050094"}, {"setTimeCommand", "1A050095"}, {"setUtcOffsetCommand", "1A050096"}}}
	};
	const std::string postamble = "FD";
};