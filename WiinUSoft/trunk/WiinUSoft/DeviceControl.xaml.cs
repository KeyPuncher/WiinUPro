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
using System.Windows.Navigation;
using System.Windows.Shapes;

using NintrollerLib;

namespace WiinUSoft
{
    public partial class DeviceControl : UserControl
    {
        private Nintroller device;
        internal Nintroller Device
        {
            get { return device; }
            set
            {
                device = value;

                if (device != null)
                {
                    device.ExtensionChange += device_ExtensionChange;
                    device.StateChange += device_StateChange;
                }
            }
        }
        internal bool Connected
        {
            get
            {
                if (device == null)
                    return false;

                return device.Connected;
            }
        }

        private ControllerType deviceType = ControllerType.Wiimote;
        internal ControllerType DeviceType { get; private set; }

        public DeviceControl()
        {
            InitializeComponent();
        }

        public DeviceControl(Nintroller nintroller) : this()
        {
            Device = nintroller;
        }

        void device_ExtensionChange(object sender, ExtensionChangeEventArgs e)
        {
            DeviceType = e.Extension;
        }

        void device_StateChange(object sender, StateChangeEventArgs e)
        {
            throw new NotImplementedException();
            
            if (DeviceType == ControllerType.ProController)
            {
                // TODO: Pro Controller Reading
            }
            else if (DeviceType == ControllerType.BalanceBoard)
            {
                // TODO: Balance Board Reading
            }
            else
            {
                // TODO: Wiimote & extension Reading
                switch (DeviceType)
                {
                    case ControllerType.Nunchuk:
                    case ControllerType.NunchukB:
                        // TODO: Nunchuck Reading
                        break;

                    case ControllerType.ClassicController:
                        // TODO: Classic Controller Reading
                        break;

                    case ControllerType.ClassicControllerPro:
                        // TODO: Classic Controller Pro Reading
                        break;
                        
                    case ControllerType.MotionPlus:
                        // TODO: Motion Plus Reading
                        break;

                        // TODO: Musical Extension readins
                }
            }
        }
    }
}
