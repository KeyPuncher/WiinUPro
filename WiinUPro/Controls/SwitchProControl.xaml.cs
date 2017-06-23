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
using Shared;
using SharpDX.DirectInput;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for SwitchProControl.xaml
    /// </summary>
    public partial class SwitchProControl : BaseControl, IJoyControl
    {
        public event Delegates.StringDel OnInputRightClick;
        public event Delegates.StringDel OnInputSelected;
        public event AssignmentCollection.AssignDelegate OnQuickAssign;

        public Guid AssociatedInstanceID { get; set; }

        public SwitchProControl()
        {
            InitializeComponent();
        }

        public void UpdateVisual(JoystickUpdate[] updates)
        {
            foreach (var update in updates)
            {
                switch (update.Offset)
                {
                    case JoystickOffset.Buttons0: bBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons1: aBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons2: yBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons3: xBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons4: lBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons5: rBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons6: zlBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons7: zrBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons8: minusBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons9: plusBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons10: leftStickButton.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons11: rightStickButton.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons12: homeBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons13: shareBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.PointOfViewControllers0:
                        leftBtn.Opacity = 0;
                        rightBtn.Opacity = 0;
                        upBtn.Opacity = 0;
                        downBtn.Opacity = 0;
                        center.Opacity = 0;
                        if (update.Value == -1)
                        {
                            break;
                        }
                        else if (update.Value == 0)
                        {
                            upBtn.Opacity = 1;
                            center.Opacity = 1;
                        }
                        else if (update.Value > 0 && update.Value < 9000)
                        {
                            upBtn.Opacity = 1;
                            rightBtn.Opacity = 1;
                            center.Opacity = 1;
                        }
                        else if (update.Value == 9000)
                        {
                            rightBtn.Opacity = 1;
                            center.Opacity = 1;
                        }
                        else if (update.Value > 9000 && update.Value < 18000)
                        {
                            rightBtn.Opacity = 1;
                            downBtn.Opacity = 1;
                            center.Opacity = 1;
                        }
                        else if (update.Value == 18000)
                        {
                            downBtn.Opacity = 1;
                            center.Opacity = 1;
                        }
                        else if (update.Value > 18000 && update.Value < 27000)
                        {
                            downBtn.Opacity = 1;
                            leftBtn.Opacity = 1;
                            center.Opacity = 1;
                        }
                        else if (update.Value == 27000)
                        {
                            leftBtn.Opacity = 1;
                            center.Opacity = 1;
                        }
                        else if (update.Value > 27000)
                        {
                            leftBtn.Opacity = 1;
                            upBtn.Opacity = 1;
                            center.Opacity = 1;
                        }
                        break;
                    case JoystickOffset.X:
                        var tmpXL = new AxisCalibration(0, 65535, 32767, 256);
                        leftStick.Margin = new Thickness(146 + 30 * tmpXL.Normal(update.Value), leftStick.Margin.Top, 0, 0);
                        leftStickButton.Margin = leftStick.Margin;
                        break;
                    case JoystickOffset.Y:
                        var tmpYL = new AxisCalibration(0, 65535, 32767, 256);
                        leftStick.Margin = new Thickness(leftStick.Margin.Left, 291 + 30 * tmpYL.Normal(update.Value), 0, 0);
                        leftStickButton.Margin = leftStick.Margin;
                        break;
                    case JoystickOffset.RotationX:
                        var tmpXR = new AxisCalibration(0, 65535, 32767, 256);
                        rightStick.Margin = new Thickness(507 + 30 * tmpXR.Normal(update.Value), rightStick.Margin.Top, 0, 0);
                        rightStickButton.Margin = rightStick.Margin;
                        break;
                    case JoystickOffset.RotationY:
                        var tmpYR = new AxisCalibration(0, 65535, 32767, 256);
                        rightStick.Margin = new Thickness(rightStick.Margin.Left, 412 + 30 * tmpYR.Normal(update.Value), 0, 0);
                        rightStickButton.Margin = rightStick.Margin;
                        break;
                }
            }
        }
    }
}
