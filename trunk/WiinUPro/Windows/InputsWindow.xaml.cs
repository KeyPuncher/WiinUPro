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
        public bool Cancelled { get; protected set; }
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

                    keyTurboCheck.IsChecked = (item as KeyboardAssignment).TurboEnabled;
                    keyTurboRate.Value = (item as KeyboardAssignment).TurboRate / 50;
                    keyInverseCheck.IsChecked = (item as KeyboardAssignment).InverseInput;
                }
                else if (item is MouseAssignment)
                {
                    var mMov = (item as MouseAssignment).Input;
                    var btn = mouseMovGrid.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == mMov.ToString());
                    if (btn != null)
                    {
                        btn.Background = keySelectedBrush;
                        _selectedMouseDirections.Add(mMov);
                    }

                    mMovementRate.Value = (item as MouseAssignment).Rate;
                }
                else if (item is MouseButtonAssignment)
                {
                    var mBtn = (item as MouseButtonAssignment).MouseButton;
                    var btn = mouseBtnGrid.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == mBtn.ToString());
                    if (btn != null)
                    {
                        btn.Background = keySelectedBrush;
                        _selectedMouseButtons.Add(mBtn);
                    }

                    mButtonTurboCheck.IsChecked = (item as MouseButtonAssignment).TurboEnabled;
                    mButtonTurboRate.Value = (item as MouseButtonAssignment).TurboRate / 50;
                    mButtonInverseCheck.IsChecked = (item as MouseButtonAssignment).InverseInput;
                }
                else if (item is MouseScrollAssignment)
                {
                    var mScroll = (item as MouseScrollAssignment).ScrollDirection;
                    var btn = mouseScrollGrid.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == mScroll.ToString());
                    if (btn != null)
                    {
                        btn.Background = keySelectedBrush;
                        _selectedMouseScroll.Add(mScroll);
                    }

                    mScrollContinuousCheck.IsChecked = (item as MouseScrollAssignment).Continuous;
                    mScrollRate.Value = (item as MouseScrollAssignment).ScrollRate / 50;
                }
                else if (item is XInputButtonAssignment)
                {
                    // TODO: for specific Device (later)
                    var xb = (item as XInputButtonAssignment).Button;
                    var img = xboxGrid.Children.OfType<Image>().ToList().Find((i) => i.Tag.ToString() == xb.ToString());
                    if (img != null)
                    {
                        img.Opacity = 100;
                        _selectedXInputButtons.Add(xb);
                    }
                    else
                    {
                        var shape = xboxGrid.Children.OfType<Shape>().ToList().Find((s) => s.Tag.ToString() == xb.ToString());
                        if (shape != null)
                        {
                            shape.Opacity = 100;
                            _selectedXInputButtons.Add(xb);
                        }
                    }

                    // Remove once multiple xinput devices can be selected
                    deviceSelection.SelectedIndex = (int)(item as XInputButtonAssignment).Device - 1;
                }
                else if (item is XInputAxisAssignment)
                {
                    var xa = (item as XInputAxisAssignment).Axis;
                    var img = xboxGrid.Children.OfType<Image>().ToList().Find((i) => i.Tag.ToString() == xa.ToString());
                    if (img != null)
                    {
                        img.Opacity = 100;
                        _selectedXInputAxes.Add(xa);
                    }

                    // Remove once multiple xinput devices can be selected
                    deviceSelection.SelectedIndex = (int)(item as XInputAxisAssignment).Device - 1;
                }
            }
        }

        // This is meant to change what selections are made for each XInput device (a, b, c, d)
        // Currently an assigment collection can only be tied to one XInput device at a time
        //void UpdateXboxVisual(ScpDirector.XInput_Device target)
        //{
        //    // Clear all the visuals
        //    foreach (var child in xboxGrid.Children)
        //    {
        //        var img = child as Image;
        //        if (img != null && !string.IsNullOrEmpty(img.Tag.ToString()) && img.Tag.ToString() != "bg")
        //        {
        //            img.Opacity = 0;
        //        }
        //        else if (child is Shape)
        //        {
        //            (child as Shape).Opacity = 0;
        //        }
        //    }

        //    // Set the visuals
        //    foreach (var xb in _selectedXInputButtons)
        //    {
        //        if (xb)
        //    }
        //}

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
            AddToList(sender, _selectedMouseDirections);

            // Can't have both in opposite directions
            if (_selectedMouseDirections.Contains(MouseInput.MoveUp) && _selectedMouseDirections.Contains(MouseInput.MoveDown))
            {
                if ((sender as Button).Tag.ToString() == MouseInput.MoveUp.ToString())
                {
                    AddToList(mouseDown, _selectedMouseDirections);
                }
                else
                {
                    AddToList(mouseUp, _selectedMouseDirections);
                }
            }

            if (_selectedMouseDirections.Contains(MouseInput.MoveLeft) && _selectedMouseDirections.Contains(MouseInput.MoveRight))
            {
                if ((sender as Button).Tag.ToString() == MouseInput.MoveLeft.ToString())
                {
                    AddToList(mouseRight, _selectedMouseDirections);
                }
                else
                {
                    AddToList(mouseLeft, _selectedMouseDirections);
                }
            }
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
                    Result.Add(new KeyboardAssignment(key)
                    {
                        TurboEnabled = keyTurboCheck.IsChecked ?? false,
                        TurboRate = (int)keyTurboRate.Value * 50,
                        InverseInput = keyInverseCheck.IsChecked ?? false
                    });
                }

                foreach (var mDir in _selectedMouseDirections)
                {
                    Result.Add(new MouseAssignment(mDir, (float)mMovementRate.Value));
                }

                foreach (var mBtn in _selectedMouseButtons)
                {
                    Result.Add(new MouseButtonAssignment(mBtn)
                    {
                        TurboEnabled = mButtonTurboCheck.IsChecked ?? false,
                        TurboRate = (int)mButtonTurboRate.Value * 50,
                        InverseInput = mButtonInverseCheck.IsChecked ?? false
                    });
                }

                foreach (var mScroll in _selectedMouseScroll)
                {
                    Result.Add(new MouseScrollAssignment(mScroll)
                    {
                        Continuous = mScrollContinuousCheck.IsChecked ?? false,
                        ScrollRate = (int)mScrollRate.Value * 50
                    });
                }

                foreach (var xBtn in _selectedXInputButtons)
                {
                    Result.Add(new XInputButtonAssignment(xBtn, _selectedDevice)
                    {
                        TurboEnabled = xTurboCheck.IsChecked ?? false,
                        TurboRate = (int)xTurboRate.Value * 50,
                        InverseInput = xInverseCheck.IsChecked ?? false
                    });
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
            Cancelled = true;
            Close();
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
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

            // TODO: Display the inputs for this specific device (later)
            // once a single assignment can have multiple XInput devices
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
