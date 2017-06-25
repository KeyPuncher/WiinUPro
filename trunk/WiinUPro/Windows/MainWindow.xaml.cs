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
using Shared.Windows;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SharpDX.DirectInput.DirectInput DInput;

        List<DeviceStatus> availableDevices;

        public MainWindow()
        {
            availableDevices = new List<DeviceStatus>();
            InitializeComponent();

            WinBtStream.OverrideSharingMode = true;
            WinBtStream.OverridenFileShare = System.IO.FileShare.ReadWrite;

            #region Test
            /*
            var devices =  WinBtStream.GetPaths();
            foreach (var info in devices)
            {
                TabItem t = new TabItem();
                var stack = new StackPanel { Orientation = Orientation.Horizontal };
                stack.Children.Add(new Image
                {
                    Source = new BitmapImage(new Uri("../Images/Icons/ProController_black_24.png", UriKind.Relative)),
                    Height = 12,
                    Margin = new Thickness(0, 0, 4, 0),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                });
                stack.Children.Add(new TextBlock { Text = "Real" + tabControl.Items.Count });
                t.Header = stack;
                NintyControl n = new NintyControl(info);
                n.OnTypeChange += (type) => { ChangeIcon(t, type); };
                t.Content = n;
                tabControl.Items.Insert(tabControl.Items.Count - 1, t);
            }
            Windows.JoyCalibrationWindow j = new Windows.JoyCalibrationWindow();
            j.Show();
            */
            #endregion

            Refresh();

            // Speech test
            //System.Speech.Synthesis.SpeechSynthesizer s = new System.Speech.Synthesis.SpeechSynthesizer();
            //s.SelectVoiceByHints(System.Speech.Synthesis.VoiceGender.Female);
            //s.SpeakAsync("Welcome");
        }

        public void Refresh()
        {
            var devices = WinBtStream.GetPaths();

            // Direct input Devices
            DInput = new SharpDX.DirectInput.DirectInput();
            var joys = DInput.GetDevices(SharpDX.DirectInput.DeviceClass.GameControl, SharpDX.DirectInput.DeviceEnumerationFlags.AllDevices);

            foreach (var j in joys)
            {
                string pid = j.ProductGuid.ToString().Substring(0, 4);
                string vid = j.ProductGuid.ToString().Substring(4, 4);

                // devices to ignore
                if (vid == "057e" && (pid == "0330" || pid == "0306"))
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

            foreach (var info in devices)
            {
                // Check if we are already showing this one
                DeviceStatus existing = availableDevices.Find((d) => d.Info.SameDevice(info.DevicePath));

                // If not add it
                if (existing == null)
                {
                    var status = new DeviceStatus(info);
                    status.ConnectClick = DoConnect;
                    status.TypeUpdated = (s, t) =>
                    {
                        foreach (var tab in tabControl.Items)
                        {
                            if (tab is TabItem && (tab as TabItem).Content == s.Control)
                            {
                                ChangeIcon(tab as TabItem, t);
                                ChangeTitle(tab as TabItem, t.ToString());
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
                    availableDevices.Add(status);
                    statusStack.Children.Add(status);
                }
            }
            
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
                stack.Children.Add(new TextBlock { Text = status.nickname.Content.ToString() });
                tab.Header = stack;
                tab.Content = status.Control;
                tabControl.Items.Add(tab);
            }
            else
            {
                // Display message
                MessageBox.Show("Unable to Connect Device", "Failed", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
        }

        private void settingToshibaMode_Checked(object sender, RoutedEventArgs e)
        {
            WinBtStream.ForceToshibaMode = settingToshibaMode.IsChecked ?? false;
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
                    xlabel1.Content = "Device 1: Connected";
                }
            }
            else if (!ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_B))
            {
                if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_B))
                {
                    xlabel2.Content = "Device 2: Connected";
                }
            }
            else if (!ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_C))
            {
                if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_C))
                {
                    xlabel3.Content = "Device 3: Connected";
                }
            }
            else if (!ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_D))
            {
                if (ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_D))
                {
                    btnAddXinput.IsEnabled = false;
                    xlabel4.Content = "Device 4: Connected";
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
                    xlabel4.Content = "Device 4: Disconnected";
                }
            }
            else if (ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_C))
            {
                if (ScpDirector.Access.DisconnectDevice(ScpDirector.XInput_Device.Device_C))
                {
                    xlabel3.Content = "Device 3: Disconnected";
                }
            }
            else if (ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_B))
            {
                if (ScpDirector.Access.DisconnectDevice(ScpDirector.XInput_Device.Device_B))
                {
                    xlabel2.Content = "Device 2: Disconnected";
                }
            }
            else if (ScpDirector.Access.IsConnected(ScpDirector.XInput_Device.Device_A))
            {
                if (ScpDirector.Access.DisconnectDevice(ScpDirector.XInput_Device.Device_A))
                {
                    btnRemoveXinput.IsEnabled = false;
                    xlabel1.Content = "Device 1: Disconnected";
                }
            }
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            Windows.SyncWindow sync = new Windows.SyncWindow();
            sync.ShowDialog();
            Refresh();
        }
    }
}
