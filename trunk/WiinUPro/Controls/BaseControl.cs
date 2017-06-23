using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Shared;

namespace WiinUPro
{
    public abstract class BaseControl : UserControl, IBaseControol
    {
        public event Delegates.StringDel OnInputRightClick;
        public event Delegates.StringDel OnInputSelected;
        public event AssignmentCollection.AssignDelegate OnQuickAssign;

        protected void CallEvent_OnInputRightClick(string value)
        {
            OnInputRightClick?.Invoke(value);
        }

        protected void CallEvent_OnInputSelected(string value)
        {
            OnInputSelected?.Invoke(value);
        }

        protected void CallEvent_OnQuickAssign(Dictionary<string, AssignmentCollection> collection)
        {
            OnQuickAssign?.Invoke(collection);
        }

        protected void OpenInput(object sender, RoutedEventArgs e = null)
        {
            var element = sender as FrameworkElement;
            var tag = element == null ? "" : element.Tag as string;

            // Open input assignment window
            if (OnInputSelected != null && tag != null)
            {
                OnInputSelected(tag);
            }
        }

        protected void Axis_Click(object sender, RoutedEventArgs e)
        {
            OpenInput(sender);
        }

        protected void Btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                OpenInput(sender);
            }
        }

        protected void Btn_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            var tag = element == null ? "" : element.Tag as string;

            // Open Context menu
            if (OnInputRightClick != null && tag != null)
            {
                OnInputRightClick(tag);
            }
        }

        protected void OpenContextMenu(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;

            if (element != null && element.ContextMenu != null)
            {
                element.ContextMenu.IsOpen = true;
            }
        }
    }

    public interface IBaseControol
    {
        event Delegates.StringDel OnInputRightClick;
        event Delegates.StringDel OnInputSelected;
        event AssignmentCollection.AssignDelegate OnQuickAssign;
    }
}
