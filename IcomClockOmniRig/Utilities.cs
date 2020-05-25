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
using System.Linq;
using System.Text.RegularExpressions;

namespace IcomClockOmniRig {
    public class Utilities {
        public static bool IsDigit(string input) {
            return input == null ? false : Regex.IsMatch(input, "^[0-9]$");
        }

        public static bool IsHex(string input) {
            return input == null ? false : Regex.IsMatch(input, "^[a-fA-F0-9]{2}$");
        }

        public static short HIWORD(int word) {
            return ((short)((word >> 16) & 0xffff));
        }

        public static short LOWORD(int word) {
            return ((short)(word & 0xffff));
        }

        public static byte HIBYTE(int word) {
            return ((byte)((word >> 8) & 0xff));
        }

        public static byte LOBYTE(int word) {
            return ((byte)(word & 0xff));
        }

        public static byte[] HexStringToByteArray(string hexString) {
            return Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(X => Convert.ToByte(hexString.Substring(X, 2), 16))
                .ToArray();
        }
    }
}
