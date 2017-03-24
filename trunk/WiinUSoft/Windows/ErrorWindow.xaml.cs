using System;
using System.Windows;
using RestSharp;
using RestSharp.Authenticators;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private const string KEY    = "key-67c739dec007c874f0f6c4362cda08b4";
        private const string DOMAIN = "sandboxb83b809e3b6d4692bea3f0664adce540.mailgun.org";

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
            Application.Current.Shutdown();
        }

        private void _sendBtn_Click(object sender, RoutedEventArgs e)
        {
            string messageBody = "";
            string appVersion = "";
            string nintrollerVersion = "";
            string installLocation = "";
            string user = "";

            try
            {
                Version version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
                appVersion = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Revision);

                Version nVersion = System.Reflection.Assembly.LoadFrom("Nintroller.dll").GetName().Version;
                nintrollerVersion = string.Format("{0}.{1}.{2}.{3}", nVersion.Major, nVersion.Minor, nVersion.Build, nVersion.Revision);

                installLocation = System.Reflection.Assembly.GetEntryAssembly().Location;

                // Just so we know if we are getting repeating errors from the same person
                user = Environment.UserName;
            }
            catch { }

            messageBody = string.Format("Date: {2}\n\nOS: {3}\n\nWiinUSoft Version: {5}\n\nNintroller Version: {6}\n\nInstall Location: {7}\n\nUser: {8}\n\nUser Comments: {4}\n\nMessage: {0}\n\nStack:\n {1}",
                    _exception.Message,                     // 0
                    _exception.StackTrace,                  // 1
                    DateTime.Now,                           // 2
                    Environment.OSVersion.ToString(),       // 3
                    _userInfo.Text,                         // 4
                    appVersion,                             // 5
                    nintrollerVersion,                      // 6
                    installLocation,                        // 7
                    user);                                  // 8

            if (_exception.InnerException != null)
            {
                messageBody += string.Format("n\nInner Message: {0}\n\nInnerStack:\n {1}",
                    _exception.InnerException.Message,      // 0
                    _exception.InnerException.StackTrace);  // 1
            }
            
            // Send Email using MailGun
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");
            client.Authenticator = new HttpBasicAuthenticator("api", KEY);
            RestRequest request = new RestRequest();
            request.AddParameter("domain", DOMAIN, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "WiinUSoft <wiinuproerror@gmail.com>");
            request.AddParameter("to", "justin@wiinupro.com");
            request.AddParameter("subject", "WiinUSoft Error");
            request.AddParameter
            (
                "text", messageBody
            );
            request.Method = Method.POST;

            try
            {
                var result = client.Execute(request);
            }
            catch
            {
                // it's okay if we fail, we just won't notify the user
            }

            Application.Current.Shutdown();
        }
    }
}
