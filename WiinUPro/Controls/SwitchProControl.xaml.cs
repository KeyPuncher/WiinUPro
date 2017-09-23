using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Shared;
using SharpDX.DirectInput;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for SwitchProControl.xaml
    /// </summary>
    public partial class SwitchProControl : BaseControl, IJoyControl
    {
        public Guid AssociatedInstanceID { get; set; }

        public event Delegates.JoystickDel OnJoystickCalibrated;
        public AxisCalibration leftXCalibration;
        public AxisCalibration leftYCalibration;
        public AxisCalibration rightXCalibration;
        public AxisCalibration rightYCalibration;

        protected bool _leftCalibration;
        protected Windows.JoyCalibrationWindow _openJoyWindow;
        protected NintrollerLib.Joystick _calLeftJoystick;
        protected NintrollerLib.Joystick _calRightJoystick;

        public SwitchProControl()
        {
            InitializeComponent();
            _calLeftJoystick = new NintrollerLib.Joystick();
            _calRightJoystick = new NintrollerLib.Joystick();
            leftXCalibration = new AxisCalibration(0, 65535, 32767, 2048);
            leftYCalibration = new AxisCalibration(0, 65535, 32767, 2048);
            rightXCalibration = new AxisCalibration(0, 65535, 32767, 2048);
            rightYCalibration = new AxisCalibration(0, 65535, 32767, 2048);
        }

        public SwitchProControl(AxisCalibration _leftXCal, AxisCalibration _leftYCal, AxisCalibration _rightXCal, AxisCalibration _rightYCal) : this()
        {
            leftXCalibration = _leftXCal;
            leftYCalibration = _leftYCal;
            rightXCalibration = _rightXCal;
            rightYCalibration = _rightYCal;
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
                        _calLeftJoystick.rawX = update.Value;
                        break;
                    case JoystickOffset.Y:
                        _calLeftJoystick.rawY = 65535 - update.Value;
                        break;
                    case JoystickOffset.RotationX:
                        _calRightJoystick.rawX = update.Value;
                        break;
                    case JoystickOffset.RotationY:
                        _calRightJoystick.rawY = 65535 - update.Value;
                        break;
                }
            }

            var joyL = JoyControl.ConvertToNintyJoy(leftXCalibration, leftYCalibration);
            var joyR = JoyControl.ConvertToNintyJoy(rightXCalibration, rightYCalibration);

            joyL.rawX = _calLeftJoystick.rawX;
            joyL.rawY = _calLeftJoystick.rawY;
            joyR.rawX = _calRightJoystick.rawX;
            joyR.rawY = _calRightJoystick.rawY;

            joyL.Normalize();
            joyR.Normalize();
            
            leftStick.Margin = new Thickness(146 + 30 * joyL.X, 291 - 30 * joyL.Y, 0, 0);
            rightStick.Margin = new Thickness(507 + 30 * joyR.X, 412 - 30 * joyR.Y, 0, 0);
            leftStickButton.Margin = leftStick.Margin;
            rightStickButton.Margin = rightStick.Margin;

            if (_openJoyWindow != null)
            {
                _openJoyWindow.Update(_leftCalibration ? _calLeftJoystick : _calRightJoystick);
            }
        }

        protected override void QuickAssign(string prefix, string type)
        {
            string[] dir = new string[4];

            if (prefix == "swpR")
            {
                dir[0] = "RotationY+";
                dir[1] = "RotationY-";
                dir[2] = "RotationX-";
                dir[3] = "RotationX+";
            }
            else
            {
                dir[0] = "Y+";
                dir[1] = "Y-";
                dir[2] = "X-";
                dir[3] = "X+";
            }

            Dictionary<string, AssignmentCollection> args = new Dictionary<string, AssignmentCollection>();

            if (type == "Mouse")
            {
                args.Add(dir[0], new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveUp) }));
                args.Add(dir[1], new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveDown) }));
                args.Add(dir[2], new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveLeft) }));
                args.Add(dir[3], new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveRight) }));
            }
            else if (type == "WASD")
            {
                args.Add(dir[0], new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_W) }));
                args.Add(dir[1], new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_S) }));
                args.Add(dir[2], new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_A) }));
                args.Add(dir[3], new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_D) }));
            }
            else if (type == "Arrows")
            {
                args.Add(dir[0], new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_UP) }));
                args.Add(dir[1], new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_DOWN) }));
                args.Add(dir[2], new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_LEFT) }));
                args.Add(dir[3], new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_RIGHT) }));
            }

            CallEvent_OnQuickAssign(args);
        }

        protected override void QuickAssignMouse_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var header = item.Header as string;
                var prefix = item.Tag as string;

                if (header != null && prefix != null)
                {
                    float speed = 1f;
                    switch (header)
                    {
                        case "50% Speed": speed = 0.5f; break;
                        case "150% Speed": speed = 1.5f; break;
                        case "200% Speed": speed = 2.0f; break;
                        case "250% Speed": speed = 2.5f; break;
                        case "300% Speed": speed = 3.0f; break;
                        case "100% Speed":
                        default:
                            speed = 1f;
                            break;
                    }

                    string[] dir = new string[4];

                    if (prefix == "swpR")
                    {
                        dir[0] = "RotationY+";
                        dir[1] = "RotationY-";
                        dir[2] = "RotationX-";
                        dir[3] = "RotationX+";
                    }
                    else
                    {
                        dir[0] = "Y+";
                        dir[1] = "Y-";
                        dir[2] = "X-";
                        dir[3] = "X+";
                    }

                    Dictionary<string, AssignmentCollection> args = new Dictionary<string, AssignmentCollection>();
                    args.Add(dir[0], new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveUp, speed) }));
                    args.Add(dir[1], new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveDown, speed) }));
                    args.Add(dir[2], new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveLeft, speed) }));
                    args.Add(dir[3], new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveRight, speed) }));
                    CallEvent_OnQuickAssign(args);
                }
            }
        }

        protected void CalibrateJoystick_Click(object sender, RoutedEventArgs e)
        {
            _leftCalibration = (sender as FrameworkElement).Tag.ToString() == "swpL";
            string targetCalibration = _leftCalibration ? App.CAL_SWP_LJOYSTICK : App.CAL_SWP_RJOYSTICK;

            NintrollerLib.Joystick nonCalibrated = new NintrollerLib.Joystick
            {
                minX = 0,
                minY = 0,
                maxX = 65535,
                maxY = 65535,
                centerX = 32767,
                centerY = 32767
            };

            NintrollerLib.Joystick curCalibration = new NintrollerLib.Joystick
            {
                minX = _leftCalibration ? leftXCalibration.min : rightXCalibration.min,
                minY = _leftCalibration ? leftYCalibration.min : rightYCalibration.min,
                maxX = _leftCalibration ? leftXCalibration.max : rightXCalibration.max,
                maxY = _leftCalibration ? leftYCalibration.max : rightYCalibration.max,
                centerX = _leftCalibration ? leftXCalibration.center : rightXCalibration.center,
                centerY = _leftCalibration ? leftYCalibration.center : rightYCalibration.center,
                deadXn = _leftCalibration ? leftXCalibration.deadNeg : rightXCalibration.deadNeg,
                deadXp = _leftCalibration ? leftXCalibration.deadPos : rightXCalibration.deadPos,
                deadYn = _leftCalibration ? leftYCalibration.deadNeg : rightYCalibration.deadNeg,
                deadYp = _leftCalibration ? leftYCalibration.deadPos : rightYCalibration.deadPos
            };
            
            Windows.JoyCalibrationWindow joyCal = new Windows.JoyCalibrationWindow(nonCalibrated, curCalibration);
            _openJoyWindow = joyCal;
            joyCal.ShowDialog();

            if (joyCal.Apply)
            {
                OnJoystickCalibrated?.Invoke(joyCal.Calibration, targetCalibration);
            }

            _openJoyWindow = null;
        }
    }
}
