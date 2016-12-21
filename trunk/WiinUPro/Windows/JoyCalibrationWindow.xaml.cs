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
            var top    = 500 - deadYPos.Value * 5;
            var left   = 500 - deadXNeg.Value * 5;
            var bottom = 500 + deadYNeg.Value * 5;
            var right  = 500 + deadXPos.Value * 5;

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

        private void antiDeadzoneSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            antiDeadzone.Height = antiDeadzone.Width = e.NewValue * 10;
            Canvas.SetTop(antiDeadzone, 500 - antiDeadzone.Height / 2);
            Canvas.SetLeft(antiDeadzone, 500 - antiDeadzone.Width / 2);
            antiDeadzoneLabel.Content = e.NewValue.ToString() + " %";
        }

        public void Update(INintrollerState stateUpdate)
        {
            // TODO: Update the visual
        }

        public void Update(Joystick joy)
        {
            var rawY = joy.rawY / (double)(_default.maxY - _default.minY) * 1000;
            var rawX = joy.rawX / (double)(_default.maxX - _default.minX) * 1000;
            Canvas.SetTop(raw, 1500 - rawY - raw.Height / 2);
            Canvas.SetLeft(raw, rawX - 500 - raw.Width / 2);

            // TODO: calculate out (anti-deadzone)
            joy.maxX = (int)Math.Round(_default.maxX * (limitXPos.Value / 100d));
            joy.minX = (int)Math.Round(_default.minX * (limitXNeg.Value / 100d));
            joy.maxY = (int)Math.Round(_default.maxY * (limitYPos.Value / 100d));
            joy.minY = (int)Math.Round(_default.minY * (limitYNeg.Value / 100d));
            // deadzones are not symetrical
            //joy.deadX = (int)Math.Round((_default.maxX - _default.minX) * (deadXPos.Value/* - deadXNeg.Value*/) / 100d);
            //joy.deadY = (int)Math.Round((_default.maxY - _default.minY) * (deadYPos.Value/* - deadYNeg.Value*/) / 100d);

            // TODO: weirdness as deadzone value increases, not working?
            joy.deadXp = (int)Math.Round((_default.maxX - _default.minX) * (deadXPos.Value / 100d));
            joy.deadXn = (int)Math.Round((_default.maxX - _default.minX) * (deadXNeg.Value / 100d));
            joy.deadYp = (int)Math.Round((_default.maxY - _default.minY) * (deadYPos.Value / 100d));
            joy.deadYn = (int)Math.Round((_default.maxY - _default.minY) * (deadYNeg.Value / 100d));
            joy.Normalize();

            //var deadX = (int)Math.Round((_default.maxX - _default.minX) * (deadXPos.Value - deadXNeg.Value) / 100d);
            //var deadY = (int)Math.Round((_default.maxY - _default.minY) * (deadYPos.Value - deadYNeg.Value) / 100d);
            //System.Diagnostics.Debug.WriteLine(deadX);
            
            Canvas.SetTop(@out, joy.Y * -500 + 500 - @out.Height / 2);
            Canvas.SetLeft(@out, joy.X * 500 + 500 - @out.Width / 2);
        }
    }
}
