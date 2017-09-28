using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using NintrollerLib;
using Shared.Windows;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for NintyControl.xaml
    /// </summary>
    public partial class NintyControl : UserControl, IDeviceControl
    {
        public delegate void TypeUpdate(ControllerType type);
        public event TypeUpdate OnTypeChange;       // Called on extension changes
        public event Action OnDisconnect;           // Called when disconnected
        public event Action<DevicePrefs> OnPrefsChange;

        internal WinBtStream _stream;               // Controller stream to the device
        internal Nintroller _nintroller;            // Physical Controller Device
        internal INintyControl _controller;         // Visual Controller Representation
        internal AssignmentCollection _clipboard;   // Assignments to be pasted
        internal string _selectedInput;             // Controller's input to be effected by change
        internal ShiftState _currentState;          // Current shift state being applied
        internal Shared.DeviceInfo _info;           // Info on this device, HID path, Type

        internal ScpDirector _scp;                  // Quick reference to SCP Director
        internal Dictionary<string, AssignmentCollection>[] _assignments;

        private bool[] _rumbleSubscriptions = new bool[4];
        private bool _setup;

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

        public Shared.DeviceInfo GetDeviceInfo()
        {
            return _info;
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

                _nintroller.Disconnected += _nintroller_Disconnected;
            }
#if DEBUGz
            else
            {
                _controller = new ProControl(Calibrations.Defaults.ProControllerDefault);
                SetupController();
                success = true;
            }
#else
            else
            {
                //MessageBox.Show("Could not connect to device!");
                success = false;
            }
#endif
            return success;
        }

        public void Disconnect()
        {
            _nintroller.RumbleEnabled = false;

            if (_stream != null)
                _stream.Close();

            if (_dummy != null)
                _dummy.Close();

            if (_controller != null && _setup)
            {
                _controller.OnChangeLEDs -= SetLeds;
                _controller.OnInputSelected -= InputSelected;
                _controller.OnInputRightClick -= InputOpenMenu;
                _controller.OnQuickAssign -= QuickAssignment;
                _controller.OnRemoveInputs -= RemoveAssignments;

                if (_controller is WiiControl)
                {
                    ((WiiControl)_controller).OnJoystickCalibrated -= _nintroller_JoystickCalibrated;
                    ((WiiControl)_controller).OnTriggerCalibrated -= _nintroller_TriggerCalibrated;
                }

                _setup = false;
            }

            _view.Child = null;
            _controller = null;

            OnDisconnect?.Invoke();
        }

        public void AddRumble(bool state)
        {
            // TODO: Consider when one is turned off while another is still on
            _nintroller.RumbleEnabled = state;
        }

        private void LoadCalibrations(DevicePrefs prefs)
        {
            ProController proCalibration = Calibrations.Defaults.ProControllerDefault;
            Wiimote wmCalibration = Calibrations.Defaults.WiimoteDefault;
            Nunchuk nunCalibration = Calibrations.Defaults.NunchukDefault;
            ClassicController ccCalibration = Calibrations.Defaults.ClassicControllerDefault;
            ClassicControllerPro ccpCalibration = Calibrations.Defaults.ClassicControllerProDefault;
            
            foreach (var calibrationFile in prefs.calibrationFiles)
            {
                switch (calibrationFile.Key)
                {
                    case App.CAL_NUN_JOYSTICK:
                    case App.CAL_CC_LJOYSTICK:
                    case App.CAL_CC_RJOYSTICK:
                    case App.CAL_CCP_LJOYSTICK:
                    case App.CAL_CCP_RJOYSTICK:
                    case App.CAL_PRO_LJOYSTICK:
                    case App.CAL_PRO_RJOYSTICK:
                        Joystick joystick;
                        if (App.LoadFromFile<Joystick>(calibrationFile.Value, out joystick))
                        {
                            if (calibrationFile.Key == App.CAL_NUN_JOYSTICK) nunCalibration.joystick = joystick;
                            else if (calibrationFile.Key == App.CAL_CC_LJOYSTICK) ccCalibration.LJoy = joystick;
                            else if (calibrationFile.Key == App.CAL_CC_RJOYSTICK) ccCalibration.RJoy = joystick;
                            else if (calibrationFile.Key == App.CAL_CCP_LJOYSTICK) ccpCalibration.LJoy = joystick;
                            else if (calibrationFile.Key == App.CAL_CCP_RJOYSTICK) ccpCalibration.RJoy = joystick;
                            else if (calibrationFile.Key == App.CAL_PRO_LJOYSTICK) proCalibration.LJoy = joystick;
                            else if (calibrationFile.Key == App.CAL_PRO_RJOYSTICK) proCalibration.RJoy = joystick;
                        }
                        break;

                    case App.CAL_CC_LTRIGGER:
                    case App.CAL_CC_RTRIGGER:
                        NintrollerLib.Trigger trigger;
                        if (App.LoadFromFile<NintrollerLib.Trigger>(calibrationFile.Value, out trigger))
                        {
                            if (calibrationFile.Key == App.CAL_CC_LTRIGGER) ccCalibration.L = trigger;
                            else if (calibrationFile.Key == App.CAL_CC_RTRIGGER) ccCalibration.R = trigger;
                        }
                        break;

                    case App.CAL_WII_IR:
                        IR ir;
                        if (App.LoadFromFile<IR>(calibrationFile.Value, out ir))
                        {
                            wmCalibration.irSensor.boundingArea = ir.boundingArea;
                        }
                        break;
                }
            }

            _nintroller.SetCalibration(proCalibration);
            _nintroller.SetCalibration(wmCalibration);
            _nintroller.SetCalibration(nunCalibration);
            _nintroller.SetCalibration(ccCalibration);
            _nintroller.SetCalibration(ccpCalibration);
        }

        // This attribute will allow Access Violation exceptions to be caught in try/catch
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private void CreateController(ControllerType type)
        {
            var prefs = AppPrefs.Instance.GetDevicePreferences(_info.DevicePath);
            if (prefs != null) LoadCalibrations(prefs);

            switch (type)
            {
                case ControllerType.ProController:
                    _controller = new ProControl();
                    ((ProControl)_controller).OnJoyCalibrated += _nintroller_JoystickCalibrated;
                    break;

                case ControllerType.Wiimote:
                case ControllerType.PartiallyInserted:
                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                case ControllerType.ClassicController:
                case ControllerType.ClassicControllerPro:
                    _controller = new WiiControl();
                    ((WiiControl)_controller).OnChangeCameraMode += (mode) =>
                    {
                        _nintroller.IRMode = mode;
                    };
                    ((WiiControl)_controller).OnChangeCameraSensitivty += (sen) =>
                    {
                        _nintroller.IRSensitivity = sen;
                    };
                    ((WiiControl)_controller).OnJoystickCalibrated += _nintroller_JoystickCalibrated;
                    ((WiiControl)_controller).OnTriggerCalibrated += _nintroller_TriggerCalibrated;
                    ((WiiControl)_controller).OnIRCalibrated += _nintroller_IRCalibrated;
                    break;
            }
        }

        private void SetupController()
        {
            if (_controller != null)
            {
                if (!_setup)
                {
                    _controller.ObtainDeviceInfoDel = GetDeviceInfo;
                    _controller.ChangeLEDs(_nintroller.Led1, _nintroller.Led2, _nintroller.Led3, _nintroller.Led4);
                    _controller.OnChangeLEDs += SetLeds;
                    _controller.OnInputSelected += InputSelected;
                    _controller.OnInputRightClick += InputOpenMenu;
                    _controller.OnQuickAssign += QuickAssignment;
                    _controller.OnRemoveInputs += RemoveAssignments;
                    _setup = true;
                }

                _view.Child = _controller as UserControl;
                ((UserControl)_view.Child).HorizontalAlignment = HorizontalAlignment.Left;
                ((UserControl)_view.Child).VerticalAlignment = VerticalAlignment.Top;
            }
        }

        public void LoadProfile(string fileName)
        {
            AssignmentProfile loadedProfile = null;
            
            if (!App.LoadFromFile<AssignmentProfile>(fileName, out loadedProfile))
            {
                var c = MessageBox.Show("Could not open or read the profile file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (loadedProfile != null)
            {
                _assignments = loadedProfile.ToAssignmentArray(this);
            }
        }

        #region Nintroller Events
        private void _nintroller_LowBattery(object sender, LowBatteryEventArgs e)
        {
            // Indicate that this controller's battery is low
            System.Speech.Synthesis.SpeechSynthesizer s = new System.Speech.Synthesis.SpeechSynthesizer();
            s.SelectVoiceByHints(System.Speech.Synthesis.VoiceGender.Female);
            s.SpeakAsync("Low Battery");
        }

        private void _nintroller_Disconnected(object sender, DisconnectedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                Disconnect();

                if (!AppPrefs.Instance.suppressConnectionLost)
                    MessageBox.Show("Lost Controller Connection");
            }));
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
                foreach (var input in e.state.ToArray())
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

            // Send VJoy Changes
            VJoyDirector.Access.ApplyAll();

            // Visaul should only be updated if tab is in view
            Dispatcher.Invoke(new Action(() =>
            {
                if (_controller != null && MainWindow.CurrentTab == this)
                {
                    _controller.UpdateVisual(e.state);
                }
            }));
        }

        private void _nintroller_JoystickCalibrated(Joystick calibration, string target, string file = "")
        {
            switch (target)
            {
                case App.CAL_NUN_JOYSTICK:
                    var nCal = _nintroller.StoredCalibrations.NunchukCalibration;
                    nCal.joystick = calibration;
                    _nintroller.SetCalibration(nCal);
                    break;

                case App.CAL_CC_LJOYSTICK:
                case App.CAL_CC_RJOYSTICK:
                    var ccCal = _nintroller.StoredCalibrations.ClassicCalibration;
                    if (target.EndsWith("L")) ccCal.LJoy = calibration;
                    else ccCal.RJoy = calibration;
                    _nintroller.SetCalibration(ccCal);
                    break;

                case App.CAL_CCP_LJOYSTICK:
                case App.CAL_CCP_RJOYSTICK:
                    var ccpCal = _nintroller.StoredCalibrations.ClassicProCalibration;
                    if (target.EndsWith("L")) ccpCal.LJoy = calibration;
                    else ccpCal.RJoy = calibration;
                    _nintroller.SetCalibration(ccpCal);
                    break;

                case App.CAL_PRO_LJOYSTICK:
                case App.CAL_PRO_RJOYSTICK:
                    var proCal = _nintroller.StoredCalibrations.ProCalibration;
                    if (target.EndsWith("L")) proCal.LJoy = calibration;
                    else proCal.RJoy = calibration;
                    _nintroller.SetCalibration(proCal);
                    break;
            }

            AppPrefs.Instance.PromptToSaveCalibration(_info.DevicePath, target, file);
        }

        private void _nintroller_TriggerCalibrated(NintrollerLib.Trigger calibration, string target, string file = "")
        {
            switch (target)
            {
                case "ccLT":
                    var lCal = _nintroller.StoredCalibrations.ClassicCalibration;
                    lCal.L = calibration;
                    _nintroller.SetCalibration(lCal);
                    break;

                case "ccRT":
                    var rCal = _nintroller.StoredCalibrations.ClassicCalibration;
                    rCal.R = calibration;
                    _nintroller.SetCalibration(rCal);
                    break;
            }

            AppPrefs.Instance.PromptToSaveCalibration(_info.DevicePath, target, file);
        }

        private void _nintroller_IRCalibrated(Windows.IRCalibration calibration, string file = "")
        {
            var wmCal = _nintroller.StoredCalibrations.WiimoteCalibration;
            wmCal.irSensor.boundingArea = calibration.boundry;
            _nintroller.SetCalibration(wmCal);

            AppPrefs.Instance.PromptToSaveCalibration(_info.DevicePath, App.CAL_WII_IR, file);
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = _nintroller.Type.ToString() + "_profile";
            dialog.DefaultExt = ".wup";
            dialog.Filter = App.PROFILE_FILTER;

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                App.SaveToFile<AssignmentProfile>(dialog.FileName, new AssignmentProfile(_assignments));
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = _nintroller.Type.ToString() + "_profile";
            dialog.DefaultExt = ".wup";
            dialog.Filter = App.PROFILE_FILTER;

            bool? doLoad = dialog.ShowDialog();

            if (doLoad == true && dialog.CheckFileExists)
            {
                LoadProfile(dialog.FileName);
            }
        }

        private void btnAddRumble_Click(object sender, RoutedEventArgs e)
        {
            var win = new Windows.RumbleWindow(_rumbleSubscriptions);
            win.ShowDialog();

            for (byte i = 0; i < 4; i++)
            {
                if (win.Result[i])
                {
                    ScpDirector.Access.SubscribeToRumble((ScpDirector.XInput_Device)(i + 1), ApplyRumble);
                }
                else if (_rumbleSubscriptions[i])
                {
                    ScpDirector.Access.UnSubscribeToRumble((ScpDirector.XInput_Device)(i + 1), ApplyRumble);
                }
            }

            _rumbleSubscriptions = win.Result;
        }

        private void btnPrefs_Click(object sender, RoutedEventArgs e)
        {
            var prefs = AppPrefs.Instance.GetDevicePreferences(_info.DevicePath);
            if (prefs == null)
            {
                prefs = new DevicePrefs()
                {
                    deviceId = _info.DevicePath,
                    nickname = _nintroller.Type.ToString()
                };
            }

            var win = new Windows.DevicePrefsWindow(prefs);
            win.ShowDialog();

            if (win.DoSave)
            {
                AppPrefs.Instance.SaveDevicePrefs(win.Preferences);
                OnPrefsChange?.Invoke(win.Preferences);
            }

            win = null;
        }

        private void dropShift_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentState = (ShiftState)dropShift.SelectedIndex;
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

            if (!win.Apply) return;

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

        private void RemoveAssignments(string[] list)
        {
            foreach (var item in list)
            {
                if (_assignments[ShiftIndex].ContainsKey(item))
                    _assignments[ShiftIndex].Remove(item);
            }
        }
        
        private void ApplyRumble(byte leftMotor, byte rightMotor)
        {
            if (leftMotor == 0 && rightMotor == 0)
            {
                _nintroller.RumbleEnabled = false;
            }
            else
            {
                // TODO: Handle soft rumble
                _nintroller.RumbleEnabled = true;
            }
        }
        #endregion
    }

    public interface INintyControl : IBaseControl
    {
        event Shared.Delegates.BoolArrDel OnChangeLEDs;
        void ApplyInput(INintrollerState state); // TOOD: I have forgotten what I want to use this for
        void UpdateVisual(INintrollerState state);
        void ChangeLEDs(bool one, bool two, bool three, bool four);
    }
}