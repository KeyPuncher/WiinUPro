namespace ShiftPad.Wii.Infared
{
    public enum InfaredCameraMode : byte
    {
        Off = 0x00,
        Basic = 0x01,     // 10 bytes
        Wide = 0x03,     // 12 bytes
        Full = 0x05      // two sets of 16 bytes (best to avoid)
    };
}
