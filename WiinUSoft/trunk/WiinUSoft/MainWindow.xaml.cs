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
using System.Diagnostics;
using NintrollerLib;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> hidList;
        private List<DeviceControl> deviceList;

        public MainWindow()
        {
            hidList = new List<string>();
            deviceList = new List<DeviceControl>();

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: move out into a method that will be the refresh method
            hidList = Nintroller.FindControllers();

            foreach (string hid in hidList)
            {
                Nintroller n = new Nintroller(hid);
                deviceList.Add(new DeviceControl(n));
                deviceList[deviceList.Count - 1].OnConnectStateChange += DeviceControl_OnConnectStateChange;
                groupAvailable.Children.Add(deviceList[deviceList.Count - 1]);
            }
        }

        private void DeviceControl_OnConnectStateChange(DeviceControl sender, DeviceState oldState, DeviceState newState)
        {
            if (oldState == newState)
                return;

            switch (oldState)
            {
                case DeviceState.Discovered:
                    groupAvailable.Children.Remove(sender);
                    break;

                case DeviceState.Connected_XInput:
                    groupXinput.Children.Remove(sender);
                    break;

                case DeviceState.Connected_VJoy:
                    groupXinput.Children.Remove(sender);
                    break;
            }

            switch (newState)
            {
                case DeviceState.Discovered:
                    groupAvailable.Children.Add(sender);
                    break;

                case DeviceState.Connected_XInput:
                    groupXinput.Children.Add(sender);
                    break;

                case DeviceState.Connected_VJoy:
                    groupXinput.Children.Add(sender);
                    break;
            }
        }
    }
}
