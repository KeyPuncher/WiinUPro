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
using System.Windows.Shapes;
using InputManager;
using ScpControl;

namespace WiinUPro
{
    /// <summary>
    /// Interaction logic for InputsWindow.xaml
    /// </summary>
    public partial class InputsWindow : Window
    {
        public AssignmentCollection Result { get; protected set; }
        public SolidColorBrush keySelectedBrush;
        public SolidColorBrush keyDeselectedBrush;

        protected ScpDirector.XInput_Device _selectedDevice = ScpDirector.XInput_Device.Device_A;
        protected NintyControl _control;

        private List<VirtualKeyCode> _selectedKeys;
        private List<MouseInput> _selectedMouseDirections;
        private List<InputManager.Mouse.MouseKeys> _selectedMouseButtons;
        private List<InputManager.Mouse.ScrollDirection> _selectedMouseScroll;
        private List<X360Button> _selectedXInputButtons;
        private List<X360Axis> _selectedXInputAxes;

        public InputsWindow()
        {
            InitializeComponent();
            Result = new AssignmentCollection();
            keySelectedBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xBF, 0x5F, 0x0F));
            keyDeselectedBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD));

            _selectedKeys = new List<VirtualKeyCode>();
            _selectedMouseDirections = new List<MouseInput>();
            _selectedMouseButtons = new List<InputManager.Mouse.MouseKeys>();
            _selectedMouseScroll = new List<InputManager.Mouse.ScrollDirection>();
            _selectedXInputButtons = new List<X360Button>();
            _selectedXInputAxes = new List<X360Axis>();
        }

        public InputsWindow(NintyControl control) : this()
        {
            _control = control;
        }

        public InputsWindow(NintyControl control, AssignmentCollection collection) : this(control)
        {
            // fill the window with current assignments
            foreach (var item in collection)
            {
                if (item is KeyboardAssignment)
                {
                    var key = (item as KeyboardAssignment).KeyCode;
                    var btn = keyboardGrid.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == key.ToString());
                    if (btn != null)
                    {
                        btn.Background = keySelectedBrush;
                        _selectedKeys.Add(key);
                    }
                }
                else if (item is XInputButtonAssignment)
                {
                    var xb = (item as XInputButtonAssignment).Button;
                }
            }
        }

        private void AddToList<TEnum>(object obj, List<TEnum> list) where TEnum : struct
        {
            var elm = obj as FrameworkElement;

            if (elm != null)
            {
                TEnum inputType;
                if (Enum.TryParse(elm.Tag.ToString(), out inputType))
                {
                    bool selected = list.Contains(inputType);

                    // This can be a Button or an Image or a shape
                    var btn = obj as Button;
                    if (btn != null)
                    {
                        selected &= btn.Background == keySelectedBrush;
                    }

                    var img = obj as Image;
                    if (img != null)
                    {
                        selected &= img.Opacity > 0;
                    }

                    var shape = obj as Shape;
                    if (shape != null)
                    {
                        selected &= shape.Opacity > 0;
                    }

                    if (selected)
                    {
                        // Deselect and remove from list
                        list.Remove(inputType);

                        if (btn != null)
                        {
                            btn.Background = keyDeselectedBrush;
                        }
                        
                        if (img != null)
                        {
                            img.Opacity = 0;
                        }

                        if (shape != null)
                        {
                            shape.Opacity = 0;
                        }
                    }
                    else
                    {
                        list.Add(inputType);

                        if (btn != null)
                        {
                            btn.Background = keySelectedBrush;
                        }

                        if (img != null)
                        {
                            img.Opacity = 100;
                        }

                        if (shape != null)
                        {
                            shape.Opacity = 100;
                        }
                    }
                }
            }
        }

        private void ToggleKey(object sender, RoutedEventArgs e)
        {
            AddToList<VirtualKeyCode>(sender, _selectedKeys);
        }

        private void ToggleMouseDirection(object sender, RoutedEventArgs e)
        {
            AddToList<MouseInput>(sender, _selectedMouseDirections);
        }

        private void ToggleMouseButton(object sender, RoutedEventArgs e)
        {
            AddToList(sender, _selectedMouseButtons);
        }

        private void ToggleMouseScroll(object sender, RoutedEventArgs e)
        {
            AddToList(sender, _selectedMouseScroll);
        }

        private void ToggleXInputButton(object sender, RoutedEventArgs e)
        {
            AddToList(sender, _selectedXInputButtons);
        }

        private void ToggleXInputAxis(object sender, RoutedEventArgs e)
        {
            AddToList(sender, _selectedXInputAxes);
        }

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            // If on the Shift Tab, that is the only assignments that can be had
            if (tabControl.SelectedIndex == 4)
            {
                var shifty = new ShiftAssignment(_control);
                shifty.Toggles = radioToggle.IsChecked ?? false;

                if (shifty.Toggles)
                {
                    if (checkNone.IsChecked ?? false)
                    {
                        shifty.ToggleStates.Add(ShiftState.None);
                    }

                    if (checkRed.IsChecked ?? false)
                    {
                        shifty.ToggleStates.Add(ShiftState.Red);
                    }

                    if (checkBlue.IsChecked ?? false)
                    {
                        shifty.ToggleStates.Add(ShiftState.Blue);
                    }

                    if (checkGreen.IsChecked ?? false)
                    {
                        shifty.ToggleStates.Add(ShiftState.Green);
                    }
                }
                else
                {
                    if (radioNone.IsChecked ?? false)
                    {
                        shifty.TargetState = ShiftState.None;
                    }
                    else if (radioRed.IsChecked ?? false)
                    {
                        shifty.TargetState = ShiftState.Red;
                    }
                    else if (radioBlue.IsChecked ?? false)
                    {
                        shifty.TargetState = ShiftState.Blue;
                    }
                    else if (radioGreen.IsChecked ?? false)
                    {
                        shifty.TargetState = ShiftState.Green;
                    }
                }

                Result.Add(shifty);
            }
            else
            {
                foreach (var key in _selectedKeys)
                {
                    Result.Add(new KeyboardAssignment(key));
                }

                foreach (var mDir in _selectedMouseDirections)
                {
                    Result.Add(new MouseAssignment(mDir, 1.0f));
                }

                foreach (var mBtn in _selectedMouseButtons)
                {
                    Result.Add(new MouseButtonAssignment(mBtn));
                }

                foreach (var xBtn in _selectedXInputButtons)
                {
                    Result.Add(new XInputButtonAssignment(xBtn, _selectedDevice));
                }

                foreach (var xAxis in _selectedXInputAxes)
                {
                    Result.Add(new XInputAxisAssignment(xAxis, _selectedDevice));
                }
            }

            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void xInputConnect_Click(object sender, RoutedEventArgs e)
        {
            ScpDirector.Access.ConnectDevice(_selectedDevice);
        }

        private void xInputDisconnect_Click(object sender, RoutedEventArgs e)
        {
            ScpDirector.Access.DisconnectDevice(_selectedDevice);
        }

        private void deviceSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                 if (deviceSelection.SelectedIndex == 0) _selectedDevice = ScpDirector.XInput_Device.Device_A;
            else if (deviceSelection.SelectedIndex == 1) _selectedDevice = ScpDirector.XInput_Device.Device_B;
            else if (deviceSelection.SelectedIndex == 2) _selectedDevice = ScpDirector.XInput_Device.Device_C;
            else if (deviceSelection.SelectedIndex == 3) _selectedDevice = ScpDirector.XInput_Device.Device_D;

            // TODO: Display the inputs for this specific device
        }

        private void ChangeShiftType(object sender, RoutedEventArgs e)
        {
            if (radioHold.IsChecked ?? false)
            {
                shiftToggleGroup.IsEnabled = false;
                shiftHoldGroup.IsEnabled = true;
            }

            if (radioToggle.IsChecked ?? false)
            {
                shiftToggleGroup.IsEnabled = true;
                shiftHoldGroup.IsEnabled = false;
            }
        }
    }
}
