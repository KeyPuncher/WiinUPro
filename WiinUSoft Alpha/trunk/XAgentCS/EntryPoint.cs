using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EasyHook;
// IPC
using XAgentCS.Interface;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.InteropServices;

namespace XAgentCS
{
    public class EntryPoint : EasyHook.IEntryPoint
    {
        #region Variables
        #region IPC Channel Variables
        private static InputInterface _interface;
        ClientInputInterfaceEventProxy _clientEventProxy = new ClientInputInterfaceEventProxy();
        IpcServerChannel _clientServerChannel = null;

        private System.Threading.ManualResetEvent _runWait;
        private static XControllers[] xDevices;
        #endregion

        #region Common Variables
        LocalHook HookGetState = null;
        LocalHook HookSetState = null;
        LocalHook HookGetCapabilities = null;
        LocalHook HookEnabled = null;
        LocalHook HookGetDSoundAudioDeviceGuids = null;
        LocalHook HookGetBatteryInformation = null;

        static bool[] enabledControllers = { true, true, true, true };
        #endregion
        #endregion

        public EntryPoint(RemoteHooking.IContext context, String channelName)
        {
            #region IPC Communication
            // create some controllers
            xDevices = new XControllers[4];
            xDevices[0] = new XControllers();
            xDevices[1] = new XControllers();
            xDevices[2] = new XControllers();
            xDevices[3] = new XControllers();

            // reference to IPC to host app
            // Methods & events ran against this will be executed in the host process
            _interface = RemoteHooking.IpcConnectClient<InputInterface>(channelName);

            // Ping to check connection
            _interface.Ping();

            // Setup Bi-Directional IPC
            System.Collections.IDictionary properties = new System.Collections.Hashtable();
            properties["name"] = channelName;
            properties["portName"] = channelName + Guid.NewGuid().ToString("N");

            System.Runtime.Remoting.Channels.BinaryServerFormatterSinkProvider binaryProvider = new System.Runtime.Remoting.Channels.BinaryServerFormatterSinkProvider();
            binaryProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            IpcServerChannel _clientServerChannel = new IpcServerChannel(properties, binaryProvider);
            System.Runtime.Remoting.Channels.ChannelServices.RegisterChannel(_clientServerChannel, false);
            #endregion
        }

        public void Run (RemoteHooking.IContext context, String channelName)
        {
            // inform server of successful injection
            _interface.Message("Injected into process ID: " + RemoteHooking.GetCurrentProcessId().ToString());

            // choices, xinput1_4.dll, xinput1_3.dll, XInput9_1_0.dll, xinput1_2.dll, xinput1_1.dll
            string xinputLibrary = "XInput1_3.dll";
            _interface.Message("Using DLL: " + xinputLibrary);

            // use a manual reset event to act as a flip switch to exit the Run event
            _runWait = new System.Threading.ManualResetEvent(false);
            _runWait.Reset();
            try
            {
                // TODO: Apply more Hooks

                #region Hooks
                IntPtr handle = NativeAPI.LoadLibrary("C:\\Windows\\system32\\" + xinputLibrary);
                // For reporting the button, trigger, and joystick states
                HookGetState = LocalHook.Create(LocalHook.GetProcAddress(xinputLibrary, "XInputGetState"), new XGetState(GetState_Hooked), this);
                HookGetState.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                // For getting force feedback
                HookSetState = LocalHook.Create(LocalHook.GetProcAddress(xinputLibrary, "XInputSetState"), new XSetState(SetState_Hooked), this);
                HookSetState.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                // For reporting controller's capabilities
                HookGetCapabilities = LocalHook.Create(LocalHook.GetProcAddress(xinputLibrary, "XInputGetCapabilities"), new XGetCapabilities(GetCapabilities_Hooked), this);
                HookGetCapabilities.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                // Indicates if XInput is enabled or disabled
                HookEnabled = LocalHook.Create(LocalHook.GetProcAddress(xinputLibrary, "XInputEnable"), new XEnabled(Enabled_Hooked), this);
                HookEnabled.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                // Get sound location hook
                HookGetDSoundAudioDeviceGuids = LocalHook.Create(LocalHook.GetProcAddress(xinputLibrary, "XInputGetDSoundAudioDeviceGuids"), new XGetDSoundAudioDeviceGuids(GetDSoundAudioDeviceGuids_Hooked), this);
                HookGetDSoundAudioDeviceGuids.ThreadACL.SetExclusiveACL(new Int32[] { 0 });

                // Battery Info Hook
                HookGetBatteryInformation = LocalHook.Create(LocalHook.GetProcAddress(xinputLibrary, "XInputGetBatteryInformation"), new XGetBatteryInformation(GetBatteryInformation_Hooked), this);
                HookGetBatteryInformation.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
                #endregion

                #region Interface Event Handlers
                _interface.StateChanged += _clientEventProxy.StateChangedProxyHandler;
                _interface.Disconnected += _clientEventProxy.DisconnectedProxyHandler;
                _interface.ControllerConnected += _clientEventProxy.ControllerConnectedProxyHandler;

                _clientEventProxy.StateChanged += (StateChangedEventArgs args) =>
                    {
                        _interface_StateChanged(args);
                    };

                _clientEventProxy.ControllerConnected += (ControllersConnectedEventArgs args) =>
                    {
                        _interface_ControllersConnected(args);
                    };

                // Use a different thread when accessing _interface from _clientEventProxy to avoid Deadlocks
                _clientEventProxy.Disconnected += () =>
                    {
                        // Allows the ability to flip the switch to exit Run()
                        _runWait.Set();
                    };
                #endregion

                // Start checking if we still have server communication
                StartServerCheck();

                // Wait until the switch is flipped
                _runWait.WaitOne();

                // Stop checking now that we are exiting Run()
                StopServerCheck();

// TODO: Remove Hooks
            }
            catch (Exception e)
            {
                _interface.Message("Error occured in Run(): " + e.ToString());
                _interface.Message("Error Message: " + e.Message);
                _interface.Message("Error Stack Trace: " + e.StackTrace);
            }
            finally
            {
                try { _interface.Message("Disconnectiog from process ID: " + RemoteHooking.GetCurrentProcessId()); }
                catch { }

                // Remove server channel
                System.Runtime.Remoting.Channels.ChannelServices.UnregisterChannel(_clientServerChannel);

                // sleep to wait for remaining messages to be recieved by the server
                System.Threading.Thread.Sleep(100);
            }

        }

