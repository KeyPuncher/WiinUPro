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
                    break;

                case DeviceState.Connected_XInput:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = false;
                    btnVjoy.IsEnabled       = false;
                    btnConfig.IsEnabled     = true;
                    btnDetatch.IsEnabled    = true;
                    break;

                case DeviceState.Connected_VJoy:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = false;
                    btnVjoy.IsEnabled       = false;
                    btnConfig.IsEnabled     = true;
                    btnDetatch.IsEnabled    = true;
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
                // TODO: Pro Controller Reading
                holder.SetValue("A", e.ProController.A);
                holder.SetValue("B", e.ProController.B);
                holder.SetValue("X", e.ProController.X);
                holder.SetValue("Y", e.ProController.Y);

                holder.SetValue("UP", e.ProController.Up);
                holder.SetValue("DOWN", e.ProController.Down);
                holder.SetValue("LEFT", e.ProController.Left);
                holder.SetValue("RIGHT", e.ProController.Right);

                holder.SetValue("L", e.ProController.L);
                holder.SetValue("R", e.ProController.R);
                holder.SetValue("ZL", e.ProController.ZL);
                holder.SetValue("ZR", e.ProController.ZR);

                holder.SetValue("START", e.ProController.Start);
                holder.SetValue("SELECT", e.ProController.Select);
                holder.SetValue("HOME", e.ProController.Home);
                holder.SetValue("LS", e.ProController.LS);
                holder.SetValue("RS", e.ProController.RS);

                //holder.SetValue("LRIGHT", e.ProController.LeftJoy.X > 0.1f);
                //holder.SetValue("LLEFT", e.ProController.LeftJoy.X < -0.1f);
                //holder.SetValue("LUP", e.ProController.LeftJoy.Y > 0.1f);
                //holder.SetValue("LDOWN", e.ProController.LeftJoy.Y < -0.1f);

                //holder.SetValue("RRIGHT", e.ProController.RightJoy.X > 0.1f);
                //holder.SetValue("RLEFT", e.ProController.RightJoy.X < -0.1f);
                //holder.SetValue("RUP", e.ProController.RightJoy.Y > 0.1f);
                //holder.SetValue("RDOWN", e.ProController.RightJoy.Y < -0.1f);

                holder.SetValue("LRIGHT", e.ProController.LeftJoy.X);
                holder.SetValue("LLEFT", e.ProController.LeftJoy.X * -1);
                holder.SetValue("LUP", e.ProController.LeftJoy.Y);
                holder.SetValue("LDOWN", e.ProController.LeftJoy.Y * -1);

                holder.SetValue("RRIGHT", e.ProController.RightJoy.X);
                holder.SetValue("RLEFT", e.ProController.RightJoy.X * -1);
                holder.SetValue("RUP", e.ProController.RightJoy.Y);
                holder.SetValue("RDOWN", e.ProController.RightJoy.Y * -1);
            }
            else if (DeviceType == ControllerType.BalanceBoard)
            {
                // TODO: Balance Board Reading
            }
            else
            {
                // TODO: Wiimote & extension Reading
                switch (DeviceType)
                {
                    case ControllerType.Nunchuk:
                    case ControllerType.NunchukB:
                        // TODO: Nunchuck Reading
                        break;

                    case ControllerType.ClassicController:
                        // TODO: Classic Controller Reading
                        break;

                    case ControllerType.ClassicControllerPro:
                        // TODO: Classic Controller Pro Reading
                        break;
                        
                    case ControllerType.MotionPlus:
                        // TODO: Motion Plus Reading
                        break;

                        // TODO: Musical Extension readins
                }
            }

            holder.Update();
        }
    }
}
