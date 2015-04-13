using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NintrollerLib.New
{
    public class Nintroller : IDisposable
    {
        #region Members
        // Events
        /// <summary>
        /// Fired evertime a data report is recieved.
        /// </summary>
        //public event EventHandler<StateChangeEventArgs> StateUpdate = delegate { };
        /// <summary>
        /// Fired when the controller's extension changes.
        /// </summary>
        //public event EventHandler<ExtensionChangeEventArgs> ExtensionChange = delegate { };

        public event EventHandler<NintrollerStateEventArgs> StateUpdate = delegate { };
        public event EventHandler<ControllerType> ExtensionChange = delegate { };
        public event EventHandler<BatteryStatus> LowBattery = delegate { };
        
        private string           _path = string.Empty;
        private bool             _connected;
        private INintrollerState _state;
        private ControllerType   _currentType = ControllerType.Unknown;
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

        public bool RumbleEnabled
        {
            get
            {
                return _rumbleBit == 0x01;
            }

            // TODO: New: Enable/Disable Rumble
        }

        public bool Led1
        {
            get
            {
                return _led1;
            }

            // TODO: New: Set LED
        }

        public bool Led2
        {
            get
            {
                return _led2;
            }

            // TODO: New: Set LED
        }

        public bool Led3
        {
            get
            {
                return _led3;
            }

            // TODO: New: Set LED
        }

        public bool Led4
        {
            get
            {
                return _led4;
            }

            // TODO: New: Set LED
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

        internal void Log(string message)
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

            buffer[1] = (byte)(((address & 0xff000000) >> 24) | _rumbleBit);
            buffer[2] =  (byte)((address & 0x00ff0000) >> 16);
            buffer[3] =  (byte)((address & 0x0000ff00) >>  8);
            buffer[4] =  (byte) (address & 0x000000ff);

            buffer[5] = (byte)((size & 0xff00) >> 8);
            buffer[6] = (byte) (size & 0xff);

            SendData(buffer);

            // TODO: New: Determine if no reading should occur until the report comes back
            // I'm thinking no. This function is typically only called when we go in status reporting mode
        }

        private void GetCalibration()
        {
            // don't attempt on Pro Controllers
            ReadMemory(0x0016, 7);
        }

        private void GetStatus()
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.StatusRequest;
            buffer[1] = _rumbleBit;

            SendData(buffer);
        }

        private void ApplyReportingType(InputReport reportType, bool continuous = true)
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.DataReportMode;
            buffer[1] = (byte)((continuous ? 0x40 : 0x00) | _rumbleBit);
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

            if (_stream != null && _stream.CanWrite)
            {
                try
                {
                    // send via the file stream
                    _stream.Write(report, 0, Constants.REPORT_LENGTH);

                    // TOOD: New: Determine if and when to use HidD_SetOutputReport
                    // TODO: New: Determine if nothing else should occur until this report is acknowledged
                    // No, we can send data without an acknowledgement
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
            buffer[1] = (byte)(((address & 0xff000000) >> 24) | _rumbleBit);
            buffer[2] = (byte) ((address & 0x00ff0000) >> 16);
            buffer[3] = (byte) ((address & 0x0000ff00) >>  8);
            buffer[4] = (byte)  (address & 0x000000ff);
            buffer[5] = (byte)data.Length;

            Array.Copy(data, 0, buffer, 6, Math.Min(data.Length, 16));

            SendData(buffer);
        }

        private void ApplyLEDs(bool one, bool two, bool three, bool four)
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.LEDs;
            buffer[1] = (byte)
            (
                (one   ? 0x10 : 0x00) |
                (two   ? 0x20 : 0x00) |
                (three ? 0x40 : 0x00) |
                (four  ? 0x80 : 0x00) |
                (_rumbleBit)
            );

            SendData(buffer);
        }

        #endregion

        #region Data Parsing

        private void ParseReport(byte[] report)
        {
            InputReport input = (InputReport)report[0];

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
                                LowBattery(this, BatteryStatus.VeryLow);
                            }

                            // LED
                            _led1 = (report[3] & 0x10) != 0;
                            _led2 = (report[3] & 0x20) != 0;
                            _led3 = (report[3] & 0x40) != 0;
                            _led4 = (report[3] & 0x80) != 0;

                            // Extension/Type
                            lock (_readingObj)
                            {
                                _readType = ReadReportType.Extension_A;
                                ReadMemory(Constants.REGISTER_EXTENSION_TYPE_2, 1);
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
                                // TODO: New: Remove extension
                                _currentType = ControllerType.Wiimote;
                                _state = new Wiimote();

                                // and Fire Event
                                ExtensionChange(this, _currentType);

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

                                // TODO: New: Fire ExtensionChange event
                                ExtensionChange(this, _currentType);
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
                    // TODO: New: continue doing whatever

                    switch (_ackType)
                    {
                        case AcknowledgementType.IR_Step1:
                            _ackType = AcknowledgementType.IR_Step2;
                            WriteToMemory(Constants.REGISTER_IR_SENSITIVITY_1, new byte[] { 0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x90, 0x00, 0x41 });
                            break;

                        case AcknowledgementType.IR_Step2:
                            _ackType = AcknowledgementType.IR_Step3;
                            WriteToMemory(Constants.REGISTER_IR_SENSITIVITY_2, new byte[] { 0x40, 0x00 });
                            break;

                        case AcknowledgementType.IR_Step3:
                            _ackType = AcknowledgementType.IR_Step4;
                            WriteToMemory(Constants.REGISTER_IR_MODE, new byte[] { 0x01 });
                            break;

                        case AcknowledgementType.IR_Step4:
                            _ackType = AcknowledgementType.IR_Step5;
                            WriteToMemory(Constants.REGISTER_IR, new byte[] { 0x08 });
                            break;

                        case AcknowledgementType.IR_Step5:
                            Log("IR Camera Enabled");
                            _ackType = AcknowledgementType.NA;
                            SetReportType(InputReport.BtnsAccIRExt);
                            break;

                        default:
                            Log("Unhandled acknowledgement");
                            _ackType = AcknowledgementType.NA;
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
                    // TODO: New: Parse controller's input
                    break;
                #endregion

                default:
                    Log("Unexpected Report type: " + input.ToString("x"));
                    break;
            }
        }

        #endregion

        public void EnableIR(IRSetting mode = IRSetting.Basic)
        {
            // TODO: New: Incorperate IRSetting

            ControllerType[] compatableTypes = new ControllerType[]
            {
                ControllerType.Wiimote,
                ControllerType.Nunchuk,
                ControllerType.NunchukB,
                ControllerType.MotionPlus,
                ControllerType.ClassicController,
                ControllerType.ClassicControllerPro
            };

            if (!compatableTypes.Contains(_currentType))
            {
                Log("Can't Enabled IR Camera for type " + _currentType.ToString());
            }
            else
            {
                Log("Enabling IR Camera");
                
                _statusType = StatusType.IR_Enable;
                byte[] buffer = new byte[Constants.REPORT_LENGTH];
                buffer[0] = (byte)OutputReport.StatusRequest;

                SendData(buffer);
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
    }

    #region New Event Args

    public class NintrollerStateEventArgs : EventArgs
    {
        public Nintroller sender;
        public ControllerType controllerType;
        public INintrollerState state;
        public BatteryStatus batteryLevel;
        
        // TODO: New: Create Constructor
        public NintrollerStateEventArgs(Nintroller device, ControllerType type, INintrollerState state, BatteryStatus battery)
        {
            this.sender = device;
            this.controllerType = type;
            this.state = state;
            this.batteryLevel = battery;
        }
    }

    #endregion

    #region New Enums
    internal enum AcknowledgementType
    {
        NA,
        IR_Step1,
        IR_Step2,
        IR_Step3,
        IR_Step4,
        IR_Step5
    }

    internal enum StatusType
    {
        Unknown,
        Requested,
        IR_Enable,
        DiscoverExtension
    }
    #endregion

    #region New Structs

    public interface INintrollerParsable
    {
        public void Parse(byte[] input);
    }

    public interface INintrollerNormalizable
    {
        public void Normalize();
    }

    public struct CoreButtons : INintrollerParsable
    {
        public bool A, B;
        public bool One, Two;
        public bool Up, Down, Left, Right;
        public bool Plus, Minus, Home;

        public void Parse(byte[] input)
        {
            InputReport type = (InputReport)input[0];

            if (type != InputReport.ExtOnly)
            {
                A     = (input[2] & 0x08) != 0;
                B     = (input[2] & 0x04) != 0;
                One   = (input[2] & 0x02) != 0;
                Two   = (input[2] & 0x01) != 0;
                Home  = (input[2] & 0x80) != 0;
                Minus = (input[2] & 0x10) != 0;
                Plus  = (input[1] & 0x10) != 0;
                Up    = (input[1] & 0x08) != 0;
                Down  = (input[1] & 0x04) != 0;
                Right = (input[1] & 0x02) != 0;
                Left  = (input[1] & 0x01) != 0;
            }
        }
    }

    public struct Accelerometer : INintrollerParsable, INintrollerNormalizable
    {
        float rawX, rawY, rawZ;
        float X, Y, Z;

        public void Parse(byte[] input)
        {
            InputReport type = (InputReport)input[0];

            InputReport[] accepted = new InputReport[]
            {
                InputReport.BtnsAcc,
                InputReport.BtnsAccExt,
                InputReport.BtnsAccIR,
                InputReport.BtnsAccIRExt
            };

            if (accepted.Contains(type))
            {
                rawX = input[3];
                rawY = input[4];
                rawZ = input[5];
            }
        }

        public void Normalize()
        {
            throw new NotImplementedException();
        }
    }

    public struct IRPoint
    {
        public int rawX, rawY, size;
        public float x, y;
        public bool visible;
    }

    public struct IR : INintrollerParsable
    {
        IRPoint point1, point2, point3, point4;

        public void Parse(byte[] input)
        {
            InputReport type = (InputReport)input[0];
            int offset = 0;

            if (type == InputReport.BtnsAccIR || type == InputReport.BtnsAccIRExt)
            {
                offset = 6;
            }
            else if (type == InputReport.BtnsIRExt)
            {
                offset = 3;
            }
            else
            {
                return;
            }

            point1.rawX = input[offset]     | ((input[offset + 2] >> 4) & 0x03) << 8;
            point1.rawY = input[offset + 1] | ((input[offset + 2] >> 6) & 0x03) << 8;

            if (type == InputReport.BtnsAccIR)
            {
                // Extended Mode
                point2.rawX = input[offset + 3]  | ((input[offset + 5]  >> 4) & 0x03) << 8;
                point2.rawY = input[offset + 4]  | ((input[offset + 5]  >> 6) & 0x03) << 8;
                point3.rawX = input[offset + 6]  | ((input[offset + 8]  >> 4) & 0x03) << 8;
                point3.rawY = input[offset + 7]  | ((input[offset + 8]  >> 6) & 0x03) << 8;
                point4.rawX = input[offset + 9]  | ((input[offset + 11] >> 4) & 0x03) << 8;
                point4.rawY = input[offset + 10] | ((input[offset + 11] >> 6) & 0x03) << 8;
                
                point1.size = input[offset + 2]  & 0x0f;
                point2.size = input[offset + 5]  & 0x0f;
                point3.size = input[offset + 8]  & 0x0f;
                point4.size = input[offset + 11] & 0x0f;
                
                point1.visible = !(input[offset]     == 0xff && input[offset + 1]  == 0xff && input[offset + 2]  == 0xff);
                point2.visible = !(input[offset + 3] == 0xff && input[offset + 4]  == 0xff && input[offset + 5]  == 0xff);
                point3.visible = !(input[offset + 6] == 0xff && input[offset + 7]  == 0xff && input[offset + 8]  == 0xff);
                point4.visible = !(input[offset + 9] == 0xff && input[offset + 10] == 0xff && input[offset + 11] == 0xff);
            }
            else
            {
                // Basic Mode
                point2.rawX = input[offset + 3] | ((input[offset + 2] >> 0) & 0x03) << 8;
                point2.rawY = input[offset + 4] | ((input[offset + 2] >> 2) & 0x03) << 8;
                point3.rawX = input[offset + 5] | ((input[offset + 7] >> 4) & 0x03) << 8;
                point3.rawY = input[offset + 6] | ((input[offset + 7] >> 6) & 0x03) << 8;
                point4.rawX = input[offset + 8] | ((input[offset + 7] >> 0) & 0x03) << 8;
                point4.rawY = input[offset + 9] | ((input[offset + 7] >> 2) & 0x03) << 8;
                
                point1.size = 0x00;
                point2.size = 0x00;
                point3.size = 0x00;
                point4.size = 0x00;
                
                point1.visible = !(input[offset]     == 0xff && input[offset + 1] == 0xff);
                point2.visible = !(input[offset + 3] == 0xff && input[offset + 4] == 0xff);
                point3.visible = !(input[offset + 5] == 0xff && input[offset + 6] == 0xff);
                point4.visible = !(input[offset + 8] == 0xff && input[offset + 9] == 0xff);
            }
        }
    }

    public struct Trigger : INintrollerParsable
    {
        public short rawValue;
        public float value;
    }

    public struct Joystick : INintrollerParsable
    {
        public short rawValue;
        public float value;
    }

    #endregion

    #region Full Structs

    struct Wiimote : INintrollerState
    {
        CoreButtons buttons;
        Accelerometer accelerometer;
        IR irSensor;
        //INintrollerState extension;

        public Wiimote(byte[] rawData)
        {
            buttons = new CoreButtons();
            accelerometer = new Accelerometer();
            irSensor = new IR();
            //extension = null;

            buttons.Parse(rawData);
        }
    }

    struct Nunchuk : INintrollerState
    {
        Wiimote wiimote;
        Accelerometer accelerometer;
        Joystick joystick;
        bool C, Z;
    }

    struct ClassicController : INintrollerState
    {
        Wiimote wiimote;
        Joystick LJoy, RJoy;
        Trigger L, R;
        bool A, B, X, Y;
        bool Up, Down, Left, Right;
        bool ZL, ZR, Plus, Minus, Home;
    }

    struct ClassicControllerPro : INintrollerState
    {
        Wiimote wiimote;
        Joystick LJoy, RJoy;
        bool A, B, X, Y;
        bool Up, Down, Left, Right;
        bool L, R, ZL, ZR;
        bool Plus, Minus, Home;
    }

    public struct ProController : INintrollerState
    {
        Joystick LJoy, RJoy;
        bool A, B, X, Y;
        bool Up, Down, Left, Right;
        bool L, R, ZL, ZR;
        bool Plus, Minus, Home;
        bool LStick, RStick;
    }

    public struct BalanceBoard : INintrollerState
    {

    }

    public struct WiimotePlus : INintrollerState
    {
        Wiimote wiimote;
        //gyro
    }
    #endregion
}