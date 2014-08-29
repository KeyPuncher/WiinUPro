// Holds all the XInput Constants

using System;
using System.Collections.Generic;


    public static class XConstants
    {
        /**** XInput_CAPABILITIES ****/
        // Device Types
        public static long XINPUT_DEVTYPE_GAMEPAD = 0x01;

        // device subtypes
        public static long XINPUT_DEVSUBTYPE_UNKNOWN = 0x00;
        public static long XINPUT_DEVSUBTYPE_GAMEPAD = 0x01;
        public static long XINPUT_DEVSUBTYPE_WHEEL = 0x02;
        public static long XINPUT_DEVSUBTYPE_ARCADE_STICK = 0x03;
        public static long XINPUT_DEVSUBTYPE_FLIGHT_STICK = 0x04;
        public static long XINPUT_DEVSUBTYPE_DANCE_PAD = 0x05;
        public static long XINPUT_DEVSUBTYPE_GUITAR = 0x06;
        public static long XINPUT_DEVSUBTYPE_GUITAR_ALTERNATE = 0x07;
        public static long XINPUT_DEVSUBTYPE_DRUM_KIT = 0x08;
        public static long XINPUT_DEVSUBTYPE_GUITAR_BASS = 0x0B;
        public static long XINPUT_DEVSUBTYPE_ARCADE_PAD = 0x13;

        // Flags
        public static long XINPUT_CAPS_VOICE_SUPPORTED = 0x0004;
        public static long XINPUT_CAPS_FFB_SUPPORTED = 0x0001;
        public static long XINPUT_CAPS_PMD_SUPPORTED = 0x0008;
        public static long XINPUT_CAPS_NO_NAVIGATION = 0x0010;

        public static long XINPUT_FLAG_GAMEPAD = 0x00000001;

        /**** XInput_Battery_Level ****/
        // Battery supported devices
        public static long BATTERY_DEVTYPE_GAMEPAD = 0x00;
        public static long BATTERY_DEVTYPE_HEADSET = 0x01;

        // Flags
        public static long BATTERY_TYPE_DISCONNECTED = 0x00;
        public static long BATTERY_TYPE_WIRED = 0x01;
        public static long BATTERY_TYPE_ALKALINE = 0x02;
        public static long BATTERY_TYPE_NIMH = 0x03;
        public static long BATTERY_TYPE_UNKNOWN = 0xFF;

        // Level
        public static long BATTERY_LEVEL_EMPTY = 0x00;
        public static long BATTERY_LEVEL_LOW = 0x01;
        public static long BATTERY_LEVEL_MEDIUM = 0x02;
        public static long BATTERY_LEVEL_FULL = 0x03;
    }

