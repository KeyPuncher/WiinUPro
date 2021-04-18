using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Shared;
using NintrollerLib;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for ProControl.xaml
    /// </summary>
    public partial class ProControl : BaseControl, INintyControl
    {
        public event Delegates.BoolArrDel OnChangeLEDs;
        public event Delegates.JoystickDel OnJoyCalibrated;

        protected Windows.JoyCalibrationWindow _openJoyWindow = null;
        protected bool _rightJoyOpen = false;
        protected ProController _lastState;

        private ProControl()
        {
            InitializeComponent();
        }

        public ProControl(string deviceID) : this()
        {
            DeviceID = deviceID;
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
            if (state is ProController)
            {
                var pro = (ProController)state;
                _lastState = pro;

                Display(aBtn, pro.A);
                Display(bBtn, pro.B);
                Display(xBtn, pro.X);
                Display(yBtn, pro.Y);
                Display(lBtn, pro.L);
                Display(rBtn, pro.R);
                Display(zlBtn, pro.ZL);
                Display(zrBtn, pro.ZR);
                Display(dpadUp, pro.Up);
                Display(dpadDown, pro.Down);
                Display(dpadLeft, pro.Left);
                Display(dpadRight, pro.Right);
                Display(dpadCenter, (pro.Up || pro.Down || pro.Left || pro.Right));
                Display(homeBtn, pro.Home);
                Display(plusBtn, pro.Plus);
                Display(minusBtn, pro.Minus);
                Display(leftStickBtn, pro.LStick);
                Display(rightStickBtn, pro.RStick);

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

        public void SetInputTooltip(string inputName, string tooltip)
        {
            switch (inputName)
            {
                case INPUT_NAMES.PRO_CONTROLLER.A: aBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.B: bBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.X: xBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.Y: yBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.L: lBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.R: rBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.ZL: zlBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.ZR: zrBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.HOME: homeBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.START: plusBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.SELECT: minusBtn.ToolTip = tooltip; break;
                case INPUT_NAMES.PRO_CONTROLLER.LUP: UpdateTooltipLine(leftStickBtn, tooltip, 0); break;
                case INPUT_NAMES.PRO_CONTROLLER.LLEFT: UpdateTooltipLine(leftStickBtn, tooltip, 1); break;
                case INPUT_NAMES.PRO_CONTROLLER.LRIGHT: UpdateTooltipLine(leftStickBtn, tooltip, 2); break;
                case INPUT_NAMES.PRO_CONTROLLER.LDOWN: UpdateTooltipLine(leftStickBtn, tooltip, 3); break;
                case INPUT_NAMES.PRO_CONTROLLER.LS: UpdateTooltipLine(leftStickBtn, tooltip, 4); break;
                case INPUT_NAMES.PRO_CONTROLLER.RUP: UpdateTooltipLine(rightStickBtn, tooltip, 0); break;
                case INPUT_NAMES.PRO_CONTROLLER.RLEFT: UpdateTooltipLine(rightStickBtn, tooltip, 1); break;
                case INPUT_NAMES.PRO_CONTROLLER.RRIGHT: UpdateTooltipLine(rightStickBtn, tooltip, 2); break;
                case INPUT_NAMES.PRO_CONTROLLER.RDOWN: UpdateTooltipLine(rightStickBtn, tooltip, 3); break;
                case INPUT_NAMES.PRO_CONTROLLER.RS: UpdateTooltipLine(rightStickBtn, tooltip, 4); break;
                case INPUT_NAMES.PRO_CONTROLLER.UP: 
                    dpadUp.ToolTip = tooltip;
                    UpdateTooltipLine(dpadCenter, tooltip, 0);
                    break;
                case INPUT_NAMES.PRO_CONTROLLER.LEFT: 
                    dpadLeft.ToolTip = tooltip;
                    UpdateTooltipLine(dpadCenter, tooltip, 1); 
                    break;
                case INPUT_NAMES.PRO_CONTROLLER.RIGHT: 
                    dpadRight.ToolTip = tooltip;
                    UpdateTooltipLine(dpadCenter, tooltip, 2); 
                    break;
                case INPUT_NAMES.PRO_CONTROLLER.DOWN: 
                    dpadDown.ToolTip = tooltip;
                    UpdateTooltipLine(dpadCenter, tooltip, 3); 
                    break;
            }
        }

        public void ClearTooltips()
        {
            string unsetText = Globalization.Translate("Input_Unset");

            aBtn.ToolTip = unsetText;
            bBtn.ToolTip = unsetText;
            xBtn.ToolTip = unsetText;
            yBtn.ToolTip = unsetText;
            lBtn.ToolTip = unsetText;
            rBtn.ToolTip = unsetText;
            zlBtn.ToolTip = unsetText;
            zrBtn.ToolTip = unsetText;
            homeBtn.ToolTip = unsetText;
            plusBtn.ToolTip = unsetText;
            minusBtn.ToolTip = unsetText;
            dpadUp.ToolTip = unsetText;
            dpadLeft.ToolTip = unsetText;
            dpadRight.ToolTip = unsetText;
            dpadDown.ToolTip = unsetText;
            UpdateTooltipLine(leftStickBtn, unsetText, 0);
            UpdateTooltipLine(leftStickBtn, unsetText, 1);
            UpdateTooltipLine(leftStickBtn, unsetText, 2);
            UpdateTooltipLine(leftStickBtn, unsetText, 3);
            UpdateTooltipLine(leftStickBtn, unsetText, 4);
            UpdateTooltipLine(rightStickBtn, unsetText, 0);
            UpdateTooltipLine(rightStickBtn, unsetText, 1);
            UpdateTooltipLine(rightStickBtn, unsetText, 2);
            UpdateTooltipLine(rightStickBtn, unsetText, 3);
            UpdateTooltipLine(rightStickBtn, unsetText, 4);
            UpdateTooltipLine(dpadCenter, unsetText, 0);
            UpdateTooltipLine(dpadCenter, unsetText, 1);
            UpdateTooltipLine(dpadCenter, unsetText, 2);
            UpdateTooltipLine(dpadCenter, unsetText, 3);
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

            CallEvent_OnQuickAssign(defaults);
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
        
        private void quickXboxbtn_Click(object sender, RoutedEventArgs e)
        {
            XboxAssign();
        }

        protected override void CalibrateInput(string inputName)
        {
            _rightJoyOpen = inputName == "proR";

            string joyTarget = _rightJoyOpen ? App.CAL_PRO_RJOYSTICK : App.CAL_PRO_LJOYSTICK;
            
            var prefs = AppPrefs.Instance.GetDevicePreferences(DeviceID);
            string filename = "";
            prefs?.calibrationFiles.TryGetValue(joyTarget, out filename);

            Windows.JoyCalibrationWindow joyCal = new Windows.JoyCalibrationWindow(
                _rightJoyOpen ? Calibrations.None.ProControllerRaw.RJoy : Calibrations.None.ProControllerRaw.LJoy,
                _rightJoyOpen ? _lastState.RJoy : _lastState.LJoy,
                filename ?? "");
            _openJoyWindow = joyCal;

#if DEBUG
            // This will allow for the dummy device window to retain focus
            if (DeviceID.StartsWith("Dummy"))
            {
                joyCal.Closed += (obj, args) =>
                {
                    if (joyCal.Apply)
                    {
                        OnJoyCalibrated?.Invoke(joyCal.Calibration, joyTarget, joyCal.FileName);
                    }

                    _openJoyWindow = null;
                };

                joyCal.Show();
                return;
            }
#endif

            joyCal.ShowDialog();

            if (joyCal.Apply)
            {
                OnJoyCalibrated?.Invoke(joyCal.Calibration, joyTarget, joyCal.FileName);
            }

            _openJoyWindow = null;
            joyCal = null;
        }
    }
}
