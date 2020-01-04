using System;
using System.Diagnostics;
using System.IO;

namespace NintrollerLib
{
    /// <summary>
    /// Used to represent a Nintendo controller
    /// </summary>
    public class Nintroller : IDisposable
    {
        #region Members
        // Events
        /// <summary>
        /// Called with updated controller input states.
        /// </summary>
        public event EventHandler<NintrollerStateEventArgs>     StateUpdate     = delegate { };
        /// <summary>
        /// Called when an extension change is detected in the controller.
        /// </summary>
        public event EventHandler<NintrollerExtensionEventArgs> ExtensionChange = delegate { };
        /// <summary>
        /// Called when the controller's battery get low.
        /// </summary>
        public event EventHandler<LowBatteryEventArgs>          LowBattery      = delegate { };
        /// <summary>
        /// Called when the connection loss is detected.
        /// </summary>
        public event EventHandler<DisconnectedEventArgs>        Disconnected    = delegate { };
        
        // General
        private bool               _connected                   = false;
        private bool               _proControllerUSupport       = true;
        private INintrollerState   _state                       = new Wiimote();
        private CalibrationStorage _calibrations                = new CalibrationStorage();
        private ControllerType     _currentType                 = ControllerType.Unknown;
        private ControllerType     _forceType                   = ControllerType.Unknown;
        private IRCamMode          _irMode                      = IRCamMode.Off;
        private IRCamSensitivity   _irSensitivity               = IRCamSensitivity.Level3;
        private byte               _rumbleBit                   = 0x00;
        private byte               _battery                     = 0x00;
        private bool               _batteryLow                  = false;
        private bool               _led1, _led2, _led3, _led4;

        // Read/Writing Variables
        private Stream           _stream;                    // Read and Write Stream
        private int              _streamSize = Constants.REPORT_LENGTH;
        private bool             _reading    = false;        // true if actively reading
        private readonly object  _readingObj = new object(); // for locking/blocking
        
        // help with parsing Reports
        private AcknowledgementType _ackType    = AcknowledgementType.NA;
        private StatusType          _statusType = StatusType.Unknown;
        private ReadReportType      _readType   = ReadReportType.Unknown;
        #endregion

        #region Properties

        /// <summary>
        /// True if the controller is open to communication.
        /// </summary>
        public bool Connected { get { return _connected; } }
        /// <summary>
        /// The data stream to the controller.
        /// </summary>
        public Stream DataStream { get { return _stream; } }
        /// <summary>
        /// The type of controller this has been identified as
        /// </summary>
        public ControllerType Type { get { return _currentType; } }
        /// <summary>
        /// True if no connection has yet been established, and the vendor product code
        /// does not uniquely identify the type of controller (WiiPro vs. Wiimote 0x330)
        /// </summary>
        public bool IsControllerTypeAmbiguous { get; set; }
        /// <summary>
        /// The calibration settings applied to the respective controller types.
        /// </summary>
        public CalibrationStorage StoredCalibrations { get { return _calibrations; } }

        /// <summary>
        /// Gets or Sets the current IR Camera Mode.
        /// (will turn the camera on or off)
        /// </summary>
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
                        case ControllerType.TaikoDrum:
                            // only certian modes can be set
                            if (value == IRCamMode.Off)
                            {
                                _irMode = value;
                                DisableIR();
                            }
                            else if (value != IRCamMode.Full) // we won't use Full
                            {
                                _irMode = value;
                                EnableIR();
                            }
                            break;

                        default:
                            // do nothing, IR usage is invalid
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Gets or Sets the IR Sensitivity Mode.
        /// (Only set if the IR Camera is On)
        /// </summary>
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

        /// <summary>
        /// Gets or Sets the controller's force feedback
        /// </summary>
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

