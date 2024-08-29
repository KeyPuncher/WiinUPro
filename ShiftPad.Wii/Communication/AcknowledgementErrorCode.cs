namespace ShiftPad.Wii.Communication
{
    internal enum AcknowledgementErrorCode : byte
    {
        Success = 0,
        Error = 0x03,
        UnknownAddress = 0x04,
        UnknownDataReport = 0x05,
        UnknownWriteOperation = 0x08
    }
}
