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
        bool isGCN { get { return Device.State is GameCubeAdapter; } }

        public DummyWindow(DummyDevice device)
        {
            InitializeComponent();
            Device = device;
            SwitchType(Device.DeviceType);
        }

        public void SwitchType(ControllerType newType)
        {
            switch (newType)
            {
                case ControllerType.Wiimote:
                    if (Device.State == null || !(Device.State is Wiimote))
                        Device.State = new Wiimote();
                    groupCore.Visibility = Visibility.Visible;
                    groupPad.Visibility = Visibility.Hidden;
                    groupPro.Visibility = Visibility.Hidden;
                    groupGPorts.Visibility = Visibility.Hidden;
                    groupGButtons.Visibility = Visibility.Hidden;
                    groupGSticks.Visibility = Visibility.Hidden;
                    break;

                case ControllerType.ProController:
                    if (Device.State == null || !(Device.State is ProController))
                        Device.State = new ProController();
                    groupCore.Visibility = Visibility.Visible;
                    groupPad.Visibility = Visibility.Visible;
                    groupPro.Visibility = Visibility.Visible;
                    groupGPorts.Visibility = Visibility.Hidden;
                    groupGButtons.Visibility = Visibility.Hidden;
                    groupGSticks.Visibility = Visibility.Hidden;
                    break;

                case ControllerType.Other:
                    if (Device.State == null || !(Device.State is GameCubeAdapter))
                        Device.State = new GameCubeAdapter();
                    groupCore.Visibility = Visibility.Hidden;
                    groupPad.Visibility = Visibility.Hidden;
                    groupPro.Visibility = Visibility.Hidden;
                    groupGPorts.Visibility = Visibility.Visible;
                    groupGButtons.Visibility = Visibility.Visible;
                    groupGSticks.Visibility = Visibility.Visible;
                    break;
            }
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
            else if (isGCN)
            {
                Device.State = ChangeGCNBoolean("gcn" + baseBtn);
            }
        }
        
        private void ChangeAnalog(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)Math.Round(e.NewValue, 2);

            if (isPro)
            {
                ProController pro = (ProController)Device.State;

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
            else if (isGCN)
            {
                GameCubeAdapter gcn = (GameCubeAdapter)Device.State;
                GameCubeController controller = GetFromSelectedPort(gcn);

                switch ((sender as FrameworkElement).Tag.ToString())
                {
                    case "LX":
                        controller.joystick.X = value;
                        controller.joystick.rawX = CalculateRaw(controller.joystick.minX, controller.joystick.maxX, value);
                        break;

                    case "LY":
                        controller.joystick.Y = value;
                        controller.joystick.rawY = CalculateRaw(controller.joystick.minY, controller.joystick.maxY, value);
                        break;

                    case "RX":
                        controller.cStick.X = value;
                        controller.cStick.rawX = CalculateRaw(controller.cStick.minX, controller.joystick.maxX, value);
                        break;

                    case "RY":
                        controller.cStick.Y = value;
                        controller.cStick.rawY = CalculateRaw(controller.cStick.minX, controller.cStick.maxX, value);
                        break;

                    case "L":
                        controller.L.full = value >= 0.9f;
                        controller.L.value = value;
                        controller.L.rawValue = (short)(value * 255);
                        break;

                    case "R":
                        controller.R.full = value >= 0.9f;
                        controller.R.value = value;
                        controller.R.rawValue = (short)(value * 255);
                        break;
                }

                Device.State = ApplyToSelectedPort(gcn, controller);
            }
        }

        private short CalculateRaw(int min, int max, float value)
        {
            var raw = (max - min) * ((value + 1)/2f) + min;
            return Convert.ToInt16(Math.Round(raw));
        }

        private ProController ChangeProBoolean(string property)
        {
            ProController pro = (ProController)Device.State;

            switch (property)
            {
                case INPUT_NAMES.PRO_CONTROLLER.A: pro.A = !pro.A; break;
                case INPUT_NAMES.PRO_CONTROLLER.B: pro.B = !pro.B; break;
                case INPUT_NAMES.PRO_CONTROLLER.X: pro.X = !pro.X; break;
                case INPUT_NAMES.PRO_CONTROLLER.Y: pro.Y = !pro.Y; break;
                case INPUT_NAMES.PRO_CONTROLLER.L: pro.L = !pro.L; break;
                case INPUT_NAMES.PRO_CONTROLLER.R: pro.R = !pro.R; break;
                case INPUT_NAMES.PRO_CONTROLLER.ZL: pro.ZL = !pro.ZL; break;
                case INPUT_NAMES.PRO_CONTROLLER.ZR: pro.ZR = !pro.ZR; break;
                case INPUT_NAMES.PRO_CONTROLLER.UP: pro.Up = !pro.Up; break;
                case INPUT_NAMES.PRO_CONTROLLER.DOWN: pro.Down = !pro.Down; break;
                case INPUT_NAMES.PRO_CONTROLLER.LEFT: pro.Left = !pro.Left; break;
                case INPUT_NAMES.PRO_CONTROLLER.RIGHT: pro.Right = !pro.Right; break;
                case INPUT_NAMES.PRO_CONTROLLER.START: pro.Start = !pro.Start; break;
                case INPUT_NAMES.PRO_CONTROLLER.SELECT: pro.Select = !pro.Select; break;
                case INPUT_NAMES.PRO_CONTROLLER.HOME: pro.Home = !pro.Home; break;
                case INPUT_NAMES.PRO_CONTROLLER.LS: pro.LStick = !pro.LStick; break;
                case INPUT_NAMES.PRO_CONTROLLER.RS: pro.RStick = !pro.RStick; break;
            }

            return pro;
        }

        private void PortChanged(object sender, RoutedEventArgs e)
        {
            if (isGCN)
            {
                GameCubeAdapter gcn = (GameCubeAdapter)Device.State;

                gcn.port1Connected = checkGPort1.IsChecked ?? false;
                gcn.port2Connected = checkGPort2.IsChecked ?? false;
                gcn.port3Connected = checkGPort3.IsChecked ?? false;
                gcn.port4Connected = checkGPort4.IsChecked ?? false;

                Device.State = gcn;
            }
        }

        private GameCubeController GetFromSelectedPort(GameCubeAdapter gcn)
        {
            switch (comboGPortSelect.SelectedIndex)
            {
                case 1: return gcn.port2;
                case 2: return gcn.port3;
                case 3: return gcn.port4;
                default: return gcn.port1;
            }
        }

        private GameCubeAdapter ApplyToSelectedPort(GameCubeAdapter gcn, GameCubeController controller)
        {
            switch (comboGPortSelect.SelectedIndex)
            {
                case 1: gcn.port2 = controller; break;
                case 2: gcn.port3 = controller; break;
                case 3: gcn.port4 = controller; break;
                default: gcn.port1 = controller; break;
            }

            return gcn;
        }

        private GameCubeAdapter ChangeGCNBoolean(string property)
        {
            GameCubeAdapter gcn = (GameCubeAdapter)Device.State;
            GameCubeController controller = GetFromSelectedPort(gcn);

            switch (property)
            {
                case INPUT_NAMES.GCN_CONTROLLER.A: controller.A = !controller.A; break;
                case INPUT_NAMES.GCN_CONTROLLER.B: controller.B = !controller.B; break;
                case INPUT_NAMES.GCN_CONTROLLER.X: controller.X = !controller.X; break;
                case INPUT_NAMES.GCN_CONTROLLER.Y: controller.Y = !controller.Y; break;
                case INPUT_NAMES.GCN_CONTROLLER.Z: controller.Z = !controller.Z; break;
                case INPUT_NAMES.GCN_CONTROLLER.UP: controller.Up = !controller.Up; break;
                case INPUT_NAMES.GCN_CONTROLLER.DOWN: controller.Down = !controller.Down; break;
                case INPUT_NAMES.GCN_CONTROLLER.LEFT: controller.Left = !controller.Left; break;
                case INPUT_NAMES.GCN_CONTROLLER.RIGHT: controller.Right = !controller.Right; break;
                case INPUT_NAMES.GCN_CONTROLLER.START: controller.Start = !controller.Start; break;
            }

            switch (comboGPortSelect.SelectedIndex)
            {
                case 1: gcn.port2 = controller; break;
                case 2: gcn.port3 = controller; break;
                case 3: gcn.port4 = controller; break;
                default: gcn.port1 = controller; break;
            }

            return ApplyToSelectedPort(gcn, controller);
        }
    }
}
