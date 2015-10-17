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
using WindowsInput.Native;

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

        public InputsWindow()
        {
            InitializeComponent();
            Result = new AssignmentCollection();
            keySelectedBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xBF, 0x5F, 0x0F));
            keyDeselectedBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xCD, 0xCD, 0xCD));

            _selectedKeys = new List<VirtualKeyCode>();
        }

        public InputsWindow(AssignmentCollection collection) : this()
        {
            // fill the window with current assignments
        }

        private void ToggleKey(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            
            if (btn != null)
            {
                VirtualKeyCode code;
                if (Enum.TryParse<VirtualKeyCode>(btn.Tag.ToString(), out code))
                {
                    if (btn.Background == keySelectedBrush && _selectedKeys.Contains(code))
                    {
                        // Deselect and remove from list
                        btn.Background = keyDeselectedBrush;
                        _selectedKeys.Remove(code);
                    }
                    else
                    {
                        btn.Background = keySelectedBrush;
                        _selectedKeys.Add(code);
                    }
                }
            }
        }

        private void acceptBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var key in _selectedKeys)
            {
                Result.Add(new KeyboardAssignment(key));
            }

            Close();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
