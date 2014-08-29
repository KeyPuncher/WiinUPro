using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WiinUSoft
{
    public class XController
    {
        // buttons
        public bool
            A, B, X, Y,
            Up, Down, Left, Right,
            L, R, LS, RS,
            Start, Select, Guide;

        // axes
        public short
            LX, LY, RX, RY;

        // triggers
        public byte
            LT, RT;

        // rumble
        public bool
            rumble;

        public XController()
        {
            LT = RT = Byte.MinValue;
        }

        public void SetValueOR(string varName, bool value)
        {
            if (value == false) return;

            switch (varName)
            {
                case "A"       : A = true; break;
                case "B"       : B = true; break;
                case "X"       : X = true; break;
                case "Y"       : Y = true; break;
                case "Up"      : Up = true; break;
                case "Down"    : Down = true; break;
                case "Left"    : Left = true; break;
                case "Right"   : Right = true; break;
                case "LBumper" : L = true; break;
                case "RBumper" : R = true; break;
                case "LStick"  : LS = true; break;
                case "RStick"  : RS = true; break;
                case "Start"   : Start = true; break;
                case "Back"    : Select = true; break;
                case "Guide"   : Guide = true; break;

                case "LTrigger": LT = Byte.MaxValue; break;
                case "RTrigger": RT = Byte.MaxValue; break;

                default        : break;
            }
        }
    }

    public enum xValue : ushort
    {
        Up      = 1,
        Down    = 2,
        Left    = 4,
        Right   = 8,
        Start   = 16,
        Select  = 32,
        LS      = 64,
        RS      = 128,
        L       = 256,
        R       = 512,
        // ??
        // ??
        A       = 4096,
        B       = 8192,
        X       = 16384,
        Y       = 32768,
        Axis    = 32760
    };
}
