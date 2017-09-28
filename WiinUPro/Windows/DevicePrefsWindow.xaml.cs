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

        protected DevicePrefsWindow()
        {
            InitializeComponent();
        }

        public DevicePrefsWindow(DevicePrefs devicePrefs) : this()
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
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".wup";
            dialog.Filter = App.PROFILE_FILTER;

            bool? didFind = dialog.ShowDialog();

            if (didFind == true && dialog.CheckFileExists)
            {
                defaultProfile.Text = dialog.FileName;
            }
        }
    }
}
