using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using SharpDX.DirectInput;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for JoyControl.xaml
    /// </summary>
    public partial class JoyControl : UserControl, IDeviceControl
    {
        public enum JoystickType
        {
            LeftJoyCon = 0x2006,
            RightJoyCon = 0x2007,
            SwitchPro = 0x2009,
            Generic = 0
        }

        public static string ToName(JoystickType type)
        {
            switch (type)
            {
                case JoystickType.LeftJoyCon:
                    return "Joy-Con (L)";
                case JoystickType.RightJoyCon:
                    return "Joy-Con (R)";
                case JoystickType.SwitchPro:
                    return "Switch Pro";
                case JoystickType.Generic:
                default:
                    return "Generic Joystick";
            }
        }

        public bool Connected { get { return _readTask != null; } }

        public delegate void JoyUpdate(Joystick joystick, JoystickUpdate[] updates);
        event JoyUpdate OnUpdate;
        public event Action OnDisconnect;
        public JoyControl associatedJoyCon = null;
        public bool isChild = false;
        public IJoyControl Control { get { return _controller; } }
        public Dictionary<JoystickOffset, AxisCalibration> calibrations;
        public event Action<DevicePrefs> OnPrefsChange;

        internal Joystick _joystick;
        internal JoystickState _state;
        internal IJoyControl _controller;
        internal AssignmentCollection _clipboard;
        internal string _selectedInput;
        internal ShiftState _currentState;
        internal Shared.DeviceInfo _info;
        internal ScpDirector _scp;
        internal Dictionary<string, AssignmentCollection>[] _assignments;
        private bool[] _rumbleSubscriptions = new bool[4];
        private Task _readTask;
        private CancellationTokenSource _readCancel;

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

        public JoystickType Type
        {
            get
            {
                if (Enum.IsDefined(typeof(JoystickType), _joystick.Properties.ProductId))
                {
                    return (JoystickType)_joystick.Properties.ProductId;
                }
                else
                {
                    return JoystickType.Generic;
                }
            }
        }

        public JoyControl()
        {
            _currentState = ShiftState.None;
            InitializeComponent();
        }

        public JoyControl(Shared.DeviceInfo deviceInfo) : this()
        {
            _assignments = new[] {
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>()
            };

            _info = deviceInfo;
            _scp = ScpDirector.Access;
            _joystick = new Joystick(MainWindow.DInput, _info.InstanceGUID);
            calibrations = new Dictionary<JoystickOffset, AxisCalibration>();

            if ((JoystickType)_joystick.Properties.ProductId == JoystickType.LeftJoyCon)
            {
                _controller = new JoyConLControl();
            }
            else if ((JoystickType)_joystick.Properties.ProductId == JoystickType.RightJoyCon)
            {
                _controller = new JoyConRControl();
            }
            else if ((JoystickType)_joystick.Properties.ProductId == JoystickType.SwitchPro)
            {
                _controller = new SwitchProControl();
                ((SwitchProControl)_controller).OnJoystickCalibrated += SwitchProJoystickCalibrated;
            }
            else
            {
                SetupJoystick();
            }
            
            if (_controller != null)
            {
                _controller.OnInputSelected += OnInputSelected;
                _controller.OnInputRightClick += OnInputRightClick;
                _controller.OnQuickAssign += OnQuickAssign;
                _stack.Children.Add((UserControl)_controller);
            }

            OnUpdate += JoyControl_OnUpdate;

            var prefs = AppPrefs.Instance.GetDevicePreferences(_info.DeviceID);
            if (prefs != null) LoadCalibrations(prefs);
        }

        private void LoadCalibrations(DevicePrefs prefs)
        {
            foreach (var calibrationFile in prefs.calibrationFiles)
            {
                switch (calibrationFile.Key)
                {
                    case App.CAL_SWP_LJOYSTICK:
                        NintrollerLib.Joystick lJoy;
                        if (_controller is SwitchProControl && App.LoadFromFile(calibrationFile.Value, out lJoy))
                        {
                            AxisCalibration xCalibration = new AxisCalibration(lJoy.minX, lJoy.maxX, lJoy.centerX, lJoy.deadXn, lJoy.deadXp);
                            AxisCalibration yCalibration = new AxisCalibration(lJoy.minY, lJoy.maxY, lJoy.centerY, lJoy.deadYn, lJoy.deadYp);
                            calibrations[JoystickOffset.X] = xCalibration;
                            calibrations[JoystickOffset.Y] = yCalibration;
                            ((SwitchProControl)_controller).leftXCalibration = xCalibration;
                            ((SwitchProControl)_controller).leftYCalibration = yCalibration;
                        }
                        break;

                    case App.CAL_SWP_RJOYSTICK:
                        NintrollerLib.Joystick rJoy;
                        if (_controller is SwitchProControl && App.LoadFromFile(calibrationFile.Value, out rJoy))
                        {
                            AxisCalibration xCalibration = new AxisCalibration(rJoy.minX, rJoy.maxX, rJoy.centerX, rJoy.deadXn, rJoy.deadXp);
                            AxisCalibration yCalibration = new AxisCalibration(rJoy.minY, rJoy.maxY, rJoy.centerY, rJoy.deadYn, rJoy.deadYp);
                            calibrations[JoystickOffset.RotationX] = xCalibration;
                            calibrations[JoystickOffset.RotationY] = yCalibration;
                            ((SwitchProControl)_controller).rightXCalibration = xCalibration;
                            ((SwitchProControl)_controller).rightYCalibration = yCalibration;
                        }
                        break;

                    default:
                        AxisCalibration axisCalibration;
                        if (App.LoadFromFile<AxisCalibration>(calibrationFile.Value, out axisCalibration))
                        {
                            JoystickOffset offset = JoystickOffset.X;
                            if (!Enum.TryParse(calibrationFile.Key, out offset)) continue;
                            calibrations[offset] = axisCalibration;
                        }
                        break;
                }
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
                if (loadedProfile.SubName == Type.ToString())
                {
                    _assignments = loadedProfile.SubProfile.ToAssignmentArray(this);
                }
                else
                {
                    _assignments = loadedProfile.ToAssignmentArray(this);
                }

                if (associatedJoyCon != null && loadedProfile.SubName == associatedJoyCon.Type.ToString())
                {
                    associatedJoyCon._assignments = loadedProfile.SubProfile.ToAssignmentArray(this);
                }
                else if (associatedJoyCon != null)
                {
                    associatedJoyCon._assignments = loadedProfile.ToAssignmentArray(this);
                }
            }
        }

        public void AssociateJoyCon(JoyControl joy)
        {
            associatedJoyCon = joy;
            isChild = false;
            joy.isChild = true;

            var parent = (Panel)((UserControl)joy.Control).Parent;
            if (parent != null)
            {
                parent.Children.Remove((UserControl)joy.Control);
            }
            
            if (associatedJoyCon.Type == JoystickType.LeftJoyCon)
            {
                _stack.Children.Insert(0, (UserControl)associatedJoyCon.Control);
            }
            else
            {
                _stack.Children.Add((UserControl)associatedJoyCon.Control);
            }

            var tab = Parent as TabItem;
            var stack = tab == null ? null : tab.Header as StackPanel;
            if (stack != null && stack.Children.Count > 1 && stack.Children[1].GetType() == typeof(TextBlock))
            {
                ((TextBlock)stack.Children[1]).Text = "Joy-Cons";
                if (stack.Children[0].GetType() == typeof(Image))
                {
                    ((Image)stack.Children[0]).Source = new BitmapImage(new Uri("../Images/Icons/switch_jc_black.png", UriKind.Relative));
                }
            }
        }

        private void JoyControl_OnUpdate(Joystick joystick, JoystickUpdate[] updates)
        {
            if (_assignments != null)
            {
                foreach (var update in updates)
                {
                    _state.Update(update);
                }

                for (int b = 0; b < _joystick.Capabilities.ButtonCount; b++)
                {
                    var key = "Buttons" + b.ToString();
                    if (_assignments[ShiftIndex].ContainsKey(key))
                    {
                        _assignments[ShiftIndex][key].ApplyAll(_state.Buttons[b] ? 1 : 0);
                    }
                }

                for (int p = 0; p < _joystick.Capabilities.PovCount; p++)
                {
                    int value = _state.PointOfViewControllers[p];
                    bool north = false, south = false, east = false, west = false;
                    if (value != -1)
                    {
                        north = value > 27000 || value < 9000;
                        south = !north && value < 27000 && value > 9000;
                        east = value > 0 && value < 18000;
                        west = !east && value > 18000;
                    }
                    if (_assignments[ShiftIndex].ContainsKey("pov" + p.ToString() + "N")) _assignments[ShiftIndex]["pov" + p.ToString() + "N"].ApplyAll(north ? 1 : 0);
                    if (_assignments[ShiftIndex].ContainsKey("pov" + p.ToString() + "S")) _assignments[ShiftIndex]["pov" + p.ToString() + "S"].ApplyAll(south ? 1 : 0);
                    if (_assignments[ShiftIndex].ContainsKey("pov" + p.ToString() + "E")) _assignments[ShiftIndex]["pov" + p.ToString() + "E"].ApplyAll(east ? 1 : 0);
                    if (_assignments[ShiftIndex].ContainsKey("pov" + p.ToString() + "W")) _assignments[ShiftIndex]["pov" + p.ToString() + "W"].ApplyAll(west ? 1 : 0);
                }

                // Link the X Axis & Y Axis together (Y is inverted)
                if (calibrations.ContainsKey(JoystickOffset.X) && calibrations.ContainsKey(JoystickOffset.Y))
                {
                    var joyL = ConvertToNintyJoy(calibrations[JoystickOffset.X], calibrations[JoystickOffset.Y]);
                    joyL.rawX = _state.X;
                    joyL.rawY = 65535 - _state.Y;
                    joyL.Normalize();

                    if (_assignments[ShiftIndex].ContainsKey("X+")) _assignments[ShiftIndex]["X+"].ApplyAll(joyL.X > 0 ? joyL.X : 0);
                    if (_assignments[ShiftIndex].ContainsKey("X-")) _assignments[ShiftIndex]["X-"].ApplyAll(joyL.X > 0 ? 0 : -joyL.X);
                    if (_assignments[ShiftIndex].ContainsKey("Y+")) _assignments[ShiftIndex]["Y+"].ApplyAll(joyL.Y > 0 ? joyL.Y : 0);
                    if (_assignments[ShiftIndex].ContainsKey("Y-")) _assignments[ShiftIndex]["Y-"].ApplyAll(joyL.Y > 0 ? 0 : -joyL.Y);
                }
                else
                {
                    if (calibrations.ContainsKey(JoystickOffset.X)) UpdateAxis(JoystickOffset.X, _state.X);
                    if (calibrations.ContainsKey(JoystickOffset.Y)) UpdateAxis(JoystickOffset.Y, _state.Y);
                }

                // Link Rotation X & Rotation Y Axes together (Rotation Y is inverted)
                if (calibrations.ContainsKey(JoystickOffset.RotationX) && calibrations.ContainsKey(JoystickOffset.RotationY))
                {
                    var joyR = ConvertToNintyJoy(calibrations[JoystickOffset.RotationX], calibrations[JoystickOffset.RotationY]);
                    joyR.rawX = _state.RotationX;
                    joyR.rawY = 65535 - _state.RotationY;
                    joyR.Normalize();

                    if (_assignments[ShiftIndex].ContainsKey("RotationX+")) _assignments[ShiftIndex]["RotationX+"].ApplyAll(joyR.X > 0 ? joyR.X : 0);
                    if (_assignments[ShiftIndex].ContainsKey("RotationX-")) _assignments[ShiftIndex]["RotationX-"].ApplyAll(joyR.X > 0 ? 0 : -joyR.X);
                    if (_assignments[ShiftIndex].ContainsKey("RotationY+")) _assignments[ShiftIndex]["RotationY+"].ApplyAll(joyR.Y > 0 ? joyR.Y : 0);
                    if (_assignments[ShiftIndex].ContainsKey("RotationY-")) _assignments[ShiftIndex]["RotationY-"].ApplyAll(joyR.Y > 0 ? 0 : -joyR.Y);
                }
                else
                {
                    if (calibrations.ContainsKey(JoystickOffset.RotationX)) UpdateAxis(JoystickOffset.RotationX, _state.RotationX);
                    if (calibrations.ContainsKey(JoystickOffset.RotationY)) UpdateAxis(JoystickOffset.RotationY, _state.RotationY);
                }

                if (calibrations.ContainsKey(JoystickOffset.Z)) UpdateAxis(JoystickOffset.Z, _state.Z);
                if (calibrations.ContainsKey(JoystickOffset.RotationZ)) UpdateAxis(JoystickOffset.RotationZ, _state.RotationZ);
                if (calibrations.ContainsKey(JoystickOffset.Sliders0)) UpdateAxis(JoystickOffset.Sliders0, _state.Sliders[0]);
                if (calibrations.ContainsKey(JoystickOffset.Sliders1)) UpdateAxis(JoystickOffset.Sliders1, _state.Sliders[1]);
            }

            // TODO: Only apply what this controller emulates, if any
            _scp.ApplyAll();
            VJoyDirector.Access.ApplyAll();
            Dispatcher.Invoke(new Action(() =>
            {
                if (MainWindow.CurrentTab != this && !isChild)
                    return;

                if (_controller != null)
                {
                    _controller.UpdateVisual(updates);
                }
                else
                {
                    UpdateGenericVisual(updates);
                }
            }));
        }

        public void ChangeState(ShiftState newState)
        {
            if (_currentState != newState)
            {
                // TODO: This would unpress any keys from all controllers, make it device specific
                KeyboardDirector.Access.Release();
                MouseDirector.Access.Release();
                _currentState = newState;
            }
        }

        public bool Connect()
        {
            // This isn't doing what I want it to :(  (at least on Windows 10)
            //if (Shared.Windows.WinBtStream.OverrideSharingMode && Shared.Windows.WinBtStream.OverridenFileShare == System.IO.FileShare.None)
            //{
            //    //_joystick.SetCooperativeLevel(new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            //    var Handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            //    _joystick.SetCooperativeLevel(Handle, CooperativeLevel.Exclusive | CooperativeLevel.Background);
            //}

            _joystick.Properties.BufferSize = 128;
            _joystick.Acquire();
            
            _state = _joystick.GetCurrentState();

            // Setup Calibrations if they don't exist already
            if (!calibrations.ContainsKey(JoystickOffset.X) && _state.X > 0) calibrations.Add(JoystickOffset.X, new AxisCalibration(0, 65535, 32767, 2048));
            if (!calibrations.ContainsKey(JoystickOffset.Y) && _state.Y > 0) calibrations.Add(JoystickOffset.Y, new AxisCalibration(0, 65535, 32767, 2048));
            if (!calibrations.ContainsKey(JoystickOffset.Z) && _state.Z > 0) calibrations.Add(JoystickOffset.Z, new AxisCalibration(0, 65535, 32767, 2048));
            if (!calibrations.ContainsKey(JoystickOffset.RotationX) && _state.RotationX > 0) calibrations.Add(JoystickOffset.RotationX, new AxisCalibration(0, 65535, 32767, 2048));
            if (!calibrations.ContainsKey(JoystickOffset.RotationY) && _state.RotationY > 0) calibrations.Add(JoystickOffset.RotationY, new AxisCalibration(0, 65535, 32767, 2048));
            if (!calibrations.ContainsKey(JoystickOffset.RotationZ) && _state.RotationZ > 0) calibrations.Add(JoystickOffset.RotationZ, new AxisCalibration(0, 65535, 32767, 2048));

            _readCancel = new CancellationTokenSource();
            _readTask = Task.Factory.StartNew(PollData, _readCancel.Token);

            return true;
        }

        public void Disconnect()
        {
            if (associatedJoyCon != null)
            {
                _stack.Children.Remove((UserControl)associatedJoyCon.Control);
                associatedJoyCon._stack.Children.Add((UserControl)associatedJoyCon.Control);
                associatedJoyCon.Disconnect();
                associatedJoyCon = null;
            }

            if (_controller != null)
            {
                _controller.OnInputSelected -= OnInputSelected;
                _controller.OnInputRightClick -= OnInputRightClick;
                _controller.OnQuickAssign -= OnQuickAssign;

                if (_controller is SwitchProControl)
                {
                    ((SwitchProControl)_controller).OnJoystickCalibrated -= SwitchProJoystickCalibrated;
                }
            }

            _readCancel?.Cancel();
            _readTask?.Wait(1000);
            _joystick?.Unacquire();
            _readTask = null;
            _readCancel = null;
            calibrations?.Clear();
            OnDisconnect?.Invoke();
        }

        public void AddRumble(bool state)
        {
            // TODO: add rumble if joystick is capable
            return;
        }

        private void PollData()
        {
            while (_readCancel != null && !_readCancel.Token.IsCancellationRequested)
            {
                try
                {
                    _joystick.Poll();
                    var data = _joystick.GetBufferedData();
                    OnUpdate(_joystick, data);
                }
                catch { /* Failed to read */ }

                // Reads 20 times a second
                Thread.Sleep(50);
            }
        }

        private void UpdateAxis(JoystickOffset offset, int value)
        {
            var key = offset.ToString();
            if (_assignments[ShiftIndex].ContainsKey(key + "+"))
                _assignments[ShiftIndex][key + "+"].ApplyAll(calibrations[offset].Normal(value, true));
            if (_assignments[ShiftIndex].ContainsKey(key + "-"))
                _assignments[ShiftIndex][key + "-"].ApplyAll(calibrations[offset].Normal(value, false));
        }

        public static NintrollerLib.Joystick ConvertToNintyJoy(AxisCalibration xAxis, AxisCalibration yAxis)
        {
            return new NintrollerLib.Joystick
            {
                minX = xAxis.min,
                minY = yAxis.min,
                centerX = xAxis.center,
                centerY = yAxis.center,
                maxX = xAxis.max,
                maxY = yAxis.max,
                deadXn = xAxis.deadNeg,
                deadYn = yAxis.deadNeg,
                deadXp = xAxis.deadPos,
                deadYp = yAxis.deadPos
            };
        }

        #region Control Events
        // Duplicate Code in these parts, Blech! But I (literally) don't have time for perfection
        // How about making IDeviceControl an abstract class instead of an interface, l ike I've now done with BaseControl

        private void OnQuickAssign(Dictionary<string, AssignmentCollection> assignments)
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

        private void OnInputRightClick(string str)
        {
            _selectedInput = str;
            subMenu.IsOpen = true;
        }

        private void OnInputSelected(string key)
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

        private void SwitchProJoystickCalibrated(NintrollerLib.Joystick joy, string target, string file = "")
        {
            AxisCalibration xCalibration = new AxisCalibration(joy.minX, joy.maxX, joy.centerX, joy.deadXn, joy.deadXp);
            AxisCalibration yCalibration = new AxisCalibration(joy.minY, joy.maxY, joy.centerY, joy.deadYn, joy.deadYp);

            if (target == App.CAL_SWP_RJOYSTICK)
            {
                calibrations[JoystickOffset.RotationX] = xCalibration;
                calibrations[JoystickOffset.RotationY] = yCalibration;
                ((SwitchProControl)_controller).rightXCalibration = xCalibration;
                ((SwitchProControl)_controller).rightYCalibration = yCalibration;
            }
            else
            {
                calibrations[JoystickOffset.X] = xCalibration;
                calibrations[JoystickOffset.Y] = yCalibration;
                ((SwitchProControl)_controller).leftXCalibration = xCalibration;
                ((SwitchProControl)_controller).leftYCalibration = yCalibration;
            }

            AppPrefs.Instance.PromptToSaveCalibration(_info.DeviceID, target, file);
        }
        #endregion

        #region GUI Events
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            var didConnect = Connect();
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = Type.ToString() + "_profile";
            dialog.DefaultExt = ".wup";
            dialog.Filter = App.PROFILE_FILTER;

            if (associatedJoyCon != null)
            {
                dialog.FileName = "Joy-Cons_profile";
            }

            bool? doSave = dialog.ShowDialog();

            if (doSave == true)
            {
                var profile = new AssignmentProfile(_assignments);

                if (associatedJoyCon != null)
                {
                    profile.SubProfile = new AssignmentProfile(associatedJoyCon._assignments);
                    profile.SubName = associatedJoyCon.Type.ToString();
                }

                App.SaveToFile<AssignmentProfile>(dialog.FileName, profile);
            }
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = Type.ToString() + "_profile";
            dialog.DefaultExt = ".wup";
            dialog.Filter = App.PROFILE_FILTER;

            if (associatedJoyCon != null)
            {
                dialog.FileName = "Joy-Cons_profile";
            }

            bool? doLoad = dialog.ShowDialog();

            if (doLoad == true && dialog.CheckFileExists)
            {
                LoadProfile(dialog.FileName);
            }
        }

        private void btnPrefs_Click(object sender, RoutedEventArgs e)
        {
            var prefs = AppPrefs.Instance.GetDevicePreferences(_info.DeviceID);
            if (prefs == null)
            {
                prefs = new DevicePrefs()
                {
                    deviceId = _info.DeviceID,
                    nickname = ToName(Type)
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

        private void btnAddRumble_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Link up rumble
        }

        private void dropShift_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentState = (ShiftState)dropShift.SelectedIndex;
        }

        private void AssignMenu_Click(object sender, RoutedEventArgs e)
        {
            OnInputSelected(_selectedInput);
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

        #region Generic Joystick Fields & Methods
        SolidColorBrush fillBrushOff = new SolidColorBrush(Color.FromArgb(0xFF, 0xC1, 0x39, 0x2B));
        SolidColorBrush fillBrushOn = new SolidColorBrush(Color.FromArgb(0xFF, 0x28, 0xAC, 0x60));
        GroupBox _buttonGroup;
        GroupBox _axisGroup;
        GroupBox _povGroup;
        Windows.AxisCalibrationWindow _openAxisCal;
        string _calibrationTarget;

        private void SetupJoystick()
        {
            var hStack = new StackPanel { Orientation = Orientation.Horizontal };
            var vStack = new StackPanel { Orientation = Orientation.Vertical };

            #region Buttons
            if (_joystick.Capabilities.ButtonCount > 0)
            {
                _buttonGroup = new GroupBox()
                {
                    Header = "Buttons",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 174,
                    Height = 220,
                    Style = (Style)Application.Current.Resources["GroupBoxStyle"]
                };

                ScrollViewer buttonScroll = new ScrollViewer();

                WrapPanel buttonWrap = new WrapPanel()
                {
                    Margin = new Thickness(0, 5, 0, 5),
                    ItemHeight = 34,
                    ItemWidth = 34
                };

                for (int b = 0; b < _joystick.Capabilities.ButtonCount; b++)
                {
                    Grid btn = new Grid
                    {
                        Tag = "Buttons" + b.ToString()
                    };

                    Ellipse btnCircle = new Ellipse()
                    {
                        Fill = fillBrushOff,
                        Stroke = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)),
                        StrokeThickness = 2,
                        Tag = "Buttons" + b.ToString()
                    };

                    Label btnText = new Label()
                    {
                        Content = (b+1).ToString(),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };

                    btn.MouseLeftButtonDown += Generic_MouseDown;
                    btn.MouseRightButtonDown += Generic_MouseRightClick;

                    btn.Children.Add(btnCircle);
                    btn.Children.Add(btnText);

                    buttonWrap.Children.Add(btn);
                }

                buttonScroll.Content = buttonWrap;
                _buttonGroup.Content = buttonScroll;
                hStack.Children.Add(_buttonGroup);
            }
            #endregion

            #region Axes
            WrapPanel axisWrap = new WrapPanel();
            _joystick.Acquire();
            var state = _joystick.GetCurrentState();
            _joystick.Unacquire();

            if (state.X > 0) AddAxis(axisWrap.Children, JoystickOffset.X);
            if (state.Y > 0) AddAxis(axisWrap.Children, JoystickOffset.Y);
            if (state.Z > 0) AddAxis(axisWrap.Children, JoystickOffset.Z);
            if (state.RotationX > 0) AddAxis(axisWrap.Children, JoystickOffset.RotationX);
            if (state.RotationY > 0) AddAxis(axisWrap.Children, JoystickOffset.RotationY);
            if (state.RotationZ > 0) AddAxis(axisWrap.Children, JoystickOffset.RotationZ);
            if (state.Sliders.Length > 0 && state.Sliders[0] > 0) AddAxis(axisWrap.Children, JoystickOffset.Sliders0);
            if (state.Sliders.Length > 1 && state.Sliders[1] > 0) AddAxis(axisWrap.Children, JoystickOffset.Sliders1);

            if (axisWrap.Children.Count > 0)
            {
                _axisGroup = new GroupBox()
                {
                    Header = "Axes",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 244,
                    Height = 220,
                    Style = (Style)Application.Current.Resources["GroupBoxStyle"]
                };

                ScrollViewer axisScroll = new ScrollViewer();

                axisScroll.Content = axisWrap;
                _axisGroup.Content = axisScroll;
                hStack.Children.Add(_axisGroup);
            }
            #endregion

            if (_joystick.Capabilities.PovCount > 0)
            {
                #region D-Pads
                _povGroup = new GroupBox()
                {
                    Header = "Directional Pads",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 244,
                    Height = hStack.Children.Count == 2 ? 80 : 220,
                    Style = (Style)Application.Current.Resources["GroupBoxStyle"]
                };

                ScrollViewer povScroll = new ScrollViewer();

                WrapPanel povWrap = new WrapPanel()
                {
                    Margin = new Thickness(0, 5, 0, 5)
                };

                for (int p = 0; p < _joystick.Capabilities.PovCount; p++)
                {
                    StackPanel povStack = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Tag = "PointOfViewControllers" + p.ToString()
                    };

                    povStack.Children.Add(new Label()
                    {
                        Content = "POV " + p.ToString()
                    });

                    Polygon up = new Polygon
                    {
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = fillBrushOff,
                        Width = 40,
                        Height = 40,
                        Stretch = Stretch.Fill,
                        Margin = new Thickness(2),
                        Points = new PointCollection
                        {
                            new Point(0.5, 0.0),
                            new Point(0.0, 1.0),
                            new Point(1.0, 1.0)
                        },
                        Tag = "pov0N"
                    };
                    up.MouseLeftButtonDown += Generic_MouseDown;
                    up.MouseRightButtonDown += Generic_MouseRightClick;
                    povStack.Children.Add(up);

                    Polygon right = new Polygon
                    {
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = fillBrushOff,
                        Width = 40,
                        Height = 40,
                        Stretch = Stretch.Fill,
                        Margin = new Thickness(2),
                        Points = new PointCollection
                        {
                            new Point(0.0, 1.0),
                            new Point(1.0, 0.5),
                            new Point(0.0, 0.0)
                        },
                        Tag = "pov0E"
                    };
                    right.MouseLeftButtonDown += Generic_MouseDown;
                    right.MouseRightButtonDown += Generic_MouseRightClick;
                    povStack.Children.Add(right);

                    Polygon down = new Polygon
                    {
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = fillBrushOff,
                        Width = 40,
                        Height = 40,
                        Stretch = Stretch.Fill,
                        Margin = new Thickness(2),
                        Points = new PointCollection
                        {
                            new Point(0.5, 1.0),
                            new Point(0.0, 0.0),
                            new Point(1.0, 0.0)
                        },
                        Tag = "pov0S"
                    };
                    down.MouseLeftButtonDown += Generic_MouseDown;
                    down.MouseRightButtonDown += Generic_MouseRightClick;
                    povStack.Children.Add(down);

                    Polygon left = new Polygon
                    {
                        Stroke = new SolidColorBrush(Colors.Black),
                        Fill = fillBrushOff,
                        Width = 40,
                        Height = 40,
                        Stretch = Stretch.Fill,
                        Margin = new Thickness(2),
                        Points = new PointCollection
                        {
                            new Point(0.0, 0.5),
                            new Point(1.0, 1.0),
                            new Point(1.0, 0.0)
                        },
                        Tag = "pov0W"
                    };
                    left.MouseLeftButtonDown += Generic_MouseDown;
                    left.MouseRightButtonDown += Generic_MouseRightClick;
                    povStack.Children.Add(left);

                    povWrap.Children.Add(povStack);
                }

                povScroll.Content = povWrap;
                _povGroup.Content = povScroll;

                if (hStack.Children.Count == 2)
                {
                    vStack.Children.Add(hStack);
                    vStack.Children.Add(_povGroup);
                    _stack.Children.Add(vStack);
                }
                else
                {
                    hStack.Children.Add(_povGroup);
                    _stack.Children.Add(hStack);
                }
                #endregion
            }
            else
            {
                _stack.Children.Add(hStack);
            }
        }

        private void AddAxis(UIElementCollection children, JoystickOffset offset)
        {
            StackPanel axisStack = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(2),
                Tag = offset.ToString()
            };

            Grid negBtn = new Grid() { Tag = offset.ToString() + "-" };
            Rectangle neg = new Rectangle
            {
                Width = 40,
                Height = 40,
                Fill = fillBrushOff,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            Label negLabel = new Label
            {
                Content = "-",
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            negBtn.MouseLeftButtonDown += Generic_MouseDown;
            negBtn.MouseRightButtonDown += Generic_MouseRightClick;
            negBtn.Children.Add(neg);
            negBtn.Children.Add(negLabel);

            Label calibrateLabel = new Label
            {
                Tag = offset.ToString(),
                Content = new TextBlock
                {
                    Text = "Calibrate",
                    TextDecorations = TextDecorations.Underline,
                    Foreground = new SolidColorBrush(Colors.DarkBlue)
                }
            };
            calibrateLabel.MouseLeftButtonDown += CalibrateAxis;

            axisStack.Children.Add(negBtn);
            axisStack.Children.Add(new StackPanel
            {
                Orientation = Orientation.Vertical,
                Children = { new Label { Content = offset.ToString(), Width = 70 }, calibrateLabel }
            });
            axisStack.Children.Add(new Label { Content = "0", Width = 50 });

            Grid posBtn = new Grid() { Tag = offset.ToString() + "+" };
            Rectangle pos = new Rectangle
            {
                Width = 40,
                Height = 40,
                Fill = fillBrushOff,
                Stroke = new SolidColorBrush(Colors.Black)
            };
            Label posLabel = new Label
            {
                Content = "+",
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            posBtn.MouseLeftButtonDown += Generic_MouseDown;
            posBtn.MouseRightButtonDown += Generic_MouseRightClick;
            posBtn.Children.Add(pos);
            posBtn.Children.Add(posLabel);

            axisStack.Children.Add(posBtn);
            children.Add(axisStack);
        }

        private void CalibrateAxis(object sender, MouseButtonEventArgs e)
        {
            _calibrationTarget = (sender as FrameworkElement).Tag.ToString();
            JoystickOffset offset = JoystickOffset.X;

            if (!Enum.TryParse(_calibrationTarget, out offset)) return;

            Windows.AxisCalibrationWindow axisCal = new Windows.AxisCalibrationWindow(calibrations[offset]);
            _openAxisCal = axisCal;
            axisCal.ShowDialog();

            if (axisCal.Apply)
            {
                calibrations[offset] = axisCal.Calibration;
                AppPrefs.Instance.PromptToSaveCalibration(_info.DeviceID, _calibrationTarget, axisCal.FileName);
            }

            axisCal = null;
        }

        private void UpdateGenericVisual(JoystickUpdate[] updates)
        {
            if (updates.Length == 0) return;

            foreach (var update in updates)
            {
                if (_openAxisCal != null && _calibrationTarget == update.Offset.ToString())
                {
                    _openAxisCal.Update(update.Value);
                }

                if (update.Offset >= JoystickOffset.Buttons0 && update.Offset <= JoystickOffset.Buttons127)
                {
                    if (_buttonGroup != null)
                    {
                        var buttonContainer = (_buttonGroup.Content as ScrollViewer).Content as WrapPanel;
                        foreach (var btnChild in buttonContainer.Children)
                        {
                            Grid btn = btnChild as Grid;
                            if (btn != null && btn.Tag.ToString() == update.Offset.ToString())
                            {
                                (btn.Children[0] as Ellipse).Fill = update.Value > 0 ? fillBrushOn : fillBrushOff;
                                break;
                            }
                        }
                    }
                }
                else if (update.Offset >= JoystickOffset.PointOfViewControllers0 && update.Offset <= JoystickOffset.PointOfViewControllers3)
                {
                    if (_povGroup != null)
                    {
                        var povContainer = (_povGroup.Content as ScrollViewer).Content as WrapPanel;
                        foreach (var povStack in povContainer.Children)
                        {
                            StackPanel pov = povStack as StackPanel;
                            if (pov != null && pov.Tag.ToString() == update.Offset.ToString())
                            {
                                bool north = false, south = false, east = false, west = false;
                                north = update.Value > -1 && (update.Value > 27000 || update.Value < 9000);
                                south = update.Value == 18000 || (update.Value < 27000 && update.Value > 9000);
                                east = update.Value == 9000 || (update.Value > 0 && update.Value < 18000);
                                west = update.Value == 27000 || (update.Value > 18000);

                                (pov.Children[1] as Polygon).Fill = north ? fillBrushOn : fillBrushOff;
                                (pov.Children[2] as Polygon).Fill = east ? fillBrushOn : fillBrushOff;
                                (pov.Children[3] as Polygon).Fill = south ? fillBrushOn : fillBrushOff;
                                (pov.Children[4] as Polygon).Fill = west ? fillBrushOn : fillBrushOff;
                                break;
                            }
                        }
                    }
                }
                else if (update.Offset < JoystickOffset.PointOfViewControllers0 && calibrations.ContainsKey(update.Offset))
                {
                    if (_axisGroup != null)
                    {
                        var axisContainer = (_axisGroup.Content as ScrollViewer).Content as WrapPanel;
                        foreach (var axisStack in axisContainer.Children)
                        {
                            StackPanel axis = axisStack as StackPanel;
                            if (axis != null && axis.Tag.ToString() == update.Offset.ToString())
                            {
                                var axisLabel = axis.Children[2] as Label;
                                if (axisLabel != null)
                                {
                                    axisLabel.Content = calibrations[update.Offset].Normal(update.Value).ToString("0.00");
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void Generic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                OnInputSelected((sender as FrameworkElement).Tag.ToString());
        }

        private void Generic_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            OnInputRightClick((sender as FrameworkElement).Tag.ToString());
        }
        #endregion
    }

    public interface IJoyControl : IBaseControl
    {
        void UpdateVisual(JoystickUpdate[] updates);
        Guid AssociatedInstanceID { get; }
    }

    public struct AxisCalibration
    {
        public int min;
        public int max;
        public int center;
        public int deadNeg;
        public int deadPos;

        public AxisCalibration(int min, int max, int center, int dead)
        {
            this.min = min;
            this.max = max;
            this.center = center;
            this.deadPos = dead;
            this.deadNeg = -dead;
        }

        public AxisCalibration(int min, int max, int center, int deadNeg, int deadPos)
        {
            this.min = min;
            this.max = max;
            this.center = center;
            this.deadNeg = deadNeg;
            this.deadPos = deadPos;
        }

        //public float Normal(int value)
        //{
        //    if (value == center) return 0;
        //    int v = value - center;
        //    if (v >= deadNeg && v <= deadPos) return 0;
        //    return v / ((max - min) / 2f);
        //}

        public float Normal(int value, bool positive = true)
        {
            if (value == center) return 0;

            int v = value - center;
            v *= positive ? 1 : -1;
            if (v >= deadNeg && v <= deadPos) return 0;
            return v / ((max - min) / 2f);
        }
    }
}
