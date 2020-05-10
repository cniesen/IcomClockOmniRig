IcomClockOmniRig
================
Windows command line program for setting the internal clock of the Icom IC-7300 or IC-7100 via OmniRig.

Download the latest Windows 64bit executable at https://github.com/cniesen/IcomClockOmniRig/releases/latest . 


Program Options
---------------
```
C:\>IcomClockOmniRig.exe -h
 ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
 ::                                                                          ::
 ::   IcomClockOmniRig 1.0  -  https://github.com/cniesen/IcomClockOmniRig   ::
 ::                                                                          ::
 ::    A program to set the Icom tranceiver clock to your computer's time    ::
 ::                                                                          ::
 ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

Usage: IcomClockOmniRig.exe [options]
Options:
        -u              Reverse local and UTC time (show UTC as clock and local time as on UTC display)

        -r <number>     The selected rig in OmniRig (default: 1)

        -m <model>      The Icom transceiver model (default: IC-7300)
                        Valid models: IC-7100, IC-7300

        -a <hex>        The Icom transceiver address (default: 94)

        -o <number>     OmniRig version number (default: 1)
                        1 = original OmniRig by VE3NEA
                        2 = updated OmniRig by HB9RYZ

        -q              Quiet, don't output messages

        -h              Show this help message
```


Notes
-----
* OmniRig needs to be installed in the default directory.  For OmniRig by VE3NEA this is in "C:\Program Files (x86)\Afreet\OmniRig\OmniRig.exe" and for OmniRig 2 by HB9RYZ this is in "C:\Program Files (x86)\Omni-Rig V2\omnirig2.exe".

* Since the seconds of the Icom clock can't be set directly, the program needs to wait until the full minute in order to set the time.  This means that the program might need up to one minute to run.

* Adding support for other Icom transceivers should be easy, although many of them support NTP (Network Time Protocol). Create an "Issue" if you needs support for your rig.

* For trouble shooting type `echo Exit Code is %errorlevel%` after the program has just run to see the exit code.

The application is written in C++ with Visual Studio.


License
-------
The code of IcomClockOmniRig has been Licensed under [GNU GPL 3.0](https://github.com/cniesen/IcomClockOmniRig/blob/master/COPYING.md).
