using System;
using System.Windows;
using SharpDX.DirectInput;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for JoyConRControl.xaml
    /// </summary>
    public partial class JoyConRControl : BaseControl, IJoyControl
    {
        public Guid AssociatedInstanceID { get; set; }
        
        public JoyConRControl()
        {
            InitializeComponent();
        }

        public void UpdateVisual(JoystickUpdate[] updates)
        {
            foreach (var update in updates)
            {
                switch (update.Offset)
                {
                    case JoystickOffset.Buttons0: aBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons1: xBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons2: bBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons3: yBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons4: slBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons5: srBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons9: plusBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons11: joyStickButton.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons12: homeBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons14: rBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.Buttons15: zrBtn.Opacity = update.Value > 0 ? 1 : 0; break;
                    case JoystickOffset.PointOfViewControllers0:
                        var margin = new Thickness(476, 990, 0, 0);
                        if (update.Value == -1)
                        {
                            // nothing
                        }
                        else if (update.Value == 0)
                        {
                            margin.Left -= 30;
                        }
                        else if (update.Value > 0 && update.Value < 9000)
                        {
                            margin.Left -= 20;
                            margin.Top -= 20;
                        }
                        else if (update.Value == 9000)
                        {
                            margin.Top -= 30;
                        }
                        else if (update.Value > 9000 && update.Value < 18000)
                        {
                            margin.Top -= 20;
                            margin.Left += 20;
                        }
                        else if (update.Value == 18000)
                        {
                            margin.Left += 30;
                        }
                        else if (update.Value > 18000 && update.Value < 27000)
                        {
                            margin.Left += 20;
                            margin.Top += 20;
                        }
                        else if (update.Value == 27000)
                        {
                            margin.Top += 30;
                        }
                        else if (update.Value > 27000)
                        {
                            margin.Top += 20;
                            margin.Left -= 20;
                        }

                        joyStick.Margin = margin;
                        joyStickButton.Margin = margin;
                        break;
                }
            }
        }
    }
}
