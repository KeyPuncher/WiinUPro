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

        public JoyCalibrationWindow()
        {
            InitializeComponent();
        }
    }
}
