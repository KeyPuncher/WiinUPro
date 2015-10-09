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
using NintrollerLib;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for NintyControl.xaml
    /// </summary>
    public partial class NintyControl : UserControl
    {
        internal Nintroller _nintroller;
        internal INintyControl _controller;

        public NintyControl()
        {
            InitializeComponent();
        }

        public NintyControl(string devicePath) : this()
        {
            _nintroller = new Nintroller(devicePath);
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (true || _nintroller.Connect())
            {
                btnConnect.IsEnabled = false;

                _controller = new ProControl();
                _controller.ChangeLEDs(_nintroller.Led1, _nintroller.Led2, _nintroller.Led3, _nintroller.Led4);
                _view.Child = _controller as ProControl;
                ((UserControl)_view.Child).HorizontalAlignment = HorizontalAlignment.Left;
                ((UserControl)_view.Child).VerticalAlignment = VerticalAlignment.Top;

                btnDisconnect.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Could not connect to device!");
            }
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            btnDisconnect.IsEnabled = false;

            _nintroller.Disconnect();
            _view.Child = null;
            _controller = null;

            btnConnect.IsEnabled = true;
        }
    }

    public interface INintyControl
    {
        void UpdateVisual(INintrollerState state);
        void ChangeLEDs(bool one, bool two, bool three, bool four);
    }
}
