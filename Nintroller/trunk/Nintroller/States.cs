using System;

namespace NintrollerLib
{

    #region Super Classes
    /// <summary>
    /// Base controller state class
    /// </summary>
    [Serializable]
    public class NintyState
    {
        /// <summary>
        /// State of LED 1
        /// </summary>
        public bool LED1 { get { return led1; } }
        /// <summary>
        /// State of LED 2
        /// </summary>
        public bool LED2 { get { return led2; } }
        /// <summary>
        /// State of LED 3
        /// </summary>
        public bool LED3 { get { return led3; } }
        /// <summary>
        /// State of LED 4
        /// </summary>
        public bool LED4 { get { return led4; } }
        /// <summary>
        /// Indicates if the battery is nearly depleted.
        /// </summary>
        public bool BatteryLow { get { return lowBattery; } }
        /// <summary>
        /// The raw battery information.
        /// </summary>
        public byte BatteryRaw { get { return batteryRaw; } }
        /// <summary>
        /// The calculated battery level.
        /// (for devices that can be calcualted for)
        /// </summary>
        public float BatteryLevel { get { return batteryLevel; } }

        /// TODO: Make decision
        //public type RawState;         // Full set of bytes from the device
        public BatteryStatus Battery;   // State of the battery

        // Variables available to all devices
        internal byte batteryRaw;       // raw byte level of the battery
        internal float batteryLevel;    // calculated battery level
        internal bool lowBattery;       // the point when we would warn the user
        internal bool hasLEDs;          // Does this device display the LEDs?
        internal bool led1, led2, led3, led4;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NintyState()
        {

        }

        /// <summary>
        /// Gets the report type for the device.
        /// </summary>
        /// <returns>Recommended report type.</returns>
        public virtual InputReport GetReportType()
        {
            return InputReport.BtnsOnly;
        }

        /// <summary>
        /// Get the rumble state.
        /// </summary>
        /// <returns>Rumble On/Off</returns>
        public virtual bool GetRumble()
        {
            return false;
        }

        /// <summary>
        /// Sets the rumble state of the device.
        /// </summary>
        /// <param name="enable">On/Off</param>
        public virtual void SetRumble(bool enable)
        {
            return;
        }

        /// <summary>
        /// Resets the controller's calibration settings.
        /// </summary>
        public virtual void ResetCalibration()
        {
            return;
        }

        // Calculate the battery levels
        internal virtual void UpdateBattery()
        {
            return;
        }

        // Parses the report
        internal virtual void ParseReport(byte[] r)
        {
            return;
        }

        public static float NormalizeAxisValue(int rawValue, JoyCalibration calibration, int deadzoneValue)
        {
            float availableRange = 0f;
            float actualValue = 0f;

            // Check if it should snap to 0
            if (Math.Abs(calibration.Mid - rawValue) < deadzoneValue)
            {
                return 0f;
            }
            else if (rawValue - calibration.Mid > 0)
            {
                // Positive Axis Calculation
                availableRange = calibration.Max - (calibration.Mid + deadzoneValue);
                actualValue = rawValue - (calibration.Mid + deadzoneValue);

                return (actualValue / availableRange);
            }
            else
            {
                // Negative Axis Calculation
                availableRange = calibration.Mid - deadzoneValue - calibration.Min;
                actualValue = rawValue - calibration.Min;

                return (actualValue / availableRange) - 1f;
            }
        }
    }

    /// <summary>
    /// Base controller extension state class
    /// </summary>
    [Serializable]
    public class ExtensionState
    {
        /// <summary>
        /// If the extension is connected.
        /// </summary>
        public bool Connected { get { return connected; } }

        // Variables available to all extensions
        internal bool connected;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExtensionState() { }

        /// <summary>
        /// Resets the devices calibration to default.
        /// </summary>
        public virtual void ResetCalibration()
        {
            return;
        }

        // parses the report
        internal virtual void ParseExtension(byte[] r, int offset)
        {
            return;
        }
    }
    #endregion

    #region Stand Alone Devices
    /// <summary>
    /// Contains variables that describe a Wii Remote
    /// </summary>
    [Serializable]
    public class WiimoteState : NintyState
    {
        /// <summary>
        /// An empty Wiimote State
        /// (noting activated)
        /// </summary>
        public static readonly WiimoteState Empty = new WiimoteState();

        #region Input
        // Buttons
        internal bool a, b, one, two, minus, plus, up, down, left, right, home;
        /// <summary>
        /// If the A button is being pressed.
        /// </summary>
        public bool A { get { return a; } }
        /// <summary>
        /// If the B button is being pressed.
        /// </summary>
        public bool B { get { return b; } }
        /// <summary>
        /// If the One button is being pressed.
        /// </summary>
        public bool One { get { return one; } }
        /// <summary>
        /// If the Two button is being pressed.
        /// </summary>
        public bool Two { get { return two; } }
        /// <summary>
        /// If the Minus button is being pressed.
        /// </summary>
        public bool Minus { get { return minus; } }
        /// <summary>
        /// If the Plus button is being pressed.
        /// </summary>
        public bool Plus { get { return plus; } }
        /// <summary>
        /// If the Up direction is active.
        /// </summary>
        public bool Up { get { return up; } }
        /// <summary>
        /// If the Down direction is active.
        /// </summary>
        public bool Down { get { return down; } }
        /// <summary>
        /// If the Left direction is active.
        /// </summary>
        public bool Left { get { return left; } }
        /// <summary>
        /// If the Right direciton is active.
        /// </summary>
        public bool Right { get { return right; } }
        /// <summary>
        /// If the Home button is being pressed.
        /// </summary>
        public bool Home { get { return home; } }
        

        // Accelerometer
        internal Point3D accRaw;
        internal Point3DF acc;
        /// <summary>
        /// The raw state of the Accelerometer.
        /// </summary>
        public Point3D AccRaw { get { return accRaw; } }
        /// <summary>
        /// The normalized state of the Accelerometer.
        /// </summary>
        public Point3DF Acc { get { return acc; } }

        // IR Sensor
        internal IRPoint ir1, ir2, ir3, ir4;
        /// <summary>
        /// Info on the first IR point.
        /// </summary>
        public IRPoint IR1 { get { return ir1; } }
        /// <summary>
        /// Info on the second IR point.
        /// </summary>
        public IRPoint IR2 { get { return ir2; } }
        /// <summary>
        /// Info on the third IR point.
        /// </summary>
        public IRPoint IR3 { get { return ir3; } }
        /// <summary>
        /// Info on the fourth IR point.
        /// </summary>
        public IRPoint IR4 { get { return ir4; } }
        #endregion

        #region Device Settings
        // Accelerometer Calibration
        internal Point3D accCenter, accDead, accRange;
        /// <summary>
        /// Calibration: The centered values for the accelerometer.
        /// </summary>
        public Point3D AccCenter { get { return accCenter; } set { accCenter = value; } }
        /// <summary>
        /// Calibration: Deadzone for the accelerometer.
        /// </summary>
        public Point3D AccDead { get { return accDead; } set { accDead = value; } }
        /// <summary>
        /// Calibration: Range of the accelrometer in each direction.
        /// </summary>
        public Point3D AccRange { get { return accRange; } set { accRange = value; } }

        // IR mode
        internal IRSetting irMode;
        /// <summary>
        /// Current Mode of the IR Camera.
        /// </summary>
        public IRSetting IRMode { get { return irMode; } }
        #endregion

        #region Extensions
        // Extension
        internal bool extensionConnected;
        /// <summary>
        /// If an extension is currently connected.
        /// </summary>
        public bool ExtensionConnected { get { return extensionConnected; } }

        /// <summary>
        /// The state of a connected extension.
        /// </summary>
        public ExtensionState Extension { get { return extension; } }
        internal ExtensionState extension = new ExtensionState();
        /// <summary>
        /// The state of a connected Nunchuck.
        /// </summary>
        public NunchuckState Nunchuck { get { return nunchuck; } }
        internal NunchuckState nunchuck = NunchuckState.Empty;
        /// <summary>
        /// The state of a connected Classic Controller.
        /// </summary>
        public ClassicControllerState ClassicController { get { return classicController; } }
        internal ClassicControllerState classicController = ClassicControllerState.Empty;
        /// <summary>
        /// The state of a connected Classic Controller Pro.
        /// </summary>
        public ClassicControllerProState ClassicControllerPro { get { return classicControllerPro; } }
        internal ClassicControllerProState classicControllerPro = ClassicControllerProState.Empty;
        /// <summary>
        /// The state of a connected Motion Plus.
        /// </summary>
        public MotionPlusState MotionPlus { get { return motionPlus; } }
        internal MotionPlusState motionPlus = MotionPlusState.Empty;

