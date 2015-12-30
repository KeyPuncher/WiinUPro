using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScpControl;

namespace WiinUPro
{
    public class ScpDirector
    {
        public const int MAX_XINPUT_INSTNACES = 4;

        #region Access
        public static ScpDirector Access { get; protected set; }

        static ScpDirector()
        {
            // TODO Director: Initialize SCPControl
            Access = new ScpDirector();
            Access.Available = true;
        }
        #endregion

        protected List<XInputBus> _xInstances;

        /// <summary>
        /// Gets the desired XInputBus (0 to 3).
        /// </summary>
        /// <param name="index">Any out of range index will return the first device.</param>
        /// <returns>The XInputBus</returns>
        protected XInputBus this[int index]
        {
            get
            {
                if (index < 0 || index >= MAX_XINPUT_INSTNACES)
                {
                    index = 0;
                }
                
                while (index >= _xInstances.Count)
                {
                    _xInstances.Add(new XInputBus(index));
                }

                return _xInstances[index];
            }
        }

        public bool Available { get; protected set; }
        public int Instances { get { return _xInstances.Count; } }

        public ScpDirector()
        {
            _xInstances = new List<XInputBus>();
        }

        public void SetButton(X360Button button, bool pressed, XInput_Device device = XInput_Device.Device_A)
        {
            this[(int)device].SetInput(button, pressed);
        }

        public void SetAxis(X360Axis axis, float value, XInput_Device device = XInput_Device.Device_A)
        {
            this[(int)device].SetInput(axis, value);
        }

        /// <summary>
        /// Connects up to the given device.
        /// Ex: If C is used then A, B, & C will be connected.
        /// </summary>
        /// <param name="device">The highest device to be connected.</param>
        /// <returns>If all connections are successful.</returns>
        public bool ConnectDevice(XInput_Device device)
        {
            bool result = false;

            for (int i = 0; i <= (int)device; i++)
            {
                result = this[i].Connect();

                if (!result)
                {
                    return false;
                }
            }

            return result;
        }

        /// <summary>
        /// Disconnects down to the given device.
        /// Ex: If A is used then all of the devices will be disconnected.
        /// </summary>
        /// <param name="device">The lowest device to be disconnected.</param>
        /// <returns>If all devices were disconnected</returns>
        public bool DisconnectDevice(XInput_Device device)
        {
            bool result = false;

            for (int i = _xInstances.Count - 1; i >= (int)device; i--)
            {
                result = this[i].Disconnect();

                if (!result)
                {
                    return false;
                }

                _xInstances.RemoveAt(i);
            }

            return result;
        }

        public void Apply(XInput_Device device = XInput_Device.Device_A)
        {
            this[(int)device].Update();
        }

        public void ApplyAll()
        {
            foreach (var bus in _xInstances)
            {
                bus.Update();
            }
        }

        public enum XInput_Device : int
        {
            Device_A = 0,
            Device_B = 1,
            Device_C = 2,
            Device_D = 3
        }

        public struct XInputState
        {
            public bool A, B, X, Y;
            public bool Up, Down, Left, Right;
            public bool LB, RB, LS, RS;
            public bool Start, Back, Guide;

            public float LX, LY, LT;
            public float RX, RY, RT;

            public bool this[X360Button btn]
            {
                set
                {
                    switch (btn)
                    {
                        case X360Button.A: A = value; break;
                        case X360Button.B: B = value; break;
                        case X360Button.X: X = value; break;
                        case X360Button.Y: Y = value; break;
                        case X360Button.LB: LB = value; break;
                        case X360Button.RB: RB = value; break;
                        case X360Button.LS: LS = value; break;
                        case X360Button.RS: RS = value; break;
                        case X360Button.Up: Up = value; break;
                        case X360Button.Down: Down = value; break;
                        case X360Button.Left: Left = value; break;
                        case X360Button.Right: Right = value; break;
                        case X360Button.Start: Start = value; break;
                        case X360Button.Back: Back = value; break;
                        case X360Button.Guide: Guide = value; break;
                        default: break;
                    }
                }
            }

