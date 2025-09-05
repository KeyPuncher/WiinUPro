# About
There are two different applications here, **WiinUPro** and **WiinUSoft**. Both are free to use.

Both of these programs are for using the Wii/U familiy of Nintendo Bluetooth controllers with Windows.
Depending on how you want to use your controllers, one or the other might be more appropriate.
WiinUPro does all the things, and WiinUSoft is only for making them act like XInput (Xbox 360) controllers.

**Note**: Only official Nintendo controllers are officially supported in this unofficial software.

***Side Note***: Most of what these programs can do can also be accomplished with Steam. They actually have a paid team to implement that stuff and there's quite a bit of functionality, if you can figure out where to find all of it.

# I'll Read this stuff Later/Never, Gimmie my download link
If you already know what you want, head over to the [Releases Section](https://github.com/KeyPuncher/WiinUPro/releases). 
Find the latest *WiinUPro* or *WiinUSoft* Release.
Expand Assets.
Then download either the \_setup.exe or .zip file.

**Note**: Also requires [Visual C++ Redistributable for Visual Studio 2012 Update 4](https://www.microsoft.com/en-us/download/details.aspx?id=30679)

# I'm lost, where should I start from?
WiinUPro and WiinUSoft are provided in two forms: zip (portable) and exe (installer).
Read following sections to know which one fits best your needs.

## Option 1: emulating keystrokes with WiinUPro
Simplest (and most compatible) choice: map controller buttons to
keyboard. No need to install anything. [Download](https://github.com/KeyPuncher/WiinUPro/releases)
and extract portable `WiinUPro_*_.zip`, then open `32|64-bit/WiinUPro.exe`. Pair,
connect and add a controller. Then just map buttons to keystrokes and start
playing. You can also save and load profiles, to easily switch between mappings
(e.g., for different games). Analog sticks can be used as if they were digital
(either toggled or not - no intermediate states).

## Option 2: emulating XBox 360 controller with WiinUSoft 
For more features (vibration, full analog support), you'll need your device to
be recognized as an actual controller (an XBox 360 controller,
internally known as XInput controller). In that case,
[download](https://github.com/KeyPuncher/WiinUPro/releases)
and run the installer for either WiinUSoft or WiinUPro.
WiinU**Soft** is the way to go if you only need the XInput feature and no
keyboard mapping. During the installation process, you'll need to include
component SCP Driver. Please, read the [SCP Driver section below](#scp-driver).

## Option 3: all-in-one with WiinUPro
To be able to emulate both keystrokes AND Xbox 360 controller, you'll need to
*install* WiinU**Pro** (just running the portable version is not enough). By
installing it, you'll be able to do the job of WiinUSoft as well.
During the installation process, you'll need to include
component SCP Driver. Please, read the [SCP Driver section below](#scp-driver).

## Option 4: WiinUPro for keystrokes, WiinUSoft for XInput
The last, fully-fledged option is to adopt both WiinUPro and WiinUSoft. This may
be useful if you need WiinUPro features, but prefer the user experience of
WiinUSoft for XInput emulation. You can fully install either WiinUPro or
WiinUSoft and just run the portable version of the other app.
During the installation process, you'll need to include
component SCP Driver. Please, read the [SCP Driver section below](#scp-driver).

## SCP Driver
**SCP (Scarlet Crush Production) Driver** component is required in two
scenarios: 1) for WiinUSoft to work at all; 2) for WiinUPro to emulate XInput.

SCP Driver and other (optional) components will be automatically presented to
you during the installation process of either app. The portable (zip) version
contains these components as separate folders, to be installed manually.

# WiinUSoft
This is for people who just want their Nintendo controllers to behave like regular Windows compatible controllers.
With the help of some drivers (part of the installer setup) it will translate the inputs of one of your connected Nintendo Wii/U familiy controllers to an emulated XInput controller.

## Features
* Works with Wii U Pro Controller, Wiimote, Nunchuk, Classic Controller and Classic Controller Pro.
* Up to 4 controllers at a time (1 Ninty device per XInput device & 1 assignment per button)
* Force Feedback
* Customize Button Layout
* Some accelerometer support
* Designed to be a simple to use as possible.
* Controller calibration adjustments.

## Instructional Video
(old but relevant)
https://youtu.be/1HWVhmdL9Dc


# WiinUPro
This is for those who want the whole kit and caboodle or at least additional functionality that WiinUSoft does not have.
This will also allow you to emulate keyboard and mouse inputs and do all sorts of fancy things like emulate multiple controllers from a single device.
It also supports additional controller types and generic joysticks as well.

## Features
* Works with Wii U Pro Controller, Wiimote, Nunchuk, Classic Controller, Classic Controller Pro, Guitar, Taiko Drum, Game Cube Adapter, Switch Pro Controller, Switch Joy-Cons, and generic Windows joysticks.
* Any number of controllers
* Up to 4 emulated XInput devices (can be mapped to any combination of controllers many:many)
* Keyboard input emulation
* Mouse Input emulation
* Direct Input emulation support (via vJoy which is quite old)
* Multiple outputs per assignment
* Force Feedback via XInput
* Fine tuning of Joystick Calibrations
* Shift States!

## What are Shift States?
You know you your keyboard has a 1 key, but if you hold shift it all of a sudden outputs an ! instead of 1?
Well that's how shift states work. You can use a single button to output different things depending on your shift state.

Going back to the 1 key, some keyboards have an FN key that will also change the 1 key to output F1. That's 3 different outputs for the same key!
WiinUPro supports 4 states, Nothing active (default), Red, Blue, and Green.
Shifts can be assigned to other buttons so you can hold down say R and change your A button from emulating the A key to emulate Space Bar.
Shift assignments can also work like Caps Lock, where you press the button once to toggle the shifted to state.
AND, they can be cascaded. So while you are shifted into Red, that might change another button assignment that then lets you shift into Blue.

Fun stuff and useful for creating configurations for games that utilize a lot of keyboard keys or have vastly different controls for on-foot vs in-vechicle.

## Informational Videos:
[Youtube Tutorial Playlist](https://www.youtube.com/playlist?list=PLGGq1CxIWfVZkVLS2zDx1_O7VqjxdMHZ7)

## Language Support
WiinUPro currently has translations for *English, Español, Français, Polski, and Deutsch.

If you are someone who would like to add translations for another language, (first of all thanks) you can view the required translations in this [Google Doc Spreadsheet](https://docs.google.com/spreadsheets/d/1GBMtDP-JgyXad1y14ACACvXAcUiwBWiiSj1K1h9dmSs/edit?usp=sharing). Once you've added translations for a new language, create a ticket for that support work to be added to the software.

# Other Questions You May or May Not Ask
How do I pronounce this?
* Doesn't matter. I say Win-You-Pro but technically it's spelt like Wing-You-Pro.
I derrived it from the short version Windows - Win, mixed in Wii - Wiin, added the U from Wii U - WiinU, and the focus was the Wii U's Pro Controller (the whole point why I made the thing) so added on Pro = WiinUPro.
Soft from WiinUSoft is a play on Lite versions of software. I don't consider it a "Lite" version because it's not restricted functionality, it's just meant for a different audience.

Is this still in development?
* It's unclear, I know.
I come back to this project in random bursts when I feel like life has opened up and that I feel like I can spend time on it again and help random people on the internet who still use it.
I do really want to finally release WiinUSoft 1.0, and I think it's close now!

Will there be more to WiinUSoft?
* WiinUSoft is for the most part done. Aside from some technical upgrades, 3.5 is the version that will be around for a while until WiinUPro becomes feature complete, then features can trickle over.

What else is on the roadmap for WiinUPro?
* Now that there's some IR support (celebrate!), the only thing left is motion controls.
I'm not going to do anything too fancy because I'm suddently motivated to finally hit version 1.0, but basically gyro aiming and tilt controls.

What's the reason for the long periods of hiatus?
* Life stuff. If I include pets there's 6 other beings in my home I take care of.
I have a full time job and I'm attending graduate school.
Not to mention my in-laws are very family oriented so there's always calls to do this or that whenever spare time does open up.
I typically run on high stress and eventually it chills out for a bit and after I recuperate a little I start this project up again.
I know there are people who are busier than I am, but I'm the busiest person I currently know.
That was actually a major motivation for open sourcing the project and moving it to GitHub.

Is the Website coming back?
* Sorry but no.
I even consider the Google Sites one to be dead. Unsure of how people find their way there actually.
I still get drive requests to download a really old version of WiinUSoft.
Github is space for this project now.
I have dreamnt about designing a web page for the project but ultimately it's more busy work that'd get in the way of progress (but maybe).

Are you going to add Switch 2 Controller Support?
* Not to this project.

Why not?
* Because this project is hard to update. It may be the second iteration of the codebase and perhaps a little better than my original beta release, but it's still not great.
I'm a much more experienced engineer now, and I have aspirations to make software that is far more maintainable.
New controllers will continue to be released over time with different functionality and whatnot, it's unreasonable to keep coming back to this one and updating it in its current state.
So I've started writing a new software from scratch, one that is much more modular and aiming to support plugins to make it far easier to add new controller support.
Plus with the plugin approach, you won't have to install a new version every time a new controller is added to be supported, you would just download the plugin that adds that controller, probably through UI from the software itself.

What else do you do?
* I make games! I most recently participated in the GMTK 2025 Game Jam, where I helped produce a small game called [It's OverMan!](https://gamesovercoffee.itch.io/its-overman).
It was a fun experience, and long long long before that I made my first game to get my feet wet [Mega Maze](https://store.steampowered.com/app/658210/Mega_Maze) and even published it on Wii U.
Publishing is a pain! Both these games are free if you need some time to kill.
Currently, I can only make games in two ways; (i) via a Game Jam and (ii) via personal un-released projects. This is due to work restrictions.
There's also other personal projects I like to do, but I'm getting better at focusing on one thing at a time, so I really do want the WiinUPro success to be the next thing to release after version 1.0.