        // The current connected extension
        private ControllerType currentExtension = ControllerType.Wiimote;
        #endregion

        // if the wiimote is rumbling
        internal bool rumble;
        /// <summary>
        /// Current state of the Wiimote's rumble.
        /// </summary>
        public bool Rumble { get { return rumble; } }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public WiimoteState() 
        {
            hasLEDs = true;
            ResetCalibration();
            irMode = IRSetting.Off;
        }

        /// <summary>
        /// The name of the Device.
        /// </summary>
        /// <returns>Device Name</returns>
        public override string ToString()
        {
            switch (currentExtension)
            {
                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                    return "Nintendo Wii Remote with Nunchuck";
                case ControllerType.ClassicController:
                    return "Nintendo Wii Remote with Classic Controller";
                case ControllerType.ClassicControllerPro:
                    return "Nintendo Wii Remote with Classic Controller Pro";
                case ControllerType.Wiimote:
                default:
                    return "Nintendo Wii Remote";
            }
        }

        /// <summary>
        /// Gets the recommended report type.
        /// </summary>
        /// <returns>Suggested Input Report</returns>
        public override InputReport GetReportType()
        {
            // depending on the extension & IR
            /// TODO: Take IR Enabled into account
            switch (currentExtension)
            {
                case ControllerType.Nunchuk:
                    return InputReport.BtnsAccIRExt;
                case ControllerType.ClassicController:
                    return InputReport.BtnsExt;
                case ControllerType.ClassicControllerPro:
                    return InputReport.BtnsExt;
                default:
                    return InputReport.BtnsAccIR;
            }
        }

        /// <summary>
        /// Gets the current rumble state.
        /// </summary>
        /// <returns>Rumbling</returns>
        public override bool GetRumble()
        {
            return rumble;
        }

        /// <summary>
        /// Sets the rumble state.
        /// </summary>
        /// <param name="enable">On/Off</param>
        public override void SetRumble(bool enable)
        {
            rumble = enable;
        }

        // Changes the extension type
        internal void SetExtension(ControllerType ext)
        {
            switch (ext)
            {
                case ControllerType.Nunchuk:
                case ControllerType.NunchukB:
                    extension = new NunchuckState();
                    extensionConnected = true;
                    currentExtension = ControllerType.Nunchuk;
                    break;
                case ControllerType.ClassicController:
                    extension = new ClassicControllerState();
                    extensionConnected = true;
                    currentExtension = ControllerType.ClassicController;
                    break;
                case ControllerType.ClassicControllerPro:
                    extension = new ClassicControllerProState();
                    extensionConnected = true;
                    currentExtension = ControllerType.ClassicControllerPro;
                    break;
                case ControllerType.MotionPlus:
                    extension = new MotionPlusState();
                    extensionConnected = true;
                    currentExtension = ControllerType.MotionPlus;
                    break;
                case ControllerType.Wiimote:
                default:
                    extension = new ExtensionState();
                    extensionConnected = false;
                    currentExtension = ControllerType.Wiimote;
                    break;
            }
        }

        /// <summary>
        /// Resets the calibration to its default.
        /// </summary>
        public override void ResetCalibration()
        {
            if (Nintroller.UseModestConfigs)
            {
                accCenter = WiimoteCalibration.Modest.AccelerometerCenter;
                accDead = WiimoteCalibration.Modest.AccelerometerDeadzone;
                accRange = WiimoteCalibration.Modest.AccelerometerRange;
            }
            else
            {
                accCenter = WiimoteCalibration.Default.AccelerometerCenter;
                accDead = WiimoteCalibration.Default.AccelerometerDeadzone;
                accRange = WiimoteCalibration.Default.AccelerometerRange;
            }
            /*
            AccCenter.X = 128;
            AccCenter.Y = 128;
            AccCenter.Z = 128;

            AccDead.X = 4;
            AccDead.Y = 4;
            AccDead.Z = 4;

            AccRange.X = 48;
            AccRange.Y = 48;
            AccRange.Z = 48;
            */
        }

        /// <summary>
        /// Changes the controllers calibratoin settings.
        /// </summary>
        /// <param name="calibration">Calibratoin to match</param>
        public void SetCalibration(WiimoteCalibration calibration)
        {
            accCenter = calibration.AccelerometerCenter;
            accDead = calibration.AccelerometerDeadzone;
            accRange = calibration.AccelerometerRange;
        }

        // Calculates the battery level
        internal override void UpdateBattery()
        {
            batteryLevel = 100.0f * (float)batteryRaw / 192.0f;
            lowBattery = batteryLevel < 0.1f;

            if (batteryLevel > 80f)
                Battery = BatteryStatus.VeryHigh;
            else if (batteryLevel > 60f)
                Battery = BatteryStatus.High;
            else if (batteryLevel > 40f)
                Battery = BatteryStatus.Medium;
            else if (batteryLevel > 20f)
                Battery = BatteryStatus.Low;
            else
                Battery = BatteryStatus.VeryLow;
        }

