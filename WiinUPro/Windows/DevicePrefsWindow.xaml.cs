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
using System.Windows.Shapes;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for DevicePrefsWindow.xaml
    /// </summary>
    public partial class DevicePrefsWindow : Window
    {
        public bool DoSave { get; protected set; }
        public DevicePrefs Preferences { get; protected set; }
        
        private DevicePrefs _modifiedPrefs;

        private int lastIndex;

        protected DevicePrefsWindow()
        {
            InitializeComponent();
        }

        public DevicePrefsWindow(DevicePrefs devicePrefs, string type = null) : this()
        {
            Preferences = devicePrefs;
            deviceID.Content = devicePrefs.deviceId;
            nickname.Text = devicePrefs.nickname;
            defaultProfile.Text = devicePrefs.defaultProfile;
            autoConnect.IsChecked = devicePrefs.autoConnect;
            
            _modifiedPrefs = new DevicePrefs()
            {
                deviceId = devicePrefs.deviceId
            };
            devicePrefs.extensionProfiles.CopyTo(_modifiedPrefs.extensionProfiles, 0);

            foreach (var calibrationPath in devicePrefs.calibrationFiles)
            {
                _modifiedPrefs.calibrationFiles.Add(calibrationPath.Key, calibrationPath.Value);

                StackPanel stack = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Top,
                };

                Button removeBtn = new Button
                {
                    Content = " X ",
                    Height = 14,
                    Width = 14,
                    FontSize = 8,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Style = (Style)Application.Current.Resources["WarningButton"],
                    Tag = calibrationPath.Key
                };

                removeBtn.Click += RemoveBtn_Click;

                stack.Children.Add(removeBtn);
                stack.Children.Add(new Rectangle()
                {
                    Width = 4
                });
                stack.Children.Add(new Label()
                {
                    Content = calibrationPath.Value,
                    FontSize = 10,
                    Height = 15,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Padding = new Thickness(0),
                    Foreground = (Brush)Application.Current.Resources["TextBody"]
                });

                calibrationWrap.Children.Add(stack);
            }
            bool wiimoteType = false;
            string[] extTypes = new string[] { "Wiimote", "Nunchuk", "ClassicController", "ClassicControllerPro", "Guitar", "TaikoDrum" };
            for(int i = 0; i < comboExtProfile.Items.Count; i++)
            {
                if (extTypes[i] == type)
                {
                    comboExtProfile.SelectedIndex = i;
                    lastIndex = i;
                    wiimoteType = true;
                    break;
                }
            }
            if (wiimoteType)
            {
                extProfile.Text = _modifiedPrefs.extensionProfiles[lastIndex];
            }
            else
            {
                comboExtProfile.Visibility = Visibility.Hidden;
                labelExtProfile.Visibility = Visibility.Hidden;
                btnExtProfile.Visibility = Visibility.Hidden;
                extProfile.Visibility = Visibility.Hidden;
                Thickness margin = labelDefaultCalibrations.Margin;
                margin.Top = 114;
                labelDefaultCalibrations.Margin = margin;
                margin = calibrationViewer.Margin;
                margin.Top = 140;
                calibrationViewer.Margin = margin;
            }
        }

        private void FindProfile(TextBox textBox)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".wup";
            dialog.Filter = App.PROFILE_FILTER;

            bool? didFind = dialog.ShowDialog();

            if (didFind == true && dialog.CheckFileExists)
            {
                textBox.Text = dialog.FileName;
            }
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement elm = sender as FrameworkElement;
            if (_modifiedPrefs.calibrationFiles.ContainsKey(elm.Tag.ToString()))
            {
                _modifiedPrefs.calibrationFiles.Remove(elm.Tag.ToString());
            }

            calibrationWrap.Children.Remove(elm.Parent as UIElement);
        }

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            _modifiedPrefs.autoConnect = autoConnect.IsChecked ?? false;
            _modifiedPrefs.nickname = nickname.Text;
            _modifiedPrefs.defaultProfile = defaultProfile.Text;
            _modifiedPrefs.extensionProfiles[comboExtProfile.SelectedIndex] = extProfile.Text;

            Preferences.Copy(_modifiedPrefs);

            DoSave = true;
            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DoSave = false;
            Close();
        }

        private void btnDefaultProfile_Click(object sender, RoutedEventArgs e)
        {
            FindProfile(defaultProfile);
        }

        private void btnExtProfile_Click(object sender, RoutedEventArgs e)
        {
            FindProfile(extProfile);
            _modifiedPrefs.extensionProfiles[comboExtProfile.SelectedIndex] = extProfile.Text;
        }

        private void comboExtProfSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_modifiedPrefs != null)
            {
                _modifiedPrefs.extensionProfiles[lastIndex] = extProfile.Text;
                lastIndex = comboExtProfile.SelectedIndex;
                extProfile.Text = _modifiedPrefs.extensionProfiles[lastIndex];
            }
        }
    }
}
