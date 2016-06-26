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

        private List<VirtualKeyCode> _selectedKeys;
        private List<MouseInput> _selectedMouseDirections;
        private List<InputManager.Mouse.MouseKeys> _selectedMouseButtons;
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
            _selectedXInputButtons = new List<X360Button>();
            _selectedXInputAxes = new List<X360Axis>();
        }

        public InputsWindow(AssignmentCollection collection) : this()
        {
            // fill the window with current assignments
        }

        private void AddToList<TEnum>(object obj, List<TEnum> list) where TEnum : struct
        {
            var btn = obj as Button;

            if (btn != null)
            {
                TEnum inputType;
                if (Enum.TryParse(btn.Tag.ToString(), out inputType))
                {
                    if (btn.Background == keySelectedBrush && list.Contains(inputType))
                    {
                        // Deselect and remove from list
                        btn.Background = keyDeselectedBrush;
                        list.Remove(inputType);
                    }
                    else
                    {
                        btn.Background = keySelectedBrush;
                        list.Add(inputType);
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
                Result.Add(new XInputButtonAssignment(xBtn) { Device = ScpDirector.XInput_Device.Device_A });
            }

            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void xInputConnect_Click(object sender, RoutedEventArgs e)
        {
            ScpDirector.Access.ConnectDevice(ScpDirector.XInput_Device.Device_A);
        }

        private void xInputDisconnect_Click(object sender, RoutedEventArgs e)
        {
            ScpDirector.Access.DisconnectDevice(ScpDirector.XInput_Device.Device_A);
        }
    }
}
