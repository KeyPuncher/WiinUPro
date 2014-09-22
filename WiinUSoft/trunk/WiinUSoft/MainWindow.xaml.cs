using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point dragStart;
        public MainWindow()
        {
            InitializeComponent();

            DragCanvas.SetCanBeDragged(dest, false);
        }

        private void Rectangle_DragEnter(object sender, DragEventArgs e)
        {
            // When the mouse is over this control while dragging
            Debug.WriteLine("Drag Enter");
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            // When dragging is dropped/stopped over this control
            Debug.WriteLine("Dropped");
        }

        private void Rectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // When the mouse button is clicked on this control
            Debug.WriteLine("Mouse Left Down");
            dragStart = e.GetPosition(null);
        }

        private void Rectangle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // when the mouse is moving over this control before dragging has started
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(sender as Rectangle, "test", DragDropEffects.Move);
            } 
        }

        private void Rectangle_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // Called while dragging this ojbect
            Debug.WriteLine("dragging");
            Debug.WriteLine(Mouse.GetPosition(null));
        }

        private void Rectangle_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            Debug.WriteLine("Continue Dragging");
            Debug.WriteLine(Mouse.GetPosition(null));
        }




        private void testRect_GotMouseCapture(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Got Mouse Capture");
        }

        private bool dragging = false;
        private void testRect_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("Mouse Capture state changed: " + e.NewValue.ToString());

            dragging = (bool)e.NewValue;

            Debug.WriteLine(dragging);
            if (!dragging)
            {
                DragCanvas.SetLeft((UIElement)sender, 0);
            }
        }

        private void testRect_IsMouseCaptureWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("Mouse Capture Within state changed: " + e.NewValue.ToString());
        }

        private void testRect_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine("mouse over");
        }

    }
}
