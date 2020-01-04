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
        bool isNunchuk { get { return Device.State is Nunchuk; } }
        bool isClassic { get { return Device.State is ClassicController; } }
        bool isClassicPro { get { return Device.State is ClassicControllerPro; } }
        bool isPro { get { return Device.State is ProController; } }
        bool isGuitar { get { return Device.State is Guitar; } }
        bool isTaiko { get { return Device.State is TaikoDrum; } }
        bool isGCN { get { return Device.State is GameCubeAdapter; } }

        public DummyWindow(DummyDevice device)
        {
            InitializeComponent();
            Device = device;
            SwitchType(Device.DeviceType);
        }

        public void SwitchType(ControllerType newType)
        {
            Title = newType.ToString();

            switch (newType)
            {
                case ControllerType.Wiimote:
                    if (Device.State == null || !(Device.State is Wiimote))
                        Device.State = new Wiimote();
                    groupCore.Visibility = Visibility.Visible;
                    wiimoteGrid.Visibility = Visibility.Visible;
                    groupNun.Visibility = Visibility.Hidden;
                    groupPad.Visibility = Visibility.Hidden;
                    groupSticks.Visibility = Visibility.Hidden;
                    groupTriggers.Visibility = Visibility.Hidden;
                    groupGuitar.Visibility = Visibility.Hidden;
                    groupTaiko.Visibility = Visibility.Hidden;
                    gcnGrid.Visibility = Visibility.Hidden;
                    break;

                case ControllerType.Nunchuk:
                    if (Device.State == null || !isNunchuk)
                        Device.State = new Nunchuk();
                    groupCore.Visibility = Visibility.Visible;
                    wiimoteGrid.Visibility = Visibility.Visible;
                    groupNun.Visibility = Visibility.Visible;
                    groupPad.Visibility = Visibility.Hidden;
                    groupSticks.Visibility = Visibility.Hidden;
                    groupTriggers.Visibility = Visibility.Hidden;
                    groupGuitar.Visibility = Visibility.Hidden;
                    groupTaiko.Visibility = Visibility.Hidden;
                    gcnGrid.Visibility = Visibility.Hidden;
                    break;

                case ControllerType.ClassicController:
                    if (Device.State == null || !isClassic)
                        Device.State = new ClassicController();
                    groupCore.Visibility = Visibility.Visible;
                    wiimoteGrid.Visibility = Visibility.Visible;
                    groupNun.Visibility = Visibility.Hidden;
                    groupPad.Visibility = Visibility.Visible;
                    groupSticks.Visibility = Visibility.Visible;
                    groupTriggers.Visibility = Visibility.Visible;
                    groupGuitar.Visibility = Visibility.Hidden;
                    groupTaiko.Visibility = Visibility.Hidden;
                    gcnGrid.Visibility = Visibility.Hidden;
                    break;

                case ControllerType.ClassicControllerPro:
                    if (Device.State == null || !isClassicPro)
                        Device.State = new ClassicControllerPro();
                    groupCore.Visibility = Visibility.Visible;
                    wiimoteGrid.Visibility = Visibility.Visible;
                    groupNun.Visibility = Visibility.Hidden;
                    groupPad.Visibility = Visibility.Visible;
                    groupSticks.Visibility = Visibility.Visible;
                    groupTriggers.Visibility = Visibility.Hidden;
                    groupGuitar.Visibility = Visibility.Hidden;
                    groupTaiko.Visibility = Visibility.Hidden;
                    gcnGrid.Visibility = Visibility.Hidden;
                    break;

                case ControllerType.ProController:
                    if (Device.State == null || !isPro)
                        Device.State = new ProController();
                    groupCore.Visibility = Visibility.Visible;
                    wiimoteGrid.Visibility = Visibility.Hidden;
                    groupNun.Visibility = Visibility.Hidden;
                    groupPad.Visibility = Visibility.Visible;
                    groupSticks.Visibility = Visibility.Visible;
                    groupTriggers.Visibility = Visibility.Hidden;
                    groupGuitar.Visibility = Visibility.Hidden;
                    groupTaiko.Visibility = Visibility.Hidden;
                    gcnGrid.Visibility = Visibility.Hidden;
                    break;

                case ControllerType.Guitar:
                    if (Device.State == null || !isGuitar)
                        Device.State = new Guitar();
                    groupCore.Visibility = Visibility.Visible;
                    wiimoteGrid.Visibility = Visibility.Visible;
                    groupNun.Visibility = Visibility.Hidden;
                    groupPad.Visibility = Visibility.Hidden;
                    groupSticks.Visibility = Visibility.Hidden;
                    groupTriggers.Visibility = Visibility.Hidden;
                    groupGuitar.Visibility = Visibility.Visible;
                    groupTaiko.Visibility = Visibility.Hidden;
                    gcnGrid.Visibility = Visibility.Hidden;
                    break;

                case ControllerType.TaikoDrum:
                    if (Device.State == null || !isTaiko)
                        Device.State = new TaikoDrum();
                    groupCore.Visibility = Visibility.Visible;
                    wiimoteGrid.Visibility = Visibility.Visible;
                    groupNun.Visibility = Visibility.Hidden;
                    groupPad.Visibility = Visibility.Hidden;
                    groupSticks.Visibility = Visibility.Hidden;
                    groupTriggers.Visibility = Visibility.Hidden;
                    groupGuitar.Visibility = Visibility.Hidden;
                    groupTaiko.Visibility = Visibility.Visible;
                    gcnGrid.Visibility = Visibility.Hidden;
                    break;


                case ControllerType.Other:
                    if (Device.State == null || !isGCN)
                        Device.State = new GameCubeAdapter();
                    groupCore.Visibility = Visibility.Hidden;
                    wiimoteGrid.Visibility = Visibility.Hidden;
                    groupNun.Visibility = Visibility.Hidden;
                    groupPad.Visibility = Visibility.Hidden;
                    groupSticks.Visibility = Visibility.Hidden;
                    groupTriggers.Visibility = Visibility.Hidden;
                    groupGuitar.Visibility = Visibility.Hidden;
                    groupTaiko.Visibility = Visibility.Hidden;
                    gcnGrid.Visibility = Visibility.Visible;
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
            else if (isWiimote)
            {
                Device.State = ChangeWiiBoolean("w" + baseBtn, (Wiimote)Device.State);
            }
            else if (isNunchuk)
            {
                Device.State = ChangeNunBoolean("n" + baseBtn);
            }
            else if (isClassic)
            {
                Device.State = ChangeCcBoolean("cc" + baseBtn);
            }
            else if (isClassicPro)
            {
                if (baseBtn == "MINUS") baseBtn = "SELECT";
                if (baseBtn == "PLUS") baseBtn = "START";

                Device.State = ChangeCcpBoolean("ccp" + baseBtn);
            }
            else if (isGuitar)
            {
                Device.State = ChangeGuitarBoolean("gut" + baseBtn);
            }
            else if (isTaiko)
            {
                Device.State = ChangeTaikoBoolean("tak" + baseBtn);
            }
            else if (isGCN)
            {
                Device.State = ChangeGCNBoolean("gcn" + baseBtn);
            }
        }

        private void button_wii_Click(object sender, RoutedEventArgs e)
        {
            string baseBtn = (sender as FrameworkElement).Tag.ToString();

            if (isPro)
            {
                if (baseBtn == "MINUS") baseBtn = "SELECT";
                if (baseBtn == "PLUS") baseBtn = "START";

                Device.State = ChangeProBoolean("pro" + baseBtn);
            }
            else if (isWiimote)
            {
                Device.State = ChangeWiiBoolean("w" + baseBtn, (Wiimote)Device.State);
            }
            else if (Device.State is IWiimoteExtension)
            {
                ((IWiimoteExtension)Device.State).wiimote = ChangeWiiBoolean("w" + baseBtn, ((IWiimoteExtension)Device.State).wiimote);
            }
        }
        
        private void ChangeAnalog(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)Math.Round(e.NewValue, 2);
            string analogInput = (sender as FrameworkElement).Tag.ToString();

            if (isPro)
            {
                Device.State = ChangeProAnalog("pro" + analogInput, value);
            }
            else if (isNunchuk)
            {
                Device.State = ChangeNunAnalog("n" + analogInput, value);
            }
            else if (isClassic)
            {
                Device.State = ChangeCcAnalog("cc" + analogInput, value);
            }
            else if (isClassicPro)
            {
                Device.State = ChangeCcpAnalog("ccp" + analogInput, value);
            }
            else if (isGuitar)
            {
                Device.State = ChangeGuitarAnalog("gut" + analogInput, value);
            }
            else if (isGCN)
            {
                GameCubeAdapter gcn = (GameCubeAdapter)Device.State;
                GameCubeController controller = GetFromSelectedPort(gcn);

                if (analogInput == "LX") analogInput = "JoyX";
                if (analogInput == "LY") analogInput = "JoyY";
                if (analogInput == "RX") analogInput = "CX";
                if (analogInput == "RY") analogInput = "CY";

                Device.State = ChangeGCNAnalog("gcn" + analogInput, value);
            }
        }

        private void ChangeWiiAnalog(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float value = (float)Math.Round(e.NewValue, 2);
            string analogInput = (sender as FrameworkElement).Tag.ToString();

            if (isWiimote)
            {
                Device.State = ChangeWiiAnalog("w" + analogInput, value, (Wiimote)Device.State);
            }
            else if (Device.State is IWiimoteExtension)
            {
                ((IWiimoteExtension)Device.State).wiimote = ChangeWiiAnalog("w" + analogInput, value, ((IWiimoteExtension)Device.State).wiimote);
            }
        }

        #region Analog Modification

        private short CalculateRaw(int min, int max, float value)
        {
            var raw = (max - min) * ((value + 1)/2f) + min;
            return Convert.ToInt16(Math.Round(raw));
        }

        private ProController ChangeProAnalog(string property, float value)
        {
            ProController pro = (ProController)Device.State;

            switch (property)
            {
                case INPUT_NAMES.PRO_CONTROLLER.LX:
                    pro.LJoy.X = value;
                    pro.LJoy.rawX = CalculateRaw(pro.LJoy.minX, pro.LJoy.maxX, value);
                    break;

                case INPUT_NAMES.PRO_CONTROLLER.LY:
                    pro.LJoy.Y = value;
                    pro.LJoy.rawY = CalculateRaw(pro.LJoy.minY, pro.LJoy.maxY, value);
                    break;

                case INPUT_NAMES.PRO_CONTROLLER.RX:
                    pro.RJoy.X = value;
                    pro.RJoy.rawX = CalculateRaw(pro.RJoy.minX, pro.RJoy.maxX, value);
                    break;

                case INPUT_NAMES.PRO_CONTROLLER.RY:
                    pro.RJoy.Y = value;
                    pro.RJoy.rawY = CalculateRaw(pro.RJoy.minY, pro.RJoy.maxY, value);
                    break;
            }

            return pro;
        }

        private Nunchuk ChangeNunAnalog(string property, float value)
        {
            Nunchuk nun = (Nunchuk)Device.State;

            switch (property)
            {
                case INPUT_NAMES.NUNCHUK.JOY_X:
                    nun.joystick.X = value;
                    nun.joystick.rawX = CalculateRaw(nun.joystick.minX, nun.joystick.maxX, value);
                    break;

                case INPUT_NAMES.NUNCHUK.JOY_Y:
                    nun.joystick.Y = value;
                    nun.joystick.rawY = CalculateRaw(nun.joystick.minY, nun.joystick.maxY, value);
                    break;
            }

            return nun;
        }

        private ClassicController ChangeCcAnalog(string property, float value)
        {
            ClassicController cc = (ClassicController)Device.State;

            switch (property)
            {
                case INPUT_NAMES.CLASSIC_CONTROLLER.LX:
                    cc.LJoy.X = value;
                    cc.LJoy.rawX = CalculateRaw(cc.LJoy.minX, cc.LJoy.maxX, value);
                    break;

                case INPUT_NAMES.CLASSIC_CONTROLLER.LY:
                    cc.LJoy.Y = value;
                    cc.LJoy.rawY = CalculateRaw(cc.LJoy.minY, cc.LJoy.maxY, value);
                    break;

                case INPUT_NAMES.CLASSIC_CONTROLLER.RX:
                    cc.RJoy.X = value;
                    cc.RJoy.rawX = CalculateRaw(cc.RJoy.minX, cc.RJoy.maxX, value);
                    break;

                case INPUT_NAMES.CLASSIC_CONTROLLER.RY:
                    cc.RJoy.Y = value;
                    cc.RJoy.rawY = CalculateRaw(cc.RJoy.minY, cc.RJoy.maxY, value);
                    break;

                case INPUT_NAMES.CLASSIC_CONTROLLER.LT:
                    cc.L.value = value;
                    cc.L.rawValue = (short)((cc.L.max - cc.L.min) * value);
                    break;

                case INPUT_NAMES.CLASSIC_CONTROLLER.RT:
                    cc.R.value = value;
                    cc.R.rawValue = (short)((cc.R.max - cc.R.min) * value);
                    break;
            }

            return cc;
        }

        private ClassicControllerPro ChangeCcpAnalog(string property, float value)
        {
            ClassicControllerPro ccp = (ClassicControllerPro)Device.State;

            switch (property)
            {
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LX:
                    ccp.LJoy.X = value;
                    ccp.LJoy.rawX = CalculateRaw(ccp.LJoy.minX, ccp.LJoy.maxX, value);
                    break;

                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LY:
                    ccp.LJoy.Y = value;
                    ccp.LJoy.rawY = CalculateRaw(ccp.LJoy.minY, ccp.LJoy.maxY, value);
                    break;

                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RX:
                    ccp.RJoy.X = value;
                    ccp.RJoy.rawX = CalculateRaw(ccp.RJoy.minX, ccp.RJoy.maxX, value);
                    break;

                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RY:
                    ccp.RJoy.Y = value;
                    ccp.RJoy.rawY = CalculateRaw(ccp.RJoy.minY, ccp.RJoy.maxY, value);
                    break;
            }

            return ccp;
        }

        private Guitar ChangeGuitarAnalog(string property, float value)
        {
            Guitar gut = (Guitar)Device.State;

            switch (property)
            {
                case INPUT_NAMES.GUITAR.JOY_X:
                    gut.joystick.X = value;
                    gut.joystick.rawX = CalculateRaw(gut.joystick.minX, gut.joystick.maxX, value);
                    break;

                case INPUT_NAMES.GUITAR.JOY_Y:
                    gut.joystick.Y = value;
                    gut.joystick.rawY = CalculateRaw(gut.joystick.minY, gut.joystick.maxY, value);
                    break;

                case INPUT_NAMES.GUITAR.WHAMMY_BAR:
                    gut.whammyBar.value = value;
                    gut.whammyBar.rawValue = (short)((gut.whammyBar.max - gut.whammyBar.min) * value);
                    break;
            }

            return gut;
        }

        private Wiimote ChangeWiiAnalog(string property, float value, Wiimote wiimote)
        {
            switch(property)
            {
                case INPUT_NAMES.WIIMOTE.ACC_X:
                    wiimote.accelerometer.X = value;
                    wiimote.accelerometer.rawX = CalculateRaw(wiimote.accelerometer.minX, wiimote.accelerometer.maxX, value);
                    break;

                case INPUT_NAMES.WIIMOTE.ACC_Y:
                    wiimote.accelerometer.Y = value;
                    wiimote.accelerometer.rawY = CalculateRaw(wiimote.accelerometer.minY, wiimote.accelerometer.maxY, value);
                    break;

                case INPUT_NAMES.WIIMOTE.ACC_Z:
                    wiimote.accelerometer.Z = value;
                    wiimote.accelerometer.rawZ = CalculateRaw(wiimote.accelerometer.minZ, wiimote.accelerometer.maxZ, value);
                    break;
            }

            return wiimote;
        }

        #endregion

        #region Button Modification

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

        private Wiimote ChangeWiiBoolean(string property, Wiimote wiimote)
        {
            switch (property)
            {
                case INPUT_NAMES.WIIMOTE.A: wiimote.buttons.A = !wiimote.buttons.A; break;
                case INPUT_NAMES.WIIMOTE.B: wiimote.buttons.B = !wiimote.buttons.B; break;
                case INPUT_NAMES.WIIMOTE.UP: wiimote.buttons.Up = !wiimote.buttons.Up; break;
                case INPUT_NAMES.WIIMOTE.DOWN: wiimote.buttons.Down = !wiimote.buttons.Down; break;
                case INPUT_NAMES.WIIMOTE.LEFT: wiimote.buttons.Left = !wiimote.buttons.Left; break;
                case INPUT_NAMES.WIIMOTE.RIGHT: wiimote.buttons.Right = !wiimote.buttons.Right; break;
                case INPUT_NAMES.WIIMOTE.MINUS: wiimote.buttons.Minus = !wiimote.buttons.Minus; break;
                case INPUT_NAMES.WIIMOTE.PLUS: wiimote.buttons.Plus = !wiimote.buttons.Plus; break;
                case INPUT_NAMES.WIIMOTE.HOME: wiimote.buttons.Home = !wiimote.buttons.Home; break;
                case INPUT_NAMES.WIIMOTE.ONE: wiimote.buttons.One = !wiimote.buttons.One; break;
                case INPUT_NAMES.WIIMOTE.TWO: wiimote.buttons.Two = !wiimote.buttons.Two; break;
            }

            return wiimote;
        }

        private Nunchuk ChangeNunBoolean(string property)
        {
            Nunchuk nun = (Nunchuk)Device.State;

            switch (property)
            {
                case INPUT_NAMES.NUNCHUK.C: nun.C = !nun.C; break;
                case INPUT_NAMES.NUNCHUK.Z: nun.Z = !nun.Z; break;
            }

            return nun;
        }

        private ClassicController ChangeCcBoolean(string property)
        {
            ClassicController cc = (ClassicController)Device.State;

            switch (property)
            {
                case INPUT_NAMES.CLASSIC_CONTROLLER.A: cc.A = !cc.A; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.B: cc.B = !cc.B; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.X: cc.X = !cc.X; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.Y: cc.Y = !cc.Y; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.ZL: cc.ZL = !cc.ZL; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.ZR: cc.ZR = !cc.ZR; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.UP: cc.Up = !cc.Up; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.DOWN: cc.Down = !cc.Down; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.LEFT: cc.Left = !cc.Left; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.RIGHT: cc.Right = !cc.Right; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.START: cc.Start = !cc.Start; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.SELECT: cc.Minus = !cc.Minus; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.HOME: cc.Home = !cc.Home; break;
            }

            return cc;
        }

        private ClassicControllerPro ChangeCcpBoolean(string property)
        {
            ClassicControllerPro ccp = (ClassicControllerPro)Device.State;

            switch (property)
            {
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.A: ccp.A = !ccp.A; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.B: ccp.B = !ccp.B; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.X: ccp.X = !ccp.X; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.Y: ccp.Y = !ccp.Y; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.L: ccp.L = !ccp.L; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.R: ccp.R = !ccp.R; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.ZL: ccp.ZL = !ccp.ZL; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.ZR: ccp.ZR = !ccp.ZR; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.UP: ccp.Up = !ccp.Up; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.DOWN: ccp.Down = !ccp.Down; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LEFT: ccp.Left = !ccp.Left; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RIGHT: ccp.Right = !ccp.Right; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.START: ccp.Start = !ccp.Start; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.SELECT: ccp.Minus = !ccp.Minus; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.HOME: ccp.Home = !ccp.Home; break;
            }

            return ccp;
        }

        private Guitar ChangeGuitarBoolean(string property)
        {
            Guitar gut = (Guitar)Device.State;

            switch (property)
            {
                case INPUT_NAMES.GUITAR.GREEN: gut.Green = !gut.Green; break;
                case INPUT_NAMES.GUITAR.RED: gut.Red = !gut.Red; break;
                case INPUT_NAMES.GUITAR.YELLOW: gut.Yellow = !gut.Yellow; break;
                case INPUT_NAMES.GUITAR.BLUE: gut.Blue = !gut.Blue; break;
                case INPUT_NAMES.GUITAR.ORANGE: gut.Orange = !gut.Orange; break;
                case INPUT_NAMES.GUITAR.STRUM_UP: gut.StrumUp = !gut.StrumUp; break;
                case INPUT_NAMES.GUITAR.STRUM_DOWN: gut.StrumDown = !gut.StrumDown; break;
                case INPUT_NAMES.GUITAR.PLUS: gut.Plus = !gut.Plus; break;
                case INPUT_NAMES.GUITAR.MINUS: gut.Minus = !gut.Minus; break;
                case INPUT_NAMES.GUITAR.TOUCH_1: gut.T1 = !gut.T1; break;
                case INPUT_NAMES.GUITAR.TOUCH_2: gut.T2 = !gut.T2; break;
                case INPUT_NAMES.GUITAR.TOUCH_3: gut.T3 = !gut.T3; break;
                case INPUT_NAMES.GUITAR.TOUCH_4: gut.T4 = !gut.T4; break;
                case INPUT_NAMES.GUITAR.TOUCH_5: gut.T5 = !gut.T5; break;
            }

            return gut;
        }

        private TaikoDrum ChangeTaikoBoolean(string property)
        {
            TaikoDrum tak = (TaikoDrum)Device.State;

            switch (property)
            {
                case INPUT_NAMES.TAIKO_DRUM.CENTER_LEFT: tak.centerLeft = !tak.centerLeft; break;
                case INPUT_NAMES.TAIKO_DRUM.CENTER_RIGHT: tak.centerRight = !tak.centerRight; break;
                case INPUT_NAMES.TAIKO_DRUM.RIM_LEFT: tak.rimLeft = !tak.rimLeft; break;
                case INPUT_NAMES.TAIKO_DRUM.RIM_RIGHT: tak.rimRight = !tak.rimRight; break;
            }

            return tak;
        }

        #endregion

        #region GCN Modification

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

        private GameCubeAdapter ChangeGCNAnalog(string property, float value)
        {
            GameCubeAdapter gcn = (GameCubeAdapter)Device.State;
            GameCubeController controller = GetFromSelectedPort(gcn);

            switch (property)
            {
                case INPUT_NAMES.GCN_CONTROLLER.JOY_X:
                    controller.joystick.X = value;
                    controller.joystick.rawX = CalculateRaw(controller.joystick.minX, controller.joystick.maxX, value);
                    break;

                case INPUT_NAMES.GCN_CONTROLLER.JOY_Y:
                    controller.joystick.Y = value;
                    controller.joystick.rawY = CalculateRaw(controller.joystick.minY, controller.joystick.maxY, value);
                    break;

                case INPUT_NAMES.GCN_CONTROLLER.C_X:
                    controller.cStick.X = value;
                    controller.cStick.rawX = CalculateRaw(controller.cStick.minX, controller.joystick.maxX, value);
                    break;

                case INPUT_NAMES.GCN_CONTROLLER.C_Y:
                    controller.cStick.Y = value;
                    controller.cStick.rawY = CalculateRaw(controller.cStick.minX, controller.cStick.maxX, value);
                    break;

                case INPUT_NAMES.GCN_CONTROLLER.L:
                case INPUT_NAMES.GCN_CONTROLLER.LT:
                    controller.L.full = value >= 0.9f;
                    controller.L.value = value;
                    controller.L.rawValue = (short)(value * 255);
                    break;

                case INPUT_NAMES.GCN_CONTROLLER.R:
                case INPUT_NAMES.GCN_CONTROLLER.RT:
                    controller.R.full = value >= 0.9f;
                    controller.R.value = value;
                    controller.R.rawValue = (short)(value * 255);
                    break;
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

        #endregion

        private void WiimoteExtensionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string typeName = (comboExtSelect.SelectedValue as System.Windows.Controls.ComboBoxItem)?.Content as string;
            if (Device != null && !string.IsNullOrEmpty(typeName))
            {
                typeName = typeName.Replace(" ", "");
                ControllerType type = ControllerType.Unknown;
                if (Enum.TryParse<ControllerType>(typeName, out type))
                {
                    Device.ChangeExtension(type);
                    SwitchType(type);
                }
                else if (typeName == "None")
                {
                    Device.ChangeExtension(ControllerType.Wiimote);
                    SwitchType(ControllerType.Wiimote);
                }
            }
        }
    }
}
