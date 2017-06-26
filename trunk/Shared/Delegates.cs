namespace Shared
{
    public static class Delegates
    {
        public delegate void BoolArrDel(bool[] arr);
        public delegate void StringDel(string str);
        public delegate void StringArrDel(string[] str);
        public delegate void JoystickeDel(NintrollerLib.Joystick joy, bool right); // not sure how I got that e in there, but works out I guess
        public delegate void JoystickDel(NintrollerLib.Joystick joy, string target);
        public delegate void TriggerDel(NintrollerLib.Trigger trig, string target);
    }
}
