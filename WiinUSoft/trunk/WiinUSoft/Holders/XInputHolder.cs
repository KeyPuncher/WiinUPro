using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ScpControl;

namespace WiinUSoft.Holders
{
    public class XInputHolder : Holder
    {
        internal int minRumble = 20;

        private XBus bus;
        private bool connected;
        private int ID;
        private Dictionary<string, float> writeReport;

        public static Dictionary<string, string> GetDefaultMapping(NintrollerLib.ControllerType type)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            // TODO: create default mapping
            switch (type)
            {
                case NintrollerLib.ControllerType.ProController:
                    result.Add(Inputs.ProController.A, Inputs.Xbox360.A);
                    result.Add(Inputs.ProController.B, Inputs.Xbox360.B);
                    result.Add(Inputs.ProController.Y, Inputs.Xbox360.X);
                    result.Add(Inputs.ProController.X, Inputs.Xbox360.Y);

                    result.Add(Inputs.ProController.UP, Inputs.Xbox360.UP);
                    result.Add(Inputs.ProController.DOWN, Inputs.Xbox360.DOWN);
                    result.Add(Inputs.ProController.LEFT, Inputs.Xbox360.LEFT);
                    result.Add(Inputs.ProController.RIGHT, Inputs.Xbox360.RIGHT);

                    result.Add(Inputs.ProController.L, Inputs.Xbox360.LB);
                    result.Add(Inputs.ProController.R, Inputs.Xbox360.RB);
                    result.Add(Inputs.ProController.ZL, Inputs.Xbox360.LT);
                    result.Add(Inputs.ProController.ZR, Inputs.Xbox360.RT);

                    //result.Add(Inputs.ProController.LX, Inputs.Xbox360.LX);
                    //result.Add(Inputs.ProController.LY, Inputs.Xbox360.LY);
                    //result.Add(Inputs.ProController.RX, Inputs.Xbox360.RX);
                    //result.Add(Inputs.ProController.RY, Inputs.Xbox360.RY);

                    result.Add(Inputs.ProController.LUP,    Inputs.Xbox360.LUP);
                    result.Add(Inputs.ProController.LDOWN,  Inputs.Xbox360.LDOWN);
                    result.Add(Inputs.ProController.LLEFT,  Inputs.Xbox360.LLEFT);
                    result.Add(Inputs.ProController.LRIGHT, Inputs.Xbox360.LRIGHT);

                    result.Add(Inputs.ProController.RUP,    Inputs.Xbox360.RUP);
                    result.Add(Inputs.ProController.RDOWN,  Inputs.Xbox360.RDOWN);
                    result.Add(Inputs.ProController.RLEFT,  Inputs.Xbox360.RLEFT);
                    result.Add(Inputs.ProController.RRIGHT, Inputs.Xbox360.RRIGHT);

                    result.Add(Inputs.ProController.LS, Inputs.Xbox360.LS);
                    result.Add(Inputs.ProController.RS, Inputs.Xbox360.RS);
                    result.Add(Inputs.ProController.SELECT, Inputs.Xbox360.BACK);
                    result.Add(Inputs.ProController.START, Inputs.Xbox360.START);
                    result.Add(Inputs.ProController.HOME, Inputs.Xbox360.GUIDE);
                    break;
            }

            return result;
        }

        public XInputHolder()
        {
            //Values = new Dictionary<string, float>();
            Values = new System.Collections.Concurrent.ConcurrentDictionary<string, float>();
            Mappings = new Dictionary<string, string>();
            Flags = new Dictionary<string, bool>();
            writeReport = new Dictionary<string, float>()
            {
                {Inputs.Xbox360.A, 0},
                {Inputs.Xbox360.B, 0},
                {Inputs.Xbox360.X, 0},
                {Inputs.Xbox360.Y, 0},
                {Inputs.Xbox360.UP, 0},
                {Inputs.Xbox360.DOWN, 0},
                {Inputs.Xbox360.LEFT, 0},
                {Inputs.Xbox360.RIGHT, 0},
                {Inputs.Xbox360.LB, 0},
                {Inputs.Xbox360.RB, 0},
                {Inputs.Xbox360.BACK, 0},
                {Inputs.Xbox360.START, 0},
                {Inputs.Xbox360.GUIDE, 0},
                {Inputs.Xbox360.LS, 0},
                {Inputs.Xbox360.RS, 0},
            };

            if (!Flags.ContainsKey(Inputs.Flags.RUMBLE))
            {
                Flags.Add(Inputs.Flags.RUMBLE, false);
            }
        }

