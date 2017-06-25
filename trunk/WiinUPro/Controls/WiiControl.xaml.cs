﻿using System;
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
        public WiiControl()
        {
            InitializeComponent();
        }

        public event Delegates.BoolArrDel OnChangeLEDs;
        public event Action<IRCamMode> OnChangeCameraMode;
        public event Action<IRCamSensitivity> OnChangeCameraSensitivty;

        public void ApplyInput(INintrollerState state)
        {
            // Do something
        }

        public void UpdateVisual(INintrollerState state)
        {
            if (state is Wiimote)
            {
                if (viewNunchuk.Visibility == Visibility.Visible) viewNunchuk.Visibility = Visibility.Collapsed;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;

                UpdateWiimoteVisual((Wiimote)state);
            }
            else if (state is Nunchuk)
            {
                if (viewNunchuk.Visibility != Visibility.Visible) viewNunchuk.Visibility = Visibility.Visible;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;

                var nun = (Nunchuk)state;
                UpdateWiimoteVisual(nun.wiimote);
            }
            else if (state is ClassicController)
            {
                if (viewNunchuk.Visibility == Visibility.Visible) viewNunchuk.Visibility = Visibility.Collapsed;
                if (viewClassicController.Visibility != Visibility.Visible) viewClassicController.Visibility = Visibility.Visible;
                if (viewClassicControllerPro.Visibility == Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Collapsed;

                var cc = (ClassicController)state;
                UpdateWiimoteVisual(cc.wiimote);

                // TODO: Make L & R Triggers work
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
            }
            else if (state is ClassicControllerPro)
            {
                if (viewNunchuk.Visibility == Visibility.Visible) viewNunchuk.Visibility = Visibility.Collapsed;
                if (viewClassicController.Visibility == Visibility.Visible) viewClassicController.Visibility = Visibility.Collapsed;
                if (viewClassicControllerPro.Visibility != Visibility.Visible) viewClassicControllerPro.Visibility = Visibility.Visible;

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
    }
}
