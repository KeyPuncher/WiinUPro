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

            props = org;
            nameInput.Text = string.IsNullOrWhiteSpace(props.name) ? defalutName : props.name;
            defaultInput.Text = props.profile;
            autoCheckbox.IsChecked = props.autoConnect;
            if (props.autoNum >= 0 && props.autoNum <= autoConnectNumber.Items.Count)
                autoConnectNumber.SelectedIndex = props.autoNum;
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (props != null)
            {
                props.autoConnect = autoConnectNumber.SelectedIndex > 0;
                props.autoNum = autoConnectNumber.SelectedIndex;
            }
        }
    }
}
