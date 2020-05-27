IcomClockOmniRig
================
Windows command line program for setting the internal clock of the Icom tranceivers like IC-7300, IC-7100 and IC-R8600 via OmniRig.

Download the latest Windows executable at https://github.com/cniesen/IcomClockOmniRig/releases/latest . 


Program Options
---------------
```
C:\>IcomClockOmniRig.exe -h
 ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
 ::                                                                          ::
 ::   IcomClockOmniRig 2.0  -  https://github.com/cniesen/IcomClockOmniRig   ::
 ::                                                                          ::
 ::    A program to set the Icom tranceiver clock to your computer's time    ::
 ::                                                                          ::
 ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

Usage: IcomClockOmniRig.exe [options]
Options:
        -u              Reverse local and UTC time (show UTC as clock and local time as on UTC display)

        -r <number>     The selected rig in OmniRig (default: 1)

        -m <model>      The Icom transceiver model (default: IC-7300)
                        Valid models: IC-7100, IC-7300, IC-7600, IC-7610, IC-7700, IC-7850, IC-7851, IC-9700, IC-R8600, IC-R9500

        -a <hex>        The Icom transceiver address (default: 94)

        -c <hex>        The controller address (default: E0)

        -o <number>     OmniRig version number (default: 1)
                        1 = original OmniRig by VE3NEA
                        2 = updated OmniRig by HB9RYZ

        -q              Quiet, don't output messages

        -f              Force tranceiver model, allow mismatch between OmniRig and this program.  Avoid this option if possible.
        
        -h              Show this help message
```


Supported Transceivers
----------------------

| Transceiver   | Date Command | Time Command | UTC Offset Command | Transceiver Address | Tested with OmniRig | Tested with OmniRig 2 |
|---------------|--------------|--------------|--------------------|---------------------|---------------------|-----------------------|
| Icom IC-7100  | 1A050120     | 1A050121     | 1A050123           | 88                  |                     |                       | 
| Icom IC-7300  | 1A050094     | 1A050095     | 1A050096           | 94                  | AE0S (2.0)          | AE0S (2.0)            |
| Icom IC-7600  | 1A050053     | 1A050054     | 1A050056           | 7A                  |                     |                       |
| Icom IC-7610  | 1A050158     | 1A050159     | 1A050162           | 98                  | VE3NEA (2.0)        |                       |
| Icom IC-7700  | 1A050058     | 1A050059     | 1A050061           | 74                  |                     |                       |
| Icom IC-7850  | 1A050095     | 1A050096     | 1A050099           | 8E                  |                     |                       |
| Icom IC-7851  | 1A050095     | 1A050096     | 1A050099           | 8E                  |                     |                       |
| Icom IC-9700  | 1A050179     | 1A050180     | 1A050184           | A2                  |                     |                       |
| Icom IC-R8600 | 1A050131     | 1A050132     | 1A050135           | 96                  |                     |                       |
| Icom IC-R9500 | 1A050048     | 1A050049     | 1A050051           | 72                  |                     |                       |

Tested IcomClockOmniRig version in parentheses.

Adding support for other Icom transceivers should be easy. Create an "Issue" if you needs support for your rig.  Other brands like Yaesu and Kenwood
might work as well. Voice up if there is a need for this feature.

Please let me know if you have tested the program.  I only have access to the Icom IC-7300 so verification that the program runs with other
transceivers is very helpful. I will note these in the above table along with the program version that was tested.  Any bug reports are 
greatly appreciated as well.  Please use Github Issues or the support email list for this. Thanks.


Notes
-----
* Since the seconds of the Icom clock can't be set directly, the program needs to wait until the full minute in order to set the time.  This means that the program might need up to one minute to run.

* OmniRig will continue to run in background if the program is killed (i.e. CTRL-C or Task Manager -> End Task).  This is normal and unavoidable.

* For trouble shooting type `echo Exit Code is %errorlevel%` after the program has just run to see the exit code.

The application is written in C# with Visual Studio.


Email Lists
-----------
* New release announcements: https://groups.io/g/IcomClockOmniRig-Announcements
* Support and discussions: https://groups.io/g/IcomClockOmniRig


License
-------
The code of IcomClockOmniRig has been Licensed under [GNU GPL 3.0](https://github.com/cniesen/IcomClockOmniRig/blob/master/COPYING.md).
