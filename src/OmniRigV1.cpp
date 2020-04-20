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

#include "OmniRigV1.h"

OmniRigV1::OmniRigV1(ProgramOptions options) {
	// Establish OmniRig COM connection
	HRESULT hr = CoInitialize(nullptr);
	if (FAILED(hr))
		exit(E_OMNIRIG_COM_INIT);

	hr = CoCreateInstance(
		__uuidof(OmniRigX),
		nullptr,
		CLSCTX_LOCAL_SERVER,
		__uuidof(IOmniRigX),
		reinterpret_cast<void**>(&pOmniRigX)

	);
	if (FAILED(hr))
		exit(E_OMNIRIG_COM_CREATE);

	if (!options.isQuiet()) {
		displayOmniRigInfo(pOmniRigX);
		displayRigInfo(pOmniRigX);
	}


	switch (options.getRigNumber()) {
		case 1:
			pRig = pOmniRigX->GetRig1();
			break;
		case 2:
			pRig = pOmniRigX->GetRig2();
			break;
		default:
			exit(E_OPTION_RIG_NUMBER);
	}

	RigStatusX rigStatus = pRig->GetStatus();
	switch (rigStatus) {
		case ST_NOTCONFIGURED:
			exit(E_OMNIRIG_STATUS_NOTCONFIGURED);
		case ST_DISABLED:
			exit(E_OMNIRIG_STATUS_DISABLED);
		case ST_PORTBUSY:
			exit(E_OMNIRIG_STATUS_PORTBUSY);
		case ST_NOTRESPONDING:
			exit(E_OMNIRIG_STATUS_NOTRESPONDING);
		case ST_ONLINE:
			break;
		default:
			exit(E_OMNIRIG_STATUS_UNKNOWN);
	}
}

OmniRigV1::~OmniRigV1() {
	// Terminate OmniRig COM connection
	pOmniRigX->Release();
	CoUninitialize();
}

void OmniRigV1::displayOmniRigInfo(IOmniRigX* pOmniRigX) {
	printf("OmniRig Software Version:  %d.%d\n", HIWORD(pOmniRigX->GetSoftwareVersion()), LOWORD(pOmniRigX->GetSoftwareVersion()));
	printf("OmniRig Interface Version: %d.%d\n", HIBYTE(pOmniRigX->GetInterfaceVersion()), LOBYTE(pOmniRigX->GetInterfaceVersion()));
}

void OmniRigV1::displayRigInfo(IOmniRigX* pOmniRigX) {
	IRigXPtr pRig1 = pOmniRigX->GetRig1();
	printf("Rig 1\n");
	printf("    Rig Type: %s\n", _com_util::ConvertBSTRToString(pRig1->GetRigType()));
	printf("    Status:   %s\n", _com_util::ConvertBSTRToString(pRig1->GetStatusStr()));
	IRigXPtr pRig2 = pOmniRigX->GetRig2();
	printf("Rig 2\n");
	printf("    Rig Type: %s\n", _com_util::ConvertBSTRToString(pRig2->GetRigType()));
	printf("    Status:   %s\n", _com_util::ConvertBSTRToString(pRig2->GetStatusStr()));
}

HRESULT OmniRigV1::sendCustomCommand(const char* command) {
	int commandLength = strlen(command) / 2;
	int responseOkLength = 6;

	byte bCommand[11] = { 0 };
	Utilities::hex2byte(command, bCommand);

	SAFEARRAYBOUND saBound;
	saBound.lLbound = 0;
	saBound.cElements = commandLength;

	SAFEARRAY* psa = SafeArrayCreate(VARENUM::VT_UI1, 1, &saBound);
	if (psa == nullptr)
		exit(E_INTERNAL_SAFEARRAY_CREATE);

	HRESULT hr = SafeArrayLock(psa);
	if (FAILED(hr))
		exit(E_INTERNAL_SAFEARRAY_LOCK);
	psa->pvData = bCommand;
	hr = SafeArrayUnlock(psa);
	if (FAILED(hr))
		exit(E_INTERNAL_SAFEARRAY_UNLOCK);

	VARIANT vtCommand;
	vtCommand.vt = VT_ARRAY | VT_UI1;
	vtCommand.parray = psa;
	return pRig->SendCustomCommand(vtCommand, commandLength + responseOkLength, "");
}