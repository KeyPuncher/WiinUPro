using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NintrollerLib;
using System.Xml.Serialization;
using System.IO;
using Shared.Windows;

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
        #region Members

        // private members
        private string devicePath;
        private Nintroller device;
        private DeviceState state;
        private float rumbleAmount      = 0;
        private int   rumbleStepCount   = 0;
        private int   rumbleStepPeriod  = 10;
        private float rumbleSlowMult    = 0.5f;
        
        // internally public members
        internal Holders.Holder holder;
        internal Property       properties;
        internal int            targetXDevice = 0;
        internal bool           lowBatteryFired = false;
        internal bool           identifying = false;
        internal string         dName = "";
        internal System.Threading.Timer updateTimer;

        // constance
        internal const int UPDATE_SPEED = 25;

        // events
        public event ConnectStateChange OnConnectStateChange;

        #endregion

        #region Properties

        internal Nintroller Device
        {
            get { return device; }
            set
            {
                device = value;

                if (device != null)
                {
                    device.ExtensionChange += device_ExtensionChange;
                    device.StateUpdate += device_StateChange;
                    device.LowBattery += device_LowBattery;
                }
            }
        }

        internal ControllerType DeviceType { get; private set; }

        internal string DevicePath
        {
            get { return devicePath; }
            private set { devicePath = value; }
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

        #endregion

        public DeviceControl()
        {
            InitializeComponent();
        }

        public DeviceControl(Nintroller nintroller, string path)
            : this()
        {
            Device = nintroller;
            devicePath = path;

            Device.Disconnected += device_Disconnected;
        }
        
        public void RefreshState()
        {
            if (state != DeviceState.Connected_XInput)
                ConnectionState = DeviceState.Discovered;

            UpdateIcon(device.Type);
            SetName(device.Type.ToString());

            // Load Properties
            properties = UserPrefs.Instance.GetDevicePref(devicePath);
            if (properties != null)
            {
                SetName(string.IsNullOrWhiteSpace(properties.name) ? device.Type.ToString() : properties.name);
                ApplyCalibration(properties.calPref, properties.calString ?? "");
            }
            else
            {
                properties = new Property(devicePath);
            }
        }

        public void SetName(string newName)
        {
            dName = newName;
            labelName.Content = new TextBlock() { Text = newName };
        }

        public void Detatch()
        {
            device.StopReading();
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
                    btnConfig.Visibility    = Visibility.Hidden;
                    btnDetatch.Visibility   = Visibility.Hidden;
                    break;

                case DeviceState.Discovered:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = true;
                    //btnVjoy.IsEnabled     = true;
                    btnConfig.IsEnabled     = false;
                    btnDetatch.IsEnabled    = false;
                    btnConfig.Visibility    = Visibility.Hidden;
                    btnDetatch.Visibility   = Visibility.Hidden;
                    break;

                case DeviceState.Connected_XInput:
                    btnIdentify.IsEnabled   = true;
                    btnProperties.IsEnabled = true;
                    btnXinput.IsEnabled     = false;
                    //btnVjoy.IsEnabled     = false;
                    btnConfig.IsEnabled     = true;
                    btnDetatch.IsEnabled    = true;
                    btnConfig.Visibility    = Visibility.Visible;
                    btnDetatch.Visibility   = Visibility.Visible;

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

        void device_ExtensionChange(object sender, NintrollerExtensionEventArgs e)
        {
            DeviceType = e.controllerType;

            if (holder != null)
            {
                holder.AddMapping(DeviceType);
            }

            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, 
                new Action(() => UpdateIcon(DeviceType)
            ));
        }

        void device_LowBattery(object sender, LowBatteryEventArgs e)
        {
            SetBatteryStatus(e.batteryLevel == BatteryStatus.Low || e.batteryLevel == BatteryStatus.VeryLow);
        }

        void device_StateChange(object sender, NintrollerStateEventArgs e)
        {
            // Makes the timer wait
            if (updateTimer != null) updateTimer.Change(1000, UPDATE_SPEED);

            if (holder == null)
            {
                return;
            }

//            float intensity = 0;
//            if (holder.Values.TryGetValue(Inputs.Flags.RUMBLE, out intensity))
//            {
//                rumbleAmount = (int)intensity;
                RumbleStep();
//            }

            holder.ClearAllValues();

            switch (e.controllerType)
            {
                // TODO: Motion Plus Reading (not for 1st release)
                // TODO: Balance Board Reading (not for 1st release)
                // TODO: Musical Extension readings (not for 1st release)
                case ControllerType.ProController:
                    #region Pro Controller
                    ProController pro = (ProController)e.state;

                    holder.SetValue(Inputs.ProController.A, pro.A);
                    holder.SetValue(Inputs.ProController.B, pro.B);
                    holder.SetValue(Inputs.ProController.X, pro.X);
                    holder.SetValue(Inputs.ProController.Y, pro.Y);

                    holder.SetValue(Inputs.ProController.UP, pro.Up);
                    holder.SetValue(Inputs.ProController.DOWN, pro.Down);
                    holder.SetValue(Inputs.ProController.LEFT, pro.Left);
                    holder.SetValue(Inputs.ProController.RIGHT, pro.Right);

                    holder.SetValue(Inputs.ProController.L, pro.L);
                    holder.SetValue(Inputs.ProController.R, pro.R);
                    holder.SetValue(Inputs.ProController.ZL, pro.ZL);
                    holder.SetValue(Inputs.ProController.ZR, pro.ZR);

                    holder.SetValue(Inputs.ProController.START, pro.Plus);
                    holder.SetValue(Inputs.ProController.SELECT, pro.Minus);
                    holder.SetValue(Inputs.ProController.HOME, pro.Home);
                    holder.SetValue(Inputs.ProController.LS, pro.LStick);
                    holder.SetValue(Inputs.ProController.RS, pro.RStick);

                    holder.SetValue(Inputs.ProController.LRIGHT, pro.LJoy.X > 0 ? pro.LJoy.X : 0f);
                    holder.SetValue(Inputs.ProController.LLEFT,  pro.LJoy.X < 0 ? pro.LJoy.X * -1 : 0f);
                    holder.SetValue(Inputs.ProController.LUP,    pro.LJoy.Y > 0 ? pro.LJoy.Y : 0f);
                    holder.SetValue(Inputs.ProController.LDOWN,  pro.LJoy.Y < 0 ? pro.LJoy.Y * -1 : 0f);

                    holder.SetValue(Inputs.ProController.RRIGHT, pro.RJoy.X > 0 ? pro.RJoy.X : 0f);
                    holder.SetValue(Inputs.ProController.RLEFT,  pro.RJoy.X < 0 ? pro.RJoy.X * -1 : 0f);
                    holder.SetValue(Inputs.ProController.RUP,    pro.RJoy.Y > 0 ? pro.RJoy.Y : 0f);
                    holder.SetValue(Inputs.ProController.RDOWN,  pro.RJoy.Y < 0 ? pro.RJoy.Y * -1 : 0f);
                    #endregion
                    break;

                case ControllerType.Wiimote:
                    Wiimote wm = (Wiimote)e.state;
                    SetWiimoteInputs(wm);
                    break;

                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                    #region Nunchuk
                    Nunchuk nun = (Nunchuk)e.state;

                    SetWiimoteInputs(nun.wiimote);

                    holder.SetValue(Inputs.Nunchuk.C, nun.C);
                    holder.SetValue(Inputs.Nunchuk.Z, nun.Z);

                    holder.SetValue(Inputs.Nunchuk.RIGHT, nun.joystick.X > 0 ? nun.joystick.X : 0f);
                    holder.SetValue(Inputs.Nunchuk.LEFT,  nun.joystick.X < 0 ? nun.joystick.X * -1 : 0f);
                    holder.SetValue(Inputs.Nunchuk.UP,    nun.joystick.Y > 0 ? nun.joystick.Y : 0f);
                    holder.SetValue(Inputs.Nunchuk.DOWN,  nun.joystick.Y < 0 ? nun.joystick.Y * -1 : 0f);

                    //TODO: Nunchuk Accelerometer (not for 1st release)
                    holder.SetValue(Inputs.Nunchuk.TILT_RIGHT, nun.accelerometer.X > 0 ? nun.accelerometer.X : 0f);
                    holder.SetValue(Inputs.Nunchuk.TILT_LEFT, nun.accelerometer.X < 0 ? nun.accelerometer.X * -1 : 0f);
                    holder.SetValue(Inputs.Nunchuk.TILT_UP, nun.accelerometer.Y > 0 ? nun.accelerometer.Y : 0f);
                    holder.SetValue(Inputs.Nunchuk.TILT_DOWN, nun.accelerometer.Y < 0 ? nun.accelerometer.Y * -1 : 0f);

                    holder.SetValue(Inputs.Nunchuk.ACC_SHAKE_X, nun.accelerometer.X > 1.15f);
                    holder.SetValue(Inputs.Nunchuk.ACC_SHAKE_Y, nun.accelerometer.Y > 1.15f);
                    holder.SetValue(Inputs.Nunchuk.ACC_SHAKE_Z, nun.accelerometer.Z > 1.15f);
                    #endregion
                    break;

                case ControllerType.ClassicController:
                    #region Classic Controller
                    ClassicController cc = (ClassicController)e.state;

                    SetWiimoteInputs(cc.wiimote);

                    holder.SetValue(Inputs.ClassicController.A, cc.A);
                    holder.SetValue(Inputs.ClassicController.B, cc.B);
                    holder.SetValue(Inputs.ClassicController.X, cc.X);
                    holder.SetValue(Inputs.ClassicController.Y, cc.Y);

                    holder.SetValue(Inputs.ClassicController.UP, cc.Up);
                    holder.SetValue(Inputs.ClassicController.DOWN, cc.Down);
                    holder.SetValue(Inputs.ClassicController.LEFT, cc.Left);
                    holder.SetValue(Inputs.ClassicController.RIGHT, cc.Right);

                    holder.SetValue(Inputs.ClassicController.L, cc.L.value > 0);
                    holder.SetValue(Inputs.ClassicController.R, cc.R.value > 0);
                    holder.SetValue(Inputs.ClassicController.ZL, cc.ZL);
                    holder.SetValue(Inputs.ClassicController.ZR, cc.ZR);

                    holder.SetValue(Inputs.ClassicController.START, cc.Start);
                    holder.SetValue(Inputs.ClassicController.SELECT, cc.Select);
                    holder.SetValue(Inputs.ClassicController.HOME, cc.Home);

                    holder.SetValue(Inputs.ClassicController.LFULL, cc.LFull);
                    holder.SetValue(Inputs.ClassicController.RFULL, cc.RFull);
                    holder.SetValue(Inputs.ClassicController.LT, cc.L.value > 0.1f ? cc.L.value : 0f);
                    holder.SetValue(Inputs.ClassicController.RT, cc.R.value > 0.1f ? cc.R.value : 0f);

                    holder.SetValue(Inputs.ClassicController.LRIGHT, cc.LJoy.X > 0 ? cc.LJoy.X : 0f);
                    holder.SetValue(Inputs.ClassicController.LLEFT, cc.LJoy.X < 0 ? cc.LJoy.X * -1 : 0f);
                    holder.SetValue(Inputs.ClassicController.LUP, cc.LJoy.Y > 0 ? cc.LJoy.Y : 0f);
                    holder.SetValue(Inputs.ClassicController.LDOWN, cc.LJoy.Y < 0 ? cc.LJoy.Y * -1 : 0f);

                    holder.SetValue(Inputs.ClassicController.RRIGHT, cc.RJoy.X > 0 ? cc.RJoy.X : 0f);
                    holder.SetValue(Inputs.ClassicController.RLEFT, cc.RJoy.X < 0 ? cc.RJoy.X * -1 : 0f);
                    holder.SetValue(Inputs.ClassicController.RUP, cc.RJoy.Y > 0 ? cc.RJoy.Y : 0f);
                    holder.SetValue(Inputs.ClassicController.RDOWN, cc.RJoy.Y < 0 ? cc.RJoy.Y * -1 : 0f);
                    #endregion
                    break;

                case ControllerType.ClassicControllerPro:
                    #region Classic Controller Pro
                    ClassicControllerPro ccp = (ClassicControllerPro)e.state;

                    SetWiimoteInputs(ccp.wiimote);

                    holder.SetValue(Inputs.ClassicControllerPro.A, ccp.A);
                    holder.SetValue(Inputs.ClassicControllerPro.B, ccp.B);
                    holder.SetValue(Inputs.ClassicControllerPro.X, ccp.X);
                    holder.SetValue(Inputs.ClassicControllerPro.Y, ccp.Y);

                    holder.SetValue(Inputs.ClassicControllerPro.UP, ccp.Up);
                    holder.SetValue(Inputs.ClassicControllerPro.DOWN, ccp.Down);
                    holder.SetValue(Inputs.ClassicControllerPro.LEFT, ccp.Left);
                    holder.SetValue(Inputs.ClassicControllerPro.RIGHT, ccp.Right);

                    holder.SetValue(Inputs.ClassicControllerPro.L, ccp.L);
                    holder.SetValue(Inputs.ClassicControllerPro.R, ccp.R);
                    holder.SetValue(Inputs.ClassicControllerPro.ZL, ccp.ZL);
                    holder.SetValue(Inputs.ClassicControllerPro.ZR, ccp.ZR);

                    holder.SetValue(Inputs.ClassicControllerPro.START, ccp.Start);
                    holder.SetValue(Inputs.ClassicControllerPro.SELECT, ccp.Select);
                    holder.SetValue(Inputs.ClassicControllerPro.HOME, ccp.Home);

                    holder.SetValue(Inputs.ClassicControllerPro.LRIGHT, ccp.LJoy.X > 0 ? ccp.LJoy.X : 0f);
                    holder.SetValue(Inputs.ClassicControllerPro.LLEFT, ccp.LJoy.X < 0 ? ccp.LJoy.X * -1 : 0f);
                    holder.SetValue(Inputs.ClassicControllerPro.LUP, ccp.LJoy.Y > 0 ? ccp.LJoy.Y : 0f);
                    holder.SetValue(Inputs.ClassicControllerPro.LDOWN, ccp.LJoy.Y < 0 ? ccp.LJoy.Y * -1 : 0f);

                    holder.SetValue(Inputs.ClassicControllerPro.RRIGHT, ccp.RJoy.X > 0 ? ccp.RJoy.X : 0f);
                    holder.SetValue(Inputs.ClassicControllerPro.RLEFT, ccp.RJoy.X < 0 ? ccp.RJoy.X * -1 : 0f);
                    holder.SetValue(Inputs.ClassicControllerPro.RUP, ccp.RJoy.Y > 0 ? ccp.RJoy.Y : 0f);
                    holder.SetValue(Inputs.ClassicControllerPro.RDOWN, ccp.RJoy.Y < 0 ? ccp.RJoy.Y * -1 : 0f);
                    #endregion
                    break;
            }
            
            holder.Update();

            // Resumes the timer in case this method is not called withing 100ms
            if (updateTimer != null) updateTimer.Change(100, UPDATE_SPEED);
        }

        private void device_Disconnected(object sender, DisconnectedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,
                new Action(() =>
                {
                    Detatch();
                    ConnectionState = device.Connected ? DeviceState.Discovered : DeviceState.None;

                    // Show balloon if minimized, otherwise a popup dialog
                    if (MainWindow.Instance.trayIcon.Visibility == Visibility.Visible)
                    {
                        MainWindow.Instance.ShowBalloon("Connection Lost", "Failed to communicate with controller. It may no longer be connected.", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Failed to communicate with controller." + Environment.NewLine + "It may no longer be connected.", "Connection Lost", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            ));
        }

        private void SetWiimoteInputs(Wiimote wm)
        {
            holder.SetValue(Inputs.Wiimote.A, wm.buttons.A);
            holder.SetValue(Inputs.Wiimote.B, wm.buttons.B);
            holder.SetValue(Inputs.Wiimote.ONE, wm.buttons.One);
            holder.SetValue(Inputs.Wiimote.TWO, wm.buttons.Two);

            holder.SetValue(Inputs.Wiimote.UP, wm.buttons.Up);
            holder.SetValue(Inputs.Wiimote.DOWN, wm.buttons.Down);
            holder.SetValue(Inputs.Wiimote.LEFT, wm.buttons.Left);
            holder.SetValue(Inputs.Wiimote.RIGHT, wm.buttons.Right);

            holder.SetValue(Inputs.Wiimote.MINUS, wm.buttons.Minus);
            holder.SetValue(Inputs.Wiimote.PLUS, wm.buttons.Plus);
            holder.SetValue(Inputs.Wiimote.HOME, wm.buttons.Home);

            //TODO: Wiimote Accelerometer and IR sensor Reading (not for 1st release)
            holder.SetValue(Inputs.Wiimote.TILT_RIGHT, wm.accelerometer.X > 0 ? wm.accelerometer.X : 0);
            holder.SetValue(Inputs.Wiimote.TILT_LEFT, wm.accelerometer.X < 0 ? wm.accelerometer.X : 0);
            holder.SetValue(Inputs.Wiimote.TILT_UP, wm.accelerometer.Y > 0 ? wm.accelerometer.Y : 0);
            holder.SetValue(Inputs.Wiimote.TILT_DOWN, wm.accelerometer.Y < 0 ? wm.accelerometer.Y : 0);

            holder.SetValue(Inputs.Wiimote.ACC_SHAKE_X, wm.accelerometer.X > 1.15);
            holder.SetValue(Inputs.Wiimote.ACC_SHAKE_Y, wm.accelerometer.Y > 1.15);
            holder.SetValue(Inputs.Wiimote.ACC_SHAKE_Z, wm.accelerometer.Z > 1.15);
        }

        private void HolderUpdate(object holderState)
        {
            if (holder == null) return;

            holder.Update();

//            float intensity = 0;
//            if (holder.Values.TryGetValue(Inputs.Flags.RUMBLE, out intensity))
//            {
//                rumbleAmount = (int)intensity;
                RumbleStep();
//            }

            SetBatteryStatus(device.BatteryLevel == BatteryStatus.Low);
        }

        void RumbleStep()
        {
            if (identifying) return;

            bool currentRumbleState = device.RumbleEnabled;

            if (!properties.useRumble)
            {
                if (currentRumbleState) device.RumbleEnabled = false;
                return;
            }

            rumbleAmount = holder.RumbleAmount;

            float dutyCycle = 0;
            float modifier = properties.rumbleIntensity * 0.5f;

            if (rumbleAmount < 256)
            {
                dutyCycle = rumbleSlowMult * (float)rumbleAmount / 256f;
            }
            else
            {
                dutyCycle = (float)rumbleAmount / 65535f;
            }

            int stopStep = (int)Math.Round(modifier * dutyCycle * rumbleStepPeriod);

            if (rumbleStepCount < stopStep)
            {
                if (!currentRumbleState) device.RumbleEnabled = true;
            }
            else
            {
                if (currentRumbleState) device.RumbleEnabled = false;
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
                            if (MainWindow.Instance.trayIcon.Visibility == Visibility.Visible)
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

        private void ApplyCalibration(Property.CalibrationPreference calPref, string calString)
        {
            // Load calibration settings
            switch (calPref)
            {
                case Property.CalibrationPreference.Defalut:
                    device.SetCalibration(Calibrations.CalibrationPreset.Default);
                    break;

                case Property.CalibrationPreference.More:
                    device.SetCalibration(Calibrations.CalibrationPreset.Modest);
                    break;

                case Property.CalibrationPreference.Extra:
                    device.SetCalibration(Calibrations.CalibrationPreset.Extra);
                    break;

                case Property.CalibrationPreference.Minimal:
                    device.SetCalibration(Calibrations.CalibrationPreset.Minimum);
                    break;

                case Property.CalibrationPreference.Raw:
                    device.SetCalibration(Calibrations.CalibrationPreset.None);
                    break;

                case Property.CalibrationPreference.Custom:
                    CalibrationStorage calStor = new CalibrationStorage(calString);
                    device.SetCalibration(calStor.ProCalibration);
                    device.SetCalibration(calStor.NunchukCalibration);
                    device.SetCalibration(calStor.ClassicCalibration);
                    device.SetCalibration(calStor.ClassicProCalibration);
                    device.SetCalibration(calStor.WiimoteCalibration);
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
            if ((device.DataStream as WinBtStream).OpenConnection() && device.DataStream.CanRead)
            {
                device.BeginReading();
                device.GetStatus();

                int tmp = 0;
                if (int.TryParse(((MenuItem)sender).Name.Replace("XOption", ""), out tmp))
                {
                    targetXDevice = tmp;
                    ConnectionState = DeviceState.Connected_XInput;
                }
            }
        }

        private void typeOption_Click(object sender, RoutedEventArgs e)
        {
            switch(icon.ContextMenu.Items.IndexOf(sender))
            {
                case 0:
                    device.ForceControllerType(ControllerType.Unknown);
                    break;

                case 1:
                    device.ForceControllerType(ControllerType.ProController);
                    break;

                case 2:
                    device.ForceControllerType(ControllerType.Wiimote);
                    break;

                case 3:
                    device.ForceControllerType(ControllerType.Nunchuk);
                    break;

                case 4:
                    device.ForceControllerType(ControllerType.ClassicController);
                    break;

                case 5:
                    device.ForceControllerType(ControllerType.ClassicControllerPro);
                    break;

                default:
                    break;
            }

            RefreshState();
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

            if (wasConnected || device.DataStream.CanRead)
            {
                if (!wasConnected)
                    device.BeginReading();

                identifying = true;
                device.RumbleEnabled = true;
                Delay(2000).ContinueWith(o =>
                {
                    identifying = false;
                    device.RumbleEnabled = false;
                    if (!wasConnected) device.StopReading();
                });

                // light show
                device.SetPlayerLED(1);
                Delay(250).ContinueWith(o => device.SetPlayerLED(2));
                Delay(500).ContinueWith(o => device.SetPlayerLED(3));
                Delay(750).ContinueWith(o => device.SetPlayerLED(4));
                Delay(1000).ContinueWith(o => device.SetPlayerLED(3));
                Delay(1250).ContinueWith(o => device.SetPlayerLED(2));
                Delay(1500).ContinueWith(o => device.SetPlayerLED(1));
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

            if (win.customCalibrate)
            {
                CalibrateWindow cb = new CalibrateWindow(device);
                cb.ShowDialog();

                if (cb.doSave)
                {
                    win.props.calString = cb.Calibration.ToString();
                    win.ShowDialog();
                }
                else
                {
                    win.Close();
                }
            }

            if (win.doSave)
            {
                ApplyCalibration(win.props.calPref, win.props.calString);
                properties = new Property(win.props);
                SetName(properties.name);
                UserPrefs.Instance.AddDevicePref(properties);
                UserPrefs.SavePrefs();
            }
        }
        #endregion
    }
}