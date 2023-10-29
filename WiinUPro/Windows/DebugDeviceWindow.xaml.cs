using NintrollerLib;
using System;
using System.Linq;
using System.Windows;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for DebugDeviceWindow.xaml
    /// </summary>
    public partial class DebugDeviceWindow : Window
    {
        private const int SIZE = 10;
        private byte[][] _buffer = new byte[SIZE][];
        private int _latest;
        private int _current;

        public DebugDeviceWindow(Nintroller nintroller)
        {
            InitializeComponent();

            nintroller.RawUpdate += Nintroller_RawUpdate;
        }

        public void Capture()
        {
            var dataCopy = _buffer[_latest].ToArray();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                display.Text += BitConverter.ToString(dataCopy) + "\n";
            }));
        }

        private void Nintroller_RawUpdate(byte[] obj)
        {
            _current += 1;
            
            if (_current >= SIZE)
                _current = 0;

            _buffer[_current] = obj;
            _latest = _current;
            _current += 1;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                live.Text = BitConverter.ToString(obj) + "\n";
            }));
        }

        private void captureBtn_Click(object sender, RoutedEventArgs e)
        {
            Capture();
        }

        private void copyBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(display.Text);
        }
    }
}
