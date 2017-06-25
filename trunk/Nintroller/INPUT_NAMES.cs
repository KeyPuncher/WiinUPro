using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib
{
    public static class INPUT_NAMES
    {
        public static class WIIMOTE
        {
            public const string A           = "wA";
            public const string B           = "wB";
            public const string ONE         = "wONE";
            public const string TWO         = "wTWO";

            // dpad when wiimote is vertical
            public const string UP          = "wUP";
            public const string DOWN        = "wDOWN";
            public const string LEFT        = "wLEFT";
            public const string RIGHT       = "wRIGHT";

            public const string MINUS       = "wMINUS";
            public const string PLUS        = "wPLUS";
            public const string HOME        = "wHOME";

            // Accelerometer
            public const string ACC_X       = "wAccX";
            public const string ACC_Y       = "wAccY";
            public const string ACC_Z       = "wAccZ";
            // tilting the controler with the wrist
            public const string TILT_RIGHT  = "wTILTRIGHT";
            public const string TILT_LEFT   = "wTILTLEFT";
            public const string TILT_UP     = "wTILTUP";
            public const string TILT_DOWN   = "wTILTDOWN";
            public const string FACE_UP     = "wTILTFACEUP";
            public const string FACE_DOWN   = "wTILTFACEDOWN";

            // Pointer from IR camera
            public const string IR_X        = "wIRX";
            public const string IR_Y        = "wIRY";
            public const string IR_UP       = "wIRUP";
            public const string IR_DOWN     = "wIRDOWN";
            public const string IR_LEFT     = "wIRLEFT";
            public const string IR_RIGHT    = "wIRRIGHT";
        }

        public static class NUNCHUK
        {
            public const string C            = "nC";
            public const string Z            = "nZ";

            public const string JOY_X        = "nJoyX";
            public const string JOY_Y        = "nJoyY";

            public const string UP           = "nUP";
            public const string DOWN         = "nDOWN";
            public const string LEFT         = "nLEFT";
            public const string RIGHT        = "nRIGHT";

            public const string ACC_X        = "nAccX";
            public const string ACC_Y        = "nAccY";
            public const string ACC_Z        = "nAccZ";
            // tilting the controler with the wrist
            public const string TILT_RIGHT   = "nTILTRIGHT";
            public const string TILT_LEFT    = "nTILTLEFT";
            public const string TILT_UP      = "nTILTUP";
            public const string TILT_DOWN    = "nTILTDOWN";
            public const string FACE_UP      = "nTILTFACEUP";
            public const string FACE_DOWN    = "nTILTFACEDOWN";
        }

        public static class CLASSIC_CONTROLLER
        {
            public const string A      = "ccA";
            public const string B      = "ccB";
            public const string X      = "ccX";
            public const string Y      = "ccY";

            public const string UP     = "ccUP";
            public const string DOWN   = "ccDOWN";
            public const string LEFT   = "ccLEFT";
            public const string RIGHT  = "ccRIGHT";

            public const string L      = "ccL";
            public const string R      = "ccR";
            public const string ZL     = "ccZL";
            public const string ZR     = "ccZR";

            public const string LX     = "ccLX";
            public const string LY     = "ccLY";
            public const string RX     = "ccRX";
            public const string RY     = "ccRY";

            public const string LUP    = "ccLUP";
            public const string LDOWN  = "ccLDOWN";
            public const string LLEFT  = "ccLLEFT";
            public const string LRIGHT = "ccLRIGHT";

            public const string RUP    = "ccRUP";
            public const string RDOWN  = "ccRDOWN";
            public const string RLEFT  = "ccRLEFT";
            public const string RRIGHT = "ccRRIGHT";

            public const string LT     = "ccLT";
            public const string RT     = "ccRT";
            public const string LFULL  = "ccLFULL";
            public const string RFULL  = "ccRFULL";

            public const string SELECT = "ccSELECT";
            public const string START  = "ccSTART";
            public const string HOME   = "ccHOME";
        }

        public static class CLASSIC_CONTROLLER_PRO
        {
            public const string A      = "ccpA";
            public const string B      = "ccpB";
            public const string X      = "ccpX";
            public const string Y      = "ccpY";

            public const string UP     = "ccpUP";
            public const string DOWN   = "ccpDOWN";
            public const string LEFT   = "ccpLEFT";
            public const string RIGHT  = "ccpRIGHT";

            public const string L      = "ccpL";
            public const string R      = "ccpR";
            public const string ZL     = "ccpZL";
            public const string ZR     = "ccpZR";

            public const string LX     = "ccpLX";
            public const string LY     = "ccpLY";
            public const string RX     = "ccpRX";
            public const string RY     = "ccpRY";

            public const string LUP    = "ccpLUP";
            public const string LDOWN  = "ccpLDOWN";
            public const string LLEFT  = "ccpLLEFT";
            public const string LRIGHT = "ccpLRIGHT";

            public const string RUP    = "ccpRUP";
            public const string RDOWN  = "ccpRDOWN";
            public const string RLEFT  = "ccpRLEFT";
            public const string RRIGHT = "ccpRRIGHT";

            public const string SELECT = "ccpSELECT";
            public const string START  = "ccpSTART";
            public const string HOME   = "ccpHOME";
        }

        public static class PRO_CONTROLLER
        {
            public const string A      = "proA";
            public const string B      = "proB";
            public const string X      = "proX";
            public const string Y      = "proY";

            public const string UP     = "proUP";
            public const string DOWN   = "proDOWN";
            public const string LEFT   = "proLEFT";
            public const string RIGHT  = "proRIGHT";

            public const string L      = "proL";
            public const string R      = "proR";
            public const string ZL     = "proZL";
            public const string ZR     = "proZR";

            public const string LX     = "proLX";
            public const string LY     = "proLY";
            public const string RX     = "proRX";
            public const string RY     = "proRY";

            public const string LUP    = "proLUP";
            public const string LDOWN  = "proLDOWN";
            public const string LLEFT  = "proLLEFT";
            public const string LRIGHT = "proLRIGHT";

            public const string RUP    = "proRUP";
            public const string RDOWN  = "proRDOWN";
            public const string RLEFT  = "proRLEFT";
            public const string RRIGHT = "proRRIGHT";

            public const string LS     = "proLS";
            public const string RS     = "proRS";

            public const string SELECT = "proSELECT";
            public const string START  = "proSTART";
            public const string HOME   = "proHOME";
        }

        public static class MOTION_PLUS
        {
            public const string GYROUP       = "mpGYROUP";
            public const string GYRODOWN     = "mpGYRODOWN";
            public const string GYROLEFT     = "mpGYROLEFT";
            public const string GYRORIGHT    = "mpGYRORIGHT";
            public const string GYROFORWARD  = "mpGYROFORWARD";
            public const string GYROBACKWARD = "mpGYROBACKWARD";
        }
    }
}
