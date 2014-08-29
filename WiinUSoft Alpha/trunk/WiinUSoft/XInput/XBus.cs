using System;
using ScpControl;

namespace WiinUSoft
{
    public partial class XBus : BusDevice
    {
        private static XBus defaultInstance;

        // Default Bus
        public static XBus Default
        {
            get
            {
                // if it hans't been created create one
                if (defaultInstance == null)
                {
                    defaultInstance = new XBus();
                    defaultInstance.Open();
                    defaultInstance.Start();
                }

                return defaultInstance;
            }
        }

        public XBus()
        {
            /*App.Current.Dispatcher.BeginInvoke(new Action(delegate()
            {
                App.Current.Exit += OnAppExit;
            }), null);*/
        }

        /*private void OnAppExit(object sender, System.Windows.ExitEventArgs e)
        {
            this.Stop();
            this.Close();
        }*/

        public static void ExitApp()
        {
            defaultInstance.Stop();
            defaultInstance.Close();
        }

        public override int Parse(byte[] Input, byte[] Output, DsModel Type = DsModel.DS3)
        {
            for (int index = 0; index < 28; index++)
            {
                Output[index] = 0x00;
            }

            Output[0] = 0x1C;
            Output[4] = Input[0];
            Output[9] = 0x14;

            if (Input[1] == 0x02) // Pad is active
            {
                UInt32 Buttons = (UInt32)((Input[10] << 0) | (Input[11] << 8) | (Input[12] << 16) | (Input[13] << 24));

                if ((Buttons & (0x1 << 0)) > 0) Output[10] |= (Byte)(1 << 5); // Back
                if ((Buttons & (0x1 << 1)) > 0) Output[10] |= (Byte)(1 << 6); // Left  Thumb
                if ((Buttons & (0x1 << 2)) > 0) Output[10] |= (Byte)(1 << 7); // Right Thumb
                if ((Buttons & (0x1 << 3)) > 0) Output[10] |= (Byte)(1 << 4); // Start

                if ((Buttons & (0x1 << 4)) > 0) Output[10] |= (Byte)(1 << 0); // Up
                if ((Buttons & (0x1 << 5)) > 0) Output[10] |= (Byte)(1 << 1); // Down
                if ((Buttons & (0x1 << 6)) > 0) Output[10] |= (Byte)(1 << 3); // Right
                if ((Buttons & (0x1 << 7)) > 0) Output[10] |= (Byte)(1 << 2); // Left

                if ((Buttons & (0x1 << 10)) > 0) Output[11] |= (Byte)(1 << 0); // Left  Shoulder
                if ((Buttons & (0x1 << 11)) > 0) Output[11] |= (Byte)(1 << 1); // Right Shoulder

                if ((Buttons & (0x1 << 12)) > 0) Output[11] |= (Byte)(1 << 7); // Y
                if ((Buttons & (0x1 << 13)) > 0) Output[11] |= (Byte)(1 << 5); // B
                if ((Buttons & (0x1 << 14)) > 0) Output[11] |= (Byte)(1 << 4); // A
                if ((Buttons & (0x1 << 15)) > 0) Output[11] |= (Byte)(1 << 6); // X

                if ((Buttons & (0x1 << 16)) > 0) Output[11] |= (Byte)(1 << 2); // Guide

                Output[12] = Input[26]; // Left Trigger
                Output[13] = Input[27]; // Right Trigger

                Output[14] = Input[14]; // LX
                Output[15] = Input[15];

                Output[16] = Input[16]; // LY
                Output[17] = Input[17];

                Output[18] = Input[18]; // RX
                Output[19] = Input[19];

                Output[20] = Input[20]; // RY
                Output[21] = Input[21];
            }

            return Input[0];
        }
    }
}
