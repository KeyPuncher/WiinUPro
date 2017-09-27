namespace Shared
{
    public static class Delegates
    {
        public delegate void BoolArrDel(bool[] arr);
        public delegate void StringDel(string str);
        public delegate void StringArrDel(string[] str);
        public delegate void JoystickDel(NintrollerLib.Joystick joy, string target, string file = "");
        public delegate void TriggerDel(NintrollerLib.Trigger trig, string target, string file = "");
        public delegate DeviceInfo ObtainDeviceInfo();
    }
}
