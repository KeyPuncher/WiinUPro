using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vJoyInterfaceWrap;

namespace WiinUPro
{
    public class VJoyDirector
    {
        public static VJoyDirector Access { get; protected set; }

        static VJoyDirector()
        {
            Access = new VJoyDirector();
        }
        
        public bool Available
        {
            get
            {
                uint verDll = 0;
                uint verDriver = 0;
                return _interface.vJoyEnabled() && _interface.DriverMatch(ref verDll, ref verDriver);
            }
        }

        public int ControllerCount { get { return _devices.Count; } }

        public List<JoyDevice> Devices { get { return _devices; } }

        protected static vJoy _interface;
        protected List<JoyDevice> _devices;
        protected Dictionary<uint, vJoy.JoystickState> _states;

        public VJoyDirector()
        {
            _interface = new vJoy();
            _devices = new List<JoyDevice>();
            _states = new Dictionary<uint, vJoy.JoystickState>();

            if (Available)
            {
                for (uint i = 1; i <= 16; i++)
                {
                    if (_interface.isVJDExists(i))
                    {
                        var status = _interface.GetVJDStatus(i);

                        if (status == VjdStat.VJD_STAT_FREE || status == VjdStat.VJD_STAT_OWN)
                        {
                            _devices.Add(new JoyDevice(i));
                        }
                    }
                }
            }
        }

        public bool AquireDevice(uint id)
        {
            if (_interface.GetVJDStatus(id) == VjdStat.VJD_STAT_OWN)
            {
                return true;
            }

            var result = _interface.AcquireVJD(id);
            
            if (result)
            {
                _states.Add(id, new vJoy.JoystickState()
                {
                    bDevice = (byte)id
                });
            }

            return result;
        }

        public void ReleaseDevice(uint id)
        {
            _states.Remove(id);
            _interface.RelinquishVJD(id);
        }

        public void SetButton(int button, bool state, uint id)
        {
            if (_states.ContainsKey(id))
            {
                var current = _states[id];
                current.Buttons &= ~(1u << (button - 1));
                if (state)
                {
                    current.Buttons |= 1u << (button - 1);
                }
                _states[id] = current;
            }
        }

        public void SetAxis(HID_USAGES axis, float value, uint id)
        {
            if (_states.ContainsKey(id) && _interface.GetVJDAxisExist(id, axis))
            {
                var current = _states[id];
                long max = 0;
                long min = 0;

                _interface.GetVJDAxisMax(id, axis, ref max);
                _interface.GetVJDAxisMin(id, axis, ref min);

                float norm = value + 1f;
                norm /= 2f;
                int output = (int)Math.Round(Math.Max(Math.Min(max * norm - min, min), max));

                switch (axis)
                {
                    case HID_USAGES.HID_USAGE_X:
                        current.AxisX = output;
                        break;
                    case HID_USAGES.HID_USAGE_Y:
                        current.AxisY = output;
                        break;
                    case HID_USAGES.HID_USAGE_Z:
                        current.AxisZ = output;
                        break;
                    case HID_USAGES.HID_USAGE_RX:
                        current.AxisXRot = output;
                        break;
                    case HID_USAGES.HID_USAGE_RY:
                        current.AxisYRot = output;
                        break;
                    case HID_USAGES.HID_USAGE_RZ:
                        current.AxisZRot = output;
                        break;
                    case HID_USAGES.HID_USAGE_SL0:
                        current.Slider = output;
                        break;
                    case HID_USAGES.HID_USAGE_SL1:
                        current.Dial = output;
                        break;
                }

                _states[id] = current;
            }
        }

