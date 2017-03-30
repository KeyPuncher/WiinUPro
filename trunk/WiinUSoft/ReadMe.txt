======================
WiinUSoft 3.0
======================

 WiinUSoft is a program that allows you to use your Nintendo
 Bluetooth Controllers as Xbox 360 Controllers for Windows.
 With it you can use up to 4 controllers at a time with force
 feedback support and you can customize the button mapping.


======================
Requirements
======================

 - The Microsoft Xbox 360 Controller Driver must be installed
   These XInput drivers should already be packaged in with Windows 10 & Windows 8.1
   http://www.microsoft.com/hardware/en-us/d/xbox-360-controller-for-windows

 - The SCP Service Driver must be installed. Use /SCP_Driver/ScpDriver.exe to install it.
   You only need the "Configure Service" installed, you do not need to install its Bluetooth Driver.


======================
Supported Controllers
======================

 = Official Controllers =
 - Nintendo Wii U Pro Controller
 - Nintendo Wii Remote
 - Nintendo Nunchuk
 - Nintendo Classic Controller
 - Nintendo Classic Controller Pro

 = 3rd Party Controllers =
 - HORI Battle Pad
 - PDP Fightpad
 - Pro Controller U


======================
Change Log
======================

 = version 3.0 =
 - Using v2.5 of the Nintroller library (not backwards compatible)
 - Added ability to sync & pair controllers to the Windows BT Stack
 - Support for both Windows & Toshiba Bluetooth Stacks
 - Added Wiimote IR Sensor support
 - Added auto refresh of device list
 - Added "Greedy Mode" option (Dark Souls Fix)
 - Added ability to set a default calibration setting
 - Added connect to first available XInput device auto connect option
 - Updated error reporting
 - Fixed crash involving applying controller calibration
 - Fixed controller icon detection
 - Adjusted Accelerometer calibration values (less tilting needed to reach maximum)
 - Minor UI Tweaks
 - PDP Fightpad tested and works (shoulders are digital by design)
 - Pro Controller U support added (works as Wiimote + Classic Controller)

 = version 2.1 =
 - Fixed Rumble not working
 - Various crash fixes

 = version 2.0 =
 - Using v2 of the Nintroller library
 - Fixed Windows 10 crash (removes blocked access)
 - Fixed file access issue when saving prefs.config
 - Added controller calibration options
 - Added ability to set WiinUSoft to launch on Windows startup
 - Added ability to start WiinUSoft minimized
 - Added ability to map an input to nothing
 - Added Wiimote and Nunchuk accelerometer mapping (primitive)
 - Added Error crash reports can now be sent
 - Added The controller type can be manually set if needed

 = version 1.1 =
 - Controller access is now blocked from other programs
 - Fixed crash when auto connecting
 - Added ability to disable rumble
 - Added rumble patterns to account for various rumble intensities
 - Added ability to adjust rumble intensity
 - HORI Battle Pad tested and works fully

 = version 1.0 =
 - First Release