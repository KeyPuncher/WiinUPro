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
using Shared.Windows;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for NintyControl.xaml
    /// </summary>
    public partial class NintyControl : UserControl
    {
        internal delegate void TypeUpdate(ControllerType type);
        internal event TypeUpdate OnTypeChange;     // Called on extension changes
        internal event Action OnDisconnect;         // Called when disconnected

        internal WinBtStream _stream;               // Controller stream to the device
        internal Nintroller _nintroller;            // Physical Controller Device
        internal INintyControl _controller;         // Visual Controller Representation
        internal AssignmentCollection _clipboard;   // Assignments to be pasted
        internal string _selectedInput;             // Controller's input to be effected by change
        internal ShiftState _currentState;          // Current shift state being applied
        internal Shared.DeviceInfo _info;           // Info on this device, HID path, Type

        internal ScpDirector _scp;                  // Quick reference to SCP Director
        internal Dictionary<string, AssignmentCollection>[] _assignments;

        // For Testing
        internal Shared.DummyDevice _dummy;

        public int ShiftIndex
        {
            get
            {
                return (int)_currentState;
            }
        }

        public ShiftState CurrentShiftState
        {
            get
            {
                return _currentState;
            }
        }

        public NintyControl()
        {
            _currentState = ShiftState.None;
            InitializeComponent();

            // Speech test
            //System.Speech.Synthesis.SpeechSynthesizer s = new System.Speech.Synthesis.SpeechSynthesizer();
            //s.SpeakAsync("Controller Found");
        }

        public NintyControl(Shared.DeviceInfo deviceInfo) : this()
        {
            _assignments = new[] {
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>()
            };

            _info = deviceInfo;
            _scp = ScpDirector.Access;
        }

        public void ChangeState(ShiftState newState)
        {
            if (_currentState != newState)
            {
                // TODO: This would unpress any keys from all controllers, make it device specific
                KeyboardDirector.Access.Release();
                MouseDirector.Access.Release();
                _currentState = newState;

                _nintroller.SetPlayerLED((int)newState + 1);

                Dispatcher.Invoke(new Action(() =>
                 {
                    if (newState == ShiftState.None) _controller.ChangeLEDs(true, false, false, false);
                    else if (newState == ShiftState.Red) _controller.ChangeLEDs(false, true, false, false);
                    else if (newState == ShiftState.Blue) _controller.ChangeLEDs(false, false, true, false);
                    else if (newState == ShiftState.Green) _controller.ChangeLEDs(false, false, false, true);
                }));
            }
        }

        public bool Connect()
        {
            bool success = false;

            if (_info.DevicePath == "Dummy")
            {
                _dummy = new Shared.DummyDevice(Calibrations.Defaults.ProControllerDefault);
                var dumWin = new Windows.DummyWindow(_dummy);
                dumWin.Show();
                _nintroller = new Nintroller(_dummy);
                OnDisconnect += () =>
                {
                    dumWin.Close();
                };
            }
            else
            {
                _stream = new WinBtStream(_info.DevicePath);
                _nintroller = new Nintroller(_stream);//, deviceInfo.Type);
            }
            _nintroller.StateUpdate += _nintroller_StateUpdate;
            _nintroller.ExtensionChange += _nintroller_ExtensionChange;
            _nintroller.LowBattery += _nintroller_LowBattery;

            // TODO: Seems OpenConnection() can still succeed on toshiba w/o device connected
            if (_dummy != null || _stream.OpenConnection())
            {
                _nintroller.BeginReading();
                _nintroller.GetStatus();
                _nintroller.SetPlayerLED(1);

                // We need a function we can await for the type to come back
                // But the hint type may be present
                CreateController(_nintroller.Type);

                if (_controller != null)
                {
                    SetupController();

                    success = true;
                    _nintroller.SetReportType(InputReport.ExtOnly, true);
                }
                else
                {
                    // Controller type not supported or unknown, teardown?
                    //success = false;
                    success = true;
                }
            }
#if DEBUG
            else
            {
                _controller = new ProControl();
                SetupController();
                success = true;
            }
#else
            else
            {
                MessageBox.Show("Could not connect to device!");
                btnConnect.IsEnabled = true;
            }
#endif
            return success;
        }

        public void Disconnect()
        {
            if (_stream != null)
                _stream.Close();

            if (_dummy != null)
                _dummy.Close();

            _view.Child = null;
            _controller = null;

            OnDisconnect?.Invoke();
        }

        private void CreateController(ControllerType type)
        {
            switch (type)
            {
                case ControllerType.ProController:
                    // TODO: Discover why this causes an access violation exceptions
                    _controller = new ProControl();
                    ((ProControl)_controller).OnJoyCalibrated += (j, rJoy) =>
                    {
                        var currentProCal = _nintroller.StoredCalibrations.ProCalibration;

                        if (rJoy)
                        {
                            currentProCal.RJoy = j;
                        }
                        else
                        {
                            currentProCal.LJoy = j;
                        }

                        _nintroller.SetCalibration(currentProCal);
                    };
                    break;
            }
        }

        private void SetupController()
        {
            if (_controller != null)
            {
                _controller.ChangeLEDs(_nintroller.Led1, _nintroller.Led2, _nintroller.Led3, _nintroller.Led4);
                _controller.OnChangeLEDs += SetLeds;
                _controller.OnInputSelected += InputSelected;
                _controller.OnInputRightClick += InputOpenMenu;
                _controller.OnQuickAssign += QuickAssignment;

                _view.Child = _controller as UserControl;
                ((UserControl)_view.Child).HorizontalAlignment = HorizontalAlignment.Left;
                ((UserControl)_view.Child).VerticalAlignment = VerticalAlignment.Top;
            }
        }

        #region Nintroller Events
        private void _nintroller_LowBattery(object sender, LowBatteryEventArgs e)
        {
            // Indicate that this controller's battery is low
        }

        private void _nintroller_ExtensionChange(object sender, NintrollerExtensionEventArgs e)
        {
            OnTypeChange?.Invoke(e.controllerType);

            // Handle the extension change
            Dispatcher.Invoke(new Action(() =>
            {
                CreateController(e.controllerType);

                if (_controller != null)
                {
                    SetupController();
                }
                else
                {
                    MessageBox.Show("Device or Extension not supported.");
                }
            }));
        }

        private void _nintroller_StateUpdate(object sender, NintrollerStateEventArgs e)
        {
            // Use the input to apply assignments.
            // This should only be done if not modifying the assignments
            //_controller.ApplyInput(e.state);
            if (_assignments != null)
            {
                foreach (var input in e.state)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("{0} :\t\t{1}", input.Key, input.Value));
                    if (_assignments[ShiftIndex].ContainsKey(input.Key))
                    {
                        _assignments[ShiftIndex][input.Key].ApplyAll(input.Value);
                    }
                }
            }

            // Send any XInput changes
            //ScpDirector.Access.ApplyAll();
            // TODO: only update devices this controller is emulating
            _scp.ApplyAll();

            // Visaul should only be updated if tab is in view
            Dispatcher.Invoke(new Action(() =>
            {
                if (_controller != null)
                {
                    _controller.UpdateVisual(e.state);
                }
            }));
        }
        #endregion

        #region GUI Events
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            // Note: Code below was commented out because it was moved to Extension Change event
            // The idea here is that we change controls when the device type is discovered or changed
            // For Wiimote extensions, the type should be passed to the WiimoteControl.
            // If connection fails we inform the user and re-enable the Connect button.
            // If the controller type is unknown or not supported, the user has the option to
            // disconnect from the controller and the control element should look disabled

            //bool success = false;

            // We can set the sharing mode type, None fixes Dark Souls and may work on Windows 10 now.
            //if (_dummy == null)
            //    _stream.SharingMode = System.IO.FileShare.None;

            var didConnect = Connect();

            //btnDisconnect.IsEnabled = didConnect;
            //btnConnect.IsEnabled = !didConnect;

            //if (success)
            //{
            //    btnDisconnect.IsEnabled = true;
            //}
            //else
            //{
            //    MessageBox.Show("Could not connect to device!");
            //    btnConnect.IsEnabled = true;
            //}
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            //btnDisconnect.IsEnabled = false;
            Disconnect();
            //btnConnect.IsEnabled = true;
        }

        private void AssignMenu_Click(object sender, RoutedEventArgs e)
        {
            InputSelected(_selectedInput);
        }

        private void CopyMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_assignments[ShiftIndex].ContainsKey(_selectedInput))
            {
                _clipboard = _assignments[ShiftIndex][_selectedInput];
            }
        }

        private void PasteMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_assignments[ShiftIndex].ContainsKey(_selectedInput))
            {
                _assignments[ShiftIndex][_selectedInput] = _clipboard;
            }
            else
            {
                _assignments[ShiftIndex].Add(_selectedInput, _clipboard);
            }
        }

        private void ClearMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_assignments[ShiftIndex].ContainsKey(_selectedInput))
            {
                _assignments[ShiftIndex].Remove(_selectedInput);
            }
        }
        #endregion

        #region Control Events
        private void SetLeds(bool[] values)
        {
            if (_nintroller != null && (values ?? new bool[0]).Length == 4)
            {
                _nintroller.Led1 = values[0];
                _nintroller.Led2 = values[1];
                _nintroller.Led3 = values[2];
                _nintroller.Led4 = values[3];
            }
        }

        private void InputSelected(string key)
        {
            InputsWindow win;
            if (_assignments[ShiftIndex].ContainsKey(key))
            {
                win = new InputsWindow(this, _assignments[ShiftIndex][key]);
            }
            else
            {
                win = new InputsWindow(this);
            }

            win.ShowDialog();

            if (win.Cancelled) return;

            if (_assignments[ShiftIndex].ContainsKey(key))
            {
                // If replacing a Shift Assignment, clear others that were set from the code below
                if (_assignments[ShiftIndex][key].Assignments.Count == 1 && _assignments[ShiftIndex][key].Assignments[0] is ShiftAssignment)
                {
                    var shift = _assignments[ShiftIndex][key].Assignments[0] as ShiftAssignment;
                    if (shift.Toggles)
                    {
                        foreach (var state in shift.ToggleStates)
                        {
                            if ((int)state != ShiftIndex)
                            {
                                _assignments[(int)state].Remove(key);
                            }
                        }
                    }
                    else if ((int)shift.TargetState != ShiftIndex)
                    {
                        _assignments[(int)shift.TargetState].Remove(key);
                    }
                }

                _assignments[ShiftIndex][key] = win.Result;
            }
            else
            {
                _assignments[ShiftIndex].Add(key, win.Result);
            }

            // Shift assignments need to be the same on each ShiftIndex
            if (win.Result.ShiftAssignment)
            {
                var shift = win.Result.Assignments[0] as ShiftAssignment;

                if (shift.Toggles)
                {
                    foreach (var state in shift.ToggleStates)
                    {
                        InputSet(state, key, win.Result);
                    }
                }
                else
                {
                    InputSet(shift.TargetState, key, win.Result);
                }
            }
        }

        private void InputSet(ShiftState shift, string key, AssignmentCollection assignments)
        {
            if (_assignments[(int)shift].ContainsKey(key))
            {
                _assignments[(int)shift][key] = assignments;
            }
            else
            {
                _assignments[(int)shift].Add(key, assignments);
            }
        }

        private void InputOpenMenu(string e)
        {
            _selectedInput = e;
            subMenu.IsOpen = true;
        }

        private void QuickAssignment(Dictionary<string, AssignmentCollection> assignments)
        {
            foreach (var item in assignments)
            {
                if (_assignments[ShiftIndex].ContainsKey(item.Key))
                {
                    _assignments[ShiftIndex][item.Key] = item.Value;
                }
                else
                {
                    _assignments[ShiftIndex].Add(item.Key, item.Value);
                }
            }
        }

        private void dropShift_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentState = (ShiftState)dropShift.SelectedIndex;
        }
        #endregion
    }

    public interface INintyControl
    {
        event Shared.Delegates.BoolArrDel OnChangeLEDs;
        event Shared.Delegates.StringDel OnInputSelected;
        event Shared.Delegates.StringDel OnInputRightClick;
        event AssignmentCollection.AssignDelegate OnQuickAssign;

        void ApplyInput(INintrollerState state);
        void UpdateVisual(INintrollerState state);
        void ChangeLEDs(bool one, bool two, bool three, bool four);
    }
}
