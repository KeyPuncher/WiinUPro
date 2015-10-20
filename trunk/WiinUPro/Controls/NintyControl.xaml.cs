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

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for NintyControl.xaml
    /// </summary>
    public partial class NintyControl : UserControl
    {
        internal Nintroller _nintroller;            // Physical Controller Device
        internal INintyControl _controller;         // Visual Controller Representation
        internal AssignmentCollection _assignmentClipboard;  // Assignments to be pasted
        internal string _selectedInput;             // Controller's input to be effected by change
        internal ShiftState _currentState;
        internal int _shiftSate = 0;                // Current shift state being applied

        internal Dictionary<string, AssignmentCollection>[] _testAssignments;

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

        public NintyControl(string devicePath) : this()
        {
            _testAssignments = new Dictionary<string, AssignmentCollection>[ShiftAssignment.SHIFT_STATE_COUNT]
            {
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>(),
                new Dictionary<string, AssignmentCollection>()
            };
            _testAssignments[0].Add(INPUT_NAMES.PRO_CONTROLLER.LX, new AssignmentCollection( new List<IAssignment>() { new TestMouseAssignment(true) }));
            _testAssignments[0].Add(INPUT_NAMES.PRO_CONTROLLER.LY, new AssignmentCollection(new List<IAssignment>() { new TestMouseAssignment(false) }));

            _nintroller = new Nintroller(devicePath);
            _nintroller.StateUpdate += _nintroller_StateUpdate; 
            _nintroller.ExtensionChange += _nintroller_ExtensionChange;
            _nintroller.LowBattery += _nintroller_LowBattery;
        }

        public void ChangeState(ShiftState newState)
        {
            if (_currentState != newState)
            {
                KeyboardDirector.Access.Release();
                MouseDirector.Access.Release();
                _currentState = newState;
            }
        }

        #region Nintroller Events
        private void _nintroller_LowBattery(object sender, LowBatteryEventArgs e)
        {
            // Indicate that this controller's battery is low
        }

        private void _nintroller_ExtensionChange(object sender, NintrollerExtensionEventArgs e)
        {
            // Handle the extension change
            Dispatcher.Invoke(new Action(() =>
            {
                bool success = false;

                switch (e.controllerType)
                {
                    case ControllerType.ProController:
                        _controller = new ProControl();
                        break;
                }

                if (_controller != null)
                {
                    _controller.ChangeLEDs(_nintroller.Led1, _nintroller.Led2, _nintroller.Led3, _nintroller.Led4);
                    _controller.OnChangeLEDs += SetLeds;
                    _controller.OnInputSelected += InputSelected;
                    _controller.OnInputRightClick += InputOpenMenu;

                    _view.Child = _controller as UserControl;
                    ((UserControl)_view.Child).HorizontalAlignment = HorizontalAlignment.Left;
                    ((UserControl)_view.Child).VerticalAlignment = VerticalAlignment.Top;

                    success = true;

                    if (success)
                    {
                        btnDisconnect.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Device or Extension not supported.");
                        //btnConnect.IsEnabled = true;
                    }
                }
            }));
        }

        private void _nintroller_StateUpdate(object sender, NintrollerStateEventArgs e)
        {
            // Use the input to apply assignments.
            // This should only be done if not modifying the assignments
            //_controller.ApplyInput(e.state);
            foreach (var input in e.state)
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("{0} :\t\t{1}", input.Key, input.Value));
                if (_testAssignments[ShiftIndex].ContainsKey(input.Key))
                {
                    _testAssignments[ShiftIndex][input.Key].ApplyAll(input.Value);
                }
            }

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

            if (_nintroller.Connect())
            {
                btnConnect.IsEnabled = false;
                _nintroller.BeginReading();
                _nintroller.GetStatus();
                _nintroller.SetPlayerLED(1);

                // We need a function we can await for the type to come back
                //switch (_nintroller.Type)
                //{
                //    case ControllerType.ProController:
                //        _controller = new ProControl();
                //        break;
                //}
                /*
                if (_controller != null)
                {
                    _controller.ChangeLEDs(_nintroller.Led1, _nintroller.Led2, _nintroller.Led3, _nintroller.Led4);
                    _controller.OnChangeLEDs += SetLeds;
                    _controller.OnInputSelected += InputSelected;
                    _controller.OnInputRightClick += InputOpenMenu;

                    _view.Child = _controller as UserControl;
                    ((UserControl)_view.Child).HorizontalAlignment = HorizontalAlignment.Left;
                    ((UserControl)_view.Child).VerticalAlignment = VerticalAlignment.Top;

                    success = true;
                }*/
                btnDisconnect.IsEnabled = true;
            }
#if DEBUG
            else
            {
                _controller = new ProControl();
                _controller.OnInputSelected += InputSelected;
                _controller.OnInputRightClick += InputOpenMenu;
                _view.Child = _controller as UserControl;
                ((UserControl)_view.Child).HorizontalAlignment = HorizontalAlignment.Left;
                ((UserControl)_view.Child).VerticalAlignment = VerticalAlignment.Top;
                //success = true;
                btnDisconnect.IsEnabled = true;
            }
#else
            else
            {
                MessageBox.Show("Could not connect to device!");
                btnConnect.IsEnabled = true;
            }
#endif

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
            btnDisconnect.IsEnabled = false;

            _nintroller.Disconnect();
            _view.Child = null;
            _controller = null;

            btnConnect.IsEnabled = true;
        }

        private void AssignMenu_Click(object sender, RoutedEventArgs e)
        {
            InputSelected(sender, _selectedInput);
        }

        private void CopyMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_testAssignments[ShiftIndex].ContainsKey(_selectedInput))
            {
                _assignmentClipboard = _testAssignments[ShiftIndex][_selectedInput];
            }
        }

        private void PasteMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_testAssignments[ShiftIndex].ContainsKey(_selectedInput))
            {
                _testAssignments[ShiftIndex][_selectedInput] = _assignmentClipboard;
            }
            else
            {
                _testAssignments[ShiftIndex].Add(_selectedInput, _assignmentClipboard);
            }
        }

        private void ClearMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_testAssignments[ShiftIndex].ContainsKey(_selectedInput))
            {
                _testAssignments[ShiftIndex].Remove(_selectedInput);
            }
        }
        #endregion

        #region Control Events
        private void SetLeds(object sender, bool[] values)
        {
            if (_nintroller != null && (values ?? new bool[0]).Length == 4)
            {
                _nintroller.Led1 = values[0];
                _nintroller.Led2 = values[1];
                _nintroller.Led3 = values[2];
                _nintroller.Led4 = values[3];
            }
        }

        private void InputSelected(object sender, string key)
        {
            System.Diagnostics.Debug.WriteLine(key);

            InputsWindow win = new InputsWindow();
            win.ShowDialog();

            if (_testAssignments[ShiftIndex].ContainsKey(key))
            {
                _testAssignments[ShiftIndex][key] = win.Result;
            }
            else
            {
                _testAssignments[ShiftIndex].Add(key, win.Result);
            }
        }

        private void InputOpenMenu(object sender, string e)
        {
            _selectedInput = e;
            subMenu.IsOpen = true;
        }
        #endregion
    }

    public interface INintyControl
    {
        event EventHandler<bool[]> OnChangeLEDs;
        event EventHandler<string> OnInputSelected;
        event EventHandler<string> OnInputRightClick;

        void ApplyInput(INintrollerState state);
        void UpdateVisual(INintrollerState state);
        void ChangeLEDs(bool one, bool two, bool three, bool four);
    }
}
