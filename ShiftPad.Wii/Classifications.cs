namespace ShiftPad.Wii
{
    public class Classifications
    {
        public const uint GAMEPAD_TYPE_WII = 100;
        /// <summary> Wii Remote without extension. </summary>
        public const ulong Wiimote              = 0x000000000000;
        /// <summary> Wii Fit Balance Board. </summary>
        public const ulong BalanceBoard         = 0x0000A4200402;
        /// <summary> Wii Remote with Nunchuk. </summary>
        public const ulong Nunchuk              = 0x0000A4200000;
        /// <summary> Wii Remote With Nunchuk. </summary>
        public const ulong NunchukB             = 0xFF00A4200000;
        /// <summary> Wii Remote with Classic Controller. </summary>
        public const ulong ClassicController    = 0x0000A4200101;
        /// <summary> Wii Remote with Classic Controller Pro. </summary>
        public const ulong ClassicControllerPro = 0x0100A4200101;
        /// <summary> Wii Remote with Motion Plus. </summary>
        public const ulong MotionPlus           = 0x0000A4200405;
        /// <summary> Wii Remote with Motion Plus and Nunchuk. </summary>
        public const ulong MotionPlusNunchuk    = 0x0000A4200505;
        // TODO: Check if there is a MotionPlusNunchukB to add.
        /// <summary> Wii Remote with Motion Plus and Classic Controller. </summary>
        public const ulong MotionPlusCC         = 0x0000A4200705;
        // TODO: Check if there is a MotionPlusCCP to add
        /// <summary> Wii Remote with Guitar. </summary>
        public const ulong Guitar               = 0x0000A4200103;
        /// <summary> Wii Remote with Drum Set. </summary>
        public const ulong Drums                = 0x0100A4200103;
        /// <summary> Wii Remote with Taiko Drum. </summary>
        public const ulong TaikoDrum            = 0x0000A4200111;
        /// <summary> Wii Remote with Turn Table. </summary>
        public const ulong TurnTable            = 0x0300A4200103;
        /// <summary> Wii Remote with Draw Tablet. </summary>
        public const ulong DrawTablet           = 0xFF00A4200013;
        /// <summary> Invalid State. </summary>
        public const ulong FalseState           = 0x010000000000;
        /// <summary> Not fully inserted extension. </summary>
        public const ulong PartiallyInserted    = 0xFFFFFFFFFFFF;
    }
}
