namespace ShiftPad.Wii.Communication
{
    internal enum InputReport : byte
    {
        // Status Reports
        Status = 0x20,
        ReadMemory = 0x21,
        Acknowledge = 0x22,

        // Data Reports
        BtnsOnly = 0x30,
        BtnsAcc = 0x31,
        BtnsExt = 0x32,
        BtnsAccIR = 0x33,
        BtnsExtB = 0x34,
        BtnsAccExt = 0x35,
        BtnsIRExt = 0x36,
        BtnsAccIRExt = 0x37,
        ExtOnly = 0x3D,
    };
}
