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
using Shared;
using NintrollerLib;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for ProControl.xaml
    /// </summary>
    public partial class ProControl : UserControl, INintyControl
    {
        public event Delegates.BoolArrDel OnChangeLEDs;
        public event Delegates.StringDel OnInputSelected;
        public event Delegates.StringDel OnInputRightClick;
        public event Delegates.JoystickeDel OnJoyCalibrated;
        public event AssignmentCollection.AssignDelegate OnQuickAssign;

        protected Windows.JoyCalibrationWindow _openJoyWindow = null;
        protected bool _rightJoyOpen = false;

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

                if (_openJoyWindow != null)
                {
                    _openJoyWindow.Update(_rightJoyOpen ? pro.RJoy : pro.LJoy);
                }
            }
        }

        public void ChangeLEDs(bool one, bool two, bool three, bool four)
        {
            led1.Opacity = one ? 1 : 0;
            led2.Opacity = two ? 1 : 0;
            led3.Opacity = three ? 1 : 0;
            led4.Opacity = four ? 1 : 0;
        }

        public void XboxAssign()
        {
            Dictionary<string, AssignmentCollection> defaults = new Dictionary<string, AssignmentCollection>();

            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.LUP, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LY_Hi) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.LDOWN, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LY_Lo) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.LRIGHT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LX_Hi) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.LLEFT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LX_Lo) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.RUP, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RY_Hi) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.RDOWN, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RY_Lo) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.RRIGHT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RX_Hi) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.RLEFT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RX_Lo) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.ZL, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LT) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.ZR, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RT) }));

            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.UP, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Up) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.DOWN, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Down) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.LEFT, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Left) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.RIGHT, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Right) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.A, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.A) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.B, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.B) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.X, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.X) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.Y, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Y) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.L, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.LB) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.R, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.RB) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.LS, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.LS) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.RS, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.RS) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.START, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Start) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.SELECT, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Back) }));
            defaults.Add(INPUT_NAMES.PRO_CONTROLLER.HOME, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Guide) }));

            if (OnQuickAssign != null)
                OnQuickAssign(defaults);
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

                OnChangeLEDs(leds);
            }
        }

        private void QuickAssign(string prefix, string type)
        {
            Dictionary<string, AssignmentCollection> args = new Dictionary<string, AssignmentCollection>();
            
            if (type == "Mouse")
            {
                args.Add(prefix + "UP", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveUp) }));
                args.Add(prefix + "DOWN", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveDown) }));
                args.Add(prefix + "LEFT", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveLeft) }));
                args.Add(prefix + "RIGHT", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveRight) }));
            }
            else if (type == "WASD")
            {
                args.Add(prefix + "UP", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_W) }));
                args.Add(prefix + "DOWN", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_S) }));
                args.Add(prefix + "LEFT", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_A) }));
                args.Add(prefix + "RIGHT", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_D) }));
            }
            else if (type == "Arrows")
            {
                args.Add(prefix + "UP", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_UP) }));
                args.Add(prefix + "DOWN", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_DOWN) }));
                args.Add(prefix + "LEFT", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_LEFT) }));
                args.Add(prefix + "RIGHT", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_RIGHT) }));
            }

            if (OnQuickAssign != null)
                OnQuickAssign(args);
        }

        private void OpenInput(object sender)
        {
            var element = sender as FrameworkElement;
            var tag = element == null ? "" : element.Tag as string;

            // Open input assignment window
            if (OnInputSelected != null && tag != null)
            {
                OnInputSelected(tag);
            }
        }

        private void Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                OpenInput(sender);
            }
        }

        private void Btn_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            var tag = element == null ? "" : element.Tag as string;

            // Open Context menu
            if (OnInputRightClick != null && tag != null)
            {
                OnInputRightClick(tag);
            }
        }

        private void OpenContextMenu(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;

            if (element != null && element.ContextMenu != null)
            {
                element.ContextMenu.IsOpen = true;
            }
        }

        private void Axis_Click(object sender, RoutedEventArgs e)
        {
            OpenInput(sender);
        }

        private void QuickAssign_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var header = item.Header as string;
                var tag = item.Tag as string;

                if (header != null && tag != null)
                {
                    QuickAssign(tag, header);
                }
            }
        }

        private void quickXboxbtn_Click(object sender, RoutedEventArgs e)
        {
            XboxAssign();
        }

        private void Calibrate_Click(object sender, RoutedEventArgs e)
        {
            _rightJoyOpen = (sender as FrameworkElement).Tag.Equals("JoyR");

            Windows.JoyCalibrationWindow joyCal = new Windows.JoyCalibrationWindow(_rightJoyOpen ? 
                Calibrations.None.ProControllerRaw.RJoy :
                Calibrations.None.ProControllerRaw.LJoy);
            _openJoyWindow = joyCal;
            joyCal.ShowDialog();

            if (!joyCal.Cancelled)
            {
                OnJoyCalibrated?.Invoke(joyCal.Calibration, _rightJoyOpen);
            }

            _openJoyWindow = null;
        }
    }
}
