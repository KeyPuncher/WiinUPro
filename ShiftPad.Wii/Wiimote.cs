using ShiftPad.Core.Gamepad;
using ShiftPad.Core.Utility;
using ShiftPad.Wii.Communication;

namespace ShiftPad.Wii
{
    public class Wiimote : IGamepad
    {
        private const int REPORT_LENGTH = 22;
        private const int MEMORY_READ_LENGTH = 7;
        private const int ACKNOWLEDGEMENT_SUCCESS = 0x00;
        private const int ACKNOWLEDGEMENT_ERROR = 0x03;
        private const int REGISTER_EXTENSION_TYPE_1 = 0x04a400fa;
        private const int REGISTER_EXTENSION_TYPE_1_LENGTH = 6;
        private const int REGISTER_EXTENSION_TYPE_2 = 0x04a400fe;
        private const int REGISTER_EXTENSION_TYPE_2_LENGTH = 1;
        private const int REGISTER_EXTENSION_INIT_1 = 0x04a400f0;
        private const int REGISTER_EXTENSION_INIT_2 = 0x04a400fb;

        public bool AlwaysCheckExtension { get; set; } = false;
        public bool InfaredCameraEnabled => false; // TODO

        private Stream _dataStream;
        private ContinuoursReader _reader;
        private byte _rumbleByte = 0x00;
        private byte _battery = 0x00; // TODO: Battery Class

        private ResponseBuffer<InputReport, byte[]> _responseBuffer;
        private Logger _logger = Logger.GetInstance(typeof(Wiimote));

        private bool ConnectedOrConnecting => _connectionStatus == ConnectionStatus.Connecting || _connectionStatus == ConnectionStatus.Connected;

        public Wiimote(Stream dataStream)
        {
            _dataStream = dataStream;
            _reader = new ContinuoursReader(dataStream, REPORT_LENGTH, ReadCallback);
            _responseBuffer = new ResponseBuffer<InputReport, byte[]>();
        }

        private void ReadCallback(byte[] data)
        {
            _logger.Log(data);
            OnRawUpdate?.Invoke(this, data);

            InputReport reportyType = (InputReport)data[0];

            // Check if this is an acknowledgement.
            if (reportyType == InputReport.Acknowledge)
            {
                var acknowledgedReport = (InputReport)data[3];
                if (_responseBuffer.SetResponse(acknowledgedReport, data))
                {
                    return;
                }
            }

            // Check if response is being waited on.
            if (_responseBuffer.SetResponse(reportyType, data))
            {
                return;
            }

            if (_connectionStatus == ConnectionStatus.Connected)
            {
                //
            }
            else if (_connectionStatus == ConnectionStatus.Connecting)
            {
                ProcessConnectingReport(data);
            }
        }

        private AcknowledgementErrorCode AcknowledgementCheck(byte[] data)
        {
            return (AcknowledgementErrorCode)data[4];
        }

        private void ProcessConnectingReport(byte[] data)
        {
            //
        }

        public async Task<bool> StatusReport()
        {
            byte[] buffer = new byte[2];
            var response = _responseBuffer.MakeRequest(InputReport.Status);
            await WriteReport(OutputReport.StatusRequest, buffer);
            var result = await response;

            if (!result.success)
            {
                return false;
            }

            var status = result.value;

            _battery = status[6];
            bool lowBattery = (status[3] & 0x01) != 0;
            if (lowBattery)
            {
                _bateryStatus = BateryStatus.Low;
            }

            // Extention not always detected for some controllers (Pro Controller U)
            bool extensionDetected = AlwaysCheckExtension || (status[3] & 0x02) != 0;
            if (extensionDetected)
            {
                var extensionReport = await MemoryRead(REGISTER_EXTENSION_TYPE_2, REGISTER_EXTENSION_TYPE_2_LENGTH);
                if (extensionReport == null)
                {
                    return false;
                }

                if (!await MemoryWrite(REGISTER_EXTENSION_INIT_1, [0x55]))
                {
                    return false;
                }

                if (!await MemoryWrite(REGISTER_EXTENSION_INIT_2, [0x00]))
                {
                    return false;
                }

                extensionReport = await MemoryRead(REGISTER_EXTENSION_TYPE_1, REGISTER_EXTENSION_TYPE_1_LENGTH);

                if (extensionReport == null)
                {
                    return false;
                }

                long typeBytes =
                    (extensionReport[6] << 40) |
                    (extensionReport[7] << 32) |
                    (extensionReport[8] << 24) |
                    (extensionReport[9] << 16) |
                    (extensionReport[10] << 8) |
                    (extensionReport[11]);

                WiiExtensionType controllerType = (WiiExtensionType)typeBytes;

                _logger.LogInfo($"Extension Type {controllerType}");

                SetControllerType(controllerType);
            }
            else
            {
                // setup as wiimote
            }

            return true;
        }

        public void SetControllerType(WiiExtensionType extensionType)
        {
            var reportType = GetReportType(extensionType);
        }

