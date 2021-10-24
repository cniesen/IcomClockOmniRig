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

namespace IcomClockOmniRig {
    class Program {
        static void Main(string[] args) {
            try {
                ProgramOptions programOptions = new ProgramOptions(args);
                OmniRigBase omnirig;
                switch (programOptions.OmnirigVersion) {
                    case OmniRigVersion.OmniRigVersion1:
                        omnirig = new OmniRigV1(programOptions);
                        break;
                    case OmniRigVersion.OmniRigVersion2:
                        omnirig = new OmniRigV2(programOptions);
                        break;
                    default:
                        throw new ArgumentException("Invalid OmniRigVersion: " + programOptions.OmnirigVersion);
                }

                if (!programOptions.Quiet) {
                    omnirig.DisplayOmniRigInfo();
                    omnirig.DisplayRigInfo();
                }
                omnirig.CheckRigStatus();
                omnirig.SetTime();
                omnirig.SetDate();
                omnirig.SetOffset();

                Environment.Exit((int)ExitCode.SUCCESS);
            } catch (ExitException e) {
                if (!String.IsNullOrWhiteSpace(e.Message)) {
                    Console.WriteLine(e.Message);
                }
                Environment.Exit((int)e.ExitCode);
            }
        }
    }
}
