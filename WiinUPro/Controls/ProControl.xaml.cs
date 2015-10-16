using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for ProControl.xaml
    /// </summary>
    public partial class ProControl : UserControl, INintyControl
    {
        public event EventHandler<bool[]> OnChangeLEDs;
        public event EventHandler<string> OnInputSelected;
        public event EventHandler<string> OnInputRightClick;

        public ProControl()
        {
            InitializeComponent();
        }

        public void ApplyInput(INintrollerState state)
        {
            ProController? pro = state as ProController?;

            if (pro != null && pro.HasValue)
            {
                //pro.Value.GetValue
            }
        }

        public void UpdateVisual(INintrollerState state)
        {
            if (state.GetType() == typeof(ProController))
            {
                var pro = (ProController)state;

                aBtn.Opacity = pro.A ? 1 : 0;
                bBtn.Opacity = pro.B ? 1 : 0;
                xBtn.Opacity = pro.X ? 1 : 0;
                yBtn.Opacity = pro.Y ? 1 : 0;
                lBtn.Opacity = pro.L ? 1 : 0;
                rBtn.Opacity = pro.R ? 1 : 0;
                zlBtn.Opacity = pro.ZL ? 1 : 0;
                zrBtn.Opacity = pro.ZR ? 1 : 0;
                dpadUp.Opacity = pro.Up ? 1 : 0;
                dpadDown.Opacity = pro.Down ? 1 : 0;
                dpadLeft.Opacity = pro.Left ? 1 : 0;
                dpadRight.Opacity = pro.Right ? 1 : 0;
                dpadCenter.Opacity = (pro.Up || pro.Down || pro.Left || pro.Right) ? 1 : 0;
                homeBtn.Opacity = pro.Home ? 1 : 0;
                plusBtn.Opacity = pro.Plus ? 1 : 0;
                minusBtn.Opacity = pro.Minus ? 1 : 0;
                leftStickBtn.Opacity = pro.LStick ? 1 : 0;
                rightStickBtn.Opacity = pro.RStick ? 1 : 0;

                leftStick.Margin = new Thickness(196 + 50 * pro.LJoy.X, 232 - 50 * pro.LJoy.Y, 0, 0);
                leftStickBtn.Margin = new Thickness(196 + 50 * pro.LJoy.X, 230 - 50 * pro.LJoy.Y, 0, 0);
                rightStick.Margin = new Thickness(980 + 50 * pro.RJoy.X, 232 - 50 * pro.RJoy.Y, 0, 0);
                rightStickBtn.Margin = new Thickness(980 + 50 * pro.RJoy.X, 230 - 50 * pro.RJoy.Y, 0, 0);
            }
        }

        public void ChangeLEDs(bool one, bool two, bool three, bool four)
        {
            led1.Opacity = one ? 1 : 0;
            led2.Opacity = two ? 1 : 0;
            led3.Opacity = three ? 1 : 0;
            led4.Opacity = four ? 1 : 0;
        }

        private void led_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var light = sender as Image;
            if (light != null)
            {
                light.Opacity = light.Opacity > 0.1 ? 0 : 1;
            }

            if (OnChangeLEDs != null)
            {
                bool[] leds = new bool[4];
                leds[0] = led1.Opacity > 0.1;
                leds[1] = led2.Opacity > 0.1;
                leds[2] = led3.Opacity > 0.1;
                leds[3] = led4.Opacity > 0.1;

                OnChangeLEDs(this, leds);
            }
        }

        private void Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var element = sender as FrameworkElement;
                var tag = element == null ? "" : element.Tag as string;

                // Open input assignment window
                if (OnInputSelected != null && tag != null)
                {
                    OnInputSelected(sender, tag);
                }
            }
        }

        private void Btn_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            var tag = element == null ? "" : element.Tag as string;

            // Open Context menu
            if (OnInputRightClick != null && tag != null)
            {
                OnInputRightClick(sender, tag);
            }
        }
    }
}
