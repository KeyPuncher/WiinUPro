using System;
using Microsoft.Win32.SafeHandles;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;

/// TODO: 
/// - Initializing Motion Plus Attatchemnt
/// - NOTE: WiimotePlus may Handle motion plus a little differently
/// - Enable/Disable IR Sensor
/// - Continous Reports
/// - Allow reading calibration for balance board


namespace NintrollerLib
{
    /// <summary>
    /// For creating connections and communicating with controllers.
    /// </summary>
    public class Nintroller : IDisposable
    {
        #region Members
        /// <summary>
        /// Fired evertime a data report is recieved.
        /// </summary>
        public event EventHandler<StateChangeEventArgs> StateChange;
        /// <summary>
        /// Fired when the controller's extension changes.
        /// </summary>
        public event EventHandler<ExtensionChangeEventArgs> ExtensionChange;
        /// <summary>
        /// If the controller is open to communication.
        /// </summary>
        public bool Connected { get { return connected; } }
        /// <summary>
        /// The controller's current state.
        /// </summary>
        public NintyState State { get { return mDeviceState; } }
        /// <summary>
        /// The HID path of the controller.
        /// </summary>
        public string HIDPath { get { return mDevicePath; } }
        /// <summary>
        /// The ID of instance of the controller.
        /// </summary>
        public Guid ID { get { return mID; } }

        private ControllerType currentType = ControllerType.Wiimote;
        private bool connected;
        
        // Read/Writing Variables
        private SafeFileHandle  mHandle;    // Handle for Reading and Writing
        private FileStream      mStream;    // Read and Write Stream
        private byte[]          mReadBuff;  // Read Buffer
        private int             mAddress;   // Address to read
        private short           mSize;      // read request size

        // Currently unused
        private bool            mAltWrite;  // Alternative Report Writing
        private short           mPID;       // Product ID for this device
        private bool            mDoNotRead; // Flag to avoid trying to read from the device
        private bool            mParsing;   // prevent double parse

        // The HID device path
        private string mDevicePath = string.Empty;
        private NintyState mDeviceState;

        // Calibration Variables - Probably won't need these
        private WiimoteCalibration              mCalibrationWiimote;
        private MotionPlusCalibration           mCalibrationMotionPlus;
        private NunchuckCalibration             mCalibrationNunchuck;
        private ClassicControllerCalibration    mCalibrationClassic;
        private ClassicControllerProCalibration mCalibrationClassicPro;
        private ProCalibration                  mCalibrationPro;

        // Read Only Variables
        private readonly AutoResetEvent mReadDone = new AutoResetEvent(false);
        private readonly AutoResetEvent mWriteDone = new AutoResetEvent(false);
        private readonly AutoResetEvent mStatusDone = new AutoResetEvent(false);
        private readonly Guid mID = Guid.NewGuid();

        // delegates
        #endregion

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// (Not ready to be connected to)
        /// </summary>
        public Nintroller() { mDeviceState = new NintyState(); }

        /// <summary>
        /// Creates a controller with it's known location.
        /// (Ideally connection ready)
        /// </summary>
        /// <param name="devicePath">The HID Path</param>
        public Nintroller(string devicePath)
        {
            mDeviceState = new WiimoteState();
            mDevicePath = devicePath;
        }
        #endregion

        #region IDispose Methods
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Disconnect();
        }
        #endregion

        /// <summary>
        /// Retreives the location of all recognized controllers.
        /// </summary>
        /// <returns>List of the HID Paths to recognized controllers.</returns>
        public static List<string> FindControllers()
        {
            List<string> ControllerList = new List<string>();
            int index = 0;
            Guid guid;
            SafeFileHandle mHandle;

            // Get the GUID of HID class
            HIDImports.HidD_GetHidGuid(out guid);

            // Handle to all HID devices
            IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, HIDImports.DIGCF_DEVICEINTERFACE);

            HIDImports.SP_DEVICE_INTERFACE_DATA diData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = Marshal.SizeOf(diData);
            
