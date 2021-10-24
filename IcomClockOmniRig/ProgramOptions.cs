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
using System.Collections.Generic;

namespace IcomClockOmniRig {
	public enum OmniRigVersion {
		OmniRigVersion1,  // original OmniRig by VE3NEA
		OmniRigVersion2   // updated OmniRig by HB9RYZ
	}

	public class ProgramOptions {
		public bool ReversedTimeZone { get; } = false;
		public int RigNumber { get; } = 1;
		public string TransceiverAddress { get; private set; } = null;
		public string ControllerAddress { get; } = "E0";
		public string TransceiverModel { get; private set; } = null;
		public OmniRigVersion OmnirigVersion { get; } = OmniRigVersion.OmniRigVersion1;
		public bool Quiet { get; } = false;
		public bool ForceTranceiverModel { get; } = false;

		private const string Preamble = "FEFE";
		private const string Postamble = "FD";

		private readonly IDictionary<string, IDictionary<string, string>> Transceivers = new Dictionary<string, IDictionary<string, string>>() {
			{"IC-705", new Dictionary<string, string> {{"setDateCommand", "1A050165"}, {"setTimeCommand", "1A050166"}, {"setUtcOffsetCommand", "1A050170"}, {"transceiverAddress", "A4"}}},
			{"IC-7100", new Dictionary<string, string> {{"setDateCommand", "1A050120"}, {"setTimeCommand", "1A050121"}, {"setUtcOffsetCommand", "1A050123"}, {"transceiverAddress", "88"}}},
			{"IC-7300", new Dictionary<string, string> {{"setDateCommand", "1A050094"}, {"setTimeCommand", "1A050095"}, {"setUtcOffsetCommand", "1A050096"}, {"transceiverAddress", "94"}}},
			{"IC-7600", new Dictionary<string, string> {{"setDateCommand", "1A050053"}, {"setTimeCommand", "1A050054"}, {"setUtcOffsetCommand", "1A050056"}, {"transceiverAddress", "7A"}}},
			{"IC-7610", new Dictionary<string, string> {{"setDateCommand", "1A050158"}, {"setTimeCommand", "1A050159"}, {"setUtcOffsetCommand", "1A050162"}, {"transceiverAddress", "98"}}},
			{"IC-7700", new Dictionary<string, string> {{"setDateCommand", "1A050058"}, {"setTimeCommand", "1A050059"}, {"setUtcOffsetCommand", "1A050061"}, {"transceiverAddress", "74"}}},
			{"IC-7850", new Dictionary<string, string> {{"setDateCommand", "1A050095"}, {"setTimeCommand", "1A050096"}, {"setUtcOffsetCommand", "1A050099"}, {"transceiverAddress", "8E"}}},
			{"IC-7851", new Dictionary<string, string> {{"setDateCommand", "1A050095"}, {"setTimeCommand", "1A050096"}, {"setUtcOffsetCommand", "1A050099"}, {"transceiverAddress", "8E"}}},
			{"IC-9700", new Dictionary<string, string> {{"setDateCommand", "1A050179"}, {"setTimeCommand", "1A050180"}, {"setUtcOffsetCommand", "1A050184"}, {"transceiverAddress", "A2"}}},
			{"IC-R8600", new Dictionary<string, string> {{"setDateCommand", "1A050131"}, {"setTimeCommand", "1A050132"}, {"setUtcOffsetCommand", "1A050135"}, {"transceiverAddress", "96"}}},
			{"IC-R9500", new Dictionary<string, string> {{"setDateCommand", "1A050048"}, {"setTimeCommand", "1A050049"}, {"setUtcOffsetCommand", "1A050051"}, {"transceiverAddress", "72"}}}
		};

