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

using System;
using static System.Math;
using static System.Threading.Thread;

namespace IcomClockOmniRig {
    abstract class OmniRigBase {
        protected readonly ProgramOptions programOptions;

        private readonly int responseTimeout = 4000; //milliseconds
        protected string timeCommand;
        protected bool? timeOk = null;
        protected string dateCommand;
        protected bool? dateOk = null;
        protected string offsetCommand;
        protected bool? offsetOk = null;

        public OmniRigBase(ProgramOptions programOptions) {
            this.programOptions = programOptions;
        }

        public void DisplayOmniRigInfo() {
            Console.WriteLine("OmniRig Software Version: " + SoftwareVersion());
            Console.WriteLine("OmniRig Interface Version: " + InterfaceVersion());
        }

        public void DisplayRigInfo() {
            int rigNum = 1;
            (string rigType, string rigStatus) rigInfo = RigInfo(rigNum);
            while (rigInfo.rigType != null) {
                Console.WriteLine("Rig " + rigNum);
                Console.WriteLine("    Rig Type: " + rigInfo.rigType);
                Console.WriteLine("    Status:   " + rigInfo.rigStatus);
                rigInfo = RigInfo(++rigNum);
            }
            Console.WriteLine();
        }

        public void CheckTransceiverModel() {
            if (programOptions.ForceTranceiverModel) {
                return;
            }

            (string rigType, string rigStatus) rigInfo = RigInfo(programOptions.RigNumber);
            if (programOptions.TransceiverModel != programOptions.LookupTransceiverModel(rigInfo.rigType)) {
                throw new ExitException(ExitCode.OPTION_TRANSCEIVER_MODEL, "Error: Transceiver model doesn't match transceiver in OmniRig.  This check can be overwritten via the '-f' flag but be careful.");
            }
        }

        public void SetTime() {
            if (!programOptions.Quiet) {
                Console.WriteLine("Waiting for the full minute to set time");
            }
            DateTime currentDatetTime = CurrentDateTime();
            while(currentDatetTime.Second != 0) {
                Sleep(50);
                currentDatetTime = CurrentDateTime();
            }
            string timeData = currentDatetTime.Hour.ToString("D2") + currentDatetTime.Minute.ToString("D2");
            timeCommand = programOptions.LookupCommand("setTimeCommand", timeData);
            SendCustomCommand(timeCommand);
            int msPassed = 0;
            while ((timeOk == null) && (msPassed < responseTimeout)) {
                Sleep(50);
                msPassed += 50;
            }
            if (!timeOk.HasValue) {
                Console.WriteLine(" - Rig response: timed out");
                throw new ExitException(ExitCode.OMNIRIG_SET_TIME_TIMEOUT, "Error: OmniRig did not respond when setting the time (Correct transceiver model, address, and command?)");
            } else if (timeOk.Value) {
                Console.WriteLine(" - Rig response: OK");
            } else { 
                Console.WriteLine(" - Rig response: Error");
                throw new ExitException(ExitCode.OMNIRIG_SET_TIME_ERROR, "Error: OmniRig raised and error when setting the time");
            }
        }

