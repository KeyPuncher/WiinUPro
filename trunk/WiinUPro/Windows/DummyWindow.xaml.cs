using System;
using System.Windows;
using NintrollerLib;
using Shared;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for DummyWindow.xaml
    /// </summary>
    public partial class DummyWindow : Window
    {
        public DummyDevice Device { get; protected set; }
        public ControllerType Type { get; set; }

        bool isWiimote { get { return Device.State is Wiimote; } }
        bool isPro { get { return Device.State is ProController; } }

        public DummyWindow(DummyDevice device)
        {
            InitializeComponent();
            Device = device;
        }

        public void SwitchType(ControllerType newType)
        {
            switch (newType)
            {
                case ControllerType.Wiimote:
                    Device.State = new Wiimote();
                    groupCore.Visibility = Visibility.Visible;
                    break;

                case ControllerType.ProController:
                    Device.State = new ProController();
                    groupCore.Visibility = Visibility.Visible;
                    break;
            }
        }

        private ProController ChangeProBoolean(string property)
        {
            ProController pro = (ProController)Device.State;

            switch (property)
            {
                case INPUT_NAMES.PRO_CONTROLLER.A     : pro.A      = !pro.A; break;
                case INPUT_NAMES.PRO_CONTROLLER.B     : pro.B      = !pro.B; break;
                case INPUT_NAMES.PRO_CONTROLLER.X     : pro.X      = !pro.X; break;
                case INPUT_NAMES.PRO_CONTROLLER.Y     : pro.Y      = !pro.Y; break;
                case INPUT_NAMES.PRO_CONTROLLER.L     : pro.L      = !pro.L; break;
                case INPUT_NAMES.PRO_CONTROLLER.R     : pro.R      = !pro.R; break;
                case INPUT_NAMES.PRO_CONTROLLER.ZL    : pro.ZL     = !pro.ZL; break;
                case INPUT_NAMES.PRO_CONTROLLER.ZR    : pro.ZR     = !pro.ZR; break;
                case INPUT_NAMES.PRO_CONTROLLER.UP    : pro.Up     = !pro.Up; break;
                case INPUT_NAMES.PRO_CONTROLLER.DOWN  : pro.Down   = !pro.Down; break;
                case INPUT_NAMES.PRO_CONTROLLER.LEFT  : pro.Left   = !pro.Left; break;
                case INPUT_NAMES.PRO_CONTROLLER.RIGHT : pro.Right  = !pro.Right; break;
                case INPUT_NAMES.PRO_CONTROLLER.START : pro.Start  = !pro.Start; break;
                case INPUT_NAMES.PRO_CONTROLLER.SELECT: pro.Select = !pro.Select; break;
                case INPUT_NAMES.PRO_CONTROLLER.HOME  : pro.Home   = !pro.Home; break;
                case INPUT_NAMES.PRO_CONTROLLER.LS    : pro.LStick = !pro.LStick; break;
                case INPUT_NAMES.PRO_CONTROLLER.RS    : pro.RStick = !pro.RStick; break;
            }

            return pro;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string baseBtn = (sender as FrameworkElement).Tag.ToString();

            if (isPro)
            {
                if (baseBtn == "MINUS") baseBtn = "SELECT";
                if (baseBtn == "PLUS") baseBtn = "START";

                Device.State = ChangeProBoolean("pro" + baseBtn);
            }
        }

        private void ChangeProAnalog(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isPro)
            {
                ProController pro = (ProController)Device.State;
                float value = (float)Math.Round(e.NewValue, 2);

                switch((sender as FrameworkElement).Tag.ToString())
                {
                    case "LX":
                        pro.LJoy.X = value;
                        pro.LJoy.rawX = CalculateRaw(pro.LJoy.minX, pro.LJoy.maxX, value);
                        break;

                    case "LY":
                        pro.LJoy.Y = value;
                        pro.LJoy.rawY = CalculateRaw(pro.LJoy.minY, pro.LJoy.maxY, value);
                        break;

                    case "RX":
                        pro.RJoy.X = value;
                        pro.RJoy.rawX = CalculateRaw(pro.RJoy.minX, pro.RJoy.maxX, value);
                        break;

                    case "RY":
                        pro.RJoy.Y = value;
                        pro.RJoy.rawY = CalculateRaw(pro.RJoy.minY, pro.RJoy.maxY, value);
                        break;
                }

                Device.State = pro;
            }
        }

        private short CalculateRaw(int min, int max, float value)
        {
            var raw = (max - min) * ((value + 1)/2f) + min;
            return Convert.ToInt16(Math.Round(raw));
        }
    }
}
