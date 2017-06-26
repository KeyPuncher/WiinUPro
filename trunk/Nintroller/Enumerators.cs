namespace NintrollerLib
{
    public enum ControllerType : long
    {
        Unknown              = -1,              // not yet determined
        Wiimote              = 0x000000000000,  // Wiimote, no extension
        ProController        = 0x0000A4200120,  // Wii U Pro Controller
        BalanceBoard         = 0x0000A4200402,  // Balance Board
        Nunchuk              = 0x0000A4200000,  // Wiimote + Nunchuk
        NunchukB             = 0xFF00A4200000,  // Wiimote + Nunchuk (type 2)
        ClassicController    = 0x0000A4200101,  // Wiimote + Classic Controller
        ClassicControllerPro = 0x0100A4200101,  // Wiimote + Classic Controller Pro
        MotionPlus           = 0x0000A4200405,  // Wiimote + Motion Plus
        // Musicals
        Guitar               = 0x0000A4200103,
        Drums                = 0x0100A4200103,
        TaikoDrum            = 0x0000A4200111,

        PartiallyInserted    = 0xFFFFFFFFFFFF
    };

    public enum BatteryStatus
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh
    };

    public enum IRCamMode : byte
    {
        Off   = 0x00,
        Basic = 0x01,     // 10 bytes
        Wide  = 0x03,     // 12 bytes
        Full  = 0x05      // two sets of 16 bytes (best to avoid)
    };

    public enum IRCamSensitivity
    {
        Level1,         // 02 00 00 71 01 00 64 00 FE; FD 05
        Level2,         // 02 00 00 71 01 00 96 00 B4; B3 04
        Level3,         // 02 00 00 71 01 00 aa 00 64; 63 03
        Level4,         // 02 00 00 71 01 00 c8 00 36; 35 03
        Level5,         // 07 00 00 71 01 00 72 00 20; 1F 03
        Custom,         // 00 00 00 00 00 00 90 00 C0; 40 00
        CustomMax,      // 00 00 00 00 00 00 FF 00 0C; 00 00
        CustomHigh      // 00 00 00 00 00 00 90 00 41; 40 00
    };

    public enum InputReport : byte
    {
        // Status Reports
        Status       = 0x20,
        ReadMem      = 0x21,
        Acknowledge  = 0x22,

        // Data Reports
        BtnsOnly     = 0x30,
        BtnsAcc      = 0x31,
        BtnsExt      = 0x32,
        BtnsAccIR    = 0x33,
        BtnsExtB     = 0x34,
        BtnsAccExt   = 0x35,
        BtnsIRExt    = 0x36,
        BtnsAccIRExt = 0x37,
        ExtOnly      = 0x3D,
    };

    public enum OutputReport : byte
    {
        LEDs           = 0x11,
        DataReportMode = 0x12,
        IREnable       = 0x13,
        SpeakerEnable  = 0x14,
        StatusRequest  = 0x15,
        WriteMemory    = 0x16,
        ReadMemory     = 0x17,
        SpeakerData    = 0x18,
        SpeakerMute    = 0x19,
        IREnable2      = 0x1A
    };

    public enum ReadReportType
    {
        Unknown,
        Extension_A,
        Extension_B,
        EnableIR
    };

    internal enum AcknowledgementType
    {
        NA,
        IR_Step1,
        IR_Step2,
        IR_Step3,
        IR_Step4,
        IR_Step5
    }

    internal enum StatusType
    {
        Unknown,
        Requested,
        IR_Enable,
        DiscoverExtension
    }
}
