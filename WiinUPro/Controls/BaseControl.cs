﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Shared;

namespace WiinUPro
{
    public abstract class BaseControl : UserControl, IBaseControl
    {
        public string DeviceID { get; protected set; }
        public event Delegates.StringDel OnInputRightClick;
        public event Delegates.StringDel OnInputSelected;
        public event AssignmentCollection.AssignDelegate OnQuickAssign;
        public event Delegates.StringArrDel OnRemoveInputs;

        protected string _inputPrefix = "";
        protected string _menuOwnerTag = "";
        protected ContextMenu _analogMenu;
        protected string _analogMenuInput = "";

        protected BaseControl()
        {
            _analogMenu = new ContextMenu();

            // Create menu items for assigning input directions.
            _analogMenu.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Up"), Tag="UP" });
            _analogMenu.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Left"), Tag="LEFT" });
            _analogMenu.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Right"), Tag="RIGHT" });
            _analogMenu.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Down"), Tag="DOWN" });
            _analogMenu.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Joy_Button"), Tag="S" });

            // Create menu items for analog triggers
            _analogMenu.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Press"), Tag="T" });
            _analogMenu.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Press_Full"), Tag="FULL" });

            foreach (var menuItem in _analogMenu.Items)
                (menuItem as MenuItem).Click += OpenSelectedInput;

            _analogMenu.Items.Add(new Separator());

            // Create menu items for quick assignment
            var quickAssign = new MenuItem { Header = Globalization.Translate("Context_Quick") };
            var quickMouse = new MenuItem { Header = Globalization.Translate("Context_Quick_Mouse") };
            {
                quickMouse.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Quick_Mouse_50"), Tag = "50" });
                quickMouse.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Quick_Mouse_100"), Tag = "100" });
                quickMouse.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Quick_Mouse_150"), Tag = "150" });
                quickMouse.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Quick_Mouse_200"), Tag = "200" });
                quickMouse.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Quick_Mouse_250"), Tag = "250" });
                quickMouse.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Quick_Mouse_300"), Tag = "300" });
            }
            foreach (var menuItem in quickMouse.Items)
                (menuItem as MenuItem).Click += QuickAssignMouse_Click;
            quickAssign.Items.Add(quickMouse);
            quickAssign.Items.Add(new MenuItem { Header = "WASD" });
            quickAssign.Items.Add(new MenuItem { Header = Globalization.Translate("Context_Quick_Arrows") });
            (quickAssign.Items[1] as MenuItem).Click += QuickAssign_Click;
            (quickAssign.Items[2] as MenuItem).Click += QuickAssign_Click;
            _analogMenu.Items.Add(quickAssign);

            var mousePointer = new MenuItem { Header = Globalization.Translate("Context_Quick_Mouse_Pointer"), Tag = "Pointer" };
            mousePointer.Click += QuickAssignMouseAbsolute_Click;
            _analogMenu.Items.Add(mousePointer);

            _analogMenu.Items.Add(new Separator());

            // Create menu items for IR camera
            var irMode = new MenuItem { Header = Globalization.Translate("Context_IR_Mode") };
            {
                irMode.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Basic") });
                irMode.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Wide") });
                irMode.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Full") });
                irMode.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Off") });
            }
            foreach (var irModeItem in irMode.Items)
                (irModeItem as MenuItem).Click += SetIRCamMode_Click;
            var irLevel = new MenuItem { Header = Globalization.Translate("Context_IR_Level") };
            {
                irLevel.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Level_1") });
                irLevel.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Level_2") });
                irLevel.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Level_3") });
                irLevel.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Level_4") });
                irLevel.Items.Add(new MenuItem { Header = Globalization.Translate("Context_IR_Level_5") });
            }
            foreach (var irLevelItem in irLevel.Items)
                (irLevelItem as MenuItem).Click += SetIRCamSensitivity_Click;
            _analogMenu.Items.Add(irMode);
            _analogMenu.Items.Add(irLevel);

            // Add Calibration menu item
            var calibrationItem = new MenuItem { Header = Globalization.Translate("Context_Calibrate") };
            calibrationItem.Click += CalibrateInput_Click;
            _analogMenu.Items.Add(calibrationItem);
            
            ContextMenu = _analogMenu;
            ContextMenuService.SetIsEnabled(this, false);
        }

