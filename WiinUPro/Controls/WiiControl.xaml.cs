using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        public void SetInputTooltip(string inputName, string tooltip)
        {
            switch (inputName)
            {
                case INPUT_NAMES.WIIMOTE.A: wBtnA.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.B: wBtnB.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.ONE: wBtnOne.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.TWO: wBtnTwo.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.PLUS: wBtnPlus.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.MINUS: wBtnMinus.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.HOME: wBtnHome.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.UP: wBtnUp.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.LEFT: wBtnLeft.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.RIGHT: wBtnRight.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.DOWN: wBtnDown.ToolTip = tooltip; break;
                case INPUT_NAMES.WIIMOTE.IR_UP: UpdateTooltipLine(irCanvas, tooltip, 0); break;
                case INPUT_NAMES.WIIMOTE.IR_LEFT: UpdateTooltipLine(irCanvas, tooltip, 1); break;
                case INPUT_NAMES.WIIMOTE.IR_RIGHT: UpdateTooltipLine(irCanvas, tooltip, 2); break;
                case INPUT_NAMES.WIIMOTE.IR_DOWN: UpdateTooltipLine(irCanvas, tooltip, 3); break;

                case INPUT_NAMES.NUNCHUK.C: nBtnC.ToolTip = tooltip; break;
                case INPUT_NAMES.NUNCHUK.Z: nBtnZ.ToolTip = tooltip; break;
                case INPUT_NAMES.NUNCHUK.UP: UpdateTooltipLine(nJoy, tooltip, 0); break;
                case INPUT_NAMES.NUNCHUK.LEFT: UpdateTooltipLine(nJoy, tooltip, 1); break;
                case INPUT_NAMES.NUNCHUK.RIGHT: UpdateTooltipLine(nJoy, tooltip, 2); break;
                case INPUT_NAMES.NUNCHUK.DOWN: UpdateTooltipLine(nJoy, tooltip, 3); break;

                case INPUT_NAMES.CLASSIC_CONTROLLER.A: ccBtnA.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.B: ccBtnB.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.X: ccBtnX.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.Y: ccBtnY.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.ZL: ccBtnZL.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.ZR: ccBtnZR.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.HOME: ccBtnHome.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.START: ccBtnStart.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.SELECT: ccBtnSelect.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.UP: ccBtnUp.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.LEFT: ccBtnLeft.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.RIGHT: ccBtnRight.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.DOWN: ccBtnDown.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.LT: UpdateTooltipLine(ccL, tooltip, 0); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.LFULL: UpdateTooltipLine(ccL, tooltip, 1); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.RT: UpdateTooltipLine(ccR, tooltip, 0); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.RFULL: UpdateTooltipLine(ccR, tooltip, 1); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.LUP: UpdateTooltipLine(ccLeftStick, tooltip, 0); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.LLEFT: UpdateTooltipLine(ccLeftStick, tooltip, 1); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.LRIGHT: UpdateTooltipLine(ccLeftStick, tooltip, 2); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.LDOWN: UpdateTooltipLine(ccLeftStick, tooltip, 3); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.RUP: UpdateTooltipLine(ccRightStick, tooltip, 0); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.RLEFT: UpdateTooltipLine(ccRightStick, tooltip, 1); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.RRIGHT: UpdateTooltipLine(ccRightStick, tooltip, 2); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER.RDOWN: UpdateTooltipLine(ccRightStick, tooltip, 3); break;

                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.A: ccpBtnA.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.B: ccpBtnB.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.X: ccpBtnX.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.Y: ccpBtnY.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.L: ccpBtnL.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.R: ccpBtnR.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.ZL: ccpBtnZL.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.ZR: ccpBtnZR.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.HOME: ccpBtnHome.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.START: ccpBtnStart.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.SELECT: ccpBtnSelect.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.UP: ccpBtnUp.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LEFT: ccpBtnLeft.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RIGHT: ccpBtnRight.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.DOWN: ccpBtnDown.ToolTip = tooltip; break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LUP: UpdateTooltipLine(ccpLeftStick, tooltip, 0); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LLEFT: UpdateTooltipLine(ccpLeftStick, tooltip, 1); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LRIGHT: UpdateTooltipLine(ccpLeftStick, tooltip, 2); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.LDOWN: UpdateTooltipLine(ccpLeftStick, tooltip, 3); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RUP: UpdateTooltipLine(ccpRightStick, tooltip, 0); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RLEFT: UpdateTooltipLine(ccpRightStick, tooltip, 1); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RRIGHT: UpdateTooltipLine(ccpRightStick, tooltip, 2); break;
                case INPUT_NAMES.CLASSIC_CONTROLLER_PRO.RDOWN: UpdateTooltipLine(ccpRightStick, tooltip, 3); break;

                case INPUT_NAMES.GUITAR.BLUE: gBtnBlue.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.ORANGE: gBtnOrange.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.YELLOW: gBtnYellow.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.GREEN: gBtnGreen.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.RED: gBtnRed.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.TOUCH_1: gTouch1.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.TOUCH_2: gTouch2.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.TOUCH_3: gTouch3.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.TOUCH_4: gTouch4.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.TOUCH_5: gTouch5.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.PLUS: gBtnPlus.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.MINUS: gBtnPlus.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.STRUM_UP: gBtnStrumUp.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.STRUM_DOWN: gBtnStrumDown.ToolTip = tooltip; break;
                case INPUT_NAMES.GUITAR.WHAMMY_BAR: UpdateTooltipLine(gWhammy, tooltip, 0); break;
                case INPUT_NAMES.GUITAR.WHAMMY_FULL: UpdateTooltipLine(gWhammy, tooltip, 1); break;
                case INPUT_NAMES.GUITAR.UP: UpdateTooltipLine(gStick, tooltip, 0); break;
                case INPUT_NAMES.GUITAR.LEFT: UpdateTooltipLine(gStick, tooltip, 1); break;
                case INPUT_NAMES.GUITAR.RIGHT: UpdateTooltipLine(gStick, tooltip, 2); break;
                case INPUT_NAMES.GUITAR.DOWN: UpdateTooltipLine(gStick, tooltip, 3); break;

                case INPUT_NAMES.TAIKO_DRUM.CENTER_LEFT: takL.ToolTip = tooltip; break;
                case INPUT_NAMES.TAIKO_DRUM.CENTER_RIGHT: takR.ToolTip = tooltip; break;
                case INPUT_NAMES.TAIKO_DRUM.RIM_LEFT: takRimL.ToolTip = tooltip; break;
                case INPUT_NAMES.TAIKO_DRUM.RIM_RIGHT: takRimR.ToolTip = tooltip; break;
            }
        }

        public void ClearTooltips()
        {
            wBtnA.ToolTip = "UNSET";
            wBtnB.ToolTip = "UNSET";
            wBtnOne.ToolTip = "UNSET";
            wBtnTwo.ToolTip = "UNSET";
            wBtnPlus.ToolTip = "UNSET";
            wBtnMinus.ToolTip = "UNSET";
            wBtnHome.ToolTip = "UNSET";
            wBtnUp.ToolTip = "UNSET";
            wBtnLeft.ToolTip = "UNSET";
            wBtnRight.ToolTip = "UNSET";
            wBtnDown.ToolTip = "UNSET";
            UpdateTooltipLine(irCanvas, "UNSET", 0);
            UpdateTooltipLine(irCanvas, "UNSET", 1);
            UpdateTooltipLine(irCanvas, "UNSET", 2);
            UpdateTooltipLine(irCanvas, "UNSET", 3);

            nBtnC.ToolTip = "UNSET";
            nBtnZ.ToolTip = "UNSET";
            UpdateTooltipLine(nJoy, "UNSET", 0);
            UpdateTooltipLine(nJoy, "UNSET", 1);
            UpdateTooltipLine(nJoy, "UNSET", 2);
            UpdateTooltipLine(nJoy, "UNSET", 3);

            ccBtnA.ToolTip = "UNSET";
            ccBtnB.ToolTip = "UNSET";
            ccBtnX.ToolTip = "UNSET";
            ccBtnY.ToolTip = "UNSET";
            ccBtnZL.ToolTip = "UNSET";
            ccBtnZR.ToolTip = "UNSET";
            ccBtnHome.ToolTip = "UNSET";
            ccBtnStart.ToolTip = "UNSET";
            ccBtnSelect.ToolTip = "UNSET";
            ccBtnUp.ToolTip = "UNSET";
            ccBtnLeft.ToolTip = "UNSET";
            ccBtnRight.ToolTip = "UNSET";
            ccBtnDown.ToolTip = "UNSET";
            UpdateTooltipLine(ccL, "UNSET", 0);
            UpdateTooltipLine(ccL, "UNSET", 1);
            UpdateTooltipLine(ccR, "UNSET", 0);
            UpdateTooltipLine(ccR, "UNSET", 1);
            UpdateTooltipLine(ccLeftStick, "UNSET", 0);
            UpdateTooltipLine(ccLeftStick, "UNSET", 1);
            UpdateTooltipLine(ccLeftStick, "UNSET", 2);
            UpdateTooltipLine(ccLeftStick, "UNSET", 3);
            UpdateTooltipLine(ccRightStick, "UNSET", 0);
            UpdateTooltipLine(ccRightStick, "UNSET", 1);
            UpdateTooltipLine(ccRightStick, "UNSET", 2);
            UpdateTooltipLine(ccRightStick, "UNSET", 3);

            ccpBtnA.ToolTip = "UNSET";
            ccpBtnB.ToolTip = "UNSET";
            ccpBtnX.ToolTip = "UNSET";
            ccpBtnY.ToolTip = "UNSET";
            ccpBtnL.ToolTip = "UNSET";
            ccpBtnR.ToolTip = "UNSET";
            ccpBtnZL.ToolTip = "UNSET";
            ccpBtnZR.ToolTip = "UNSET";
            ccpBtnHome.ToolTip = "UNSET";
            ccpBtnStart.ToolTip = "UNSET";
            ccpBtnSelect.ToolTip = "UNSET";
            ccpBtnUp.ToolTip = "UNSET";
            ccpBtnLeft.ToolTip = "UNSET";
            ccpBtnRight.ToolTip = "UNSET";
            ccpBtnDown.ToolTip = "UNSET";
            UpdateTooltipLine(ccpLeftStick, "UNSET", 0);
            UpdateTooltipLine(ccpLeftStick, "UNSET", 1);
            UpdateTooltipLine(ccpLeftStick, "UNSET", 2);
            UpdateTooltipLine(ccpLeftStick, "UNSET", 3);
            UpdateTooltipLine(ccpRightStick, "UNSET", 0);
            UpdateTooltipLine(ccpRightStick, "UNSET", 1);
            UpdateTooltipLine(ccpRightStick, "UNSET", 2);
            UpdateTooltipLine(ccpRightStick, "UNSET", 3);

            gBtnBlue.ToolTip = "UNSET";
            gBtnOrange.ToolTip = "UNSET";
            gBtnYellow.ToolTip = "UNSET";
            gBtnGreen.ToolTip = "UNSET";
            gBtnRed.ToolTip = "UNSET";
            gTouch1.ToolTip = "UNSET";
            gTouch2.ToolTip = "UNSET";
            gTouch3.ToolTip = "UNSET";
            gTouch4.ToolTip = "UNSET";
            gTouch5.ToolTip = "UNSET";
            gBtnPlus.ToolTip = "UNSET";
            gBtnPlus.ToolTip = "UNSET";
            gBtnStrumUp.ToolTip = "UNSET";
            gBtnStrumDown.ToolTip = "UNSET";
            UpdateTooltipLine(gWhammy, "UNSET", 0);
            UpdateTooltipLine(gWhammy, "UNSET", 1);
            UpdateTooltipLine(gStick, "UNSET", 0);
            UpdateTooltipLine(gStick, "UNSET", 1);
            UpdateTooltipLine(gStick, "UNSET", 2);
            UpdateTooltipLine(gStick, "UNSET", 3);

            takL.ToolTip = "UNSET";
            takR.ToolTip = "UNSET";
            takRimL.ToolTip = "UNSET";
            takRimR.ToolTip = "UNSET";
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

        protected override void OpenJoystickMenu(object sender, MouseButtonEventArgs e)
        {
            base.OpenJoystickMenu(sender, e);

            // Preserving existing profile backwards compatability with dynamic context menu.
            // Remove the "Joy" text between things like nJoyUp input names.
            _analogMenuInput = _menuOwnerTag.Replace("Joy", "");
        }

        protected override void OpenTriggerMenu(object sender, MouseButtonEventArgs e)
        {
            base.OpenTriggerMenu(sender, e);

            // Preserving existing profile backwards compatability with dynamic context menu.
            // Remove the trailing "T" text between things like ccLT input names.
            _analogMenuInput = _menuOwnerTag.Replace("LT", "L").Replace("RT", "R").Replace("WT", "W");
        }

        protected override void QuickAssign(string prefix, string type)
        {
            // Preserving existing profile backwards compatability with dynamic context menu.
            // Remove the "Joy" text between things like nJoyUp input names.
            base.QuickAssign(prefix.Replace("Joy", ""), type);
        }

        protected override void QuickAssignMouse_Click(object sender, RoutedEventArgs e)
        {
            _menuOwnerTag = _menuOwnerTag.Replace("Joy", "");
            base.QuickAssignMouse_Click(sender, e);
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
