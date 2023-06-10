# About
There are two different applications here, **WiinUPro** and **WiinUSoft**. Both are free to use.

Both of these programs are for using the Wii/U familiy of Nintendo Bluetooth controllers with Windows.
Depending on how you want to use your controllers, one or the other might be more appropriate.
WiinUPro does all the things, and WiinUSoft is only for making them act like XInput (Xbox 360) controllers.

**Note**: Only official Nintendo controllers are officially supported in this unofficial software.

***Side Note***: Most of what these programs can do can also be accomplished with Steam. They actually have a paid team to implement that stuff and there's quite a bit of funcitonality, if you can figure out where to find all of it.

# I'll Read this stuff Later/Never, Gimmie my download link
If you already know what you want, head over to the [Releases Section](https://github.com/KeyPuncher/WiinUPro/releases). 
Find the latest *WiinUPro* or *WiinUSoft* Release.
Expand Assets.
Then download either the \_setup.exe or .zip file.

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

# Other Questions You May or May Not Ask
How do I pronounce this?
* Doesn't matter. I say Win-You-Pro but technically it's spelt like Wing-You-Pro.
I derrived it from the short version Windows - Win, mixed in Wii - Wiin, added the U from Wii U - WiinU, and the focus was the Wii U's Pro Controller (the whole point why I mad the thing) so added on Pro = WiinUPro.
Soft from WiinUSoft is a play on Lite versions of software. I don't consider it a "Lite" version because it's not restricted functionality, it's just meant for a different audience.

Is this still in development?
* It's unclear, I know.
I come back to this project in random bursts when I feel like life has opened up and that I feel like I can spend time on it again and help random people on the internet who stil use it.
I would love to finish it and finally release WiinUSoft 1.0, but it just feels so far away. There's so many other projects I wish to work on as well, but time does not permit.
Heck, it's mid 2023 and I'm just now adding a readme after being on Github since 2017. I first created my little side project here two decades ago.

Will there be more to WiinUSoft?
* WiinUSoft is for the most part done. Aside from some technical upgrades, 3.5 is the version that will be around for a while until WiinUPro becomes feature complete, then features can trickle over.

What else is on the roadmap for WiinUPro?
* Roadmap is used very loosely here FYI.
I really really want to add easy to configure and use gyro controls and IR calibration.
Sure it has some IR support now, but calibration could be so much easier.
Plus IR could be obsolete with the motion plus capabilities.
I have ideas, it just requires a lot of work and short bursts don't work well for that sort of level of changes. I did start a branch though.

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
I have dreamnt about designing a web page for the project but ultimately it's more busy work that'd get in the way of progress.

You mentioned other projects?
* There are many things I've been wanting to do that I've thought about for years, decades, and even minutes.
Nothing to report on most them. I've had to come to terms with abandoning some, for example a handheld retro pi I started 8 years ago I've finally let go of even after all the work of only think left to do is make the case and wire up the joysticks.
Most recent nearly completed one was a custom split keyboard of my own liking.
Was going to be modular so you can hot swap out one side of it for gaming reasons.
I had the full qwerty keyboard working, in case and all but some wiring issues happened (I think?) and one side stopped talking to the other so that's been set aside for some months now.
Going to have to redesign the case and make a circuit board this time to avoid wireing mishaps.
Another one is I started working on a Game Boy emulator. My first emulator yet.
I know there are tons out there but I've always wanted to learn how to make one and I want to do it my way.
Plus it's very important for me to understand how the Game Boy works for a dream project of mine.

What is this a blog? No more questions. I'll probably have to come back and prune this stuff out at some point. I'm in the wrong region to say this but, Cheers.
