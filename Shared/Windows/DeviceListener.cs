using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Shared.Windows
{
    public class DeviceListener
    {
        #region Class Constants
        // https://msdn.microsoft.com/en-us/library/aa363480(v=vs.85).aspx
        private const int DbtDeviceArrival        = 0x8000;  // system detected a new device        
        private const int DbtDeviceRemoveComplete = 0x8004;  // device is gone     
        private const int DbtDevNodesChanged      = 0x0007;  // A device has been added to or removed from the system.
  
        private const int DbtDevtypDeviceinterface = 5;
        private const int WmDevicechange = 0x0219;          // device change event message

        // https://msdn.microsoft.com/en-us/library/aa363431(v=vs.85).aspx
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;

        // https://msdn.microsoft.com/en-us/library/windows/hardware/ff553426(v=vs.85).aspx
        public static readonly Guid GuidInterfaceUSB = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
        public static readonly Guid GuidInterfaceHID = new Guid("745A17A0-74D3-11D0-B6FE-00A0C90f57DA");
        public static readonly Guid GuidInterfaceBT  = new Guid("E0CBF06C-CD8B-4647-BB8A-263B43F0F974");
        #endregion

        public static DeviceListener Instance { get; private set; }

        public event Action OnDevicesUpdated;

        private IntPtr notificationHandle;
        
        static DeviceListener()
        {
            Instance = new DeviceListener();
        }

        private DeviceListener() { }
        
        public void RegisterDeviceNotification(Window window, Guid deviceClass, bool usbOnly = false)
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
            source.AddHook(HwndHandler);

            IntPtr windowHandle = source.Handle;

            var deviceInterface = new DevBroadcastDeviceInterface
            {
                DeviceType = DbtDevtypDeviceinterface,
                Reserved = 0,
                ClassGuid = deviceClass,
                Name = 0
            };

            deviceInterface.Size = Marshal.SizeOf(deviceInterface);
            IntPtr buffer = Marshal.AllocHGlobal(deviceInterface.Size);
            Marshal.StructureToPtr(deviceInterface, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, usbOnly ? 0 : DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
        }
        
        public void UnregisterDeviceNotification()
        {
            UnregisterDeviceNotification(notificationHandle);
        }

        private IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            // Only checking changed event since it gets called when devices are added and removed
            // while remove notifications don't always get called.
            if (msg == WmDevicechange && (int)wparam == DbtDevNodesChanged)
            {
                OnDevicesUpdated?.Invoke();
            }

            handled = false;
            return IntPtr.Zero;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

        [DllImport("user32.dll")]
        private static extern bool UnregisterDeviceNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential)]
        private struct DevBroadcastDeviceInterface
        {
            internal int Size;
            internal int DeviceType;
            internal int Reserved;
            internal Guid ClassGuid;
            internal short Name;
        }
    }
}
