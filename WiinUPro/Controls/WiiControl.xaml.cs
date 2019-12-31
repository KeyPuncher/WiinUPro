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

                UpdateWiimoteVisual((Wiimote)state);
            }
            else if (state is Nunchuk)
            {
                if (viewNunchuk.Visibility != Visibility.Visible) viewNunchuk.Visibility = Visibility.Visible;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;
                if (viewGuitar.Visibility == Visibility.Visible) viewGuitar.Visibility = Visibility.Collapsed;

                var nun = (Nunchuk)state;
                UpdateWiimoteVisual(nun.wiimote);

                nBtnC.Opacity = nun.C ? 1 : 0;
                nBtnZ.Opacity = nun.Z ? 1 : 0;
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

                var cc = (ClassicController)state;
                UpdateWiimoteVisual(cc.wiimote);

                ccBtnA.Opacity      = cc.A ? 1 : 0;
                ccBtnB.Opacity      = cc.B ? 1 : 0;
                ccBtnX.Opacity      = cc.X ? 1 : 0;
                ccBtnY.Opacity      = cc.Y ? 1 : 0;
                ccBtnUp.Opacity     = cc.Up ? 1 : 0;
                ccBtnDown.Opacity   = cc.Down ? 1 : 0;
                ccBtnRight.Opacity  = cc.Right ? 1 : 0;
                ccBtnLeft.Opacity   = cc.Left ? 1 : 0;
                ccPadCenter.Opacity = cc.Up || cc.Down || cc.Left || cc.Right ? 1 : 0;
                ccBtnHome.Opacity   = cc.Home ? 1 : 0;
                ccBtnSelect.Opacity = cc.Select ? 1 : 0;
                ccBtnStart.Opacity  = cc.Start ? 1 : 0;
                ccBtnZL.Opacity     = cc.ZL ? 1 : 0;
                ccBtnZR.Opacity     = cc.ZR ? 1 : 0;
                ccL.Opacity         = cc.L.value > 0 ? 1 : 0;
                ccR.Opacity         = cc.R.value > 0 ? 1 : 0;
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

                var ccp = (ClassicControllerPro)state;
                UpdateWiimoteVisual(ccp.wiimote);

                ccpBtnA.Opacity      = ccp.A ? 1 : 0;
                ccpBtnB.Opacity      = ccp.B ? 1 : 0;
                ccpBtnX.Opacity      = ccp.X ? 1 : 0;
                ccpBtnY.Opacity      = ccp.Y ? 1 : 0;
                ccpBtnUp.Opacity     = ccp.Up ? 1 : 0;
                ccpBtnDown.Opacity   = ccp.Down ? 1 : 0;
                ccpBtnRight.Opacity  = ccp.Right ? 1 : 0;
                ccpBtnLeft.Opacity   = ccp.Left ? 1 : 0;
                ccpPadCenter.Opacity = ccp.Up || ccp.Down || ccp.Left || ccp.Right ? 1 : 0;
                ccpBtnHome.Opacity   = ccp.Home ? 1 : 0;
                ccpBtnSelect.Opacity = ccp.Select ? 1 : 0;
                ccpBtnStart.Opacity  = ccp.Start ? 1 : 0;
                ccpBtnZL.Opacity     = ccp.ZL ? 1 : 0;
                ccpBtnZR.Opacity     = ccp.ZR ? 1 : 0;
                ccpBtnL.Opacity      = ccp.L ? 1 : 0;
                ccpBtnR.Opacity      = ccp.R ? 1 : 0;
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

                var gut = (Guitar)state;
                UpdateWiimoteVisual(gut.wiimote);

                gBtnGreen.Opacity = gut.Green ? 1 : 0;
                gBtnRed.Opacity = gut.Red ? 1 : 0;
                gBtnYellow.Opacity = gut.Yellow ? 1 : 0;
                gBtnBlue.Opacity = gut.Blue ? 1 : 0;
                gBtnOrange.Opacity = gut.Orange ? 1 : 0;
                gBtnStrumUp.Opacity = gut.StrumUp ? 1 : 0;
                gBtnStrumDown.Opacity = gut.StrumDown ? 1 : 0;
                gBtnPlus.Opacity = gut.Plus ? 1 : 0;
                gBtnMinus.Opacity = gut.Minus ? 1 : 0;
                gTouch1.Opacity = gut.T1 ? 1 : 0;
                gTouch2.Opacity = gut.T2 ? 1 : 0;
                gTouch3.Opacity = gut.T3 ? 1 : 0;
                gTouch4.Opacity = gut.T4 ? 1 : 0;
                gTouch5.Opacity = gut.T5 ? 1 : 0;
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

            wBtnA.Opacity     = wiimote.buttons.A ? 1 : 0;
            wBtnB.Opacity     = wiimote.buttons.B ? 1 : 0;
            wBtnOne.Opacity   = wiimote.buttons.One ? 1 : 0;
            wBtnTwo.Opacity   = wiimote.buttons.Two ? 1 : 0;
            wBtnUp.Opacity    = wiimote.buttons.Up ? 1 : 0;
            wBtnRight.Opacity = wiimote.buttons.Right ? 1 : 0;
            wBtnDown.Opacity  = wiimote.buttons.Down ? 1 : 0;
            wBtnLeft.Opacity  = wiimote.buttons.Left ? 1 : 0;
            wBtnMinus.Opacity = wiimote.buttons.Minus ? 1 : 0;
            wBtnPlus.Opacity  = wiimote.buttons.Plus ? 1 : 0;
            wBtnHome.Opacity  = wiimote.buttons.Home ? 1 : 0;
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

        private void SetIRMode(object sender, RoutedEventArgs e)
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

        private void SetIRSensitivity(object sender, RoutedEventArgs e)
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

        private void CalibrateJoystick_Click(object sender, RoutedEventArgs e)
        {
            _calibrationTarget = (sender as FrameworkElement).Tag.ToString();

            Joystick nonCalibrated = new Joystick();
            Joystick curCalibration = new Joystick();
            switch (_calibrationTarget)
            {
                case App.CAL_NUN_JOYSTICK:
                    nonCalibrated = Calibrations.None.NunchukRaw.joystick;
                    if (_lastState is Nunchuk) curCalibration = ((Nunchuk)_lastState).joystick;
                    break;
                case App.CAL_CC_LJOYSTICK:
                    nonCalibrated = Calibrations.None.ClassicControllerRaw.LJoy;
                    if (_lastState is ClassicController) curCalibration = ((ClassicController)_lastState).LJoy;
                    break;
                case App.CAL_CC_RJOYSTICK:
                    nonCalibrated = Calibrations.None.ClassicControllerRaw.RJoy;
                    if (_lastState is ClassicController) curCalibration = ((ClassicController)_lastState).RJoy;
                    break;
                case App.CAL_CCP_LJOYSTICK: nonCalibrated = Calibrations.None.ClassicControllerProRaw.LJoy;
                    if (_lastState is ClassicControllerPro) curCalibration = ((ClassicControllerPro)_lastState).LJoy;
                    break;
                case App.CAL_CCP_RJOYSTICK:
                    nonCalibrated = Calibrations.None.ClassicControllerProRaw.RJoy;
                    if (_lastState is ClassicControllerPro) curCalibration = ((ClassicControllerPro)_lastState).RJoy;
                    break;
                case App.CAL_GUT_JOYSTICK:
                    nonCalibrated = Calibrations.None.GuitarRaw.joystick;
                    if (_lastState is Guitar) curCalibration = ((Guitar)_lastState).joystick;
                    break;
                default: return;
            }

            Windows.JoyCalibrationWindow joyCal = new Windows.JoyCalibrationWindow(nonCalibrated, curCalibration);
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

        private void CalibrateTrigger_Click(object sender, RoutedEventArgs e)
        {
            _calibrationTarget = (sender as FrameworkElement).Tag.ToString();

            var nonCalibrated = new NintrollerLib.Trigger();
            var curCalibrated = new NintrollerLib.Trigger();

            if (!(_lastState is ClassicController)) return;

            if (_calibrationTarget == App.CAL_CC_RTRIGGER)
            {
                nonCalibrated = Calibrations.None.ClassicControllerRaw.R;
                curCalibrated = ((ClassicController)_lastState).R;
            }
            else if (_calibrationTarget == App.CAL_CC_LTRIGGER)
            {
                nonCalibrated = Calibrations.None.ClassicControllerRaw.L;
                curCalibrated = ((ClassicController)_lastState).L;
            }
            else if (_calibrationTarget == App.CAL_GUT_WHAMMY)
            {
                nonCalibrated = Calibrations.None.GuitarRaw.whammyBar;
                curCalibrated = ((Guitar)_lastState).whammyBar;
            }

            Windows.TriggerCalibrationWindow trigCal = new Windows.TriggerCalibrationWindow(nonCalibrated, curCalibrated);
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

        private void CalibrateIR_Click(object sender, RoutedEventArgs e)
        {
            _calibrationTarget = (sender as FrameworkElement).Tag.ToString();
            if (_calibrationTarget != App.CAL_WII_IR) return;

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
