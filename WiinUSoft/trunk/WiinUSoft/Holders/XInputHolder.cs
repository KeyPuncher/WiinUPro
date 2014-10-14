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

        public static Dictionary<string, string> GetDefaultMapping(NintrollerLib.ControllerType type)
        {
            throw new NotImplementedException();

            // TODO: create default mapping
        }

        public XInputHolder()
        {
            Values = new Dictionary<string, float>();
            Mappings = new Dictionary<string, string>();
            Flags = new Dictionary<string, bool>();

            if (!Flags.ContainsKey(Inputs.Flags.RUMBLE))
            {
                Flags.Add(Inputs.Flags.RUMBLE, false);
            }
        }

        public override void Update()
        {
            if (!connected)
            {
                return;
            }

            byte[] rumble = new byte[8];
            byte[] report = new byte[28];

            //bus.Parse(input, reportB);

            // TODO: populate report
            #region Populate Report
            report[0] = 0x1C;
            report[4] = (byte)ID;
            // 5 to 8 should remain 0
            report[9] = 0x14;

            foreach (KeyValuePair<string, string> map in Mappings)
            {
                int index = 10;
                int offset = -1;

                switch (map.Value.ToUpper())
                {
                        // Digital (1 Bit)
                    case Inputs.Xbox360.UP   : index = 10; offset = 0; break;
                    case Inputs.Xbox360.DOWN : index = 10; offset = 1; break;
                    case Inputs.Xbox360.LEFT : index = 10; offset = 2; break;
                    case Inputs.Xbox360.RIGHT: index = 10; offset = 3; break;
                    case Inputs.Xbox360.START: index = 10; offset = 4; break;
                    case Inputs.Xbox360.BACK : index = 10; offset = 5; break;
                    case Inputs.Xbox360.LS   : index = 10; offset = 6; break;
                    case Inputs.Xbox360.RS   : index = 10; offset = 7; break;
                    case Inputs.Xbox360.LB   : index = 11; offset = 0; break;
                    case Inputs.Xbox360.RB   : index = 11; offset = 1; break;
                    case Inputs.Xbox360.GUIDE: index = 11; offset = 2; break;
                    case Inputs.Xbox360.A    : index = 11; offset = 4; break;
                    case Inputs.Xbox360.B    : index = 11; offset = 5; break;
                    case Inputs.Xbox360.X    : index = 11; offset = 6; break;
                    case Inputs.Xbox360.Y    : index = 11; offset = 7; break;
                        // Triggers (1 Byte)
                    case Inputs.Xbox360.LT: index = 12; offset = -1; break;
                    case Inputs.Xbox360.RT: index = 13; offset = -1; break;
                        // Analog (2 Byte)
                    case Inputs.Xbox360.LX: index = 14; offset = -1; break;
                    case Inputs.Xbox360.LY: index = 16; offset = -1; break;
                    case Inputs.Xbox360.RX: index = 18; offset = -1; break;
                    case Inputs.Xbox360.RY: index = 20; offset = -1; break;
                }
                
                if (offset > -1 && index < report.Length)
                {
                    report[index] |= (byte)(1 << offset);
                }
                else if (index == 12 || index == 13)
                {
                    report[index] = GetRawTrigger(Values[map.Key]);
                }
                else if (index + 1 < report.Length)
                {
                    report[index]     = (byte)((GetRawAxis(Values[map.Key]) >> 0) & 0xFF);
                    report[index + 1] = (byte)((GetRawAxis(Values[map.Key]) >> 8) & 0xFF);
                }

                //if (map.Value.ToUpper() == "UP"   ) report[10] |= (byte)(1 << 0);
                //if (map.Value.ToUpper() == "DOWN" ) report[10] |= (byte)(1 << 1);
                //if (map.Value.ToUpper() == "LEFT" ) report[10] |= (byte)(1 << 2);
                //if (map.Value.ToUpper() == "RIGHT") report[10] |= (byte)(1 << 3);
                //if (map.Value.ToUpper() == "START") report[10] |= (byte)(1 << 4);
                //if (map.Value.ToUpper() == "BACK" ) report[10] |= (byte)(1 << 5);
                //if (map.Value.ToUpper() == "LS"   ) report[10] |= (byte)(1 << 6);
                //if (map.Value.ToUpper() == "RS"   ) report[10] |= (byte)(1 << 7);

                //if (map.Value.ToUpper() == "LB"  ) report[11] |= (byte)(1 << 0);
                //if (map.Value.ToUpper() == "RB"  ) report[11] |= (byte)(1 << 1);
                //if (map.Value.ToUpper() == "GUIDE") report[11] |= (byte)(1 << 2);

                //if (map.Value.ToUpper() == "A") report[11] |= (byte)(1 << 4);
                //if (map.Value.ToUpper() == "B") report[11] |= (byte)(1 << 5);
                //if (map.Value.ToUpper() == "X") report[11] |= (byte)(1 << 6);
                //if (map.Value.ToUpper() == "Y") report[11] |= (byte)(1 << 7);
            }
            #endregion

            if (bus.Report(report, rumble))
            {
                if (rumble[1] == 0x08)
                {
                    byte big = (byte)rumble[3];
                    byte small = (byte)rumble[4];
                    // TODO: Revise Rumble
                    System.Diagnostics.Debug.WriteLine("Big Rumble: " + big.ToString());
                    System.Diagnostics.Debug.WriteLine("Small Rumble: " + small.ToString());
                    
                    // Check if it's strong enough to rumble
                    int strength = BitConverter.ToInt32(new byte[] { rumble[3], rumble[4] }, 0);
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
            if (axis < 0.0)
            {
                return -32767;
            }

            return (Int32)((axis - 0.5) * 2 * 32767);
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
            return base.Parse(Input, Output, Type);
        }
    }

    public enum XInputMapping
    {
        A,
        B,
        X,
        Y,
        RB,
        RS,
        RT,
        Start,
        Guide,
        Back,
        Up,
        Down,
        Left,
        Right,
        LB,
        LS,
        LT,
        RUp,
        RDown,
        RLeft,
        RRight,
        LUp,
        LDown,
        LLeft,
        LRight
    }
}
