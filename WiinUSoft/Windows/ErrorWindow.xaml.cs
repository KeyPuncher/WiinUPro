using System;
using System.Windows;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private Exception _exception;

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow(Exception ex)
            : this()
        {
            _exception = ex;

            _errorMessage.Content = ex.Message;
            _errorStack.Text = ex.StackTrace;

            if (ex.Message.Contains("NintrollerLib"))
            {
                Version nVersion = System.Reflection.Assembly.LoadFrom("Nintroller.dll").GetName().Version;
                if (nVersion < new Version(2, 5))
                {
                    _errorMessage.Content = "Then Nintroller library is out of date.";
                    _errorStack.Text = "Please try the following:" + Environment.NewLine +
                        Environment.NewLine + "1) Uninstall WiinUSoft" + 
                        Environment.NewLine + "2) Reinstall WiinUSoft using the latest installer" +
                        Environment.NewLine + "3) Verify that the installed Nintroller.dll in the installation folder" +
                        " is version 2.5 by right clicking the file, choosing Properties, and choose the Details tab.";
                    _dontSendBtn.Content = "Close";
                }
            }
        }

        private void _dontSendBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }
    }
}