        #region Hook Delegates
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate UInt32 XGetState(UInt32 dwUserIndex, ref XInputState pState);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate UInt32 XSetState(UInt32 dwUserIndex, ref XInputVibration pVibration);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate UInt32 XGetCapabilities(UInt32 dwUserIndex, UInt32 dwFlags, ref XInputCapabilities pCapabilities);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate void XEnabled(bool enable);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate UInt32 XGetDSoundAudioDeviceGuids(UInt32 dwUserIndex, ref Guid pDSoundRenderGuid, ref Guid pDSoundCaptureGuid);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        delegate UInt32 XGetBatteryInformation(UInt32 dwUserIndex, Byte devType, ref XInputBatteryInformation pBatteryInformation);
        #endregion

        #region XInput DLL Imports
        [DllImport("xinput1_3.dll")]
        public static extern UInt32 XInputGetState(UInt32 dwUserIndex, ref XInputState pState);

        [DllImport("xinput1_3.dll")]
        public static extern UInt32 XInputSetState(UInt32 dwUserIndex, ref XInputVibration pVibration);

        [DllImport("xinput1_3.dll")]
        public static extern UInt32 XInputGetCapabilities(UInt32 dwUserIndex, UInt32 dwFlags, ref XInputCapabilities pCapabilities);

        [DllImport("xinput1_3.dll")]
        public static extern void XInputEnable(bool enable);
        #endregion

        #region Hooked Functions
        static UInt32 GetState_Hooked(UInt32 dwUserIndex, ref XInputState pState)
        {
            // TODO: Substitute with (ushort) xValue.
            pState.Gamepad.wButtons = (ushort)
            (
                (xDevices[dwUserIndex].A      ? (ushort)xValue.A      : 0) + 
                (xDevices[dwUserIndex].B      ? (ushort)xValue.B      : 0) +
                (xDevices[dwUserIndex].X      ? (ushort)xValue.X      : 0) +
                (xDevices[dwUserIndex].Y      ? (ushort)xValue.Y      : 0) +
                (xDevices[dwUserIndex].Up     ? (ushort)xValue.Up     : 0) +
                (xDevices[dwUserIndex].Down   ? (ushort)xValue.Down   : 0) +
                (xDevices[dwUserIndex].Left   ? (ushort)xValue.Left   : 0) +
                (xDevices[dwUserIndex].Right  ? (ushort)xValue.Right  : 0) +
                (xDevices[dwUserIndex].Start  ? (ushort)xValue.Start  : 0) +
                (xDevices[dwUserIndex].Select ? (ushort)xValue.Select : 0) +
                (xDevices[dwUserIndex].LS     ? (ushort)xValue.LS     : 0) +
                (xDevices[dwUserIndex].RS     ? (ushort)xValue.RS     : 0) +
                (xDevices[dwUserIndex].L      ? (ushort)xValue.L      : 0) +
                (xDevices[dwUserIndex].R      ? (ushort)xValue.R      : 0)
            );

            pState.Gamepad.bLeftTrigger = xDevices[dwUserIndex].LT;
            pState.Gamepad.bRightTrigger = xDevices[dwUserIndex].RT;

            pState.Gamepad.sThumbLX = xDevices[dwUserIndex].LX;
            pState.Gamepad.sThumbLY = xDevices[dwUserIndex].LY;
            pState.Gamepad.sThumbRX = xDevices[dwUserIndex].RX;
            pState.Gamepad.sThumbRY = xDevices[dwUserIndex].RY;

            return 0;
        }

