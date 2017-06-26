using System;
using System.Windows;
using SharpDX.DirectInput;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for JoyConLControl.xaml
    /// </summary>
    public partial class JoyConLControl : BaseControl, IJoyControl
    {
        public Guid AssociatedInstanceID { get; set; }

        public JoyConLControl()
        {
            InitializeComponent();
        }

        public void UpdateVisual(JoystickUpdate[] updates)
        {
            foreach (var update in updates)
            {
                switch (update.Offset)
                {
                    case JoystickOffset.Buttons0: leftBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons1: downBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons2: upBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons3: rightBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons4: slBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons5: srBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons8: minusBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons10: joyStickButton.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons13: shareBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons14: lBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons15: zlBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.PointOfViewControllers0:
                        var margin = new Thickness(185, 599, 0, 0);
                        if (update.Value == -1)
                        {
                            // nothing
                        }
                        else if (update.Value == 0)
                        {
                            margin.Left += 30;
                        }
                        else if (update.Value > 0 && update.Value < 9000)
                        {
                            margin.Left += 20;
                            margin.Top += 20;
                        }
                        else if (update.Value == 9000)
                        {
                            margin.Top += 30;
                        }
                        else if (update.Value > 9000 && update.Value < 18000)
                        {
                            margin.Top += 20;
                            margin.Left -= 20;
                        }
                        else if (update.Value == 18000)
                        {
                            margin.Left -= 30;
                        }
                        else if (update.Value > 18000 && update.Value < 27000)
                        {
                            margin.Left -= 20;
                            margin.Top -= 20;
                        }
                        else if (update.Value == 27000)
                        {
                            margin.Top -= 30;
                        }
                        else if (update.Value > 27000)
                        {
                            margin.Top -= 20;
                            margin.Left += 20;
                        }

                        joyStick.Margin = margin;
                        joyStickButton.Margin = margin;
                        break;
                }
            }
        }
    }
}
