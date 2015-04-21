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

        //private void ExtensionChange(object sender, ExtensionChangeEventArgs e)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        _stackDigitalInputs.Children.Clear();

        //        if (_nintroller.Type == ControllerType.Wiimote)
        //        {
        //            // Add Digital Inputs
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "A" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "B" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "1" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "2" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Up" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Down" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Left" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Right" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Plus" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Minus" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Home" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "IR1 in view" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "IR2 in view" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "IR3 in view" });
        //            _stackDigitalInputs.Children.Add(new CheckBox() { Content = "IR4 in view" });

        //            // Add Analog Inputs
        //            for (int i = 0; i < 8; i++)
        //            {
        //                _stackAnalogInputs.Children.Add(new Label());
        //                _stackAnalogInputs.Children.Add(new Label());
        //                _stackAnalogInputs.Children.Add(new Label());
        //                _stackAnalogInputs.Children.Add(new Label());
        //                _stackAnalogInputs.Children.Add(new Label());
        //                _stackAnalogInputs.Children.Add(new Label());
        //                _stackAnalogInputs.Children.Add(new Label());
        //                _stackAnalogInputs.Children.Add(new Label());
        //            }

        //            _nintroller.StateChange += StateChange;
        //        }
        //    });
        //}

        private void ExtensionChange(object sender, NintrollerExtensionEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _stackDigitalInputs.Children.Clear();

                if (e.controllerType == ControllerType.Wiimote)
                {
                    // Add Digital Inputs
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "A" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "B" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "1" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "2" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Up" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Down" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Left" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Right" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Plus" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Minus" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "Home" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "IR1 in view" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "IR2 in view" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "IR3 in view" });
                    _stackDigitalInputs.Children.Add(new CheckBox() { Content = "IR4 in view" });

                    // Add Analog Inputs
                    for (int i = 0; i < 8; i++)
                    {
                        _stackAnalogInputs.Children.Add(new Label());
                        _stackAnalogInputs.Children.Add(new Label());
                        _stackAnalogInputs.Children.Add(new Label());
                        _stackAnalogInputs.Children.Add(new Label());
                        _stackAnalogInputs.Children.Add(new Label());
                        _stackAnalogInputs.Children.Add(new Label());
                        _stackAnalogInputs.Children.Add(new Label());
                        _stackAnalogInputs.Children.Add(new Label());
                    }

                    //_nintroller.StateChange += StateChange;
                }
            });
        }

        //private void StateChange(object sender, StateChangeEventArgs e)
        //{
        //    Dispatcher.Invoke(() =>
        //    {
        //        // Update Digital Inputs
        //        if (_stackDigitalInputs.Children.Count < 15) return;
        //        ((CheckBox)_stackDigitalInputs.Children[0]).IsChecked = e.Wiimote.A;
        //        ((CheckBox)_stackDigitalInputs.Children[1]).IsChecked = e.Wiimote.B;
        //        ((CheckBox)_stackDigitalInputs.Children[2]).IsChecked = e.Wiimote.One;
        //        ((CheckBox)_stackDigitalInputs.Children[3]).IsChecked = e.Wiimote.Two;
        //        ((CheckBox)_stackDigitalInputs.Children[4]).IsChecked = e.Wiimote.Up;
        //        ((CheckBox)_stackDigitalInputs.Children[5]).IsChecked = e.Wiimote.Down;
        //        ((CheckBox)_stackDigitalInputs.Children[6]).IsChecked = e.Wiimote.Left;
        //        ((CheckBox)_stackDigitalInputs.Children[7]).IsChecked = e.Wiimote.Right;
        //        ((CheckBox)_stackDigitalInputs.Children[8]).IsChecked = e.Wiimote.Plus;
        //        ((CheckBox)_stackDigitalInputs.Children[9]).IsChecked = e.Wiimote.Minus;
        //        ((CheckBox)_stackDigitalInputs.Children[10]).IsChecked = e.Wiimote.Home;
        //        ((CheckBox)_stackDigitalInputs.Children[11]).IsChecked = e.Wiimote.IR1.InView;
        //        ((CheckBox)_stackDigitalInputs.Children[12]).IsChecked = e.Wiimote.IR2.InView;
        //        ((CheckBox)_stackDigitalInputs.Children[13]).IsChecked = e.Wiimote.IR3.InView;
        //        ((CheckBox)_stackDigitalInputs.Children[14]).IsChecked = e.Wiimote.IR4.InView;

        //        // Update Analog Inputs
        //        if (_stackAnalogInputs.Children.Count < 4) return;
        //        ((Label)_stackAnalogInputs.Children[0]).Content = "Acc: " + e.Wiimote.Acc.ToString();
        //        ((Label)_stackAnalogInputs.Children[1]).Content = "Acc X: " + e.Wiimote.AccRaw.X.ToString();
        //        ((Label)_stackAnalogInputs.Children[2]).Content = "Acc Y: " + e.Wiimote.AccRaw.Y.ToString();
        //        ((Label)_stackAnalogInputs.Children[3]).Content = "Acc Z: " + e.Wiimote.AccRaw.Z.ToString();
        //        ((Label)_stackAnalogInputs.Children[4]).Content = string.Format("IR 1: x:{0} y:{1} size:{2}", e.Wiimote.IR1.RawX, e.Wiimote.IR1.RawY, e.Wiimote.IR1.Size);
        //        ((Label)_stackAnalogInputs.Children[5]).Content = string.Format("IR 2: x:{0} y:{1} size:{2}", e.Wiimote.IR2.RawX, e.Wiimote.IR2.RawY, e.Wiimote.IR2.Size);
        //        ((Label)_stackAnalogInputs.Children[6]).Content = string.Format("IR 3: x:{0} y:{1} size:{2}", e.Wiimote.IR3.RawX, e.Wiimote.IR3.RawY, e.Wiimote.IR3.Size);
        //        ((Label)_stackAnalogInputs.Children[7]).Content = string.Format("IR 4: x:{0} y:{1} size:{2}", e.Wiimote.IR4.RawX, e.Wiimote.IR4.RawY, e.Wiimote.IR4.Size);
        //    });
        //}

        private void StateUpdate(object sender, NintrollerStateEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Wiimote wm = new Wiimote();
                if (e.controllerType == ControllerType.Wiimote)
                {
                    wm = (Wiimote)e.state;
                }
                
                // Update Digital Inputs
                if (_stackDigitalInputs.Children.Count < 15) return;
                ((CheckBox)_stackDigitalInputs.Children[0]).IsChecked  = wm.buttons.A;
                ((CheckBox)_stackDigitalInputs.Children[1]).IsChecked  = wm.buttons.B;
                ((CheckBox)_stackDigitalInputs.Children[2]).IsChecked  = wm.buttons.One;
                ((CheckBox)_stackDigitalInputs.Children[3]).IsChecked  = wm.buttons.Two;
                ((CheckBox)_stackDigitalInputs.Children[4]).IsChecked  = wm.buttons.Up;
                ((CheckBox)_stackDigitalInputs.Children[5]).IsChecked  = wm.buttons.Down;
                ((CheckBox)_stackDigitalInputs.Children[6]).IsChecked  = wm.buttons.Left;
                ((CheckBox)_stackDigitalInputs.Children[7]).IsChecked  = wm.buttons.Right;
                ((CheckBox)_stackDigitalInputs.Children[8]).IsChecked  = wm.buttons.Plus;
                ((CheckBox)_stackDigitalInputs.Children[9]).IsChecked  = wm.buttons.Minus;
                ((CheckBox)_stackDigitalInputs.Children[10]).IsChecked = wm.buttons.Home;
                ((CheckBox)_stackDigitalInputs.Children[11]).IsChecked = wm.irSensor.point1.visible;
                ((CheckBox)_stackDigitalInputs.Children[12]).IsChecked = wm.irSensor.point2.visible;
                ((CheckBox)_stackDigitalInputs.Children[13]).IsChecked = wm.irSensor.point3.visible;
                ((CheckBox)_stackDigitalInputs.Children[14]).IsChecked = wm.irSensor.point4.visible;
                
                // Update Analog Inputs
                if (_stackAnalogInputs.Children.Count < 4) return;
                ((Label)_stackAnalogInputs.Children[0]).Content = "Acc: "   + wm.accelerometer.ToString();
                ((Label)_stackAnalogInputs.Children[1]).Content = "Acc X: " + wm.accelerometer.rawX.ToString();
                ((Label)_stackAnalogInputs.Children[2]).Content = "Acc Y: " + wm.accelerometer.rawY.ToString();
                ((Label)_stackAnalogInputs.Children[3]).Content = "Acc Z: " + wm.accelerometer.rawZ.ToString();
                ((Label)_stackAnalogInputs.Children[4]).Content = string.Format("IR 1: x:{0} y:{1} size:{2}", wm.irSensor.point1.rawX, wm.irSensor.point1.rawY, wm.irSensor.point1.size);
                ((Label)_stackAnalogInputs.Children[5]).Content = string.Format("IR 2: x:{0} y:{1} size:{2}", wm.irSensor.point2.rawX, wm.irSensor.point2.rawY, wm.irSensor.point2.size);
                ((Label)_stackAnalogInputs.Children[6]).Content = string.Format("IR 3: x:{0} y:{1} size:{2}", wm.irSensor.point3.rawX, wm.irSensor.point3.rawY, wm.irSensor.point3.size);
                ((Label)_stackAnalogInputs.Children[7]).Content = string.Format("IR 4: x:{0} y:{1} size:{2}", wm.irSensor.point4.rawX, wm.irSensor.point4.rawY, wm.irSensor.point4.size);
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