            // Look Through All Devices
            while (HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref diData))
            {
                UInt32 size;

                // get buffer size for the device
                HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out size, IntPtr.Zero);

                // create detail struct
                HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();

                diDetail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);

                // get the detail struct
                if (HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, IntPtr.Zero))
                {
        //            Debug.WriteLine(string.Format("{0}: {1} - {2}", index, diDetail.DevicePath, Marshal.GetLastWin32Error()));

                    // open read/write handle for device
                    mHandle = HIDImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

                    // create attributes structure
                    HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
                    attrib.Size = Marshal.SizeOf(attrib);

                    // get attributes
                    if (HIDImports.HidD_GetAttributes(mHandle.DangerousGetHandle(), ref attrib))
                    {
                        if (attrib.VendorID == Constants.VID && (attrib.ProductID == Constants.PID1 || attrib.ProductID == Constants.PID2))
                        {
                            ControllerList.Add(diDetail.DevicePath);
                        }
                    }

                    mHandle.Close();
                }
                else
                {
                    Debug.WriteLine("Failed to get info on this device.\n");
                }

                index++;
            }

            // clean list
            HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);

            Debug.WriteLine("Total Devices: " + ControllerList.Count.ToString());

            return ControllerList;
        }

        /// <summary>
        /// Retreives the location of all the unrecognized devices.
        /// </summary>
        /// <returns></returns>
        public static List<string> FindOtherDevices()
        {
            List<string> ControllerList = new List<string>();
            int index = 0;
            Guid guid;
            SafeFileHandle mHandle;

            // Get the GUID of HID class
            HIDImports.HidD_GetHidGuid(out guid);

            // Handle to all HID devices
            IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, HIDImports.DIGCF_DEVICEINTERFACE);

            HIDImports.SP_DEVICE_INTERFACE_DATA diData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = Marshal.SizeOf(diData);

            // Look Through All Devices
            while (HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref diData))
            {
                UInt32 size;

                // get buffer size for the device
                HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out size, IntPtr.Zero);

                // create detail struct
                HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();

                diDetail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);

                // get the detail struct
                if (HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, IntPtr.Zero))
                {
                    //            Debug.WriteLine(string.Format("{0}: {1} - {2}", index, diDetail.DevicePath, Marshal.GetLastWin32Error()));

                    // open read/write handle for device
                    mHandle = HIDImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

                    // create attributes structure
                    HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
                    attrib.Size = Marshal.SizeOf(attrib);

                    // get attributes
                    if (HIDImports.HidD_GetAttributes(mHandle.DangerousGetHandle(), ref attrib))
                    {
                        if (attrib.VendorID != Constants.VID && attrib.ProductID != Constants.PID1 && attrib.ProductID != Constants.PID2)
                        {
                            ControllerList.Add(diDetail.DevicePath);
                        }
                    }

                    mHandle.Close();
                }
                else
                {
                    Debug.WriteLine("Failed to get info on this device.\n");
                }

                index++;
            }

            // clean list
            HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);

            Debug.WriteLine("Total Unrecognized Devices: " + ControllerList.Count.ToString());

            return ControllerList;
        }

        #region Connectivity
        /// <summary>
        /// Test to see if the controller is able to be connected to.
        /// </summary>
        /// <returns>Controller able to connect.</returns>
        public bool ConnectTest()
        {
            bool retunValue = true;

            if (string.IsNullOrEmpty(mDevicePath))
                return false;

            mHandle = HIDImports.CreateFile(mDevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);
            mStream = new FileStream(mHandle, FileAccess.ReadWrite, Constants.REPORT_LENGTH, true);

            try
            {
                if (mStream != null && mStream.CanRead)
                {
                    byte[] bArray = new byte[Constants.REPORT_LENGTH];
                    mStream.BeginRead(bArray, 0, Constants.REPORT_LENGTH, new AsyncCallback(AsyncOnce), bArray);
                }


                byte[] buff = new byte[Constants.REPORT_LENGTH];

                buff[0] = (byte)OutputReport.StatusRequest;
                buff[1] = 0x00;

                mStream.Write(buff, 0, Constants.REPORT_LENGTH);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                retunValue = false;
                //HIDImports.HidD_SetOutputReport(this.mHandle.DangerousGetHandle(), buff, (uint)buff.Length);
            }

            if (retunValue && !mStatusDone.WaitOne(500, false))
                retunValue = false;

            Disconnect();

            return retunValue;
        }

        /// <summary>
        /// Opens a controller connection for communication.
        /// </summary>
        /// <returns>Successful connection.</returns>
        public bool Connect()
        {
            if (string.IsNullOrEmpty(mDevicePath))
                return false;

            // Open Read/Write Handle
            mHandle = HIDImports.CreateFile(mDevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

            // create File Stream
            mStream = new FileStream(mHandle, FileAccess.ReadWrite, Constants.REPORT_LENGTH, true);

            // begin read?
            AsyncRead();

            // get calibration? Trying to calibrate a Pro Controller or a newer device will disconnect it!
            //GetCalibration();

            // get status?
            GetStatus();

            connected = true;
            return true;
        }

        /// <summary>
        /// Disconnects from the controller.
        /// </summary>
        public void Disconnect()
        {
            if (mStream != null)
                mStream.Close();

            if (mHandle != null)
                mHandle.Close();

            connected = false;

            Debug.WriteLine("Disconnected");
        }
        #endregion

        #region Data Reading

        // Initiate Asynchronous Reading
        private void AsyncRead()
        {
            if (mStream != null && mStream.CanRead)
            {
                byte[] bArray = new byte[Constants.REPORT_LENGTH];
                mStream.BeginRead(bArray, 0, Constants.REPORT_LENGTH, new AsyncCallback(AsyncData), bArray);
            }
        }

        // Data has been read from the device
        private void AsyncData(IAsyncResult data)
        {
            byte[] result = (byte[])data.AsyncState;

            try
            {
                mStream.EndRead(data);

                if (ParseReport(result))
                {
                    if (StateChange != null)
                        StateChange(this, new StateChangeEventArgs(mDeviceState));
                }

                AsyncRead();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Async Read Was Cancelled");
            }
        }

        // For doing the Connection Test
        private void AsyncOnce(IAsyncResult data)
        {
            try
            {
                mStream.EndRead(data);
                mStatusDone.Set();
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Async Read Was Cancelled");
            }
        }

        // Read Data From Device
        private byte[] ReadData(int address, short size)
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            mReadBuff = new byte[size];
            mAddress = address & 0xffff;
            mSize = size;

            buffer[0] = (byte)OutputReport.ReadMemory;
            buffer[1] = (byte)(((address & 0xff000000) >> 24) | (byte)(mDeviceState.GetRumble() ? 0x01 : 0x00));
            buffer[2] = (byte)((address & 0x00ff0000) >> 16);
            buffer[3] = (byte)((address & 0x0000ff00) >> 8);
            buffer[4] = (byte)(address & 0x000000ff);
            buffer[5] = (byte)((size & 0xff00) >> 8);
            buffer[6] = (byte)(size & 0xff);

            WriteReport(buffer);

            if (!mReadDone.WaitOne(1000, false))
            {
                Debug.WriteLine("Failed to Wait during Read");
                //throw new Exception("Error Reading Data");
            }

            return mReadBuff;
        }

        // get calibration from the controller
        private void GetCalibration()
        {
            byte[] buff = ReadData(0x0016, 7);
        }

        private void GetStatus()
        {
            byte[] buff = new byte[Constants.REPORT_LENGTH];

            buff[0] = (byte)OutputReport.StatusRequest;
            buff[1] = (byte)(mDeviceState.GetRumble() ? 0x01 : 0x00);

            WriteReport(buff);

            if (!mStatusDone.WaitOne(3000, false))
                Debug.WriteLine("Timeout for status report");
        }

        /// <summary>
        /// Refresh information such as the battery level and extensions.
        /// </summary>
        public void RefreshStatus()
        {
            GetStatus();
        }

        /// <summary>
        /// Sets the device's report type.
        /// </summary>
        /// <param name="reportType">Report to switch to.</param>
        public void ChangeReport(InputReport reportType)
        {
            SetReport(reportType);
        }

        private void SetReport(InputReport reportType)
        {
            byte[] buff = new byte[Constants.REPORT_LENGTH];

            buff[0] = (byte)OutputReport.DataReportMode;
            buff[1] = (byte)(0x00 | (byte)(mDeviceState.GetRumble() ? 0x01 : 0x00)); // 0x40 for continous
            buff[2] = (byte)reportType;

            WriteReport(buff);
        }
        #endregion

        #region Data Writing
        private void WriteReport(byte[] report)
        {
            Debug.WriteLine("Writing a Report for: " + Enum.Parse(typeof(OutputReport), report[0].ToString()));

            if (mStream != null)
            {
                try
                {
                    mStream.Write(report, 0, Constants.REPORT_LENGTH);
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                    HIDImports.HidD_SetOutputReport(this.mHandle.DangerousGetHandle(), report, (uint)report.Length);
                }
            }
            else
                HIDImports.HidD_SetOutputReport(this.mHandle.DangerousGetHandle(), report, (uint)report.Length);

            /*if (mAltWrite)
                HIDImports.HidD_SetOutputReport(this.mHandle.DangerousGetHandle(), report, (uint)report.Length);
            else if (mStream != null)
                mStream.Write(report, 0, Constants.REPORT_LENGTH);*/

            if (report[0] == (byte)OutputReport.WriteMemory)
            {
                if (!mWriteDone.WaitOne(1000, false))
                    Debug.WriteLine("Failed to Wait during Writing");
            }
        }

        private void WriteByte(int address, byte data)
        {
            WriteBytes(address, 1, new byte[] { data });
        }

        private void WriteBytes(int address, byte size, byte[] data)
        {
            byte[] bytes = new byte[Constants.REPORT_LENGTH];

            bytes[0] = (byte)OutputReport.WriteMemory;
            bytes[1] = (byte)(((address & 0xff000000) >> 24) | (byte)(mDeviceState.GetRumble() ? 0x01 : 0x00));
            bytes[2] = (byte)((address & 0x00ff0000) >> 16);
            bytes[3] = (byte)((address & 0x0000ff00) >> 8);
            bytes[4] = (byte)(address & 0x000000ff);
            bytes[5] = size;
            Array.Copy(data, 0, bytes, 6, size);

            WriteReport(bytes);
        }

        private void WriteLEDs()
        {
            byte[] ledReport = new byte[Constants.REPORT_LENGTH];

            ledReport[0] = (byte)OutputReport.LEDs;
            ledReport[1] = (byte)(
                            (mDeviceState.led1 ? 0x10 : 0x00) |
                            (mDeviceState.led2 ? 0x20 : 0x00) |
                            (mDeviceState.led3 ? 0x40 : 0x00) |
                            (mDeviceState.led4 ? 0x80 : 0x00) |
                            (mDeviceState.GetRumble() ? 0x01 : 0x00));

            WriteReport(ledReport);
        }
        #endregion

        #region Data Parsing
        private bool ParseReport(byte[] report)
        {
            InputReport input = (InputReport)report[0];
//            Debug.WriteLine(BitConverter.ToString(report));
            switch (input)
            {
                // Any of the following reports can be parsed by the controller
                case InputReport.BtnsOnly:
                case InputReport.BtnsAcc:
                case InputReport.BtnsExt:
                case InputReport.BtnsAccIR:
                case InputReport.BtnsExtB:
                case InputReport.BtnsAccExt:
                case InputReport.BtnsIRExt:
                case InputReport.BtnsAccIRExt:
                case InputReport.ExtOnly:
                    mDeviceState.ParseReport(report);
                    break;
                #region Data Reports OLD
                /*case InputReport.BtnsOnly:
                    Debug.WriteLine("Core Buttons");
                    mDeviceState.ParseReport(report);
                    break;

                case InputReport.BtnsAcc:
                    Debug.WriteLine("Core Buttons and Accelerometer");
                    break;

                case InputReport.BtnsExt:
                    Debug.WriteLine("Core Buttons with 8 Extension bytes");
                    break;

                case InputReport.BtnsAccIR:
                    Debug.WriteLine("Core Buttons and Accelerometer with 12 IR bytes");
                    break;

                case InputReport.BtnsExtB:
                    Debug.WriteLine("Core buttons with 19 Extension Bytes");
                    break;

                case InputReport.BtnsAccExt:
                    Debug.WriteLine("Core Buttons and Acceleromter with 16 Extension Bytes");
                    break;

                case InputReport.BtnsIRExt:
                    Debug.WriteLine("Core Buttons with 10 IR bytes and 9 Extension Bytes");
                    break;

                case InputReport.BtnsAccIRExt:
                    Debug.WriteLine("Core Buttons and Accelerometer with 10 IR bytes and 6 Extension Bytes");
                    break;

                case InputReport.ExtOnly:
  //                  Debug.WriteLine("21 Extension Bytes");
                    mDeviceState.ParseReport(report);
                    break;*/
                #endregion

                // After we Identify the controller, we can change the input report type.
                #region Status Reports
                case InputReport.Status:
                    Debug.WriteLine("Status Report");
                    /// Parse Buttons? (maybe not)

                    // Get Extension
                    AsyncRead();
                    byte[] extensionType = ReadData(Constants.REGISTER_EXTENSION_TYPE_2, 1);
                    Debug.WriteLine("Extenstion Bytes: " + extensionType[0].ToString("x2"));

                    bool extensionConnected = (report[3] & 0x02) != 0;
                    Debug.WriteLine("Extension Connected: " + extensionConnected.ToString());

                    /// TODO: Account for Wiimote Plus controllers
                    if (extensionConnected)
                    {
                        // Initialize the extension
                        if(extensionType[0] != 0x04)
                        {
                            AsyncRead();
                            WriteByte(Constants.REGISTER_EXTENSION_INIT_1, 0x55);
                            WriteByte(Constants.REGISTER_EXTENSION_INIT_2, 0x00);
                        }
                    

                        AsyncRead();
                        byte[] ext = ReadData(Constants.REGISTER_EXTENSION_TYPE, 6);
                        long type = ((long)ext[0] << 40) | ((long)ext[1] << 32) | ((long)ext[2]) << 24 | ((long)ext[3]) << 16 | ((long)ext[4]) << 8 | ext[5];

                        Debug.WriteLine((ControllerType)type);

                        if (currentType != (ControllerType)type)
                        {
                            currentType = (ControllerType)type;

                            switch ((ControllerType)type)
                            {
                                case ControllerType.ProController:
                                    mDeviceState = new ProControllerState();
                                    break;
                                case ControllerType.BalanceBoard:
                                    mDeviceState = new BalanceBoardState();
                                    break;
                                case ControllerType.Nunchuk:
                                case ControllerType.NunchukB:
                                case ControllerType.ClassicController:
                                case ControllerType.ClassicControllerPro:
                                case ControllerType.MotionPlus:
                                    ((WiimoteState)mDeviceState).SetExtension(currentType);
                                   break;
                                ///TODO: Musicals
                                case ControllerType.PartiallyInserted:
                                    GetStatus();
                                    break;
                            }

                            if (ExtensionChange != null)
                                ExtensionChange(this, new ExtensionChangeEventArgs(mID, currentType));
                        }
                    }
                    else if (currentType != ControllerType.Wiimote)
                    {
                        // Remove the extension
                        ((WiimoteState)mDeviceState).extension = new ExtensionState();
                        currentType = ControllerType.Wiimote;
                        ((WiimoteState)mDeviceState).SetExtension(currentType);

                        if (ExtensionChange != null)
                            ExtensionChange(this, new ExtensionChangeEventArgs(mID, currentType));
                   }

                    // Battery Level
                    mDeviceState.batteryRaw = report[6];
                    mDeviceState.lowBattery = (report[3] & 0x01) != 0;
                    mDeviceState.UpdateBattery();

                    // Get LEDs
                    mDeviceState.led1 = (report[3] & 0x10) != 0;
                    mDeviceState.led2 = (report[3] & 0x20) != 0;
                    mDeviceState.led3 = (report[3] & 0x40) != 0;
                    mDeviceState.led4 = (report[3] & 0x80) != 0;

                    // Get & Set the report type
                    SetReport(mDeviceState.GetReportType());

                    mStatusDone.Set();
                    break;

                case InputReport.ReadMem:
                    Debug.WriteLine("Read Memory");
                    // Parse Buttons
                    ParseRead(report);
                    break;

                case InputReport.Acknowledge:
                    Debug.WriteLine("Acknowledge Report");
                    mWriteDone.Set();
                    break;
                #endregion
                default:
                    Debug.WriteLine("Couldn't Parse Report type: " + input.ToString("x"));
                    return false;
            }

            return true;
        }

        private void ParseRead(byte[] r)
        {
            if ((r[3] & 0x08) != 0)
                throw new Exception("Can't read bytes (Don't Exist?)");

            if ((r[3] & 0x07) != 0)
            {
                Debug.WriteLine("Trying to Read in Write-Only mode");
                mReadDone.Set();
                return;
            }

            int size    = (r[3] >> 4) + 1;
            int offset  = (r[4] << 8 | r[5]);

            Array.Copy(r, 6, mReadBuff, offset - mAddress, size);

            if (mAddress + mSize == offset + size)
                mReadDone.Set();
        }
        #endregion

        #region Settings
        /// <summary>
        /// Sets each LED individually.
        /// </summary>
        /// <param name="LED1">Leftmost LED</param>
        /// <param name="LED2">Center left LED</param>
        /// <param name="LED3">Center right LED</param>
        /// <param name="LED4">Rightmost LED</param>
        public void SetLEDs(bool LED1, bool LED2, bool LED3, bool LED4)
        {
            mDeviceState.led1 = LED1;
            mDeviceState.led2 = LED2;
            mDeviceState.led3 = LED3;
            mDeviceState.led4 = LED4;

            if (mDeviceState.hasLEDs)
                WriteLEDs();
        }
        
        /// <summary>
        /// Sets the LEDs to a reversed binary display.
        /// </summary>
        /// <param name="bin">Decimal binary value to use (0 - 15).</param>
        public void SetLEDs(int bin)
        {
            mDeviceState.led1 = (bin & 0x01) > 0;
            mDeviceState.led2 = (bin & 0x02) > 0;
            mDeviceState.led3 = (bin & 0x04) > 0;
            mDeviceState.led4 = (bin & 0x08) > 0;

            if (mDeviceState.hasLEDs)
                WriteLEDs();
        }

        // set leds to a player number (1 = 1st, 2 = 2nd, 3 = 3rd, 4 = 4th 5 = 1st & 2nd, ect.)
        /// <summary>
        /// Sets the LEDs to correspond with the player number.
        /// (e.g. 1 = 1st LED &amp; 4 = 4th LED)
        /// </summary>
        /// <param name="num">Player LED to set (0 - 15)</param>
        public void SetPlayerLED(int num)
        {
            // 1st LED
            if (num == 1 || num == 5 || num == 8 || num == 10 || num == 11 || num > 12)
                mDeviceState.led1 = true;
            else
                mDeviceState.led1 = false;

            // 2nd LED
            if (num == 2 || num == 5 || num == 6 || num == 9 || num == 11 || num == 12 || num > 13)
                mDeviceState.led2 = true;
            else
                mDeviceState.led2 = false;

            // 3rd LED
            if (num == 3 || num == 6 || num == 7 || num == 8 || num == 11 || num == 12 || num == 13 || num == 15)
                mDeviceState.led3 = true;
            else
                mDeviceState.led3 = false;

            // 4th LED
            if (num == 4 || num == 7 || num == 9 || num == 10 || num > 11)
                mDeviceState.led4 = true;
            else
                mDeviceState.led4 = false;

            if (mDeviceState.hasLEDs)
                WriteLEDs();
        }

        /// <summary>
        /// Enable or Disable the rumble.
        /// </summary>
        /// <param name="set">Rumble State</param>
        public void SetRumble(bool set)
        {
            mDeviceState.SetRumble(set);
            WriteLEDs();
        }

        /// <summary>
        /// Initiate the Motion Plus Extension
        /// </summary>
        public void StartMotionPlus()
        {
            /// TODO: determine if we need to pass through Nunchuck or Classic Controller
            WriteByte(Constants.REGISTER_MOTIONPLUS_INIT, 0x04);
        }
        #endregion

        #region Setting Calibration
        /// <summary>
        /// Set's the controller's calibration
        /// </summary>
        /// <param name="calibration">Pro Controller's Calibration</param>
        public void UpdateCalibration(ProCalibration calibration)
        {
            mCalibrationPro = calibration;
            if (mDeviceState.GetType() == typeof(ProControllerState))
                ((ProControllerState)mDeviceState).SetCalibration(calibration);
        }

        /// <summary>
        /// Set's the controller's calibration
        /// </summary>
        /// <param name="calibration">Wiimote's Calibration</param>
        public void UpdateCalibration(WiimoteCalibration calibration)
        {
            mCalibrationWiimote = calibration;
            if (mDeviceState.GetType() == typeof(WiimoteState))
                ((WiimoteState)mDeviceState).SetCalibration(calibration);
        }

        /// <summary>
        /// Set's the controller's calibration
        /// </summary>
        /// <param name="calibration">Nunchuck's Calibration</param>
        public void UpdateCalibration(NunchuckCalibration calibration)
        {
            mCalibrationNunchuck = calibration;
            if (mDeviceState.GetType() == typeof(WiimoteState))
            {
                if (((WiimoteState)mDeviceState).extension.GetType() == typeof(NunchuckState))
                    ((NunchuckState)((WiimoteState)mDeviceState).extension).SetCalibration(calibration);
            }
        }

        /// <summary>
        /// Set's the controller's calibration
        /// </summary>
        /// <param name="calibration">Classic Controller's Calibration</param>
        public void UpdateCalibration(ClassicControllerCalibration calibration)
        {
            mCalibrationClassic = calibration;
            if (mDeviceState.GetType() == typeof(WiimoteState))
            {
                if (((WiimoteState)mDeviceState).extension.GetType() == typeof(ClassicControllerState))
                    ((ClassicControllerState)((WiimoteState)mDeviceState).extension).SetCalibration(calibration);
            }
        }

        /// <summary>
        /// Set's the controller's calibration
        /// </summary>
        /// <param name="calibration">Classic Controller Pro's Calibration</param>
        public void UpdateCalibration(ClassicControllerProCalibration calibration)
        {
            mCalibrationClassicPro = calibration;
            if (mDeviceState.GetType() == typeof(WiimoteState))
            {
                if (((WiimoteState)mDeviceState).extension.GetType() == typeof(ClassicControllerProState))
                    ((ClassicControllerProState)((WiimoteState)mDeviceState).extension).SetCalibration(calibration);
            }
        }

        /* TODO: Motion Plus Calibration
        public void UpdateCalibration(MotionPlusCalibration calibration)
        {
            mCalibrationMotionPlus = calibration;
            if (mDeviceState.GetType() == typeof(WiimoteState))
            {
                if (((WiimoteState)mDeviceState).Extension.GetType() == typeof(MotionPlusState))
                    ((MotionPlusState)((WiimoteState)mDeviceState).Extension).SetCalibration(calibration);
            }
        }
        */
        #endregion
    }
}
