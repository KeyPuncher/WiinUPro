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

        public ImageSource Icon { get { return icon.Source; } }

        public DeviceStatus(DeviceInfo info)
        {
            InitializeComponent();

            Info = info;

            if (info.InstanceGUID.Equals(Guid.Empty))
            {
                Ninty = new NintyControl(Info);
                Ninty.OnTypeChange += Ninty_OnTypeChange;
                Ninty.OnDisconnect += Ninty_OnDisconnect;
                UpdateType(info.Type);
            }
            else
            {
                Joy = new JoyControl(Info);
                Joy.OnDisconnect += Ninty_OnDisconnect;
                if (info.VID == "057e" && info.PID == "2006")
                {
                    nickname.Content = "Joy-Con (L)";
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/switch_jcl_black.png", UriKind.Relative));
                }
                else if (info.VID == "057e" && info.PID == "2007")
                {
                    nickname.Content = "Joy-Con (R)";
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/switch_jcr_black.png", UriKind.Relative));
                }
                else if (info.VID == "057e" && info.PID == "2009")
                {
                    nickname.Content = "Switch Pro";
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/switch_pro_black.png", UriKind.Relative));
                }
                else
                {
                    nickname.Content = "Generic Joystick";
                    icon.Source = new BitmapImage(new Uri("../Images/Icons/joystick_icon.png", UriKind.Relative));
                }
            }
        }

        private void Ninty_OnDisconnect()
        {
            connectBtn.IsEnabled = true;
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

        public void UpdateType(ControllerType type)
        {
            // TODO: Default to unknown icon
            string img = "ProController_black_24.png";

            switch (type)
            {
                case ControllerType.ProController:
                    img = "ProController_black_24.png";
                    nickname.Content = "Pro Controller";
                    break;

                case ControllerType.Wiimote:
                    img = "wiimote_black_24.png";
                    nickname.Content = "Wiimote";
                    break;

                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                    img = "Wiimote+Nunchuck_black_24.png";
                    nickname.Content = "Nunchuk";
                    break;

                case ControllerType.ClassicController:
                    img = "Classic_black_24.png";
                    nickname.Content = "Classic Controller";
                    break;

                case ControllerType.ClassicControllerPro:
                    img = "ClassicPro_black_24.png";
                    nickname.Content = "Classic Controller Pro";
                    break;
            }

            icon.Source = new BitmapImage(new Uri("../Images/Icons/" + img, UriKind.Relative));
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

                var prefs = AppPrefs.Instance.GetDevicePreferences(Info.DevicePath);
                if (prefs != null && !string.IsNullOrEmpty(prefs.defaultProfile))
                {
                    Ninty.LoadProfile(prefs.defaultProfile);
                }
            }
            else if (Joy != null)
            {
                result = Joy.Connect();
            }

            if (result)
            {
                connectBtn.IsEnabled = false;
            }

            ConnectClick?.Invoke(this, result);
        }
    }

    public interface IDeviceControl
    {
        event Action OnDisconnect;
        int ShiftIndex { get; }
        ShiftState CurrentShiftState { get; }
        void ChangeState(ShiftState newState);
        bool Connect();
        void Disconnect();
        void AddRumble(bool state);
    }
}
