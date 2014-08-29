using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUSoft
{
    public class XReport
    {
        private int ID;

        #region Analog Inputs
        public double LX = 0.5;
        public double LY = 0.5;
        public double RX = 0.5;
        public double RY = 0.5;

        public double TriggerL;
        public double TriggerR;
        #endregion

        #region Digital Inputs
        public bool A;
        public bool B;
        public bool X;
        public bool Y;

        public bool Up;
        public bool Down;
        public bool Right;
        public bool Left;

        public bool BumperL;
        public bool BumperR;
        public bool LS;
        public bool RS;

        public bool Start;
        public bool Back;
        public bool Guide;
        #endregion

        public XReport(int id)
        {
            ID = id;
        }

        public Byte[] ToBytes()
        {
            Byte[] buff = new Byte[28];

            buff[0] = (Byte)ID;
            buff[1] = 0x02;

            buff[10] = 0;
            buff[11] = 0;
            buff[12] = 0;
            buff[13] = 0;

            buff[10] |= (Byte)(Back  ? 1 << 0 : 0);
            buff[10] |= (Byte)(LS    ? 1 << 1 : 0);
            buff[10] |= (Byte)(RS    ? 1 << 2 : 0);
            buff[10] |= (Byte)(Start ? 1 << 3 : 0);

            buff[10] |= (Byte)(Up    ? 1 << 4 : 0);
            buff[10] |= (Byte)(Down  ? 1 << 5 : 0);
            buff[10] |= (Byte)(Right ? 1 << 6 : 0);
            buff[10] |= (Byte)(Left  ? 1 << 7 : 0);

            buff[11] |= (Byte)(BumperL ? 1 << 2 : 0);
            buff[11] |= (Byte)(BumperR ? 1 << 3 : 0);

            buff[11] |= (Byte)(Y ? 1 << 4 : 0);
            buff[11] |= (Byte)(B ? 1 << 5 : 0);
            buff[11] |= (Byte)(A ? 1 << 6 : 0);
            buff[11] |= (Byte)(X ? 1 << 7 : 0);

            buff[12] |= (Byte)(Guide ? 1 << 0 : 0);

            buff[14] = (Byte)((GetRawAxis(LX) >> 0) & 0xFF);
            buff[15] = (Byte)((GetRawAxis(LX) >> 8) & 0xFF);

            buff[16] = (Byte)((GetRawAxis(LY) >> 0) & 0xFF);
            buff[17] = (Byte)((GetRawAxis(LY) >> 8) & 0xFF);

            buff[18] = (Byte)((GetRawAxis(RX) >> 0) & 0xFF);
            buff[19] = (Byte)((GetRawAxis(RX) >> 8) & 0xFF);

            buff[20] = (Byte)((GetRawAxis(RY) >> 0) & 0xFF);
            buff[21] = (Byte)((GetRawAxis(RY) >> 8) & 0xFF);

            buff[26] = GetRawTrigger(TriggerL);
            buff[27] = GetRawTrigger(TriggerR);

            return buff;
        }

        public Int32 GetRawAxis(double axis)
        {
            if (axis > 1.0)
            {
                return 32767;
            }
            if (axis < 0.0)
            {
                return -32767;
            }

            return (Int32)((axis - 0.5) * 2 * 32767);
        }

        public Byte GetRawTrigger(double trigger)
        {
            if (trigger > 1.0)
            {
                return 255;
            }
            if (trigger < 0.0)
            {
                return 0;
            }

            return (Byte)(trigger * 255);
        }

        public void SetButton(string assignment, bool value)
        {
            switch (assignment.ToLower())
            {
                case "a":
                    A = value;
                    break;

                case "b":
                    B = value;
                    break;

                case "x":
                    X = value;
                    break;

                case "y":
                    Y = value;
                    break;

                case "up":
                    Up = value;
                    break;

                case "down":
                    Down = value;
                    break;

                case "left":
                    Left = value;
                    break;

                case "right":
                    Right = value;
                    break;

                case "start":
                    Start = value;
                    break;

                case "back":
                    Back = value;
                    break;

                case "guide":
                    Guide = value;
                    break;

                case "lbumper":
                    BumperL = value;
                    break;

                case "rbumper":
                    BumperR = value;
                    break;

                case "ltrigger":
                    TriggerL = value ? 1.0 : 0;
                    break;

                case "rtrigger":
                    TriggerR = value ? 1.0 : 0;
                    break;

                case "lstick":
                    LS = value;
                    break;

                case "rstick":
                    RS = value;
                    break;

                case "lx":
                    LX = value ? 1.0 : 0;
                    break;

                case "ly":
                    LY = value ? 1.0 : 0;
                    break;

                case "rx":
                    RX = value ? 1.0 : 0;
                    break;

                case "ry":
                    RY = value ? 1.0 : 0;
                    break;

                default:
                    break;
            }
        }

        // TODO: Set Analog
    }
}
