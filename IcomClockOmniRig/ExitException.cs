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

    public enum ExitCode {
        SUCCESS = 0,

		OPTION_INVALID = -1,
		OPTION_RIG_NUMBER = -2,
		OPTION_TRANSCEIVER_MODEL = -3,
		OPTION_TRANSCEIVER_ADDRESS = -4,
		OPTION_CONTROLLER_ADDRESS = -5,
		OPTION_OMNIRIG_VERSION = -6,

		OMNIRIG_COM_INIT = -10,
		OMNIRIG_COM_CREATE = -11,

		OMNIRIG_STATUS_NOTCONFIGURED = -50,
		OMNIRIG_STATUS_DISABLED = -51,
		OMNIRIG_STATUS_PORTBUSY = -52,
		OMNIRIG_STATUS_NOTRESPONDING = -53,
		OMNIRIG_STATUS_UNKNOWN = -54,

		INTERNAL_OMNIRIG_SET_TIME = -500,
		INTERNAL_OMNIRIG_SET_DATE = -501,
		INTERNAL_OMNIRIG_SET_OFFSET = -502,

		INTERNAL_COMMAND_MAP_TRANSCEIVER = -600,
		INTERNAL_COMMAND_MAP_COMMAND = -601,
    }

    public class ExitException : Exception {
		public ExitCode ExitCode { get; }

		public ExitException(ExitCode exitCode) :base("") {
			ExitCode = exitCode;
		}

		public ExitException(ExitCode exitCode, string message) :base(message) {
			ExitCode = exitCode;
        }
    }
}
