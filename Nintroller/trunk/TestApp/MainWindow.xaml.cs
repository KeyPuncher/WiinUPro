﻿using System;
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

using NintrollerLib.New;
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
            //var controllers = Nintroller.FindControllers();
            var controllers = Nintroller.GetControllerPaths();

            DeviceList = new List<string>();

            foreach (string id in controllers)
            {
                DeviceList.Add(id);
            }

            _comboBoxDeviceList.ItemsSource = DeviceList;
        }

        private void _btnConnect_Click(object sender, RoutedEventArgs e)
        {
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
                        // Add Digital Inputs
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "C" });
                        _stackDigitalInputs.Children.Add(new CheckBox() { IsHitTestVisible = false, Content = "Z" });

                        // Add Analog Inputs
                        for (int i = 0; i < 3; i++)
                        {
                            _stackAnalogInputs.Children.Add(new Label());
                        }

                        AddWiimoteInputs();
                        #endregion
                        break;

                    case ControllerType.ClassicController:
                        #region Classic Controller Inputs
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
                        for (int i = 0; i < 6; i++)
                        {
                            _stackAnalogInputs.Children.Add(new Label());
                        }

                        AddWiimoteInputs();
                        #endregion
                        break;

                    case ControllerType.ClassicControllerPro:
                        #region Classic Controller Pro Inputs
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
                        for (int i = 0; i < 4; i++)
                        {
                            _stackAnalogInputs.Children.Add(new Label());
                        }

                        AddWiimoteInputs();
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
                            Wiimote wm = new Wiimote();
                            wm = (Wiimote)e.state; // try/catch if we must

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
                            if (_stackAnalogInputs.Children.Count < 4) return;
                            ((Label)_stackAnalogInputs.Children[0]).Content = "Acc: " + wm.accelerometer.ToString();
                            ((Label)_stackAnalogInputs.Children[1]).Content = "Acc X: " + wm.accelerometer.rawX.ToString();
                            ((Label)_stackAnalogInputs.Children[2]).Content = "Acc Y: " + wm.accelerometer.rawY.ToString();
                            ((Label)_stackAnalogInputs.Children[3]).Content = "Acc Z: " + wm.accelerometer.rawZ.ToString();
                            ((Label)_stackAnalogInputs.Children[4]).Content = "IR: " + wm.irSensor.ToString();
                            ((Label)_stackAnalogInputs.Children[4]).Content = string.Format("IR 1: x:{0} y:{1} size:{2}", wm.irSensor.point1.rawX, wm.irSensor.point1.rawY, wm.irSensor.point1.size);
                            ((Label)_stackAnalogInputs.Children[5]).Content = string.Format("IR 2: x:{0} y:{1} size:{2}", wm.irSensor.point2.rawX, wm.irSensor.point2.rawY, wm.irSensor.point2.size);
                            ((Label)_stackAnalogInputs.Children[6]).Content = string.Format("IR 3: x:{0} y:{1} size:{2}", wm.irSensor.point3.rawX, wm.irSensor.point3.rawY, wm.irSensor.point3.size);
                            ((Label)_stackAnalogInputs.Children[7]).Content = string.Format("IR 4: x:{0} y:{1} size:{2}", wm.irSensor.point4.rawX, wm.irSensor.point4.rawY, wm.irSensor.point4.size);
                            #endregion
                            break;

                        case ControllerType.Nunchuk:
                        case ControllerType.NunchukB:
                            #region Nunchuk Display
                            #endregion
                            break;

                        case ControllerType.ClassicController:
                            #region Classic Controller Display
                            #endregion
                            break;

                        case ControllerType.ClassicControllerPro:
                            #region Classic Controller Pro Display
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

        private void _btnEnableIR_Click(object sender, RoutedEventArgs e)
        {
            //_nintroller.InitIRCamera();
            //_nintroller.EnableIR();
            _nintroller.IRMode = IRCamMode.Wide;
        }
    }
}