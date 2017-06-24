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

        public delegate void JoyUpdate(Joystick joystick, JoystickUpdate[] updates);
        event JoyUpdate OnUpdate;
        public event Action OnDisconnect;
        public JoyControl associatedJoyCon = null;
        public IJoyControl Control { get { return _controller; } }
        public Dictionary<JoystickOffset, AxisCalibration> calibrations;

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
            }
            else
            {

            }
            
            if (_controller != null)
            {
                _controller.OnInputSelected += OnInputSelected;
                _controller.OnInputRightClick += OnInputRightClick;
                _controller.OnQuickAssign += OnQuickAssign;
                _stack.Children.Add((UserControl)_controller);
            }

            OnUpdate += JoyControl_OnUpdate;
        }
        
        public void AssociateJoyCon(JoyControl joy)
        {
            associatedJoyCon = joy;

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

                if (calibrations.ContainsKey(JoystickOffset.X)) UpdateAxis(JoystickOffset.X, _state.X);
                if (calibrations.ContainsKey(JoystickOffset.Y)) UpdateAxis(JoystickOffset.Y, _state.Y);
                if (calibrations.ContainsKey(JoystickOffset.Z)) UpdateAxis(JoystickOffset.Z, _state.Z);
                if (calibrations.ContainsKey(JoystickOffset.RotationX)) UpdateAxis(JoystickOffset.RotationX, _state.RotationX);
                if (calibrations.ContainsKey(JoystickOffset.RotationY)) UpdateAxis(JoystickOffset.RotationY, _state.RotationY);
                if (calibrations.ContainsKey(JoystickOffset.RotationZ)) UpdateAxis(JoystickOffset.RotationZ, _state.RotationZ);
                if (calibrations.ContainsKey(JoystickOffset.Sliders0)) UpdateAxis(JoystickOffset.Sliders0, _state.Sliders[0]);
                if (calibrations.ContainsKey(JoystickOffset.Sliders1)) UpdateAxis(JoystickOffset.Sliders1, _state.Sliders[1]);
            }

            // TODO: Only apply what this controller emulates, if any
            _scp.ApplyAll();
            VJoyDirector.Access.ApplyAll();
            Dispatcher.Invoke(new Action(() =>
            {
                _controller?.UpdateVisual(updates);
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
            if (_state.X > 0) calibrations.Add(JoystickOffset.X, new AxisCalibration(0, 65535, 32767, 256));
            if (_state.Y > 0) calibrations.Add(JoystickOffset.Y, new AxisCalibration(0, 65535, 32767, 256));
            if (_state.Z > 0) calibrations.Add(JoystickOffset.Z, new AxisCalibration(0, 65535, 32767, 256));
            if (_state.RotationX > 0) calibrations.Add(JoystickOffset.RotationX, new AxisCalibration(0, 65535, 32767, 256));
            if (_state.RotationY > 0) calibrations.Add(JoystickOffset.RotationY, new AxisCalibration(0, 65535, 32767, 256));
            if (_state.RotationZ > 0) calibrations.Add(JoystickOffset.RotationZ, new AxisCalibration(0, 65535, 32767, 256));

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

            _readCancel?.Cancel();
            _readTask?.Wait();
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
            while (!_readCancel.Token.IsCancellationRequested)
            {
                try
                {
                    _joystick.Poll();
                    var data = _joystick.GetBufferedData();

                    if (data.Length > 0)
                    {
                        OnUpdate(_joystick, data);
                    }
                }
                catch { /* Failed to read */ }
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

        #region Control Events
        // Duplicate Code in these parts, Blech! But I (literally) don't have time for perfection
        // How about making IDeviceControl an abstract class instead of an interface

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
                XmlSerializer serializer = new XmlSerializer(typeof(AssignmentProfile));

                using (FileStream stream = File.Create(dialog.FileName))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    var profile = new AssignmentProfile(_assignments);

                    if (associatedJoyCon != null)
                    {
                        profile.SubProfile = new AssignmentProfile(associatedJoyCon._assignments);
                        profile.SubName = associatedJoyCon.Type.ToString();
                    }

                    serializer.Serialize(writer, profile);
                    writer.Close();
                    stream.Close();
                }
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
            AssignmentProfile loadedProfile = null;

            if (doLoad == true && dialog.CheckFileExists)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AssignmentProfile));

                    using (FileStream stream = File.OpenRead(dialog.FileName))
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedProfile = serializer.Deserialize(reader) as AssignmentProfile;
                        reader.Close();
                        stream.Close();
                    }
                }
                catch (Exception err)
                {
                    var c = MessageBox.Show("Could not open the file \"" + err.Message + "\".", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (loadedProfile != null)
                {
                    if (loadedProfile.SubName == Type.ToString())
                    {
                        _assignments = loadedProfile.SubProfile.ToAssignmentArray();
                    }
                    else
                    {
                        _assignments = loadedProfile.ToAssignmentArray();
                    }

                    if (associatedJoyCon != null && loadedProfile.SubName == associatedJoyCon.Type.ToString())
                    {
                        associatedJoyCon._assignments = loadedProfile.SubProfile.ToAssignmentArray();
                    }
                    else if (associatedJoyCon != null)
                    {
                        associatedJoyCon._assignments = loadedProfile.ToAssignmentArray();
                    }
                }
            }
        }

        private void btnAddRumble_Click(object sender, RoutedEventArgs e)
        {
            
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
    }

    public interface IJoyControl : IBaseControol
    {
        void UpdateVisual(JoystickUpdate[] updates);
        Guid AssociatedInstanceID { get; }
    }

    public struct AxisCalibration
    {
        public int min;
        public int max;
        public int center;
        public uint dead;

        public AxisCalibration(int min, int max, int center, uint dead)
        {
            this.min = min;
            this.max = max;
            this.center = center;
            this.dead = dead;
        }

        public float Normal(int value)
        {
            if (Math.Abs(value - center) <= dead) return 0;
            return (value - center) / ((max - min) / 2f);
        }

        public float Normal(int value, bool positive)
        {
            int v = center - value;
            v *= positive ? 1 : -1;
            if (v <= dead) return 0;
            return v / ((max - min) / 2f);
        }
    }
}
