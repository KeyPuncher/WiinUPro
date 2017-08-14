using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        static int lastSelectedTab = 0;

        public AssignmentCollection Result { get; protected set; }
        public bool Apply { get; protected set; }
        public Style keyStyle;
        public Style keySelectedStyle;

        protected ScpDirector.XInput_Device _selectedDevice = ScpDirector.XInput_Device.Device_A;
        protected IDeviceControl _control;

        private List<VirtualKeyCode> _selectedKeys;
        private List<MouseInput> _selectedMouseDirections;
        private List<InputManager.Mouse.MouseKeys> _selectedMouseButtons;
        private List<InputManager.Mouse.ScrollDirection> _selectedMouseScroll;
        private List<X360Button> _selectedXInputButtons;
        private List<X360Axis> _selectedXInputAxes;
        private List<int> _selectedVJoyButtons;
        private List<VJoyDirector.AxisDirection> _selectedVJoyAxes;
        private List<VJoyDirector.POVDirection> _selectedVJoyPOVs;

        public InputsWindow()
        {
            InitializeComponent();
            Result = new AssignmentCollection();

            keyStyle = (Style)Application.Current.Resources["KeyButton"];
            keySelectedStyle = (Style)Application.Current.Resources["KeyButtonActive"];

            vJoyOffLabel.Visibility = VJoyDirector.Access.Available ? Visibility.Collapsed : Visibility.Visible;
            if (vJoyOffLabel.Visibility == Visibility.Collapsed && VJoyDirector.Access.Devices.Count > 0)
            {
                foreach (var joyDevice in VJoyDirector.Access.Devices)
                {
                    joystickSelection.Items.Add("Joystick " + joyDevice.ID);
                }

                joystickSelection.SelectedIndex = 0;
            }

            _selectedKeys = new List<VirtualKeyCode>();
            _selectedMouseDirections = new List<MouseInput>();
            _selectedMouseButtons = new List<InputManager.Mouse.MouseKeys>();
            _selectedMouseScroll = new List<InputManager.Mouse.ScrollDirection>();
            _selectedXInputButtons = new List<X360Button>();
            _selectedXInputAxes = new List<X360Axis>();
            _selectedVJoyButtons = new List<int>();
            _selectedVJoyAxes = new List<VJoyDirector.AxisDirection>();
            _selectedVJoyPOVs = new List<VJoyDirector.POVDirection>();
        }

        public InputsWindow(IDeviceControl control) : this()
        {
            _control = control;
            tabControl.SelectedIndex = lastSelectedTab;
        }

        public InputsWindow(IDeviceControl control, AssignmentCollection collection) : this(control)
        {
            var xImages = new List<Image>();
            foreach (var border in xboxGrid.Children.OfType<Border>().ToList())
            {
                if (border.Child is Image)
                {
                    xImages.Add(border.Child as Image);
                }
            }

            // If this collection is only a shift assignment, populate the shift tab
            if (collection.ShiftAssignment)
            {
                ShiftAssignment shifty = collection.Assignments[0] as ShiftAssignment;
                radioToggle.IsChecked = shifty.Toggles;

                if (shifty.Toggles)
                {
                    checkNone.IsChecked  = shifty.ToggleStates.Contains(ShiftState.None);
                    checkRed.IsChecked   = shifty.ToggleStates.Contains(ShiftState.Red);
                    checkBlue.IsChecked  = shifty.ToggleStates.Contains(ShiftState.Blue);
                    checkGreen.IsChecked = shifty.ToggleStates.Contains(ShiftState.Green);

                    shiftToggleGroup.IsEnabled = true;
                    shiftHoldGroup.IsEnabled = false;
                }
                else
                {
                    switch (shifty.TargetState)
                    {
                        case ShiftState.None:  radioNone.IsChecked = true; break;
                        case ShiftState.Red:   radioRed.IsChecked = true; break;
                        case ShiftState.Blue:  radioBlue.IsChecked = true; break;
                        case ShiftState.Green: radioGreen.IsChecked = true; break;
                    }
                }

                tabControl.SelectedIndex = 4;
                return;
            }

            // Show a tab where the user has assignments chosen
            bool hasKey = false, hasMouse = false, hasJoy = false, hasX = false, hasShift = false;

            // fill the window with current assignments
            foreach (var item in collection)
            {
                if (item is KeyboardAssignment)
                {
                    hasKey = true;
                    var key = (item as KeyboardAssignment).KeyCode;
                    var btn = keyboardGrid.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == key.ToString());
                    if (btn != null)
                    {
                        btn.Style = keySelectedStyle;
                        _selectedKeys.Add(key);
                    }

                    keyTurboCheck.IsChecked = (item as KeyboardAssignment).TurboEnabled;
                    keyTurboRate.Value = (item as KeyboardAssignment).TurboRate / 50;
                    keyInverseCheck.IsChecked = (item as KeyboardAssignment).InverseInput;                    
                }
                else if (item is MouseAssignment)
                {
                    hasMouse = true;
                    var mMov = (item as MouseAssignment).Input;
                    var btn = mouseMovGrid.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == mMov.ToString());
                    if (btn != null)
                    {
                        btn.Style = keySelectedStyle;
                        _selectedMouseDirections.Add(mMov);
                    }

                    mMovementRate.Value = (item as MouseAssignment).Rate;
                }
                else if (item is MouseButtonAssignment)
                {
                    hasMouse = true;
                    var mBtn = (item as MouseButtonAssignment).MouseButton;
                    var btn = mouseBtnGrid.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == mBtn.ToString());
                    if (btn != null)
                    {
                        btn.Style = keySelectedStyle;
                        _selectedMouseButtons.Add(mBtn);
                    }

                    mButtonTurboCheck.IsChecked = (item as MouseButtonAssignment).TurboEnabled;
                    mButtonTurboRate.Value = (item as MouseButtonAssignment).TurboRate / 50;
                    mButtonInverseCheck.IsChecked = (item as MouseButtonAssignment).InverseInput;
                }
                else if (item is MouseScrollAssignment)
                {
                    hasMouse = true;
                    var mScroll = (item as MouseScrollAssignment).ScrollDirection;
                    var btn = mouseScrollGrid.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == mScroll.ToString());
                    if (btn != null)
                    {
                        btn.Style = keySelectedStyle;
                        _selectedMouseScroll.Add(mScroll);
                    }

                    mScrollContinuousCheck.IsChecked = (item as MouseScrollAssignment).Continuous;
                    mScrollRate.Value = (item as MouseScrollAssignment).ScrollRate / 50;
                }
                else if (item is XInputButtonAssignment)
                {
                    hasX = true;
                    // TODO: for specific Device (later)
                    var xb = (item as XInputButtonAssignment).Button;
                    var img = xImages.Find((i) => i.Tag.ToString() == xb.ToString());
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
                    hasX = true;
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
                else if (item is VJoyButtonAssignment)
                {
                    hasJoy = true;
                    var vb = (item as VJoyButtonAssignment);
                    if (joystickSelection.HasItems && joystickSelection.Items.Contains("Joystick " + vb.DeviceId))
                    {
                        joystickSelection.SelectedItem = "Joystick " + vb.DeviceId;
                        var btn = buttonsWrap.Children.OfType<Button>().ToList().Find((b) => b.Tag.ToString() == vb.Button.ToString());
                        if (btn != null)
                        {
                            btn.Style = keySelectedStyle;
                            _selectedVJoyButtons.Add(vb.Button);
                        }
                    }
                }
                else if (item is VJoyAxisAssignment)
                {
                    hasJoy = true;
                    var va = (item as VJoyAxisAssignment);
                    if (joystickSelection.HasItems && joystickSelection.Items.Contains("Joystick " + va.DeviceId))
                    {
                        joystickSelection.SelectedItem = "Joystick " + va.DeviceId;
                        string lblStr = va.Axis.ToString().Replace("HID_USAGE_", "");
                        if (lblStr.Length > 2) lblStr = lblStr.Substring(0, 2);
                        foreach (StackPanel stack in axisStack.Children)
                        {
                            if ((stack.Children[0] as Label).Content.ToString().ToUpper().StartsWith(lblStr.ToUpper()))
                            {
                                VJoyDirector.AxisDirection e;
                                int index = va.Positive ? 2 : 1;

                                if (va.Axis == HID_USAGES.HID_USAGE_SL0 && (stack.Children[index] as Button).Tag.ToString().Contains("1"))
                                {
                                    continue;
                                }

                                if (va.Axis == HID_USAGES.HID_USAGE_SL1 && (stack.Children[index] as Button).Tag.ToString().Contains("0"))
                                {
                                    continue;
                                }

                                    if (Enum.TryParse((stack.Children[index] as Button).Tag.ToString(), out e))
                                {
                                    (stack.Children[index] as Button).Style = keySelectedStyle;
                                    _selectedVJoyAxes.Add(e);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (item is VJoyPOVAssignment)
                {
                    hasJoy = true;
                    var vp = (item as VJoyPOVAssignment);
                    if (joystickSelection.HasItems && joystickSelection.Items.Contains("Joystick " + vp.DeviceId))
                    {
                        joystickSelection.SelectedItem = "Joystick " + vp.DeviceId;
                        int index = vp.POVNum - 1;
                        if (povStack.Children.Count > index)
                        {
                            StackPanel stack = povStack.Children[index] as StackPanel;
                            var btn = stack.Children[(int)vp.Direction % 4 + 1] as Button;
                            btn.Style = keySelectedStyle;
                            _selectedVJoyPOVs.Add(vp.Direction);
                        }
                    }
                }
                else if (item is RumbleAssignment)
                {
                    rumbleOnPress.IsChecked = true;
                }
                else if (item is ShiftAssignment)
                {
                    hasShift = true;
                }
            }

            bool changeTab = false;
            switch (lastSelectedTab)
            {
                case 0: changeTab = !hasKey; break;
                case 1: changeTab = !hasMouse; break;
                case 2: changeTab = !hasX; break;
                case 3: changeTab = !hasJoy; break;
                case 4: changeTab = !hasShift; break;
            }

            if (changeTab && (hasKey || hasMouse || hasX || hasJoy || hasShift))
            {
                if (hasKey) tabControl.SelectedIndex = 0;
                else if (hasMouse) tabControl.SelectedIndex = 1;
                else if (hasX) tabControl.SelectedIndex = 2;
                else if (hasJoy) tabControl.SelectedIndex = 3;
                else if (hasShift) tabControl.SelectedIndex = 4;
            }
        }

        private void SetupJoystick(uint id)
        {
            buttonsWrap.Children.Clear();
            axisStack.Children.Clear();
            povStack.Children.Clear();

            var device = VJoyDirector.Access.Devices.Find((d) => d.ID == id);

            if (device != null)
            {
                // Add buttons
                for (int b = 1; b <= device.Buttons; b++)
                {
                    var btn = new Button()
                    {
                        Content = b.ToString(),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 42,
                        Height = 42,
                        Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x21, 0x21, 0x21)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Tag = b,
                        Style = FindResource("KeyButton") as Style
                    };
                    btn.Click += ToggleVJoyButton;
                    buttonsWrap.Children.Add(btn);
                }

                joyBtnGroup.Visibility = device.Buttons > 0 ? Visibility.Visible : Visibility.Collapsed;

                // Add Axes
                foreach (var axis in device.Axes)
                {
                    string tag = "";
                    switch (axis)
                    {
                        case "X Axis":
                            tag = "X";
                            break;
                        case "Y Axis":
                            tag = "Y";
                            break;
                        case "Z Axis":
                            tag = "Z";
                            break;
                        case "Rx Axis":
                            tag = "RX";
                            break;
                        case "Ry Axis":
                            tag = "RY";
                            break;
                        case "Rz Axis":
                            tag = "RZ";
                            break;
                        case "Slider 1":
                            tag = "SL0";
                            break;
                        case "Slider 2":
                            tag = "SL1";
                            break;
                        default: tag = "X"; break;
                    }

                    var stack = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Height = 50,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    var lbl = new Label()
                    {
                        Content = axis,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Width = 136,
                        FontWeight = FontWeights.Bold,
                        Foreground = FindResource("TextHeader") as SolidColorBrush
                    };

                    var pos = new Button()
                    {
                        Content = "+",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 42,
                        Height = 42,
                        Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x21, 0x21, 0x21)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Tag = tag + "_Pos",
                        Margin = new Thickness(10, 0, 0, 0),
                        Style = FindResource("KeyButton") as Style
                    };

                    var neg = new Button()
                    {
                        Content = "-",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 42,
                        Height = 42,
                        Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x21, 0x21, 0x21)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Tag = tag + "_Neg",
                        Margin = new Thickness(10, 0, 0, 0),
                        Style = FindResource("KeyButton") as Style
                    };

                    pos.Click += ToggleVJoyAxis;
                    neg.Click += ToggleVJoyAxis;

                    stack.Children.Add(lbl);
                    stack.Children.Add(neg);
                    stack.Children.Add(pos);

                    axisStack.Children.Add(stack);
                }

                joyAxisGroup.Visibility = device.Axes.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

                // Add POVs
                for (int i = 1; i <= device.POVs + device.POV4Ds; i++)
                {
                    var stack = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Height = 50,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    string txt;
                    switch (i)
                    {
                        case 1: txt = "1st"; break;
                        case 2: txt = "2nd"; break;
                        case 3: txt = "3rd"; break;
                        case 4: txt = "4th"; break;
                        default: txt = "POV"; break;
                    }

                    var lbl = new Label()
                    {
                        Content = txt,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Width = 32,
                        FontWeight = FontWeights.Bold,
                        Foreground = FindResource("TextHeader") as SolidColorBrush
                    };

                    var up = new Button()
                    {
                        Content = "Up",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 42,
                        Height = 42,
                        Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x21, 0x21, 0x21)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Tag = "_" + i + "Up",
                        Margin = new Thickness(10, 0, 0, 0),
                        Style = FindResource("KeyButton") as Style
                    };

                    var down = new Button()
                    {
                        Content = "Dn",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 42,
                        Height = 42,
                        Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x21, 0x21, 0x21)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Tag = "_" + i + "Down",
                        Margin = new Thickness(10, 0, 0, 0),
                        Style = FindResource("KeyButton") as Style
                    };

                    var left = new Button()
                    {
                        Content = "Lt",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 42,
                        Height = 42,
                        Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x21, 0x21, 0x21)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Tag = "_" + i + "Left",
                        Margin = new Thickness(10, 0, 0, 0),
                        Style = FindResource("KeyButton") as Style
                    };

                    var right = new Button()
                    {
                        Content = "Rt",
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Width = 42,
                        Height = 42,
                        Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD)),
                        BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x21, 0x21, 0x21)),
                        FontWeight = FontWeights.Bold,
                        FontSize = 16,
                        Tag = "_" + i + "Right",
                        Margin = new Thickness(10, 0, 0, 0),
                        Style = FindResource("KeyButton") as Style
                    };

                    up.Click += ToggleVJoyPOV;
                    down.Click += ToggleVJoyPOV;
                    left.Click += ToggleVJoyPOV;
                    right.Click += ToggleVJoyPOV;

                    stack.Children.Add(lbl);
                    stack.Children.Add(up);
                    stack.Children.Add(down);
                    stack.Children.Add(left);
                    stack.Children.Add(right);

                    povStack.Children.Add(stack);
                }

                joyPOVGroup.Visibility = device.POVs + device.POV4Ds > 0 ? Visibility.Visible : Visibility.Collapsed;
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
                        selected &= btn.Style == keySelectedStyle;
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
                            btn.Style = keyStyle;
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
                            btn.Style = keySelectedStyle;
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

        private void AddToList(object obj, List<int> list)
        {
            var elm = obj as FrameworkElement;

            if (elm != null)
            {
                int inputType;
                if (int.TryParse(elm.Tag.ToString(), out inputType))
                {
                    bool selected = list.Contains(inputType);

                    // This can be a Button or an Image or a shape
                    var btn = obj as Button;
                    if (btn != null)
                    {
                        selected &= btn.Style == keySelectedStyle;
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
                            btn.Style = keyStyle;
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
                            btn.Style = keySelectedStyle;
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

        #region Toggle Assignment Events

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

        private void ToggleVJoyButton(object sender, RoutedEventArgs e)
        {
            AddToList(sender, _selectedVJoyButtons);
        }

        private void ToggleVJoyAxis(object sender, RoutedEventArgs e)
        {
            AddToList(sender, _selectedVJoyAxes);
        }

        private void ToggleVJoyPOV(object sender, RoutedEventArgs e)
        {
            AddToList(sender, _selectedVJoyPOVs);
        }

        #endregion

        #region Button Click Events

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            // Check if Rumble should occur when pressed
            if (rumbleOnPress.IsChecked ?? false)
            {
                Result.Add(new RumbleAssignment(_control.AddRumble));
            }
            
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

                uint vDevice = 1;
                if (joystickSelection.SelectedIndex > -1)
                {
                    uint.TryParse(joystickSelection.SelectedValue.ToString().Replace("Joystick ", ""), out vDevice);
                    VJoyDirector.Access.AquireDevice(vDevice);
                }

                foreach (var vBtn in _selectedVJoyButtons)
                {
                    Result.Add(new VJoyButtonAssignment(vBtn, vDevice));
                }

                foreach (var vAxis in _selectedVJoyAxes)
                {
                    bool p = false;
                    Result.Add(new VJoyAxisAssignment(ToHidAxis(vAxis, out p), p, vDevice));
                }

                foreach (var vPOV in _selectedVJoyPOVs)
                {
                    Result.Add(new VJoyPOVAssignment(vPOV, vDevice));
                }
            }

            Apply = true;
            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            Apply = true;
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

        #endregion

        #region Other UI Events

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

        private void detectBox_KeyDown(object sender, KeyEventArgs e)
        {
            var detectedKey = ToVK(e.Key);
            
            if (detectedKey == null)
            {
                detectBox.Text = "Key not identified";
            }
            else
            {
                detectBox.Text = "Detected key " + detectedKey.ToString().Replace("VK_", "").Replace("K_", "");
                var btn = keyboardGrid.Children.OfType<Button>().Where(x => x.Tag.ToString() == detectedKey.ToString()).FirstOrDefault();
                ToggleKey(btn, null);
            }
        }

        private void joystickSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetupJoystick(VJoyDirector.Access.Devices[joystickSelection.SelectedIndex].ID);
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border)
            {
                (sender as Border).BorderThickness = new Thickness(1);
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border)
            {
                (sender as Border).BorderThickness = new Thickness(0);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            lastSelectedTab = tabControl.SelectedIndex;
        }

        #endregion

        static VirtualKeyCode? ToVK(Key key)
        {
            switch (key)
            {
                // Alpha
                case Key.A: return VirtualKeyCode.K_A;
                case Key.B: return VirtualKeyCode.K_B;
                case Key.C: return VirtualKeyCode.K_C;
                case Key.D: return VirtualKeyCode.K_D;
                case Key.E: return VirtualKeyCode.K_E;
                case Key.F: return VirtualKeyCode.K_F;
                case Key.G: return VirtualKeyCode.K_G;
                case Key.H: return VirtualKeyCode.K_H;
                case Key.I: return VirtualKeyCode.K_I;
                case Key.J: return VirtualKeyCode.K_J;
                case Key.K: return VirtualKeyCode.K_K;
                case Key.L: return VirtualKeyCode.K_L;
                case Key.M: return VirtualKeyCode.K_M;
                case Key.N: return VirtualKeyCode.K_N;
                case Key.O: return VirtualKeyCode.K_O;
                case Key.P: return VirtualKeyCode.K_P;
                case Key.Q: return VirtualKeyCode.K_Q;
                case Key.R: return VirtualKeyCode.K_R;
                case Key.S: return VirtualKeyCode.K_S;
                case Key.T: return VirtualKeyCode.K_T;
                case Key.U: return VirtualKeyCode.K_U;
                case Key.V: return VirtualKeyCode.K_V;
                case Key.W: return VirtualKeyCode.K_W;
                case Key.X: return VirtualKeyCode.K_X;
                case Key.Y: return VirtualKeyCode.K_Y;
                case Key.Z: return VirtualKeyCode.K_Z;
                // Numeric
                case Key.D0: return VirtualKeyCode.K_0;
                case Key.D1: return VirtualKeyCode.K_1;
                case Key.D2: return VirtualKeyCode.K_2;
                case Key.D3: return VirtualKeyCode.K_3;
                case Key.D4: return VirtualKeyCode.K_4;
                case Key.D5: return VirtualKeyCode.K_5;
                case Key.D6: return VirtualKeyCode.K_6;
                case Key.D7: return VirtualKeyCode.K_7;
                case Key.D8: return VirtualKeyCode.K_8;
                case Key.D9: return VirtualKeyCode.K_9;
                // NumPad
                case Key.NumPad0: return VirtualKeyCode.VK_NUMPAD0;
                case Key.NumPad1: return VirtualKeyCode.VK_NUMPAD1;
                case Key.NumPad2: return VirtualKeyCode.VK_NUMPAD2;
                case Key.NumPad3: return VirtualKeyCode.VK_NUMPAD3;
                case Key.NumPad4: return VirtualKeyCode.VK_NUMPAD4;
                case Key.NumPad5: return VirtualKeyCode.VK_NUMPAD5;
                case Key.NumPad6: return VirtualKeyCode.VK_NUMPAD6;
                case Key.NumPad7: return VirtualKeyCode.VK_NUMPAD7;
                case Key.NumPad8: return VirtualKeyCode.VK_NUMPAD8;
                case Key.NumPad9: return VirtualKeyCode.VK_NUMPAD9;
                // Arrows
                case Key.Down: return VirtualKeyCode.VK_DOWN;
                case Key.Left: return VirtualKeyCode.VK_LEFT;
                case Key.Right: return VirtualKeyCode.VK_RIGHT;
                case Key.Up: return VirtualKeyCode.VK_UP;
                // Math
                case Key.Add: return VirtualKeyCode.VK_ADD;
                case Key.Subtract: return VirtualKeyCode.VK_SUBTRACT;
                case Key.Divide: return VirtualKeyCode.VK_DIVIDE;
                case Key.Multiply: return VirtualKeyCode.VK_MULTIPLY;
                case Key.Decimal: return VirtualKeyCode.VK_DECIMAL;
                // Browser
                case Key.BrowserBack:      return VirtualKeyCode.VK_BROWSER_BACK;
                case Key.BrowserFavorites: return VirtualKeyCode.VK_BROWSER_FAVORITES;
                case Key.BrowserForward:   return VirtualKeyCode.VK_BROWSER_FORWARD;
                case Key.BrowserHome:      return VirtualKeyCode.VK_BROWSER_HOME;
                case Key.BrowserRefresh:   return VirtualKeyCode.VK_BROWSER_REFRESH;
                case Key.BrowserSearch:    return VirtualKeyCode.VK_BROWSER_SEARCH;
                case Key.BrowserStop:      return VirtualKeyCode.VK_BROWSER_STOP;
                // Function keys
                case Key.F1: return VirtualKeyCode.VK_F1;
                case Key.F2: return VirtualKeyCode.VK_F2;
                case Key.F3: return VirtualKeyCode.VK_F3;
                case Key.F4: return VirtualKeyCode.VK_F4;
                case Key.F5: return VirtualKeyCode.VK_F5;
                case Key.F6: return VirtualKeyCode.VK_F6;
                case Key.F7: return VirtualKeyCode.VK_F7;
                case Key.F8: return VirtualKeyCode.VK_F8;
                case Key.F9: return VirtualKeyCode.VK_F9;
                case Key.F10: return VirtualKeyCode.VK_F10;
                case Key.F11: return VirtualKeyCode.VK_F11;
                case Key.F12: return VirtualKeyCode.VK_F12;
                case Key.F13: return VirtualKeyCode.VK_F13;
                case Key.F14: return VirtualKeyCode.VK_F14;
                case Key.F15: return VirtualKeyCode.VK_F15;
                case Key.F16: return VirtualKeyCode.VK_F16;
                case Key.F17: return VirtualKeyCode.VK_F17;
                case Key.F18: return VirtualKeyCode.VK_F18;
                case Key.F19: return VirtualKeyCode.VK_F19;
                case Key.F20: return VirtualKeyCode.VK_F20;
                case Key.F21: return VirtualKeyCode.VK_F21;
                case Key.F22: return VirtualKeyCode.VK_F22;
                case Key.F23: return VirtualKeyCode.VK_F23;
                case Key.F24: return VirtualKeyCode.VK_F24;
                // IME keys
                case Key.FinalMode: return VirtualKeyCode.VK_FINAL;
                case Key.HangulMode: return VirtualKeyCode.VK_HANGUL;
                case Key.HanjaMode: return VirtualKeyCode.VK_HANJA;
                //case Key.KanaMode: return VirtualKeyCode.VK_KANA;
                //case Key.KanjiMode: return VirtualKeyCode.VK_KANJI;
                case Key.JunjaMode: return VirtualKeyCode.VK_JUNJA;
                case Key.ImeAccept: return VirtualKeyCode.VK_ACCEPT;
                case Key.ImeConvert: return VirtualKeyCode.VK_CONVERT;
                case Key.ImeModeChange: return VirtualKeyCode.VK_MODECHANGE;
                case Key.ImeNonConvert: return VirtualKeyCode.VK_NONCONVERT;
                case Key.ImeProcessed: return VirtualKeyCode.VK_PROCESSKEY;
                // OEM keys
                case Key.Oem1: return VirtualKeyCode.VK_OEM_1;  // Semicolon
                case Key.Oem2: return VirtualKeyCode.VK_OEM_2;  // Question mark
                case Key.Oem3: return VirtualKeyCode.VK_OEM_3;  // Tilde
                case Key.Oem4: return VirtualKeyCode.VK_OEM_4;  // open braket
                case Key.Oem5: return VirtualKeyCode.VK_OEM_5;  // Pipe
                case Key.Oem6: return VirtualKeyCode.VK_OEM_6;  // close braket
                case Key.Oem7: return VirtualKeyCode.VK_OEM_7;  // Quotes
                case Key.Oem8: return VirtualKeyCode.VK_OEM_8;
                case Key.Oem102: return VirtualKeyCode.VK_OEM_102;  // Backslash
                case Key.OemAttn: return VirtualKeyCode.VK_ATTN;
                //case Key.OemBackTab: return VirtualKeyCode
                case Key.OemClear: return VirtualKeyCode.VK_OEM_CLEAR;
                case Key.OemComma: return VirtualKeyCode.VK_OEM_COMMA;
                //case Key.OemCopy: return VirtualKeyCode
                //case Key.OemEnlw: return VirtualKeyCode;
                //case Key.OemFinish: return VirtualKeyCode
                case Key.OemMinus: return VirtualKeyCode.VK_OEM_MINUS;
                case Key.OemPeriod: return VirtualKeyCode.VK_OEM_PERIOD;
                case Key.OemPlus: return VirtualKeyCode.VK_OEM_PLUS;
                // Media Keys
                case Key.MediaNextTrack: return VirtualKeyCode.VK_MEDIA_NEXT_TRACK;
                case Key.MediaPlayPause: return VirtualKeyCode.VK_MEDIA_PLAY_PAUSE;
                case Key.MediaPreviousTrack: return VirtualKeyCode.VK_MEDIA_PREV_TRACK;
                case Key.MediaStop: return VirtualKeyCode.VK_MEDIA_STOP;

                case Key.Delete: return VirtualKeyCode.VK_DELETE;
                case Key.System: return VirtualKeyCode.VK_MENU;
                case Key.LeftAlt: return VirtualKeyCode.VK_LMENU;
                case Key.LeftCtrl: return VirtualKeyCode.VK_LCONTROL;
                case Key.LeftShift: return VirtualKeyCode.VK_LSHIFT;
                case Key.LWin: return VirtualKeyCode.VK_LWIN;
                case Key.RightAlt: return VirtualKeyCode.VK_RMENU;
                case Key.RightCtrl: return VirtualKeyCode.VK_RCONTROL;
                case Key.RightShift: return VirtualKeyCode.VK_RSHIFT;
                case Key.RWin: return VirtualKeyCode.VK_RWIN;
                case Key.Home: return VirtualKeyCode.VK_HOME;
                case Key.Insert: return VirtualKeyCode.VK_INSERT;
                case Key.PrintScreen: return VirtualKeyCode.VK_SNAPSHOT;
                case Key.Next: return VirtualKeyCode.VK_NEXT;
                case Key.Prior: return VirtualKeyCode.VK_PRIOR;
                case Key.NumLock: return VirtualKeyCode.VK_NUMLOCK;
                case Key.Capital: return VirtualKeyCode.VK_CAPITAL;
                case Key.Back: return VirtualKeyCode.VK_BACK;
                case Key.Print: return VirtualKeyCode.VK_PRINT;
                case Key.LaunchApplication1: return VirtualKeyCode.VK_LAUNCH_APP1;
                case Key.LaunchApplication2: return VirtualKeyCode.VK_LAUNCH_APP2;
                case Key.LaunchMail: return VirtualKeyCode.VK_LAUNCH_MAIL;
                case Key.Help: return VirtualKeyCode.VK_HELP;
                case Key.Apps: return VirtualKeyCode.VK_APPS;
                case Key.Attn: return VirtualKeyCode.VK_ATTN;
                case Key.Cancel: return VirtualKeyCode.VK_CANCEL;
                case Key.Clear: return VirtualKeyCode.VK_CLEAR;
                case Key.CrSel: return VirtualKeyCode.VK_CRSEL;
                case Key.End: return VirtualKeyCode.VK_END;
                case Key.Return: return VirtualKeyCode.VK_RETURN;
                case Key.EraseEof: return VirtualKeyCode.VK_EREOF;
                case Key.Escape: return VirtualKeyCode.VK_ESCAPE;
                case Key.Execute: return VirtualKeyCode.VK_EXECUTE;
                case Key.ExSel: return VirtualKeyCode.VK_EXSEL;
                case Key.Pa1: return VirtualKeyCode.VK_PA1;
                case Key.Pause: return VirtualKeyCode.VK_PAUSE;
                case Key.Play: return VirtualKeyCode.VK_PLAY;
                case Key.Scroll: return VirtualKeyCode.VK_SCROLL;
                case Key.Select: return VirtualKeyCode.VK_SELECT;
                case Key.SelectMedia: return VirtualKeyCode.VK_LAUNCH_MEDIA_SELECT;
                case Key.Separator: return VirtualKeyCode.VK_SEPARATOR;
                case Key.Sleep: return VirtualKeyCode.VK_SLEEP;
                case Key.Space: return VirtualKeyCode.VK_SPACE;
                case Key.Tab: return VirtualKeyCode.VK_TAB;
                case Key.VolumeDown: return VirtualKeyCode.VK_VOLUME_DOWN;
                case Key.VolumeMute: return VirtualKeyCode.VK_VOLUME_MUTE;
                case Key.VolumeUp: return VirtualKeyCode.VK_VOLUME_UP;
                case Key.Zoom: return VirtualKeyCode.VK_ZOOM;
            }

            return null;
        }

        static HID_USAGES ToHidAxis(VJoyDirector.AxisDirection axis, out bool positive)
        {
            var split = axis.ToString().Split('_');
            positive = split[1] == "Pos";
            return (HID_USAGES)Enum.Parse(typeof(HID_USAGES), "HID_USAGE_" + split[0]);
        }
    }
}
