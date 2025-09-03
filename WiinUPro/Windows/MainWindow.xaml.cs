﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Shared.Windows;
using Microsoft.Win32;
using LibUsbDotNet.Main;
using Shared;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SharpDX.DirectInput.DirectInput DInput;

        public static object CurrentTab
        {
            get
            {
                return _instance.tabControl.SelectedContent;
            }
        }

        public static MainWindow Instance => _instance;

        private static MainWindow _instance;

        public bool WindowHidden { get; private set; } = false;
        
        List<DeviceStatus> _availableDevices;
        DateTime _lastRefreshTime;

        public MainWindow()
        {
            _instance = this;
            _availableDevices = new List<DeviceStatus>();
            InitializeComponent();

            WinBtStream.OverrideSharingMode = true;
            WinBtStream.OverridenFileShare = System.IO.FileShare.ReadWrite;
        }
        
        public void Refresh()
        {
            // Prevent this method from being spammed
            if (DateTime.Now.Subtract(_lastRefreshTime).TotalSeconds < 1) return;
            _lastRefreshTime = DateTime.Now;

            // Bluetooth Devices
            var devices = WinBtStream.GetPaths();

            // Direct input Devices
            DInput = new SharpDX.DirectInput.DirectInput();
            var joys = DInput.GetDevices(SharpDX.DirectInput.DeviceClass.GameControl, SharpDX.DirectInput.DeviceEnumerationFlags.AllDevices);

            foreach (var j in joys)
            {
                string pid = j.ProductGuid.ToString().Substring(0, 4);
                string vid = j.ProductGuid.ToString().Substring(4, 4);

                // devices to ignore
                if (vid == "057e" && (pid == "0330" || pid == "0306" || pid == "0337"))
                {
                    continue;
                }
                else if (vid == "1234" && pid == "bead")
                {
                    continue;
                }

                devices.Add(new Shared.DeviceInfo()
                {
                    InstanceGUID = j.InstanceGuid,
                    PID = pid,
                    VID = vid
                });
            }

            // GCN Adapter
            WinUsbStream gcnStream = new WinUsbStream(new UsbDeviceFinder(0x057E, 0x0337));
            Shared.DeviceInfo gcnDevice = null;
            if (gcnStream.DeviceFound())
            {
                gcnDevice = new Shared.DeviceInfo()
                {
                    VID = "057E",
                    PID = "0337",
                    Type = NintrollerLib.ControllerType.Other
                };

                devices.Add(gcnDevice);
            }
#if DEBUG
            // Test GCN Device
            else
            {
                devices.Add(new Shared.DeviceInfo() { DevicePath = "Dummy GCN", VID = "057E", PID = "0337", Type = NintrollerLib.ControllerType.Other });
            }

            // Test Device
            devices.Add(new Shared.DeviceInfo() { DevicePath = "Dummy", Type = NintrollerLib.ControllerType.ProController });
            devices.Add(new Shared.DeviceInfo() { DevicePath = "Dummy Wiimote", Type = NintrollerLib.ControllerType.Wiimote });
#endif

            foreach (var info in devices)
            {
                // Check if we are already showing this one
                DeviceStatus existing = _availableDevices.Find((d) => d.Info.SameDevice(info.DeviceID));

                // If not add it
                if (existing == null)
                {
                    var status = new DeviceStatus(info, info == gcnDevice ? gcnStream : null);
                    status.ConnectClick = DoConnect;
                    status.TypeUpdated = (s, t) =>
                    {
                        var p = AppPrefs.Instance.GetDevicePreferences(s.Info.DevicePath);
                        string title = "";

                        if (p != null && !string.IsNullOrWhiteSpace(p.nickname))
                        {
                            title = p.nickname;
                        }
                        else
                        {
                            title = t.ToString();
                        }

                        foreach (var tab in tabControl.Items)
                        {
                            if (tab is TabItem && (tab as TabItem).Content == s.Control)
                            {
                                ChangeIcon(tab as TabItem, t);
                                ChangeTitle(tab as TabItem, title);
                            }
                        }
                    };
                    status.CloseTab = (s) =>
                    {
                        // Find associated tab, skip first as it is home
                        for (int i = 1; i < tabControl.Items.Count; i++)
                        {
                            var tab = tabControl.Items[i];
                            if (tab is TabItem && (tab as TabItem).Content == s.Control)
                            {
                                tabControl.Items.RemoveAt(i);
                                break;
                            }
                        }
                    };
                    status.OnPrefsChange = (s, p) =>
                    {
                        if (!string.IsNullOrWhiteSpace(p.nickname))
                        {
                            foreach (var tab in tabControl.Items)
                            {
                                if (tab is TabItem && (tab as TabItem).Content == s.Control)
                                {
                                    ChangeTitle(tab as TabItem, p.nickname);
                                }
                            }
                        }
                    };
                    status.OnRumbleSubscriptionChange = RumbleSettingsChanged;
                    _availableDevices.Add(status);
                    statusStack.Children.Add(status);

                    DevicePrefs devicePrefs = AppPrefs.Instance.GetDevicePreferences(info.DeviceID);
                    if (devicePrefs.autoConnect)
                    {
                        status.AutoConnect();
                    }
                }
                else if (!existing.Connected)
                {
                    DevicePrefs existingPrefs = AppPrefs.Instance.GetDevicePreferences(info.DeviceID);
                    if (existingPrefs.autoConnect)
                    {
                        existing.AutoConnect();
                    }
                }
            }
            
        }

        private void Device_RawInput(object sender, SharpDX.RawInput.RawInputEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e is SharpDX.RawInput.HidInputEventArgs);
        }

        private void DoConnect(DeviceStatus status, bool result)
        {
            if (result && status.Control != null)
            {
                // Associate L & R Joy-Cons
                if (status.Joy != null)
                {
                    if (status.Joy.Type == JoyControl.JoystickType.LeftJoyCon || status.Joy.Type == JoyControl.JoystickType.RightJoyCon)
                    {
                        foreach (var item in tabControl.Items)
                        {
                            if (item is TabItem && ((TabItem)item).Content is JoyControl)
                            {
                                var jc = ((JoyControl)((TabItem)item).Content);
                                if (jc.associatedJoyCon == null)
                                {
                                    if ((jc.Type == JoyControl.JoystickType.LeftJoyCon || jc.Type == JoyControl.JoystickType.RightJoyCon)
                                        && jc.Type != status.Joy.Type)
                                    {
                                        jc.AssociateJoyCon(status.Joy);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                var prefs = AppPrefs.Instance.GetDevicePreferences(status.Info.DeviceID);

                // If connection to device succeeds add a tab
                TabItem tab = new TabItem();
                StackPanel stack = new StackPanel { Orientation = Orientation.Horizontal };
                stack.Children.Add(new Image
                {
                    Source = status.Icon,
                    Height = 12,
                    Margin = new Thickness(0, 0, 4, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                });

                // Use the nickname if there is one
                if (prefs != null && !string.IsNullOrWhiteSpace(prefs.nickname))
                {
                    stack.Children.Add(new TextBlock { Text = prefs.nickname });
                }
                else
                {
                    stack.Children.Add(new TextBlock { Text = status.nickname.Content.ToString() });
                }

                tab.Header = stack;
                tab.Content = status.Control;
                tabControl.Items.Add(tab);
            }
            else if (!AppPrefs.Instance.suppressConnectionLost)
            {
                // Display message
                MessageBox.Show(
                    Globalization.Translate("Error_Connection"), 
                    Globalization.Translate("Error_Failed"), 
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /***********
         * The strategy here is once the program launches, it will gather all controller paths
         * and all saved information on each device and use what was saved about the device
         * to help populate details on each tab. The users can click on each tab and then
         * attempt to connect that controller then they are good to go.
         */

        private void AddController(object sender, MouseButtonEventArgs e)
        {
            // More testing
            TabItem test = new TabItem();
            var stack = new StackPanel() { Orientation = Orientation.Horizontal };
            stack.Children.Add(new Image()
            {
                Source = new BitmapImage(new Uri("../Images/Icons/ProController_white_24.png", UriKind.Relative)),
                Height = 12,
                Margin = new Thickness(0, 0, 4, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            });
            stack.Children.Add(new TextBlock() { Text = "DUMMY " + tabControl.Items.Count.ToString() });
            test.Header = stack;
            NintyControl nin = new NintyControl(new Shared.DeviceInfo() { DevicePath = "Dummy", Type = NintrollerLib.ControllerType.ProController });
            nin.OnTypeChange += (NintrollerLib.ControllerType type) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    ((Image)stack.Children[0]).Source = new BitmapImage(new Uri("../Images/Icons/ProController_black_24.png", UriKind.Relative));
                }));
            };
            test.Content = nin;
            tabControl.Items.Insert(tabControl.Items.Count - 1, test);
            tabControl.SelectedIndex = tabControl.Items.Count - 2;
            nin.Connect();
        }

        private void ChangeIcon(TabItem target, NintrollerLib.ControllerType type)
        {
            string img = "ProController_black_24.png";

            switch (type)
            {
                case NintrollerLib.ControllerType.ProController:
                    img = "ProController_black_24.png";
                    break;

                case NintrollerLib.ControllerType.Wiimote:
                    img = "wiimote_black_24.png";
                    break;

                case NintrollerLib.ControllerType.Nunchuk:
                case NintrollerLib.ControllerType.NunchukB:
                    img = "Wiimote+Nunchuck_black_24.png";
                    break;

                case NintrollerLib.ControllerType.ClassicController:
                    img = "Classic_black_24.png";
                    break;

                case NintrollerLib.ControllerType.ClassicControllerPro:
                    img = "ClassicPro_black_24.png";
                    break;
            }

            var stack = target.Header as StackPanel;
            if (stack != null && stack.Children.Count > 0 && stack.Children[0].GetType() == typeof(Image))
            {
                ((Image)stack.Children[0]).Source = new BitmapImage(new Uri("../Images/Icons/" + img, UriKind.Relative));
            }
        }

        private void ChangeTitle(TabItem target, string name)
        {
            var stack = target.Header as StackPanel;
            if (stack != null && stack.Children.Count > 1 && stack.Children[1].GetType() == typeof(TextBlock))
            {
                ((TextBlock)stack.Children[1]).Text = name;
            }
        }

        #region Settings Options

        private void settingAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            AppPrefs.Instance.SetAutoStart(settingAutoStart.IsChecked ?? false);
        }

        private void settingStartMinimized_Checked(object sender, RoutedEventArgs e)
        {
            AppPrefs.Instance.startMinimized = settingStartMinimized.IsChecked ?? false;
        }

        private void settingExclusiveMode_Checked(object sender, RoutedEventArgs e)
        {
            //WinBtStream.OverrideSharingMode = settingExclusiveMode.IsChecked ?? false;
            WinBtStream.OverrideSharingMode = true;
            
            if (settingExclusiveMode.IsChecked ?? false)
            {
                WinBtStream.OverridenFileShare = System.IO.FileShare.None;
            }
            else
            {
                WinBtStream.OverridenFileShare = System.IO.FileShare.ReadWrite;
            }

            AppPrefs.Instance.useExclusiveMode = settingExclusiveMode.IsChecked ?? false;
        }

        private void settingToshibaMode_Checked(object sender, RoutedEventArgs e)
        {
            WinBtStream.ForceToshibaMode = settingToshibaMode.IsChecked ?? false;
            AppPrefs.Instance.useToshibaMode = WinBtStream.ForceToshibaMode;
        }

        private void settingSuppressLostConn_Checked(object sender, RoutedEventArgs e)
        {
            AppPrefs.Instance.suppressConnectionLost = settingSuppressLostConn.IsChecked ?? false;
        }

        private void settingAutoAddXInputDevices_Checked(object sender, RoutedEventArgs e)
        {
            AppPrefs.Instance.autoAddXInputDevices = settingAutoAddXInputDevices.IsChecked ?? false;
            settingProfileQueuing.IsEnabled = settingAutoAddXInputDevices.IsChecked ?? false;
        }

        private void settingLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppPrefs.Instance.language != settingLanguage.SelectedIndex)
            {
                AppPrefs.Instance.language = settingLanguage.SelectedIndex;
                Globalization.SetSelectedLanguage(settingLanguage.SelectedIndex);

                // This will apply some translations but not all since some are created in code instead of being tagged on the xaml
                Globalization.ApplyTranslations(this);
                if (tabControl.Items.Count > 1)
                {
                    for (int i = 1; i < tabControl.Items.Count; ++i)
                    {
                        Globalization.ApplyTranslations(tabControl.Items[i] as DependencyObject);
                    }
                }

                MessageBox.Show(
                    Globalization.Translate("Restart_Msg"),
                    Globalization.Translate("Restart"),
                    MessageBoxButton.OK);
            }
        }

        private void settingMinimizeOnClose_Checked(object sender, RoutedEventArgs e)
        {
            AppPrefs.Instance.minimizeToTray = settingMinimizeOnClose.IsChecked ?? false;
        }
        private void settingProfileQueuing_Checked(object sender, RoutedEventArgs e)
        {
            AppPrefs.Instance.profileQueuing = settingProfileQueuing.IsChecked ?? false;
        }

        #endregion

        private void SetXInputDeviceStatus(ScpDirector.XInput_Device device)
        {
            string status = Globalization.Translate("Status_Disconnected");

            if (ScpDirector.Access.IsConnected(device))
            {
                status = Globalization.Translate("Status_Connected");
            }

            status = Globalization.TranslateFormat("XInput_Device_Num", (int)device, status);

            switch (device)
            {
                case ScpDirector.XInput_Device.Device_A:
                    xlabel1.Content = status;
                    break;
                case ScpDirector.XInput_Device.Device_B:
                    xlabel2.Content = status;
                    break;
                case ScpDirector.XInput_Device.Device_C:
                    xlabel3.Content = status;
                    break;
                case ScpDirector.XInput_Device.Device_D:
                    xlabel4.Content = status;
                    break;
            }
        }

        public void RumbleSettingsChanged(DeviceStatus s, bool[] rumbleSubscriptions)
        {
            if (AppPrefs.Instance.autoAddXInputDevices)
            {
                int n = 3;
                for (; n > -1; n--)
                {
                    if (rumbleSubscriptions[n])
                    {
                        break;
                    }
                }

                if (n > -1 && !ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_A))
                {
                    bool connected = false;
                    for (int i = 0; i < 4; i++)
                    {
                        ScpDirector.Access.SetModifier(i);
                        connected = ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_A);
                        if (connected) break;
                    }

                    if (connected)
                    {
                        btnRemoveXinput.IsEnabled = true;
                        xlabel1.Content = Globalization.TranslateFormat("XInput_Device_Num", "1", Globalization.Translate("Status_Auto"));
                    }
                }
                if (n > 0 && !ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_B))
                {
                    if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_B))
                    {
                        xlabel2.Content = Globalization.TranslateFormat("XInput_Device_Num", "2", Globalization.Translate("Status_Auto"));
                    }
                }
                if (n > 1 && !ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_C))
                {
                    if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_C))
                    {
                        xlabel3.Content = Globalization.TranslateFormat("XInput_Device_Num", "3", Globalization.Translate("Status_Auto"));
                    }
                }
                if (n > 2 && !ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_D))
                {
                    if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_D))
                    {
                        btnAddXinput.IsEnabled = false;
                        xlabel4.Content = Globalization.TranslateFormat("XInput_Device_Num", "4", Globalization.Translate("Status_Auto"));
                    }
                }
            }
        }

        private void btnAddXinput_Click(object sender, RoutedEventArgs e)
        {
            if (!ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_A))
            {
                bool connected = false;
                for (int i = 0; i < 4; i++)
                {
                    ScpDirector.Access.SetModifier(i);
                    connected = ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_A);
                    if (connected) break;
                }

                if (connected)
                {
                    btnRemoveXinput.IsEnabled = true;
                    xlabel1.Content = Globalization.TranslateFormat("XInput_Device_Num", "1", Globalization.Translate("Status_Connected"));
                }
            }
            else if (!ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_B))
            {
                if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_B))
                {
                    xlabel2.Content = Globalization.TranslateFormat("XInput_Device_Num", "2", Globalization.Translate("Status_Connected"));
                }
            }
            else if (!ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_C))
            {
                if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_C))
                {
                    xlabel3.Content = Globalization.TranslateFormat("XInput_Device_Num", "3", Globalization.Translate("Status_Connected"));
                }
            }
            else if (!ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_D))
            {
                if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_D))
                {
                    btnAddXinput.IsEnabled = false;
                    xlabel4.Content = Globalization.TranslateFormat("XInput_Device_Num", "4", Globalization.Translate("Status_Connected"));
                }
            }
        }

        private void btnRemoveXinput_Click(object sender, RoutedEventArgs e)
        {
            if (ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_D))
            {
                if (ScpDirector.Access.DisconnectDevice(ScpDirector.XInput_Device.Device_D))
                {
                    btnAddXinput.IsEnabled = true;
                    xlabel4.Content = Globalization.TranslateFormat("XInput_Device_Num", "4", Globalization.Translate("Status_Disconnected"));
                }
            }
            else if (ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_C))
            {
                if (ScpDirector.Access.DisconnectDevice(ScpDirector.XInput_Device.Device_C))
                {
                    xlabel3.Content = Globalization.TranslateFormat("XInput_Device_Num", "3", Globalization.Translate("Status_Disconnected"));
                }
            }
            else if (ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_B))
            {
                if (ScpDirector.Access.DisconnectDevice(ScpDirector.XInput_Device.Device_B))
                {
                    xlabel2.Content = Globalization.TranslateFormat("XInput_Device_Num", "2", Globalization.Translate("Status_Disconnected"));
                }
            }
            else if (ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_A))
            {
                if (ScpDirector.Access.DisconnectDevice(ScpDirector.XInput_Device.Device_A))
                {
                    btnRemoveXinput.IsEnabled = false;
                    xlabel1.Content = Globalization.TranslateFormat("XInput_Device_Num", "1", Globalization.Translate("Status_Disconnected"));
                }
            }
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            Windows.SyncWindow sync = new Windows.SyncWindow();
            sync.ShowDialog();
            Refresh();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Globalization.ApplyTranslations(this);

            foreach (string language in Globalization.GetAvailableLanguages()) 
            {
                var languageBox = new ComboBoxItem
                {
                    Content = language
                };
                settingLanguage.Items.Add(languageBox);
            }

            settingLanguage.SelectedIndex = AppPrefs.Instance.language;

            // Get the application version
            try
            {
                Version version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                versionLabel.Content = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
            catch { }

            // Check if auto start is enabled via registry
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                settingAutoStart.IsChecked |= key.GetValue("WiinUPro") != null;
            }
            catch { }

            // Check if auto start is enabled
            settingAutoStart.IsChecked = AppPrefs.Instance.GetAutoStartSet();

            // Check Suppress Connection Lost
            settingSuppressLostConn.IsChecked = AppPrefs.Instance.suppressConnectionLost;

            // Check Auto Add Xinput Devices
            if (AppPrefs.Instance.autoAddXInputDevices)
            {
                settingAutoAddXInputDevices.IsChecked = true;
                settingProfileQueuing.IsEnabled = true;
            }

            SetXInputDeviceStatus(ScpDirector.XInput_Device.Device_A);
            SetXInputDeviceStatus(ScpDirector.XInput_Device.Device_B);
            SetXInputDeviceStatus(ScpDirector.XInput_Device.Device_C);
            SetXInputDeviceStatus(ScpDirector.XInput_Device.Device_D);

            // Check Start Minimized
            if (AppPrefs.Instance.startMinimized)
            {
                settingStartMinimized.IsChecked = true;
                HideWindow();
            }

            // Check Exclusive Mode
            if (AppPrefs.Instance.useExclusiveMode)
            {
                settingExclusiveMode.IsChecked = true;
                settingExclusiveMode_Checked(this, new RoutedEventArgs());
            }

            // Check Toshiba Mode
            if (AppPrefs.Instance.useToshibaMode)
            {
                settingToshibaMode.IsChecked = true;
                settingToshibaMode_Checked(this, new RoutedEventArgs());
            }

            // Check Minimize To System Tray
            settingMinimizeOnClose.IsChecked = AppPrefs.Instance.minimizeToTray;

            // Check Profile Queuing
            settingProfileQueuing.IsChecked = AppPrefs.Instance.profileQueuing;

            Refresh();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Stop listening for devices
            DeviceListener.Instance.UnregisterDeviceNotification();
            DeviceListener.Instance.OnDevicesUpdated -= WindowsDevicesChanged;

            // Disconnect all XInput devices
            ScpDirector.Access.DisconnectAll();

            // Save preferences
            AppPrefs.Save();
        }

        private void TrayExit_Clicked(object sender, EventArgs e)
        {
            trayIcon.Visibility = Visibility.Hidden;
            this.Close();
        }

        private void TrayShow_Clicked(object sender, EventArgs e)
        {
            ShowWindow();
        }

        public void HideWindow()
        {
            // Currently only minimizing if option is selected.
            // Users may not want to have it minimize to tray all the time.
            // Contemplating if minimize on close should be another option or not...
            if (AppPrefs.Instance.minimizeToTray)
            {
                trayIcon.Visibility = Visibility.Visible;
                Hide();
            }
        }

        public void ShowWindow()
        {
            trayIcon.Visibility = Visibility.Hidden;
            Show();
            WindowState = WindowState.Normal;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Listen for devices coming and going
            DeviceListener.Instance.OnDevicesUpdated += WindowsDevicesChanged;
            DeviceListener.Instance.RegisterDeviceNotification(this, DeviceListener.GuidInterfaceHID, true);
        }

        private void WindowsDevicesChanged()
        {
            Refresh();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            WindowHidden = WindowState == WindowState.Minimized;

            if (WindowHidden)
            {
                HideWindow();
            }
        }
    }

    /// Used to invoke the double click action fo the tray icon
    class ShowWindowCommand : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.Instance?.ShowWindow();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
