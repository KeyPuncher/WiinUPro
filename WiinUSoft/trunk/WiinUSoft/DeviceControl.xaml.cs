using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NintrollerLib;

namespace WiinUSoft
{
    public enum DeviceState
    {
        None = 0,
        Discovered,
        Connected_XInput,
        Connected_VJoy
    }

    public delegate void ConnectStateChange(DeviceControl sender, DeviceState oldState, DeviceState newState);

    public partial class DeviceControl : UserControl
    {
        private Nintroller device;
        internal Nintroller Device
        {
            get { return device; }
            set
            {
                device = value;

                if (device != null)
                {
                    device.ExtensionChange += device_ExtensionChange;
                    device.StateChange += device_StateChange;
                }
            }
        }
        internal bool Connected
        {
            get
            {
                if (device == null)
                    return false;

                return device.Connected;
            }
        }

        private ControllerType deviceType = ControllerType.Wiimote;
        internal ControllerType DeviceType { get; private set; }
        internal Holders.Holder holder;

        public event ConnectStateChange OnConnectStateChange;
        private DeviceState state;
        internal DeviceState ConnectionState
        {
            get
            {
                return state;
            }

            set
            {
                if (value != state)
                {
                    DeviceState previous = state;
                    SetState(value);
                    
                    if (OnConnectStateChange != null)
                    {
                        OnConnectStateChange(this, previous, value);
                    }
                }
            }
        }

        public DeviceControl()
        {
            InitializeComponent();
        }

        public DeviceControl(Nintroller nintroller) : this()
        {
            Device = nintroller;

            // TODO: Extract this method to a refresh state or something
            if (Device.ConnectTest())
            {
                ConnectionState = DeviceState.Discovered;
            }
        }

        public void SetName(string newName)
        {
            labelName.Content = new TextBox() { Text = newName };
        }

        public void SetState(DeviceState newState)
        {
            state = newState;

            switch (newState)
            {
                case DeviceState.Discovered:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = true;
                    btnVjoy.IsEnabled       = true;
                    btnConfig.IsEnabled     = false;
                    btnDetatch.IsEnabled    = false;
                    btnConfig.Visibility    = System.Windows.Visibility.Hidden;
                    btnDetatch.Visibility   = System.Windows.Visibility.Hidden;
                    break;

                case DeviceState.Connected_XInput:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = false;
                    btnVjoy.IsEnabled       = false;
                    btnConfig.IsEnabled     = true;
                    btnDetatch.IsEnabled    = true;
                    btnConfig.Visibility    = System.Windows.Visibility.Visible;
                    btnDetatch.Visibility   = System.Windows.Visibility.Visible;

                    // TODO: Instantiate holder
                    var xHolder = new Holders.XInputHolder(device.Type);
                    xHolder.ConnectXInput(1);
                    holder = xHolder;
                    break;

                case DeviceState.Connected_VJoy:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = false;
                    btnVjoy.IsEnabled       = false;
                    btnConfig.IsEnabled     = true;
                    btnDetatch.IsEnabled    = true;
                    btnConfig.Visibility    = System.Windows.Visibility.Visible;
                    btnDetatch.Visibility   = System.Windows.Visibility.Visible;

                    // TODO: Instantiate VJoy Holder
                    break;
            }
        }

        void device_ExtensionChange(object sender, ExtensionChangeEventArgs e)
        {
            DeviceType = e.Extension;
        }

