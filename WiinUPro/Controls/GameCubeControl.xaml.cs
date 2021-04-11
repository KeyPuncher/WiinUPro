using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NintrollerLib;
using Shared;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for GameCubeControl.xaml
    /// </summary>
    public partial class GameCubeControl : BaseControl, INintyControl
    {
        public event Delegates.BoolArrDel OnChangeLEDs;
        public event Delegates.JoystickDel OnJoyCalibrated;
        public event Delegates.TriggerDel OnTriggerCalibrated;

        protected Windows.JoyCalibrationWindow _openJoyWindow = null;
        protected Windows.TriggerCalibrationWindow _openTrigWindow = null;
        protected string _calibrationTarget = "";
        protected GameCubeAdapter _lastState;
        protected GameCubePort _activePort = GameCubePort.PORT1;

        private string _connectedStatus;
        private string _disconnectedStatus;

        protected enum GameCubePort
        {
            PORT1 = 1,
            PORT2 = 2,
            PORT3 = 3,
            PORT4 = 4
        }

        public GameCubeControl()
        {
            _inputPrefix = "1_";
            _connectedStatus = Globalization.Translate("Controller_Connected");
            _disconnectedStatus = Globalization.Translate("Controller_Disconnect");

            InitializeComponent();

            for (int i = 0; i < 4; ++i)
                (portSelection.Items[i] as MenuItem).Header = Globalization.TranslateFormat("GCN_Port", i);
        }

        public GameCubeControl(string deviceId) : this()
        {
            DeviceID = deviceId;
        }

        public void ApplyInput(INintrollerState state)
        {
            // Maybe one day I will remember what this was for or just remove it
        }

        public void UpdateVisual(INintrollerState state)
        {
            if (state is GameCubeAdapter gcn)
            {
                _lastState = gcn;
                GameCubeController activePort;

                bool connecited = GetActivePort(out activePort);

                A.Opacity = activePort.A ? 1 : 0;
                B.Opacity = activePort.B ? 1 : 0;
                X.Opacity = activePort.X ? 1 : 0;
                Y.Opacity = activePort.Y ? 1 : 0;
                Z.Opacity = activePort.Z ? 1 : 0;
                START.Opacity = activePort.Start ? 1 : 0;

                dpadUp.Opacity = activePort.Up ? 1 : 0;
                dpadDown.Opacity = activePort.Down ? 1 : 0;
                dpadLeft.Opacity = activePort.Left ? 1 : 0;
                dpadRight.Opacity = activePort.Right ? 1 : 0;

                L.Opacity = activePort.L.value > 0 ? 1 : 0;
                R.Opacity = activePort.R.value > 0 ? 1 : 0;

                var lOpacityMask = L.OpacityMask as LinearGradientBrush;
                if (lOpacityMask != null && lOpacityMask.GradientStops.Count == 2)
                {
                    double offset = 1 - System.Math.Min(1, activePort.L.value);
                    lOpacityMask.GradientStops[0].Offset = offset;
                    lOpacityMask.GradientStops[1].Offset = offset;
                }

                var rOpacityMask = R.OpacityMask as LinearGradientBrush;
                if (rOpacityMask != null && rOpacityMask.GradientStops.Count == 2)
                {
                    double offset = 1 - System.Math.Min(1, activePort.R.value);
                    rOpacityMask.GradientStops[0].Offset = offset;
                    rOpacityMask.GradientStops[1].Offset = offset;
                }

                joystick.Margin = new Thickness(190 + 100 * activePort.joystick.X, 272 - 100 * activePort.joystick.Y, 0, 0);
                cStick.Margin = new Thickness(887 + 100 * activePort.cStick.X, 618 - 100 * activePort.cStick.Y, 0, 0);

                connectionStatus.Content = connecited ? _connectedStatus : _disconnectedStatus;

                if (_openJoyWindow != null)
                {
                    if (_calibrationTarget == "joy") _openJoyWindow.Update(activePort.joystick);
                    else if (_calibrationTarget == "cStk") _openJoyWindow.Update(activePort.cStick);
                }
                else if (_openTrigWindow != null)
                {
                    if (_calibrationTarget == "L")  _openTrigWindow.Update(activePort.L);
                    else if (_calibrationTarget == "R") _openTrigWindow.Update(activePort.R);
                }
            }
        }

        public void ChangeLEDs(bool one, bool two, bool three, bool four)
        {
            // Doesn't use LEDs
        }

        private bool GetActivePort(out GameCubeController controller)
        {
            switch (_activePort)
            {
                default:
                case GameCubePort.PORT1:
                    controller = _lastState.port1;
                    return _lastState.port1Connected;
                case GameCubePort.PORT2:
                    controller = _lastState.port2;
                    return _lastState.port2Connected;
                case GameCubePort.PORT3:
                    controller = _lastState.port3;
                    return _lastState.port3Connected;
                case GameCubePort.PORT4:
                    controller = _lastState.port4;
                    return _lastState.port4Connected;
            }
        }

        protected override void CalibrateInput(string inputName)
        {
            if (inputName == _inputPrefix + R.Tag.ToString())
                CalibrateTrigger(true);
            else if (inputName == _inputPrefix + L.Tag.ToString())
                CalibrateTrigger(false);
            else if (inputName == _inputPrefix + joystick.Tag.ToString())
                CalibrateJoystick(false);
            else if (inputName == _inputPrefix + cStick.Tag.ToString())
                CalibrateJoystick(true);
        }

        private void CalibrateJoystick(bool cStick)
        {
            GameCubeController controller;
            GetActivePort(out controller);

            var nonCalibrated = new NintrollerLib.Joystick();
            var curCalibrated = new NintrollerLib.Joystick();

            if (cStick)
            {
                _calibrationTarget = "cStk";
                nonCalibrated = Calibrations.None.GameCubeControllerRaw.cStick;
                curCalibrated = controller.cStick;
            }
            else
            {
                _calibrationTarget = "joy";
                nonCalibrated = Calibrations.None.GameCubeControllerRaw.joystick;
                curCalibrated = controller.joystick;
            }

            Windows.JoyCalibrationWindow joyCal = new Windows.JoyCalibrationWindow(nonCalibrated, curCalibrated);
            _openJoyWindow = joyCal;

#if DEBUG
            // Don't use show dialog so dummy values can be modified
            if (DeviceID?.StartsWith("Dummy") ?? false)
            {
                joyCal.Closed += (obj, args) =>
                {
                    if (joyCal.Apply)
                    {
                        OnJoyCalibrated?.Invoke(joyCal.Calibration, _calibrationTarget, joyCal.FileName);
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
                OnJoyCalibrated?.Invoke(joyCal.Calibration, _calibrationTarget, joyCal.FileName);
            }
            
            _openJoyWindow = null;
            joyCal = null;
        }

        private void CalibrateTrigger(bool rightTrigger)
        {
            GameCubeController controller;
            GetActivePort(out controller);

            var nonCalibrated = new NintrollerLib.Trigger();
            var curCalibrated = new NintrollerLib.Trigger();

            if (rightTrigger)
            {
                _calibrationTarget = "R";
                nonCalibrated = Calibrations.None.GameCubeControllerRaw.R;
                curCalibrated = controller.R;
            }
            else
            {
                _calibrationTarget = "L";
                nonCalibrated = Calibrations.None.GameCubeControllerRaw.L;
                curCalibrated = controller.L;
            }

            Windows.TriggerCalibrationWindow trigCal = new Windows.TriggerCalibrationWindow(nonCalibrated, curCalibrated);
            _openTrigWindow = trigCal;

#if DEBUG
            // Don't use show dialog so dummy values can be modified
            if (DeviceID?.StartsWith("Dummy") ?? false)
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
            trigCal = null;
        }

        private void portSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (portSelection.SelectedIndex)
            {
                default:
                case 0:
                    _activePort = GameCubePort.PORT1;
                    break;
                case 1:
                    _activePort = GameCubePort.PORT2;
                    break;
                case 2:
                    _activePort = GameCubePort.PORT3;
                    break;
                case 3:
                    _activePort = GameCubePort.PORT4;
                    break;
            }

            _inputPrefix = ((int)_activePort).ToString() + "_";
        }
    }
}
