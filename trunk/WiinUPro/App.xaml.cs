using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal const string PROFILE_FILTER = "WiinUPro Profile|*.wup";
        internal const string JOY_CAL_FILTER = "Joystick Calibration|*.joy";
        internal const string TRIG_CAL_FILTER = "Trigger Calibration|*.trg";
        internal const string AXIS_CAL_FILTER = "Axis Calibration|*.axs";
        internal const string IR_CAL_FILTER = "IR Calibration|*.irc";
    }
}