        // Parses the byte report
        internal override void ParseReport(byte[] r)
        {
            InputReport type = (InputReport)r[0];

            #region Parse Buttons
            if (type != InputReport.ExtOnly)
            {
                a = (r[2] & 0x08) != 0;
                b = (r[2] & 0x04) != 0;
                one = (r[2] & 0x02) != 0;
                two = (r[2] & 0x01) != 0;
                minus = (r[2] & 0x10) != 0;
                home = (r[2] & 0x80) != 0;
                plus = (r[1] & 0x10) != 0;
                up = (r[1] & 0x08) != 0;
                down = (r[1] & 0x04) != 0;
                left = (r[1] & 0x01) != 0;
                right = (r[1] & 0x02) != 0;
            }
            #endregion

            #region Parse Accelerometer
            if (type == InputReport.BtnsAcc
                || type == InputReport.BtnsAccExt
                || type == InputReport.BtnsAccIR
                || type == InputReport.BtnsAccIRExt)
            {
                accRaw.X = r[3];
                accRaw.Y = r[4];
                accRaw.Z = r[5];

                //if (Math.Abs(accRaw.X - accCenter.X) < accDead.X)
                //    acc.X = 0;
                //else
                //    acc.X = 2f * (float)(accRaw.X - accCenter.X) / (float)accRange.X;

                //if (Math.Abs(accRaw.Y - accCenter.Y) < accDead.Y)
                //    acc.Y = 0;
                //else
                //    acc.Y = 2f * (float)(accRaw.Y - accCenter.Y) / (float)accRange.Y;

                //if (Math.Abs(accRaw.Z - accCenter.Z) < accDead.Z)
                //    acc.Z = 0;
                //else
                //    acc.Z = 2f * (float)(accRaw.Z - accCenter.Z) / (float)accRange.Z;

                // Mormalized Accelerometer
                acc.X = NintyState.NormalizeAxisValue(accRaw.X, new JoyCalibration(accCenter.X - accRange.X, accCenter.X, accCenter.X + accRange.X), accDead.X);
                acc.Y = NintyState.NormalizeAxisValue(accRaw.Y, new JoyCalibration(accCenter.Y - accRange.Y, accCenter.Y, accCenter.Y + accRange.Y), accDead.Y);
                acc.Z = NintyState.NormalizeAxisValue(accRaw.Z, new JoyCalibration(accCenter.Z - accRange.Z, accCenter.Z, accCenter.Z + accRange.Z), accDead.Z);
            }
            #endregion

            // TODO: Only parse IR if it's enabled
            #region Parse IR
            int offset = 0;

            switch (type)
            {
                case InputReport.BtnsAccIR:
                case InputReport.BtnsAccIRExt:
                    offset = 6;
                    break;
                case InputReport.BtnsIRExt:
                    offset = 3;
                    break;
                default:
                    break;
            }

            if (offset != 0)
            {
                ir1.RawX = r[offset]     | ((r[offset + 2] >> 4) & 0x03) << 8;
                ir1.RawY = r[offset + 1] | ((r[offset + 2] >> 6) & 0x03) << 8;

                if (type == InputReport.BtnsAccIR)
                {   // extended
                    ir2.RawX = r[offset +  3] | ((r[offset +  5] >> 4) & 0x03) << 8;
                    ir2.RawY = r[offset +  4] | ((r[offset +  5] >> 6) & 0x03) << 8;
                    ir3.RawX = r[offset +  6] | ((r[offset +  8] >> 4) & 0x03) << 8;
                    ir3.RawY = r[offset +  7] | ((r[offset +  8] >> 6) & 0x03) << 8;
                    ir4.RawX = r[offset +  9] | ((r[offset + 11] >> 4) & 0x03) << 8;
                    ir4.RawY = r[offset + 10] | ((r[offset + 11] >> 6) & 0x03) << 8;

                    ir1.Size = r[offset + 2]  & 0x0f;
                    ir2.Size = r[offset + 5]  & 0x0f;
                    ir3.Size = r[offset + 8]  & 0x0f;
                    ir4.Size = r[offset + 11] & 0x0f;

                    ir1.InView = !(r[offset]     == 0xff && r[offset + 1] == 0xff && r[offset + 2] == 0xff);
                    ir2.InView = !(r[offset + 3] == 0xff && r[offset + 4] == 0xff && r[offset + 5] == 0xff);
                    ir3.InView = !(r[offset + 6] == 0xff && r[offset + 7] == 0xff && r[offset + 8] == 0xff);
                    ir4.InView = !(r[offset + 9] == 0xff && r[offset + 10] == 0xff && r[offset + 11] == 0xff);
                }
                else
                {   // basic
                    ir2.RawX = r[offset + 3] | ((r[offset + 2] >> 0) & 0x03) << 8;
                    ir2.RawY = r[offset + 4] | ((r[offset + 2] >> 2) & 0x03) << 8;
                    ir3.RawX = r[offset + 5] | ((r[offset + 7] >> 4) & 0x03) << 8;
                    ir3.RawY = r[offset + 6] | ((r[offset + 7] >> 6) & 0x03) << 8;
                    ir4.RawX = r[offset + 8] | ((r[offset + 7] >> 0) & 0x03) << 8;
                    ir4.RawY = r[offset + 9] | ((r[offset + 7] >> 2) & 0x03) << 8;

                    ir1.Size = 0x00;
                    ir2.Size = 0x00;
                    ir3.Size = 0x00;
                    ir4.Size = 0x00;

                    ir1.InView = !(r[offset]     == 0xff && r[offset + 1] == 0xff);
                    ir2.InView = !(r[offset + 3] == 0xff && r[offset + 4] == 0xff);
                    ir3.InView = !(r[offset + 5] == 0xff && r[offset + 6] == 0xff);
                    ir4.InView = !(r[offset + 8] == 0xff && r[offset + 9] == 0xff);
                }
            }
            #endregion

            #region Parse Extension
            if (extension != null)
            {
                switch (type)
                {
                    case InputReport.BtnsExt:
                    case InputReport.BtnsExtB:
                        extension.ParseExtension(r, 3);
                        break;
                    case InputReport.BtnsAccExt:
                        extension.ParseExtension(r, 6);
                        break;
                    case InputReport.BtnsIRExt:
                        extension.ParseExtension(r, 13);
                        break;
                    case InputReport.BtnsAccIRExt:
                        extension.ParseExtension(r, 16);
                        break;
                    case InputReport.ExtOnly:
                        extension.ParseExtension(r, 1);
                        break;
                    default:
                        break;
                }

                nunchuck = NunchuckState.Empty;
                classicController = ClassicControllerState.Empty;
                classicControllerPro = ClassicControllerProState.Empty;
                motionPlus = MotionPlusState.Empty;

                if (extension.GetType() == typeof(NunchuckState))
                    nunchuck = (NunchuckState)extension;
                else if (extension.GetType() == typeof(ClassicControllerState))
                    classicController = (ClassicControllerState)extension;
                else if (extension.GetType() == typeof(ClassicControllerProState))
                    classicControllerPro = (ClassicControllerProState)extension;
                else if (extension.GetType() == typeof(MotionPlusState))
                    motionPlus = (MotionPlusState)extension;
            }
            #endregion
        }
    }

    /// <summary>
    /// Contains variables that describe a Wii Remote Plus
    /// </summary>
    [Serializable]
    public class WiimotePlusState : WiimoteState
    {
        // May Not need a seperate class for the motion plus inside

        /// <summary>
        /// An empty WiimotePlus State
        /// (noting activated)
        /// </summary>
        public static readonly WiimotePlusState Empty = new WiimotePlusState();

        // Gyroscope
        internal Point3D gyroRaw;
        internal Point3DF gyro;
        /// <summary>
        /// The raw state of the gyroscope.
        /// </summary>
        public Point3D GyroRaw { get { return gyroRaw; } }
        /// <summary>
        /// The normalized values of the gyroscope.
        /// </summary>
        public Point3DF Gyro { get { return gyro; } }

        /// <summary>
        /// Default constructor
        /// </summary>
        public WiimotePlusState() : base()
        {
            hasLEDs = true;
            ResetCalibration();
        }

        /// <summary>
        /// The name of the device.
        /// </summary>
        /// <returns>Device name</returns>
        public override string ToString()
        {
            return "Nintendo Wii Remote Plus";
        }

        /// <summary>
        /// Resets the calibration to the default settings.
        /// </summary>
        public override void ResetCalibration()
        {
            base.ResetCalibration();

            // TODO: Set Gyro Calibration
        }

        // Calculates the battery level
        internal override void UpdateBattery()
        {
            batteryLevel = 100.0f * (float)batteryRaw / 192.0f;
            lowBattery = batteryLevel < 0.1f;

            if (batteryLevel > 80f)
                Battery = BatteryStatus.VeryHigh;
            else if (batteryLevel > 60f)
                Battery = BatteryStatus.High;
            else if (batteryLevel > 40f)
                Battery = BatteryStatus.Medium;
            else if (batteryLevel > 20f)
                Battery = BatteryStatus.Low;
            else
                Battery = BatteryStatus.VeryLow;
        }

        // Parses the byte report
        internal override void ParseReport(byte[] r)
        {
            /// TODO: Parse Gyro
            
            base.ParseReport(r);
        }
    }  

    /// <summary>
    /// Contains variables that describe a Wii U Pro Controller
    /// </summary>
    [Serializable]
    public class ProControllerState : NintyState
    {
        /// <summary>
        /// An Empty Pro Controller State
        /// (nothing activated)
        /// </summary>
        public static readonly ProControllerState Empty = new ProControllerState();

        #region Buttons
        internal bool a, b, x, y, lButton, rButton, zl, zr, lStick, rStick, plus, minus, home, up, down, left, right;
        /// <summary>
        /// If the A button is being pressed.
        /// </summary>
        public bool A { get { return a; } }
        /// <summary>
        /// If the B button is being pressed.
        /// </summary>
        public bool B { get { return b; } }
        /// <summary>
        /// If the X button is being pressed.
        /// </summary>
        public bool X { get { return x; } }
        /// <summary>
        /// If the Y button is being pressed.
        /// </summary>
        public bool Y { get { return y; } }
        /// <summary>
        /// If the L button is being pressed.
        /// </summary>
        public bool L { get { return lButton; } }
        /// <summary>
        /// If the R button is being pressed.
        /// </summary>
        public bool R { get { return rButton; } }
        /// <summary>
        /// If the ZL button is being pressed.
        /// </summary>
        public bool ZL { get { return zl; } }
        /// <summary>
        /// If the ZR button is being pressed.
        /// </summary>
        public bool ZR { get { return zr; } }
        /// <summary>
        /// If the Left Stick is being pressed.
        /// </summary>
        public bool LS { get { return lStick; } }
        /// <summary>
        /// If the Right Stick is being pressed.
        /// </summary>
        public bool RS { get { return rStick; } }
        /// <summary>
        /// If the Start button is being pressed.
        /// </summary>
        public bool Start { get { return plus; } }
        /// <summary>
        /// If the Select button is being pressed.
        /// </summary>
        public bool Select { get { return minus; } }
        /// <summary>
        /// If the Home button is being pressed.
        /// </summary>
        public bool Home { get { return home; } }
        /// <summary>
        /// If the Up button is being pressed.
        /// </summary>
        public bool Up { get { return up; } }
        /// <summary>
        /// If the Down button is being pressed.
        /// </summary>
        public bool Down { get { return down; } }
        /// <summary>
        /// If the Left button is being pressed.
        /// </summary>
        public bool Left { get { return left; } }
        /// <summary>
        /// If the Right button is being pressed.
        /// </summary>
        public bool Right { get { return right; } }
        #endregion

