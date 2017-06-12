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
            public const string A           = "wmA";
            public const string B           = "wmB";
            public const string ONE         = "wmONE";
            public const string TWO         = "wmTWO";

            // dpad when wiimote is vertical
            public const string UP          = "wmUP";
            public const string DOWN        = "wmDOWN";
            public const string LEFT        = "wmLEFT";
            public const string RIGHT       = "wmRIGHT";

            public const string MINUS       = "wmMINUS";
            public const string PLUS        = "wmPLUS";
            public const string HOME        = "wmHOME";

            public const string ACC_X       = "wmAccX";
            public const string ACC_Y       = "wmAccY";
            public const string ACC_Z       = "wmAccZ";

            // Pointer from IR camera
            public const string IR_X         = "wmIRX";
            public const string IR_Y         = "wmIRY";
        }

        public static class NUNCHUK
        {
            public const string C            = "nunC";
            public const string Z            = "nunZ";

            public const string UP           = "nunUP";
            public const string DOWN         = "nunDOWN";
            public const string LEFT         = "nunLEFT";
            public const string RIGHT        = "nunRIGHT";

            public const string ACC_X        = "nunAccX";
            public const string ACC_Y        = "nunAccY";
            public const string ACC_Z        = "nunAccZ";
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