            public float this[X360Axis axis]
            {
                set
                {
                    switch (axis)
                    {
                        case X360Axis.LX_Hi:
                        case X360Axis.LX_Lo:
                            LX = value;
                            break;
                        case X360Axis.LY_Hi:
                        case X360Axis.LY_Lo:
                            LY = value;
                            break;
                        case X360Axis.LT:
                            LT = value;
                            break;
                        case X360Axis.RX_Hi:
                        case X360Axis.RX_Lo:
                            RX = value;
                            break;
                        case X360Axis.RY_Hi:
                        case X360Axis.RY_Lo:
                            RY = value;
                            break;
                        case X360Axis.RT:
                            RT = value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected class XInputBus : BusDevice
        {
            public XInputState inputs;
            public int ID { get; protected set; }
            public bool PluggedIn { get; protected set; }
            public bool Started { get; protected set; }

            public XInputBus(int id)
            {
                inputs = new XInputState();
                ID = id;
                Plugin(id);
            }

            public bool Connect()
            {
                if (!PluggedIn)
                {
                    PluggedIn = Plugin(ID);
                }

                if (!Started)
                {
                    Started = Open() && Start();
                }

                return Started;
            }

            public bool Disconnect()
            {
                if (PluggedIn)
                {
                    Started = !Stop();
                    Close();
                    PluggedIn = !Unplug(ID);
                }

                return PluggedIn == false;
            }

            public void SetInput(X360Button button, bool state)
            {
                inputs[button] = state;
            }

            public void SetInput(X360Axis axis, float value)
            {
                inputs[axis] = value;
            }

            public void Update()
            {
                if (!Started) return;

                byte[] rumble = new byte[8];
                byte[] output = new byte[28];

                // Fill the output to be sent
                int serial = IndexToSerial((byte)ID);
                output[0] = 0x1C;
                output[4] = (byte)((serial >> 0) & 0xFF);
                output[5] = (byte)((serial >> 8) & 0xFF);
                output[6] = (byte)((serial >> 16) & 0xFF);
                output[7] = (byte)((serial >> 24) & 0xFF);
                output[9] = 0x14;

                // buttons
                int buttonFlags = 0x00;             // try X360Button.Up here instead
                buttonFlags |= (byte)(inputs.Up     ? 1 << 0 : 0);
                buttonFlags |= (byte)(inputs.Down   ? 1 << 1 : 0);
                buttonFlags |= (byte)(inputs.Right  ? 1 << 2 : 0);
                buttonFlags |= (byte)(inputs.Left   ? 1 << 3 : 0);
                buttonFlags |= (byte)(inputs.Start  ? 1 << 4 : 0);
                buttonFlags |= (byte)(inputs.Back   ? 1 << 5 : 0);
                buttonFlags |= (byte)(inputs.LS     ? 1 << 6 : 0);
                buttonFlags |= (byte)(inputs.RS     ? 1 << 2 : 0);
                buttonFlags |= (byte)(inputs.LB     ? 1 << 8 : 0);
                buttonFlags |= (byte)(inputs.RB     ? 1 << 9 : 0);
                buttonFlags |= (byte)(inputs.Guide  ? 1 << 10 : 0);
                buttonFlags |= (byte)(inputs.A      ? 1 << 12 : 0);
                buttonFlags |= (byte)(inputs.B      ? 1 << 13 : 0);
                buttonFlags |= (byte)(inputs.X      ? 1 << 14 : 0);
                buttonFlags |= (byte)(inputs.Y      ? 1 << 15 : 0);
                output[(uint)X360Axis.BT_Lo] = (byte)((buttonFlags >> 0) & 0xFF);
                output[(uint)X360Axis.BT_Hi] = (byte)((buttonFlags >> 8) & 0xFF);

                // triggers
                output[(uint)X360Axis.LT] = GetRawTrigger(inputs.LT);
                output[(uint)X360Axis.RT] = GetRawTrigger(inputs.RT);

                // Left Joystick
                int rawLX = GetRawAxis(inputs.LX);
                int rawLY = GetRawAxis(inputs.LY);
                output[(uint)X360Axis.LX_Lo] = (byte)((rawLX >> 0) & 0xFF);
                output[(uint)X360Axis.LX_Hi] = (byte)((rawLX >> 8) & 0xFF);
                output[(uint)X360Axis.LY_Lo] = (byte)((rawLY >> 0) & 0xFF);
                output[(uint)X360Axis.LY_Hi] = (byte)((rawLY >> 8) & 0xFF);

                // Right Joystick
                int rawRX = GetRawAxis(inputs.RX);
                int rawRY = GetRawAxis(inputs.RY);
                output[(uint)X360Axis.RX_Lo] = (byte)((rawRX >> 0) & 0xFF);
                output[(uint)X360Axis.RX_Hi] = (byte)((rawRX >> 8) & 0xFF);
                output[(uint)X360Axis.RY_Lo] = (byte)((rawRY >> 0) & 0xFF);
                output[(uint)X360Axis.RY_Hi] = (byte)((rawRY >> 8) & 0xFF);

                if (Report(output, rumble))
                {
                    // True on rumble state change
                    if (rumble[1] == 0x08)
                    {
                        // TODO: rumble
                        // perhaps rumble even if change flag not set
                    }
                }
            }

            public int GetRawAxis(float axis)
            {
                if (axis > 1.0f)
                {
                    return 32767;
                }
                if (axis < -1.0f)
                {
                    return -32767;
                }

                return (int)(axis * 32767);
            }

            public byte GetRawTrigger(float trigger)
            {
                if (trigger > 1.0f)
                {
                    return 0xFF;
                }
                if (trigger < 0.0f)
                {
                    return 0;
                }

                return (byte)(trigger * 0xFF);
            }
        }
    }
}
