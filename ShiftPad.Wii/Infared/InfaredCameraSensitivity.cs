namespace ShiftPad.Wii.Infared
{
    public enum InfaredCameraSensitivity
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
}
