using System.Collections.Generic;
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
        protected bool[] _deviceStatus;

        /// <summary>
        /// Gets the desired XInputBus (0 to 3).
        /// </summary>
        /// <param name="index">Any out of range index will return the first device.</param>
        /// <returns>The XInputBus</returns>
        //protected XInputBus this[int index]
        //{
        //    get
        //    {
        //        if (index < 0 || index >= MAX_XINPUT_INSTNACES)
        //        {
        //            index = 0;
        //        }
        //        
        //        while (index >= _xInstances.Count)
        //        {
        //            _xInstances.Add(new XInputBus(index));
        //        }
        //
        //        return _xInstances[index];
        //    }
        //}
                
        public bool Available { get; protected set; }
        public int Instances { get { return _xInstances.Count; } }

        // Left motor is the larger one
        public delegate void RumbleChangeDelegate(byte leftMotor, byte rightMotor);

        public ScpDirector()
        {
            _xInstances = new List<XInputBus>
            {
                new XInputBus((int)XInput_Device.Device_A),
                new XInputBus((int)XInput_Device.Device_B),
                new XInputBus((int)XInput_Device.Device_C),
                new XInputBus((int)XInput_Device.Device_D)
            };
            _deviceStatus = new bool[] { false, false, false, false };
        }

        public void SetButton(X360Button button, bool pressed)
        {
            SetButton(button, pressed, XInput_Device.Device_A);
        }

        public void SetButton(X360Button button, bool pressed, XInput_Device device)
        {
            //this[(int)device].SetInput(button, pressed);
            _xInstances[(int)device - 1].SetInput(button, pressed);
        }

        public void SetAxis(X360Axis axis, float value)
        {
            SetAxis(axis, value, XInput_Device.Device_A);
        }

        public void SetAxis(X360Axis axis, float value, XInput_Device device)
        {
            //this[(int)device].SetInput(axis, value);
            _xInstances[(int)device - 1].SetInput(axis, value);
        }

        /// <summary>
        /// Connects up to the given device.
        /// Ex: If C is used then A, B, & C will be connected.
        /// </summary>
        /// <param name="device">The highest device to be connected.</param>
        /// <returns>If all connections are successful.</returns>
        public bool ConnectDevice(XInput_Device device)
        {
            bool result = _deviceStatus[(int)device - 1];

            if (!result)
            {
                result = _xInstances[(int)device - 1].Connect();
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
            if (_deviceStatus[(int)device - 1])
            {
                return true;
            }
            else
            {
                return _xInstances[(int)device - 1].Disconnect();
            }
        }

        public bool IsConnected(XInput_Device device)
        {
            return _xInstances[(int)device - 1].PluggedIn;
        }

        public void Apply(XInput_Device device)
        {
            //this[(int)device].Update();
            _xInstances[(int)device - 1].Update();
        }

        public void ApplyAll()
        {
            foreach (var bus in _xInstances.ToArray())
            {
                if (bus.PluggedIn)
                bus.Update();
            }
        }

        public void SetModifier(int value)
        {
            XInputBus.Modifier = value;
        }

        public void SubscribeToRumble(XInput_Device device, RumbleChangeDelegate callback)
        {
            _xInstances[(int)device - 1].RumbleEvent += callback;
        }

        public void UnSubscribeToRumble(XInput_Device device, RumbleChangeDelegate callback)
        {
            _xInstances[(int)device - 1].RumbleEvent -= callback;
        }

        public enum XInput_Device : int
        {
            Device_A = 1,
            Device_B = 2,
            Device_C = 3,
            Device_D = 4
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

                get
                {
                    switch (btn)
                    {
                        case X360Button.A: return A;
                        case X360Button.B: return B;
                        case X360Button.X: return X;
                        case X360Button.Y: return Y;
                        case X360Button.LB: return LB;
                        case X360Button.RB: return RB;
                        case X360Button.LS: return LS;
                        case X360Button.RS: return RS;
                        case X360Button.Up: return Up;
                        case X360Button.Down: return Down;
                        case X360Button.Left: return Left;
                        case X360Button.Right: return Right;
                        case X360Button.Start: return Start;
                        case X360Button.Back: return Back;
                        case X360Button.Guide: return Back;
                        default: return false;
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
                            LX = value;
                            break;
                        case X360Axis.LX_Lo:
                            LX = -value;
                            break;
                        case X360Axis.LY_Hi:
                            LY = value;
                            break;
                        case X360Axis.LY_Lo:
                            LY = -value;
                            break;
                        case X360Axis.LT:
                            LT = value;
                            break;
                        case X360Axis.RX_Hi:
                            RX = value;
                            break;
                        case X360Axis.RX_Lo:
                            RX = -value;
                            break;
                        case X360Axis.RY_Hi:
                            RY = value;
                            break;
                        case X360Axis.RY_Lo:
                            RY = -value;
                            break;
                        case X360Axis.RT:
                            RT = value;
                            break;
                        default:
                            break;
                    }
                }

                get
                {
                    switch (axis)
                    {
                        case X360Axis.LX_Hi:
                        case X360Axis.LX_Lo:
                            return LX;
                        case X360Axis.LY_Hi:
                        case X360Axis.LY_Lo:
                            return LY;
                        case X360Axis.LT:
                            return LT;
                        case X360Axis.RX_Hi:
                        case X360Axis.RX_Lo:
                            return RX;
                        case X360Axis.RY_Hi:
                        case X360Axis.RY_Lo:
                            return RY;
                        case X360Axis.RT:
                            return RT;
                        default:
                            return 0;
                    }
                }
            }

            public void Reset()
            {
                A = B = X = Y = false;
                Up = Down = Left = Right = false;
                LB = RB = LS = RS = false;
                Start = Back = Guide = false;
                LX = LY = LT = 0;
                RX = RY = RT = 0;
            }
        }

        protected class BusAccess : BusDevice
        {
            public static BusAccess Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new BusAccess();
                        _instance.Open();
                        _instance.Start();
                    }

                    return _instance;
                }
            }

            public static BusAccess _instance;

            protected BusAccess()
            {
                App.Current.Exit += App_Exit;
        }

            private void App_Exit(object sender, System.Windows.ExitEventArgs e)
            {
                if (_instance != null)
        {
                    _instance.Stop();
                    _instance.Close();
                }
            }
        }

        protected class XInputBus
        {
            public static int Modifier;

            public XInputState inputs;
            public event RumbleChangeDelegate RumbleEvent;

            public int ID
            {
                get { return _id + Modifier; }
                protected set { _id = value; }
            }
            public bool PluggedIn { get; protected set; }

            protected BusAccess busRef;

            private int _id;
            private float tempLX = -10;
            private float tempLY = -10;
            private float tempRX = -10;
            private float tempRY = -10;

            public XInputBus(int id)
            {
                inputs = new XInputState();
                ID = id;
                busRef = BusAccess.Instance;
            }

            public bool Connect()
            {
                if (!PluggedIn)
                {
                    busRef.Unplug(ID);
                    PluggedIn = busRef.Plugin(ID);
                }

                return PluggedIn;
            }

            public bool Disconnect()
            {
                if (PluggedIn)
                {
                    PluggedIn = !busRef.Unplug(ID);
                    RumbleEvent?.Invoke(0, 0);
                }

                return PluggedIn == false;
            }

            public void SetInput(X360Button button, bool state)
            {
                inputs[button] = state;// || inputs[button];
            }

            public void SetInput(X360Axis axis, float value)
            {
                switch (axis)
                {
                    case X360Axis.LX_Hi:
                    case X360Axis.LX_Lo:
                        if (value > tempLX)
                        {
                            tempLX = value;
                inputs[axis] = value;
            }
                        break;

                    case X360Axis.LY_Hi:
                    case X360Axis.LY_Lo:
                        if (value > tempLY)
                        {
                            tempLY = value;
                            inputs[axis] = value;
                        }
                        break;

                    case X360Axis.RX_Hi:
                    case X360Axis.RX_Lo:
                        if (value > tempRX)
                        {
                            tempRX = value;
                            inputs[axis] = value;
                        }
                        break;

                    case X360Axis.RY_Hi:
                    case X360Axis.RY_Lo:
                        if (value > tempRY)
                        {
                            tempRY = value;
                            inputs[axis] = value;
                        }
                        break;

                    default:
                        inputs[axis] = value;// == 0 ? inputs[axis] : value;
                        break;
                }
            }

            public void Update()
            {
                //if (!Started) return;

                // reset temps
                tempLX = -10;
                tempLY = -10;
                tempRX = -10;
                tempRY = -10;

                byte[] rumble = new byte[8];
                byte[] output = new byte[28];

                // Fill the output to be sent
                output[0] = 0x1C;
                output[4] = (byte)ID;
                output[9] = 0x14;

                // buttons
                int buttonFlags = 0x00;
                output[10] |= (byte)(inputs.Up     ? 1 << 0 : 0);
                output[10] |= (byte)(inputs.Down   ? 1 << 1 : 0);
                output[10] |= (byte)(inputs.Left   ? 1 << 2 : 0);
                output[10] |= (byte)(inputs.Right  ? 1 << 3 : 0);
                output[10] |= (byte)(inputs.Start  ? 1 << 4 : 0);
                output[10] |= (byte)(inputs.Back   ? 1 << 5 : 0);
                output[10] |= (byte)(inputs.LS     ? 1 << 6 : 0);
                output[10] |= (byte)(inputs.RS     ? 1 << 7 : 0);
                output[11] |= (byte)(inputs.LB     ? 1 << 0 : 0);
                output[11] |= (byte)(inputs.RB     ? 1 << 1 : 0);
                output[11] |= (byte)(inputs.Guide  ? 1 << 2 : 0);
                output[11] |= (byte)(inputs.A      ? 1 << 4 : 0);
                output[11] |= (byte)(inputs.B      ? 1 << 5 : 0);
                output[11] |= (byte)(inputs.X      ? 1 << 6 : 0);
                output[11] |= (byte)(inputs.Y      ? 1 << 7 : 0);

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

                if (busRef.Report(output, rumble))
                {
                    // True on rumble state change
                    if (rumble[1] == 0x08)
                    {
                        RumbleEvent?.Invoke(rumble[3], rumble[4]);
                    }
                }

                //inputs.Reset();
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
