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

        internal int targetXDevice = 0;

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

        public void Detatch()
        {
            device.Disconnect();
            holder.Close();
            ConnectionState = DeviceState.Discovered;
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

                    var xHolder = new Holders.XInputHolder(device.Type);
                    xHolder.ConnectXInput(targetXDevice);
                    holder = xHolder;
                    device.SetPlayerLED(targetXDevice);
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
                holder.SetValue(Inputs.Wiimote.A, e.Wiimote.A);
                holder.SetValue(Inputs.Wiimote.B, e.Wiimote.B);
                holder.SetValue(Inputs.Wiimote.ONE, e.Wiimote.One);
                holder.SetValue(Inputs.Wiimote.TWO, e.Wiimote.Two);

                holder.SetValue(Inputs.Wiimote.UP, e.Wiimote.Up);
                holder.SetValue(Inputs.Wiimote.DOWN, e.Wiimote.Down);
                holder.SetValue(Inputs.Wiimote.LEFT, e.Wiimote.Left);
                holder.SetValue(Inputs.Wiimote.RIGHT, e.Wiimote.Right);

                holder.SetValue(Inputs.Wiimote.MINUS, e.Wiimote.Minus);
                holder.SetValue(Inputs.Wiimote.PLUS, e.Wiimote.Plus);
                holder.SetValue(Inputs.Wiimote.HOME, e.Wiimote.Home);

                // Accelerometer and IR sensor

                switch (DeviceType)
                {
                    case ControllerType.Nunchuk:
                    case ControllerType.NunchukB:
                        // TODO: Nunchuck Reading
                        holder.SetValue(Inputs.Nunchuk.C, e.Wiimote.Nunchuck.C);
                        holder.SetValue(Inputs.Nunchuk.Z, e.Wiimote.Nunchuck.Z);

                        holder.SetValue(Inputs.Nunchuk.RIGHT, e.Wiimote.Nunchuck.Joy.X > 0 ? e.Wiimote.Nunchuck.Joy.X : 0f);
                        holder.SetValue(Inputs.Nunchuk.LEFT , e.Wiimote.Nunchuck.Joy.X < 0 ? e.Wiimote.Nunchuck.Joy.X * -1 : 0f);
                        holder.SetValue(Inputs.Nunchuk.UP   , e.Wiimote.Nunchuck.Joy.Y > 0 ? e.Wiimote.Nunchuck.Joy.Y : 0f);
                        holder.SetValue(Inputs.Nunchuk.DOWN , e.Wiimote.Nunchuck.Joy.Y < 0 ? e.Wiimote.Nunchuck.Joy.Y * -1 : 0f);

                        // Accelerometer
                        break;

                    case ControllerType.ClassicController:
                        holder.SetValue(Inputs.ClassicController.A, e.Wiimote.ClassicController.A);
                        holder.SetValue(Inputs.ClassicController.B, e.Wiimote.ClassicController.B);
                        holder.SetValue(Inputs.ClassicController.X, e.Wiimote.ClassicController.X);
                        holder.SetValue(Inputs.ClassicController.Y, e.Wiimote.ClassicController.Y);
                                        
                        holder.SetValue(Inputs.ClassicController.UP, e.Wiimote.ClassicController.Up);
                        holder.SetValue(Inputs.ClassicController.DOWN, e.Wiimote.ClassicController.Down);
                        holder.SetValue(Inputs.ClassicController.LEFT, e.Wiimote.ClassicController.Left);
                        holder.SetValue(Inputs.ClassicController.RIGHT, e.Wiimote.ClassicController.Right);
                                        
                        holder.SetValue(Inputs.ClassicController.L, e.Wiimote.ClassicController.L);
                        holder.SetValue(Inputs.ClassicController.R, e.Wiimote.ClassicController.R);
                        holder.SetValue(Inputs.ClassicController.ZL, e.Wiimote.ClassicController.ZL);
                        holder.SetValue(Inputs.ClassicController.ZR, e.Wiimote.ClassicController.ZR);
                                       
                        holder.SetValue(Inputs.ClassicController.START, e.Wiimote.ClassicController.Start);
                        holder.SetValue(Inputs.ClassicController.SELECT, e.Wiimote.ClassicController.Select);
                        holder.SetValue(Inputs.ClassicController.HOME, e.Wiimote.ClassicController.Home);
                                        
                        holder.SetValue(Inputs.ClassicController.LFULL, e.Wiimote.ClassicController.LFull);
                        holder.SetValue(Inputs.ClassicController.RFULL, e.Wiimote.ClassicController.RFull);
                        holder.SetValue(Inputs.ClassicController.LT, e.Wiimote.ClassicController.LTrigger);
                        holder.SetValue(Inputs.ClassicController.LT, e.Wiimote.ClassicController.RTrigger);
                                        
                        holder.SetValue(Inputs.ClassicController.LRIGHT, e.Wiimote.ClassicController.LeftJoy.X > 0 ? e.Wiimote.ClassicController.LeftJoy.X : 0f);
                        holder.SetValue(Inputs.ClassicController.LLEFT , e.Wiimote.ClassicController.LeftJoy.X < 0 ? e.Wiimote.ClassicController.LeftJoy.X * -1 : 0f);
                        holder.SetValue(Inputs.ClassicController.LUP   , e.Wiimote.ClassicController.LeftJoy.Y > 0 ? e.Wiimote.ClassicController.LeftJoy.Y : 0f);
                        holder.SetValue(Inputs.ClassicController.LDOWN , e.Wiimote.ClassicController.LeftJoy.Y < 0 ? e.Wiimote.ClassicController.LeftJoy.Y * -1 : 0f);
                                        
                        holder.SetValue(Inputs.ClassicController.RRIGHT, e.Wiimote.ClassicController.RightJoy.X > 0 ? e.Wiimote.ClassicController.RightJoy.X : 0f);
                        holder.SetValue(Inputs.ClassicController.RLEFT , e.Wiimote.ClassicController.RightJoy.X < 0 ? e.Wiimote.ClassicController.RightJoy.X * -1 : 0f);
                        holder.SetValue(Inputs.ClassicController.RUP   , e.Wiimote.ClassicController.RightJoy.Y > 0 ? e.Wiimote.ClassicController.RightJoy.Y : 0f);
                        holder.SetValue(Inputs.ClassicController.RDOWN , e.Wiimote.ClassicController.RightJoy.Y < 0 ? e.Wiimote.ClassicController.RightJoy.Y * -1 : 0f);
                        break;

                    case ControllerType.ClassicControllerPro:
                        holder.SetValue(Inputs.ClassicControllerPro.A, e.Wiimote.ClassicControllerPro.A);
                        holder.SetValue(Inputs.ClassicControllerPro.B, e.Wiimote.ClassicControllerPro.B);
                        holder.SetValue(Inputs.ClassicControllerPro.X, e.Wiimote.ClassicControllerPro.X);
                        holder.SetValue(Inputs.ClassicControllerPro.Y, e.Wiimote.ClassicControllerPro.Y);

                        holder.SetValue(Inputs.ClassicControllerPro.UP, e.Wiimote.ClassicControllerPro.Up);
                        holder.SetValue(Inputs.ClassicControllerPro.DOWN, e.Wiimote.ClassicControllerPro.Down);
                        holder.SetValue(Inputs.ClassicControllerPro.LEFT, e.Wiimote.ClassicControllerPro.Left);
                        holder.SetValue(Inputs.ClassicControllerPro.RIGHT, e.Wiimote.ClassicControllerPro.Right);

                        holder.SetValue(Inputs.ClassicControllerPro.L, e.Wiimote.ClassicControllerPro.L);
                        holder.SetValue(Inputs.ClassicControllerPro.R, e.Wiimote.ClassicControllerPro.R);
                        holder.SetValue(Inputs.ClassicControllerPro.ZL, e.Wiimote.ClassicControllerPro.ZL);
                        holder.SetValue(Inputs.ClassicControllerPro.ZR, e.Wiimote.ClassicControllerPro.ZR);

                        holder.SetValue(Inputs.ClassicControllerPro.START, e.Wiimote.ClassicControllerPro.Start);
                        holder.SetValue(Inputs.ClassicControllerPro.SELECT, e.Wiimote.ClassicControllerPro.Select);
                        holder.SetValue(Inputs.ClassicControllerPro.HOME, e.Wiimote.ClassicControllerPro.Home);

                        holder.SetValue(Inputs.ClassicControllerPro.LRIGHT, e.Wiimote.ClassicControllerPro.LeftJoy.X > 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.X : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.LLEFT , e.Wiimote.ClassicControllerPro.LeftJoy.X < 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.X * -1 : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.LUP   , e.Wiimote.ClassicControllerPro.LeftJoy.Y > 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.Y : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.LDOWN , e.Wiimote.ClassicControllerPro.LeftJoy.Y < 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.Y * -1 : 0f);

                        holder.SetValue(Inputs.ClassicControllerPro.RRIGHT, e.Wiimote.ClassicControllerPro.RightJoy.X > 0 ? e.Wiimote.ClassicControllerPro.RightJoy.X : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.RLEFT , e.Wiimote.ClassicControllerPro.RightJoy.X < 0 ? e.Wiimote.ClassicControllerPro.RightJoy.X * -1 : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.RUP   , e.Wiimote.ClassicControllerPro.RightJoy.Y > 0 ? e.Wiimote.ClassicControllerPro.RightJoy.Y : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.RDOWN , e.Wiimote.ClassicControllerPro.RightJoy.Y < 0 ? e.Wiimote.ClassicControllerPro.RightJoy.Y * -1 : 0f);
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
            if (btnXinput.ContextMenu != null)
            {
                XOption1.IsEnabled = Holders.XInputHolder.availabe[0];
                XOption2.IsEnabled = Holders.XInputHolder.availabe[1];
                XOption3.IsEnabled = Holders.XInputHolder.availabe[2];
                XOption4.IsEnabled = Holders.XInputHolder.availabe[3];

                btnXinput.ContextMenu.PlacementTarget = btnXinput;
                btnXinput.ContextMenu.IsOpen = true;
            }
        }

        private void XOption_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Select which device number to connect to & if available
            if (device.ConnectTest() && device.Connect())
            {
                int tmp = 0;
                if (int.TryParse(((MenuItem)sender).Name.Replace("XOption", ""), out tmp))
                {
                    targetXDevice = tmp;
                    ConnectionState = DeviceState.Connected_XInput;
                }
            }
        }

        private void btnDetatch_Click(object sender, RoutedEventArgs e)
        {
            Detatch();
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Open mapping screen
            var config = new ConfigWindow(holder.Mappings, deviceType);
            config.ShowDialog();
            if (config.result)
            {
                foreach (KeyValuePair<string, string> pair in config.map)
                {
                    holder.SetMapping(pair.Key, pair.Value);
                }
            }
        }
    }
}