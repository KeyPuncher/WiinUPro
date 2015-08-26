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
using System.Windows.Shapes;

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
        }

        private void _dontSendBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _sendBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();

            // Option 1, send an email
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add("justin@wiinupro.com");
            message.Subject = "WiinUSoft Error";
            message.From = new System.Net.Mail.MailAddress("justin@wiinupro.com");
            message.Body = _exception.InnerException != null
                ? string.Format("Date: {2}\n\nUser Comments: {5}\n\nMessage: {0}\n\nStack: {1}\n\nInner Message: {3}\n\nInnerStack:\n {4}",
                    _exception.Message,                     // 0
                    _exception.StackTrace,                  // 1
                    System.DateTime.Now,                    // 2
                    _exception.InnerException.Message,      // 3
                    _exception.InnerException.StackTrace,   // 4
                    _userInfo.Text)                         // 5
                : string.Format("Date: {2}\n\nUser Comments: {3}\n\nMessage: {0}\n\nStack:\n {1}",
                    _exception.Message,                     // 0
                    _exception.StackTrace,                  // 1
                    System.DateTime.Now,                    // 2
                    _userInfo.Text);                        // 3

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("wiinuproError@gmail.com", "wiinupro100");
            smtp.EnableSsl = true;
            smtp.Timeout = 30;

            try
            {
                smtp.Send(message);
            }
            catch
            {
                // it's okay if we fail, we just won't notify the user
            }
            
            // Option 2, post to server
        }
    }
}
