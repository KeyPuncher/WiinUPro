using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        internal Joystick _joystick;
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

            if ((JoystickType)_joystick.Properties.ProductId == JoystickType.LeftJoyCon)
            {
                _controller = new JoyConLControl();
            }
            else if ((JoystickType)_joystick.Properties.ProductId == JoystickType.RightJoyCon)
            {
                _controller = new JoyConRControl();
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
                foreach(var update in updates)
                {
                    System.Diagnostics.Debug.WriteLine(update);
                    string key = update.Offset.ToString();
                    
                    // Split PointOfView Controllers into 4
                    if (key[0] == 'P')
                    {
                        string subKey = key.Replace("PointOfViewControllers", "pov");
                        bool north = false, south = false, east = false, west = false;
                        if (update.Value != -1)
                        {
                            north = update.Value > 27000 || update.Value < 9000;
                            south = !north && update.Value < 27000 && update.Value > 9000;
                            east = update.Value > 0 && update.Value < 18000;
                            west = !east && update.Value > 18000;
                        }
                        if (_assignments[ShiftIndex].ContainsKey(subKey + "N")) _assignments[ShiftIndex][subKey + "N"].ApplyAll(north ? 1 : 0);
                        if (_assignments[ShiftIndex].ContainsKey(subKey + "S")) _assignments[ShiftIndex][subKey + "S"].ApplyAll(south ? 1 : 0);
                        if (_assignments[ShiftIndex].ContainsKey(subKey + "E")) _assignments[ShiftIndex][subKey + "E"].ApplyAll(east ? 1 : 0);
                        if (_assignments[ShiftIndex].ContainsKey(subKey + "W")) _assignments[ShiftIndex][subKey + "W"].ApplyAll(west ? 1 : 0);
                    }
                    else
                    {
                        if (_assignments[ShiftIndex].ContainsKey(update.Offset.ToString()))
                        {
                            _assignments[ShiftIndex][key].ApplyAll(update.Value / 128f);
                        }
                    }
                }
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
            _joystick.Properties.BufferSize = 128;
            _joystick.Acquire();
            _joystick.Poll();
            var data = _joystick.GetBufferedData();

            _readCancel = new CancellationTokenSource();
            _readTask = Task.Factory.StartNew(PollData, _readCancel.Token);

            return true;
        }

        public void Disconnect()
        {
            if (associatedJoyCon != null)
            {
                associatedJoyCon.Disconnect();
            }

            _readCancel.Cancel();
            _readTask.Wait();
            _joystick.Unacquire();
            _readTask = null;
            _readCancel = null;
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
            
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            
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

    public interface IJoyControl
    {
        event Shared.Delegates.StringDel OnInputSelected;
        event Shared.Delegates.StringDel OnInputRightClick;
        event AssignmentCollection.AssignDelegate OnQuickAssign;

        void UpdateVisual(JoystickUpdate[] updates);
        Guid AssociatedInstanceID { get; }
    }
}
