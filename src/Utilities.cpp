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

#include "Utilities.h"

byte Utilities::char2byte(char input)
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
void Utilities::hex2byte(const char* src, byte* target)
{
	while (*src && src[1])
	{
		*(target++) = char2byte(*src) * 16 + char2byte(src[1]);
		src += 2;
	}
}

bool Utilities::isDigit(const char* digit) {
    return std::regex_match(std::string(digit), std::regex("[0-9]"));
}

bool Utilities::isHex(const char* hex) {
	return std::regex_match(std::string(hex), std::regex("[a-fA-F0-9]{2}"));
}