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
using Shared;
using NintrollerLib;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for DeviceStatus.xaml
    /// </summary>
    public partial class DeviceStatus : UserControl
    {
        public DeviceInfo Info;
        public NintyControl Ninty;
        public Action<DeviceStatus, bool> ConnectClick;
        public Action<DeviceStatus, ControllerType> TypeUpdated;
        public Action<DeviceStatus> CloseTab;

        public ImageSource Icon { get { return icon.Source; } }

        public DeviceStatus(DeviceInfo info)
        {
            InitializeComponent();

            Info = info;
            Ninty = new NintyControl(Info);
            Ninty.OnTypeChange += Ninty_OnTypeChange;
            Ninty.OnDisconnect += Ninty_OnDisconnect;
            UpdateType(info.Type);
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
            // TODO: Default to unknown
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

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = Ninty.Connect();

            if (result)
            {
                connectBtn.IsEnabled = false;
            }

            ConnectClick?.Invoke(this, result);
        }
    }
}
