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
}

bool ProgramOptions::isReversedTimeZone()
{
	return false;
}

int ProgramOptions::getRigNumber() {
	return 1;
}

std::string ProgramOptions::getTranceiverAddress() {
	return "94";
}

OmniRigVersion ProgramOptions::getOmnirigVersion() {
	return OmniRigVersion::OmniRigVersion1;
}

bool ProgramOptions::isQuiet() {
	return false;
}
