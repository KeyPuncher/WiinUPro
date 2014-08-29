

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XAgentCS
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XInputGamepad
    {
        public UInt16 wButtons;
        public Byte bLeftTrigger;
        public Byte bRightTrigger;
        public Int16 sThumbLX;
        public Int16 sThumbLY;
        public Int16 sThumbRX;
        public Int16 sThumbRY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInputState
    {
        public UInt32 dwPacketNumber;
        public XInputGamepad Gamepad;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInputVibration
    {
        public UInt16 wLeftMotorSpeed;
        public UInt16 wRightMotorSpeed;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XInputCapabilities
    {
        public Byte type;
        public Byte subType;
        public UInt16 Flags;
        public XInputGamepad Gamepad;
        public XInputVibration Vibration;
    }

    // !XInput_USE_9_1_0
    [StructLayout(LayoutKind.Sequential)]
    public struct XInputBatteryInformation
    {
        public Byte BatteryType;
        public Byte BatteryLevel;
    }

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public struct XInputKeystroke
    {
        public UInt16 VirtualKey;
        public Char Unicode;
        public UInt16 Flags;
        public Byte UserIndex;
        public Byte HidCode;
    }

    public class WrapClass
    {

        [DllImport("xinput1_3.dll")]
        public static extern UInt32 XInputGetState(UInt32 dwUserIndex, ref XInputState pState);

        [DllImport("xinput1_3.dll")]
        public static extern UInt32 XInputSetState(UInt32 dwUserIndex, ref XInputVibration pVibration);

        [DllImport("xinput1_3.dll")]
        public static extern UInt32 XInputGetCapabilities(UInt32 dwUserIndex, UInt32 dwFlags, ref XInputCapabilities pCapabilities);

        [DllImport("xinput1_3.dll")]
        public static extern void XInputEnable(long enable);

        [DllImport("xinput1_3.dll")]
        public static extern UInt32 XInputGetBatteryInformation(UInt32 dwUserIndex, Byte devType, ref XInputBatteryInformation pBatteryInformation);

        [DllImport("xinput1_3.dll")]
        public static extern UInt32 XInputGetKeystroke(UInt32 dwUserIndex, UInt32 dwReserved, XInputKeystroke pKeystroke);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);
    }
}
