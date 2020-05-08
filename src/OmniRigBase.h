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

#include <map>
#include <wtypes.h>
#include "ExitCodes.h"
#include "ProgramOptions.h"
#include "Utilities.h"

class OmniRigBase {
public:
	OmniRigBase();
	OmniRigBase(ProgramOptions &options);
	virtual ~OmniRigBase();
	virtual HRESULT sendCustomCommand(const std::string command);
	void setTime(const SYSTEMTIME currentDatetime);
	void setDate(const SYSTEMTIME currentDatetime);
	void setUtcOffset();
private:
	ProgramOptions* options = nullptr;
	std::string lookupCommand(std::string command, std::string data);
	const std::string preamble = "FEFE";
	const std::map<std::string, std::map<std::string, std::string>> commands = {
		{"7100", {{"setDateCommand", "1A050120"}, {"setTimeCommand", "1A050121"}, {"setUtcOffsetCommand", "1A050123"}}},
		{"7300", {{"setDateCommand", "1A050094"}, {"setTimeCommand", "1A050095"}, {"setUtcOffsetCommand", "1A050096"}}}
	};
	const std::string postamble = "FD";
};