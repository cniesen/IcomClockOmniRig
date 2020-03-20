IcomClockOmniRig
================
Windows command line program for setting the internal clock of the Icom IC-7300 via OmniRig.

The application is written in C++ with Visual Studio.

**Current status:** Beginning of development

Connection to OmniRig's Component Object Model is successful.  The predefined calls like getting the frequency works.  
However, I still haven't had success sending custom commands.  I also don't have a clue how to receive the custom replies from OmniRig.
Somehow I need to register an event handler for this.

Any help to advance this project is greatly appreciated.


License
-------
The code of IcomClockOmniRig has been Licensed under [GNU GPL 3.0](https://github.com/cniesen/IcomClockOmniRig/blob/master/COPYING.md).