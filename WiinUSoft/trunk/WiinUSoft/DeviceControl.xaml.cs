using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NintrollerLib;
using System.Xml.Serialization;
using System.IO;

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

        internal ControllerType DeviceType { get; private set; }
        internal Holders.Holder holder;
        internal Property properties;

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
        internal bool lowBatteryFired = false;
        internal string dName = "";

        internal System.Threading.Timer updateTimer;
        internal const int UPDATE_SPEED = 25;

        public DeviceControl()
        {
            InitializeComponent();
        }

        public DeviceControl(Nintroller nintroller)
            : this()
        {
            Device = nintroller;
            //RefreshState();
        }

        public void RefreshState()
        {
            if (Device.ConnectTest())
            {
                ConnectionState = DeviceState.Discovered;

                if (device.Type != ControllerType.ProController && device.Type != ControllerType.BalanceBoard)
                {
                    device.Connect();
                    device.Disconnect();
                }

                UpdateIcon(device.Type);
                SetName(device.Type.ToString());

                // Load Properties
                properties = UserPrefs.Instance.GetDevicePref(device.HIDPath);
                if (properties != null)
                {
                    SetName(string.IsNullOrWhiteSpace(properties.name) ? device.Type.ToString() : properties.name);
                }
                else
                {
                    properties = new Property(device.HIDPath);
                }
            }
            else
            {
                ConnectionState = DeviceState.None;
            }
        }

        public void SetName(string newName)
        {
            dName = newName;
            labelName.Content = new TextBlock() { Text = newName };
        }

        public void Detatch()
        {
            device.Disconnect();
            holder.Close();
            lowBatteryFired = false;
            ConnectionState = DeviceState.Discovered;
            Dispatcher.BeginInvoke
            (
                System.Windows.Threading.DispatcherPriority.Background,
                new Action(() => statusGradient.Color = (Color)FindResource("AntemBlue")
            ));
        }

        public void SetState(DeviceState newState)
        {
            state = newState;
            if (updateTimer != null)
            {
                updateTimer.Dispose();
                updateTimer = null;
            }

            switch (newState)
            {
                case DeviceState.None:
                    btnIdentify.IsEnabled   = false;
                    btnProperties.IsEnabled = false;
                    btnXinput.IsEnabled     = false;
                    //btnVjoy.IsEnabled     = false;
                    btnConfig.IsEnabled     = false;
                    btnDetatch.IsEnabled    = false;
                    btnConfig.Visibility    = System.Windows.Visibility.Hidden;
                    btnDetatch.Visibility   = System.Windows.Visibility.Hidden;
                    break;

                case DeviceState.Discovered:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = true;
                    //btnVjoy.IsEnabled     = true;
                    btnConfig.IsEnabled     = false;
                    btnDetatch.IsEnabled    = false;
                    btnConfig.Visibility    = System.Windows.Visibility.Hidden;
                    btnDetatch.Visibility   = System.Windows.Visibility.Hidden;
                    break;

                case DeviceState.Connected_XInput:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = false;
                    //btnVjoy.IsEnabled     = false;
                    btnConfig.IsEnabled     = true;
                    btnDetatch.IsEnabled    = true;
                    btnConfig.Visibility    = System.Windows.Visibility.Visible;
                    btnDetatch.Visibility   = System.Windows.Visibility.Visible;

                    var xHolder = new Holders.XInputHolder(device.Type);
                    LoadProfile(properties.profile, xHolder);
                    xHolder.ConnectXInput(targetXDevice);
                    holder = xHolder;
                    device.SetPlayerLED(targetXDevice);
                    updateTimer = new System.Threading.Timer(HolderUpdate, device, 1000, UPDATE_SPEED);
                    break;

                //case DeviceState.Connected_VJoy:
                //    btnIdentify.IsEnabled = true;
                //    btnProperties.IsEnabled = true;
                //    btnXinput.IsEnabled = false;
                //    btnVjoy.IsEnabled = false;
                //    btnConfig.IsEnabled = true;
                //    btnDetatch.IsEnabled = true;
                //    btnConfig.Visibility = System.Windows.Visibility.Visible;
                //    btnDetatch.Visibility = System.Windows.Visibility.Visible;

                //    // Instantiate VJoy Holder (not for 1st release)
                //    break;
            }
        }

        void device_ExtensionChange(object sender, ExtensionChangeEventArgs e)
        {
            DeviceType = e.Extension;

            if (holder != null)
            {
                holder.AddMapping(DeviceType);
            }

            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, 
                new Action(() => UpdateIcon(DeviceType)
            ));
        }

        void device_StateChange(object sender, StateChangeEventArgs e)
        {
            // Makes the timer wait
            if (updateTimer != null) updateTimer.Change(1000, UPDATE_SPEED);

            if (holder == null)
            {
                return;
            }

            bool lowBat = false;

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
                holder.SetValue(Inputs.ProController.LLEFT, e.ProController.LeftJoy.X < 0 ? e.ProController.LeftJoy.X * -1 : 0f);
                holder.SetValue(Inputs.ProController.LUP, e.ProController.LeftJoy.Y > 0 ? e.ProController.LeftJoy.Y : 0f);
                holder.SetValue(Inputs.ProController.LDOWN, e.ProController.LeftJoy.Y < 0 ? e.ProController.LeftJoy.Y * -1 : 0f);

                holder.SetValue(Inputs.ProController.RRIGHT, e.ProController.RightJoy.X > 0 ? e.ProController.RightJoy.X : 0f);
                holder.SetValue(Inputs.ProController.RLEFT, e.ProController.RightJoy.X < 0 ? e.ProController.RightJoy.X * -1 : 0f);
                holder.SetValue(Inputs.ProController.RUP, e.ProController.RightJoy.Y > 0 ? e.ProController.RightJoy.Y : 0f);
                holder.SetValue(Inputs.ProController.RDOWN, e.ProController.RightJoy.Y < 0 ? e.ProController.RightJoy.Y * -1 : 0f);

                //bool doRumble = properties.useRumble && holder.GetFlag(Inputs.Flags.RUMBLE);
                //if (doRumble != e.ProController.Rumble)
                //{
                //    device.SetRumble(doRumble);
                //}

                float intensity = 0;
                if (holder.Values.TryGetValue(Inputs.Flags.RUMBLE, out intensity))
                {
                    rumbleIntensity = (int)intensity;
                    RumbleStep();
                }

                lowBat = e.ProController.BatteryLow && !e.ProController.Charging;
            }
            else if (DeviceType == ControllerType.BalanceBoard)
            {
                // TODO: Balance Board Reading (not for 1st release)
            }
            else
            {
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

                //TODO: Wiimote Accelerometer and IR sensor Reading (not for 1st release)

                switch (DeviceType)
                {
                    case ControllerType.Nunchuk:
                    case ControllerType.NunchukB:
                        holder.SetValue(Inputs.Nunchuk.C, e.Wiimote.Nunchuck.C);
                        holder.SetValue(Inputs.Nunchuk.Z, e.Wiimote.Nunchuck.Z);

                        holder.SetValue(Inputs.Nunchuk.RIGHT, e.Wiimote.Nunchuck.Joy.X > 0 ? e.Wiimote.Nunchuck.Joy.X : 0f);
                        holder.SetValue(Inputs.Nunchuk.LEFT, e.Wiimote.Nunchuck.Joy.X < 0 ? e.Wiimote.Nunchuck.Joy.X * -1 : 0f);
                        holder.SetValue(Inputs.Nunchuk.UP, e.Wiimote.Nunchuck.Joy.Y > 0 ? e.Wiimote.Nunchuck.Joy.Y : 0f);
                        holder.SetValue(Inputs.Nunchuk.DOWN, e.Wiimote.Nunchuck.Joy.Y < 0 ? e.Wiimote.Nunchuck.Joy.Y * -1 : 0f);

                        //TODO: Nunchuk Accelerometer (not for 1st release)
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
                        holder.SetValue(Inputs.ClassicController.LT, e.Wiimote.ClassicController.LTrigger > 0.1f ? e.Wiimote.ClassicController.LTrigger : 0f);
                        holder.SetValue(Inputs.ClassicController.RT, e.Wiimote.ClassicController.RTrigger > 0.1f ? e.Wiimote.ClassicController.RTrigger : 0f);

                        holder.SetValue(Inputs.ClassicController.LRIGHT, e.Wiimote.ClassicController.LeftJoy.X > 0 ? e.Wiimote.ClassicController.LeftJoy.X : 0f);
                        holder.SetValue(Inputs.ClassicController.LLEFT, e.Wiimote.ClassicController.LeftJoy.X < 0 ? e.Wiimote.ClassicController.LeftJoy.X * -1 : 0f);
                        holder.SetValue(Inputs.ClassicController.LUP, e.Wiimote.ClassicController.LeftJoy.Y > 0 ? e.Wiimote.ClassicController.LeftJoy.Y : 0f);
                        holder.SetValue(Inputs.ClassicController.LDOWN, e.Wiimote.ClassicController.LeftJoy.Y < 0 ? e.Wiimote.ClassicController.LeftJoy.Y * -1 : 0f);

                        holder.SetValue(Inputs.ClassicController.RRIGHT, e.Wiimote.ClassicController.RightJoy.X > 0 ? e.Wiimote.ClassicController.RightJoy.X : 0f);
                        holder.SetValue(Inputs.ClassicController.RLEFT, e.Wiimote.ClassicController.RightJoy.X < 0 ? e.Wiimote.ClassicController.RightJoy.X * -1 : 0f);
                        holder.SetValue(Inputs.ClassicController.RUP, e.Wiimote.ClassicController.RightJoy.Y > 0 ? e.Wiimote.ClassicController.RightJoy.Y : 0f);
                        holder.SetValue(Inputs.ClassicController.RDOWN, e.Wiimote.ClassicController.RightJoy.Y < 0 ? e.Wiimote.ClassicController.RightJoy.Y * -1 : 0f);
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
                        holder.SetValue(Inputs.ClassicControllerPro.LLEFT, e.Wiimote.ClassicControllerPro.LeftJoy.X < 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.X * -1 : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.LUP, e.Wiimote.ClassicControllerPro.LeftJoy.Y > 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.Y : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.LDOWN, e.Wiimote.ClassicControllerPro.LeftJoy.Y < 0 ? e.Wiimote.ClassicControllerPro.LeftJoy.Y * -1 : 0f);

                        holder.SetValue(Inputs.ClassicControllerPro.RRIGHT, e.Wiimote.ClassicControllerPro.RightJoy.X > 0 ? e.Wiimote.ClassicControllerPro.RightJoy.X : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.RLEFT, e.Wiimote.ClassicControllerPro.RightJoy.X < 0 ? e.Wiimote.ClassicControllerPro.RightJoy.X * -1 : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.RUP, e.Wiimote.ClassicControllerPro.RightJoy.Y > 0 ? e.Wiimote.ClassicControllerPro.RightJoy.Y : 0f);
                        holder.SetValue(Inputs.ClassicControllerPro.RDOWN, e.Wiimote.ClassicControllerPro.RightJoy.Y < 0 ? e.Wiimote.ClassicControllerPro.RightJoy.Y * -1 : 0f);
                        break;

                    case ControllerType.MotionPlus:
                        // TODO: Motion Plus Reading (not for 1st release)
                        break;

                    // TODO: Musical Extension readings (not for 1st release)
                }

                // Rumble is currently disabled because the wiimote only reports when something changes (which can be changed)
                //bool doRumble = properties.useRumble && holder.GetFlag(Inputs.Flags.RUMBLE);
                //if (doRumble != e.Wiimote.Rumble)
                //{
                //    device.SetRumble(doRumble);
                //}

                //lowBat = e.Wiimote.BatteryLow || e.Wiimote.Battery == BatteryStatus.VeryLow;

                float intensity = 0;
                if (holder.Values.TryGetValue(Inputs.Flags.RUMBLE, out intensity))
                {
                    rumbleIntensity = (int)intensity;
                    RumbleStep();
                }
            }

            holder.Update();
            SetBatteryStatus(lowBat);

            // Resumes the timer in case this method is not called withing 100ms
            if (updateTimer != null) updateTimer.Change(100, UPDATE_SPEED);
        }

        private void HolderUpdate(object state)
        {
            holder.Update();

            float intensity = 0;
            if (holder.Values.TryGetValue(Inputs.Flags.RUMBLE, out intensity))
            {
                rumbleIntensity = (int)intensity;
                RumbleStep();
            }

            //bool doRumble = properties.useRumble && holder.GetFlag(Inputs.Flags.RUMBLE);
            //if (doRumble != device.State.GetRumble())
            //{
            //    device.SetRumble(doRumble);
            //}
            //rumbler.SetIntensity(doRumble ? (int)holder.Values[Inputs.Flags.RUMBLE] : 0);

            SetBatteryStatus(device.State.BatteryLow);
        }

        int rumbleIntensity = 0;
        int rumbleStepCount = 0;
        int rumbleStepPeriod = 10;
        float rumbleSlowMult = 0.5f;
        void RumbleStep()
        {
            bool currentRumbleState = device.State.GetRumble();

            float dutyCycle = 0;

            if (rumbleIntensity < 256)
            {
                dutyCycle = rumbleSlowMult * (float)rumbleIntensity / 256f;
            }
            else
            {
                dutyCycle = (float)rumbleIntensity / 65535f;
            }

            int stopStep = (int)Math.Round(dutyCycle * rumbleStepPeriod);

            if (rumbleStepCount < stopStep)
            {
                if (!currentRumbleState) device.SetRumble(true);
            }
            else
            {
                if (currentRumbleState) device.SetRumble(false);
            }

            rumbleStepCount += 1;

            if (rumbleStepCount >= rumbleStepPeriod)
            {
                rumbleStepCount = 0;
            }
        }

        static System.Threading.Tasks.Task Delay(int milliseconds)
        {
            var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();
            new System.Threading.Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
            return tcs.Task;
        }

        private void SetBatteryStatus(bool isLow)
        {
            if (isLow && !lowBatteryFired)
            {
                Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new Action(() =>
                        {
                            statusGradient.Color = (Color)FindResource("LowBattery");
                            if (MainWindow.Instance.trayIcon.Visibility == System.Windows.Visibility.Visible)
                            {
                                lowBatteryFired = true;
                                MainWindow.Instance.ShowBalloon
                                (
                                    "Battery Low",
                                    dName + (!dName.Equals(device.Type.ToString()) ? " (" + device.Type.ToString() + ") " : " ")
                                    + "is running low on battery life.",
                                    Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Warning,
                                    System.Media.SystemSounds.Hand
                                );
                            }
                        }
                ));
            }
            else if (!isLow && lowBatteryFired)
            {
                statusGradient = (GradientStop)FindResource("AntemBlue");
                lowBatteryFired = false;
            }
        }

        private void LoadProfile(string profilePath, Holders.Holder h)
        {
            Profile loadedProfile = null;

            if (!string.IsNullOrWhiteSpace(profilePath) && File.Exists(profilePath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Profile));

                    using (FileStream stream = File.OpenRead(profilePath))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedProfile = serializer.Deserialize(reader) as Profile;
                        reader.Close();
                        stream.Close();
                    }
                }
                catch { }
            }

            if (loadedProfile == null)
            {
                loadedProfile = UserPrefs.Instance.defaultProfile;
            }

            if (loadedProfile != null)
            {
                for (int i = 0; i < Math.Min(loadedProfile.controllerMapKeys.Count, loadedProfile.controllerMapValues.Count); i++)
                {
                    h.SetMapping(loadedProfile.controllerMapKeys[i], loadedProfile.controllerMapValues[i]);
                }
            }
        }

        private void UpdateIcon(ControllerType cType)
        {
            if (icon.Source == (ImageSource)Application.Current.Resources["ProIcon"])
            {
                return;
            }

            switch (cType)
            {
                case ControllerType.ProController:
                    icon.Source = (ImageSource)Application.Current.Resources["ProIcon"];
                    break;
                case ControllerType.ClassicControllerPro:
                    icon.Source = (ImageSource)Application.Current.Resources["CCPIcon"];
                    break;
                case ControllerType.ClassicController:
                    icon.Source = (ImageSource)Application.Current.Resources["CCIcon"];
                    break;
                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                    icon.Source = (ImageSource)Application.Current.Resources["WNIcon"];
                    break;

                default:
                    icon.Source = (ImageSource)Application.Current.Resources["WIcon"];
                    break;
            }
        }

        #region UI Events
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
            var config = new ConfigWindow(holder.Mappings, device.Type);
            config.ShowDialog();
            if (config.result)
            {
                foreach (KeyValuePair<string, string> pair in config.map)
                {
                    holder.SetMapping(pair.Key, pair.Value);
                }
            }
        }

        private void btnIdentify_Click(object sender, RoutedEventArgs e)
        {
            bool wasConnected = Connected;

            if (wasConnected || device.Connect())
            {
                if (holder != null && device.Type == ControllerType.ProController)
                {
                    holder.Flags[Inputs.Flags.RUMBLE] = true;
                    Delay(2000).ContinueWith(o => holder.Flags[Inputs.Flags.RUMBLE] = false);
                }
                else
                {
                    device.SetRumble(true);
                    Delay(2000).ContinueWith(o =>
                    {
                        device.SetRumble(false);
                        if (!wasConnected) device.Disconnect();
                    });
                }

                // light show
                device.SetLEDs(1);
                Delay(250).ContinueWith(o => device.SetLEDs(2));
                Delay(500).ContinueWith(o => device.SetLEDs(4));
                Delay(750).ContinueWith(o => device.SetLEDs(8));
                Delay(1000).ContinueWith(o => device.SetLEDs(4));
                Delay(1250).ContinueWith(o => device.SetLEDs(2));
                Delay(1500).ContinueWith(o => device.SetLEDs(1));
                if (targetXDevice != 0)
                    Delay(1750).ContinueWith(o => device.SetPlayerLED(targetXDevice));
            }
        }

        private void btnVjoy_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (btnVjoy_image == null)
                return;

            if ((bool)e.NewValue)
            {
                btnVjoy_image.Opacity = 1.0;
            }
            else
            {
                btnVjoy_image.Opacity = 0.5;
            }
        }

        private void btnXinput_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (btnXinput_image == null)
                return;

            if ((bool)e.NewValue)
            {
                btnXinput_image.Opacity = 1.0;
            }
            else
            {
                btnXinput_image.Opacity = 0.5;
            }
        }

        private void btnProperties_Click(object sender, RoutedEventArgs e)
        {
            PropWindow win = new PropWindow(properties, device.Type.ToString());
            win.ShowDialog();

            if (win.doSave)
            {
                properties = new Property(win.props);
                SetName(properties.name);
                UserPrefs.Instance.AddDevicePref(properties);
                UserPrefs.SavePrefs();
            }
        }
        #endregion
    }
}