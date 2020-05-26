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

using OmniRig1;
using System;
using System.Runtime.InteropServices;

namespace IcomClockOmniRig {
    class OmniRigV1 : OmniRigBase {

        private readonly OmniRigX OmniRig;
        private IRigX Rig = null;

        public OmniRigV1(ProgramOptions programOptions) : base(programOptions) {
            try {
                OmniRig = new OmniRigX();
            } catch (COMException e) {
                throw new ExitException(ExitCode.OMNIRIG_COM_INIT, "Error: OmniRig not found (Is OmniRig installed?)", e);
            }
            OmniRig = new OmniRigX();
            OmniRig.CustomReply += OmniRig_CustomReply;
        }

        protected override string SoftwareVersion() {
            return Utilities.HIWORD(OmniRig.SoftwareVersion) + "." + Utilities.LOWORD(OmniRig.SoftwareVersion);
        }

        protected override string InterfaceVersion() {
            return Utilities.HIBYTE(OmniRig.InterfaceVersion) + "." + Utilities.LOBYTE(OmniRig.InterfaceVersion);
        }

        protected override (string rigType, string rigStatus) RigInfo(int rigNumber) {
            switch (rigNumber) {
                case 1:
                    return (OmniRig.Rig1.RigType, OmniRig.Rig1.Status.ToString());
                case 2:
                    return (OmniRig.Rig2.RigType, OmniRig.Rig2.Status.ToString());
                default:
                    return (null, null);
            }
        }

        protected override void SendCustomCommand(string command) {
            Console.Write("Sending command: " + command);
            byte[] byteCommand = Utilities.HexStringToByteArray(command);
            GetRig().SendCustomCommand(byteCommand, byteCommand.Length + 6, null);
        }

        private IRigX GetRig() {
            if (Rig == null) {
                switch (programOptions.RigNumber) {
                    case 1:
                        Rig = OmniRig.Rig1;
                        break;
                    case 2:
                        Rig = OmniRig.Rig2;
                        break;
                    default:
                        throw new ExitException(ExitCode.OPTION_RIG_NUMBER, "Invalid rig number: " + programOptions.RigNumber);
                }
            }
            return Rig;
        }

        public override void CheckRigStatus() {
            switch (GetRig().Status) {
                case OmniRig1.RigStatusX.ST_NOTCONFIGURED:
                    throw new ExitException(ExitCode.OMNIRIG_STATUS_NOTCONFIGURED, "ERROR: Rig " + programOptions.RigNumber + " in OmniRig not configured");
                case OmniRig1.RigStatusX.ST_DISABLED:
                    throw new ExitException(ExitCode.OMNIRIG_STATUS_DISABLED, "ERROR: OmniRig reports disabled");
                case OmniRig1.RigStatusX.ST_PORTBUSY:
                    throw new ExitException(ExitCode.OMNIRIG_STATUS_PORTBUSY, "ERROR: OmniRig reports COM port busy (Is the transceiver connected? Is the COM port used by some other program?)");
                case OmniRig1.RigStatusX.ST_NOTRESPONDING:
                    throw new ExitException(ExitCode.OMNIRIG_STATUS_NOTRESPONDING, "ERROR: Transceiver does not respond to OmniRig commands (Is the transceiver turned on?)");
                case OmniRig1.RigStatusX.ST_ONLINE:
                    break;
                default:
                    throw new ExitException(ExitCode.OMNIRIG_STATUS_UNKNOWN, "");
            }
        }
    }
}
