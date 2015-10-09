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
        internal Nintroller _nintroller;

        public NintyControl()
        {
            InitializeComponent();
        }

        public NintyControl(string devicePath) : this()
        {
            _nintroller = new Nintroller(devicePath);
            _view.Child = new ProControl();
            ((UserControl)_view.Child).HorizontalAlignment = HorizontalAlignment.Left;
            ((UserControl)_view.Child).VerticalAlignment = VerticalAlignment.Top;
        }
    }

    public interface INintyControl
    {
        void UpdateVisual(INintrollerState state);
    }
}
