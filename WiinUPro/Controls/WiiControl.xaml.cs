using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NintrollerLib;
using Shared;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for WiiControl.xaml
    /// </summary>
    public partial class WiiControl : BaseControl, INintyControl
    {
        public event Delegates.BoolArrDel OnChangeLEDs;
        public event Action<IRCamMode> OnChangeCameraMode;
        public event Action<IRCamSensitivity> OnChangeCameraSensitivty;
        public event Delegates.JoystickDel OnJoystickCalibrated;
        public event Delegates.TriggerDel OnTriggerCalibrated;
        public event Windows.IRCalibrationWindow.IRCalibrationDel OnIRCalibrated;

        protected string _calibrationTarget;
        protected Windows.JoyCalibrationWindow _openJoyWindow;
        protected Windows.TriggerCalibrationWindow _openTrigWindow;
        protected Windows.IRCalibrationWindow _openIRWindow;
        protected INintrollerState _lastState;

        private WiiControl()
        {
            InitializeComponent();
        }

        public WiiControl(string deviceID) : this()
        {
            DeviceID = deviceID;
        }

        public void ApplyInput(INintrollerState state)
        {
            // Do something
        }

        public void UpdateVisual(INintrollerState state)
        {
            _lastState = state;
            if (state is Wiimote)
            {
                if (viewNunchuk.Visibility == Visibility.Visible) viewNunchuk.Visibility = Visibility.Collapsed;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;
                if (viewGuitar.Visibility == Visibility.Visible) viewGuitar.Visibility = Visibility.Collapsed;
                if (viewTaikoDrum.Visibility == Visibility.Visible) viewTaikoDrum.Visibility = Visibility.Collapsed;

                UpdateWiimoteVisual((Wiimote)state);
            }
            else if (state is Nunchuk)
            {
                if (viewNunchuk.Visibility != Visibility.Visible) viewNunchuk.Visibility = Visibility.Visible;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;
                if (viewGuitar.Visibility == Visibility.Visible) viewGuitar.Visibility = Visibility.Collapsed;
                if (viewTaikoDrum.Visibility == Visibility.Visible) viewTaikoDrum.Visibility = Visibility.Collapsed;

                var nun = (Nunchuk)state;
                UpdateWiimoteVisual(nun.wiimote);

                Display(nBtnC, nun.C);
                Display(nBtnZ, nun.Z);
                nJoy.Margin = new Thickness(70 + 30 * nun.joystick.X, 360 - 30 * nun.joystick.Y, 0, 0);

                if (_openJoyWindow != null && _calibrationTarget == App.CAL_NUN_JOYSTICK)
                {
                    _openJoyWindow.Update(nun.joystick);
                }
            }
            else if (state is ClassicController)
            {
                if (viewNunchuk.Visibility == Visibility.Visible) viewNunchuk.Visibility = Visibility.Collapsed;
                if (viewClassicController.Visibility != Visibility.Visible) viewClassicController.Visibility = Visibility.Visible;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;
                if (viewGuitar.Visibility == Visibility.Visible) viewGuitar.Visibility = Visibility.Collapsed;
                if (viewTaikoDrum.Visibility == Visibility.Visible) viewTaikoDrum.Visibility = Visibility.Collapsed;

                var cc = (ClassicController)state;
                UpdateWiimoteVisual(cc.wiimote);

                Display(ccBtnA, cc.A);
                Display(ccBtnB, cc.B);
                Display(ccBtnX, cc.X);
                Display(ccBtnY, cc.Y);
                Display(ccBtnUp, cc.Up);
                Display(ccBtnDown, cc.Down);
                Display(ccBtnRight, cc.Right);
                Display(ccBtnLeft, cc.Left);
                ccPadCenter.Opacity = cc.Up || cc.Down || cc.Left || cc.Right ? 1 : 0;
                Display(ccBtnHome, cc.Home);
                Display(ccBtnSelect, cc.Select);
                Display(ccBtnStart, cc.Start);
                Display(ccBtnZL, cc.ZL);
                Display(ccBtnZR, cc.ZR);
                Display(ccL, cc.L.value > 0);
                Display(ccR, cc.R.value > 0);
                ccLeftStick.Margin  = new Thickness(208 + 30 * cc.LJoy.X, 210 - 30 * cc.LJoy.Y, 0, 0);
                ccRightStick.Margin = new Thickness(364 + 30 * cc.RJoy.X, 210 - 30 * cc.RJoy.Y, 0, 0);

                if (_openJoyWindow != null && _calibrationTarget.StartsWith("cc"))
                {
                    if (_calibrationTarget == App.CAL_CC_LJOYSTICK) _openJoyWindow.Update(cc.LJoy);
                    else if (_calibrationTarget == App.CAL_CC_RJOYSTICK) _openJoyWindow.Update(cc.RJoy);
                }
                else if (_openTrigWindow != null && _calibrationTarget.StartsWith("cc"))
                {
                    if (_calibrationTarget == "ccLT") _openTrigWindow.Update(cc.L);
                    else if (_calibrationTarget == "ccRT") _openTrigWindow.Update(cc.R);
                }
            }
            else if (state is ClassicControllerPro)
            {
                if (viewNunchuk.Visibility == Visibility.Visible) viewNunchuk.Visibility = Visibility.Collapsed;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility != Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Visible;
                if (viewGuitar.Visibility == Visibility.Visible) viewGuitar.Visibility = Visibility.Collapsed;
                if (viewTaikoDrum.Visibility == Visibility.Visible) viewTaikoDrum.Visibility = Visibility.Collapsed;

                var ccp = (ClassicControllerPro)state;
                UpdateWiimoteVisual(ccp.wiimote);

                Display(ccpBtnA, ccp.A);
                Display(ccpBtnB, ccp.B);
                Display(ccpBtnX, ccp.X);
                Display(ccpBtnY, ccp.Y);
                Display(ccpBtnUp, ccp.Up);
                Display(ccpBtnDown, ccp.Down);
                Display(ccpBtnRight, ccp.Right);
                Display(ccpBtnLeft, ccp.Left);
                ccpPadCenter.Opacity = ccp.Up || ccp.Down || ccp.Left || ccp.Right ? 1 : 0;
                Display(ccpBtnHome, ccp.Home);
                Display(ccpBtnSelect, ccp.Select);
                Display(ccpBtnStart, ccp.Start);
                Display(ccpBtnZL, ccp.ZL);
                Display(ccpBtnZR, ccp.ZR);
                Display(ccpBtnL, ccp.L);
                Display(ccpBtnR, ccp.R);
                ccpLeftStick.Margin  = new Thickness(255 + 30 * ccp.LJoy.X, 279 - 30 * ccp.LJoy.Y, 0, 0);
                ccpRightStick.Margin = new Thickness(485 + 30 * ccp.RJoy.X, 279 - 30 * ccp.RJoy.Y, 0, 0);

                if (_openJoyWindow != null && _calibrationTarget.StartsWith("ccp"))
                {
                    if (_calibrationTarget == App.CAL_CCP_LJOYSTICK) _openJoyWindow.Update(ccp.LJoy);
                    else if (_calibrationTarget == App.CAL_CCP_RJOYSTICK) _openJoyWindow.Update(ccp.RJoy);
                }
            }
            else if (state is Guitar)
            {
                if (viewNunchuk.Visibility == Visibility.Visible) viewNunchuk.Visibility = Visibility.Collapsed;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;
                if (viewGuitar.Visibility != Visibility.Visible) viewGuitar.Visibility = Visibility.Visible;
                if (viewTaikoDrum.Visibility == Visibility.Visible) viewTaikoDrum.Visibility = Visibility.Collapsed;

                var gut = (Guitar)state;
                UpdateWiimoteVisual(gut.wiimote);

                Display(gBtnGreen, gut.Green);
                Display(gBtnRed, gut.Red);
                Display(gBtnYellow, gut.Yellow);
                Display(gBtnBlue, gut.Blue);
                Display(gBtnOrange, gut.Orange);
                Display(gBtnStrumUp, gut.StrumUp);
                Display(gBtnStrumDown, gut.StrumDown);
                Display(gBtnPlus, gut.Plus);
                Display(gBtnMinus, gut.Minus);
                Display(gTouch1, gut.T1);
                Display(gTouch2, gut.T2);
                Display(gTouch3, gut.T3);
                Display(gTouch4, gut.T4);
                Display(gTouch5, gut.T5);
                gStick.Margin = new Thickness(1236 + 30 * gut.joystick.X, 283 - 30 * gut.joystick.Y, 0, 0);
                gStick.Margin = new Thickness(1236 + 30 * gut.joystick.X, 283 - 30 * gut.joystick.Y, 0, 0);
                gWhammy.Margin = new Thickness(345 + 60 * gut.whammyBar.value, 815 - 20 * gut.whammyBar.value, 0, 0);

                if (_openJoyWindow != null && _calibrationTarget == App.CAL_GUT_JOYSTICK)
                {
                    _openJoyWindow.Update(gut.joystick);
                }
                else if (_openTrigWindow != null && _calibrationTarget == App.CAL_GUT_WHAMMY)
                {
                    _openTrigWindow.Update(gut.whammyBar);
                }
            }
            else if (state is TaikoDrum)
            {
                if (viewNunchuk.Visibility == Visibility.Visible) viewNunchuk.Visibility = Visibility.Collapsed;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;
                if (viewGuitar.Visibility == Visibility.Visible) viewGuitar.Visibility = Visibility.Collapsed;
                if (viewTaikoDrum.Visibility != Visibility.Visible) viewTaikoDrum.Visibility = Visibility.Visible;

                var tak = (TaikoDrum)state;
                UpdateWiimoteVisual(tak.wiimote);

                takL.Fill = tak.centerLeft ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.White;
                takR.Fill = tak.centerRight ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.White;
                takRimL.Stroke = tak.rimLeft ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.White;
                takRimR.Stroke = tak.rimRight ? System.Windows.Media.Brushes.Blue : System.Windows.Media.Brushes.White;
            }
        }

        public void ChangeLEDs(bool one, bool two, bool three, bool four)
        {
            // Update
        }

        public void XboxAssign(ScpDirector.XInput_Device device = ScpDirector.XInput_Device.Device_A)
        {
            Dictionary<string, AssignmentCollection> defaults = new Dictionary<string, AssignmentCollection>();

            // Depends on what extension is connected
            // IR
            //defaults.Add(INPUT_NAMES.WIIMOTE.UP,    new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RY_Hi, device) }));
            //defaults.Add(INPUT_NAMES.WIIMOTE.RDOWN,  new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RY_Lo, device) }));
            //defaults.Add(INPUT_NAMES.WIIMOTE.RRIGHT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RX_Hi, device) }));
            //defaults.Add(INPUT_NAMES.WIIMOTE.RLEFT,  new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RX_Lo, device) }));

            defaults.Add(INPUT_NAMES.WIIMOTE.UP,     new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Up, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.DOWN,   new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Down, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.LEFT,   new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Left, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.RIGHT,  new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Right, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.A,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.A, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.B,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.B, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.MINUS,  new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Start, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.PLUS, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Back, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.HOME,   new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Guide, device) }));

            defaults.Add(INPUT_NAMES.NUNCHUK.UP,    new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LY_Hi, device) }));
            defaults.Add(INPUT_NAMES.NUNCHUK.DOWN,  new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LY_Lo, device) }));
            defaults.Add(INPUT_NAMES.NUNCHUK.RIGHT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LX_Hi, device) }));
            defaults.Add(INPUT_NAMES.NUNCHUK.LEFT,  new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LX_Lo, device) }));
            
            defaults.Add(INPUT_NAMES.WIIMOTE.ONE,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.X, device) }));
            defaults.Add(INPUT_NAMES.WIIMOTE.TWO,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Y, device) }));
            defaults.Add(INPUT_NAMES.NUNCHUK.C,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.LB, device) }));
            defaults.Add(INPUT_NAMES.NUNCHUK.Z,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.RB, device) }));
            //defaults.Add(INPUT_NAMES.NUNCHUK.ZL,     new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LT, device) }));
            //defaults.Add(INPUT_NAMES.NUNCHUK.ZR,     new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RT, device) }));
            //defaults.Add(INPUT_NAMES.WIIMOTE.LS,     new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.LS, device) }));
            //defaults.Add(INPUT_NAMES.WIIMOTE.RS,     new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.RS, device) }));

            #region Classic Controller
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.LUP,    new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LY_Hi, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.LDOWN,  new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LY_Lo, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.LRIGHT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LX_Hi, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.LLEFT,  new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LX_Lo, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.RUP,    new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RY_Hi, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.RDOWN,  new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RY_Lo, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.RRIGHT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RX_Hi, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.RLEFT,  new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RX_Lo, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.LT,     new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LT, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.RT,     new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RT, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.UP,     new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Up, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.DOWN,   new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Down, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.LEFT,   new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Left, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.RIGHT,  new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Right, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.A,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.A, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.B,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.B, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.X,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.X, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.Y,      new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Y, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.ZL,     new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.LB, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.ZR,     new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.RB, device) }));
            //defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.LFULL,  new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.LS, device) }));
            //defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.RFULL,  new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.RS, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.START,  new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Start, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.SELECT, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Back, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER.HOME,   new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Guide, device) }));
            #endregion

            #region Classic Controller Pro
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LUP, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LY_Hi, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LDOWN, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LY_Lo, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LRIGHT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LX_Hi, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LLEFT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LX_Lo, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RUP, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RY_Hi, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RDOWN, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RY_Lo, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RRIGHT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RX_Hi, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RLEFT, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RX_Lo, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.ZL, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.LT, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.ZR, new AssignmentCollection(new List<IAssignment> { new XInputAxisAssignment(ScpControl.X360Axis.RT, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.UP, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Up, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.DOWN, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Down, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LEFT, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Left, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RIGHT, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Right, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.A, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.A, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.B, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.B, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.X, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.X, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.Y, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Y, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.L, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.LB, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.R, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.RB, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.START, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Start, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.SELECT, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Back, device) }));
            defaults.Add(INPUT_NAMES.CLASSIC_CONTROLLER_PRO.HOME, new AssignmentCollection(new List<IAssignment> { new XInputButtonAssignment(ScpControl.X360Button.Guide, device) }));
            #endregion
            
            CallEvent_OnQuickAssign(defaults);
        }

        private void UpdateWiimoteVisual(Wiimote wiimote)
        {
            if (_openIRWindow != null)
            {
                _openIRWindow.Update(wiimote.irSensor);
            }

            Display(wBtnA, wiimote.buttons.A);
            Display(wBtnB, wiimote.buttons.B);
            Display(wBtnOne, wiimote.buttons.One);
            Display(wBtnTwo, wiimote.buttons.Two);
            Display(wBtnUp, wiimote.buttons.Up);
            Display(wBtnRight, wiimote.buttons.Right);
            Display(wBtnDown, wiimote.buttons.Down);
            Display(wBtnLeft, wiimote.buttons.Left);
            Display(wBtnMinus, wiimote.buttons.Minus);
            Display(wBtnPlus, wiimote.buttons.Plus);
            Display(wBtnHome, wiimote.buttons.Home);
            wCenterPad.Opacity = wiimote.buttons.Up || wiimote.buttons.Down || wiimote.buttons.Left || wiimote.buttons.Right ? 1 : 0;

            irPoint1.Visibility = wiimote.irSensor.point1.visible ? Visibility.Visible : Visibility.Collapsed;
            irPoint2.Visibility = wiimote.irSensor.point2.visible ? Visibility.Visible : Visibility.Collapsed;
            irPoint3.Visibility = wiimote.irSensor.point3.visible ? Visibility.Visible : Visibility.Collapsed;
            irPoint4.Visibility = wiimote.irSensor.point4.visible ? Visibility.Visible : Visibility.Collapsed;

            if (wiimote.irSensor.point1.visible)
            {
                Canvas.SetLeft(irPoint1, wiimote.irSensor.point1.rawX / 1023f * irCanvas.Width);
                Canvas.SetTop(irPoint1, wiimote.irSensor.point1.rawY / 1023f * irCanvas.Height);
            }
            if (wiimote.irSensor.point2.visible)
            {
                Canvas.SetLeft(irPoint2, wiimote.irSensor.point2.rawX / 1023f * irCanvas.Width);
                Canvas.SetTop(irPoint2, wiimote.irSensor.point2.rawY / 1023f * irCanvas.Height);
            }
            if (wiimote.irSensor.point3.visible)
            {
                Canvas.SetLeft(irPoint3, wiimote.irSensor.point3.rawX / 1023f * irCanvas.Width);
                Canvas.SetTop(irPoint3, wiimote.irSensor.point3.rawY / 1023f * irCanvas.Height);
            }
            if (wiimote.irSensor.point4.visible)
            {
                Canvas.SetLeft(irPoint4, wiimote.irSensor.point4.rawX / 1023f * irCanvas.Width);
                Canvas.SetTop(irPoint4, wiimote.irSensor.point4.rawY / 1023f * irCanvas.Height);
            }
        }

        protected override void CallEvent_OnQuickAssign(Dictionary<string, AssignmentCollection> collection)
        {
            // If we are quick assigning IR input, remove any conflixing assignment first
            bool doRemove = false;
            foreach (var key in collection.Keys)
            {
                if (key.StartsWith("wIR"))
                {
                    doRemove = true;
                    break;
                }
            }

            if (doRemove)
            {
                CallEvent_OnRemoveInputs(new string[] 
                {
                    INPUT_NAMES.WIIMOTE.IR_X,
                    INPUT_NAMES.WIIMOTE.IR_Y,
                    INPUT_NAMES.WIIMOTE.IR_UP,
                    INPUT_NAMES.WIIMOTE.IR_DOWN,
                    INPUT_NAMES.WIIMOTE.IR_LEFT,
                    INPUT_NAMES.WIIMOTE.IR_RIGHT
                });
            }

            base.CallEvent_OnQuickAssign(collection);
        }

        private void QuickAssignMouseAbsolute_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, AssignmentCollection> args = new Dictionary<string, AssignmentCollection>();
            args.Add(INPUT_NAMES.WIIMOTE.IR_X, new AssignmentCollection(new List<IAssignment> { new MouseAbsoluteAssignment(MousePosition.X) }));
            args.Add(INPUT_NAMES.WIIMOTE.IR_Y, new AssignmentCollection(new List<IAssignment> { new MouseAbsoluteAssignment(MousePosition.Y) }));
            CallEvent_OnQuickAssign(args);
        }

        protected override void SetIRCamMode_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var header = item.Header as string;

                if (header != null)
                {
                    IRCamMode camMode = IRCamMode.Off;
                    if (Enum.TryParse(header, out camMode))
                    {
                        OnChangeCameraMode?.Invoke(camMode);
                    }
                }
            }
        }

        protected override void SetIRCamSensitivity_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var header = item.Header as string;

                if (header != null)
                {
                    IRCamSensitivity camSen = IRCamSensitivity.Level3;
                    if (Enum.TryParse(header.Replace(" ", ""), out camSen))
                    {
                        OnChangeCameraSensitivty?.Invoke(camSen);
                    }
                }
            }
        }

        protected override void CalibrateInput(string inputName)
        {
            _calibrationTarget = inputName;

            switch (inputName)
            {
                case App.CAL_WII_IR:
                    CalibrateIR();
                    break;

                // Joysticks
                case App.CAL_NUN_JOYSTICK:
                    CalibrateJoystick(Calibrations.None.NunchukRaw.joystick, (_lastState as Nunchuk?)?.joystick);
                    break;
                case App.CAL_CC_LJOYSTICK:
                    CalibrateJoystick(Calibrations.None.ClassicControllerRaw.LJoy, (_lastState as ClassicController?)?.LJoy);
                    break;
                case App.CAL_CC_RJOYSTICK:
                    CalibrateJoystick(Calibrations.None.ClassicControllerRaw.RJoy, (_lastState as ClassicController?)?.RJoy);
                    break;
                case App.CAL_CCP_LJOYSTICK:
                    CalibrateJoystick(Calibrations.None.ClassicControllerProRaw.LJoy, (_lastState as ClassicControllerPro?)?.LJoy);
                    break;
                case App.CAL_CCP_RJOYSTICK:
                    CalibrateJoystick(Calibrations.None.ClassicControllerProRaw.RJoy, (_lastState as ClassicControllerPro?)?.LJoy);
                    break;
                case App.CAL_GUT_JOYSTICK:
                    CalibrateJoystick(Calibrations.None.GuitarRaw.joystick, (_lastState as Guitar?)?.joystick);
                    break;

                // Triggers
                case App.CAL_CC_RTRIGGER:
                    CalibrateTrigger(Calibrations.None.ClassicControllerRaw.R, (_lastState as ClassicController?)?.R);
                    break;

                case App.CAL_CC_LTRIGGER:
                    CalibrateTrigger(Calibrations.None.ClassicControllerRaw.L, (_lastState as ClassicController?)?.L);
                    break;

                case App.CAL_GUT_WHAMMY:
                    CalibrateTrigger(Calibrations.None.GuitarRaw.whammyBar, (_lastState as Guitar?)?.whammyBar);
                    break;

                default:
                    _calibrationTarget = string.Empty;
                    break;
            }
        }

        private void CalibrateJoystick(Joystick nonCalibrated, Joystick? currentCalibration)
        {
            // If we don't have a value for the current calibration, use the raw value.
            Windows.JoyCalibrationWindow joyCal = new Windows.JoyCalibrationWindow(nonCalibrated, currentCalibration.HasValue ? currentCalibration.Value : nonCalibrated);
            _openJoyWindow = joyCal;

#if DEBUG
            // This will allow for the dummy device window to retain focus
            if (DeviceID.StartsWith("Dummy"))
            {
                joyCal.Closed += (obj, args) =>
                {
                    if (joyCal.Apply)
                    {
                        OnJoystickCalibrated?.Invoke(joyCal.Calibration, _calibrationTarget, joyCal.FileName);
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
                OnJoystickCalibrated?.Invoke(joyCal.Calibration, _calibrationTarget, joyCal.FileName);
            }

            _openJoyWindow = null;
        }

        private void CalibrateTrigger(NintrollerLib.Trigger nonCalibrated, NintrollerLib.Trigger? currentCalibration)
        {
            // If no current trigger calibration, use the raw calibration
            Windows.TriggerCalibrationWindow trigCal = new Windows.TriggerCalibrationWindow(nonCalibrated, currentCalibration.HasValue ? currentCalibration.Value : nonCalibrated);
            _openTrigWindow = trigCal;

#if DEBUG
            if (DeviceID.StartsWith("Dummy"))
            {
                trigCal.Closed += (obj, args) =>
                {
                    if (trigCal.Apply)
                    {
                        OnTriggerCalibrated?.Invoke(trigCal.Calibration, _calibrationTarget, trigCal.FileName);
                    }

                    _openTrigWindow = null;
                };
                trigCal.Show();

                return;
            }
#endif

            trigCal.ShowDialog();

            if (trigCal.Apply)
            {
                OnTriggerCalibrated?.Invoke(trigCal.Calibration, _calibrationTarget, trigCal.FileName);
            }

            _openTrigWindow = null;
        }

        private void CalibrateIR()
        {
            IR lastIR = new IR();
            if (_lastState is Wiimote) lastIR = ((Wiimote)_lastState).irSensor;
            if (_lastState is Nunchuk) lastIR = ((Nunchuk)_lastState).wiimote.irSensor;
            if (_lastState is ClassicController) lastIR = ((ClassicController)_lastState).wiimote.irSensor;
            if (_lastState is ClassicControllerPro) lastIR = ((ClassicControllerPro)_lastState).wiimote.irSensor;

            Windows.IRCalibrationWindow irCal = new Windows.IRCalibrationWindow(lastIR);
            _openIRWindow = irCal;
            irCal.ShowDialog();

            if (irCal.Apply)
            {
                OnIRCalibrated?.Invoke(irCal.Calibration, irCal.FileName);
            }

            _openIRWindow = null;
        }
    }
}
