using Shared;
using System;
using System.Windows;
using System.Windows.Controls;
using NintrollerLib;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for IRCalibrationWindow.xaml
    /// </summary>
    public partial class IRCalibrationWindow : Window
    {
        public delegate void IRCalibrationDel(IRCalibration irCal, string file = "");
        public bool Apply { get; set; }
        public IRCalibration Calibration { get { return _irCalibration; } }
        public string FileName { get; protected set; }

        protected IRCalibration _irCalibration;
        protected bool set;

        protected IRCalibrationWindow()
        {
            _irCalibration = new IRCalibration();
            InitializeComponent();
        }

        public IRCalibrationWindow(IR current, string filename = "") : this()
        {
            FileName = filename;

            _irCalibration.leftBounds = current.leftBounds;
            _irCalibration.rightBounds = current.rightBounds;
            _irCalibration.topBounds = current.topBounds;
            _irCalibration.bottomBounds = current.bottomBounds;
            _irCalibration.offscreenBehavior = current.offscreenBehavior;
            _irCalibration.minVisiblePoints = current.minimumVisiblePoints;

            minVisiblePoints.SelectedIndex = (int)_irCalibration.minVisiblePoints;

            switch (current.offscreenBehavior)
            {
                case IRCamOffscreenBehavior.UseLastPoint:
                    radioLast.IsChecked = true;
                    radioCenter.IsChecked = false;
                    break;

                case IRCamOffscreenBehavior.ReturnToCenter:
                    radioLast.IsChecked = false;
                    radioCenter.IsChecked = true;
                    break;
            }

            areaLeft.Value = _irCalibration.leftBounds;
            areaRight.Value = _irCalibration.rightBounds;
            areaTop.Value = _irCalibration.topBounds;
            areaBottom.Value = _irCalibration.bottomBounds;

            if (current.deadArea is SquareBoundry)
            {
                SquareBoundry sqr = (SquareBoundry)current.deadArea;
                _irCalibration.deadzone = sqr;
                boxWidth.Value = sqr.width;
                boxHeight.Value = sqr.height;
                boxX.Value = sqr.center_x;
                boxY.Value = sqr.center_y;
            }

            set = true;
            AreaUpdated(0);
            BoxUpdated(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Globalization.ApplyTranslations(this);
        }

        public void Update(IR update)
        {
            point1.Opacity = update.point1.visible ? 1 : 0;
            point2.Opacity = update.point2.visible ? 1 : 0;
            point3.Opacity = update.point3.visible ? 1 : 0;
            point4.Opacity = update.point4.visible ? 1 : 0;

            Canvas.SetLeft(point1, 1023 - update.point1.rawX - point1.Width / 2);
            Canvas.SetLeft(point2, 1023 - update.point2.rawX - point2.Width / 2);
            Canvas.SetLeft(point3, 1023 - update.point3.rawX - point3.Width / 2);
            Canvas.SetLeft(point4, 1023 - update.point4.rawX - point4.Width / 2);

            Canvas.SetTop(point1, update.point1.rawY - point1.Height / 2);
            Canvas.SetTop(point2, update.point2.rawY - point2.Height / 2);
            Canvas.SetTop(point3, update.point3.rawY - point3.Height / 2);
            Canvas.SetTop(point4, update.point4.rawY - point4.Height / 2);

            IR visual = update;
            visual.deadArea = _irCalibration.deadzone;
            visual.leftBounds = _irCalibration.leftBounds;
            visual.rightBounds = _irCalibration.rightBounds;
            visual.topBounds = _irCalibration.topBounds;
            visual.bottomBounds = _irCalibration.bottomBounds;
            visual.minimumVisiblePoints = (IRCamMinimumVisiblePoints)_irCalibration.minVisiblePoints;
            visual.offscreenBehavior = (IRCamOffscreenBehavior)_irCalibration.offscreenBehavior;
            visual.Normalize();

            Canvas.SetLeft(output, (visual.X + 1)/2 * 1023 - output.Width / 2);
            Canvas.SetTop(output, (-visual.Y + 1)/2 * 767 - output.Height / 2);
        }
        
        private void BoxUpdated(int ignore)
        {
            if (!set) return;
            box.Width = boxWidth.Value;
            box.Height = boxHeight.Value;
            Canvas.SetLeft(box, boxX.Value - _irCalibration.deadzone.width / 2);
            Canvas.SetTop(box, boxY.Value - _irCalibration.deadzone.height / 2);
            UpdateCalibration();
        }

        private void AreaUpdated(int ignore)
        {
            if (!set) return;
            area.Width = Math.Max(0, areaRight.Value - areaLeft.Value);
            area.Height = Math.Max(0, areaBottom.Value - areaTop.Value);
            Canvas.SetLeft(area, areaLeft.Value);
            Canvas.SetTop(area, areaTop.Value);
            UpdateCalibration();
        }

        private void UpdateCalibration()
        {
            _irCalibration.deadzone.width = boxWidth.Value;
            _irCalibration.deadzone.height = boxHeight.Value;
            _irCalibration.deadzone.center_x = boxX.Value;
            _irCalibration.deadzone.center_y = boxY.Value;

            _irCalibration.leftBounds = areaLeft.Value;
            _irCalibration.rightBounds = areaRight.Value;
            _irCalibration.bottomBounds = areaBottom.Value;
            _irCalibration.topBounds = areaTop.Value;
        }

        private void ScreenBehaviorChange(object sender, RoutedEventArgs e)
        {
            if (!set) return;

            if (radioCenter.IsChecked == true)
            {
                _irCalibration.offscreenBehavior = IRCamOffscreenBehavior.ReturnToCenter;
            }
            else
            {
                _irCalibration.offscreenBehavior = IRCamOffscreenBehavior.UseLastPoint;
            }
        }

        //private void jitterSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //
        //}

        private void visiblePoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!set) return;
            if (minVisiblePoints.SelectedIndex == 1)
            {
                _irCalibration.minVisiblePoints = IRCamMinimumVisiblePoints.One;
            }
            else
            {
                _irCalibration.minVisiblePoints = IRCamMinimumVisiblePoints.Two;
            }
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
            dialog.FileName = string.IsNullOrEmpty(FileName) ? "ir_Calibration" : FileName;
            dialog.DefaultExt = ".irc";
            dialog.Filter = App.IR_CAL_FILTER;

            bool? doLoad = dialog.ShowDialog();
            IRCalibration loadedConfig;

            if (doLoad == true && dialog.CheckFileExists)
            {
                if (App.LoadFromFile<IRCalibration>(dialog.FileName, out loadedConfig))
                {
                    FileName = dialog.FileName;
                    _irCalibration = loadedConfig;
                    boxWidth.Value = _irCalibration.deadzone.width;
                    boxHeight.Value = _irCalibration.deadzone.height;
                    boxX.Value = _irCalibration.deadzone.center_x;
                    boxY.Value = _irCalibration.deadzone.center_y;
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
            dialog.FileName = string.IsNullOrEmpty(FileName) ? "ir_Calibration" : FileName;
            dialog.DefaultExt = ".irc";
            dialog.Filter = App.IR_CAL_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                if (App.SaveToFile<IRCalibration>(dialog.FileName, _irCalibration))
                {
                    FileName = dialog.FileName;
                }
            }
        }
    }

    public struct IRCalibration
    {
        public SquareBoundry deadzone;
        public int leftBounds, rightBounds, topBounds, bottomBounds;
        public IRCamOffscreenBehavior offscreenBehavior;
        public IRCamMinimumVisiblePoints minVisiblePoints;
    }
}
