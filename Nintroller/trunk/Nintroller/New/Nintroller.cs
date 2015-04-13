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
        public event EventHandler<StateChangeEventArgs> StateUpdate = delegate { };
        /// <summary>
        /// Fired when the controller's extension changes.
        /// </summary>
        public event EventHandler<ExtensionChangeEventArgs> ExtensionChange = delegate { };
        
        private string           _path = string.Empty;
        private bool             _connected;
        private NintyState       _state;
        private ControllerType   _currentType = ControllerType.Unknown;

        // Read/Writing Variables
        private SafeFileHandle   _fileHandle;       // Handle for Reading and Writing
        private FileStream       _stream;           // Read and Write Stream
        private ReadReportType   _readType;         // help with parsing ReadMem reports
        private object           _readingObj;       // for locking/blocking
        private bool             _reading = false;  // notes if actevly reading
        
        // help with parsing Acknowledgement Reports
        private AcknowledgementType _ackType = AcknowledgementType.NA; 

        // Calibration Variables - Probably won't need these
        private WiimoteCalibration mCalibrationWiimote;
        private MotionPlusCalibration mCalibrationMotionPlus;
        private NunchuckCalibration mCalibrationNunchuck;
        private ClassicControllerCalibration mCalibrationClassic;
        private ClassicControllerProCalibration mCalibrationClassicPro;
        private ProCalibration mCalibrationPro;
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
        #endregion

        #region LifeCycle

        /// <summary>
        /// Creates a controller with it's known location.
        /// (Ideally connection ready)
        /// </summary>
        /// <param name="devicePath">The HID Path</param>
        public Nintroller(string devicePath)
        {
            _state = new NintyState();
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

                // TODO: New: Parse the read result

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

            buffer[1] = (byte)((address & 0xff000000) >> 24); // TODO: New: OR with rumble bit (0x01 or 0x00)
            buffer[2] = (byte)((address & 0x00ff0000) >> 16);
            buffer[3] = (byte)((address & 0x0000ff00) >>  8);
            buffer[4] = (byte) (address & 0x000000ff);

            buffer[5] = (byte)((size & 0xff00) >> 8);
            buffer[6] = (byte) (size & 0xff);

            SendData(buffer);

            // TODO: New: Determine if no reading should occur until the report comes back
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
            buffer[1] = 0x00; // TODO: New: Use Rumble bit

            SendData(buffer);
        }

        private void ApplyReportingType(InputReport reportType)
        {
            byte[] buffer = new byte[Constants.REPORT_LENGTH];

            buffer[0] = (byte)OutputReport.DataReportMode;
            buffer[1] = (byte)(0x00); // TOOD: New: 0x40 for continues, OR with Rumble Bit
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
            buffer[1] = (byte)((address & 0xff000000) >> 24); // TODO: New: OR with Rumble bit
            buffer[2] = (byte)((address & 0x00ff0000) >> 16);
            buffer[3] = (byte)((address & 0x0000ff00) >>  8);
            buffer[4] = (byte) (address & 0x000000ff);
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
                (0x00) // TODO: New: OR with rumble bit
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

                    // Battery Level
                    byte rawBattery = report[6];
                    bool lowBattery = (report[3] & 0x01) != 0;
                    // TODO: New: Update Battery

                    // LED
                    bool led1 = (report[3] & 0x10) != 0;
                    bool led2 = (report[3] & 0x20) != 0;
                    bool led3 = (report[3] & 0x40) != 0;
                    bool led4 = (report[3] & 0x80) != 0;
                    // TODO: New: Update LEDs

                    // Extension/Type
                    lock (_readingObj)
                    {
                        _readType = ReadReportType.Extension_A;
                        ReadMemory(Constants.REGISTER_EXTENSION_TYPE_2, 1);
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

                                // and Fire Event

                                // and set report
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
                                        //
                                        break;

                                    case ControllerType.BalanceBoard:
                                        //
                                        break;

                                    case ControllerType.Nunchuk:
                                    case ControllerType.NunchukB:
                                    case ControllerType.ClassicController:
                                    case ControllerType.ClassicControllerPro:
                                    case ControllerType.MotionPlus:
                                        //
                                        break;

                                    case ControllerType.PartiallyInserted:
                                        // try again
                                        GetStatus();
                                        break;

                                    case ControllerType.Drums:
                                    case ControllerType.Guitar:
                                    case ControllerType.TaikoDrum:
                                        //
                                        break;

                                    default:
                                        Log("Unhandled controller type");
                                        break;
                                }

                                // TODO: Get calibration if PID != 330

                                // TODO: New: Fire ExtensionChange event
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

        public void EnableIR()
        {
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
    }

    #region New Event Args

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
    #endregion
}