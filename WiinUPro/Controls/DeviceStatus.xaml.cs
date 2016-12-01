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

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for DeviceStatus.xaml
    /// </summary>
    public partial class DeviceStatus : UserControl
    {
        public DeviceInfo Info;

        public DeviceStatus(DeviceInfo info)
        {
            InitializeComponent();

            Info = info;
            UpdateType(info.Type);
        }

        public void UpdateType(NintrollerLib.ControllerType type)
        {
            // TODO: Default to unknown
            string img = "ProController_black_24.png";

            switch (type)
            {
                case NintrollerLib.ControllerType.ProController:
                    img = "ProController_white_24.png";
                    nickname.Content = "Pro Controller";
                    break;

                case NintrollerLib.ControllerType.Wiimote:
                    img = "wiimote_black_24.png";
                    nickname.Content = "Wiimote";
                    break;

                case NintrollerLib.ControllerType.Nunchuk:
                case NintrollerLib.ControllerType.NunchukB:
                    img = "Wiimote+Nunchuck_black_24.png";
                    nickname.Content = "Nunchuk";
                    break;

                case NintrollerLib.ControllerType.ClassicController:
                    img = "Classic_black_24.png";
                    nickname.Content = "Classic Controller";
                    break;

                case NintrollerLib.ControllerType.ClassicControllerPro:
                    img = "ClassicPro_black_24.png";
                    nickname.Content = "Classic Controller Pro";
                    break;
            }

            icon.Source = new BitmapImage(new Uri("../Images/Icons/" + img, UriKind.Relative));
        }
    }
}
