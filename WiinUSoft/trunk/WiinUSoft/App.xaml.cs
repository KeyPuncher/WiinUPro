using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace WiinUSoft
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "wiinupro-or-wiinusoft-instance";

        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MessageBox.Show("WiinUSoft is already Running!");

            // show the original instance
            if (this.MainWindow.WindowState == WindowState.Minimized)
            {
                ((MainWindow)this.MainWindow).ShowWindow();
            }

            this.MainWindow.Activate();
            return true;
        }
    }
}
