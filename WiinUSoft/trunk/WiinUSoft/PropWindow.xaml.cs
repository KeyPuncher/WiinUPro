using System;
using System.Windows;
using System.Windows.Controls;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for PropWindow.xaml
    /// </summary>
    public partial class PropWindow : Window
    {
        // TODO: connect to specific device
        public bool doSave = false;
        public Property props;

        public PropWindow(Property org, string defalutName = "")
        {
            InitializeComponent();

            props = new Property(org);
            nameInput.Text = string.IsNullOrWhiteSpace(props.name) ? defalutName : props.name;
            defaultInput.Text = props.profile;
            autoCheckbox.IsChecked = props.autoConnect;
            rumbleEnabled.IsChecked = props.useRumble;
            if (props.autoNum >= 0 && props.autoNum <= autoConnectNumber.Items.Count)
            {
                autoConnectNumber.SelectedIndex = props.autoNum;
            }
            if (props.rumbleIntensity >= 0 && props.rumbleIntensity <= rumbleSelection.Items.Count)
            {
                rumbleSelection.SelectedIndex = props.rumbleIntensity;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            doSave = true;
            Close();
        }

        private void autoCheckbox_Click(object sender, RoutedEventArgs e)
        {
            props.autoConnect = autoCheckbox.IsChecked == true;
        }

        private void rumbleEnabled_Click(object sender, RoutedEventArgs e)
        {
            props.useRumble = rumbleEnabled.IsChecked == true;
        }

        private void nameInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            props.name = nameInput.Text;
        }

        private void defaultInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            props.profile = defaultInput.Text;
        }

        private void defaultBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".wsp";
            dialog.Filter = App.PROFILE_FILTER;

            Nullable<bool> doLoad = dialog.ShowDialog();

            if (doLoad == true && dialog.CheckFileExists)
            {
                defaultInput.Text = dialog.FileName;
            }
        }

        private void AutoConnect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (props != null)
            {
                props.autoConnect = autoConnectNumber.SelectedIndex > 0;
                props.autoNum = autoConnectNumber.SelectedIndex;
            }
        }

        private void Rumble_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (props != null)
            {
                props.useRumble = rumbleSelection.SelectedIndex > 0;
                props.rumbleIntensity = rumbleSelection.SelectedIndex;
            }
        }
    }
}
