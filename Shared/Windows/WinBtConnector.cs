using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Windows
{
    public class WinBtConnector
    {
        private Func<string, ConnectType> _isDeviceSupported;
        private Action<StatusUpdate, string> _updateCallback;
        private Action _completeCallback;
        private CancellationToken _cancelToken;
        private Task _task;

        public bool IsRunning { get { return _task != null && _task.Status == TaskStatus.Running; } }

        public WinBtConnector(Func<string, ConnectType> isSupported, Action<StatusUpdate, string> updateCallback, Action onComplete)
        {
            _isDeviceSupported = isSupported;
            _updateCallback = updateCallback;
            _completeCallback = onComplete;
        }

        public void BeginSync(CancellationToken cancelToken)
        {
            _cancelToken = cancelToken;
            _task = new Task(Sync, cancelToken);
            _task.Start();
        }

        private void Sync()
        {
            int pairedCount = 0;
            Guid HIDServiceClass = Guid.Parse(NativeImports.HID_GUID);
            List<IntPtr> btRadios = new List<IntPtr>();
            var radioParams = new NativeImports.BLUETOOTH_FIND_RADIO_PARAMS();
            IntPtr radioHandle;
            IntPtr foundHandle;

            // Look for Windows Bluetooth Hardware
            radioParams.Initialize();
            foundHandle = NativeImports.BluetoothFindFirstRadio(ref radioParams, out radioHandle);
            bool additionalRadios = false;

            do
            {
                if (radioHandle != IntPtr.Zero)
                    btRadios.Add(radioHandle);

                additionalRadios = NativeImports.BluetoothFindNextRadio(ref foundHandle, out radioHandle);
            } 
            while (additionalRadios);

            NativeImports.BluetoothFindRadioClose(foundHandle);

            if (btRadios.Count > 0)
            {
                _updateCallback?.Invoke(StatusUpdate.Searching, string.Empty);

                // Keep looking until cancelled or we've paired a device
                while (!_cancelToken.IsCancellationRequested && pairedCount == 0)
                {
                    for (int r = 0; r < btRadios.Count; ++r)
                    {
                        if (_cancelToken.IsCancellationRequested) break;

                        IntPtr found;
                        var radioInfo = new NativeImports.BLUETOOTH_RADIO_INFO();

                        radioInfo.Initialize();

                        uint getInfoError = NativeImports.BluetoothGetRadioInfo(btRadios[r], ref radioInfo);

                        // Success
                        if (getInfoError == 0)
                        {
                            var deviceInfo = new NativeImports.BLUETOOTH_DEVICE_INFO();
                            deviceInfo.Initialize();

                            var searchParams = new NativeImports.BLUETOOTH_DEVICE_SEARCH_PARAMS();
                            searchParams.Initialize();
                            searchParams.fReturnAuthenticated = false;
                            searchParams.fReturnRemembered = false;
                            searchParams.fReturnConnected = false;
                            searchParams.fReturnUnknown = true;
                            searchParams.fIssueInquiry = true;
                            searchParams.cTimeoutMultiplier = 2;
                            searchParams.hRadio = btRadios[r];

                            found = NativeImports.BluetoothFindFirstDevice(ref searchParams, ref deviceInfo);

                            // At least one Device Found
                            if (found != IntPtr.Zero)
                            {
                                do
                                {
                                    if (_cancelToken.IsCancellationRequested) break;

                                    // Nothing to do if it is already connected
                                    if (deviceInfo.fConnected)
                                    {
                                        continue;
                                    }

                                    var connectionType = _isDeviceSupported(deviceInfo.szName);

                                    if (connectionType != ConnectType.Unsupported)
                                    {
                                        _updateCallback(StatusUpdate.DeviceFound, deviceInfo.szName);

                                        // Disconnect if currently paired before re-pairing it
                                        if (deviceInfo.fRemembered)
                                        {
                                            _updateCallback?.Invoke(StatusUpdate.Unpairing, deviceInfo.szName);
                                            uint removeErr = NativeImports.BluetoothRemoveDevice(ref deviceInfo.Address);

                                            if (removeErr != 0)
                                            {
                                                _updateCallback?.Invoke(StatusUpdate.Error_Unpairing, $"{removeErr}");
                                                continue;
                                            }
                                        }

                                        if (connectionType == ConnectType.WiiLike)
                                        {
                                            // Build connection password
                                            StringBuilder password = new StringBuilder();
                                            var bytes = BitConverter.GetBytes(radioInfo.address);
                                            for (int i = 0; i < 6; ++i)
                                            {
                                                password.Append((char)bytes[i]);
                                            }

                                            _updateCallback?.Invoke(StatusUpdate.Pairing, string.Empty);
                                            var pairErr = NativeImports.BluetoothAuthenticateDevice(IntPtr.Zero, btRadios[r], ref deviceInfo, password.ToString(), 6);
                                            if (pairErr != 0)
                                            {
                                                _updateCallback?.Invoke(StatusUpdate.Error_Pairing, $"{pairErr}");
                                                continue;
                                            }

                                            _updateCallback?.Invoke(StatusUpdate.CheckingServices, string.Empty);
                                            uint pcService = 16;
                                            var guids = new Guid[16];
                                            var serviceErr = NativeImports.BluetoothEnumerateInstalledServices(btRadios[r], ref deviceInfo, ref pcService, guids);
                                            if (serviceErr != 0)
                                            {
                                                _updateCallback?.Invoke(StatusUpdate.Error_CheckingServices, $"{serviceErr}");
                                                continue;
                                            }

                                            _updateCallback?.Invoke(StatusUpdate.SettingService, string.Empty);
                                            var activateError = NativeImports.BluetoothSetServiceState(btRadios[r], ref deviceInfo, ref HIDServiceClass, 0x01);
                                            if (activateError != 0)
                                            {
                                                _updateCallback?.Invoke(StatusUpdate.Error_SettingService, $"{activateError}");
                                                continue;
                                            }

                                            _updateCallback?.Invoke(StatusUpdate.Success, deviceInfo.szName);
                                            pairedCount += 1;
                                        }
                                        else if (connectionType == ConnectType.Unauthenticated)
                                        {
                                            // Try without authentication
                                            var err = NativeImports.BluetoothAuthenticateDeviceEx(IntPtr.Zero, IntPtr.Zero, ref deviceInfo, null, NativeImports.AUTHENTICATION_REQUIREMENTS.MITMProtectionNotRequired);

                                            if (err == 0)
                                            {
                                                _updateCallback?.Invoke(StatusUpdate.Success, deviceInfo.szName);
                                                pairedCount += 1;
                                            }
                                            else
                                            {
                                                _updateCallback?.Invoke(StatusUpdate.Error_Pairing, $"{err}");

                                                // Try with Authentication code 0000
                                                _updateCallback?.Invoke(StatusUpdate.Pairing, $"Using 0000 {deviceInfo.szName}");
                                                err = NativeImports.BluetoothAuthenticateDevice(IntPtr.Zero, btRadios[r], ref deviceInfo, "0000", 4);

                                                if (err == 0)
                                                {
                                                    _updateCallback?.Invoke(StatusUpdate.Success, deviceInfo.szName);
                                                    pairedCount += 1;
                                                }
                                                else
                                                {
                                                    _updateCallback?.Invoke(StatusUpdate.Error_Pairing, $"{err}");

                                                    // Try with Authentication code 1234
                                                    _updateCallback?.Invoke(StatusUpdate.Pairing, $"Using 1234 {deviceInfo.szName}");
                                                    err = NativeImports.BluetoothAuthenticateDevice(IntPtr.Zero, btRadios[r], ref deviceInfo, "1234", 4);

                                                    if (err == 0)
                                                    {
                                                        _updateCallback?.Invoke(StatusUpdate.Success, deviceInfo.szName);
                                                        pairedCount += 1;
                                                    }
                                                    else
                                                    {
                                                        _updateCallback?.Invoke(StatusUpdate.Error_Pairing, $"{err}");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                while (NativeImports.BluetoothFindNextDevice(found, ref deviceInfo));
                            }
                        }
                        else
                        {
                            _updateCallback?.Invoke(StatusUpdate.Error_RadioInfo, $"{getInfoError}");
                        }
                    }
                }

                // Close Opened Radio Handles
                foreach (var openRadio in btRadios)
                {
                    NativeImports.CloseHandle(openRadio);
                }
            }
            else
            {
                _updateCallback?.Invoke(StatusUpdate.NoRadios, string.Empty);
            }

            new AsyncCallback(result => {
                _completeCallback?.Invoke();
            }).BeginInvoke(null, null, this);
        }

        public enum ConnectType : byte
        {
            Unsupported = 0,

            /// <summary>
            /// Connect w/o authentication
            /// </summary>
            Unauthenticated = 1,

            /// <summary>
            /// Connect using Bluetooth Radio's reversed MAC Address
            /// </summary>
            WiiLike = 2
        }

        public enum StatusUpdate : byte
        {
            Complete = 0,
            NoRadios,
            Searching,
            DeviceFound,
            Unpairing,
            Pairing,
            CheckingServices,
            SettingService,
            Success,

            Error_RadioInfo,
            Error_Unpairing,
            Error_Pairing,
            Error_CheckingServices,
            Error_SettingService
        }
    }
}
