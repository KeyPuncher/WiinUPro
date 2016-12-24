using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public static class Delegates
    {
        public delegate void BoolArrDel(bool[] arr);
        public delegate void StringDel(string str);
        public delegate void JoystickeDel(NintrollerLib.Joystick joy, bool right);
    }
}
