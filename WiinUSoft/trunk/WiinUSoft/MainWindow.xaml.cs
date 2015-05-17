using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using NintrollerLib.New;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using System.Media;

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
            //Nintroller.UseModestConfigs = true;
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

        public void ShowBalloon(string title, string message, BalloonIcon icon, SystemSound sound = null)
        {
            //if (trayIcon.Visibility == System.Windows.Visibility.Hidden)
            //    trayIcon.Visibility = System.Windows.Visibility.Visible;
            trayIcon.ShowBalloonTip(title, message, icon);

            if (sound != null)
            {
                sound.Play();
            }
        }

        private void Refresh()
        {
            //hidList = Nintroller.FindControllers();
            hidList = Nintroller.GetControllerPaths();
            List<KeyValuePair<int, DeviceControl>> connectSeq = new List<KeyValuePair<int, DeviceControl>>();

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
                        if (existingDevice.properties.autoConnect && existingDevice.ConnectionState == DeviceState.Discovered)
                        {
                            connectSeq.Add(new KeyValuePair<int, DeviceControl>(existingDevice.properties.autoNum, existingDevice));
                        }
                    }
                }
                else if (n.ConnectTest())
                {
                    deviceList.Add(new DeviceControl(n));
                    deviceList[deviceList.Count - 1].OnConnectStateChange += DeviceControl_OnConnectStateChange;
                    deviceList[deviceList.Count - 1].RefreshState();
                    if (deviceList[deviceList.Count - 1].properties.autoConnect)
                    {
                        connectSeq.Add(new KeyValuePair<int, DeviceControl>(deviceList[deviceList.Count - 1].properties.autoNum, deviceList[deviceList.Count - 1]));
                    }
                }
            }

            // Auto connect in preferred order
            for (int i = 1; i < connectSeq.Count; i++)
            {
                if (connectSeq[i].Key < connectSeq[i - 1].Key)
                {
                    var tmp = connectSeq[i];
                    connectSeq[i] = connectSeq[i - 1];
                    connectSeq[i - 1] = tmp;
                    i--;
                }
            }

            int target = 0;
            while(!Holders.XInputHolder.availabe[target] && target < 4)
            {
                target++;
            }

            foreach(KeyValuePair<int, DeviceControl> d in connectSeq)
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();
                new System.Threading.Timer(_ => tcs.SetResult(null)).Change(1000, -1);
                tcs.Task.Wait();

                if (Holders.XInputHolder.availabe[target] && target < 4 && d.Value.Device.Connect())
                {
                    d.Value.targetXDevice = target + 1;
                    d.Value.ConnectionState = DeviceState.Connected_XInput;
                    d.Value.Device.BeginReading();
                    d.Value.Device.GetStatus();
                    target++;
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
