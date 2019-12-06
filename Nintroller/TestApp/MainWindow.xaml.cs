using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> DeviceList { get; set; }

        private Nintroller _nintroller;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void _btnFind_Click(object sender, RoutedEventArgs e)
        {
            /* This code is all obsolete
             * The Nintroller library is no longer responsible for finding controllers
            //var controllers = Nintroller.FindControllers();
            var controllers = Nintroller.GetControllerPaths();

            DeviceList = new List<string>();

            foreach (string id in controllers)
            {
                DeviceList.Add(id);
            }

            _comboBoxDeviceList.ItemsSource = DeviceList;
            */
        }

        private void _btnConnect_Click(object sender, RoutedEventArgs e)
        {
            /* This Code is obsolete
            if (_nintroller != null)
            {
                _nintroller.ExtensionChange -= ExtensionChange;
                //_nintroller.StateChange -= StateChange;
                _nintroller.StateUpdate -= StateUpdate;
                _nintroller.Disconnect();
                _nintroller = null;
                _stackDigitalInputs.Children.Clear();
                _stackAnalogInputs.Children.Clear();
                _btnConnect.Content = "Connect";
            }
            else if (_comboBoxDeviceList.SelectedItem != null)
            {
                _nintroller = new Nintroller((string)_comboBoxDeviceList.SelectedItem);

                bool success = _nintroller.Connect();

                if (!success)
                {
                    Debug.WriteLine("Failed to connect");
                    _nintroller = null;
                }
                else
                {
                    _btnConnect.Content = "Disconnect";

                    _nintroller.ExtensionChange += ExtensionChange;
                    _nintroller.StateUpdate += StateUpdate;
                    _nintroller.BeginReading();
                    _nintroller.GetStatus();
                    _nintroller.SetPlayerLED(1);
                }
            }
            */
        }

        private void ExtensionChange(object sender, NintrollerExtensionEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _stackDigitalInputs.Children.Clear();

                switch(e.controllerType)
                {
                    case ControllerType.ProController:
                        #region Pro Controller Inputs
                        // Add Digital Inputs
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "A" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "B" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "X" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Y" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Up" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Down" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Left" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Right" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "L" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "R" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "ZL" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "ZR" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Plus" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Minus" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Home" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "LS" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "RS" });

                        // Add Analog Inputs
                        for (int i = 0; i < 6; i++)
                        {
                            _stackAnalogInputs.Children.Add(new Label());
                        }
                        #endregion
                        break;

                    case ControllerType.Wiimote:
                        #region Wiimote Inputs
                        AddWiimoteInputs();
                        #endregion
                        break;

                    case ControllerType.Nunchuk:
                    case ControllerType.NunchukB:
                        #region Nunchuk Inputs

                        AddWiimoteInputs();

                        // Add Digital Inputs
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "C" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Z" });

                        // Add Analog Inputs
                        for (int i = 0; i < 7; i++)
                        {
                            _stackAnalogInputs.Children.Add(new Label());
                        }

                        #endregion
                        break;

                    case ControllerType.ClassicController:
                        #region Classic Controller Inputs

                        AddWiimoteInputs();

                        // Add Digital Inputs
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "A" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "B" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "X" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Y" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Up" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Down" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Left" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Right" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "L" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "R" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "L-Full" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "R-Full" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "ZL" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "ZR" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Plus" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Minus" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Home" });

                        // Add Analog Inputs
                        for (int i = 0; i < 8; i++)
                        {
                            _stackAnalogInputs.Children.Add(new Label());
                        }
                        #endregion
                        break;

                    case ControllerType.ClassicControllerPro:
                        #region Classic Controller Pro Inputs

                        AddWiimoteInputs();

                        // Add Digital Inputs
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "A" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "B" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "X" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Y" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Up" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Down" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Left" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Right" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "L" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "R" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "ZL" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "ZR" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Plus" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Minus" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Home" });

                        // Add Analog Inputs
                        for (int i = 0; i < 6; i++)
                        {
                            _stackAnalogInputs.Children.Add(new Label());
                        }

                        #endregion
                        break;

                    case ControllerType.BalanceBoard:
                        #region Balance Board Inputs
                        // Add Analog Inputs
                        for (int i = 0; i < 4; i++)
                        {
                            _stackAnalogInputs.Children.Add(new Label());
                        }
                        #endregion
                        break;
                }

                Console.WriteLine(_stackDigitalInputs.Children.Count);
            });
        }

        private void AddWiimoteInputs()
        {
            // Digital
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "A" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "B" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "1" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "2" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Up" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Down" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Left" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Right" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Plus" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Minus" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Home" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "IR1 in view" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "IR2 in view" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "IR3 in view" });
            _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "IR4 in view" });

            // Analog (Acc & IR)
            for (int i = 0; i < 8; i++)
            {
                _stackAnalogInputs.Children.Add(new Label());
            }
        }

        private void StateUpdate(object sender, NintrollerStateEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    switch (e.controllerType)
                    {
                        case ControllerType.ProController:
                            #region Pro Controller Display
                            ProController pro = new ProController();
                            pro = (ProController)e.state;

                            if (_stackDigitalInputs.Children.Count < 17) return;
                            ((CheckBox)_stackDigitalInputs.Children[0]).IsChecked = pro.A;
                            ((CheckBox)_stackDigitalInputs.Children[1]).IsChecked = pro.B;
                            ((CheckBox)_stackDigitalInputs.Children[2]).IsChecked = pro.X;
                            ((CheckBox)_stackDigitalInputs.Children[3]).IsChecked = pro.Y;
                            ((CheckBox)_stackDigitalInputs.Children[4]).IsChecked = pro.Up;
                            ((CheckBox)_stackDigitalInputs.Children[5]).IsChecked = pro.Down;
                            ((CheckBox)_stackDigitalInputs.Children[6]).IsChecked = pro.Left;
                            ((CheckBox)_stackDigitalInputs.Children[7]).IsChecked = pro.Right;
                            ((CheckBox)_stackDigitalInputs.Children[8]).IsChecked = pro.L;
                            ((CheckBox)_stackDigitalInputs.Children[9]).IsChecked = pro.R;
                            ((CheckBox)_stackDigitalInputs.Children[10]).IsChecked = pro.ZL;
                            ((CheckBox)_stackDigitalInputs.Children[11]).IsChecked = pro.ZR;
                            ((CheckBox)_stackDigitalInputs.Children[12]).IsChecked = pro.Plus;
                            ((CheckBox)_stackDigitalInputs.Children[13]).IsChecked = pro.Minus;
                            ((CheckBox)_stackDigitalInputs.Children[14]).IsChecked = pro.Home;
                            ((CheckBox)_stackDigitalInputs.Children[15]).IsChecked = pro.LStick;
                            ((CheckBox)_stackDigitalInputs.Children[16]).IsChecked = pro.RStick;

                            if (_stackAnalogInputs.Children.Count < 4) return;
                            ((Label)_stackAnalogInputs.Children[0]).Content = pro.LJoy.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[1]).Content = pro.LJoy.rawY.ToString();
                            ((Label)_stackAnalogInputs.Children[2]).Content = pro.LJoy.ToString();
                            ((Label)_stackAnalogInputs.Children[3]).Content = pro.RJoy.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[4]).Content = pro.RJoy.rawY.ToString();
                            ((Label)_stackAnalogInputs.Children[5]).Content = pro.RJoy.ToString();
                            #endregion
                            break;

                        case ControllerType.Wiimote:
                            #region Wiimote Display

                            //Wiimote wm = new Wiimote();
                            //wm = (Wiimote)e.state; // try/catch if we must
                            UpdateWiimoteInputs((Wiimote)e.state);

                            #endregion
                            break;

                        case ControllerType.Nunchuk:
                        case ControllerType.NunchukB:
                            #region Nunchuk Display

                            var nun = ((Nunchuk)e.state);
                            UpdateWiimoteInputs(nun.wiimote);

                            // Update Digital Inputs
                            if (_stackDigitalInputs.Children.Count < 17) return;
                            ((CheckBox)_stackDigitalInputs.Children[15]).IsChecked = nun.C;
                            ((CheckBox)_stackDigitalInputs.Children[16]).IsChecked = nun.Z;

                            if (_stackAnalogInputs.Children.Count < 15) return;
                            ((Label)_stackAnalogInputs.Children[8]).Content = "Left Joy: " + nun.joystick.ToString();
                            ((Label)_stackAnalogInputs.Children[9]).Content = "LJoy X: " + nun.joystick.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[10]).Content = "LJoy Y: " + nun.joystick.rawY.ToString();
                            ((Label)_stackAnalogInputs.Children[11]).Content = "Acc: " + nun.accelerometer.ToString();
                            ((Label)_stackAnalogInputs.Children[12]).Content = "Acc X: " + nun.accelerometer.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[13]).Content = "Acc Y: " + nun.accelerometer.rawY.ToString();
                            ((Label)_stackAnalogInputs.Children[14]).Content = "Acc Z: " + nun.accelerometer.rawZ.ToString();

                            #endregion
                            break;

                        case ControllerType.ClassicController:
                            #region Classic Controller Display

                            var cc = ((ClassicController)e.state);
                            UpdateWiimoteInputs(cc.wiimote);

                            // Update Digital Inputs
                            if (_stackDigitalInputs.Children.Count < 32) return;
                            ((CheckBox)_stackDigitalInputs.Children[15]).IsChecked = cc.A;
                            ((CheckBox)_stackDigitalInputs.Children[16]).IsChecked = cc.B;
                            ((CheckBox)_stackDigitalInputs.Children[17]).IsChecked = cc.X;
                            ((CheckBox)_stackDigitalInputs.Children[18]).IsChecked = cc.Y;
                            ((CheckBox)_stackDigitalInputs.Children[19]).IsChecked = cc.Up;
                            ((CheckBox)_stackDigitalInputs.Children[20]).IsChecked = cc.Down;
                            ((CheckBox)_stackDigitalInputs.Children[21]).IsChecked = cc.Left;
                            ((CheckBox)_stackDigitalInputs.Children[22]).IsChecked = cc.Right;
                            ((CheckBox)_stackDigitalInputs.Children[23]).IsChecked = cc.L.value > 0;
                            ((CheckBox)_stackDigitalInputs.Children[24]).IsChecked = cc.R.value > 0;
                            ((CheckBox)_stackDigitalInputs.Children[25]).IsChecked = cc.LFull;// cc.L.full;
                            ((CheckBox)_stackDigitalInputs.Children[26]).IsChecked = cc.RFull;// cc.R.full;
                            ((CheckBox)_stackDigitalInputs.Children[27]).IsChecked = cc.ZL;
                            ((CheckBox)_stackDigitalInputs.Children[28]).IsChecked = cc.ZR;
                            ((CheckBox)_stackDigitalInputs.Children[29]).IsChecked = cc.Plus;
                            ((CheckBox)_stackDigitalInputs.Children[30]).IsChecked = cc.Minus;
                            ((CheckBox)_stackDigitalInputs.Children[31]).IsChecked = cc.Home;

                            if (_stackAnalogInputs.Children.Count < 16) return;
                            ((Label)_stackAnalogInputs.Children[8]).Content = "Left Joy: " + cc.LJoy.ToString();
                            ((Label)_stackAnalogInputs.Children[9]).Content = "LJoy X: " + cc.LJoy.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[10]).Content = "LJoy Y: " + cc.LJoy.rawY.ToString();
                            ((Label)_stackAnalogInputs.Children[11]).Content = "Right Joy: " + cc.RJoy.ToString();
                            ((Label)_stackAnalogInputs.Children[12]).Content = "RJoy X: " + cc.RJoy.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[13]).Content = "RJoy Y: " + cc.RJoy.rawY.ToString();
                            ((Label)_stackAnalogInputs.Children[14]).Content = "LTrigger: " + cc.L.rawValue.ToString();
                            ((Label)_stackAnalogInputs.Children[15]).Content = "RTrigger: " + cc.R.rawValue.ToString();

                            #endregion
                            break;

                        case ControllerType.ClassicControllerPro:
                            #region Classic Controller Pro Display

                            var ccp = ((ClassicControllerPro)e.state);
                            UpdateWiimoteInputs(ccp.wiimote);

                            // Update Digital Inputs
                            if (_stackDigitalInputs.Children.Count < 30) return;
                            ((CheckBox)_stackDigitalInputs.Children[15]).IsChecked = ccp.A;
                            ((CheckBox)_stackDigitalInputs.Children[16]).IsChecked = ccp.B;
                            ((CheckBox)_stackDigitalInputs.Children[17]).IsChecked = ccp.X;
                            ((CheckBox)_stackDigitalInputs.Children[18]).IsChecked = ccp.Y;
                            ((CheckBox)_stackDigitalInputs.Children[19]).IsChecked = ccp.Up;
                            ((CheckBox)_stackDigitalInputs.Children[20]).IsChecked = ccp.Down;
                            ((CheckBox)_stackDigitalInputs.Children[21]).IsChecked = ccp.Left;
                            ((CheckBox)_stackDigitalInputs.Children[22]).IsChecked = ccp.Right;
                            ((CheckBox)_stackDigitalInputs.Children[23]).IsChecked = ccp.L;
                            ((CheckBox)_stackDigitalInputs.Children[24]).IsChecked = ccp.R;
                            ((CheckBox)_stackDigitalInputs.Children[25]).IsChecked = ccp.ZL;
                            ((CheckBox)_stackDigitalInputs.Children[26]).IsChecked = ccp.ZR;
                            ((CheckBox)_stackDigitalInputs.Children[27]).IsChecked = ccp.Plus;
                            ((CheckBox)_stackDigitalInputs.Children[28]).IsChecked = ccp.Minus;
                            ((CheckBox)_stackDigitalInputs.Children[29]).IsChecked = ccp.Home;

                            if (_stackAnalogInputs.Children.Count < 14) return;
                            ((Label)_stackAnalogInputs.Children[8]).Content = "Left Joy: " + ccp.LJoy.ToString();
                            ((Label)_stackAnalogInputs.Children[9]).Content = "LJoy X: " + ccp.LJoy.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[10]).Content = "LJoy Y: " + ccp.LJoy.rawY.ToString();
                            ((Label)_stackAnalogInputs.Children[11]).Content = "Right Joy: " + ccp.RJoy.ToString();
                            ((Label)_stackAnalogInputs.Children[12]).Content = "RJoy X: " + ccp.RJoy.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[13]).Content = "RJoy Y: " + ccp.RJoy.rawY.ToString();

                            #endregion
                            break;

                        case ControllerType.BalanceBoard:
                            #region Balance Board Display
                            #endregion
                            break;
                    }
                }
                catch(Exception ex)
                {

                }
            });
        }

        private void UpdateWiimoteInputs(Wiimote wm)
        {
            // Update Digital Inputs
            if (_stackDigitalInputs.Children.Count < 15) return;
            ((CheckBox)_stackDigitalInputs.Children[0]).IsChecked = wm.buttons.A;
            ((CheckBox)_stackDigitalInputs.Children[1]).IsChecked = wm.buttons.B;
            ((CheckBox)_stackDigitalInputs.Children[2]).IsChecked = wm.buttons.One;
            ((CheckBox)_stackDigitalInputs.Children[3]).IsChecked = wm.buttons.Two;
            ((CheckBox)_stackDigitalInputs.Children[4]).IsChecked = wm.buttons.Up;
            ((CheckBox)_stackDigitalInputs.Children[5]).IsChecked = wm.buttons.Down;
            ((CheckBox)_stackDigitalInputs.Children[6]).IsChecked = wm.buttons.Left;
            ((CheckBox)_stackDigitalInputs.Children[7]).IsChecked = wm.buttons.Right;
            ((CheckBox)_stackDigitalInputs.Children[8]).IsChecked = wm.buttons.Plus;
            ((CheckBox)_stackDigitalInputs.Children[9]).IsChecked = wm.buttons.Minus;
            ((CheckBox)_stackDigitalInputs.Children[10]).IsChecked = wm.buttons.Home;
            ((CheckBox)_stackDigitalInputs.Children[11]).IsChecked = wm.irSensor.point1.visible;
            ((CheckBox)_stackDigitalInputs.Children[12]).IsChecked = wm.irSensor.point2.visible;
            ((CheckBox)_stackDigitalInputs.Children[13]).IsChecked = wm.irSensor.point3.visible;
            ((CheckBox)_stackDigitalInputs.Children[14]).IsChecked = wm.irSensor.point4.visible;

            // Update Analog Inputs
            if (_stackAnalogInputs.Children.Count < 8) return;
            ((Label)_stackAnalogInputs.Children[0]).Content = "Acc: " + wm.accelerometer.ToString();
            ((Label)_stackAnalogInputs.Children[1]).Content = "Acc X: " + wm.accelerometer.rawX.ToString();
            ((Label)_stackAnalogInputs.Children[2]).Content = "Acc Y: " + wm.accelerometer.rawY.ToString();
            ((Label)_stackAnalogInputs.Children[3]).Content = "Acc Z: " + wm.accelerometer.rawZ.ToString();
            ((Label)_stackAnalogInputs.Children[4]).Content = "IR: " + wm.irSensor.ToString();
            ((Label)_stackAnalogInputs.Children[4]).Content = string.Format("IR 1: x:{0} y:{1} size:{2}", wm.irSensor.point1.rawX, wm.irSensor.point1.rawY, wm.irSensor.point1.size);
            ((Label)_stackAnalogInputs.Children[5]).Content = string.Format("IR 2: x:{0} y:{1} size:{2}", wm.irSensor.point2.rawX, wm.irSensor.point2.rawY, wm.irSensor.point2.size);
            ((Label)_stackAnalogInputs.Children[6]).Content = string.Format("IR 3: x:{0} y:{1} size:{2}", wm.irSensor.point3.rawX, wm.irSensor.point3.rawY, wm.irSensor.point3.size);
            ((Label)_stackAnalogInputs.Children[7]).Content = string.Format("IR 4: x:{0} y:{1} size:{2}", wm.irSensor.point4.rawX, wm.irSensor.point4.rawY, wm.irSensor.point4.size);
        }

        private void _btnEnableIR_Click(object sender, RoutedEventArgs e)
        {
            //_nintroller.InitIRCamera();
            //_nintroller.EnableIR();
            _nintroller.IRMode = IRCamMode.Wide;
        }

        private void typeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_nintroller == null) return;

            switch (typeBox.SelectedIndex)
            {
                case 0:
                    _nintroller.ForceControllerType(ControllerType.Unknown);
                    break;

                case 1:
                    _nintroller.ForceControllerType(ControllerType.ProController);
                    break;

                case 2:
                    _nintroller.ForceControllerType(ControllerType.Wiimote);
                    break;

                case 3:
                    _nintroller.ForceControllerType(ControllerType.Nunchuk);
                    break;

                case 4:
                    _nintroller.ForceControllerType(ControllerType.ClassicController);
                    break;

                case 5:
                    _nintroller.ForceControllerType(ControllerType.ClassicControllerPro);
                    break;

                default:
                    break;
            }
        }
    }
}
