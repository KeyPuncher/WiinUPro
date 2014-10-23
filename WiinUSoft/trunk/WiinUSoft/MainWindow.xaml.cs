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
            Refresh();
        }

        private void Refresh()
        {
            hidList = Nintroller.FindControllers();

            foreach (string hid in hidList)
            {
                Nintroller n = new Nintroller(hid);
                DeviceControl existingDevice = null;

                foreach (DeviceControl d in deviceList)
                {
                    if (d.Device.HIDPath == hid)
                    {
                        existingDevice = d;
                        break;
                    }
                }

                if (existingDevice != null)
                {
                    if (!existingDevice.Connected && !n.ConnectTest())
                    {
                        existingDevice.ConnectionState = DeviceState.None;
                    }
                    else if (!existingDevice.Connected && n.ConnectTest())
                    {
                        existingDevice.ConnectionState = DeviceState.Discovered;
                    }
                }
                else if (n.ConnectTest())
                {
                    deviceList.Add(new DeviceControl(n));
                    deviceList[deviceList.Count - 1].OnConnectStateChange += DeviceControl_OnConnectStateChange;
                    groupAvailable.Children.Add(deviceList[deviceList.Count - 1]);
                }
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

        private void btnDetatchAllXInput_Click(object sender, RoutedEventArgs e)
        {
            List<DeviceControl> detatchList = new List<DeviceControl>();
            foreach (DeviceControl d in groupXinput.Children)
            {
                detatchList.Add(d);
            }
            foreach (DeviceControl d in detatchList)
            {
                d.Detatch();
            }
        }

        private void btnIdentify_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
