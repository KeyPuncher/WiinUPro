using System;
using System.IO;
using System.Xml.Serialization;
using NintrollerLib;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for TriggerCalibrationWindow.xaml
    /// </summary>
    public partial class TriggerCalibrationWindow : System.Windows.Window
    {
        public bool Apply { get; protected set; }
        public Trigger Calibration { get { return _trigger; } }

        protected Trigger _trigger;
        protected Trigger _default;
        protected bool set;

        protected TriggerCalibrationWindow()
        {
            InitializeComponent();
        }
        public TriggerCalibrationWindow(Trigger nonCalibrated, Trigger prevCalibration) : this()
        {
            _default = nonCalibrated;
            set = true;
            Set(prevCalibration);
        }

        public void Set(Trigger cal)
        {
            if (!set) return;

            _trigger = cal;

            double minPercent = (_default.max - _trigger.min) / (double)(_default.max - _default.min);
            rawMin.Margin = new System.Windows.Thickness(210 - (200 * minPercent), 13, 0, 0);

            double maxPercent = (_trigger.max - _default.min) / (double)(_default.max - _default.min);
            rawMax.Margin = new System.Windows.Thickness(10 + (200 * maxPercent), 13, 0, 0);
        }

        public void Update(Trigger value)
        {
            _default.rawValue = value.rawValue;
            _default.Normalize();
            raw.Width = 200 * _default.value;

            _trigger.rawValue = value.rawValue;
            _trigger.Normalize();
            output.Width = 200 * _trigger.value;
        }

        private void acceptBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Apply = true;
            Close();
        }

        private void cancelBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void saveBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "trigger_calibration";
            dialog.DefaultExt = ".trg";
            dialog.Filter = App.TRIG_CAL_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Trigger));

                using (FileStream stream = File.Create(dialog.FileName))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    serializer.Serialize(writer, _trigger);
                    writer.Close();
                    stream.Close();
                }
            }
        }

        private void loadBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "trigger_Calibration";
            dialog.DefaultExt = ".trg";
            dialog.Filter = App.TRIG_CAL_FILTER;

            bool? doLoad = dialog.ShowDialog();
            Trigger? loadedConfig = null;

            if (doLoad == true && dialog.CheckFileExists)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Trigger));

                    using (FileStream stream = File.OpenRead(dialog.FileName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedConfig = serializer.Deserialize(reader) as Trigger?;
                        reader.Close();
                        stream.Close();
                    }
                }
                catch (Exception err)
                {
                    var c = System.Windows.MessageBox.Show("Could not open the file \"" + err.Message + "\".", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }

                if (loadedConfig != null && loadedConfig.HasValue)
                {
                    min.Value = loadedConfig.Value.min;
                    max.Value = loadedConfig.Value.max;
                }
            }
        }

        private void MinUpdated(int value)
        {
            _trigger.min = value;
            Set(_trigger);
        }

        private void MaxUpdated(int value)
        {
            _trigger.max = value;
            Set(_trigger);
        }
    }
}
