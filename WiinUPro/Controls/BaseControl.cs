using System.Collections.Generic;
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

        protected virtual void OpenInput(object sender, RoutedEventArgs e = null)
        {
            var element = sender as FrameworkElement;
            var tag = element == null ? "" : _inputPrefix + element.Tag as string;

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

        protected virtual void OpenContextMenu(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;

            if (element != null && element.ContextMenu != null)
            {
                element.ContextMenu.IsOpen = true;
            }
        }

        protected virtual void QuickAssign(string prefix, string type)
        {
            Dictionary<string, AssignmentCollection> args = new Dictionary<string, AssignmentCollection>();

            if (type == "Mouse")
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
            else if (type == "Arrows")
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
                var tag = _inputPrefix + item.Tag as string;

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
                var header = item.Header as string;
                var prefix = item.Tag as string;

                if (header != null && prefix != null)
                {
                    float speed = 1f;
                    switch (header)
                    {
                        case "50% Speed": speed = 0.5f; break;
                        case "150% Speed": speed = 1.5f; break;
                        case "200% Speed": speed = 2.0f; break;
                        case "250% Speed": speed = 2.5f; break;
                        case "300% Speed": speed = 3.0f; break;
                        case "100% Speed":
                        default:
                            speed = 1f;
                            break;
                    }

                    Dictionary<string, AssignmentCollection> args = new Dictionary<string, AssignmentCollection>();
                    args.Add(_inputPrefix + prefix + "UP", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveUp, speed) }));
                    args.Add(_inputPrefix + prefix + "DOWN", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveDown, speed) }));
                    args.Add(_inputPrefix + prefix + "LEFT", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveLeft, speed) }));
                    args.Add(_inputPrefix + prefix + "RIGHT", new AssignmentCollection(new List<IAssignment> { new MouseAssignment(MouseInput.MoveRight, speed) }));
                    CallEvent_OnQuickAssign(args);
                }
            }
        }
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
