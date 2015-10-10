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
            _nintroller.StateUpdate += _nintroller_StateUpdate; 
            _nintroller.ExtensionChange += _nintroller_ExtensionChange;
            _nintroller.LowBattery += _nintroller_LowBattery;
        }

        #region Nintroller Events
        private void _nintroller_LowBattery(object sender, LowBatteryEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void _nintroller_ExtensionChange(object sender, NintrollerExtensionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void _nintroller_StateUpdate(object sender, NintrollerStateEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (_controller != null)
                {
                    _controller.UpdateVisual(e.state);
                }
            }));
        }

        #endregion

        #region GUI Events
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_nintroller.Connect())
            {
                btnConnect.IsEnabled = false;
                _nintroller.BeginReading();
                _nintroller.GetStatus();
                _nintroller.SetPlayerLED(1);

                _controller = new ProControl();
                _controller.OnChangeLEDs += SetLeds;
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
        #endregion

        #region Control Events
        private void SetLeds(object sender, bool[] values)
        {
            if (_nintroller != null && (values ?? new bool[0]).Length == 4)
            {
                _nintroller.Led1 = values[0];
                _nintroller.Led2 = values[1];
                _nintroller.Led3 = values[2];
                _nintroller.Led4 = values[3];
            }
        }
        #endregion
    }

    public interface INintyControl
    {
        event EventHandler<bool[]> OnChangeLEDs;

        void UpdateVisual(INintrollerState state);
        void ChangeLEDs(bool one, bool two, bool three, bool four);
    }
}