        void device_StateChange(object sender, StateChangeEventArgs e)
        {
            if (holder == null)
            {
                return;
            }
            
            if (DeviceType == ControllerType.ProController)
            {
                holder.SetValue(Inputs.ProController.A, e.ProController.A);
                holder.SetValue(Inputs.ProController.B, e.ProController.B);
                holder.SetValue(Inputs.ProController.X, e.ProController.X);
                holder.SetValue(Inputs.ProController.Y, e.ProController.Y);
                                
                holder.SetValue(Inputs.ProController.UP, e.ProController.Up);
                holder.SetValue(Inputs.ProController.DOWN, e.ProController.Down);
                holder.SetValue(Inputs.ProController.LEFT, e.ProController.Left);
                holder.SetValue(Inputs.ProController.RIGHT, e.ProController.Right);
                                
                holder.SetValue(Inputs.ProController.L, e.ProController.L);
                holder.SetValue(Inputs.ProController.R, e.ProController.R);
                holder.SetValue(Inputs.ProController.ZL, e.ProController.ZL);
                holder.SetValue(Inputs.ProController.ZR, e.ProController.ZR);
                                
                holder.SetValue(Inputs.ProController.START, e.ProController.Start);
                holder.SetValue(Inputs.ProController.SELECT, e.ProController.Select);
                holder.SetValue(Inputs.ProController.HOME, e.ProController.Home);
                holder.SetValue(Inputs.ProController.LS, e.ProController.LS);
                holder.SetValue(Inputs.ProController.RS, e.ProController.RS);

                //holder.SetValue("LRIGHT", e.ProController.LeftJoy.X > 0.1f);
                //holder.SetValue("LLEFT", e.ProController.LeftJoy.X < -0.1f);
                //holder.SetValue("LUP", e.ProController.LeftJoy.Y > 0.1f);
                //holder.SetValue("LDOWN", e.ProController.LeftJoy.Y < -0.1f);

                //holder.SetValue("RRIGHT", e.ProController.RightJoy.X > 0.1f);
                //holder.SetValue("RLEFT", e.ProController.RightJoy.X < -0.1f);
                //holder.SetValue("RUP", e.ProController.RightJoy.Y > 0.1f);
                //holder.SetValue("RDOWN", e.ProController.RightJoy.Y < -0.1f);

                holder.SetValue(Inputs.ProController.LRIGHT, e.ProController.LeftJoy.X > 0 ? e.ProController.LeftJoy.X : 0f);
                holder.SetValue(Inputs.ProController.LLEFT , e.ProController.LeftJoy.X < 0 ? e.ProController.LeftJoy.X * -1 : 0f);
                holder.SetValue(Inputs.ProController.LUP   , e.ProController.LeftJoy.Y > 0 ? e.ProController.LeftJoy.Y : 0f);
                holder.SetValue(Inputs.ProController.LDOWN , e.ProController.LeftJoy.Y < 0 ? e.ProController.LeftJoy.Y * -1 : 0f);
                                
                holder.SetValue(Inputs.ProController.RRIGHT, e.ProController.RightJoy.X > 0 ? e.ProController.RightJoy.X : 0f);
                holder.SetValue(Inputs.ProController.RLEFT , e.ProController.RightJoy.X < 0 ? e.ProController.RightJoy.X * -1 : 0f);
                holder.SetValue(Inputs.ProController.RUP   , e.ProController.RightJoy.Y > 0 ? e.ProController.RightJoy.Y : 0f);
                holder.SetValue(Inputs.ProController.RDOWN , e.ProController.RightJoy.Y < 0 ? e.ProController.RightJoy.Y * -1 : 0f);

                bool doRumble = holder.GetFlag(Inputs.Flags.RUMBLE);
                if (doRumble != e.ProController.Rumble)
                {
                    device.SetRumble(doRumble);
                }
            }
            else if (DeviceType == ControllerType.BalanceBoard)
            {
                // TODO: Balance Board Reading
            }
            else
            {
                // TODO: Wiimote & extension Reading
                holder.SetValue("A", e.Wiimote.A);
                holder.SetValue("B", e.Wiimote.B);
                holder.SetValue("ONE", e.Wiimote.One);
                holder.SetValue("TWO", e.Wiimote.Two);

                holder.SetValue("UP", e.Wiimote.Up);
                holder.SetValue("DOWN", e.Wiimote.Down);
                holder.SetValue("LEFT", e.Wiimote.Left);
                holder.SetValue("RIGHT", e.Wiimote.Right);

                holder.SetValue("MINUS", e.Wiimote.Minus);
                holder.SetValue("PLUS", e.Wiimote.Plus);
                holder.SetValue("HOME", e.Wiimote.Home);

                // Accelerometer and IR sensor

                switch (DeviceType)
                {
                    case ControllerType.Nunchuk:
                    case ControllerType.NunchukB:
                        // TODO: Nunchuck Reading
                        holder.SetValue("eC", e.Wiimote.Nunchuck.C);
                        holder.SetValue("eZ", e.Wiimote.Nunchuck.Z);

                        holder.SetValue("eJRIGHT", e.Wiimote.Nunchuck.Joy.X > 0 ? e.Wiimote.Nunchuck.Joy.X : 0f);
                        holder.SetValue("eJLEFT" , e.Wiimote.Nunchuck.Joy.X < 0 ? e.Wiimote.Nunchuck.Joy.X * -1 : 0f);
                        holder.SetValue("eJUP"   , e.Wiimote.Nunchuck.Joy.Y > 0 ? e.Wiimote.Nunchuck.Joy.Y : 0f);
                        holder.SetValue("eJDOWN" , e.Wiimote.Nunchuck.Joy.Y < 0 ? e.Wiimote.Nunchuck.Joy.Y * -1 : 0f);

                        // Accelerometer
                        break;

                    case ControllerType.ClassicController:
                        holder.SetValue("eA", e.Wiimote.ClassicController.A);
                        holder.SetValue("eB", e.Wiimote.ClassicController.B);
                        holder.SetValue("eX", e.Wiimote.ClassicController.X);
                        holder.SetValue("eY", e.Wiimote.ClassicController.Y);

                        holder.SetValue("eUP", e.Wiimote.ClassicController.Up);
                        holder.SetValue("eDOWN", e.Wiimote.ClassicController.Down);
                        holder.SetValue("eLEFT", e.Wiimote.ClassicController.Left);
                        holder.SetValue("eRIGHT", e.Wiimote.ClassicController.Right);

                        holder.SetValue("eL", e.Wiimote.ClassicController.L);
                        holder.SetValue("eR", e.Wiimote.ClassicController.R);
                        holder.SetValue("eZL", e.Wiimote.ClassicController.ZL);
                        holder.SetValue("eZR", e.Wiimote.ClassicController.ZR);

                        holder.SetValue("eSTART", e.Wiimote.ClassicController.Start);
                        holder.SetValue("eSELECT", e.Wiimote.ClassicController.Select);
                        holder.SetValue("eHOME", e.Wiimote.ClassicController.Home);

                        holder.SetValue("eLFULL", e.Wiimote.ClassicController.LFull);
                        holder.SetValue("eRFULL", e.Wiimote.ClassicController.RFull);
                        holder.SetValue("eLTRIGGER", e.Wiimote.ClassicController.LTrigger);
                        holder.SetValue("eRTRIGGER", e.Wiimote.ClassicController.RTrigger);

                        holder.SetValue("eLRIGHT", e.Wiimote.ClassicController.LeftJoy.X > 0 ? e.Wiimote.ClassicController.LeftJoy.X : 0f);
                        holder.SetValue("eLLEFT" , e.Wiimote.ClassicController.LeftJoy.X < 0 ? e.Wiimote.ClassicController.LeftJoy.X * -1 : 0f);
                        holder.SetValue("eLUP"   , e.Wiimote.ClassicController.LeftJoy.Y > 0 ? e.Wiimote.ClassicController.LeftJoy.Y : 0f);
                        holder.SetValue("eLDOWN" , e.Wiimote.ClassicController.LeftJoy.Y < 0 ? e.Wiimote.ClassicController.LeftJoy.Y * -1 : 0f);

                        holder.SetValue("eRRIGHT", e.Wiimote.ClassicController.RightJoy.X > 0 ? e.Wiimote.ClassicController.RightJoy.X : 0f);
                        holder.SetValue("eRLEFT" , e.Wiimote.ClassicController.RightJoy.X < 0 ? e.Wiimote.ClassicController.RightJoy.X * -1 : 0f);
                        holder.SetValue("eRUP"   , e.Wiimote.ClassicController.RightJoy.Y > 0 ? e.Wiimote.ClassicController.RightJoy.Y : 0f);
                        holder.SetValue("eRDOWN" , e.Wiimote.ClassicController.RightJoy.Y < 0 ? e.Wiimote.ClassicController.RightJoy.Y * -1 : 0f);
                        break;

                    case ControllerType.ClassicControllerPro:
                        holder.SetValue("eA", e.Wiimote.ClassicControllerPro.A);
                        holder.SetValue("eB", e.Wiimote.ClassicControllerPro.B);
                        holder.SetValue("eX", e.Wiimote.ClassicControllerPro.X);
                        holder.SetValue("eY", e.Wiimote.ClassicControllerPro.Y);

                        holder.SetValue("eUP", e.Wiimote.ClassicControllerPro.Up);
                        holder.SetValue("eDOWN", e.Wiimote.ClassicControllerPro.Down);
                        holder.SetValue("eLEFT", e.Wiimote.ClassicControllerPro.Left);
                        holder.SetValue("eRIGHT", e.Wiimote.ClassicControllerPro.Right);

                        holder.SetValue("eL", e.Wiimote.ClassicControllerPro.L);
                        holder.SetValue("eR", e.Wiimote.ClassicControllerPro.R);
                        holder.SetValue("eZL", e.Wiimote.ClassicControllerPro.ZL);
                        holder.SetValue("eZR", e.Wiimote.ClassicControllerPro.ZR);

                        holder.SetValue("eSTART", e.Wiimote.ClassicControllerPro.Start);
                        holder.SetValue("eSELECT", e.Wiimote.ClassicControllerPro.Select);
                        holder.SetValue("eHOME", e.Wiimote.ClassicControllerPro.Home);

                        holder.SetValue("eLRIGHT", e.Wiimote.ClassicControllerPro.LeftJoy.X > 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.X : 0f);
                        holder.SetValue("eLLEFT" , e.Wiimote.ClassicControllerPro.LeftJoy.X < 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.X * -1 : 0f);
                        holder.SetValue("eLUP"   , e.Wiimote.ClassicControllerPro.LeftJoy.Y > 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.Y : 0f);
                        holder.SetValue("eLDOWN" , e.Wiimote.ClassicControllerPro.LeftJoy.Y < 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.Y * -1 : 0f);

                        holder.SetValue("eRRIGHT", e.Wiimote.ClassicControllerPro.RightJoy.X > 0 ? e.Wiimote.ClassicControllerPro.RightJoy.X : 0f);
                        holder.SetValue("eRLEFT" , e.Wiimote.ClassicControllerPro.RightJoy.X < 0 ? e.Wiimote.ClassicControllerPro.RightJoy.X * -1 : 0f);
                        holder.SetValue("eRUP"   , e.Wiimote.ClassicControllerPro.RightJoy.Y > 0 ? e.Wiimote.ClassicControllerPro.RightJoy.Y : 0f);
                        holder.SetValue("eRDOWN" , e.Wiimote.ClassicControllerPro.RightJoy.Y < 0 ? e.Wiimote.ClassicControllerPro.RightJoy.Y * -1 : 0f);
                        break;
                        
                    case ControllerType.MotionPlus:
                        // TODO: Motion Plus Reading
                        break;

                        // TODO: Musical Extension readins
                }
            }

            holder.Update();
        }

        private void btnXinput_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Select which device number to connect to & if available
            if (device.Connect())
            {
                ConnectionState = DeviceState.Connected_XInput;
            }
        }
    }
}