        public void SetPOV(int pov, POVDirection direction, bool state, uint id)
        {
            if (_states.ContainsKey(id))
            {
                var current = _states[id];
                uint value = 0xFF;
                if (state)
                {
                    switch (direction.ToString().Substring(2))
                    {
                        case "Up":
                            value = 0x00;
                            break;
                        case "Right":
                            value = 0x01;
                            break;
                        case "Down":
                            value = 0x02;
                            break;
                        case "Left":
                            value = 0x03;
                            break;
                        default: break;
                    }
                }

                if (_interface.GetVJDContPovNumber(id) > 0)
                {
                    uint existing = 0xFFFFFFFF;
                    switch (pov)
                    {
                        case 1:
                            existing = current.bHats;
                            break;
                        case 2:
                            existing = current.bHatsEx1;
                            break;
                        case 3:
                            existing = current.bHatsEx2;
                            break;
                        case 4:
                            existing = current.bHatsEx3;
                            break;
                        default: break;
                    }

                    if (state && existing == 0xFFFFFFFF)
                    {
                        value *= 9000;
                    }
                    else if (state)
                    {
                        // TODO: Fix direction switching, Down 2 Right & Left 2 Up and back both work as expected
                        switch (direction.ToString().Substring(2))
                        {
                            case "Up":
                                if (existing > 4500 && existing < 18000) value = 4500;
                                else if (existing > 18000 && existing < 36000) value = 31500;
                                break;
                            case "Right":
                                if (existing < 4500 || existing > 31500) value = 4500;
                                else if (existing > 9000 && existing < 27000) value = 13500;
                                break;
                            case "Down":
                                if (existing < 18000 && existing > 0) value = 13500;
                                else if (existing > 18000 && existing < 36000) value = 22500;
                                break;
                            case "Left":
                                if (existing > 27000 || existing < 9000) value = 31500;
                                else if (existing < 27000 && existing > 9000) value = 22500;
                                break;
                            default: break;
                        }
                    }
                    else
                    {
                        value = 0xFFFFFFFF;
                        switch (direction.ToString().Substring(2))
                        {
                            case "Up":
                                if (existing > 4500 && existing < 18000) value = 9000;
                                else if (existing > 18000 && existing < 36000) value = 27000;
                                break;
                            case "Right":
                                if (existing < 4500 || existing > 31500) value = 0;
                                else if (existing > 9000 && existing < 27000) value = 18000;
                                break;
                            case "Down":
                                if (existing < 18000 && existing > 0) value = 9000;
                                else if (existing > 18000 && existing < 36000) value = 18000;
                                break;
                            case "Left":
                                if (existing > 27000 || existing < 9000) value = 0;
                                else if (existing < 27000 && existing > 9000) value = 18000;
                                break;
                            default: break;
                        }
                    }

                    switch (pov)
                    {
                        case 1:
                            current.bHats = value;
                            break;
                        case 2:
                            current.bHatsEx1 = value;
                            break;
                        case 3:
                            current.bHatsEx2 = value;
                            break;
                        case 4:
                            current.bHatsEx3 = value;
                            break;
                        default: break;
                    }
                }
                else
                {
                    var shift = ((pov - 1) * 4);

                    if (!state)
                    {
                        uint existing = current.bHats & (uint)(0xFF << shift);
                        existing = existing >> shift;

                        switch (direction.ToString().Substring(2))
                        {
                            case "Up":
                                if (existing != 0x00) value = existing;
                                break;
                            case "Right":
                                if (existing != 0x01) value = existing;
                                break;
                            case "Down":
                                if (existing != 0x02) value = existing;
                                break;
                            case "Left":
                                if (existing != 0x03) value = existing;
                                break;
                            default: break;
                        }
                    }

                    current.bHats &= (0xFFFFFFFF & (uint)(0x00 << shift));
                    current.bHats |= (value << shift);
                }

                _states[id] = current;
            }
        }

        public void ApplyAll()
        {
            foreach (var state in _states)
            {
                vJoy.JoystickState s = state.Value;
                _interface.UpdateVJD(state.Key, ref s);
            }
        }

        public void Apply(uint deviceId)
        {
            if (_states.ContainsKey(deviceId))
            {
                vJoy.JoystickState s = _states[deviceId];
                _interface.UpdateVJD(deviceId, ref s);
            }
        }

        public class JoyDevice
        {
            public uint ID { get; protected set; }
            public int Buttons { get; protected set; }
            public int POVs { get; protected set; }
            public int POV4Ds { get; protected set; }
            public bool hasX { get; protected set; }
            public bool hasY { get; protected set; }
            public bool hasZ { get; protected set; }
            public bool hasRx { get; protected set; }
            public bool hasRy { get; protected set; }
            public bool hasRz { get; protected set; }
            public bool hasSlider { get; protected set; }
            public bool hasSlider2 { get; protected set; }
            public bool ForceFeedback { get; protected set; }
            public List<string> Axes { get; protected set; }

            public JoyDevice(uint id)
            {
                ID            = id;
                Buttons       = _interface.GetVJDButtonNumber(id);
                POVs          = _interface.GetVJDContPovNumber(id);
                POV4Ds        = _interface.GetVJDDiscPovNumber(id);
                hasX          = _interface.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
                hasY          = _interface.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
                hasZ          = _interface.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
                hasRx         = _interface.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
                hasRy         = _interface.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RY);
                hasRz         = _interface.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
                hasSlider     = _interface.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_SL0);
                hasSlider2    = _interface.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_SL1);
                ForceFeedback = _interface.IsDeviceFfb(id);
                Axes = new List<string>();

                if (hasX) Axes.Add("X Axis");
                if (hasY) Axes.Add("Y Axis");
                if (hasZ) Axes.Add("Z Axis");
                if (hasRx) Axes.Add("Rx Axis");
                if (hasRy) Axes.Add("Ry Axis");
                if (hasRz) Axes.Add("Rz Axis");
                if (hasSlider) Axes.Add("Slider 1");
                if (hasSlider2) Axes.Add("Slider 2");
            }
        }

        public enum POVDirection
        {
            _1Up,
            _1Down,
            _1Left,
            _1Right,

            _2Up,
            _2Down,
            _2Left,
            _2Right,

            _3Up,
            _3Down,
            _3Left,
            _3Right,

            _4Up,
            _4Down,
            _4Left,
            _4Right,
        }
    }
}