        static UInt32 SetState_Hooked(UInt32 dwUserIndex, ref XInputVibration pVibration)
        {
            if (dwUserIndex >= enabledControllers.Length || !enabledControllers[dwUserIndex] || dwUserIndex < 0) 
                return 1167;

            if (pVibration.wLeftMotorSpeed + pVibration.wRightMotorSpeed > 5000)
            {
                _interface.SetRumble((int)dwUserIndex, true);
            }
            else if (xDevices[dwUserIndex].rumble)
            {
                _interface.SetRumble((int)dwUserIndex, false);
            }

            return 0;
        }

        static UInt32 GetCapabilities_Hooked(UInt32 dwUserIndex, UInt32 dwFlags, ref XInputCapabilities pCapabilities)
        {
            if (enabledControllers == null || enabledControllers.Length < 4)
                return 0;

            if (dwUserIndex > enabledControllers.Length)
                return 1167;

            if (enabledControllers[dwUserIndex])
            {
                pCapabilities.Flags                      = (ushort)0x0004;
                pCapabilities.type                       = (Byte)0x01;
                pCapabilities.subType                    = (Byte)0x01;

                pCapabilities.Gamepad.wButtons           = 0xF3FF;
                pCapabilities.Gamepad.bLeftTrigger       = 0xFF;
                pCapabilities.Gamepad.bRightTrigger      = 0xFF;

                //pCapabilities.Gamepad.sThumbLX           = 0xFF;
                //pCapabilities.Gamepad.sThumbLY           = 0xFF;
                //pCapabilities.Gamepad.sThumbRX           = 0xFF;
                //pCapabilities.Gamepad.sThumbRY           = 0xFF;
                
                pCapabilities.Gamepad.sThumbLX =
                pCapabilities.Gamepad.sThumbLY =
                pCapabilities.Gamepad.sThumbRX =
                pCapabilities.Gamepad.sThumbRY = (short)BitConverter.ToInt32(new byte[] { 0xC0, 0xFF }, 0);

                pCapabilities.Vibration.wLeftMotorSpeed  = 0xFF;
                pCapabilities.Vibration.wRightMotorSpeed = 0xFF;

                return 0;
            }
            else
                return 1167;
        }

        static void Enabled_Hooked(bool enable)
        {
            XInputEnable(true);
        }

        static UInt32 GetDSoundAudioDeviceGuids_Hooked(UInt32 dwUserIndex, ref Guid pDSoundRenderGuid, ref Guid pDSoundCaptureGuid)
        {
            if (enabledControllers == null || enabledControllers.Length < 4)
                return 0;

            if (dwUserIndex > enabledControllers.Length)
                return 1167;

            if (enabledControllers[dwUserIndex])
            {
                pDSoundRenderGuid = new Guid();
                pDSoundCaptureGuid = new Guid();

                return 0;
            }
            else
                return 1167;
        }

        static UInt32 GetBatteryInformation_Hooked(UInt32 dwUserIndex, Byte devType, ref XInputBatteryInformation pBatteryInformation)
        {
            if (enabledControllers == null || enabledControllers.Length < 4)
                return 0;

            if (dwUserIndex > enabledControllers.Length)
                return 1167;

            if (enabledControllers[dwUserIndex])
            {
                pBatteryInformation.BatteryType = 0x01;     // Wired
                pBatteryInformation.BatteryLevel = 0x03;    // Full

                return 0;
            }
            else
                return 1167;
        }
        #endregion

        #region IPC - Events from Server
        void _interface_StateChanged(StateChangedEventArgs args)
        {
            xDevices[args.Player] = args.State;
        }

        void _interface_ControllersConnected(ControllersConnectedEventArgs args)
        {
            if (enabledControllers == null || enabledControllers.Length < 4)
                return;

            enabledControllers[0] = args.P1Connected;
            enabledControllers[1] = args.P2Connected;
            enabledControllers[2] = args.P3Connected;
            enabledControllers[3] = args.P4Connected;
        }
        #endregion

        #region IPC - Check for server communicatoin
        Task _checkServer;
        long _stopCheck = 0;

        private void StartServerCheck()
        {
            _checkServer = new Task(() =>
            {
                try
                {
                    while (System.Threading.Interlocked.Read(ref _stopCheck) == 0)
                    {
                        // check every second
                        System.Threading.Thread.Sleep(1000);

                        // this will throw an exception if unavailable
                        _interface.Ping();
                    }
                }
                catch
                {
                    // Flip the switch
                    _runWait.Set();
                }
            });

            // Starts the seperate thread task we just wrote above
            _checkServer.Start();
        }

        private void StopServerCheck()
        {
            System.Threading.Interlocked.Increment(ref _stopCheck);
        }
        #endregion
    }
}
