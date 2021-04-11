using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        bool cancelled = false;

        public SyncWindow()
        {
            InitializeComponent();
        }

        public void Sync()
        {
            Guid HIDServiceClass = Guid.Parse(NativeImports.HID_GUID);
            List<IntPtr> btRadios = new List<IntPtr>();
            int pairedCount = 0;
            IntPtr foundRadio;
            IntPtr handle;
            NativeImports.BLUETOOTH_FIND_RADIO_PARAMS radioParams = new NativeImports.BLUETOOTH_FIND_RADIO_PARAMS();

            radioParams.Initialize();

            handle = NativeImports.BluetoothFindFirstRadio(ref radioParams, out foundRadio);
            bool next = handle != IntPtr.Zero;

            do
            {
                if (foundRadio != IntPtr.Zero)
                {
                    btRadios.Add(foundRadio);
                }

                next = NativeImports.BluetoothFindNextRadio(ref handle, out foundRadio);
            }
            while (next);
            
            if (btRadios.Count > 0)
            {
                Prompt(Globalization.Translate("Sync_Searching"));

                while (pairedCount == 0 && !cancelled)
                {
                    for (int r = 0; r < btRadios.Count; r++)
                    {
                        IntPtr found;
                        NativeImports.BLUETOOTH_RADIO_INFO radioInfo = new NativeImports.BLUETOOTH_RADIO_INFO();
                        NativeImports.BLUETOOTH_DEVICE_INFO deviceInfo = new NativeImports.BLUETOOTH_DEVICE_INFO();
                        NativeImports.BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams = new NativeImports.BLUETOOTH_DEVICE_SEARCH_PARAMS();

                        radioInfo.Initialize();
                        deviceInfo.Initialize();
                        searchParams.Initialize();

                        uint getInfoError = NativeImports.BluetoothGetRadioInfo(btRadios[r], ref radioInfo);
                        
                        // Success
                        if (getInfoError == 0)
                        {
                            searchParams.fReturnAuthenticated = false;
                            searchParams.fReturnRemembered = false;
                            searchParams.fReturnConnected = false;
                            searchParams.fReturnUnknown = true;
                            searchParams.fIssueInquiry = true;
                            searchParams.cTimeoutMultiplier = 2;
                            searchParams.hRadio = btRadios[r];

                            found = NativeImports.BluetoothFindFirstDevice(ref searchParams, ref deviceInfo);

                            if (found != IntPtr.Zero)
                            {
                                do
                                {
                                    bool controller = SupportedDevice(deviceInfo.szName);
                                    bool wiiDevice = deviceInfo.szName.StartsWith("Nintendo RVL");
                                    
                                    if (controller || wiiDevice)
                                    {
                                        Prompt(Globalization.TranslateFormat("Sync_Found", deviceInfo.szName));

                                        StringBuilder password = new StringBuilder();
                                        uint pcService = 16;
                                        Guid[] guids = new Guid[16];
                                        bool success = true;

                                        if (deviceInfo.fRemembered)
                                        {
                                            // Remove current pairing
                                            Prompt(Globalization.TranslateFormat("Sync_Unpairing"));
                                            uint errForget = NativeImports.BluetoothRemoveDevice(ref deviceInfo.Address);
                                            success = errForget == 0;
                                        }

                                        if (wiiDevice)
                                        {
                                            // use MAC address of BT radio as pin
                                            var bytes = BitConverter.GetBytes(radioInfo.address);
                                            for (int i = 0; i < 6; i++)
                                            {
                                                password.Append((char)bytes[i]);
                                            }

                                            if (success)
                                            {
                                                Prompt(Globalization.Translate("Sync_Pairing"));
                                                var errPair = NativeImports.BluetoothAuthenticateDevice(IntPtr.Zero, btRadios[r], ref deviceInfo, password.ToString(), 6);
                                                success = errPair == 0;
                                            }

                                            if (success)
                                            {
                                                Prompt(Globalization.Translate("Sync_Service"));
                                                var errService = NativeImports.BluetoothEnumerateInstalledServices(btRadios[r], ref deviceInfo, ref pcService, guids);
                                                success = errService == 0;
                                            }

                                            if (success)
                                            {
                                                Prompt(Globalization.Translate("Sync_HID"));
                                                var errActivate = NativeImports.BluetoothSetServiceState(btRadios[r], ref deviceInfo, ref HIDServiceClass, 0x01);
                                                success = errActivate == 0;
                                            }

                                            if (success)
                                            {
                                                Prompt(Globalization.Translate("Sync_Success"));
                                                pairedCount += 1;
                                            }
                                            else
                                            {
                                                Prompt(Globalization.Translate("Sync_Failure"));
                                            }
                                        }
                                        else
                                        {
                                            Prompt(Globalization.Translate("Sync_Finish"));
                                            var err = NativeImports.BluetoothAuthenticateDeviceEx(IntPtr.Zero, btRadios[r], ref deviceInfo, null, NativeImports.AUTHENTICATION_REQUIREMENTS.MITMProtectionNotRequired);

                                            if (err == 0)
                                            {
                                                Prompt(Globalization.Translate("Sync_Success"));
                                                pairedCount += 1;
                                            }
                                            else
                                            {
                                                Prompt(Globalization.Translate("Sync_Incomplete"));
                                            }
                                        }
                                    }
                                }
                                while (NativeImports.BluetoothFindNextDevice(found, ref deviceInfo));
                            }
                        }
                        else
                        {
                            // Failed to get Bluetooth Radio Info
                            Prompt(Globalization.Translate("Sync_Bluetooth_Failed"));
                        }
                    }
                }

                // Close Opened Radios
                foreach (var openRadio in btRadios)
                {
                    NativeImports.CloseHandle(openRadio);
                }
            }
            else
            {
                // No Bluetooth Radios found
                Prompt(Globalization.Translate("Sync_No_Bluetooth"));
                System.Threading.Thread.Sleep(3000);
            }

            NativeImports.BluetoothFindRadioClose(handle);

            Dispatcher.BeginInvoke((Action)(() => Close()));
        }

        public bool SupportedDevice(string deviceName)
        {
            switch (deviceName)
            {
                case "Pro Controller": return true;
                case "Joy-Con (L)": return true;
                case "Joy-Con (R)": return true;
            }

            return false;
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
            Task t = new Task(() => Sync());
            t.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancelled = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Prompt(Globalization.Translate("Sync_Cancel"));
            cancelled = true;
        }
    }
}
