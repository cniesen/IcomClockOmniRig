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
            SendCustomCommand(programOptions.LookupCommand("setTimeCommand", timeData));
        }

        public void SetDate() {
            DateTime currentDatetTime = CurrentDateTime();
            string dateData = currentDatetTime.Year.ToString("D4") + currentDatetTime.Month.ToString("D2") + currentDatetTime.Day.ToString("D2");
            SendCustomCommand(programOptions.LookupCommand("setDateCommand", dateData));
            
            // For the rare occasion that the date just switched between generating the date and setting it on the radio
            if (currentDatetTime.Day != CurrentDateTime().Day) {
                currentDatetTime = CurrentDateTime();
                dateData = currentDatetTime.Year.ToString("D4") + currentDatetTime.Month.ToString("D2") + currentDatetTime.Day.ToString("D2");
                SendCustomCommand(programOptions.LookupCommand("setDateCommand", dateData));
            }
        }

        public void SetOffset() {
            string offsetData;
            TimeSpan offset = DateTimeOffset.Now.Offset;
            if ((offset.Hours < 0) == (programOptions.ReversedTimeZone)) {
                offsetData = Abs(offset.Hours).ToString("D2") + offset.Minutes.ToString("D2") +"00";
            } else {
                offsetData = Abs(offset.Hours).ToString("D2") + offset.Minutes.ToString("D2") + "01";
            }
            SendCustomCommand(programOptions.LookupCommand("setUtcOffsetCommand", offsetData));
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
