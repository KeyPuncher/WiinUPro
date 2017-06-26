using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for AxisCalibrationWindow.xaml
    /// </summary>
    public partial class AxisCalibrationWindow : Window
    {
        public bool Apply { get; protected set; }
        public AxisCalibration Calibration { get { return _axis; } }

        protected AxisCalibration _axis;
        protected bool set;
        protected int _lastValue;

        protected AxisCalibrationWindow()
        {
            InitializeComponent();
        }

        public AxisCalibrationWindow(AxisCalibration prevCalibration) : this()
        {
            _axis = prevCalibration;
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
            dialog.FileName = "axis_Calibration";
            dialog.DefaultExt = ".axs";
            dialog.Filter = App.AXIS_CAL_FILTER;

            bool? doLoad = dialog.ShowDialog();
            AxisCalibration? loadedConfig = null;

            if (doLoad == true && dialog.CheckFileExists)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AxisCalibration));

                    using (FileStream stream = File.OpenRead(dialog.FileName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedConfig = serializer.Deserialize(reader) as AxisCalibration?;
                        reader.Close();
                        stream.Close();
                    }
                }
                catch (Exception err)
                {
                    var c = MessageBox.Show("Could not open the file \"" + err.Message + "\".", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (loadedConfig != null && loadedConfig.HasValue)
                {
                    center.Value = loadedConfig.Value.center;
                    min.Value = (int)Math.Round(100 - 100 * loadedConfig.Value.min / 65535d);
                    max.Value = (int)Math.Round(100 * loadedConfig.Value.max / 65535d);
                    deadMax.Value = (int)Math.Round(100 * loadedConfig.Value.deadPos / 65535d);
                    deadMin.Value = (int)Math.Round(-100 * loadedConfig.Value.deadNeg / 65535d);
                }
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "axis_Calibration";
            dialog.DefaultExt = ".axs";
            dialog.Filter = App.AXIS_CAL_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AxisCalibration));

                using (FileStream stream = File.Create(dialog.FileName))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    serializer.Serialize(writer, _axis);
                    writer.Close();
                    stream.Close();
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
