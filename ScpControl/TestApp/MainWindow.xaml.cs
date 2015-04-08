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
            var controllers = Nintroller.FindControllers();

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
                _nintroller.StateChange -= StateChange;
                _nintroller.Disconnect();
                _nintroller = null;
                _stackDigitalInputs.Children.Clear();
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

                    _nintroller.SetPlayerLED(1);
                    _nintroller.ExtensionChange += ExtensionChange;
                }
            }
        }

        private void ExtensionChange(object sender, ExtensionChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                _stackDigitalInputs.Children.Clear();

                if (_nintroller.Type == ControllerType.Wiimote)
                {
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

                    _nintroller.StateChange += StateChange;
                }
            });
        }

        private void StateChange(object sender, StateChangeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ((CheckBox)_stackDigitalInputs.Children[0]).IsChecked = e.Wiimote.A;
                ((CheckBox)_stackDigitalInputs.Children[1]).IsChecked = e.Wiimote.B;
                ((CheckBox)_stackDigitalInputs.Children[2]).IsChecked = e.Wiimote.One;
                ((CheckBox)_stackDigitalInputs.Children[3]).IsChecked = e.Wiimote.Two;
                ((CheckBox)_stackDigitalInputs.Children[4]).IsChecked = e.Wiimote.Up;
                ((CheckBox)_stackDigitalInputs.Children[5]).IsChecked = e.Wiimote.Down;
                ((CheckBox)_stackDigitalInputs.Children[6]).IsChecked = e.Wiimote.Left;
                ((CheckBox)_stackDigitalInputs.Children[7]).IsChecked = e.Wiimote.Right;
                ((CheckBox)_stackDigitalInputs.Children[8]).IsChecked = e.Wiimote.Plus;
                ((CheckBox)_stackDigitalInputs.Children[9]).IsChecked = e.Wiimote.Minus;
                ((CheckBox)_stackDigitalInputs.Children[10]).IsChecked = e.Wiimote.Home;
            });
        }
    }
}
