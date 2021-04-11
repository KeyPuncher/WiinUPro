using Shared;
using System;
using System.Windows;
using System.Windows.Controls;
using NintrollerLib;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for JoyCalibrationWindow.xaml
    /// </summary>
    public partial class JoyCalibrationWindow : Window
    {
        public bool Apply { get; protected set; }
        public Joystick Calibration { get { return _joystick; } }
        public string FileName { get; protected set; }

        protected int rawXCenter, rawYCenter;
        protected short rawXLimitMax, rawXLimitMin, rawYLimitMax, rawYLimitMin;
        protected short rawXDeadMax, rawXDeadMin, rawYDeadMax, rawYDeadMin;
        
        protected Joystick _joystick;
        protected Joystick _default;

        public string[] Assignments { get; protected set; }
        
        public JoyCalibrationWindow(Joystick noneCalibration, Joystick prevCalibration, string filename = "")
        {
            _default = noneCalibration;
            FileName = filename;

            InitializeComponent();
            Set(prevCalibration);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Globalization.ApplyTranslations(this);
        }

        private void CenterXUpdated(int x)
        {
            rawXCenter = x;
        }
        
        private void CenterYUpdated(int y)
        {
            rawYCenter = y;
        }

        private void LimitsUpdated(int obj)
        {
            if (limitYPos == null || limitYNeg == null) return;
            if (limitXPos == null || limitXNeg == null) return;
            
            // Calculate Points
            var top    = 500 - limitYPos.Value * 5;
            var left   = 500 - limitXNeg.Value * 5;
            var bottom = 500 + limitYNeg.Value * 5;
            var right  = 500 + limitXPos.Value * 5;

            // Adjust Points of the circle
            limitQ1Arc.Point = new Point(500, top);
            limitQ2Arc.Point = new Point(left, 500);
            limitQ3Arc.Point = new Point(500, bottom);
            limitQ4Arc.Point = new Point(right, 500);
            limitQ1Path.StartPoint = new Point(right, 500);

            // Adjust Radii
            limitQ1Arc.Size = new Size(right - 500, 500 - top);
            limitQ2Arc.Size = new Size(500 - left, 500 - top);
            limitQ3Arc.Size = new Size(500 - left, bottom - 500);
            limitQ4Arc.Size = new Size(right - 500, bottom - 500);
        }

        private void DeadzoneUpdated(int ignore)
        {
            if (deadYPos == null || deadYNeg == null) return;
            if (deadXPos == null || deadXNeg == null) return;

            // Calculate Points
            var top    = 500 - Math.Max(deadYPos.Value, 1) * 5;
            var left   = 500 - Math.Max(deadXNeg.Value, 1) * 5;
            var bottom = 500 + Math.Max(deadYNeg.Value, 1) * 5;
            var right  = 500 + Math.Max(deadXPos.Value, 1) * 5;

            // Adjust Points of the circle
            deadQ1Arc.Point = new Point(500, top);
            deadQ2Arc.Point = new Point(left, 500);
            deadQ3Arc.Point = new Point(500, bottom);
            deadQ4Arc.Point = new Point(right, 500);
            deadQ1Path.StartPoint = new Point(right, 500);

            // Adjust Radii
            deadQ1Arc.Size = new Size(right - 500, 500 - top);
            deadQ2Arc.Size = new Size(500 - left, 500 - top);
            deadQ3Arc.Size = new Size(500 - left, bottom - 500);
            deadQ4Arc.Size = new Size(right - 500, bottom - 500);

            // Square
            rawDeadzoneSqr.Width = Math.Max((deadXPos.Value + deadXNeg.Value) * 5, 1);
            rawDeadzoneSqr.Height = Math.Max((deadYPos.Value + deadYNeg.Value) * 5, 1);
            rawDeadzoneSqr.Margin = new Thickness(600 - Math.Max(deadXNeg.Value * 5, 1), 600 - Math.Max(deadYPos.Value * 5, 1), 0, 0);
        }

        public void Update(Joystick joy)
        {
            // Display Raw Value (no deadzone & expected limits)
            Joystick rawDisplay = new Joystick();
            rawDisplay.Calibrate(_default);
            rawDisplay.rawX = joy.rawX;
            rawDisplay.rawY = joy.rawY;
            rawDisplay.centerX = _default.centerX + rawXCenter;
            rawDisplay.centerY = _default.centerY + rawYCenter;
            rawDisplay.Normalize();
            Canvas.SetLeft(raw, 500 * rawDisplay.X + 500 - raw.Width / 2);
            Canvas.SetTop(raw, -500 * rawDisplay.Y + 500 - raw.Height / 2);

            // Apply Center
            joy.centerX = _default.centerX + rawXCenter;
            joy.centerY = _default.centerY + rawYCenter;

            // Apply Limits
            joy.maxX = (int)Math.Round((_default.maxX - _default.centerX) * (limitXPos.Value / 100d)) + _default.centerX;
            joy.minX = _default.centerX - (int)Math.Round((_default.centerX - _default.minX) * (limitXNeg.Value / 100d));
            joy.maxY = (int)Math.Round((_default.maxY - _default.centerY) * (limitYPos.Value / 100d)) + _default.centerX;
            joy.minY = _default.centerY - (int)Math.Round((_default.centerY - _default.minY) * (limitYNeg.Value / 100d));
            
            // Apply Deadzone (not symetrical)
            joy.deadXp =  (int)Math.Round((_default.maxX - _default.centerX) * (deadXPos.Value / 100d));
            joy.deadXn = -(int)Math.Round((_default.maxX - _default.centerX) * (deadXNeg.Value / 100d));
            joy.deadYp =  (int)Math.Round((_default.maxY - _default.centerY) * (deadYPos.Value / 100d));
            joy.deadYn = -(int)Math.Round((_default.maxY - _default.centerY) * (deadYNeg.Value / 100d));
            joy.Normalize();

            // Apply Anti Deadzone
            var anti = (float)antiDeadzoneSlider.Value / 100f;
            if (joy.X != 0)
            {
                var xRange = 1f - anti;
                joy.X = joy.X * xRange + anti * Math.Sign(joy.X);
            }
            if (joy.Y != 0)
            {
                var yRange = 1f - anti;
                joy.Y = joy.Y * yRange + anti * Math.Sign(joy.Y);
            }
            
            
            // Display Output
            Canvas.SetTop(@out, joy.Y * -500 + 500 - @out.Height / 2);
            Canvas.SetLeft(@out, joy.X * 500 + 500 - @out.Width / 2);
            xLabel.Content = string.Format("X: {0}%", Math.Round(joy.X * 100));
            yLabel.Content = string.Format("Y: {0}%", Math.Round(joy.Y * 100));
        }

        public void Set(Joystick prevCalibration)
        {
            centerX.Value = prevCalibration.centerX - _default.centerX;
            centerY.Value = prevCalibration.centerY - _default.centerY;
            limitXPos.Value = (int)Math.Round((prevCalibration.maxX - _default.centerX) / (double)(_default.maxX - _default.centerX) * 100d);
            limitXNeg.Value = (int)Math.Round((_default.centerX - prevCalibration.minX) / (double)(_default.centerX - _default.minX) * 100d);
            limitYPos.Value = (int)Math.Round((prevCalibration.maxY - _default.centerY) / (double)(_default.maxY - _default.centerY) * 100d);
            limitYNeg.Value = (int)Math.Round((_default.centerY - prevCalibration.minY) / (double)(_default.centerY - _default.minY) * 100d);
            deadXPos.Value = (int)Math.Round(prevCalibration.deadXp / (double)(_default.maxX - _default.centerX) * 100d);
            deadXNeg.Value = -(int)Math.Round(prevCalibration.deadXn / (double)(_default.centerX - _default.minX) * 100d);
            deadYPos.Value = (int)Math.Round(prevCalibration.deadYp / (double)(_default.maxY - _default.centerY) * 100d);
            deadYNeg.Value = -(int)Math.Round(prevCalibration.deadYn / (double)(_default.centerY - _default.minX) * 100d);
            antiDeadzoneSlider.Value = Math.Round(prevCalibration.antiDeadzone * 10);
        }

        protected void Convert()
        {
            _joystick = new Joystick();
            _joystick.centerX = _default.centerX + rawXCenter;
            _joystick.centerY = _default.centerY + rawYCenter;
            _joystick.maxX = (int)Math.Round((_default.maxX - _default.centerX) * (limitXPos.Value / 100d) + _default.centerX);
            _joystick.minX = (int)Math.Round(_default.centerX - (_default.centerX - _default.minX) * (limitXNeg.Value / 100d));
            _joystick.maxY = (int)Math.Round((_default.maxY - _default.centerY) * (limitYPos.Value / 100d) + _default.centerY);
            _joystick.minY = (int)Math.Round(_default.centerY - (_default.centerY - _default.minY) * (limitYNeg.Value / 100d));
            _joystick.deadXp = (int)Math.Round((_default.maxX - _default.centerX) * (deadXPos.Value / 100d));
            _joystick.deadXn = -(int)Math.Round((_default.maxX - _default.centerX) * (deadXNeg.Value / 100d));
            _joystick.deadYp = (int)Math.Round((_default.maxY - _default.centerY) * (deadYPos.Value / 100d));
            _joystick.deadYn = -(int)Math.Round((_default.maxY - _default.centerY) * (deadYNeg.Value / 100d));
            _joystick.antiDeadzone = (float)antiDeadzoneSlider.Value / 10f;
        }

        private void antiDeadzoneSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            antiDeadzone.Height = antiDeadzone.Width = e.NewValue * 10;
            Canvas.SetTop(antiDeadzone, 500 - antiDeadzone.Height / 2);
            Canvas.SetLeft(antiDeadzone, 500 - antiDeadzone.Width / 2);
            antiDeadzoneLabel.Content = e.NewValue.ToString() + " %";
        }

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            Convert();
            Apply = true;
            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            Convert();

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = string.IsNullOrEmpty(FileName) ? "joystick_calibration" : FileName;
            dialog.DefaultExt = ".joy";
            dialog.Filter = App.JOY_CAL_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                if (App.SaveToFile<Joystick>(dialog.FileName, _joystick))
                {
                    FileName = dialog.FileName;
                }
            }
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = string.IsNullOrEmpty(FileName) ? "joystick_calibration" : FileName;
            dialog.DefaultExt = ".joy";
            dialog.Filter = App.JOY_CAL_FILTER;

            bool? doLoad = dialog.ShowDialog();
            Joystick loadedConfig;

            if (doLoad == true && dialog.CheckFileExists)
            {
                if (App.LoadFromFile<Joystick>(dialog.FileName, out loadedConfig))
                {
                    FileName = dialog.FileName;
                    Set(loadedConfig);
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
    }
}
