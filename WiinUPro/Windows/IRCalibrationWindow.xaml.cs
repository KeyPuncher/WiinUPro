using System;
using System.Windows;
using System.Windows.Controls;
using NintrollerLib;
using System.Xml.Serialization;
using System.IO;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for IRCalibrationWindow.xaml
    /// </summary>
    public partial class IRCalibrationWindow : Window
    {
        public delegate void IRCalibrationDel(IRCalibration irCal);
        public bool Apply { get; set; }
        public IRCalibration Calibration { get { return _irCalibration; } }

        protected IRCalibration _irCalibration;
        protected bool set;

        public IRCalibrationWindow()
        {
            _irCalibration = new IRCalibration();
            InitializeComponent();
            set = true;
        }

        public IRCalibrationWindow(IR current) : this()
        {
            if (current.boundingArea is SquareBoundry)
            {
                SquareBoundry sqr = (SquareBoundry)current.boundingArea;
                _irCalibration.boundry = sqr;
                boxWidth.Value = sqr.width;
                boxHeight.Value = sqr.height;
                boxX.Value = sqr.center_x - sqr.width / 2;
                boxY.Value = sqr.center_y - sqr.height / 2;
            }
        }

        public void Update(IR update)
        {
            point1.Opacity = update.point1.visible ? 1 : 0;
            point2.Opacity = update.point2.visible ? 1 : 0;
            point3.Opacity = update.point3.visible ? 1 : 0;
            point4.Opacity = update.point4.visible ? 1 : 0;

            Canvas.SetLeft(point1, 1024 - update.point1.rawX - point1.Width / 2);
            Canvas.SetLeft(point2, 1024 - update.point2.rawX - point2.Width / 2);
            Canvas.SetLeft(point3, 1024 - update.point3.rawX - point3.Width / 2);
            Canvas.SetLeft(point4, 1024 - update.point4.rawX - point4.Width / 2);

            Canvas.SetTop(point1, update.point1.rawY - point1.Height / 2);
            Canvas.SetTop(point2, update.point2.rawY - point2.Height / 2);
            Canvas.SetTop(point3, update.point3.rawY - point3.Height / 2);
            Canvas.SetTop(point4, update.point4.rawY - point4.Height / 2);

            IR visual = update;
            visual.Normalize();
            Canvas.SetLeft(output, (visual.X + 1)/2 * 1023 - output.Width / 2);
            Canvas.SetTop(output, (-visual.Y + 1)/2 * 1023 - output.Height / 2);
        }
        
        private void BoxUpdated(int ignore)
        {
            if (!set) return;
            box.Width = boxWidth.Value;
            box.Height = boxHeight.Value;
            Canvas.SetLeft(box, boxX.Value);
            Canvas.SetTop(box, boxY.Value);
        }

        private void ScreenBehaviorChange(object sender, RoutedEventArgs e)
        {
            // TODO:
        }

        private void jitterSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // TODO:
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
            dialog.FileName = "ir_Calibration";
            dialog.DefaultExt = ".irc";
            dialog.Filter = App.IR_CAL_FILTER;

            bool? doLoad = dialog.ShowDialog();
            IRCalibration? loadedConfig = null;

            if (doLoad == true && dialog.CheckFileExists)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(IRCalibration));

                    using (FileStream stream = File.OpenRead(dialog.FileName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedConfig = serializer.Deserialize(reader) as IRCalibration?;
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
                    _irCalibration = loadedConfig.Value;
                    boxWidth.Value = _irCalibration.boundry.width;
                    boxHeight.Value = _irCalibration.boundry.height;
                    boxX.Value = _irCalibration.boundry.center_x - _irCalibration.boundry.width / 2;
                    boxY.Value = _irCalibration.boundry.center_y - _irCalibration.boundry.height / 2;
                }
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "ir_calibration";
            dialog.DefaultExt = ".irc";
            dialog.Filter = App.IR_CAL_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(IRCalibration));

                using (FileStream stream = File.Create(dialog.FileName))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    serializer.Serialize(writer, _irCalibration);
                    writer.Close();
                    stream.Close();
                }
            }
        }
    }

    public struct IRCalibration
    {
        public SquareBoundry boundry;
        public bool useLastGoodPoint;
        public int jitterReduction;
    }
}
