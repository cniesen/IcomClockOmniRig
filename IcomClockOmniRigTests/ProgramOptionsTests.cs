using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace IcomClockOmniRig.Tests {
    [TestClass()]
    public class ProgramOptionsTests {
        readonly string help_beginning =
            " ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n" +
            " ::                                                                          ::\n" +
            " ::   IcomClockOmniRig 2.0  -  https://github.com/cniesen/IcomClockOmniRig   ::\n" +
            " ::                                                                          ::\n" +
            " ::    A program to set the Icom tranceiver clock to your computer's time    ::\n" +
            " ::                                                                          ::\n" +
            " ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::\n\n";

        readonly string help_ending =
            "Usage: " + AppDomain.CurrentDomain.FriendlyName + " [options]\n" +
            "Options:\n" +
            "\t-u\t\tReverse local and UTC time (show UTC as clock and local time as on UTC display)\n\n" +
            "\t-r <number>\tThe selected rig in OmniRig (default: 1)\n\n" +
            "\t-m <model>\tThe Icom transceiver model (default: IC-7300)\n" +
            "\t\t\tValid models: IC-7100, IC-7300, IC-7610\n\n" +
            "\t-a <hex>\tThe Icom transceiver address (default: 94)\n\n" +
            "\t-c <hex>\tThe controller address (default: E0)\n\n" +
            "\t-o <number>\tOmniRig version number (default: 1)\n" +
            "\t\t\t1 = original OmniRig by VE3NEA\n" +
            "\t\t\t2 = updated OmniRig by HB9RYZ\n\n" +
            "\t-q\t\tQuiet, don't output messages\n\n" +
            "\t-f\t\tForce tranceiver model, allow mismatch between OmniRig and this program.  Avoid this option if possible.\n\n" +
            "\t-h\t\tShow this help message\n\n";

        [TestMethod()]
        public void Args_Defaults() {
            ProgramOptions programOptions = new ProgramOptions(new string[0]);
            Assert.IsFalse(programOptions.ReversedTimeZone);
            Assert.AreEqual(1, programOptions.RigNumber);
            Assert.AreEqual("IC-7300", programOptions.TransceiverModel);
            Assert.AreEqual("94", programOptions.TransceiverAddress);
            Assert.AreEqual("E0", programOptions.ControllerAddress);
            Assert.IsFalse(programOptions.Quiet);
            Assert.IsFalse(programOptions.ForceTranceiverModel);
        }

        [TestMethod()]
        public void Args_Reverse() {
            ProgramOptions programOptions = new ProgramOptions(new string[1] { "-u" });
            Assert.IsTrue(programOptions.ReversedTimeZone);
        }

        [TestMethod()]
        public void Args_Rig() {
            ProgramOptions programOptions = new ProgramOptions(new string[2] { "-r", "1" });
            Assert.AreEqual(1, programOptions.RigNumber);
            programOptions = new ProgramOptions(new string[2] { "-r", "2" });
            Assert.AreEqual(2, programOptions.RigNumber);
        }

        [TestMethod()]
        public void Args_Rig_OmniRig2() {
            ProgramOptions programOptions = new ProgramOptions(new string[4] { "-o", "2", "-r", "1" });
            Assert.AreEqual(1, programOptions.RigNumber);
            programOptions = new ProgramOptions(new string[4] { "-o", "2", "-r", "2" });
            Assert.AreEqual(2, programOptions.RigNumber);
            programOptions = new ProgramOptions(new string[4] { "-o", "2", "-r", "3" });
            Assert.AreEqual(3, programOptions.RigNumber);
            programOptions = new ProgramOptions(new string[4] { "-o", "2", "-r", "4" });
            Assert.AreEqual(4, programOptions.RigNumber);
        }

        [TestMethod()]
        public void Args_Rig_Missing() {
            string expectedErrorMessage = "ERROR: No rig number specified\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[1] { "-r" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_RIG_NUMBER, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_Rig_Invalid() {
            string expectedErrorMessage = "ERROR: Invalid rig number specified: foobar\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[2] { "-r", "foobar" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_RIG_NUMBER, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_Rig_InvalidNumber() {
            string expectedErrorMessage = "ERROR: Invalid rig number specified: 3\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[2] { "-r", "3" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_RIG_NUMBER, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_TransceiverModel() {
            ProgramOptions programOptions = new ProgramOptions(new string[2] { "-m", "IC-7100" });
            Assert.AreEqual("IC-7100", programOptions.TransceiverModel);
        }

        [TestMethod()]
        public void Args_TransceiverModel_Missing() {
            string expectedErrorMessage = "ERROR: No transceiver model specified\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[1] { "-m" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_TRANSCEIVER_MODEL, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_TransceiverModel_Invalid() {
            string expectedErrorMessage = "ERROR: Invalid transceiver model specified: foobar\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[2] { "-m", "foobar" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_TRANSCEIVER_MODEL, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_TransceiverAddress() {
            ProgramOptions programOptions = new ProgramOptions(new string[2] { "-a", "3D" });
            Assert.AreEqual("3D", programOptions.TransceiverAddress);
        }

        [TestMethod()]
        public void Args_TransceiverAddress_Missing() {
            string expectedErrorMessage = "ERROR: No transceiver address specified\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[1] { "-a" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_TRANSCEIVER_ADDRESS, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_TransceiverAddress_Invalid() {
            string expectedErrorMessage = "ERROR: Invalid transceiver address specified: foobar\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[2] { "-a", "foobar" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_TRANSCEIVER_ADDRESS, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_ControllerAddress() {
            ProgramOptions programOptions = new ProgramOptions(new string[2] { "-c", "3D" });
            Assert.AreEqual("3D", programOptions.ControllerAddress);
        }

        [TestMethod()]
        public void Args_ControllerAddress_Missing() {
            string expectedErrorMessage = "ERROR: No controller address specified\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[1] { "-c" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_CONTROLLER_ADDRESS, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_ControllerAddress_Invalid() {
            string expectedErrorMessage = "ERROR: Invalid controller address specified: foobar\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[2] { "-c", "foobar" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_CONTROLLER_ADDRESS, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_OmniRigVersion() {
            ProgramOptions programOptions = new ProgramOptions(new string[2] { "-o", "1" });
            Assert.AreEqual(OmniRigVersion.OmniRigVersion1, programOptions.OmnirigVersion);
             programOptions = new ProgramOptions(new string[2] { "-o", "2" });
            Assert.AreEqual(OmniRigVersion.OmniRigVersion2, programOptions.OmnirigVersion);
        }

        [TestMethod()]
        public void Args_OmniRigVersion_Missing() {
            string expectedErrorMessage = "ERROR: No OmniRig version specified\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[1] { "-o" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_OMNIRIG_VERSION, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_OmniRigVersion_Invalid() {
            string expectedErrorMessage = "ERROR: Invalid OmniRig version specified: 3\n\n\r\n";

            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[2] { "-o", "3" });
                } catch (ExitException e) {
                    Assert.AreEqual(ExitCode.OPTION_OMNIRIG_VERSION, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), expectedErrorMessage + help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void Args_Quiet() {
            ProgramOptions programOptions = new ProgramOptions(new string[1] { "-q" });
            Assert.IsTrue(programOptions.Quiet);
        }

        [TestMethod()]
        public void Args_ForceTranceiverModel() {
            ProgramOptions programOptions = new ProgramOptions(new string[1] { "-f" });
            Assert.IsTrue(programOptions.ForceTranceiverModel);
        }

        [TestMethod()]
        public void Args_Help() {
            using (StringWriter sw = new StringWriter()) {
                Console.SetOut(sw);
                try {
                    ProgramOptions programOptions = new ProgramOptions(new string[1] { "-h" });
                } catch (ExitException e) {
                    // Help throws an ExitCode.SUCCESS to terminate the program nicely.
                    Assert.AreEqual(ExitCode.SUCCESS, e.ExitCode);
                    Assert.AreEqual("", e.Message);
                }
                StringAssert.StartsWith(sw.ToString(), help_beginning);
                StringAssert.EndsWith(sw.ToString(), help_ending);
            }
        }

        [TestMethod()]
        public void LookupTransceiverAddress() {
            ProgramOptions programOptions = new ProgramOptions(new string[0]);
            Assert.AreEqual("94", programOptions.LookupTransceiverAddress());
            programOptions = new ProgramOptions(new string[2] { "-m", "IC-7100" });
            Assert.AreEqual("88", programOptions.LookupTransceiverAddress());
            programOptions = new ProgramOptions(new string[2] { "-m", "IC-7300" });
            Assert.AreEqual("94", programOptions.LookupTransceiverAddress());
            programOptions = new ProgramOptions(new string[2] { "-m", "IC-7610" });
            Assert.AreEqual("98", programOptions.LookupTransceiverAddress());

        }

        [TestMethod()]
        public void LookupCommandTest_Default() {
            ProgramOptions programOptions = new ProgramOptions(new string[0]);
            Assert.AreEqual("FEFE94E01A05009420200524FD", programOptions.LookupCommand("setDateCommand", "20200524"));
            Assert.AreEqual("FEFE94E01A0500951340FD", programOptions.LookupCommand("setTimeCommand", "1340"));
            Assert.AreEqual("FEFE94E01A05009650001FD", programOptions.LookupCommand("setUtcOffsetCommand", "50001"));
        }

        [TestMethod()]
        public void LookupCommandTest_Custom_Addresses() {
            ProgramOptions programOptions = new ProgramOptions(new string[4] { "-a", "C8", "-c", "44" });
            Assert.AreEqual("FEFEC8441A05009420200524FD", programOptions.LookupCommand("setDateCommand", "20200524"));
            Assert.AreEqual("FEFEC8441A0500951340FD", programOptions.LookupCommand("setTimeCommand", "1340"));
            Assert.AreEqual("FEFEC8441A05009650001FD", programOptions.LookupCommand("setUtcOffsetCommand", "50001"));
        }

        [TestMethod()]
        public void LookupCommandTest_IC7100() {
            ProgramOptions programOptions = new ProgramOptions(new string[2] { "-m", "IC-7100" });
            Assert.AreEqual("FEFE88E01A05012020200524FD", programOptions.LookupCommand("setDateCommand", "20200524"));
            Assert.AreEqual("FEFE88E01A0501211340FD", programOptions.LookupCommand("setTimeCommand", "1340"));
            Assert.AreEqual("FEFE88E01A05012350001FD", programOptions.LookupCommand("setUtcOffsetCommand", "50001"));
        }

        [TestMethod()]
        public void LookupCommandTest_IC7610() {
            ProgramOptions programOptions = new ProgramOptions(new string[2] { "-m", "IC-7610" });
            Assert.AreEqual("FEFE98E01A05015820200524FD", programOptions.LookupCommand("setDateCommand", "20200524"));
            Assert.AreEqual("FEFE98E01A0501591340FD", programOptions.LookupCommand("setTimeCommand", "1340"));
            Assert.AreEqual("FEFE98E01A05016250001FD", programOptions.LookupCommand("setUtcOffsetCommand", "50001"));
        }

        [TestMethod()]
        [ExpectedException(typeof(KeyNotFoundException), "Command setFooBar not found for tranceiver IC-7300")]
        public void LookupCommandTest_MissingCommand() {
            ProgramOptions programOptions = new ProgramOptions(new string[0]);
            programOptions.LookupCommand("setFooBar", "20200524");
        }
    }
}