        #region Joysticks
        internal Point2D leftJoyRaw, rightJoyRaw;
        internal Point2DF leftJoy, rightJoy;
        /// <summary>
        /// The Raw left joystick information.
        /// </summary>
        public Point2D LeftJoyRaw { get { return leftJoyRaw; } }
        /// <summary>
        /// The Raw right joystick information.
        /// </summary>
        public Point2D RightJoyRaw { get { return rightJoyRaw; } }
        /// <summary>
        /// The state of the left joystick.
        /// </summary>
        public Point2DF LeftJoy { get { return leftJoy; } }
        /// <summary>
        /// The state of the right joystick.
        /// </summary>
        public Point2DF RightJoy { get { return rightJoy; } }
        #endregion

        #region Device Settings
        // Joystick Calibration
        internal JoyCalibration leftJoyXCalibration, leftJoyYCalibration;
        internal JoyCalibration rightJoyXCalibration, rightJoyYCalibration;
        internal Point2D leftJoyDeadZone, rightJoyDeadZone;
        /// <summary>
        /// The minimum, maximum, and center of the Left Joy's X-axis.
        /// </summary>
        public JoyCalibration LeftJoyXCalibration { get { return leftJoyXCalibration; } set { leftJoyXCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of the Left Joy's Y-axis.
        /// </summary>
        public JoyCalibration LeftJoyYCalibration { get { return leftJoyYCalibration; } set { leftJoyYCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of the Right Joy's X-axis.
        /// </summary>
        public JoyCalibration RightJoyXCalibration { get { return rightJoyXCalibration; } set { rightJoyXCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of teh Right Joy's Y-axis.
        /// </summary>
        public JoyCalibration RightJoyYCalibration { get { return rightJoyYCalibration; } set { rightJoyYCalibration = value; } }
        /// <summary>
        /// The deadzone size of the X &amp; Y axes for the left joystick.
        /// </summary>
        public Point2D LeftJoyDeadZone { get { return leftJoyDeadZone; } set { leftJoyDeadZone = value; } }
        /// <summary>
        /// The deadzone size of the X &amp; Y axes for the right joystick.
        /// </summary>
        public Point2D RightJoyDeadZone { get { return rightJoyDeadZone; } set { rightJoyDeadZone = value; } }

        internal bool charging, usbConnected;
        /// <summary>
        /// Indicates if the controller is charging.
        /// </summary>
        public bool Charging { get { return charging; } }
        /// <summary>
        /// Indicates if the USB is connected to the Controller.
        /// </summary>
        public bool USBConnected { get { return usbConnected; } }
        // There might be another byte or more of data on the batery level
        #endregion

        internal bool rumble;
        /// <summary>
        /// The state of the Pro controller's rumble.
        /// </summary>
        public bool Rumble { get { return rumble; } }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ProControllerState() 
        {
            hasLEDs = true;
            ResetCalibration();
        }

        #region Public Methods
        /// <summary>
        /// Returns the name of the device.
        /// </summary>
        /// <returns>Device Name</returns>
        public override string ToString()
        {
            return "Nintendo Wii U Pro Controller";
        }

        /// <summary>
        /// Gets the recommended report type.
        /// </summary>
        /// <returns>Suggested Input Report</returns>
        public override InputReport GetReportType()
        {
            return InputReport.ExtOnly;
        }

        /// <summary>
        /// Gets the current rumble state.
        /// </summary>
        /// <returns>Rumbling</returns>
        public override bool GetRumble()
        {
            return rumble;
        }

        /// <summary>
        /// Sets the rumble state.
        /// </summary>
        /// <param name="enable">On/Off</param>
        public override void SetRumble(bool enable)
        {
            rumble = enable;
        }

        /// <summary>
        /// Resets the calibration settings to the default.
        /// </summary>
        public override void ResetCalibration()
        {
            if (Nintroller.UseModestConfigs)
            {
                leftJoyXCalibration = ProCalibration.Modest.JoystickLX;
                leftJoyYCalibration = ProCalibration.Modest.JoystickLY;
                leftJoyDeadZone = ProCalibration.Modest.JoystickLDeadZone;

                rightJoyXCalibration = ProCalibration.Modest.JoystickRX;
                rightJoyYCalibration = ProCalibration.Modest.JoystickRY;
                rightJoyDeadZone = ProCalibration.Modest.JoystickRDeadZone;
            }
            else
            {
                leftJoyXCalibration = ProCalibration.Default.JoystickLX;
                leftJoyYCalibration = ProCalibration.Default.JoystickLY;
                leftJoyDeadZone = ProCalibration.Default.JoystickLDeadZone;

                rightJoyXCalibration = ProCalibration.Default.JoystickRX;
                rightJoyYCalibration = ProCalibration.Default.JoystickRY;
                rightJoyDeadZone = ProCalibration.Default.JoystickRDeadZone;
            }
            /*
            LeftJoyXCalibration.Min = 1024;
            LeftJoyXCalibration.Mid = 2048;
            LeftJoyXCalibration.Max = 3072;

            LeftJoyYCalibration.Mid = 1024;
            LeftJoyYCalibration.Mid = 2048;
            LeftJoyYCalibration.Max = 3072;

            LeftJoyDeadZone.X = 256;
            LeftJoyDeadZone.Y = 256;

            RightJoyXCalibration.Min = 1024;
            RightJoyXCalibration.Mid = 2048;
            RightJoyXCalibration.Max = 3072;

            RightJoyYCalibration.Min = 1024;
            RightJoyYCalibration.Mid = 2048;
            RightJoyYCalibration.Max = 3072;

            RightJoyDeadZone.X = 256;
            RightJoyDeadZone.Y = 256;
            */
        }

        /// <summary>
        /// Sets the calibration settings.
        /// </summary>
        /// <param name="calibration">Calibration to apply</param>
        public void SetCalibration(ProCalibration calibration)
        {
            leftJoyXCalibration = calibration.JoystickLX;
            leftJoyYCalibration = calibration.JoystickLY;
            rightJoyXCalibration = calibration.JoystickRX;
            rightJoyYCalibration = calibration.JoystickRY;
       }
        #endregion

        // Calculates the battery level
        internal override void UpdateBattery()
        {
            batteryLevel = 2f * ((float)batteryRaw - 205f);
            lowBattery = batteryLevel < 50f;

            if (batteryLevel > 90f)
                Battery = BatteryStatus.VeryHigh;
            else if (batteryLevel > 80f)
                Battery = BatteryStatus.High;
            else if (batteryLevel > 70f)
                Battery = BatteryStatus.Medium;
            else if (batteryLevel > 60f)
                Battery = BatteryStatus.Low;
            else
                Battery = BatteryStatus.VeryLow;
        }

        // Parses the byte report
        internal override void ParseReport(byte[] r)
        {
            #region Set Offset
            int offset = 0;

            switch ((InputReport)r[0])
            {
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                case InputReport.BtnsExt:
                case InputReport.BtnsExtB:
                    offset = 3;
                    break;
                case InputReport.BtnsAccExt:
                    offset = 6;
                    break;
                case InputReport.BtnsIRExt:
                    offset = 13;
                    break;
                case InputReport.BtnsAccIRExt:
                    offset = 16;
                    break;
                default:
                    break;
            }
            #endregion

            #region Parse
            if (offset == 0)
                return;

            // Buttons
            a = (r[offset + 9] & 0x10) == 0;
            b = (r[offset + 9] & 0x40) == 0;
            x = (r[offset + 9] & 0x08) == 0;
            y = (r[offset + 9] & 0x20) == 0;
            lButton = (r[offset + 8] & 0x20) == 0;
            rButton = (r[offset + 8] & 0x02) == 0;
            zl = (r[offset + 9] & 0x80) == 0;
            zr = (r[offset + 9] & 0x04) == 0;
            lStick = (r[offset + 10] & 0x02) == 0;
            rStick = (r[offset + 10] & 0x01) == 0;
            plus = (r[offset + 8] & 0x04) == 0;
            minus = (r[offset + 8] & 0x10) == 0;
            home = (r[offset + 8] & 0x08) == 0;

            // DPad
            up = (r[offset + 9] & 0x01) == 0;
            down = (r[offset + 8] & 0x40) == 0;
            left = (r[offset + 9] & 0x02) == 0;
            right = (r[offset + 8] & 0x80) == 0;

            // Joysticks
            leftJoyRaw.X = BitConverter.ToInt16(r, offset);
            leftJoyRaw.Y = BitConverter.ToInt16(r, offset + 4);
            rightJoyRaw.X = BitConverter.ToInt16(r, offset + 2);
            rightJoyRaw.Y = BitConverter.ToInt16(r, offset + 6);

            // Other
            charging = (r[offset + 10] & 0x04) == 0;
            usbConnected = (r[offset + 10] & 0x08) == 0;

            // Normaliezed Joysticks
            leftJoy.X = NormalizeAxisValue(leftJoyRaw.X, leftJoyXCalibration, leftJoyDeadZone.X);
            leftJoy.Y = NormalizeAxisValue(leftJoyRaw.Y, leftJoyYCalibration, leftJoyDeadZone.Y);
            rightJoy.X = NormalizeAxisValue(rightJoyRaw.X, rightJoyXCalibration, rightJoyDeadZone.X);
            rightJoy.Y = NormalizeAxisValue(rightJoyRaw.Y, rightJoyYCalibration, rightJoyDeadZone.Y);
            #endregion
        }
    }

    /// <summary>
    /// Contains variables that describe a Wii Balance Board
    /// </summary>
    [Serializable]
    public class BalanceBoardState : NintyState
    {
        /// <summary>
        /// An Empty Balance Board State
        /// (Nothing activated)
        /// </summary>
        public static readonly BalanceBoardState Empty = new BalanceBoardState();

        #region Members
        // Sensors
        internal BalanceBoardSensor raw, sensor;
        /// <summary>
        /// The raw values of the Balance Board sensors.
        /// </summary>
        public BalanceBoardSensor Raw { get { return raw; } }
        /// <summary>
        /// The state of the Balance Board sensors.
        /// </summary>
        public BalanceBoardSensor Sensor { get { return sensor; } }

        // Sensor Calibration
        internal BalanceBoardSensor calibration;
        /// <summary>
        /// The calibration for the Balance Board at Zero
        /// </summary>
        public BalanceBoardSensor Calibration { get { return calibration; } }

        internal float weightKg, weightLb;
        internal Point2DF centerOfGravity;
        /// <summary>
        /// Weight on the balance board in Kilograms.
        /// </summary>
        public float WeightKg { get { return weightKg; } }
        /// <summary>
        /// Weight on the balance board in Pounds.
        /// </summary>
        public float WeightLbs { get { return weightLb; } }
        /// <summary>
        /// Center of Gravity point on the balance board.
        /// </summary>
        public Point2DF CenterOfGravity { get { return centerOfGravity; } }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BalanceBoardState() 
        {
            // TODO: Default Calibration
        }

        #region Public Methods
        /// <summary>
        /// Zero's the weight.
        /// </summary>
        public void Zero()
        {
            // TODO: Zero / recalibrate the balance board
        }

        /// <summary>
        /// Gets the recommended report type.
        /// </summary>
        /// <returns>Suggested Input Report</returns>
        public override InputReport GetReportType()
        {
            return InputReport.ExtOnly;
        }
        #endregion

        // Calculates the battery level
        internal override void UpdateBattery()
        {
            batteryLevel = 100.0f * (float)batteryRaw / 192.0f;
            lowBattery = batteryLevel < 0.1f;

            if (batteryLevel > 0.8f)
                Battery = BatteryStatus.VeryHigh;
            else if (batteryLevel > 0.6f)
                Battery = BatteryStatus.High;
            else if (batteryLevel > 0.4f)
                Battery = BatteryStatus.Medium;
            else if (batteryLevel > 0.2f)
                Battery = BatteryStatus.Low;
            else
                Battery = BatteryStatus.VeryLow;
        }

        // Parses the byte report
        internal override void ParseReport(byte[] r)
        {
            #region Set Offset
            int offset = 0;

            switch ((InputReport)r[0])
            {
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                case InputReport.BtnsExt:
                case InputReport.BtnsExtB:
                    offset = 3;
                    break;
                case InputReport.BtnsAccExt:
                    offset = 6;
                    break;
                case InputReport.BtnsIRExt:
                    offset = 13;
                    break;
                case InputReport.BtnsAccIRExt:
                    offset = 16;
                    break;
                default:
                    break;
            }
            #endregion

            #region Parse
            if (offset == 0)
                return;

            raw.TopRight     = (short)((short)r[offset    ] << 8 | r[offset + 1]);
            raw.BottomRight  = (short)((short)r[offset + 2] << 8 | r[offset + 3]);
            raw.TopLeft      = (short)((short)r[offset + 4] << 8 | r[offset + 5]);
            raw.BottomLeft   = (short)((short)r[offset + 6] << 8 | r[offset + 7]);

            /// TODO: Calculate other members (like weight distribution)
            #endregion
        }
    }
    #endregion 

    #region Device Extensions
    /// <summary>
    /// Contains variables that describe a Nunchuck
    /// </summary>
    [Serializable]
    public class NunchuckState : ExtensionState
    {
        /// <summary>
        /// An empty Nunchuck state
        /// (nothing active)
        /// </summary>
        public static readonly NunchuckState Empty = new NunchuckState();

        #region Inputs
        // Buttons
        internal bool c, z;
        /// <summary>
        /// If the C button is being pressed.
        /// </summary>
        public bool C { get { return c; } }
        /// <summary>
        /// If the Z button is being pressed.
        /// </summary>
        public bool Z { get { return z; } }

        // Joystick
        internal Point2D joyRaw;
        internal Point2DF joy;
        /// <summary>
        /// The raw values of the joystick.
        /// </summary>
        public Point2D JoyRaw { get { return joyRaw; } }
        /// <summary>
        /// The normalized state of the joystick.
        /// </summary>
        public Point2DF Joy { get { return joy; } }

        // Accelerometer
        internal Point3D accRaw;
        internal Point3DF acc;
        /// <summary>
        /// The Raw values of the accelerometer.
        /// </summary>
        public Point3D AccRaw { get { return accRaw; } }
        /// <summary>
        /// The normalized state of the accelerometer.
        /// </summary>
        public Point3DF Acc { get { return acc; } }
        #endregion

        #region Calibratoins
        // Joystick Calibration
        internal JoyCalibration joyXCalibration, joyYCalibration;
        internal Point2D joyDeadZone;
        /// <summary>
        /// Minimum, maximum, and center of the joystick's X-axis.
        /// </summary>
        public JoyCalibration JoyXCalibration { get { return joyXCalibration; } set { joyXCalibration = value; } }
        /// <summary>
        /// Minimum, maximum, and center of the joystick's Y-axis.
        /// </summary>
        public JoyCalibration JoyYCalibration { get { return joyYCalibration; } set { joyYCalibration = value; } }
        /// <summary>
        /// Deadzone range for the joystick's X &amp; Y axes.
        /// </summary>
        public Point2D JoyDeadZone { get { return joyDeadZone; } set { joyDeadZone = value; } }

        // Accelerometer Calibration
        internal Point3D accCenter, accDead, accRange;
        /// <summary>
        /// Calibration: The Center for the accelerometer.
        /// </summary>
        public Point3D AccCenter { get { return accCenter; } set { accCenter = value; } }
        /// <summary>
        /// Calibration: Deadzone for the accelerometer
        /// </summary>
        public Point3D AccDead { get { return accDead; } set { accDead = value; } }
        /// <summary>
        /// Calibration: Range of the accelrometer in each direction.
        /// </summary>
        public Point3D AccRange { get { return accRange; } set { accRange = value; } }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NunchuckState ()
        {
            ResetCalibration();
        }

        /// <summary>
        /// Resetes the calibration settings to the default.
        /// </summary>
        public override void ResetCalibration()
        {
            if (Nintroller.UseModestConfigs)
            {
                joyXCalibration = NunchuckCalibration.Modest.JoystickX;
                joyYCalibration = NunchuckCalibration.Modest.JoystickY;
                joyDeadZone = NunchuckCalibration.Modest.JoystickDeadzone;

                accCenter = NunchuckCalibration.Modest.AccelerometerCenter;
                accDead = NunchuckCalibration.Modest.AccelerometerDeadzone;
                accRange = NunchuckCalibration.Modest.AccelerometerRange;
            }
            else
            {
                joyXCalibration = NunchuckCalibration.Default.JoystickX;
                joyYCalibration = NunchuckCalibration.Default.JoystickY;
                joyDeadZone = NunchuckCalibration.Default.JoystickDeadzone;

                accCenter = NunchuckCalibration.Default.AccelerometerCenter;
                accDead = NunchuckCalibration.Default.AccelerometerDeadzone;
                accRange = NunchuckCalibration.Default.AccelerometerRange;
            }
            /*
            JoyXCalibration.Min = 32;
            JoyXCalibration.Mid = 128;
            JoyXCalibration.Max = 224;

            JoyYCalibration.Min = 32;
            JoyYCalibration.Mid = 128;
            JoyYCalibration.Max = 224;

            JoyDeadZone.X = 8;
            JoyDeadZone.Y = 8;

            AccCenter.X = 128;
            AccCenter.Y = 128;
            AccCenter.Z = 128;

            AccDead.X = 4;
            AccDead.Y = 4;
            AccDead.Z = 4;

            AccRange.X = 112;
            AccRange.Y = 112;
            AccRange.Z = 112;
            */
        }

        /// <summary>
        /// Sets the calibration settings.
        /// </summary>
        /// <param name="calibration">Calibration settings</param>
        public void SetCalibration(NunchuckCalibration calibration)
        {
            joyXCalibration = calibration.JoystickX;
            joyYCalibration = calibration.JoystickY;
            accCenter = calibration.AccelerometerCenter;
            accDead = calibration.AccelerometerDeadzone;
            accRange = calibration.AccelerometerRange;
        }

        // Parses the byte report
        internal override void ParseExtension(byte[] r, int offset)
        {
            // Buttons
            c = (r[offset + 5] & 0x02) == 0;
            z = (r[offset + 5] & 0x01) == 0;

            // Joysticks
            joyRaw.X = r[offset];
            joyRaw.Y = r[offset + 1];

            // Accelerometer
            accRaw.X = r[offset + 2];
            accRaw.Y = r[offset + 3];
            accRaw.Z = r[offset + 4];

            // Normaliezed Joysticks
            joy.X = NintyState.NormalizeAxisValue(joyRaw.X, joyXCalibration, joyDeadZone.X);
            joy.Y = NintyState.NormalizeAxisValue(joyRaw.Y, joyYCalibration, joyDeadZone.Y);
            
            // Mormalized Accelerometer
            acc.X = NintyState.NormalizeAxisValue(accRaw.X, new JoyCalibration(accCenter.X - accRange.X, accCenter.X, accCenter.X + accRange.X), accDead.X);
            acc.Y = NintyState.NormalizeAxisValue(accRaw.Y, new JoyCalibration(accCenter.Y - accRange.Y, accCenter.Y, accCenter.Y + accRange.Y), accDead.Y);
            acc.Z = NintyState.NormalizeAxisValue(accRaw.Z, new JoyCalibration(accCenter.Z - accRange.Z, accCenter.Z, accCenter.Z + accRange.Z), accDead.Z);
        }
    }

    /// <summary>
    /// Contains variables that describe a Classic Controller
    /// </summary>
    [Serializable]
    public class ClassicControllerState : ExtensionState
    {
        /// <summary>
        /// An Empty Classic Controller State
        /// (noting active)
        /// </summary>
        public static readonly ClassicControllerState Empty = new ClassicControllerState();

        #region Buttons
        internal bool a, b, x, y, lPressure, rPressure, lFull, rFull, zL, zR, plus, minus, home, up, down, left, right;
        /// <summary>
        /// If the A button is being pressed.
        /// </summary>
        public bool A { get { return a; } }
        /// <summary>
        /// If the B button is being pressed.
        /// </summary>
        public bool B { get { return b; } }
        /// <summary>
        /// If the X button is being pressed.
        /// </summary>
        public bool X { get { return x; } }
        /// <summary>
        /// If the Y button is being pressed.
        /// </summary>
        public bool Y { get { return y; } }
        /// <summary>
        /// If the L trigger is being pressed.
        /// </summary>
        public bool L { get { return lPressure; } }
        /// <summary>
        /// If the R trigger is being pressed.
        /// </summary>
        public bool R { get { return rPressure; } }
        /// <summary>
        /// If the L trigger is being fully pressed.
        /// </summary>
        public bool LFull { get { return lFull; } }
        /// <summary>
        /// If the R trigger is being fully pressed.
        /// </summary>
        public bool RFull { get { return rFull; } }
        /// <summary>
        /// If the ZL button is being pressed.
        /// </summary>
        public bool ZL { get { return zL; } }
        /// <summary>
        /// If the ZR button is being pressed.
        /// </summary>
        public bool ZR { get { return zR; } }
        /// <summary>
        /// If the Start button is being pressed.
        /// </summary>
        public bool Start { get { return plus; } }
        /// <summary>
        /// If the Select button is being pressed.
        /// </summary>
        public bool Select { get { return minus; } }
        /// <summary>
        /// If the Home button is being pressed.
        /// </summary>
        public bool Home { get { return home; } }
        /// <summary>
        /// If the Up button is being pressed.
        /// </summary>
        public bool Up { get { return up; } }
        /// <summary>
        /// If the Down button is being pressed.
        /// </summary>
        public bool Down { get { return down; } }
        /// <summary>
        /// If the Left button is being pressed.
        /// </summary>
        public bool Left { get { return left; } }
        /// <summary>
        /// If the Right button is being pressed.
        /// </summary>
        public bool Right { get { return right; } }
        #endregion

        #region Joysticks & Triggers
        internal Point2D leftJoyRaw, rightJoyRaw;
        internal Point2DF leftJoy, rightJoy;
        /// <summary>
        /// The Raw left joystick information.
        /// </summary>
        public Point2D LeftJoyRaw { get { return leftJoyRaw; } }
        /// <summary>
        /// The Raw right joystick information.
        /// </summary>
        public Point2D RightJoyRaw { get { return rightJoyRaw; } }
        /// <summary>
        /// The state of the left joystick.
        /// </summary>
        public Point2DF LeftJoy { get { return leftJoy; } }
        /// <summary>
        /// The state of the right joystick.
        /// </summary>
        public Point2DF RightJoy { get { return rightJoy; } }

        // Triggers
        internal int lTriggerRaw, rTriggerRaw;
        internal float lTrigger, rTrigger;
        /// <summary>
        /// The raw left trigger value.
        /// </summary>
        public int LTriggerRaw { get { return lTriggerRaw; } }
        /// <summary>
        /// The raw right trigger value.
        /// </summary>
        public int RTriggerRaw { get { return rTriggerRaw; } }
        /// <summary>
        /// The normalized left trigger.
        /// </summary>
        public float LTrigger { get { return lTrigger; } }
        /// <summary>
        /// The normailized right trigger.
        /// </summary>
        public float RTrigger { get { return rTrigger; } }
        #endregion

        #region Calibration
        // Joystick Calibration
        internal JoyCalibration leftJoyXCalibration, leftJoyYCalibration;
        internal JoyCalibration rightJoyXCalibration, rightJoyYCalibration;
        internal Point2D leftJoyDeadZone, rightJoyDeadZone;
        /// <summary>
        /// The minimum, maximum, and center of the Left Joy's X-axis.
        /// </summary>
        public JoyCalibration LeftJoyXCalibration { get { return leftJoyXCalibration; } set { leftJoyXCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of the Left Joy's Y-axis.
        /// </summary>
        public JoyCalibration LeftJoyYCalibration { get { return leftJoyYCalibration; } set { leftJoyYCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of the Right Joy's X-axis.
        /// </summary>
        public JoyCalibration RightJoyXCalibration { get { return rightJoyXCalibration; } set { rightJoyXCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of teh Right Joy's Y-axis.
        /// </summary>
        public JoyCalibration RightJoyYCalibration { get { return rightJoyYCalibration; } set { rightJoyYCalibration = value; } }
        /// <summary>
        /// The deadzone size of the X &amp; Y axes for the left joystick.
        /// </summary>
        public Point2D LeftJoyDeadZone { get { return leftJoyDeadZone; } set { leftJoyDeadZone = value; } }
        /// <summary>
        /// The deadzone size of the X &amp; Y axes for the right joystick.
        /// </summary>
        public Point2D RightJoyDeadZone { get { return rightJoyDeadZone; } set { rightJoyDeadZone = value; } }

        // Trigger Calibration
        internal int lTriggerMin, lTriggerMax, rTriggerMin, rTriggerMax;
        /// <summary>
        /// The minimum value for the left trigger.
        /// </summary>
        public int LTriggerMin { get { return lTriggerMin; } set { lTriggerMin = value; } }
        /// <summary>
        /// The maximum value for the left trigger.
        /// </summary>
        public int LTriggerMax { get { return lTriggerMax; } set { lTriggerMax = value; } }
        /// <summary>
        /// The minimum value for the right trigger.
        /// </summary>
        public int RTriggerMin { get { return rTriggerMin; } set { rTriggerMin = value; } }
        /// <summary>
        /// The maximum value for the right trigger.
        /// </summary>
        public int RTriggerMax { get { return rTriggerMax; } set { rTriggerMax = value; } }
        #endregion

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ClassicControllerState()
        {
            ResetCalibration();
        }

        /// <summary>
        /// Resets the calibration setttings to default.
        /// </summary>
        public override void ResetCalibration()
        {
            if (Nintroller.UseModestConfigs)
            {
                leftJoyXCalibration = ClassicControllerCalibration.Modest.JoystickLX;
                leftJoyYCalibration = ClassicControllerCalibration.Modest.JoystickLY;
                leftJoyDeadZone = ClassicControllerCalibration.Modest.JoystickLDeadZone;

                rightJoyXCalibration = ClassicControllerCalibration.Modest.JoystickRX;
                rightJoyYCalibration = ClassicControllerCalibration.Modest.JoystickRY;
                rightJoyDeadZone = ClassicControllerCalibration.Modest.JoystickRDeadZone;

                lTriggerMin = ClassicControllerCalibration.Modest.TriggerLMin;
                lTriggerMax = ClassicControllerCalibration.Modest.TriggerLMax;

                lTriggerMin = ClassicControllerCalibration.Modest.TriggerRMin;
                lTriggerMax = ClassicControllerCalibration.Modest.TriggerRMax;
            }
            else
            {
                leftJoyXCalibration = ClassicControllerCalibration.Default.JoystickLX;
                leftJoyYCalibration = ClassicControllerCalibration.Default.JoystickLY;
                leftJoyDeadZone = ClassicControllerCalibration.Default.JoystickLDeadZone;

                rightJoyXCalibration = ClassicControllerCalibration.Default.JoystickRX;
                rightJoyYCalibration = ClassicControllerCalibration.Default.JoystickRY;
                rightJoyDeadZone = ClassicControllerCalibration.Default.JoystickRDeadZone;

                lTriggerMin = ClassicControllerCalibration.Default.TriggerLMin;
                lTriggerMax = ClassicControllerCalibration.Default.TriggerLMax;

                lTriggerMin = ClassicControllerCalibration.Default.TriggerRMin;
                lTriggerMax = ClassicControllerCalibration.Default.TriggerRMax;
            }
            /*
            LeftJoyXCalibration = LeftJoyYCalibration = new JoyCalibration(0, 31, 63);
            RightJoyXCalibration = RightJoyYCalibration = new JoyCalibration(0, 15, 31);

            LeftJoyDeadZone.X = LeftJoyDeadZone.Y = 4;
            RightJoyDeadZone.X = RightJoyDeadZone.Y = 2;

            LTriggerMin = RTriggerMin = 0;
            LTriggerMax = RTriggerMax = 31;
            */
        }

        /// <summary>
        /// Sets the calibration settings.
        /// </summary>
        /// <param name="calibration">Calibration setting</param>
        public void SetCalibration(ClassicControllerCalibration calibration)
        {
            leftJoyXCalibration = calibration.JoystickLX;
            leftJoyYCalibration = calibration.JoystickLY;
            rightJoyXCalibration = calibration.JoystickRX;
            rightJoyYCalibration = calibration.JoystickRY;
            lTriggerMin = calibration.TriggerLMin;
            lTriggerMax = calibration.TriggerLMax;
            rTriggerMin = calibration.TriggerRMin;
            rTriggerMax = calibration.TriggerRMax;
       }

        // Parse the byte report
        internal override void ParseExtension(byte[] r, int offset)
        {
            // Buttons
            a = (r[offset + 5] & 0x10) == 0;
            b = (r[offset + 5] & 0x40) == 0;
            x = (r[offset + 5] & 0x08) == 0;
            y = (r[offset + 5] & 0x20) == 0;
            lFull = (r[offset + 4] & 0x20) == 0;  // Until the Click
            rFull = (r[offset + 4] & 0x02) == 0;  // Until the Click
            zL = (r[offset + 5] & 0x80) == 0;
            zR = (r[offset + 5] & 0x04) == 0;
            plus = (r[offset + 4] & 0x04) == 0;
            minus = (r[offset + 4] & 0x10) == 0;
            home = (r[offset + 4] & 0x08) == 0;

            // Dpad
            up = (r[offset + 5] & 0x01) == 0;
            down = (r[offset + 4] & 0x40) == 0;
            left = (r[offset + 5] & 0x02) == 0;
            right = (r[offset + 4] & 0x80) == 0;

            // Joysticks
            leftJoyRaw.X = (byte)(r[offset] & 0x3f);
            leftJoyRaw.Y = (byte)(r[offset + 1] & 0x03f);
            rightJoyRaw.X = (byte)(r[offset + 2] >> 7 | (r[offset + 1] & 0xc0) >> 5 | (r[offset] & 0xc0) >> 3);
            rightJoyRaw.Y = (byte)(r[offset + 2] & 0x1f);

            // Triggers
            lTriggerRaw = (byte)(((r[offset + 2] & 0x60) >> 2) | (r[offset + 3] >> 5));
            rTriggerRaw = (byte)(r[offset + 3] & 0x1f);

            // Normaliezed Joysticks
            leftJoy.X = NintyState.NormalizeAxisValue(leftJoyRaw.X, leftJoyXCalibration, leftJoyDeadZone.X);
            leftJoy.Y = NintyState.NormalizeAxisValue(leftJoyRaw.Y, leftJoyYCalibration, leftJoyDeadZone.Y);
            rightJoy.X = NintyState.NormalizeAxisValue(rightJoyRaw.X, rightJoyXCalibration, rightJoyDeadZone.X);
            rightJoy.Y = NintyState.NormalizeAxisValue(rightJoyRaw.Y, rightJoyYCalibration, rightJoyDeadZone.Y);

            // Normalized Triggers
            if (lTriggerRaw < lTriggerMin)
                lTrigger = 0;
            else
                lTrigger = (float)(lTriggerRaw - lTriggerMin) / (float)(lTriggerMax == 0 ? ClassicControllerCalibration.Default.TriggerLMax : lTriggerMax - lTriggerMin);

            if (rTriggerRaw < rTriggerMin)
                rTrigger = 0;
            else
                rTrigger = (float)(rTriggerRaw - rTriggerMin) / (float)(rTriggerMax == 0 ? ClassicControllerCalibration.Default.TriggerRMax : rTriggerMax - rTriggerMin);

            lPressure = (lTriggerRaw > lTriggerMin + lTriggerMax / 4);
            rPressure = (rTriggerRaw > rTriggerMin + rTriggerMax / 4);
        }
    }

    /// <summary>
    /// Contains variables that describe a Classic Controller Pro
    /// </summary>
    [Serializable]
    public class ClassicControllerProState : ExtensionState
    {
        /// <summary>
        /// An Empty Classic Controller Pro State
        /// (noting active)
        /// </summary>
        public static readonly ClassicControllerProState Empty = new ClassicControllerProState();

        #region Buttons
        internal bool a, b, x, y, lButton, rButton, zL, zR, plus, minus, home, up, down, left, right;
        /// <summary>
        /// If the A button is being pressed.
        /// </summary>
        public bool A { get { return a; } }
        /// <summary>
        /// If the B button is being pressed.
        /// </summary>
        public bool B { get { return b; } }
        /// <summary>
        /// If the X button is being pressed.
        /// </summary>
        public bool X { get { return x; } }
        /// <summary>
        /// If the Y button is being pressed.
        /// </summary>
        public bool Y { get { return y; } }
        /// <summary>
        /// If the L trigger is being pressed.
        /// </summary>
        public bool L { get { return lButton; } }
        /// <summary>
        /// If the R trigger is being pressed.
        /// </summary>
        public bool R { get { return rButton; } }
        /// <summary>
        /// If the ZL button is being pressed.
        /// </summary>
        public bool ZL { get { return zL; } }
        /// <summary>
        /// If the ZR button is being pressed.
        /// </summary>
        public bool ZR { get { return zR; } }
        /// <summary>
        /// If the Start button is being pressed.
        /// </summary>
        public bool Start { get { return plus; } }
        /// <summary>
        /// If the Select button is being pressed.
        /// </summary>
        public bool Select { get { return minus; } }
        /// <summary>
        /// If the Home button is being pressed.
        /// </summary>
        public bool Home { get { return home; } }
        /// <summary>
        /// If the Up button is being pressed.
        /// </summary>
        public bool Up { get { return up; } }
        /// <summary>
        /// If the Down button is being pressed.
        /// </summary>
        public bool Down { get { return down; } }
        /// <summary>
        /// If the Left button is being pressed.
        /// </summary>
        public bool Left { get { return left; } }
        /// <summary>
        /// If the Right button is being pressed.
        /// </summary>
        public bool Right { get { return right; } }
        #endregion

        #region Joysticks
        internal Point2D leftJoyRaw, rightJoyRaw;
        internal Point2DF leftJoy, rightJoy;
        /// <summary>
        /// The Raw left joystick information.
        /// </summary>
        public Point2D LeftJoyRaw { get { return leftJoyRaw; } }
        /// <summary>
        /// The Raw right joystick information.
        /// </summary>
        public Point2D RightJoyRaw { get { return rightJoyRaw; } }
        /// <summary>
        /// The state of the left joystick.
        /// </summary>
        public Point2DF LeftJoy { get { return leftJoy; } }
        /// <summary>
        /// The state of the right joystick.
        /// </summary>
        public Point2DF RightJoy { get { return rightJoy; } }
        #endregion

        #region Joystick Calibration
        internal JoyCalibration leftJoyXCalibration, leftJoyYCalibration;
        internal JoyCalibration rightJoyXCalibration, rightJoyYCalibration;
        internal Point2D leftJoyDeadZone, rightJoyDeadZone;
        /// <summary>
        /// The minimum, maximum, and center of the Left Joy's X-axis.
        /// </summary>
        public JoyCalibration LeftJoyXCalibration { get { return leftJoyXCalibration; } set { leftJoyXCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of the Left Joy's Y-axis.
        /// </summary>
        public JoyCalibration LeftJoyYCalibration { get { return leftJoyYCalibration; } set { leftJoyYCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of the Right Joy's X-axis.
        /// </summary>
        public JoyCalibration RightJoyXCalibration { get { return rightJoyXCalibration; } set { rightJoyXCalibration = value; } }
        /// <summary>
        /// The minimum, maximum, and center of teh Right Joy's Y-axis.
        /// </summary>
        public JoyCalibration RightJoyYCalibration { get { return rightJoyYCalibration; } set { rightJoyYCalibration = value; } }
        /// <summary>
        /// The deadzone size of the X &amp; Y axes for the left joystick.
        /// </summary>
        public Point2D LeftJoyDeadZone { get { return leftJoyDeadZone; } set { leftJoyDeadZone = value; } }
        /// <summary>
        /// The deadzone size of the X &amp; Y axes for the right joystick.
        /// </summary>
        public Point2D RightJoyDeadZone { get { return rightJoyDeadZone; } set { rightJoyDeadZone = value; } }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ClassicControllerProState ()
        {
            ResetCalibration();
        }

        /// <summary>
        /// Reset the calibration setting to default.
        /// </summary>
        public override void ResetCalibration()
        {
            if (Nintroller.UseModestConfigs)
            {
                leftJoyXCalibration = ClassicControllerProCalibration.Modest.JoystickLX;
                leftJoyYCalibration = ClassicControllerProCalibration.Modest.JoystickLY;
                leftJoyDeadZone = ClassicControllerProCalibration.Modest.JoystickLDeadZone;

                rightJoyXCalibration = ClassicControllerProCalibration.Modest.JoystickRX;
                rightJoyYCalibration = ClassicControllerProCalibration.Modest.JoystickRY;
                rightJoyDeadZone = ClassicControllerProCalibration.Modest.JoystickRDeadZone;
            }
            else
            {
                leftJoyXCalibration = ClassicControllerProCalibration.Default.JoystickLX;
                leftJoyYCalibration = ClassicControllerProCalibration.Default.JoystickLY;
                leftJoyDeadZone = ClassicControllerProCalibration.Default.JoystickLDeadZone;

                rightJoyXCalibration = ClassicControllerProCalibration.Default.JoystickRX;
                rightJoyYCalibration = ClassicControllerProCalibration.Default.JoystickRY;
                rightJoyDeadZone = ClassicControllerProCalibration.Default.JoystickRDeadZone;
            }
            /*
            LeftJoyXCalibration = LeftJoyYCalibration = new JoyCalibration(0, 31, 63);
            RightJoyXCalibration = RightJoyYCalibration = new JoyCalibration(0, 15, 31);

            LeftJoyDeadZone.X = LeftJoyDeadZone.Y = 4;
            RightJoyDeadZone.X = RightJoyDeadZone.Y = 2;
            */
        }

        /// <summary>
        /// Sets the calibration settings.
        /// </summary>
        /// <param name="calibration">Calibration setting</param>
        public void SetCalibration(ClassicControllerProCalibration calibration)
        {
            leftJoyXCalibration = calibration.JoystickLX;
            leftJoyYCalibration = calibration.JoystickLY;
            rightJoyXCalibration = calibration.JoystickRX;
            rightJoyYCalibration = calibration.JoystickRY;
        }

        // Parse the byte report
        internal override void ParseExtension(byte[] r, int offset)
        {
            // Buttons
            a = (r[offset + 5] & 0x10) == 0;
            b = (r[offset + 5] & 0x40) == 0;
            x = (r[offset + 5] & 0x08) == 0;
            y = (r[offset + 5] & 0x20) == 0;
            lButton = (r[offset + 4] & 0x20) == 0;
            rButton = (r[offset + 4] & 0x02) == 0;
            zL = (r[offset + 5] & 0x80) == 0;
            zR = (r[offset + 5] & 0x04) == 0;
            plus = (r[offset + 4] & 0x04) == 0;
            minus = (r[offset + 4] & 0x10) == 0;
            home = (r[offset + 4] & 0x08) == 0;

            // Dpad
            up = (r[offset + 5] & 0x01) == 0;
            down = (r[offset + 4] & 0x40) == 0;
            left = (r[offset + 5] & 0x02) == 0;
            right = (r[offset + 4] & 0x80) == 0;

            // Joysticks
            leftJoyRaw.X = (byte)(r[offset] & 0x3f);
            leftJoyRaw.Y = (byte)(r[offset + 1] & 0x03f);
            rightJoyRaw.X = (byte)(r[offset + 2] >> 7 | (r[offset + 1] & 0xc0) >> 5 | (r[offset] & 0xc0) >> 3);
            rightJoyRaw.Y = (byte)(r[offset + 2] & 0x1f);

            // Normaliezed Joysticks
            leftJoy.X = NintyState.NormalizeAxisValue(leftJoyRaw.X, leftJoyXCalibration, leftJoyDeadZone.X);
            leftJoy.Y = NintyState.NormalizeAxisValue(leftJoyRaw.Y, leftJoyYCalibration, leftJoyDeadZone.Y);
            rightJoy.X = NintyState.NormalizeAxisValue(rightJoyRaw.X, rightJoyXCalibration, rightJoyDeadZone.X);
            rightJoy.Y = NintyState.NormalizeAxisValue(rightJoyRaw.Y, rightJoyYCalibration, rightJoyDeadZone.Y);
        }
    }

    [Serializable]
    public class MotionPlusState : ExtensionState
    {
        /// <summary>
        /// Empty Motion Plus State
        /// (nothing active)
        /// </summary>
        public static readonly MotionPlusState Empty = new MotionPlusState();

        // Gyroscope
        internal Point3D gyroRaw;
        internal Point3DF gyro;
        /// <summary>
        /// The raw values of the gyroscope.
        /// </summary>
        public Point3D GyroRaw { get { return gyroRaw; } }
        /// <summary>
        /// The normalized state of the gyroscope.
        /// </summary>
        public Point3DF Gyro { get { return gyro; } }

        // Calibration
        internal Point3D gyroStill, gyroDead;
        /// <summary>
        /// Values when the gyroscope isn't moving.
        /// </summary>
        public Point3D GyroStill { get { return gyroStill; } }
        /// <summary>
        /// Size of the gyroscope's deadzone.
        /// </summary>
        public Point3D GyroDead { get { return gyroDead; } }

        // Extension
        internal bool extensionConnected;
        internal ExtensionState extension;
        /// <summary>
        /// If an extension is currently connected.
        /// </summary>
        public bool ExtensionConnected { get { return extensionConnected; } }
        /// <summary>
        /// The state of the connected extension
        /// </summary>
        public ExtensionState Extension { get { return extension; } }

        // Parses the byte report data
        internal override void ParseExtension(byte[] r, int offset)
        {
            gyroRaw.X = (r[offset + 0] | (r[offset + 3] & 0xfa) << 6);
            gyroRaw.Y = (r[offset + 1] | (r[offset + 4] & 0xfa) << 6);
            gyroRaw.Z = (r[offset + 2] | (r[offset + 5] & 0xfa) << 6);

            /// TODO: Calculate Gryo Values
        }

        /// TODO: WiimotePlus Calibration Function
    }

    /// TODO: GuitarState
    /// TODO: DrumState
    /// TODO: TaikoDrum
    /// TODO: DJ Turn table
    #endregion
}
