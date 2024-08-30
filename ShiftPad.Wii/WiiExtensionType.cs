namespace ShiftPad.Wii
{
    public enum WiiExtensionType : long
    {
        Unknown = -1,              // not yet determined
        Wiimote = 0x000000000000,  // Wiimote, no extension
        ProController = 0x0000A4200120,  // Wii U Pro Controller
        BalanceBoard = 0x0000A4200402,  // Balance Board
        Nunchuk = 0x0000A4200000,  // Wiimote + Nunchuk
        NunchukB = 0xFF00A4200000,  // Wiimote + Nunchuk (type 2)
        ClassicController = 0x0000A4200101,  // Wiimote + Classic Controller
        ClassicControllerPro = 0x0100A4200101,  // Wiimote + Classic Controller Pro
        MotionPlus = 0x0000A4200405,  // Wiimote + Motion Plus
        MotionPlusNunchuk = 0x0000A4200505,  // Wiimote + Motion Plus + Nunchuk passthrough
        MotionPlusNunchukB = 0xFF00A4200505,  // Wiimote + Motion Plus + Nunchuk passthrough
        MotionPlusCC = 0x0000A4200705,  // Wiimote + Motion Plus + Classic Controller passthrough
        Guitar = 0x0000A4200103,
        Drums = 0x0100A4200103,
        TaikoDrum = 0x0000A4200111,
        TurnTable = 0x0300A4200103,
        DrawTablet = 0xFF00A4200013,

        FalseState = 0x010000000000,  // Seen when reconnecting to a Pro Controller
        PartiallyInserted = 0xFFFFFFFFFFFF
    };
}
