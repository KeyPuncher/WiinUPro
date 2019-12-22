using System;
using NintrollerLib;

namespace Shared
{
    public class DeviceInfo
    {
        public string DeviceID
        {
            get
            {
                if (!string.IsNullOrEmpty(DevicePath))
                    return DevicePath;
                else if (!InstanceGUID.Equals(Guid.Empty))
                    return InstanceGUID.ToString();
                else
                    return string.Format("{0}_{1}", VID, PID);
            }
        }

        // For Wii/U Controllers
        public string DevicePath { get; set; }
        public string DeviceName { get; set; }
        public string SerialNumber { get; set; }
        public ControllerType Type { get; set; }

        // For Joysticks
        public Guid InstanceGUID { get; set; } = Guid.Empty;
        public string VID { get; set; }
        public string PID { get; set; }

        public bool SameDevice(string identifier)
        {
            if (!string.IsNullOrEmpty(DevicePath))
            {
                return identifier == DevicePath;
            }
            else
            {
                return identifier == InstanceGUID.ToString();
            }

            return false;
        }

        public bool SameDevice(Guid guid)
        {
            if (InstanceGUID != Guid.Empty)
            {
                return guid.Equals(InstanceGUID);
            }

            return false;
        }
    }
}
