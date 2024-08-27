namespace ShiftPad.Wii.Communication
{
    internal enum OutputReport : byte
    {
        LEDs = 0x11,
        DataReportMode = 0x12,
        IREnable = 0x13,
        SpeakerEnable = 0x14,
        StatusRequest = 0x15,
        WriteMemory = 0x16,
        ReadMemory = 0x17,
        SpeakerData = 0x18,
        SpeakerMute = 0x19,
        IREnable2 = 0x1A
    };
}
