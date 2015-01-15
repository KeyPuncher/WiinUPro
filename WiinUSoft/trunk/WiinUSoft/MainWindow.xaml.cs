using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using NintrollerLib;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        private List<string> hidList;
        private List<DeviceControl> deviceList;

        public MainWindow()
        {
            hidList = new List<string>();
            deviceList = new List<DeviceControl>();

            InitializeComponent();

            Instance = this;
        }

        public void HideWindow()
        {
            if (WindowState == System.Windows.WindowState.Minimized)
            {
                trayIcon.Visibility = System.Windows.Visibility.Visible;
                Hide();
            }
        }

        public void ShowWindow()
        {
            trayIcon.Visibility = System.Windows.Visibility.Hidden;
            Show();
            WindowState = System.Windows.WindowState.Normal;
        }

        public void ShowBalloon(string title, string message, BalloonIcon icon)
        {
            //if (trayIcon.Visibility == System.Windows.Visibility.Hidden)
            //    trayIcon.Visibility = System.Windows.Visibility.Visible;
            trayIcon.ShowBalloonTip(title, message, icon);
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
                    if (!existingDevice.Connected)
                    {
                        existingDevice.RefreshState();
                    }
                }
                else if (n.ConnectTest())
                {
                    deviceList.Add(new DeviceControl(n));
                    deviceList[deviceList.Count - 1].OnConnectStateChange += DeviceControl_OnConnectStateChange;
                    deviceList[deviceList.Count - 1].RefreshState();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Refresh();
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

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            HideWindow();
        }

        private void MenuItem_Show_Click(object sender, RoutedEventArgs e)
        {
            ShowWindow();
        }

        private void MenuItem_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            foreach (DeviceControl dc in deviceList)
            {
                if (dc.ConnectionState == DeviceState.Connected_XInput
                 || dc.ConnectionState == DeviceState.Connected_VJoy)
                {
                    dc.Detatch();
                }
            }

            Close();
        }
    }

    class ShowWindowCommand : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.Instance.ShowWindow();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event System.EventHandler CanExecuteChanged;
    }
}
