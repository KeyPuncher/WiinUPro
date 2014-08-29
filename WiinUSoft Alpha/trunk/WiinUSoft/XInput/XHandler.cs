// Temporary
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUSoft
{
    public class XHandler
    {
        // required XInput components
        private XBus XBus;
        private XDevice device;
        private XReport report;

        private long id;

        public Action<Byte, Byte> OnRumble { get; set; }

        public XHandler(long id)
        {
            this.id = id;
            XBus = XBus.Default;
        }

        public bool Reset()
        {
            report = new XReport((int)id);
            return true;
        }

        public bool Connect()
        {
            this.Disconnect();
            device = new XDevice(XBus, (int)id);
            report = new XReport((int)id);
            device.OnRumble += device_OnRumble;
            return device.Connect();
        }

        public bool Disconnect()
        {
            if (device != null)
            {
                return device.Remove();
            }

            return false;
        }

        private void device_OnRumble(byte big, byte small)
        {
            if (OnRumble != null)
            {
                OnRumble(big, small);
            }
        }

        public bool StartUpdate()
        {
            return true;
        }

        public bool EndUpdate()
        {
            if (device != null && report != null)
            {
                return device.Update(report);
            }

            return false;
        }

        public void SetButton(string assignment, bool value)
        {
            report.SetButton(assignment, value);
        }

        public void SetAxis(string assignment, double value)
        {
            switch (assignment.ToLower())
            {
                case "lx":
                    report.LX = value;
                    break;

                case "ly":
                    report.LY = value;
                    break;

                case "rx":
                    report.RX = value;
                    break;

                case "ry":
                    report.RY = value;
                    break;
            }
        }
    }
}
