using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScpControl;

namespace WiinUPro
{
    class ScpDirector
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
                this[i].Open();
                result = this[i].Start();

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
                this[i].Stop();
                result = this[i].Close();

                if (!result)
                {
                    return false;
                }

                this[i].Unplug(this[i].ID);
                _xInstances.RemoveAt(i);
            }

            return result;
        }

        public void Apply(XInput_Device device = XInput_Device.Device_A)
        {
            // TODO: Send Bus through
        }

        public void ApplyAll()
        {
            // TODO: Send all reports
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

            public XInputBus(int id)
            {
                inputs = new XInputState();
                ID = id;
                Plugin(id);
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
                // TODO: Replace dummy
                int dummy = 0;

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
                output[(uint)X360Axis.BT_Lo] = 0x00;
                output[(uint)X360Axis.BT_Hi] = 0x00;

                // triggers
                output[(uint)X360Axis.LT] = (byte)dummy;
                output[(uint)X360Axis.RT] = (byte)dummy;

                // Left Joystick
                output[(uint)X360Axis.LX_Lo] = (byte)((dummy >> 0) & 0xFF);
                output[(uint)X360Axis.LX_Hi] = (byte)((dummy >> 8) & 0xFF);
                output[(uint)X360Axis.LY_Lo] = (byte)((dummy >> 0) & 0xFF);
                output[(uint)X360Axis.LY_Hi] = (byte)((dummy >> 8) & 0xFF);

                // Right Joystick
                output[(uint)X360Axis.RX_Lo] = (byte)((dummy >> 0) & 0xFF);
                output[(uint)X360Axis.RX_Hi] = (byte)((dummy >> 8) & 0xFF);
                output[(uint)X360Axis.RY_Lo] = (byte)((dummy >> 0) & 0xFF);
                output[(uint)X360Axis.RY_Hi] = (byte)((dummy >> 8) & 0xFF);

                if (Report(output, rumble))
                {
                    // True on rumble state change
                    if (rumble[1] == 0x08)
                    {
                        // rumble
                        // perhaps rumble even if change flag not set
                    }
                }
            }
        }
    }
}