		private readonly IDictionary<string, string> RigTypes = new Dictionary<string, string>() {
			{"IC-705", "IC-705"}, {"IC-705-DATA", "IC-705-DATA"},
			{"IC-7100", "IC-7100"}, {"IC-7100-DATA-FIL1", "IC-7100"}, {"IC-7100e4", "IC-7100"}, {"IC-7100e4-DATA", "IC-7100"},
			{"IC-7300", "IC-7300"}, {"IC-7300-DATA", "IC-7300"},
			{"IC-7600", "IC-7600"}, {"IC-7600v2", "IC-7600"}, {"IC-7600v2-DATA", "IC-7600"},
			{"IC-7610", "IC-7610"}, {"IC-7610-DATA", "IC-7610"}, {"IC-7610-DATA-FIL1", "IC-7610"},
			{"IC-7700", "IC-7700"}, {"IC-7700v2", "IC-7700"}, {"IC-7700v2-DATA", "IC-7700"},
			{"IC-7850", "IC-7850"}, {"IC-7850-DATA", ""}, {"IC-7850-DATA-FIL1", ""},
			{"IC-7851", "IC-7851"}, {"IC-7851-DATA", "IC-7851"}, {"IC-7851-DATA-FIL1", "IC-7851"},
			{"IC-9700", "IC-9700"}, {"IC-9700-DATA", "IC-9700"}, {"IC-9700-SAT", "IC-9700"},
			{"IC-R8600", "IC-R8600"},
			{"IC-R9500", "IC-R9500"}
		};

