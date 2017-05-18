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
    }
}
