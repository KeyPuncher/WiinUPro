using System;
using System.Collections.Generic;
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
using NintrollerLib;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for JoyCalibrationWindow.xaml
    /// </summary>
    public partial class JoyCalibrationWindow : Window
    {
        public bool Cancelled { get; protected set; }
        public Joystick Calibration { get { return _joystick; } }
        public float AntiDeadzone { get; protected set; }

        protected short rawXLimitMax, rawXLimitMin, rawYLimitMax, rawYLimitMin;
        protected short rawXDeadMax, rawXDeadMin, rawYDeadMax, rawYDeadMin;
        
        protected Joystick _joystick;
        protected Joystick _default;

        public string[] Assignments { get; protected set; }
        
        public JoyCalibrationWindow(Joystick noneCalibration)
        {
            _default = noneCalibration;
            InitializeComponent();
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
        }

        public void Update(Joystick joy)
        {
            // Display Raw Value (no deadzone & expected limits)
            _default.rawX = joy.rawX;
            _default.rawY = joy.rawY;
            _default.Normalize();
            Canvas.SetLeft(raw, 500 * _default.X + 500 - raw.Width / 2);
            Canvas.SetTop(raw, -500 * _default.Y + 500 - raw.Height / 2);

            // Apply Limits
            joy.maxX = (int)Math.Round(_default.maxX * (limitXPos.Value / 100d));
            joy.minX = (int)Math.Round(_default.minX * (limitXNeg.Value / 100d));
            joy.maxY = (int)Math.Round(_default.maxY * (limitYPos.Value / 100d));
            joy.minY = (int)Math.Round(_default.minY * (limitYNeg.Value / 100d));
            
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

        private void antiDeadzoneSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            antiDeadzone.Height = antiDeadzone.Width = e.NewValue * 10;
            Canvas.SetTop(antiDeadzone, 500 - antiDeadzone.Height / 2);
            Canvas.SetLeft(antiDeadzone, 500 - antiDeadzone.Width / 2);
            antiDeadzoneLabel.Content = e.NewValue.ToString() + " %";
        }

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            AntiDeadzone = (float)antiDeadzoneSlider.Value / 10f;
            _joystick = new Joystick();
            _joystick.centerX = _default.centerX;
            _joystick.centerY = _default.centerY;
            _joystick.maxX = (int)Math.Round(_default.maxX * (limitXPos.Value / 100d));
            _joystick.minX = (int)Math.Round(_default.minX * (limitXNeg.Value / 100d));
            _joystick.maxY = (int)Math.Round(_default.maxY * (limitYPos.Value / 100d));
            _joystick.minY = (int)Math.Round(_default.minY * (limitYNeg.Value / 100d));
            _joystick.deadXp = (int)Math.Round((_default.maxX - _default.centerX) * (deadXPos.Value / 100d));
            _joystick.deadXn = -(int)Math.Round((_default.maxX - _default.centerX) * (deadXNeg.Value / 100d));
            _joystick.deadYp = (int)Math.Round((_default.maxY - _default.centerY) * (deadYPos.Value / 100d));
            _joystick.deadYn = -(int)Math.Round((_default.maxY - _default.centerY) * (deadYNeg.Value / 100d));

            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Cancelled = true;
            Close();
        }
    }
}
