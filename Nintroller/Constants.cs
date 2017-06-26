namespace NintrollerLib
{
    internal static class Constants
    {
        // Vendor ID (Nintendo) & Product IDs
        public const int VID    = 0x057e;
        public const int PID1   = 0x0306;       // Legacy Wiimotes
        public const int PID2   = 0x0330;       // Newer Wiimotes (And Pro Controllers)

        // Report Size, There are several reports to choose from
        public const int REPORT_LENGTH = 22;   // Buttons, Accelerometer, IR, and Extension

        // Wiimote Registers
        public const int REGISTER_IR = 0x04b00030;
        public const int REGISTER_IR_SENSITIVITY_1 = 0x04b00000;
        public const int REGISTER_IR_SENSITIVITY_2 = 0x04b0001a;
        public const int REGISTER_IR_MODE = 0x04b00033;
        public const int REGISTER_EXTENSION_INIT_1 = 0x04a400f0;
        public const int REGISTER_EXTENSION_INIT_2 = 0x04a400fb;
        public const int REGISTER_EXTENSION_TYPE = 0x04a400fa;
        public const int REGISTER_EXTENSION_TYPE_2 = 0x04a400fe;
        public const int REGISTER_EXTENSION_CALIBRATION = 0x04a40020;
        public const int REGISTER_MOTIONPLUS_INIT = 0x04a600fe;

        // Length and Width between Balance Board Sensors
        public const int BB_LENGTH = 43;
        public const int BB_WIDTH = 24;

        // Pound - KG Conversion
        public const float KG_TO_LBS = 2.20462262f;
    }
}