        private InputReport GetReportType(WiiExtensionType extensionType)
        {
            switch (extensionType)
            {
                case WiiExtensionType.Wiimote:
                    return InfaredCameraEnabled ? InputReport.BtnsAccIR : InputReport.BtnsAcc;
                
                case WiiExtensionType.Nunchuk:
                case WiiExtensionType.NunchukB:
                case WiiExtensionType.MotionPlus:
                case WiiExtensionType.MotionPlusNunchuk:
                case WiiExtensionType.MotionPlusNunchukB:
                case WiiExtensionType.TaikoDrum:
                    return InfaredCameraEnabled ? InputReport.BtnsAccIRExt : InputReport.BtnsAccExt;

                case WiiExtensionType.ClassicController:
                case WiiExtensionType.ClassicControllerPro:
                    return InfaredCameraEnabled ? InputReport.BtnsIRExt : InputReport.BtnsExt;

                case WiiExtensionType.Guitar:
                case WiiExtensionType.Drums:
                case WiiExtensionType.TurnTable:
                case WiiExtensionType.DrawTablet:
                    return InputReport.BtnsAccExt;

                case WiiExtensionType.BalanceBoard:
                    return InputReport.ExtOnly;

                case WiiExtensionType.PartiallyInserted:
                default:
                    _logger.LogInfo($"Unhandled extenstion type {(long)extensionType:X12}");
                    return InputReport.BtnsOnly;
            }
        }


        #region Device Memory Access
        private byte[] PrepareMemoryBuffer(int address, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            buffer[1] = (byte)(0xFF & (address >> 24));
            buffer[2] = (byte)(0xFF & (address >> 16));
            buffer[3] = (byte)(0xFF & (address >> 8));
            buffer[4] = (byte)(0xFF & address);
            return buffer;
        }

        private async Task<byte[]> MemoryRead(byte[] report)
        {
            var response = _responseBuffer.MakeRequest(InputReport.ReadMemory);
            await WriteReport(OutputReport.ReadMemory, report);
            var result = await response;
            
            if (result.success)
            {
                return result.value;
            }

            return new byte[REPORT_LENGTH];
        }

        private async Task<byte[]> MemoryRead(int address, short size)
        {
            var buffer = PrepareMemoryBuffer(address, MEMORY_READ_LENGTH);
            buffer[5] = (byte)(0xFF & (size >> 8));
            buffer[6] = (byte)(0xFF & size);
            return await MemoryRead(buffer);
        }

        private async Task<byte[]> MemoryRead(int address, byte[] data)
        {
            var buffer = PrepareMemoryBuffer(address, REPORT_LENGTH);
            buffer[5] = (byte)data.Length;
            Array.Copy(data, 0, buffer, 6, Math.Min(data.Length, REPORT_LENGTH - 6));
            return await MemoryRead(buffer);
        }

        private async Task<bool> MemoryWrite(int address, byte[] data)
        {
            var buffer = PrepareMemoryBuffer(address, REPORT_LENGTH);
            buffer[5] = (byte)data.Length;
            Array.Copy(data, 0, buffer, 6, Math.Min(data.Length, REPORT_LENGTH - 6));

            var response = _responseBuffer.MakeRequest(InputReport.Acknowledge);
            await WriteReport(OutputReport.WriteMemory, buffer);

            var result = await response;
            return result.success && (AcknowledgementCheck(result.value) == AcknowledgementErrorCode.Success);
        }
        #endregion

        #region Writing
        private async Task WriteReport(OutputReport reportType, byte[] report)
        {
            report[0] = (byte)reportType;
            report[1] |= _rumbleByte;
            await _dataStream.WriteAsync(report);
        }
        #endregion

        #region IGamepad Implementation
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public uint Type => Classifications.GAMEPAD_TYPE_WII;
        public ulong SubType => _subType;
        public ConnectionStatus Connection => _connectionStatus;
        public IGamepadCalibration Calibration => _calibration;
        public BateryStatus Bettery => _bateryStatus;

        public event Action<IGamepad, IGamepadState> OnStateUpdate;
        public event Action<IGamepad> OnDisconnected;
        public event Action<IGamepad, BateryStatus> OnBatteryStatusChange;
        public event Action<IGamepad, byte[]> OnRawUpdate;

        private string _name;
        private ulong _subType = Classifications.Wiimote;
        private ConnectionStatus _connectionStatus;
        private IGamepadCalibration _calibration;
        private BateryStatus _bateryStatus;

        public async Task<bool> Connect()
        {
            if (_dataStream == null || !_dataStream.CanRead || !_dataStream.CanWrite)
            {
                return false;
            }

            _connectionStatus = ConnectionStatus.Connecting;

            await _reader.StartReading();

            if (!await StatusReport())
            {
                _connectionStatus = ConnectionStatus.Discovered;
                return false;
            }
            
            // TODO: Check if now ready

            _connectionStatus = ConnectionStatus.Connected;
            return true;
        }

        public Task Disconnect()
        {
            _reader.StopReading();
            _responseBuffer.Cancel();
            _connectionStatus = ConnectionStatus.Disconnected;
            _responseBuffer = new ResponseBuffer<InputReport, byte[]>();
            return Task.CompletedTask;
        }

        public Task SetCalibration(IGamepadCalibration calibration)
        {
            throw new NotImplementedException();
        }

        public async Task StartReading()
        {
            if (_connectionStatus == ConnectionStatus.Connected)
                await _reader.StartReading();
        }

        public Task StopReading()
        {
            _reader.StopReading();
            return Task.CompletedTask;
        }
        #endregion
    }
}
