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
                _nintroller.Disconnect();
                _nintroller = null;
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
                }
            }
        }
    }
}