        public XInputHolder(NintrollerLib.ControllerType t) : this()
        {
            Mappings = GetDefaultMapping(t);
        }

        public override void Update()
        {
            //if (!connected)
            //{
            //    return;
            //}

            byte[] rumble = new byte[8];
            byte[] report = new byte[28];

            //bus.Parse(input, reportB);

            // TODO: populate report
            #region Populate Report
            //report[0] = 0x1C;
            //report[4] = (byte)ID;
            //// 5 to 8 should remain 0
            //report[9] = 0x14;

            //float LX = 0f;
            //float LY = 0f;
            //float RX = 0f;
            //float RY = 0f;

            //foreach (KeyValuePair<string, string> map in Mappings)
            //{
            //    int index = 10;
            //    int offset = -1;

            //    switch (map.Value.ToUpper())
            //    {
            //            // Digital (1 Bit)
            //        case Inputs.Xbox360.UP   : index = 10; offset = 0; break;
            //        case Inputs.Xbox360.DOWN : index = 10; offset = 1; break;
            //        case Inputs.Xbox360.LEFT : index = 10; offset = 2; break;
            //        case Inputs.Xbox360.RIGHT: index = 10; offset = 3; break;
            //        case Inputs.Xbox360.START: index = 10; offset = 4; break;
            //        case Inputs.Xbox360.BACK : index = 10; offset = 5; break;
            //        case Inputs.Xbox360.LS   : index = 10; offset = 6; break;
            //        case Inputs.Xbox360.RS   : index = 10; offset = 7; break;
            //        case Inputs.Xbox360.LB   : index = 11; offset = 0; break;
            //        case Inputs.Xbox360.RB   : index = 11; offset = 1; break;
            //        case Inputs.Xbox360.GUIDE: index = 11; offset = 2; break;
            //        case Inputs.Xbox360.A    : index = 11; offset = 4; break;
            //        case Inputs.Xbox360.B    : index = 11; offset = 5; break;
            //        case Inputs.Xbox360.X    : index = 11; offset = 6; break;
            //        case Inputs.Xbox360.Y    : index = 11; offset = 7; break;
            //            // Triggers (1 Byte)
            //        case Inputs.Xbox360.LT: index = 12; offset = -1; break;
            //        case Inputs.Xbox360.RT: index = 13; offset = -1; break;
            //            // Analog (2 Byte)
            //        //case Inputs.Xbox360.LX: index = 14; offset = -1; break;
            //        //case Inputs.Xbox360.LY: index = 16; offset = -1; break;
            //        //case Inputs.Xbox360.RX: index = 18; offset = -1; break;
            //        //case Inputs.Xbox360.RY: index = 20; offset = -1; break;
            //        case Inputs.Xbox360.LLEFT:  LX -= Values[map.Key]; offset = -1; break;
            //        case Inputs.Xbox360.LRIGHT: LX += Values[map.Key]; offset = -1; break;
            //        case Inputs.Xbox360.LUP:    LY += Values[map.Key]; offset = -1; break;
            //        case Inputs.Xbox360.LDOWN:  LY -= Values[map.Key]; offset = -1; break;
            //        case Inputs.Xbox360.RLEFT:  RX -= Values[map.Key]; offset = -1; break;
            //        case Inputs.Xbox360.RRIGHT: RX += Values[map.Key]; offset = -1; break;
            //        case Inputs.Xbox360.RUP:    RY += Values[map.Key]; offset = -1; break;
            //        case Inputs.Xbox360.RDOWN:  RY -= Values[map.Key]; offset = -1; break;
            //    }
                
            //    if (offset > -1 && index < report.Length)
            //    {
            //        report[index] |= (byte)(1 << offset);
            //    }
            //    else if (index == 12 || index == 13)
            //    {
            //        report[index] = GetRawTrigger(Values[map.Key]);
            //    }
            //    //else if (index + 1 < report.Length)
            //    //{
            //    //    report[index]     = (byte)((GetRawAxis(Values[map.Key]) >> 0) & 0xFF);
            //    //    report[index + 1] = (byte)((GetRawAxis(Values[map.Key]) >> 8) & 0xFF);
            //    //}

            //    //if (map.Value.ToUpper() == "UP"   ) report[10] |= (byte)(1 << 0);
            //    //if (map.Value.ToUpper() == "DOWN" ) report[10] |= (byte)(1 << 1);
            //    //if (map.Value.ToUpper() == "LEFT" ) report[10] |= (byte)(1 << 2);
            //    //if (map.Value.ToUpper() == "RIGHT") report[10] |= (byte)(1 << 3);
            //    //if (map.Value.ToUpper() == "START") report[10] |= (byte)(1 << 4);
            //    //if (map.Value.ToUpper() == "BACK" ) report[10] |= (byte)(1 << 5);
            //    //if (map.Value.ToUpper() == "LS"   ) report[10] |= (byte)(1 << 6);
            //    //if (map.Value.ToUpper() == "RS"   ) report[10] |= (byte)(1 << 7);

            //    //if (map.Value.ToUpper() == "LB"  ) report[11] |= (byte)(1 << 0);
            //    //if (map.Value.ToUpper() == "RB"  ) report[11] |= (byte)(1 << 1);
            //    //if (map.Value.ToUpper() == "GUIDE") report[11] |= (byte)(1 << 2);

            //    //if (map.Value.ToUpper() == "A") report[11] |= (byte)(1 << 4);
            //    //if (map.Value.ToUpper() == "B") report[11] |= (byte)(1 << 5);
            //    //if (map.Value.ToUpper() == "X") report[11] |= (byte)(1 << 6);
            //    //if (map.Value.ToUpper() == "Y") report[11] |= (byte)(1 << 7);
            //}

            //report[14] = (byte)((GetRawAxis(LX) >> 0) & 0xFF);
            //report[15] = (byte)((GetRawAxis(LX) >> 8) & 0xFF);
            //report[16] = (byte)((GetRawAxis(LY) >> 0) & 0xFF);
            //report[17] = (byte)((GetRawAxis(LY) >> 8) & 0xFF);
            //report[18] = (byte)((GetRawAxis(RX) >> 0) & 0xFF);
            //report[19] = (byte)((GetRawAxis(RX) >> 8) & 0xFF);
            //report[20] = (byte)((GetRawAxis(RY) >> 0) & 0xFF);
            //report[21] = (byte)((GetRawAxis(RY) >> 8) & 0xFF);
            #endregion

            #region Populate Report Revised
            report[0] = (byte)ID;
            report[1] = 0x02;

            report[10] = 0;
            report[11] = 0;
            report[12] = 0;
            report[13] = 0;

            float LX = 0f;
            float LY = 0f;
            float RX = 0f;
            float RY = 0f;

            float LT = 0f;
            float RT = 0f;

            foreach (KeyValuePair<string, string> map in Mappings)
            {
                if (writeReport.ContainsKey(map.Value))
                {
                    writeReport[map.Value] = Values[map.Key];
                }
                else
                {
                    switch (map.Value)
                    {
                        case Inputs.Xbox360.LLEFT : LX -= Values[map.Key]; break;
                        case Inputs.Xbox360.LRIGHT: LX += Values[map.Key]; break;
                        case Inputs.Xbox360.LUP   : LY += Values[map.Key]; break;
                        case Inputs.Xbox360.LDOWN : LY -= Values[map.Key]; break;
                        case Inputs.Xbox360.RLEFT : RX -= Values[map.Key]; break;
                        case Inputs.Xbox360.RRIGHT: RX += Values[map.Key]; break;
                        case Inputs.Xbox360.RUP   : RY += Values[map.Key]; break;
                        case Inputs.Xbox360.RDOWN : RY -= Values[map.Key]; break;
                        case Inputs.Xbox360.LT: LT = Values[map.Key]; break;
                        case Inputs.Xbox360.RT: RT = Values[map.Key]; break;
                    }
                }
            }

            report[10] |= (byte)(writeReport[Inputs.Xbox360.BACK]  > 0f ? 1 << 0 : 0);
            report[10] |= (byte)(writeReport[Inputs.Xbox360.LS]    > 0f ? 1 << 1 : 0);
            report[10] |= (byte)(writeReport[Inputs.Xbox360.RS]    > 0f ? 1 << 2 : 0);
            report[10] |= (byte)(writeReport[Inputs.Xbox360.START] > 0f ? 1 << 3 : 0);
            report[10] |= (byte)(writeReport[Inputs.Xbox360.UP]    > 0f ? 1 << 4 : 0);
            report[10] |= (byte)(writeReport[Inputs.Xbox360.DOWN]  > 0f ? 1 << 5 : 0);
            report[10] |= (byte)(writeReport[Inputs.Xbox360.RIGHT] > 0f ? 1 << 6 : 0);
            report[10] |= (byte)(writeReport[Inputs.Xbox360.LEFT]  > 0f ? 1 << 7 : 0);

            report[11] |= (byte)(writeReport[Inputs.Xbox360.LB]    > 0f ? 1 << 2 : 0);
            report[11] |= (byte)(writeReport[Inputs.Xbox360.RB]    > 0f ? 1 << 3 : 0);
            report[11] |= (byte)(writeReport[Inputs.Xbox360.Y]     > 0f ? 1 << 4 : 0);
            report[11] |= (byte)(writeReport[Inputs.Xbox360.B]     > 0f ? 1 << 5 : 0);
            report[11] |= (byte)(writeReport[Inputs.Xbox360.A]     > 0f ? 1 << 6 : 0);
            report[11] |= (byte)(writeReport[Inputs.Xbox360.X]     > 0f ? 1 << 7 : 0);

            report[12] |= (byte)(writeReport[Inputs.Xbox360.GUIDE] > 0f ? 1 << 0 : 0);

            report[14] = (byte)((GetRawAxis(LX) >> 0) & 0xFF);
            report[15] = (byte)((GetRawAxis(LX) >> 8) & 0xFF);
            report[16] = (byte)((GetRawAxis(LY) >> 0) & 0xFF);
            report[17] = (byte)((GetRawAxis(LY) >> 8) & 0xFF);
            report[18] = (byte)((GetRawAxis(RX) >> 0) & 0xFF);
            report[19] = (byte)((GetRawAxis(RX) >> 8) & 0xFF);
            report[20] = (byte)((GetRawAxis(RY) >> 0) & 0xFF);
            report[21] = (byte)((GetRawAxis(RY) >> 8) & 0xFF);

            report[26] = GetRawTrigger(LT);
            report[27] = GetRawTrigger(RT);
            #endregion

            byte[] reportB = new byte[28];
            bus.Parse(report, reportB);

            if (bus.Report(reportB, rumble))
            {
                if (rumble[1] == 0x08)
                {
                    byte big = (byte)rumble[3];
                    byte small = (byte)rumble[4];
                    // TODO: Revise Rumble
                    System.Diagnostics.Debug.WriteLine("Big Rumble: " + big.ToString());
                    System.Diagnostics.Debug.WriteLine("Small Rumble: " + small.ToString());
                    
                    // Check if it's strong enough to rumble
                    int strength = BitConverter.ToInt32(new byte[] { rumble[4], rumble[3], 0x00, 0x00 }, 0);
                    Flags[Inputs.Flags.RUMBLE] = (strength > minRumble);
                }
                else
                {
                    Flags[Inputs.Flags.RUMBLE] = false;
                }
            }
        }

        public bool ConnectXInput(int id)
        {
            bus = XBus.Default;
            
            if (connected)
            {
                bus.Unplug(id);
            }
            bus.Unplug(id);
            connected = bus.Plugin(id);
            ID = id;
            return connected;
        }

        public bool RemoveXInput(int id)
        {
            if (bus.Unplug(id))
            {
                ID = 0;
                connected = false;
                return true;
            }

            return false;
        }

        public Int32 GetRawAxis(double axis)
        {
            if (axis > 1.0)
            {
                return 32767;
            }
            if (axis < -1.0)
            {
                return -32767;
            }

            return (Int32)(axis * 32767);
        }

        public byte GetRawTrigger(double trigger)
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
    }

    public class XBus : BusDevice
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
            App.Current.Exit += StopDevice;
        }

        private void StopDevice(object sender, System.Windows.ExitEventArgs e)
        {
            if (defaultInstance != null)
            {
                defaultInstance.Stop();
                defaultInstance.Close();
            }
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
