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

#import "C:\Program Files (x86)\Omni-Rig V2\omnirig2.exe"
using namespace OmniRig;

class OmniRigV2 : public OmniRigBase {
public:
	OmniRigV2(ProgramOptions options);
	virtual ~OmniRigV2();
	HRESULT sendCustomCommand(const char* command);
private:
	IOmniRigX* pOmniRigX = nullptr;
	IRigXPtr pRig = nullptr;
	void displayOmniRigInfo(IOmniRigX* pOmniRigX);
	void displayRigInfo(IOmniRigX* pOmniRigX);
};