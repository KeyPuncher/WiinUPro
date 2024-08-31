using ShiftPad.Core.Battery;

namespace ShiftPad.Core.Gamepad
{
    public interface IGamepad
    {
        /// <summary>
        /// The name or nickname of the gamepad.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The class of gamepad (fixed).
        /// </summary>
        public uint Type { get; }
        /// <summary>
        /// The specific type of this gamepad (variable).
        /// </summary>
        public ulong SubType { get; }
        /// <summary>
        /// Denotes if the gamepad is open for communication.
        /// </summary>
        public ConnectionStatus Connection { get; }
        /// <summary>
        /// The calibration setting for this gamepad.
        /// </summary>
        public IGamepadCalibration Calibration { get; }
        /// <summary>
        /// Information about the gamepad's battery.
        /// </summary>
        public IBateryState Bettery { get; }

        /// <summary>
        /// Fires when the gamepad's state changes.
        /// </summary>
        public event Action<IGamepad, IGamepadState> OnStateUpdate;
        /// <summary>
        /// Fires when the gamepad is unexpectedly disconnected.
        /// </summary>
        public event Action<IGamepad> OnDisconnected;
        /// <summary>
        /// Fires when the battery amount croshes a threshold.
        /// </summary>
        public event Action<IGamepad, BateryStatus> OnBatteryStatusChange;
        /// <summary>
        /// Fires with gamepad's state changes, called with the raw byte array.
        /// </summary>
        public event Action<IGamepad, byte[]> OnRawUpdate;

        /// <summary>
        /// Initializes the gamepad for connection.
        /// </summary>
        /// <returns>True if successful.</returns>
        public Task<bool> Connect();
        /// <summary>
        /// Ends communication with the gamepad.
        /// </summary>
        public Task Disconnect();
        /// <summary>
        /// Begins polling the gamepad for input changes.
        /// </summary>
        public Task StartReading();
        /// <summary>
        /// Ends polling the gamepad for input changes.
        /// </summary>
        public Task StopReading();
        /// <summary>
        /// Applies the given calibration to the gamepad.
        /// </summary>
        /// <param name="calibration">The gamepad calibration to apply.</param>
        public Task SetCalibration(IGamepadCalibration calibration);
    }
}
