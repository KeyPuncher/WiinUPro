using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }

        private void MoveSelector(Shape s, Ellipse selector, Line guide)
        {
            double top = Canvas.GetTop(((UIElement)s.Parent));
            top += s.Margin.Top;
            top -= selector.Height / 2;
            top += s.Height / 2;

            double left = Canvas.GetLeft((UIElement)(s.Parent));
            left += s.Margin.Left;
            left -= selector.Width / 2;
            left += s.Width / 2;

            Canvas.SetTop(selector, top);
            Canvas.SetLeft(selector, left);

            Canvas.SetLeft(guide, Canvas.GetLeft(selector) + selector.Width / 2);
            guide.Y2 = Canvas.GetTop(selector) - selector.Height / 2 - selector.StrokeThickness;

            if (guide == guideMap)
            {
                guideMeet.X2 = Canvas.GetLeft(guide);
            }
            else
            {
                guideMeet.X1 = Canvas.GetLeft(guide);
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Shape clickArea = (Shape)sender;
            clickArea.StrokeThickness = 1;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Shape clickArea = (Shape)sender;
            clickArea.StrokeThickness = 0;
        }

        private void device_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Shape s = (Shape)sender;
            deviceLabel.Content = s.ToolTip;
            MoveSelector((Shape)sender, selectionDevice, guideDevice);
            // TODO: select map
        }

        private void map_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Shape s = (Shape)sender;
            mapLabel.Content = s.ToolTip;
            MoveSelector((Shape)sender, selectionMap, guideMap);
            // TODO: update mapping
        }
    }
}
