using System;
using NintrollerLib;

namespace Shared
{
    public class DeviceInfo
    {
        // For Wii/U Controllers
        public string DevicePath { get; set; }
        public ControllerType Type { get; set; }

        // For Joysticks
        public Guid InstanceGUID { get; set; } = Guid.Empty;
        public string PID { get; set; }

        public bool SameDevice(string path)
        {
            if (!string.IsNullOrEmpty(DevicePath))
            {
                return path == DevicePath;
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
