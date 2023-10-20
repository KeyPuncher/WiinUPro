using System;
using System.Threading;
using System.Windows;
using Shared;
using Shared.Windows;

namespace WiinUPro.Windows
{
    /// <summary>
    /// Interaction logic for SyncWindow.xaml
    /// </summary>
    public partial class SyncWindow : Window
    {
        WinBtConnector connector;
        CancellationTokenSource cancellationToken;

        public SyncWindow()
        {
            connector = new WinBtConnector(DeviceSupported, ConnectionUpdate, Completed);
            InitializeComponent();
        }

        private WinBtConnector.ConnectType DeviceSupported(string deviceName)
        {
            if (deviceName.StartsWith("Nintendo RVL"))
                return WinBtConnector.ConnectType.WiiLike;

            switch (deviceName)
            {
                case "Pro Controller":
                case "Joy-Con (L)":
                case "Joy-Con (R)":
                    return WinBtConnector.ConnectType.Unauthenticated;
            }

            return WinBtConnector.ConnectType.Unsupported;
        }

        private void ConnectionUpdate(WinBtConnector.StatusUpdate status, string message)
        {
            switch (status)
            {
                case WinBtConnector.StatusUpdate.Complete:
                    Prompt(Globalization.Translate("Sync_Finish"));
                    break;
                case WinBtConnector.StatusUpdate.NoRadios:
                    Prompt(Globalization.Translate("Sync_No_Bluetooth"));
                    break;
                case WinBtConnector.StatusUpdate.Searching:
                    Prompt(Globalization.Translate("Sync_Searching"));
                    break;
                case WinBtConnector.StatusUpdate.DeviceFound:
                    Prompt(Globalization.TranslateFormat("Sync_Found", message));
                    break;
                case WinBtConnector.StatusUpdate.Unpairing:
                    Prompt(Globalization.TranslateFormat("Sync_Unpairing"));
                    break;
                case WinBtConnector.StatusUpdate.Pairing:
                    Prompt(Globalization.Translate("Sync_Pairing") + $" {message}");
                    break;
                case WinBtConnector.StatusUpdate.CheckingServices:
                    Prompt(Globalization.Translate("Sync_Service"));
                    break;
                case WinBtConnector.StatusUpdate.SettingService:
                    Prompt(Globalization.Translate("Sync_HID"));
                    break;
                case WinBtConnector.StatusUpdate.Success:
                    Prompt(Globalization.Translate("Sync_Success"));
                    break;
                case WinBtConnector.StatusUpdate.Error_RadioInfo:
                    Prompt(Globalization.Translate("Sync_Bluetooth_Failed") + $" Code {message}");
                    break;
                case WinBtConnector.StatusUpdate.Error_Unpairing:
                    Prompt(Globalization.Translate("Sync_Incomplete") + $" Code {message} (Unpairing)");
                    break;
                case WinBtConnector.StatusUpdate.Error_Pairing:
                    Prompt(Globalization.Translate("Sync_Failure") + $" Code {message}");
                    break;
                case WinBtConnector.StatusUpdate.Error_CheckingServices:
                    Prompt(Globalization.Translate("Sync_Incomplete") + $" Code {message} (Services 1)");
                    break;
                case WinBtConnector.StatusUpdate.Error_SettingService:
                    Prompt(Globalization.Translate("Sync_Incomplete") + $" Code {message} (Services 2)");
                    break;
            }
        }

        private void Completed()
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    Close();
                }));
            }
        }

        private void Prompt(string text)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                status.Text += text + Environment.NewLine;
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Globalization.ApplyTranslations(this);
            cancellationToken = new CancellationTokenSource();
            connector.BeginSync(cancellationToken.Token);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancellationToken.Cancel();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            if (connector.IsRunning)
            {
                Prompt(Globalization.Translate("Sync_Cancel"));
                cancellationToken.Cancel();
            }
            else
            {
                Close();
            }
        }
    }
}