        protected void SetupAnalogMenuForJoystick(bool withStickClick)
        {
            int i = 0;
            for (; i < 4; ++i) 
            {
                (_analogMenu.Items[i] as MenuItem).Visibility = Visibility.Visible;
            }

            (_analogMenu.Items[i++] as MenuItem).Visibility = withStickClick ? Visibility.Visible : Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as Separator).Visibility = Visibility.Visible;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Visible;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as Separator).Visibility = Visibility.Visible;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Visible;
        }

        protected void SetupAnalogMenuForDpad()
        {
            int i = 0;
            for (; i < 4; ++i)
            {
                (_analogMenu.Items[i] as MenuItem).Visibility = Visibility.Visible;
            }

            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as Separator).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Visible;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as Separator).Visibility = Visibility.Visible;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
        }

        protected void SetupAnalogMenuForTrigger()
        {
            int i = 0;
            for (; i < 5; ++i)
            {
                (_analogMenu.Items[i] as MenuItem).Visibility = Visibility.Collapsed;
            }

            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Visible;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Visible;
            (_analogMenu.Items[i++] as Separator).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as Separator).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Collapsed;
            (_analogMenu.Items[i++] as MenuItem).Visibility = Visibility.Visible;
        }

        protected void SetupAnalogMenuForIRCam()
        {
            int i = 0;
            for (; i < 4; ++i)
            {
                (_analogMenu.Items[i] as MenuItem).Visibility = Visibility.Visible;
            }

            for (; i < 7; ++i)
            {
                (_analogMenu.Items[i] as MenuItem).Visibility = Visibility.Collapsed;
            }
            
            for (; i < _analogMenu.Items.Count; ++i)
            {
                ((Control)_analogMenu.Items[i]).Visibility = Visibility.Visible;
            }
        }

        protected void UpdateTooltipLine(FrameworkElement element, string update, int line)
        {
            if (!(element.ToolTip is string))
                return;

            string[] parts = ((string)element.ToolTip).Split('\n');
            if (parts.Length > line)
                parts[line] = update;

            string result = parts[0];
            for (int i = 1; i < parts.Length; ++i)
                result += "\n" + parts[i];

            element.ToolTip = result;
        }

        #region Event passthroughs
        protected virtual void CallEvent_OnInputRightClick(string value)
        {
            OnInputRightClick?.Invoke(value);
        }

        protected virtual void CallEvent_OnInputSelected(string value)
        {
            OnInputSelected?.Invoke(value);
        }

        protected virtual void CallEvent_OnQuickAssign(Dictionary<string, AssignmentCollection> collection)
        {
            OnQuickAssign?.Invoke(collection);
        }

        protected virtual void CallEvent_OnRemoveInputs(string[] inputs)
        {
            OnRemoveInputs?.Invoke(inputs);
        }
        #endregion

        protected virtual void OpenInput(object sender, RoutedEventArgs e = null)
        {
            var element = sender as FrameworkElement;
            var tag = element == null ? "" : _inputPrefix + element.Tag as string;

            // TODO: Solve issue where this is being triggered twice for the guitar
            // Open input assignment window
            if (OnInputSelected != null && tag != null)
            {
                OnInputSelected(tag);
            }
        }

        protected virtual void Axis_Click(object sender, RoutedEventArgs e)
        {
            OpenInput(sender);
        }

        protected virtual void Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                OpenInput(sender);
            }
        }

        protected virtual void Btn_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            var tag = element == null ? "" : element.Tag as string;

            // Open Context menu
            if (OnInputRightClick != null && tag != null)
            {
                OnInputRightClick(tag);
            }
        }

        #region Context Menu Actions
        protected virtual void OpenContextMenu(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;

            if (element != null && element.ContextMenu != null)
            {
                element.ContextMenu.IsOpen = true;
            }
        }

        protected virtual void OpenJoystickMenu(object sender, MouseButtonEventArgs e)
        {
            var item = sender as FrameworkElement;

            if (item != null)
            {
                _menuOwnerTag = item.Tag as string;
                SetupAnalogMenuForJoystick(false);
                _analogMenuInput = _inputPrefix + (item.Tag as string);
                _analogMenu.IsOpen = true;
            }
        }

        protected virtual void OpenJoystickClickMenu(object sender, MouseButtonEventArgs e)
        {
            var item = sender as FrameworkElement;

            if (item != null)
            {
                _menuOwnerTag = item.Tag as string;
                SetupAnalogMenuForJoystick(true);
                _analogMenuInput = _inputPrefix + (item.Tag as string);
                _analogMenu.IsOpen = true;
            }
        }

        protected virtual void OpenDpadMenu(object sender, MouseButtonEventArgs e)
        {
            var item = sender as FrameworkElement;

            if (item != null)
            {
                _menuOwnerTag = item.Tag as string;
                SetupAnalogMenuForDpad();
                _analogMenuInput = _inputPrefix + (item.Tag as string);
                _analogMenu.IsOpen = true;
            }
        }

        protected virtual void OpenTriggerMenu(object sender, MouseButtonEventArgs e)
        {
            var item = sender as FrameworkElement;

            if (item != null)
            {
                _menuOwnerTag = item.Tag as string;
                SetupAnalogMenuForTrigger();
                _analogMenuInput = _inputPrefix + (item.Tag as string);
                _analogMenu.IsOpen = true;
            }
        }

        protected virtual void OpenIRCamMenu(object sender, MouseButtonEventArgs e)
        {
            var item = sender as FrameworkElement;

            if (item != null)
            {
                _menuOwnerTag = item.Tag as string;
                SetupAnalogMenuForIRCam();
                _analogMenuInput = _inputPrefix + (item.Tag as string);
                _analogMenu.IsOpen = true;
            }
        }

        protected virtual void OpenSelectedInput(object sender, RoutedEventArgs e)
        {
            OnInputSelected?.Invoke(_analogMenuInput + (sender as FrameworkElement)?.Tag?.ToString());
        }

        protected virtual void QuickAssign(string prefix, string type)
        {
            Dictionary<string, AssignmentCollection> args = new Dictionary<string, AssignmentCollection>();

            if (type == Globalization.Translate("Context_Quick_Mouse"))
            {
                args.Add(prefix + "UP", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveUp) }));
                args.Add(prefix + "DOWN", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveDown) }));
                args.Add(prefix + "LEFT", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveLeft) }));
                args.Add(prefix + "RIGHT", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveRight) }));
            }
            else if (type == "WASD")
            {
                args.Add(prefix + "UP", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_W) }));
                args.Add(prefix + "DOWN", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_S) }));
                args.Add(prefix + "LEFT", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_A) }));
                args.Add(prefix + "RIGHT", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.K_D) }));
            }
            else if (type == Globalization.Translate("Context_Quick_Arrows"))
            {
                args.Add(prefix + "UP", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_UP) }));
                args.Add(prefix + "DOWN", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_DOWN) }));
                args.Add(prefix + "LEFT", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_LEFT) }));
                args.Add(prefix + "RIGHT", new AssignmentCollection(new List<IAssignment> { new KeyboardAssignment(InputManager.VirtualKeyCode.VK_RIGHT) }));
            }

            CallEvent_OnQuickAssign(args);
        }

        protected virtual void QuickAssign_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var header = item.Header as string;
                var tag = _inputPrefix + _menuOwnerTag ?? "" + item.Tag as string;

                if (header != null && tag != null)
                {
                    QuickAssign(tag, header);
                }
            }
        }

        protected virtual void QuickAssignMouse_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var mouseSpeed = item.Tag as string;

                if (mouseSpeed != null)
                {
                    float speed;
                    switch (mouseSpeed)
                    {
                        case "50": speed = 0.5f; break;
                        case "150": speed = 1.5f; break;
                        case "200": speed = 2.0f; break;
                        case "250": speed = 2.5f; break;
                        case "300": speed = 3.0f; break;
                        case "100":
                        default:
                            speed = 1f;
                            break;
                    }

                    string prefix = (_inputPrefix ?? "") + (_menuOwnerTag ?? "");

                    Dictionary<string, AssignmentCollection> args = new Dictionary<string, AssignmentCollection>();
                    args.Add(prefix + "UP", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveUp, speed) }));
                    args.Add(prefix + "DOWN", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveDown, speed) }));
                    args.Add(prefix + "LEFT", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveLeft, speed) }));
                    args.Add(prefix + "RIGHT", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveRight, speed) }));
                    CallEvent_OnQuickAssign(args);
                }
            }
        }

        protected virtual void QuickAssignMouseAbsolute_Click(object sender, RoutedEventArgs e)
        {
            // To be overriden in WiiControl.
        }

        protected virtual void SetIRCamMode_Click(object sender, RoutedEventArgs e)
        {
            // To be overriden in WiiControl.
        }

        protected virtual void SetIRCamSensitivity_Click(object sender, RoutedEventArgs e)
        {
            // To be overriden in WiiControl.
        }

        protected virtual void CalibrateInput_Click(object sender, RoutedEventArgs e)
        {
            CalibrateInput((_inputPrefix ?? "") + (_menuOwnerTag ?? ""));
        }
        #endregion

        protected abstract void CalibrateInput(string inputName);

        #region Input Highlighting
        public static readonly DependencyProperty HighlightProperty = App.IsDesignMode ? null : DependencyProperty.Register("HighlightInput", typeof(bool), typeof(UIElement));
        public static readonly DependencyProperty DisplayProperty = App.IsDesignMode ? null : DependencyProperty.Register("DisplayInput", typeof(bool), typeof(UIElement));

        /// For mousing over an input
        protected void HighlightElement(UIElement element, bool doHighlight)
        {
            if (doHighlight != (bool)element.GetValue(HighlightProperty))
            {
                element.SetValue(HighlightProperty, doHighlight);
                element.Opacity += doHighlight ? 0.2d : -0.2d;
            }
        }

        /// For displaying when input is activated
        protected void Display(UIElement element, bool doDisplay)
        {
            if (doDisplay != (bool)element.GetValue(DisplayProperty))
            {
                element.SetValue(DisplayProperty, doDisplay);
                element.Opacity += doDisplay ? 0.8d : -0.8d;
            }
        }

        protected void Highlight(object sender, MouseEventArgs e)
        {
            HighlightElement((UIElement)sender, true);
        }

        protected void Unhighlight(object sender, MouseEventArgs e)
        {
            HighlightElement((UIElement)sender, false);
        }
        #endregion
    }

    public interface IBaseControl
    {
        string DeviceID { get; }
        event Delegates.StringDel OnInputRightClick;
        event Delegates.StringDel OnInputSelected;
        event AssignmentCollection.AssignDelegate OnQuickAssign;
        event Delegates.StringArrDel OnRemoveInputs;
    }
}
