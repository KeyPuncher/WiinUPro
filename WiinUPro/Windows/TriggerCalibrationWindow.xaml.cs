using System;
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
        public string FileName { get; protected set; }

        protected Trigger _trigger;
        protected Trigger _default;
        protected bool set;

        protected TriggerCalibrationWindow()
        {
            InitializeComponent();
        }

        public TriggerCalibrationWindow(Trigger nonCalibrated, Trigger prevCalibration, string filename = "") : this()
        {
            _default = nonCalibrated;
            FileName = filename;
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
            dialog.FileName = string.IsNullOrEmpty(FileName) ? "trigger_Calibration" : FileName;
            dialog.DefaultExt = ".trg";
            dialog.Filter = App.TRIG_CAL_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                if (App.SaveToFile<Trigger>(dialog.FileName, _trigger))
                {
                    FileName = dialog.FileName;
                }
            }
        }

        private void loadBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = string.IsNullOrEmpty(FileName) ? "trigger_Calibration" : FileName;
            dialog.DefaultExt = ".trg";
            dialog.Filter = App.TRIG_CAL_FILTER;

            bool? doLoad = dialog.ShowDialog();
            Trigger loadedConfig;

            if (doLoad == true && dialog.CheckFileExists)
            {
                if (App.LoadFromFile<Trigger>(dialog.FileName, out loadedConfig))
                {
                    FileName = dialog.FileName;
                    min.Value = loadedConfig.min;
                    max.Value = loadedConfig.max;
                }
                else
                {
                    var c = System.Windows.MessageBox.Show("Could not open the file \"" + dialog.FileName + "\".", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
