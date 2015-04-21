using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NintrollerLib.New
{
    public class Nintroller : IDisposable
    {
        #region Members
        // Events
        public event EventHandler<NintrollerStateEventArgs> StateUpdate = delegate { };
        public event EventHandler<NintrollerExtensionEventArgs> ExtensionChange = delegate { };
        public event EventHandler<LowBatteryEventArgs> LowBattery = delegate { };
        //public event EventHandler<ControllerType> ExtensionChange;
        //public event EventHandler<BatteryStatus> LowBattery;
        
        // General
        private string           _path = string.Empty;
        private bool             _connected;
        private INintrollerState _state = new Wiimote();
        private ControllerType   _currentType = ControllerType.Unknown;
        private IRCamMode        _irMode = IRCamMode.Off;
        private IRCamSensitivity _irSensitivity = IRCamSensitivity.Level3;
        private byte             _rumbleBit = 0x00;
        private byte             _battery = 0x00;
        private bool             _batteryLow;
        private bool             _led1, _led2, _led3, _led4;

        // Read/Writing Variables
        private SafeFileHandle   _fileHandle;                // Handle for Reading and Writing
        private FileStream       _stream;                    // Read and Write Stream
        private bool             _reading = false;           // notes if actevly reading
        private readonly object  _readingObj = new object(); // for locking/blocking
        
        // help with parsing Reports
        private AcknowledgementType _ackType = AcknowledgementType.NA;
        private StatusType          _statusType = StatusType.Unknown;
        private ReadReportType      _readType = ReadReportType.Unknown;

        #endregion

        #region Properties
        /// <summary>
        /// If the controller is open to communication.
        /// </summary>
        public bool Connected { get { return _connected; } }
        /// <summary>
        /// The HID path of the controller.
        /// </summary>
        public string HIDPath { get { return _path; } }
        /// <summary>
        /// The type of controller this has been identified as
        /// </summary>
        public ControllerType Type { get { return _currentType; } }

        public IRCamMode IRMode
        {
            get { return _irMode; }
            set
            {
                if (_irMode != value)
                {
                    switch (_currentType)
                    {
                        case ControllerType.Wiimote:
                            // this can be set to any mode
                            _irMode = value;
                            
                            if (value == IRCamMode.Off)
                            {
                                DisableIR();
                            }
                            else
                            {
                                EnableIR();
                            }
                            break;

                        case ControllerType.ClassicController:
                        case ControllerType.ClassicControllerPro:
                        case ControllerType.Nunchuk:
                        case ControllerType.NunchukB:
                            // on certian modes can be set
                            if (value == IRCamMode.Off)
                            {
                                _irMode = value;
                                DisableIR();
                            }
                            else if (value != IRCamMode.Full)
                            {
                                _irMode = value;
                                EnableIR();
                            }
                            break;

                        default:
                            // do nothing
                            break;
                    }
                }
            }
        }

        public IRCamSensitivity IRSensitivity
        {
            get { return _irSensitivity; }
            set
            {
                if (_irSensitivity != value && _irMode != IRCamMode.Off)
                {
                    switch (_currentType)
                    {
                        case ControllerType.Wiimote:
                        case ControllerType.ClassicController:
                        case ControllerType.ClassicControllerPro:
                        case ControllerType.Nunchuk:
                        case ControllerType.NunchukB:
                            _irSensitivity = value;
                            EnableIR();
                            break;

                        default:
                            // do nothing
                            break;
                    }
                }
            }
        }

        public bool RumbleEnabled
        {
            get
            {
                return _rumbleBit == 0x01;
            }
            set
            {
                _rumbleBit = (byte)(value ? 0x01 : 0x00);
                ApplyLEDs();
            }
        }

        public bool Led1
        {
            get
            {
                return _led1;
            }
            set
            {
                if (_led1 != value)
                {
                    _led1 = value;
                    ApplyLEDs();
                }
            }
        }

        public bool Led2
        {
            get
            {
                return _led2;
            }
            set
            {
                if (_led2 != value)
                {
                    _led2 = value;
                    ApplyLEDs();
                }
            }
        }

        public bool Led3
        {
            get
            {
                return _led3;
            }
            set
            {
                if (_led3 != value)
                {
                    _led3 = value;
                    ApplyLEDs();
                }
            }
        }

        public bool Led4
        {
            get
            {
                return _led4;
            }
            set
            {
                if (_led4 != value)
                {
                    _led4 = value;
                    ApplyLEDs();
                }
            }
        }

        public BatteryStatus BatteryLevel
        {
            get
            {
                if (_batteryLow)
                {
                    return BatteryStatus.VeryLow;
                }
                else
                {
                    // Wiimote's parsing
                    //batteryLevel = 100.0f * (float)batteryRaw / 192.0f;
                    //lowBattery = batteryLevel < 0.1f;
                    //
                    //if (batteryLevel > 80f)
                    //    Battery = BatteryStatus.VeryHigh;
                    //else if (batteryLevel > 60f)
                    //    Battery = BatteryStatus.High;
                    //else if (batteryLevel > 40f)
                    //    Battery = BatteryStatus.Medium;
                    //else if (batteryLevel > 20f)
                    //    Battery = BatteryStatus.Low;
                    //else
                    //    Battery = BatteryStatus.VeryLow;

                    // Pro Controller's parsing
                    //batteryLevel = 2f * ((float)batteryRaw - 205f);
                    //lowBattery = batteryLevel < 50f;

                    //if (batteryLevel > 90f)
                    //    Battery = BatteryStatus.VeryHigh;
                    //else if (batteryLevel > 80f)
                    //    Battery = BatteryStatus.High;
                    //else if (batteryLevel > 70f)
                    //    Battery = BatteryStatus.Medium;
                    //else if (batteryLevel > 60f)
                    //    Battery = BatteryStatus.Low;
                    //else
                    //    Battery = BatteryStatus.VeryLow;

                    // TODO: New: Check if battery parsing is right
                    if (_battery == 0xFF)
                        return BatteryStatus.VeryHigh;
                    else if (_battery > 0xE0)
                        return BatteryStatus.High;
                    else if (_battery > 0xA0)
                        return BatteryStatus.Medium;
                    else
                        return BatteryStatus.Low;
                }
            }
        }
        #endregion

        #region LifeCycle

        /// <summary>
        /// Creates a controller with it's known location.
        /// (Ideally connection ready)
        /// </summary>
        /// <param name="devicePath">The HID Path</param>
        public Nintroller(string devicePath)
        {
            _state = null;
            _path = devicePath;
        }

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }

        internal static void Log(string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#endif
        }

        #endregion

        #region Connectivity

        public bool Connect()
        {
            if (string.IsNullOrWhiteSpace(_path))
                return false;

            try
            {
                // Open Read 'n Write file handle
                _fileHandle = HIDImports.CreateFile(_path, FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

                // create a stream from the file
                _stream = new FileStream(_fileHandle, FileAccess.ReadWrite, Constants.REPORT_LENGTH, true);

                _connected = true;

                Log("Connected to device (" + _path + ")");
                return true;
            }
            catch (Exception ex)
            {
                Log("Error Connecting to device (" + _path + "): " + ex.Message);
                return false;
            }
        }

        public void Disconnect()
        {
            _reading = false;

            if (_stream != null)
                _stream.Close();

            if (_fileHandle != null)
                _fileHandle.Close();

            _connected = false;

            Log("Disconnected device (" + _path + ")");
        }

        #endregion

        #region Data Requesting

        public void BeginReading()
        {
            // kickoff the reading process if it hasn't started already
            if (!_reading && _stream != null)
            {
                _reading = true;
                ReadAsync();
            }
        }

        // Performs an asynchronous read
        private void ReadAsync()
        {
            if (_stream != null && _stream.CanRead)
            {
                lock (_readingObj)
                {
                    byte[] readResult = new byte[Constants.REPORT_LENGTH];
                    
                    try
                    {
                        _stream.BeginRead(readResult, 0, readResult.Length, new AsyncCallback(RecieveDataAsync), readResult);
                    }
                    catch (ObjectDisposedException)
                    {
                        Log("Can't read, the stream was disposed");
                    }
                }
            }
        }

        // Recieve Data from reading the stream
        private void RecieveDataAsync (IAsyncResult data)
        {
            try
            {
                // Convert the result
                byte[] result = data.AsyncState as byte[];

                // Must be called for each BeginRead()
                _stream.EndRead(data);

                ParseReport(result);

                // start another read if we are still to be reading
                if (_reading)
                {
                    ReadAsync();
                }
            }
            catch (OperationCanceledException)
            {
                Log("Async Read Was Cancelled");
            }
        }

        // Request data from the device's memory
        private void ReadMemory(int address, short size)
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.ReadMemory;

            buffer[1] = (byte)(((address & 0xFF000000) >> 24) | _rumbleBit);
            buffer[2] = (byte) ((address & 0x00FF0000) >> 16);
            buffer[3] = (byte) ((address & 0x0000FF00) >>  8);
            buffer[4] = (byte)  (address & 0x000000FF);

            buffer[5] = (byte)((size & 0xFF00) >> 8);
            buffer[6] = (byte) (size & 0xFF);

            SendData(buffer);
        }

        private void ReadMemory(int address, byte[] data)
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.ReadMemory;

            buffer[1] = (byte)(((address & 0xFF000000) >> 24) | _rumbleBit);
            buffer[2] = (byte)((address & 0x00FF0000) >> 16);
            buffer[3] = (byte)((address & 0x0000FF00) >> 8);
            buffer[4] = (byte)(address & 0x000000FF);
            buffer[5] = (byte)data.Length;

            Array.Copy(data, 0, buffer, 6, Math.Min(data.Length, 16));

            SendData(buffer);
        }

        private void GetCalibration()
        {
            // don't attempt on Pro Controllers
            ReadMemory(0x0016, 7);
        }

        public void GetStatus()
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.StatusRequest;
            buffer[1] = _rumbleBit;

            SendData(buffer);
        }

        private void ApplyReportingType(InputReport reportType, bool continuous = false)
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.DataReportMode;
            buffer[1] = (byte)((continuous ? 0x04 : 0x00) | _rumbleBit);
            buffer[2] = (byte)reportType;

            SendData(buffer);
        }

        public void SetReportType(InputReport reportType)
        {
            if (reportType == InputReport.Acknowledge ||
                reportType == InputReport.ReadMem     ||
                reportType == InputReport.Status)
            {
                Log("Can't Set the report type to: " + reportType.ToString());
            }
            else
            {
                ApplyReportingType(reportType);
            }
        }
        #endregion

        #region Data Sending

        private void SendData(byte[] report)
        {
            if (!_connected)
            {
                Log("Can't Send data, we are not connected!");
                return;
            }

            Log("Sending " + Enum.Parse(typeof(OutputReport), report[0].ToString()) + " report");
            Log(BitConverter.ToString(report));

            if (_stream != null && _stream.CanWrite)
            {
                try
                {
                    // send via the file stream
                    _stream.Write(report, 0, Constants.REPORT_LENGTH);

                    // TOOD: New: Determine if and when to use HidD_SetOutputReport
                }
                catch (Exception ex)
                {
                    Log("Error while writing to the stream: " + ex.Message);
                }
            }
        }

        private void WriteToMemory(int address, byte[] data)
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.WriteMemory;
            buffer[1] = (byte)(((address & 0xFF000000) >> 24) | _rumbleBit);
            buffer[2] = (byte) ((address & 0x00FF0000) >> 16);
            buffer[3] = (byte) ((address & 0x0000FF00) >>  8);
            buffer[4] = (byte)  (address & 0x000000FF);
            buffer[5] = (byte)data.Length;

            Array.Copy(data, 0, buffer, 6, Math.Min(data.Length, 16));

            Debug.WriteLine(BitConverter.ToString(buffer));

            SendData(buffer);
        }

        private void ApplyLEDs()
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.LEDs;
            buffer[1] = (byte)
            (
                (_led1 ? 0x10 : 0x00) |
                (_led2 ? 0x20 : 0x00) |
                (_led3 ? 0x40 : 0x00) |
                (_led4 ? 0x80 : 0x00) |
                (_rumbleBit)
            );

            SendData(buffer);
        }

        #endregion

        #region Data Parsing

        private void ParseReport(byte[] report)
        {
            InputReport input = (InputReport)report[0];
            bool error = (report[4] & 0x0F) == 0x03;

            switch(input)
            {
                #region Status Reports
                case InputReport.Status:
                    #region Parse Status
                    Log("Status Report");

                    // core buttons can be parsed if desired

                    switch (_statusType)
                    {
                        case StatusType.Requested:
                            //
                            break;

                        case StatusType.IR_Enable:
                            EnableIR();
                            break;

                        case StatusType.Unknown:
                        default:
                            // Battery Level
                            _battery = report[6];
                            bool lowBattery = (report[3] & 0x01) != 0;
                            
                            if (lowBattery && !_batteryLow)
                            {
                                //LowBattery(this, BatteryStatus.VeryLow);
                                LowBattery(this, new LowBatteryEventArgs(BatteryStatus.VeryLow));
                            }

                            // LED
                            _led1 = (report[3] & 0x10) != 0;
                            _led2 = (report[3] & 0x20) != 0;
                            _led3 = (report[3] & 0x40) != 0;
                            _led4 = (report[3] & 0x80) != 0;

                            // Extension/Type
                            bool ext = (report[3] & 0x02) != 0;
                            if (ext)
                            {
                                //lock (_readingObj)
                                //{
                                //    _readType = ReadReportType.Extension_A;
                                //    ReadMemory(Constants.REGISTER_EXTENSION_TYPE_2, 1);
                                // 16-04-A4-00-F0-01-55-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00
                                // 16-04-A4-00-FB-01-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00
                                //}
                            }
                            else if (_currentType != ControllerType.Wiimote)
                            {
                                _currentType = ControllerType.Wiimote;
                                _state = new Wiimote();
                                _state.Update(report);

                                // and Fire Event
                                //ExtensionChange(this, _currentType);
                                ExtensionChange(this, new NintrollerExtensionEventArgs(_currentType));

                                // and set report
                                SetReportType(InputReport.BtnsAccIR);
                            }
                            break;
                    }
                    #endregion
                    break;

                case InputReport.ReadMem:
                    #region Parse ReadMem
                    Log("Read Memory Report | " + _readType.ToString());

                    switch(_readType)
                    {
                        case ReadReportType.Extension_A:
                            bool hasExtension = (report[3] & 0x02) != 0;

                            // TODO: Account for Wiimote+ controllers
                            if (hasExtension)
                            {
                                // Initialize
                                lock (_readingObj)
                                {
                                    if (report[0] != 0x04)
                                    {
                                        WriteToMemory(Constants.REGISTER_EXTENSION_INIT_1, new byte[] { 0x55 });
                                        WriteToMemory(Constants.REGISTER_EXTENSION_INIT_2, new byte[] { 0x00 });
                                    }
                                }

                                _readType = ReadReportType.Extension_B;
                                ReadMemory(Constants.REGISTER_EXTENSION_TYPE, 6);
                            }
                            else if (_currentType != ControllerType.Wiimote)
                            {
                                _currentType = ControllerType.Wiimote;
                                _state = new Wiimote();
                                _state.Update(report);

                                // and Fire Event
                                //ExtensionChange(this, _currentType);
                                ExtensionChange(this, new NintrollerExtensionEventArgs(_currentType));

                                // and set report
                                SetReportType(InputReport.BtnsAccIR);
                            }
                            break;

                        case ReadReportType.Extension_B:
                            if (report.Length < 6)
                            {
                                _readType = ReadReportType.Unknown;
                                return;
                            }

                            byte[] r = new byte[6];
                            Array.Copy(report, 6, r, 0, 6);

                            long type = 
                                ((long)r[0] << 40) | 
                                ((long)r[1] << 32) | 
                                ((long)r[2] << 24) | 
                                ((long)r[3] << 16) | 
                                ((long)r[4] <<  8) | r[5];

                            if (_currentType != (ControllerType)type)
                            {
                                _currentType = (ControllerType)type;

                                Log("Controller type: " + _currentType.ToString());

                                // TODO: New: Handle the controller type
                                switch(_currentType)
                                {
                                    case ControllerType.ProController:
                                        _state = new ProController();
                                        break;

                                    case ControllerType.BalanceBoard:
                                        _state = new BalanceBoard();
                                        break;

                                    case ControllerType.Nunchuk:
                                    case ControllerType.NunchukB:
                                        _state = new Nunchuk();
                                        break;

                                    case ControllerType.ClassicController:
                                        _state = new ClassicController();
                                        break;

                                    case ControllerType.ClassicControllerPro:
                                        _state = new ClassicControllerPro();
                                        break;

                                    case ControllerType.MotionPlus:
                                        _state = new WiimotePlus();
                                        break;

                                    case ControllerType.PartiallyInserted:
                                        // try again
                                        GetStatus();
                                        break;

                                    case ControllerType.Drums:
                                    case ControllerType.Guitar:
                                    case ControllerType.TaikoDrum:
                                        // TODO: New: Musicals
                                        break;

                                    default:
                                        Log("Unhandled controller type");
                                        break;
                                }

                                // TODO: Get calibration if PID != 330

                                // Fire ExtensionChange event
                                //ExtensionChange(this, _currentType);
                                ExtensionChange(this, new NintrollerExtensionEventArgs(_currentType));
                                // set report
                            }
                            break;

                        default:
                            Log("Unrecognized Read Memory report");
                            break;
                    }
                    #endregion
                    break;

                case InputReport.Acknowledge:
                    #region Parse Acknowledgement
                    Log("Output Acknowledged");

                    switch (_ackType)
                    {
                        case AcknowledgementType.IR_Step1:
                            byte[] sensitivityBlock1 = null;
                            
                            switch (_irSensitivity)
                            {
                                case IRCamSensitivity.Custom:
                                    sensitivityBlock1 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x90, 0x00, 0xC0 };
                                    break;

                                case IRCamSensitivity.CustomHigh:
                                    sensitivityBlock1 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x90, 0x00, 0x41 };
                                    break;

                                case IRCamSensitivity.CustomMax:
                                    sensitivityBlock1 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x0C };
                                    break;

                                case IRCamSensitivity.Level1:
                                    sensitivityBlock1 = new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x64, 0x00, 0xFE };
                                    break;
                                    
                                case IRCamSensitivity.Level2:
                                    sensitivityBlock1 = new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x96, 0x00, 0xB4 };
                                    break;

                                case IRCamSensitivity.Level4:
                                    sensitivityBlock1 = new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xc8, 0x00, 0x36 };
                                    break;

                                case IRCamSensitivity.Level5:
                                    sensitivityBlock1 = new byte[] { 0x07, 0x00, 0x00, 0x71, 0x01, 0x00, 0x72, 0x00, 0x20 };
                                    break;

                                case IRCamSensitivity.Level3:
                                default:
                                    sensitivityBlock1 = new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xaa, 0x00, 0x64 };
                                    break;
                            }

                            _ackType = AcknowledgementType.IR_Step2;
                            WriteToMemory(Constants.REGISTER_IR_SENSITIVITY_1, sensitivityBlock1);
                            break;

                        case AcknowledgementType.IR_Step2:
                            byte[] sensitivityBlock2 = null;
                            
                            switch (_irSensitivity)
                            {
                                case IRCamSensitivity.Custom:
                                    sensitivityBlock2 = new byte[] { 0x40, 0x00 };
                                    break;

                                case IRCamSensitivity.CustomHigh:
                                    sensitivityBlock2 = new byte[] { 0x40, 0x00 };
                                    break;

                                case IRCamSensitivity.CustomMax:
                                    sensitivityBlock2 = new byte[] { 0x00, 0x00 };
                                    break;

                                case IRCamSensitivity.Level1:
                                    sensitivityBlock2 = new byte[] { 0xFD, 0x05 };
                                    break;
                                    
                                case IRCamSensitivity.Level2:
                                    sensitivityBlock2 = new byte[] { 0xB3, 0x04 };
                                    break;

                                case IRCamSensitivity.Level4:
                                    sensitivityBlock2 = new byte[] { 0x35, 0x03 };
                                    break;

                                case IRCamSensitivity.Level5:
                                    sensitivityBlock2 = new byte[] { 0x1F, 0x03 };
                                    break;

                                case IRCamSensitivity.Level3:
                                default:
                                    sensitivityBlock2 = new byte[] { 0x63, 0x03 };
                                    break;
                            }

                            _ackType = AcknowledgementType.IR_Step3;
                            WriteToMemory(Constants.REGISTER_IR_SENSITIVITY_2, sensitivityBlock2);
                            break;

                        case AcknowledgementType.IR_Step3:
                            _ackType = AcknowledgementType.IR_Step4;
                            WriteToMemory(Constants.REGISTER_IR_MODE, new byte[] { (byte)_irMode });
                            break;

                        case AcknowledgementType.IR_Step4:
                            _ackType = AcknowledgementType.IR_Step5;
                            WriteToMemory(Constants.REGISTER_IR, new byte[] { 0x08 });
                            break;

                        case AcknowledgementType.IR_Step5:
                            Log("IR Camera Enabled");
                            _ackType = AcknowledgementType.NA;

                            switch (_irMode)
                            {
                                case IRCamMode.Off:
                                    SetReportType(InputReport.BtnsAccExt);
                                    break;

                                case IRCamMode.Basic:
                                    SetReportType(InputReport.BtnsAccIRExt);
                                    break;

                                case IRCamMode.Wide:
                                    SetReportType(InputReport.BtnsAccIR);
                                    break;

                                case IRCamMode.Full:
                                    // not a supported report type right now
                                    SetReportType(InputReport.BtnsIRExt);
                                    break;
                            }
                            break;

                        default:
                            Log("Unhandled acknowledgement");
                            _ackType = AcknowledgementType.NA;
                            Log(BitConverter.ToString(report));
                            break;
                    }
                    #endregion
                    break;
                #endregion

                #region Data Reports
                case InputReport.BtnsOnly:
                case InputReport.BtnsAcc:
                case InputReport.BtnsExt:
                case InputReport.BtnsAccIR:
                case InputReport.BtnsExtB:
                case InputReport.BtnsAccExt:
                case InputReport.BtnsIRExt:
                case InputReport.BtnsAccIRExt:
                case InputReport.ExtOnly:
                    if (_state != null)
                    {
                        _state.Update(report);
                        var arg = new NintrollerStateEventArgs(_currentType, _state, BatteryLevel);
                        StateUpdate(this, arg);
                    }
                    break;
                #endregion

                default:
                    Log("Unexpected Report type: " + input.ToString("x"));
                    break;
            }
        }

        #endregion

        #region General

        public static List<string> GetControllerPaths()
        {
            List<string> result = new List<string>();
            Guid hidGuid;
            int index = 0;
            SafeFileHandle mHandle;

            // Get GUID of the HID class
            HIDImports.HidD_GetHidGuid(out hidGuid);

            // handle for HID devices
            IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, HIDImports.DIGCF_DEVICEINTERFACE);
            HIDImports.SP_DEVICE_INTERFACE_DATA diData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = Marshal.SizeOf(diData);

            // Step through all devices
            while (HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref hidGuid, index, ref diData))
            {
                UInt32 size;
                // get device buffer size
                HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out size, IntPtr.Zero);

                // create detail struct
                HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();
                diDetail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);

                // populate detail struct
                if (HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, IntPtr.Zero))
                {
                    // open read/write handle for the device
                    mHandle = HIDImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

                    // create attributes structure
                    HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
                    attrib.Size = Marshal.SizeOf(attrib);

                    // populate attributes
                    if (HIDImports.HidD_GetAttributes(mHandle.DangerousGetHandle(), ref attrib))
                    {
                        // check if it matches what we are looking for
                        if (attrib.VendorID == Constants.VID && (attrib.ProductID == Constants.PID1 || attrib.ProductID == Constants.PID2))
                        {
                            result.Add(diDetail.DevicePath);
                        }
                    }

                    mHandle.Close();
                }
                else
                {
                    Log("Failed to get info on a device.");
                }

                index += 1;
            }

            // clean up
            HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);
            Log("Total Controllers Found: " + result.Count.ToString());
            return result;
        }

        internal static float Normalize(int raw, int min, int center, int max, int dead)
        {
            float availableRange = 0f;
            float actualValue = 0f;

            if (Math.Abs(center - raw) < dead)
            {
                return 0f;
            }
            else if (raw - center > 0)
            {
                availableRange = max - (center + dead);
                actualValue = raw - (center + dead);

                return (actualValue / availableRange);
            }
            else
            {
                availableRange = center - dead - min;
                actualValue = raw - center;

                if (availableRange == 0)
                {
                    return 0f;
                }

                return (actualValue / availableRange) - 1f;
            }
        }

        internal static float Normalize(float raw, float min, float center, float max, float dead)
        {
            float availableRange = 0f;
            float actualValue = 0f;

            if (Math.Abs(center - raw) < dead)
            {
                return 0f;
            }
            else if (raw - center > 0)
            {
                availableRange = max - (center + dead);
                actualValue = raw - (center + dead);

                return (actualValue / availableRange);
            }
            else
            {
                availableRange = center - dead - min;
                actualValue = raw - center;

                return (actualValue / availableRange) - 1f;
            }
        }

        private void EnableIR()
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];
            buffer[0] = (byte)OutputReport.IREnable;
            buffer[1] = (byte)(0x04);
            SendData(buffer);

            buffer[0] = (byte)OutputReport.IREnable2;
            buffer[1] = (byte)(0x04);
            SendData(buffer);

            _ackType = AcknowledgementType.IR_Step1;
            WriteToMemory(Constants.REGISTER_IR, new byte[] { 0x08 });
            // continue other steps in Acknowledgement Reporting
        }

        private void DisableIR()
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];
            buffer[0] = (byte)OutputReport.IREnable;
            buffer[1] = (byte)(0x00);
            SendData(buffer);

            buffer[0] = (byte)OutputReport.IREnable2;
            buffer[1] = (byte)(0x00);
            SendData(buffer);

            // TODO: New: Check if we need to monitor the acknowledgment report
        }

        private void StartMotionPlus()
        {
            // TODO: New: Motion Plus
            // determine if we need to pass through Nunchuck or Classic Controller
            //WriteByte(Constants.REGISTER_MOTIONPLUS_INIT, 0x04);
            //WriteToMemory(Constants.REGISTER_MOTIONPLUS_INIT, new byte[] { 0x04 });
        }

        /// <summary>
        /// Sets the LEDs to a reversed binary display.
        /// </summary>
        /// <param name="bin">Decimal binary value to use (0 - 15).</param>
        public void SetBinaryLEDs(int bin)
        {
            _led1 = (bin & 0x01) > 0;
            _led2 = (bin & 0x02) > 0;
            _led3 = (bin & 0x04) > 0;
            _led4 = (bin & 0x08) > 0;

            ApplyLEDs();
        }

        /// <summary>
        /// Sets the LEDs to correspond with the player number.
        /// (e.g. 1 = 1st LED &amp; 4 = 4th LED)
        /// </summary>
        /// <param name="num">Player LED to set (0 - 15)</param>
        public void SetPlayerLED(int num)
        {
            // 1st LED
            if (num == 1 || num == 5 || num == 8 || num == 10 || num == 11 || num > 12)
                _led1 = true;
            else
                _led1 = false;

            // 2nd LED
            if (num == 2 || num == 5 || num == 6 || num == 9 || num == 11 || num == 12 || num > 13)
                _led2 = true;
            else
                _led2 = false;

            // 3rd LED
            if (num == 3 || num == 6 || num == 7 || num == 8 || num == 11 || num == 12 || num == 13 || num == 15)
                _led3 = true;
            else
                _led3 = false;

            // 4th LED
            if (num == 4 || num == 7 || num == 9 || num == 10 || num > 11)
                _led4 = true;
            else
                _led4 = false;

            ApplyLEDs();
        }

        #endregion

        #region Calibration

        public void SetCalibration(Wiimote wiimoteCalibration)
        {
            if (_currentType == ControllerType.Wiimote)
            {
                ((Wiimote)_state).accelerometer.Calibrate(wiimoteCalibration.accelerometer);
            }
            else if (_currentType == ControllerType.Nunchuk || _currentType == ControllerType.NunchukB)
            {
                ((Nunchuk)_state).wiimote.accelerometer.Calibrate(wiimoteCalibration.accelerometer);
            }
            else if (_currentType == ControllerType.ClassicController)
            {
                ((ClassicController)_state).wiimote.accelerometer.Calibrate(wiimoteCalibration.accelerometer);
            }
            else if (_currentType == ControllerType.ClassicControllerPro)
            {
                ((ClassicControllerPro)_state).wiimote.accelerometer.Calibrate(wiimoteCalibration.accelerometer);
            }
        }

        public void SetCalibration(Nunchuk nunchukCalibration)
        {
            if (_currentType == ControllerType.Nunchuk || _currentType == ControllerType.NunchukB)
            {
                ((Nunchuk)_state).joystick.Calibrate(nunchukCalibration.joystick);
                ((Nunchuk)_state).accelerometer.Calibrate(nunchukCalibration.accelerometer);
            }
        }

        public void SetCalibration(ClassicController classicCalibration)
        {
            if (_currentType == ControllerType.ClassicController)
            {
                ((ClassicController)_state).LJoy.Calibrate(classicCalibration.LJoy);
                ((ClassicController)_state).RJoy.Calibrate(classicCalibration.RJoy);
                ((ClassicController)_state).L.Calibrate(classicCalibration.L);
                ((ClassicController)_state).R.Calibrate(classicCalibration.R);
            }
        }

        public void SetCalibration(ClassicControllerPro classicProCalibration)
        {
            ((ClassicControllerPro)_state).LJoy.Calibrate(classicProCalibration.LJoy);
            ((ClassicControllerPro)_state).RJoy.Calibrate(classicProCalibration.RJoy);
        }

        public void SetCalibration(ProController proCalibration)
        {
            ((ProController)_state).LJoy.Calibrate(proCalibration.LJoy);
            ((ProController)_state).RJoy.Calibrate(proCalibration.RJoy);
        }

        #endregion
    }

    #region New Event Args

    public class NintrollerStateEventArgs : EventArgs
    {
        public ControllerType controllerType;
        public INintrollerState state;
        public BatteryStatus batteryLevel;
        
        public NintrollerStateEventArgs(ControllerType type, INintrollerState state, BatteryStatus battery)
        {
            this.controllerType = type;
            this.state          = state;
            this.batteryLevel   = battery;
        }
    }

    public class NintrollerExtensionEventArgs : EventArgs
    {
        public ControllerType controllerType;

        public NintrollerExtensionEventArgs(ControllerType type)
        {
            controllerType = type;
        }
    }

    public class LowBatteryEventArgs : EventArgs
    {
        public BatteryStatus batteryLevel;

        public LowBatteryEventArgs(BatteryStatus level)
        {
            batteryLevel = level;
        }
    }

    #endregion

}