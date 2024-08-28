using ShiftPad.Core.Gamepad;
using ShiftPad.Core.Utility;
using System;
using System.Collections.Generic;

namespace ShiftPad.Wii
{
    public class Wiimote : IGamepad
    {
        private const int REPORT_LENGTH = 22;

        private Stream _dataStream;
        private ContinuoursReader _reader;

        private Logger _logger = Logger.GetInstance(typeof(Wiimote));

        public Wiimote(Stream dataStream)
        {
            _dataStream = dataStream;
            _reader = new ContinuoursReader(dataStream, REPORT_LENGTH, ReadCallback);
        }

        private void ReadCallback(byte[] data)
        {
            _logger.Log(data);
            OnRawUpdate?.Invoke(this, data);
        }

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
            if (_dataStream == null || !_dataStream.CanRead)
            {
                return false;
            }

            await _reader.StartReading();
            _connectionStatus = ConnectionStatus.Connected;

            return true;
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task SetCalibration(IGamepadCalibration calibration)
        {
            throw new NotImplementedException();
        }

        public Task StartReading()
        {
            throw new NotImplementedException();
        }

        public Task StopReading()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
