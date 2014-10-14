using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ScpControl;

namespace WiinUSoft.Holders
{
    public class XInputHolder : Holder
    {
        private XBus bus;
        private bool connected;
        private int ID;

        public XInputHolder()
        {
            Values = new Dictionary<string, float>();
            Mappings = new Dictionary<string, string>();
            Flags = new Dictionary<string, bool>();

            if (!Flags.ContainsKey("RUMBLE"))
            {
                Flags.Add("RUMBLE", false);
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
                    case "UP"   : index = 10; offset = 0; break;
                    case "DOWN" : index = 10; offset = 1; break;
                    case "LEFT" : index = 10; offset = 2; break;
                    case "RIGHT": index = 10; offset = 3; break;
                    case "START": index = 10; offset = 4; break;
                    case "BACK" : index = 10; offset = 5; break;
                    case "LS"   : index = 10; offset = 6; break;
                    case "RS"   : index = 10; offset = 7; break;
                    case "LB"   : index = 11; offset = 0; break;
                    case "RB"   : index = 11; offset = 1; break;
                    case "GUIDE": index = 11; offset = 2; break;
                    case "A"    : index = 11; offset = 4; break;
                    case "B"    : index = 11; offset = 5; break;
                    case "X"    : index = 11; offset = 6; break;
                    case "Y"    : index = 11; offset = 7; break;
                }
                
                if (offset > -1 && index < report.Length)
                {
                    report[index] |= (byte)(1 << offset);
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
                // TODO: other mappings
            }
            #endregion

            if (bus.Report(report, rumble))
            {
                if (rumble[1] == 0x08)
                {
                    byte big = (byte)rumble[3];
                    byte small = (byte)rumble[4];

                    // TODO: Rumble
                    System.Diagnostics.Debug.WriteLine("Big Rumble: " + big.ToString());
                    System.Diagnostics.Debug.WriteLine("Small Rumble: " + small.ToString());
                    Flags["RUMBLE"] = true;
                }
                else
                {
                    Flags["RUMBLE"] = false;
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
        A = "A",
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
