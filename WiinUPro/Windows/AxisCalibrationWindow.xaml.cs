using Shared;
using System;
using System.Windows;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for AxisCalibrationWindow.xaml
    /// </summary>
    public partial class AxisCalibrationWindow : Window
    {
        public bool Apply { get; protected set; }
        public AxisCalibration Calibration { get { return _axis; } }
        public string FileName { get; protected set; }

        protected AxisCalibration _axis;
        protected bool set;
        protected int _lastValue;

        protected AxisCalibrationWindow()
        {
            InitializeComponent();
        }

        public AxisCalibrationWindow(AxisCalibration prevCalibration, string filename = "") : this()
        {
            _axis = prevCalibration;
            FileName = filename;
            set = true;
            center.Value = prevCalibration.center;
            min.Value = (int)Math.Round(100 - 100 * prevCalibration.min / 65535d);
            max.Value = (int)Math.Round(100 * prevCalibration.max / 65535d);
            deadMax.Value = (int)Math.Round(100 * prevCalibration.deadPos / 65535d);
            deadMin.Value = (int)Math.Round(-100 * prevCalibration.deadNeg / 65535d);
        }

        public void Set(AxisCalibration cal)
        {
            if (!set) return;

            _axis = cal;

            limit.Width = 400 * Math.Abs(_axis.max - _axis.min) / 65535d;
            limit.Margin = new Thickness(10 + (400 * _axis.min / 65535d), 13, 0, 0);

            dead.Width = 400 * (_axis.deadPos - _axis.deadNeg) / 65535d;
            dead.Margin = new Thickness(10 + 200 - (dead.Width / 2) + ((_axis.deadPos + _axis.deadNeg) / 65535d) * 200, 13, 0, 0);
            Update(_lastValue);
        }

        public void Update(int newValue)
        {
            _lastValue = newValue;

            raw.Margin = new Thickness(9 + 400 * (newValue / 65535d), 13, 0, 0);

            var percent = _axis.Normal(newValue);
            output.Margin = new Thickness(9 + 400 * (percent + 1)/2, 48, 0, 0);
            value.Content = string.Format("{0}%", Math.Round(percent * 100));
        }

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            Apply = true;
            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = string.IsNullOrEmpty(FileName) ? "axis_Calibration" : FileName;
            dialog.DefaultExt = ".axs";
            dialog.Filter = App.AXIS_CAL_FILTER;

            bool? doLoad = dialog.ShowDialog();
            AxisCalibration loadedConfig;

            if (doLoad == true && dialog.CheckFileExists)
            {
                if (App.LoadFromFile<AxisCalibration>(dialog.FileName, out loadedConfig))
                {
                    FileName = dialog.FileName;
                    center.Value = loadedConfig.center;
                    min.Value = (int)Math.Round(100 - 100 * loadedConfig.min / 65535d);
                    max.Value = (int)Math.Round(100 * loadedConfig.max / 65535d);
                    deadMax.Value = (int)Math.Round(100 * loadedConfig.deadPos / 65535d);
                    deadMin.Value = (int)Math.Round(-100 * loadedConfig.deadNeg / 65535d);
                }
                else
                {
                    MessageBox.Show(
                        Globalization.TranslateFormat("Calibration_Load_Error_Msg", dialog.FileName),
                        Globalization.Translate("Calibration_Load_Error"),
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                }
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = string.IsNullOrEmpty(FileName) ? "axis_Calibration" : FileName;
            dialog.DefaultExt = ".axs";
            dialog.Filter = App.AXIS_CAL_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                if (App.SaveToFile<AxisCalibration>(dialog.FileName, _axis))
                {
                    FileName = dialog.FileName;
                }
            }
        }

        private void CenterUpdated(int v)
        {
            _axis.center = v;
        }

        private void MinUpdated(int v)
        {
            if (!set) return;
            _axis.min = (int)Math.Round(65535 - 65535 * v / 100d);
            Set(_axis);
        }

        private void MaxUpdated(int v)
        {
            if (!set) return;
            _axis.max = (int)Math.Round(65535 * v / 100d);
            Set(_axis);
        }

        private void DeadMinUpdated(int v)
        {
            if (!set) return;
            _axis.deadNeg = (int)Math.Round(65535 * v / -100d);
            Set(_axis);
        }

        private void DeadMaxUpdated(int v)
        {
            if (!set) return;
            _axis.deadPos = (int)Math.Round(65535 * v / 100d);
            Set(_axis);
        }
    }
}
