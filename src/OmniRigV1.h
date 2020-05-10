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

#include "OmniRigBase.h"

#import "C:\Program Files (x86)\Afreet\OmniRig\OmniRig.exe"  rename_namespace("OmniRig1")

class OmniRigV1 : public OmniRigBase {
public:
	OmniRigV1(ProgramOptions &options);
	virtual ~OmniRigV1();
	HRESULT sendCustomCommand(const std::string command);
private:
	OmniRig1::IOmniRigX* pOmniRigX = nullptr;
	OmniRig1::IRigXPtr pRig = nullptr;
	void displayOmniRigInfo(OmniRig1::IOmniRigX* pOmniRigX);
	void displayRigInfo(OmniRig1::IOmniRigX* pOmniRigX);
};