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

        private Stream _dataStream;
        private ContinuoursReader _reader;
        private byte _rumbleByte = 0x00;

        private ConcurrentDictionary<InputReport, byte[]> _responseReports;
        private Logger _logger = Logger.GetInstance(typeof(Wiimote));

        private bool ConnectedOrConnecting => _connectionStatus == ConnectionStatus.Connecting || _connectionStatus == ConnectionStatus.Connected;

        public Wiimote(Stream dataStream)
        {
            _dataStream = dataStream;
            _reader = new ContinuoursReader(dataStream, REPORT_LENGTH, ReadCallback);
            _responseReports = new ConcurrentDictionary<InputReport, byte[]>();
            _responseReports[InputReport.ReadMemory] = null;
        }

        private void ReadCallback(byte[] data)
        {
            _logger.Log(data);
            OnRawUpdate?.Invoke(this, data);

            InputReport reportyType = (InputReport)data[0];
            if (_responseReports.ContainsKey(reportyType))
            {
                _responseReports[reportyType] = data;
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

        private void ProcessConnectingReport(byte[] data)
        {
            //
        }

        private async Task<byte[]> GetResponse(InputReport report)
        {
            byte[] bytes;
            while (!_responseReports.TryGetValue(report, out bytes) || bytes == null)
            {
                await Task.Yield();
                if (!ConnectedOrConnecting)
                {
                    return new byte[REPORT_LENGTH];
                }
            }

            return bytes;
        }

        public async Task StatusReport()
        {
            byte[] buffer = new byte[2];
            await WriteReport(OutputReport.StatusRequest, buffer);
        }

        #region Reading Device Memory
        private byte[] PrepareReadMemoryBuffer(int address, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            buffer[1] = (byte)(0xFF & (address >> 24));
            buffer[2] = (byte)(0xFF & (address >> 16));
            buffer[3] = (byte)(0xFF & (address >> 8));
            buffer[4] = (byte)(0xFF & address);
            return buffer;
        }

        public async Task<byte[]> MemoryRead(byte[] report)
        {
            await WriteReport(OutputReport.ReadMemory, report);
            return await GetResponse(InputReport.ReadMemory);
        }

        public async Task<byte[]> MemoryRead(int address, short size)
        {
            var buffer = PrepareReadMemoryBuffer(address, MEMORY_READ_LENGTH);
            buffer[5] = (byte)(0xFF & (size >> 8));
            buffer[6] = (byte)(0xFF & size);
            return await MemoryRead(buffer);
        }

        public async Task<byte[]> MemoryRead(int address, byte[] data)
        {
            var buffer = PrepareReadMemoryBuffer(address, REPORT_LENGTH);
            buffer[5] = (byte)data.Length;
            Array.Copy(data, 0, buffer, 6, Math.Min(data.Length, REPORT_LENGTH - 6));
            return await MemoryRead(buffer);
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
