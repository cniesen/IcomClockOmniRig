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

#include "OmniRigBase.h"

OmniRigBase::OmniRigBase() {

}

OmniRigBase::OmniRigBase(ProgramOptions &o) {
	options = &o;
}

OmniRigBase::~OmniRigBase()
{
}

HRESULT OmniRigBase::sendCustomCommand(const std::string command)
{
    std::cout << "Sending command: " << command << std::endl;
	return E_NOTIMPL;
}

void OmniRigBase::setTime(const SYSTEMTIME currentDatetime) {
	std::string timeData = Utilities::zeroPad(currentDatetime.wHour, 2) + Utilities::zeroPad(currentDatetime.wMinute, 2);
	HRESULT hr = sendCustomCommand(options->lookupCommand("setTimeCommand", timeData));
	if (FAILED(hr))
		exit(E_INTERNAL_OMNIRIG_SET_TIME);
}

void OmniRigBase::setDate(const SYSTEMTIME currentDatetime) {
	std::string dateData = Utilities::zeroPad(currentDatetime.wYear, 4) + Utilities::zeroPad(currentDatetime.wMonth, 2) + Utilities::zeroPad(currentDatetime.wDay, 2);
	HRESULT hr = sendCustomCommand(options->lookupCommand("setDateCommand", dateData));
	if (FAILED(hr))
		exit(E_INTERNAL_OMNIRIG_SET_DATE);
}

void OmniRigBase::setUtcOffset() {
	TIME_ZONE_INFORMATION timeZoneInformation;
	GetTimeZoneInformation(&timeZoneInformation);
	long offset = timeZoneInformation.Bias + timeZoneInformation.DaylightBias;
	std::string offsetData = "";

	if (offset < 0) {
		offset = offset * -1;
		long hours = offset / 60;
		long minutes = offset % 60;
		offsetData += std::string(2 - std::to_string(hours).length(), '0') + std::to_string(hours);
		offsetData += std::string(2 - std::to_string(minutes).length(), '0') + std::to_string(minutes);
		if (options->isReversedTimeZone()) {
			offsetData += "00";
		}
		else {
			offsetData += "01";
		}
	}
	else {
		long hours = offset / 60;
		long minutes = offset % 60;
		offsetData += std::string(2 - std::to_string(hours).length(), '0') + std::to_string(hours);
		offsetData += std::string(2 - std::to_string(minutes).length(), '0') + std::to_string(minutes);
		if (options->isReversedTimeZone()) {
			offsetData += "00";
		}
		else {
			offsetData += "01";
		}
	}

	HRESULT hr = sendCustomCommand(options->lookupCommand("setUtcOffsetCommand", offsetData));
	if (FAILED(hr))
		exit(E_INTERNAL_OMNIRIG_SET_OFFSET);
}