        public void SetDate() {
            DateTime currentDatetTime = CurrentDateTime();
            string dateData = currentDatetTime.Year.ToString("D4") + currentDatetTime.Month.ToString("D2") + currentDatetTime.Day.ToString("D2");
            dateCommand = programOptions.LookupCommand("setDateCommand", dateData);
            SendCustomCommand(dateCommand);
            int msPassed = 0;
            while ((timeOk == null) && (msPassed < responseTimeout)) {
                Sleep(50);
                msPassed += 50;
            }
            if (!timeOk.HasValue) {
                Console.WriteLine(" - Rig response: timed out");
                throw new ExitException(ExitCode.OMNIRIG_SET_TIME_TIMEOUT, "Error: OmniRig did not respond when setting the time (Correct transceiver model, address, and command?)");
            } else if (!timeOk.Value) {
                Console.WriteLine(" - Rig response: Error");
                throw new ExitException(ExitCode.OMNIRIG_SET_TIME_ERROR, "Error: OmniRig raised and error when setting the time");
            }

            // For the rare occasion that the date just switched between generating the date and setting it on the radio
            if (currentDatetTime.Day != CurrentDateTime().Day) {
                currentDatetTime = CurrentDateTime();
                dateData = currentDatetTime.Year.ToString("D4") + currentDatetTime.Month.ToString("D2") + currentDatetTime.Day.ToString("D2");
                dateCommand = programOptions.LookupCommand("setDateCommand", dateData);
                SendCustomCommand(dateCommand);
                msPassed = 0;
                while ((timeOk == null) && (msPassed < responseTimeout)) {
                    Sleep(50);
                    msPassed += 50;
                }
                if (!timeOk.HasValue) {
                    Console.WriteLine(" - Rig response: timed out");
                    throw new ExitException(ExitCode.OMNIRIG_SET_TIME_TIMEOUT, "Error: OmniRig did not respond when setting the time (Correct transceiver model, address, and command?)");
                } else if (!timeOk.Value) {
                    Console.WriteLine(" - Rig response: Error");
                    throw new ExitException(ExitCode.OMNIRIG_SET_TIME_ERROR, "Error: OmniRig raised and error when setting the time");
                }
            }
            Console.WriteLine(" - Rig response: OK");

        }

        public void SetOffset() {
            string offsetData;
            TimeSpan offset = DateTimeOffset.Now.Offset;
            if ((offset.Hours < 0) == (programOptions.ReversedTimeZone)) {
                offsetData = Abs(offset.Hours).ToString("D2") + offset.Minutes.ToString("D2") +"00";
            } else {
                offsetData = Abs(offset.Hours).ToString("D2") + offset.Minutes.ToString("D2") + "01";
            }
            offsetCommand = programOptions.LookupCommand("setUtcOffsetCommand", offsetData);
            SendCustomCommand(offsetCommand);
            int msPassed = 0;
            while ((offsetOk == null) && (msPassed < responseTimeout)) {
                Sleep(50);
                msPassed += 50;
            }
            if (!offsetOk.HasValue) {
                Console.WriteLine(" - Rig response: timed out");
                throw new ExitException(ExitCode.OMNIRIG_SET_OFFSET_TIMEOUT, "Error: OmniRig did not respond when setting the utc offset (Correct transceiver model, address, and command?)");
            } else if (offsetOk.Value) {
                Console.WriteLine(" - Rig response: OK");
            } else {
                Console.WriteLine(" - Rig response: Error");
                throw new ExitException(ExitCode.OMNIRIG_SET_OFFSET_ERROR, "Error: OmniRig raised and error when setting the utc offset");
            }
        }

        //Omnirig CustomReply event
        protected void OmniRig_CustomReply(int rigNumber, object command, object reply) {
            if (rigNumber != programOptions.RigNumber) {
                return;
            }

            string sCommand = Utilities.ByteArrayToHexString((byte[])command);
            string sReply = Utilities.ByteArrayToHexString((byte[])reply);

            if ((sCommand == timeCommand) && sReply.StartsWith(sCommand)) {
                timeOk = (sCommand + programOptions.ResponseOk() == sReply);
            } else if ((sCommand == dateCommand) && sReply.StartsWith(sCommand)) {
                dateOk = (sCommand + programOptions.ResponseOk() == sReply);
            } else if ((sCommand == offsetCommand) && sReply.StartsWith(sCommand)) {
                offsetOk = (sCommand + programOptions.ResponseOk() == sReply);
            }
        }

        protected abstract string SoftwareVersion();
        protected abstract string InterfaceVersion();
        protected abstract (string rigType, string rigStatus) RigInfo(int rigNumber);
        public abstract void CheckRigStatus();
        protected abstract void SendCustomCommand(string command);

        private DateTime CurrentDateTime() {
            if (programOptions.ReversedTimeZone) {
                return DateTime.UtcNow;
            } else {
                return DateTime.Now;
            }
        }
    }
}
