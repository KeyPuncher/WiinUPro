// temporary
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUSoft
{
    public class XDevice
    {
        private int ID;
        private XBus bus;

        public Action<Byte, Byte> OnRumble;

        // Constructor
        public XDevice(XBus bus, int ID)
        {
            this.bus = bus;
            this.ID = ID;
        }

        public bool Connect()
        {
            return bus.Plugin(ID);
        }

        public bool Remove()
        {
            return bus.Unplug(ID);
        }

        // Updates the Bus
        public bool Update(XReport reportobj)
        {
            Byte[] input = reportobj.ToBytes();
            Byte[] rumble = new Byte[8];
            Byte[] report = new Byte[28];

            bus.Parse(input, report);

            if (bus.Report(report, rumble))
            {
                if (rumble[1] == 0x08)
                {
                    Byte big = (Byte)(rumble[3]);
                    Byte small = (Byte)(rumble[4]);

                    if (OnRumble != null)
                    {
                        OnRumble(big, small);
                    }
                }
                return true;
            }

            return false;
        }
    }
}
