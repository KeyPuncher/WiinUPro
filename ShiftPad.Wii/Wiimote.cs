using ShiftPad.Core.Gamepad;
using ShiftPad.Core.Utility;
using ShiftPad.Wii.Communication;
using System;
using System.Collections.Concurrent;

namespace ShiftPad.Wii
{
    public class Wiimote : IGamepad
    {
        private const int REPORT_LENGTH = 22;
        private const int MEMORY_READ_LENGTH = 7;
        private const int ACKNOWLEDGEMENT_SUCCESS = 0x00;
        private const int ACKNOWLEDGEMENT_ERROR = 0x03;
        private const int REGISTER_EXTENSION_TYPE_2 = 0x04a400fe;
        private const int REGISTER_EXTENSION_TYPE_2_LENGTH = 1;

        public bool AlwaysCheckExtension { get; set; } = false;

        private Stream _dataStream;
        private ContinuoursReader _reader;
        private byte _rumbleByte = 0x00;
        private byte _battery = 0x00; // TODO: Battery Class

        private ConcurrentDictionary<InputReport, byte[]> _responseReports;
        private Logger _logger = Logger.GetInstance(typeof(Wiimote));

        private bool ConnectedOrConnecting => _connectionStatus == ConnectionStatus.Connecting || _connectionStatus == ConnectionStatus.Connected;

        public Wiimote(Stream dataStream)
        {
            _dataStream = dataStream;
            _reader = new ContinuoursReader(dataStream, REPORT_LENGTH, ReadCallback);
            _responseReports = new ConcurrentDictionary<InputReport, byte[]>();
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
                if (_responseReports.TryUpdate(acknowledgedReport, data, null))
                {
                    return;
                }
            }

            // Check if response is being waited on.
            if (_responseReports.TryUpdate(reportyType, data, null))
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

        private async Task<byte[]> ResponseRequest(InputReport report)
        {
            // Yield if another action is waiting the same report.
            while (_responseReports.ContainsKey(report))
            {
                await Task.Yield();
                if (!ConnectedOrConnecting)
                {
                    _responseReports.Remove(report, out _);
                    return new byte[REPORT_LENGTH];
                }
            }

            byte[] bytes;
            while (!_responseReports.TryGetValue(report, out bytes) || bytes == null)
            {
                await Task.Yield();
                if (!ConnectedOrConnecting)
                {
                    _responseReports.Remove(report, out _);
                    return new byte[REPORT_LENGTH];
                }
            }

            _ = _responseReports.Remove(report, out _);
            return bytes;
        }

        public async Task StatusReport()
        {
            byte[] buffer = new byte[2];
            var response = ResponseRequest(InputReport.Status);
            await WriteReport(OutputReport.StatusRequest, buffer);
            var status = await response;

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
                
            }
            else
            {
                // setup as wiimote
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
            var response = ResponseRequest(InputReport.ReadMemory);
            await WriteReport(OutputReport.ReadMemory, report);
            return await response;
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

        private async void MemoryWrite(int address, byte[] data)
        {
            var buffer = PrepareMemoryBuffer(address, REPORT_LENGTH);
            buffer[5] = (byte)data.Length;
            Array.Copy(data, 0, buffer, 6, Math.Min(data.Length, REPORT_LENGTH - 6));
            await WriteReport(OutputReport.WriteMemory, buffer);
            // TODO: This will be a report acknowledgement
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
            await StatusReport();

            // TODO: Check if now ready

            _connectionStatus = ConnectionStatus.Connected;
            return true;
        }

        public Task Disconnect()
        {
            _reader.StopReading();
            _connectionStatus = ConnectionStatus.Disconnected;
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
