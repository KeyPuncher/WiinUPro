using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Shared;
using NintrollerLib;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for DeviceStatus.xaml
    /// </summary>
    public partial class DeviceStatus : UserControl
    {
        public UserControl Control
        {
            get
            {
                if (Ninty != null)
                {
                    return Ninty;
                }
                else
                {
                    return Joy;
                }
            }
        }

        public DeviceInfo Info;
        public NintyControl Ninty;
        public JoyControl Joy;
        public Action<DeviceStatus, bool> ConnectClick;
        public Action<DeviceStatus, ControllerType> TypeUpdated;
        public Action<DeviceStatus> CloseTab;
        public Action<DeviceStatus, DevicePrefs> OnPrefsChange;
        public Action<DeviceStatus, bool[]> OnRumbleSubscriptionChange;

        public ImageSource Icon { get { return icon.Source; } }

        private int extIndex = -1;

        public bool Connected
        {
            get
            {
                if (Ninty != null)
                {
                    return Ninty.Connected;
                }

                if (Joy != null)
                {
                    return Joy.Connected;
                }

                return false;
            }
        }

        public DeviceStatus(DeviceInfo info, CommonStream stream = null)
        {
            InitializeComponent();

            connectBtn.Content = Globalization.Translate(connectBtn.Uid);
            status.Content = Globalization.Translate("Status_Not_Connected");
            nickname.Content = Globalization.Translate("Status_Unidentified");

            Info = info;

            if (info.InstanceGUID.Equals(Guid.Empty))
            {
                if (stream == null)
                {
                    Ninty = new NintyControl(Info);
                }
                else
                {
                    Ninty = new NintyControl(Info, stream);
                }

                Ninty.OnTypeChange += Ninty_OnTypeChange;
                Ninty.OnDisconnect += Ninty_OnDisconnect;
                Ninty.OnPrefsChange += Ninty_OnPrefsChange;
                Ninty.OnRumbleSubscriptionChange += Ninty_OnRumbleSubscriptionChange;

                // Use saved icon if there is one
                var prefs = AppPrefs.Instance.GetDevicePreferences(Info.DevicePath);
                if (prefs != null && !string.IsNullOrWhiteSpace(prefs.icon))
                {
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/" + prefs.icon, UriKind.Relative));
                    nickname.Content = string.IsNullOrWhiteSpace(prefs.nickname) ? info.Type.ToName() : prefs.nickname;
                }
                else
                {
                    UpdateType(info.Type);
                }
            }
            else
            {
                Joy = new JoyControl(Info);
                Joy.OnDisconnect += Ninty_OnDisconnect;
                Joy.OnPrefsChange += Ninty_OnPrefsChange;
                nickname.Content = JoyControl.ToName(Joy.Type);
                if (info.VID == "057e" && info.PID == "2006")
                {
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/switch_jcl_black.png", UriKind.Relative));
                }
                else if (info.VID == "057e" && info.PID == "2007")
                {
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/switch_jcr_black.png", UriKind.Relative));
                }
                else if (info.VID == "057e" && info.PID == "2009")
                {
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/switch_pro_black.png", UriKind.Relative));
                }
                else
                {
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/joystick_icon.png", UriKind.Relative));
                }
            }
        }

        private void Ninty_OnDisconnect()
        {
            connectBtn.IsEnabled = true;
            status.Content = Globalization.Translate("Status_Not_Connected");
            CloseTab?.Invoke(this);
        }

        private void Ninty_OnTypeChange(ControllerType type)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                UpdateType(type);
                TypeUpdated?.Invoke(this, type);
            }));
        }

        private void Ninty_OnPrefsChange(DevicePrefs prefs)
        {
            if (!string.IsNullOrWhiteSpace(prefs.nickname))
            {
                nickname.Content = prefs.nickname;
            }

            OnPrefsChange?.Invoke(this, prefs);
        }

        private void Ninty_OnRumbleSubscriptionChange(bool[] rumbleSubscriptions)
        {
            OnRumbleSubscriptionChange?.Invoke(this, rumbleSubscriptions);
        }

        public void UpdateType(ControllerType type)
        {
            // TODO: Default to unknown icon
            string img = "ProController_black_24.png";
            string deviceName = "";

            switch (type)
            {
                case ControllerType.ProController:
                    img = "ProController_black_24.png";
                    deviceName = "Pro Controller";
                    break;

                case ControllerType.Wiimote:
                    img = "wiimote_black_24.png";
                    deviceName = "Wiimote";
                    extIndex = 0;
                    break;

                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                    img = "Wiimote+Nunchuck_black_24.png";
                    deviceName = "Nunchuk";
                    extIndex = 1;
                    break;

                case ControllerType.ClassicController:
                    img = "Classic_black_24.png";
                    deviceName = "Classic Controller";
                    extIndex = 2;
                    break;

                case ControllerType.ClassicControllerPro:
                    img = "ClassicPro_black_24.png";
                    deviceName = "Classic Controller Pro";
                    extIndex = 3;
                    break;

                case ControllerType.Guitar:
                    extIndex = 4;
                    break;

                case ControllerType.TaikoDrum:
                    extIndex = 5;
                    break;

                case ControllerType.Other:
                    // TODO
                    deviceName = "GCN Adapter";
                    break;
            }

            var prefs = AppPrefs.Instance.GetDevicePreferences(Info.DevicePath);
            if (prefs != null)
            {
                if (!string.IsNullOrWhiteSpace(prefs.nickname))
                {
                    deviceName = prefs.nickname;
                }

                prefs.icon = img;
                AppPrefs.Instance.SaveDevicePrefs(prefs);

                string profileToLoad = string.Empty;

                if (extIndex > -1 && prefs.extensionProfiles.Length > extIndex && !string.IsNullOrEmpty(prefs.extensionProfiles[extIndex]))
                {
                    profileToLoad = prefs.extensionProfiles[extIndex] ?? string.Empty;
                }
                else if (!string.IsNullOrEmpty(prefs.defaultProfile))
                {
                    profileToLoad = prefs.defaultProfile ?? string.Empty;
                }

                if (AppPrefs.Instance.autoAddXInputDevices && AppPrefs.Instance.profileQueuing)
                {
                    profileToLoad = GetQueuedProfile(profileToLoad);
                }

                if (!string.IsNullOrWhiteSpace(profileToLoad))
                {
                    Ninty.LoadProfile(profileToLoad);
                }
            }

            icon.Source = new BitmapImage(new Uri("../Images/Icons/" + img, UriKind.Relative));
#if DEBUG
            if (Info.DevicePath != null && Info.DevicePath.StartsWith("Dummy"))
                nickname.Content = Info.DevicePath;
            else
#endif
                nickname.Content = deviceName;
        }

        private string GetQueuedProfile(string originalProfile)
        {
            string baseName = string.Empty;
            if (originalProfile.ToLower().EndsWith("_x1.wup"))
            {
                var i = originalProfile.ToLower().LastIndexOf("_x1.wup");
                baseName = originalProfile.Substring(0, i);
            }
            else if (originalProfile.ToLower().EndsWith(".wup"))
            {
                var i = originalProfile.ToLower().LastIndexOf(".wup");
                baseName = originalProfile.Substring(0, i);
            }

            // early out
            if (string.IsNullOrEmpty(baseName))
            {
                return originalProfile;
            }

            // Figure out how many devices are already taken
            int deviceNum = (int)ScpDirector.XInput_Device.Device_A;
            for (; deviceNum <= (int)ScpDirector.XInput_Device.Device_D; deviceNum += 1)
            {
                if (ScpDirector.Access.IsConnected((ScpDirector.XInput_Device)deviceNum))
                    continue;

                var targetProfile = $"{baseName}_x{deviceNum}.wup";
                if (System.IO.File.Exists(targetProfile))
                {
                    _ = ScpDirector.Access.ConnectDevice((ScpDirector.XInput_Device)deviceNum);
                    return targetProfile;
                }
            }

            return originalProfile;
        }

        public void AutoConnect()
        {
            if (connectBtn.IsEnabled)
            {
                connectBtn_Click(this, new RoutedEventArgs());
            }
        }

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            if (Ninty != null)
            {
                result = Ninty.Connect();
            }
            else if (Joy != null)
            {
                result = Joy.Connect();

                var prefs = AppPrefs.Instance.GetDevicePreferences(Info.DeviceID);
                if (prefs != null && !string.IsNullOrEmpty(prefs.defaultProfile))
                {
                    Joy.LoadProfile(prefs.defaultProfile);
                }
            }

            if (result)
            {
                connectBtn.IsEnabled = false;
                status.Content = Globalization.Translate("Status_Connected");
            }

            ConnectClick?.Invoke(this, result);
        }

        private void debug_Click(object sender, RoutedEventArgs e)
        {
            connectBtn_Click(sender, e);

            if (Ninty.Connected)
            {
                var debugger = new Windows.DebugDeviceWindow(Ninty._nintroller);
                debugger.Show();
            }
        }
    }

    public interface IDeviceControl
    {
        event Action OnDisconnect;
        event Action<DevicePrefs> OnPrefsChange;
        int ShiftIndex { get; }
        ShiftState CurrentShiftState { get; }
        bool Connected { get; }
        void ChangeState(ShiftState newState);
        bool Connect();
        void Disconnect();
        void AddRumble(bool state);
    }
}