        /// <summary>
        /// Gets or Sets the LED in position 1
        /// </summary>
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
        /// <summary>
        /// Gets or Sets the LED in position 2
        /// </summary>
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
        /// <summary>
        /// Gets or Sets the LED in position 3
        /// </summary>
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
        /// <summary>
        /// Gets or Sets the LED in position 4
        /// </summary>
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
        /// <summary>
        /// The controller's current approximate battery level.
        /// </summary>
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
                    // Calculate the approximate battery level based on the controller type
                    if (_currentType == ControllerType.ProController)
                    {
                        var level = 2f * ((float)_battery - 205f);

                        if (level > 90f)
                            return BatteryStatus.VeryHigh;
                        else if (level > 80f)
                            return BatteryStatus.High;
                        else if (level > 70f)
                            return BatteryStatus.Medium;
                        else if (level > 60f)
                            return BatteryStatus.Low;
                        else
                            return BatteryStatus.VeryLow;
                    }
                    else
                    {
                        var level = 100f * (float)_battery / 192f;

                        if (level > 80f)
                            return BatteryStatus.VeryHigh;
                        else if (level > 60f)
                            return BatteryStatus.High;
                        else if (level > 40f)
                            return BatteryStatus.Medium;
                        else if (level > 20f)
                            return BatteryStatus.Low;
                        else
                            return BatteryStatus.VeryLow;
                    }
                }
            }
        }

        #endregion

        #region Necessities

        /// <summary>
        /// Creates an instance using the provided data stream.
        /// </summary>
        /// <param name="dataStream">Stream to the controller.</param>
        public Nintroller(Stream dataStream)
        {
            _state = null;
            _stream = dataStream;
        }

        /// <summary>
        /// Creates an instance using the provided data stream and expected controller type.
        /// </summary>
        /// <param name="dataStream">Stream to the controller.</param>
        /// <param name="hintType">Expected type of the controller.</param>
        public Nintroller(Stream dataStream, ControllerType hintType) : this(dataStream)
        {
            _currentType = hintType;
            if(hintType==ControllerType.ProController)
            {
                this.IsControllerTypeAmbiguous = true;
            }
        }

        /// <summary>
        /// Creates an instance using the provided data stream and uses the PID to help identify the controller.
        /// </summary>
        /// <param name="dataStream">Stream to the controller.</param>
        /// <param name="pid">The Product ID of the device.</param>
        public Nintroller(Stream dataStream, string pid) : this(dataStream)
        {
            if (pid == Constants.PID3.ToString("X4"))
            {
                _streamSize = Constants.REPORT_LENGTH_GCN;
                _currentType = ControllerType.Other;
                _state = new GameCubeAdapter(true);

                if (_calibrations.GameCubeAdapterCalibration.CalibrationEmpty)
                {
                    _state.SetCalibration(Calibrations.CalibrationPreset.Default);
                }
                else
                {
                    _state.SetCalibration(_calibrations.GameCubeAdapterCalibration);
                }
            }
        }

        /// <summary>
        /// Creates an instance using the provided data stream and uses the PID to help identify the controller.
        /// </summary>
        /// <param name="dataStream">Stream to the controller.</param>
        /// <param name="pid">The Product ID of the device.</param>
        public Nintroller(Stream dataStream, short pid) : this(dataStream, pid.ToString("X4")) { }
        
        /// <summary>
        /// Disposes
        /// </summary>
        public void Dispose()
        {
            StopReading();
            GC.SuppressFinalize(this);

            if (_stream != null)
                _stream.Close();
        }

        internal static void Log(string message)
        {
            #if DEBUG
            Debug.WriteLine(message);
            #endif
        }

        #endregion

        #region Connectivity
        
        /// <summary>
        /// Opens a connection stream to the device.
        /// (Reading is not yet started)
        /// </summary>
        /// <returns>Success</returns>
        [Obsolete("Open the stream instead then use BeginReading instead.")]
        public bool Connect()
        {
            try
            {
                _connected = _stream != null && _stream.CanRead && _stream.CanWrite;
                Log("Connected to device");
            }
            catch (Exception ex)
            {
                Log("Error Connecting to device: " + ex.Message);
            }

            return _connected;
        }
        
        /// <summary>
        /// Closes the connection stream to the device.
        /// </summary>
        [Obsolete("Use StopReading instead and then close the stream.")]
        public void Disconnect()
        {
            _reading = false;

            if (_stream != null)
                _stream.Close();

            _connected = false;

            Log("Disconnected device");
        }

        #endregion

        #region Data Requesting
        /// <summary>
        /// Starts asynchronously recieving data from the device.
        /// </summary>
        public void BeginReading()
        {
            _connected = true;
            
            // kickoff the reading process if it hasn't started already
            if (!_reading && _stream != null)
            {
                _reading = true;
                ReadAsync();
            }
        }

        /// <summary>
        /// Sends a status request to the device.
        /// </summary>
        public void GetStatus()
        {
            byte[] buffer = new byte[2];

            buffer[0] = (byte)OutputReport.StatusRequest;
            buffer[1] = _rumbleBit;

            SendData(buffer);
        }

        /// <summary>
        /// Changes the device's reporting type.
        /// </summary>
        /// <param name="reportType">The report type to set to.</param>
        /// <param name="continuous">If data should be sent repeatingly or only on changes.</param>
        public void SetReportType(InputReport reportType, bool continuous = false)
        {
            if (reportType == InputReport.Acknowledge ||
                reportType == InputReport.ReadMem ||
                reportType == InputReport.Status)
            {
                Log("Can't Set the report type to: " + reportType.ToString());
            }
            else
            {
                ApplyReportingType(reportType, continuous);
            }
        }

        // Performs an asynchronous read
        private void ReadAsync()
        {
            if (_stream != null && _stream.CanRead)
            {
                IAsyncResult ar = null;
                System.Threading.WaitHandle wh = null;

                lock (_readingObj)
                {
                    byte[] readResult = new byte[_streamSize];
                    
                    try
                    {
                        ar = _stream.BeginRead(readResult, 0, readResult.Length, RecieveDataAsync, readResult);
                        wh = ar.AsyncWaitHandle;
                    }
                    catch (ObjectDisposedException)
                    {
                        Log("Can't read, the stream was disposed");
                    }
                    catch (IOException e)
                    {
                        Log("Error Begining Read, is it not connected?");
                        StopReading();
                        Disconnected?.Invoke(this, new DisconnectedEventArgs(e));
                    }
                }

                // Wait 3 seconds for a response in the background
                System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
                {
                    try
                    {
                        if (wh != null && !wh.SafeWaitHandle.IsClosed && !wh.WaitOne(3000))
                        {
                            // If read is not completed send a status report to check connection status
                            GetStatus();
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        Log("Disposed");
                    }
                });
                t.Start();
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

                if (_currentType == ControllerType.Other)
                {
                    ParseOtherReport(result);
                }
                else
                {
                    ParseReport(result);
                }

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
            catch (IOException e)
            {
                Log("IO Error, is the device not connected?");
                if (_reading || _connected)
                {
                    StopReading();
                    Disconnected?.Invoke(this, new DisconnectedEventArgs(e));
                }
            }
        }

        // Request data from the device's memory
        private void ReadMemory(int address, short size)
        {
            byte[] buffer = new byte[7];

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

        // Read calibration from the controller
        private void GetCalibration()
        {
            // TODO: Test (possibly move)
            // don't attempt on Pro Controllers
            ReadMemory(0x0016, 7);
        }

        // Sets the reporting mode type
        private void ApplyReportingType(InputReport reportType, bool continuous = false)
        {
            byte[] buffer = new byte[3];

            buffer[0] = (byte)OutputReport.DataReportMode;
            buffer[1] = (byte)((continuous ? 0x04 : 0x00) | _rumbleBit);
            buffer[2] = (byte)reportType;

            SendData(buffer);
        }
        #endregion

        #region Data Sending
        // sends bytes to the device
        private void SendData(byte[] report)
        {
            if (_currentType == ControllerType.Other)
            {
                Log("Not going to use this method to send Wii data to other devices!");
            }

            if (!_connected)
            {
                Log("Can't Send data, we are not connected!");
                return;
            }
            
            try
            {
                _stream.Write(report, 0, report.Length);
            }
            catch (Exception ex)
            {
                Log("Error while writing to the stream: " + ex.Message);
            }
        }

        // writes bytes to the device's memory
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

            SendData(buffer);
        }

        // set's the device's LEDs and Rumble states
        private void ApplyLEDs()
        {
            byte[] buffer = new byte[2];

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
                    Log(BitConverter.ToString(report));
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
                            // TODO: LED Check - we probably don't want this one
                            //_led1 = (report[3] & 0x10) != 0;
                            //_led2 = (report[3] & 0x20) != 0;
                            //_led3 = (report[3] & 0x40) != 0;
                            //_led4 = (report[3] & 0x80) != 0;

                            // Extension/Type
                            // Not relyable for Pro Controller U
                            bool ext = (report[3] & 0x02) != 0;
                            if (ext || _proControllerUSupport)
                            {
                                //lock (_readingObj)
                                //{
                                    _readType = ReadReportType.Extension_A;
                                    ReadMemory(Constants.REGISTER_EXTENSION_TYPE_2, 1);
                                // 16-04-A4-00-F0-01-55-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00
                                // 16-04-A4-00-FB-01-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00
                                //}
                            }
                            else if (_currentType != ControllerType.Wiimote)
                            {
                                _currentType = ControllerType.Wiimote;
                                _state = new Wiimote();
                                if (_calibrations.WiimoteCalibration.CalibrationEmpty)
                                {
                                    _state.SetCalibration(Calibrations.CalibrationPreset.Default);
                                }
                                else
                                {
                                    _state.SetCalibration(_calibrations.WiimoteCalibration);
                                }
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
                    Log(BitConverter.ToString(report));

                    bool noError = (report[3] & 0xF) == 0;
                    if (!noError)
                        Log("Possible ReadMem Error: " + (report[3] & 0x0F).ToString());

                    switch(_readType)
                    {
                        case ReadReportType.Extension_A:
                            // Initialize
                            lock (_readingObj)
                            {
                                // TODO: Can report[0] ever be equal to 0x04 here? Considering it has to be 0x21 to get here...
                                if (report[0] != 0x04)
                                {
                                    WriteToMemory(Constants.REGISTER_EXTENSION_INIT_1, new byte[] { 0x55 });
                                    WriteToMemory(Constants.REGISTER_EXTENSION_INIT_2, new byte[] { 0x00 });
                                }
                            }

                            _readType = ReadReportType.Extension_B;
                            ReadMemory(Constants.REGISTER_EXTENSION_TYPE, 6);
                            break;

                        case ReadReportType.Extension_B:
                            if (report.Length < 6)
                            {
                                _readType = ReadReportType.Unknown;
                                Log("Report length not long enough for Extension_B");
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

                            bool typeChange = false;
                            ControllerType newType = ControllerType.PartiallyInserted;

                            if (_currentType != (ControllerType)type || _state == null)
                            {
                                typeChange = true;
                                newType = (ControllerType)type;
                            }
                            else if (_forceType != ControllerType.Unknown &&
                                     _forceType != ControllerType.PartiallyInserted &&
                                     _currentType != _forceType)
                            {
                                typeChange = true;
                                newType = _forceType;
                            }

                            if (typeChange)
                            {
                                Log("Controller type: " + newType.ToString());
                                // TODO: Check parsing after applying a report type (Pro is working, CC is not)
                                InputReport applyReport = InputReport.BtnsOnly;
                                bool continuiousReporting = true;

                                switch(newType)
                                {
                                    case ControllerType.Wiimote:
                                        _state = new Wiimote();
                                        if (_calibrations.WiimoteCalibration.CalibrationEmpty)
                                        {
                                            _state.SetCalibration(Calibrations.CalibrationPreset.Default);
                                        }
                                        else
                                        {
                                            _state.SetCalibration(_calibrations.WiimoteCalibration);
                                        }
                                        applyReport = InputReport.BtnsAccIR;
                                        _irMode = IRCamMode.Basic;
                                        EnableIR();
                                        break;

                                    case ControllerType.ProController:
                                        _state = new ProController();
                                        if (_calibrations.ProCalibration.CalibrationEmpty)
                                        {
                                            _state.SetCalibration(Calibrations.CalibrationPreset.Default);
                                        }
                                        else
                                        {
                                            _state.SetCalibration(_calibrations.ProCalibration);
                                        }
                                        applyReport = InputReport.ExtOnly;
                                        break;

                                    case ControllerType.BalanceBoard:
                                        _state = new BalanceBoard();
                                        applyReport = InputReport.ExtOnly;
                                        break;

                                    case ControllerType.Nunchuk:
                                    case ControllerType.NunchukB:
                                        _state = new Nunchuk(_calibrations.WiimoteCalibration);

                                        if (_calibrations.NunchukCalibration.CalibrationEmpty)
                                        {
                                            _state.SetCalibration(Calibrations.CalibrationPreset.Default);
                                        }
                                        else
                                        {
                                            _state.SetCalibration(_calibrations.NunchukCalibration);
                                        }

                                        if (_irMode == IRCamMode.Off)
                                        {
                                            applyReport = InputReport.BtnsAccExt;
                                        }
                                        else
                                        {
                                            applyReport = InputReport.BtnsAccIRExt;
                                        }
                                        break;

                                    case ControllerType.ClassicController:
                                        _state = new ClassicController(_calibrations.WiimoteCalibration);

                                        if (_calibrations.ClassicCalibration.CalibrationEmpty)
                                        {
                                            _state.SetCalibration(Calibrations.CalibrationPreset.Default);
                                        }
                                        else
                                        {
                                            _state.SetCalibration(_calibrations.ClassicCalibration);
                                        }

                                        if (_irMode == IRCamMode.Off)
                                        {
                                            applyReport = InputReport.BtnsExt;
                                        }
                                        else
                                        {
                                            applyReport = InputReport.BtnsAccIRExt;
                                        }
                                        break;

                                    case ControllerType.ClassicControllerPro:
                                        _state = new ClassicControllerPro(_calibrations.WiimoteCalibration);

                                        if (_calibrations.ClassicProCalibration.CalibrationEmpty)
                                        {
                                            _state.SetCalibration(Calibrations.CalibrationPreset.Default);
                                        }
                                        else
                                        {
                                            _state.SetCalibration(_calibrations.ClassicProCalibration);
                                        }

                                        if (_irMode == IRCamMode.Off)
                                        {
                                            applyReport = InputReport.BtnsAccExt;
                                        }
                                        else
                                        {
                                            applyReport = InputReport.BtnsAccIRExt;
                                        }
                                        break;

                                    case ControllerType.MotionPlus:
                                        _state = new WiimotePlus();
                                        // TODO: Calibration: apply stored motion plus calibration
                                        if (_irMode == IRCamMode.Off)
                                        {
                                            applyReport = InputReport.BtnsAccExt;
                                        }
                                        else
                                        {
                                            applyReport = InputReport.BtnsAccIRExt;
                                        }
                                        break;

                                    case ControllerType.MotionPlusNunchuk:
                                    case ControllerType.MotionPlusCC:
                                        // TODO: Add Motion Plus support
                                        Log("Unsupported controller type");
                                        break;

                                    case ControllerType.Guitar:
                                        _state = new Guitar();
                                        if (_calibrations.GuitarCalibration.CalibrationEmpty)
                                        {
                                            _state.SetCalibration(Calibrations.CalibrationPreset.Default);
                                        }
                                        else
                                        {
                                            _state.SetCalibration(_calibrations.GuitarCalibration);
                                        }
                                        applyReport = InputReport.BtnsAccExt;
                                        break;

                                    case ControllerType.TaikoDrum:
                                        _state = new TaikoDrum();
                                        if (_irMode == IRCamMode.Off)
                                        {
                                            applyReport = InputReport.BtnsAccExt;
                                        }
                                        else
                                        {
                                            applyReport = InputReport.BtnsAccIRExt;
                                        }
                                        break;

                                    case ControllerType.Drums:
                                    case ControllerType.TurnTable:
                                        // TODO: More control types
                                        Log("Unsupported controller type");
                                        break;

                                    case ControllerType.PartiallyInserted:
                                        // try again
                                        GetStatus();
                                        return;

                                    default:
                                        Log("Unhandled controller type");
                                        break;
                                }

                                _currentType = newType;

                                // TODO: Get calibration if PID != 330

                                //_state.SetCalibration(Calibrations.CalibrationPreset.Default);

                                // Fire ExtensionChange event
                                //ExtensionChange(this, _currentType);
                                ExtensionChange(this, new NintrollerExtensionEventArgs(_currentType));
                                
                                // set Report
                                ApplyReportingType(applyReport, continuiousReporting);
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

                    if (report[4] == 0x03)
                    {
                        Log("Possible Error with Operation");
                        Log(BitConverter.ToString(report));
                        return;
                    }

                    switch (_ackType)
                    {
                        case AcknowledgementType.NA:
                            #region Default Acknowledgement
                            Log("Acknowledgement Report");
                            Log(BitConverter.ToString(report));
                            // Core buttons can be parsed here
                            // 20 BB BB LF 00 00 VV
                            // 20 = Acknowledgement Report
                            // BB BB = Core Buttons
                            // LF = LED Status & Flags
                            //     0x01 = Battery very low
                            //     0x02 = Extension connected
                            //     0x04 = Speaker enabled
                            //     0x08 = IR camera enabled
                            //     0x10 = LED 1
                            //     0x20 = LED 2
                            //     0x40 = LED 3
                            //     0x80 = LED 4
                            // VV = current battery level

                            // Gather Flags
                            _batteryLow    = (report[3] & 0x01) == 1;
                            bool extension = (report[3] & 0x02) == 1;
                            bool speaker   = (report[3] & 0x04) == 1;
                            bool irOn      = (report[3] & 0x08) == 1;
                            
                            // Gather LEDs
                            // TODO: LED Check - we may want this one
                            //_led1 = (report[3] & 0x10) == 1;
                            //_led2 = (report[3] & 0x20) == 1;
                            //_led3 = (report[3] & 0x40) == 1;
                            //_led4 = (report[3] & 0x80) == 1;

                            //if (extension)
                            //{
                            //    _readType = ReadReportType.Extension_A;
                            //    ReadMemory(Constants.REGISTER_EXTENSION_TYPE_2, 1);
                            //}
                            //else if (_currentType != ControllerType.Wiimote)
                            //{
                            //    _currentType = ControllerType.Wiimote;
                            //    _state = new Wiimote();
                            //    _state.Update(report);
                            //
                            //    // Fire event
                            //    ExtensionChange(this, new NintrollerExtensionEventArgs(_currentType));
                            //
                            //    // and set report
                            //    SetReportType(InputReport.BtnsAccIR);
                            //}
                            #endregion
                            break;

                        case AcknowledgementType.IR_Step1:
                            #region IR Step 1
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
                            #endregion
                            break;

                        case AcknowledgementType.IR_Step2:
                            #region IR Step 2
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
                            #endregion
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
                            #region Final IR Step
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
                            #endregion
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

                        try
                        {
                            // got an access violation here once
                            //StateUpdate(this, arg);

                            // let's try not including the sender
                            StateUpdate(null, arg);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Debug.WriteLine("State Update Exception: " + ex.Message);
#endif
                        }
                    }
                    break;
                #endregion

                default:
                    Log("Unexpected Report type: " + input.ToString("x"));
                    break;
            }
        }

        private void ParseOtherReport(byte[] report)
        {
            _state.Update(report);
            var arg = new NintrollerStateEventArgs(_currentType, _state, BatteryLevel);
            StateUpdate?.Invoke(null, arg);
         }

        #endregion

        #region General

        internal static float Normalize(int raw, int min, int center, int max)
        {
            if (raw == center) return 0;

            float actual = raw - center;
            float range = 0;

            if (raw > center)
            {
                range = max - center;
            }
            else
            {
                range = center - min;
            }

            if (range == 0) return 0;

            return actual / range;
        }
        
        internal static float Normalize(int raw, int min, int center, int max, int dead)
        {
            float actual = 0;
            float range = 0;

            if (Math.Abs(center - raw) <= dead)
            {
                return 0f;
            }
            else
            {
                if (raw > center)
                {
                    actual = raw - (center + dead);
                    range = max - (center + dead);
                }
                else if (raw < center)
                {
                    actual = raw - (center - dead);
                    range = (center - dead) - min;
                }
            }

            if (range == 0)
                return 0f;

            return actual / range;
        }

        internal static float Normalize(int raw, int min, int center, int max, int deadP, int deadN)
        {
            float actual = 0;
            float range = 0;
            
            if (raw - center <= deadP && raw - center >= deadN)
            {
                return 0f;
            }
            else
            {
                if (raw > center)
                {
                    actual = raw - (center + deadP);
                    range = max - (center + deadP);
                }
                else if (raw < center)
                {
                    actual = raw - deadN - center;
                    range = (center + deadN) - min;
                }
            }

            if (range == 0)
                return 0f;

            return actual / range;
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
            byte[] buffer = new byte[2];
            
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
            byte[] buffer = new byte[2];
            
            buffer[0] = (byte)OutputReport.IREnable;
            buffer[1] = (byte)(0x00);
            SendData(buffer);

            buffer[0] = (byte)OutputReport.IREnable2;
            buffer[1] = (byte)(0x00);
            SendData(buffer);

            // TODO: Check if we need to monitor the acknowledgment report
        }

        private void StartMotionPlus()
        {
            // TODO: Motion Plus
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

        /// <summary>
        /// Forces the controller to be read as the provided type.
        /// </summary>
        /// <param name="type">Type to be parsed as. Setting it to Unknown or Partially Inserted clears it.</param>
        public void ForceControllerType(ControllerType type)
        {
            _forceType = type;

            if (_connected)
            {
                GetStatus();
            }
        }

        /// <summary>
        /// Stop reading from the device
        /// </summary>
        public void StopReading()
        {
            _reading = false;
            _connected = false;
        }

        #endregion

        #region Calibration

        /// <summary>
        /// Sets the device's calibrations based on a preset.
        /// </summary>
        /// <param name="preset">Preset to be used.</param>
        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            if (_state != null)
                _state.SetCalibration(preset);

            if (_calibrations != null)
                _calibrations.SetCalibrations(preset);
        }
        /// <summary>
        /// Sets the device's calibrations based on a string.
        /// </summary>
        /// <param name="calibrationStorageString">Calibration storage string to use.</param>
        public void SetCalibration(string calibrationStorageString)
        {
            _calibrations.SetCalibrations(calibrationStorageString);
            
            // TODO: apply 
        }
        /// <summary>
        /// Sets the controller calibration for the Wiimote
        /// </summary>
        /// <param name="wiimoteCalibration">The Wiimote Struct with the calibration values to use</param>
        public void SetCalibration(Wiimote wiimoteCalibration)
        {
            _calibrations.WiimoteCalibration = wiimoteCalibration;

            if (_state != null &&(
                _currentType == ControllerType.Wiimote || 
                _currentType == ControllerType.Nunchuk || 
                _currentType == ControllerType.NunchukB ||
                _currentType == ControllerType.ClassicController || 
                _currentType == ControllerType.ClassicControllerPro) ||
                _currentType == ControllerType.Guitar)
            {
                _state.SetCalibration(wiimoteCalibration);
            }
        }
        /// <summary>
        /// Sets the controller calibration for the Nunchuk
        /// </summary>
        /// <param name="nunchukCalibration">The Nunchuk Struct with the calibration values to use</param>
        public void SetCalibration(Nunchuk nunchukCalibration)
        {
            _calibrations.NunchukCalibration = nunchukCalibration;

            if (_currentType == ControllerType.Nunchuk || _currentType == ControllerType.NunchukB)
            {
                _state.SetCalibration(nunchukCalibration);
            }
        }
        /// <summary>
        /// Sets the controller calibration for the Classic Controller
        /// </summary>
        /// <param name="classicCalibration">The ClassicController Struct with the calibration values to use</param>
        public void SetCalibration(ClassicController classicCalibration)
        {
            _calibrations.ClassicCalibration = classicCalibration;

            if (_currentType == ControllerType.ClassicController)
            {
                _state.SetCalibration(classicCalibration);
            }
        }
        /// <summary>
        /// Sets the controller calibration for the Classic Controller Pro
        /// </summary>
        /// <param name="classicProCalibration">The ClassicControllerPro Struct with the calibration values to use</param>
        public void SetCalibration(ClassicControllerPro classicProCalibration)
        {
            _calibrations.ClassicProCalibration = classicProCalibration;

            if (_currentType == ControllerType.ClassicControllerPro)
            {
                _state.SetCalibration(classicProCalibration);
            }
        }
        /// <summary>
        /// Sets the controller calibration for the Pro Controller
        /// </summary>
        /// <param name="proCalibration">The ProController Struct with the calibration values to use</param>
        public void SetCalibration(ProController proCalibration)
        {
            _calibrations.ProCalibration = proCalibration;

            if (_state != null && _currentType == ControllerType.ProController)
            {
                _state.SetCalibration(proCalibration);
            }
        }
        public void SetCalibration(Guitar gutCalibration)
        {
            _calibrations.GuitarCalibration = gutCalibration;

            if (_state != null && _currentType == ControllerType.Guitar)
            {
                _state.SetCalibration(gutCalibration);
            }
        }
        /// <summary>
        /// Sets the calibrations for the GameCube Adapter
        /// </summary>
        /// <param name="gcnCalibration">The GameCubeAdapter Struct with the calibration values to use</param>
        public void SetCalibration(GameCubeAdapter gcnCalibration)
        {
            _calibrations.GameCubeAdapterCalibration = gcnCalibration;

            if (_state != null && _currentType == ControllerType.Other)
            {
                _state.SetCalibration(gcnCalibration);
            }
        }


        #endregion
    }

    #region New Event Args

    /// <summary>
    /// Class for controller state update.
    /// </summary>
    public class NintrollerStateEventArgs : EventArgs
    {
        /// <summary>
        /// The controller type being updated.
        /// </summary>
        public ControllerType controllerType;
        /// <summary>
        /// The controller's updated state.
        /// </summary>
        public INintrollerState state;
        /// <summary>
        /// The controller's last known battery level.
        /// </summary>
        public BatteryStatus batteryLevel;
        
        /// <summary>
        /// Create the event argument with provided parameters.
        /// </summary>
        /// <param name="type">The controller type.</param>
        /// <param name="newState">The updated controller state.</param>
        /// <param name="battery">The controller's last known battery level.</param>
        public NintrollerStateEventArgs(ControllerType type, INintrollerState newState, BatteryStatus battery)
        {
            controllerType = type;
            state          = newState;
            batteryLevel   = battery;
        }
    }

    /// <summary>
    /// Class for extension change event.
    /// </summary>
    public class NintrollerExtensionEventArgs : EventArgs
    {
        /// <summary>
        /// The updated controller type.
        /// </summary>
        public ControllerType controllerType;

        /// <summary>
        /// Create an instance with the provided new type.
        /// </summary>
        /// <param name="type">The new type of the controller</param>
        public NintrollerExtensionEventArgs(ControllerType type)
        {
            controllerType = type;
        }
    }

    /// <summary>
    /// Class for low battery event.
    /// </summary>
    public class LowBatteryEventArgs : EventArgs
    {
        /// <summary>
        /// The current battery level of the controller.
        /// </summary>
        public BatteryStatus batteryLevel;

        /// <summary>
        /// Create an instance with the changed battery level.
        /// </summary>
        /// <param name="level">Current battery level of the controller.</param>
        public LowBatteryEventArgs(BatteryStatus level)
        {
            batteryLevel = level;
        }
    }

    /// <summary>
    /// Class for disconnected event.
    /// </summary>
    public class DisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Exception thrown if applicable.
        /// </summary>
        public Exception error;

        /// <summary>
        /// Creates instance using provided exception.
        /// </summary>
        /// <param name="err">Exception thrown when disconnect detected.</param>
        public DisconnectedEventArgs(Exception err)
        {
            error = err;
        }
    }

    #endregion

}