using Shared;
using System.Windows;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for RumbleWindow.xaml
    /// </summary>
    public partial class RumbleWindow : Window
    {
        public bool[] Result { get; protected set; }
        public RumbleWindow()
        {
            InitializeComponent();
            Result = new bool[4];
        }

        public RumbleWindow(bool[] subscriptions) : this()
        {
            if (subscriptions.Length >= 4)
            {
                xDeviceA.IsChecked = subscriptions[0];
                xDeviceB.IsChecked = subscriptions[1];
                xDeviceC.IsChecked = subscriptions[2];
                xDeviceD.IsChecked = subscriptions[3];
                Result = subscriptions;
            }
        }

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            Result[0] = xDeviceA.IsChecked ?? false;
            Result[1] = xDeviceB.IsChecked ?? false;
            Result[2] = xDeviceC.IsChecked ?? false;
            Result[3] = xDeviceD.IsChecked ?? false;

            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Globalization.ApplyTranslations(this);
        }
    }
}