		public ProgramOptions(string[] args) {
			if (!Quiet) {
				PrintProgramInfo();
			}

			for (int i = 0; i < args.Length; ++i) {
				string arg = args[i]; 
				if(arg == "-u") {
					ReversedTimeZone = true;
				} else if (arg == "-r") {
					if (++i >= args.Length) {
						Console.WriteLine("ERROR: No rig number specified\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_RIG_NUMBER);
					}
					if (Utilities.IsDigit(args[i])) {
						RigNumber = Convert.ToInt32(args[i]);
					} else {
						Console.WriteLine("ERROR: Invalid rig number specified: " + args[i] + "\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_RIG_NUMBER);
					}
				} else if (arg == "-m") {
					if (++i >= args.Length) {
						Console.WriteLine("ERROR: No transceiver model specified\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_TRANSCEIVER_MODEL);
					}
					if (IsValidTransceiverModel(args[i])) {
						TransceiverModel = args[i];
					} else {
						Console.WriteLine("ERROR: Invalid transceiver model specified: " + args[i] + "\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_TRANSCEIVER_MODEL);
					}
				} else if (arg == "-a") {
					if (++i >= args.Length) {
						Console.WriteLine("ERROR: No transceiver address specified\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_TRANSCEIVER_ADDRESS);
					}
					if (Utilities.IsHex(args[i])) {
						TransceiverAddress = args[i];
					} else {
						Console.WriteLine("ERROR: Invalid transceiver address specified: " + args[i] + "\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_TRANSCEIVER_ADDRESS);
					}
				} else if (arg == "-c") {
					if (++i >= args.Length) {
						Console.WriteLine("ERROR: No controller address specified\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_CONTROLLER_ADDRESS);
					}
					if (Utilities.IsHex(args[i])) {
						ControllerAddress = args[i];
					} else {
						Console.WriteLine("ERROR: Invalid controller address specified: " + args[i] + "\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_CONTROLLER_ADDRESS);
					}
				} else if (arg == "-o") {
					if (++i >= args.Length) {
						Console.WriteLine("ERROR: No OmniRig version specified\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_OMNIRIG_VERSION);
					}
					if (args[i] == "1") {
						OmnirigVersion = OmniRigVersion.OmniRigVersion1;
					} else if (args[i] == "2") {
						OmnirigVersion = OmniRigVersion.OmniRigVersion2;
					} else {
						Console.WriteLine("ERROR: Invalid OmniRig version specified: " + args[i] + "\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_OMNIRIG_VERSION);
					}
				} else if (arg == "-q") {
			        Quiet = true;
				} else if (arg == "-h") {
					PrintHelp();
					throw new ExitException(ExitCode.SUCCESS);
				} else {
			        Console.WriteLine("ERROR: Invalid option specified: " + arg + "\n\n");
			        PrintHelp();
					throw new ExitException(ExitCode.OPTION_INVALID);
				}
	        }

			switch (OmnirigVersion) {
				case OmniRigVersion.OmniRigVersion1:
					if (RigNumber < 1 || RigNumber > 2) {
						Console.WriteLine("ERROR: Invalid rig number specified: " + RigNumber + "\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_RIG_NUMBER);

					}
					break;
				case OmniRigVersion.OmniRigVersion2:
					if (RigNumber < 1 || RigNumber > 4) {
						Console.WriteLine("ERROR: Invalid rig number specified: " + RigNumber + "\n\n");
						PrintHelp();
						throw new ExitException(ExitCode.OPTION_RIG_NUMBER);
					}
					break;
				default:
					throw new ArgumentException("Invalid OmniRigVersion: " + OmnirigVersion);
			}
		}
		
		private void PrintProgramInfo() {
			Console.Write(
				" ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n" +
				" ::                                                                          ::\n" +
				" ::   IcomClockOmniRig 2.1  -  https://github.com/cniesen/IcomClockOmniRig   ::\n" +
				" ::                                                                          ::\n" +
				" ::    A program to set the Icom tranceiver clock to your computer's time    ::\n" +
				" ::                                                                          ::\n" +
				" ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n\n"
				);
		}

		private void PrintHelp() {
			if (Quiet) {
				PrintProgramInfo();
			}
			Console.Write(
				"Usage: " + AppDomain.CurrentDomain.FriendlyName + " [options]\n" +
				"Options:\n" +
				"\t-u\t\tReverse local and UTC time (show UTC as clock and local time as on UTC display)\n\n" +
				"\t-r <number>\tThe selected rig in OmniRig (default: 1)\n\n" +
				"\t-m <model>\tThe Icom transceiver model (default: auto detect from OmniRig)\n" +
				"\t\t\tValid models: " + ListValidTransceiverModels() + "\n\n" +
				"\t-a <hex>\tThe Icom transceiver address (default: rig default address)\n\n" +
				"\t-c <hex>\tThe controller address (default: E0)\n\n" +
				"\t-o <number>\tOmniRig version number (default: 1)\n" +
				"\t\t\t1 = original OmniRig by VE3NEA\n" +
				"\t\t\t2 = updated OmniRig by HB9RYZ\n\n" +
				"\t-q\t\tQuiet, don't output messages\n\n" +
				"\t-h\t\tShow this help message\n\n"
				);
		}

		private void PrintOptions() {
			Console.Write("Program Options:\n" +
				"    Using OmniRig version: " + ((OmnirigVersion == OmniRigVersion.OmniRigVersion2) ? "2 (by HB9RYZ)" : "1 (by VE3NEA)") + "\n" +
				"    Using OmniRig rig: " + RigNumber + "\n" +
				"    Using transceiver model: " + TransceiverModel + "\n" +
				"    Using transceiver address: " + TransceiverAddress + "\n" +
				"    Reverse clock and UTC time: " + (ReversedTimeZone ? "yes" : "no") + "\n\n"
				);

		}

		public void InitRigBasedDefaults(string rigType) {
			if (TransceiverModel == null) {
				TransceiverModel = LookupTransceiverModel(rigType);
			}

			if (TransceiverAddress == null) {
				TransceiverAddress = LookupTransceiverAddress();
			}

			if (!Quiet) {
				PrintOptions();
			}
		}

		private bool IsValidTransceiverModel(string transceiverModel) {
			return Transceivers.ContainsKey(transceiverModel);
		}

		private string LookupTransceiverModel(string rigType) {
			try {
				return RigTypes[rigType];
			} catch (KeyNotFoundException e) {
				foreach (KeyValuePair<String, String> rigTypeMapping in RigTypes) {
					if (rigType.StartsWith(rigTypeMapping.Key + "-")) {
						return rigTypeMapping.Value;
					}
				}
				throw new KeyNotFoundException("OmniRig's rig type '" + rigType + "' not found", e);
			}
		}

		public string LookupTransceiverAddress() {
			try {
				return Transceivers[TransceiverModel]["transceiverAddress"];
			} catch (KeyNotFoundException e) {
				throw new KeyNotFoundException("Transceiver address not found for tranceiver " + TransceiverModel, e);

			}
		}

		public string LookupCommand(string command, string data) {
			try {
				return Preamble + TransceiverAddress + ControllerAddress + Transceivers[TransceiverModel][command] + data + Postamble;
			} catch (KeyNotFoundException e) {
				throw new KeyNotFoundException("Command '" + command + "' not found for tranceiver " + TransceiverModel, e);
				
			}
		}

		private string ListValidTransceiverModels() {
			return String.Join(", ", Transceivers.Keys);
		}

		public string ResponseOk() {
			return "FEFE" + ControllerAddress + TransceiverAddress + "FBFD";
		}

	}
}