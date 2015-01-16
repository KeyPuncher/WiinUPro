using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NintrollerLib
{
    #region Structures
    /// <summary>
    /// 2D point with X &amp; Y
    /// </summary>
    [Serializable]
    public struct Point2D
    {
        public int X, Y;

        public Point2D(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}}}", X, Y);
        }
    }

    /// <summary>
    /// 2D point with X &amp; Y as float values
    /// </summary>
    [Serializable]
    public struct Point2DF
    {
        public float X, Y;

        public Point2DF(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}}}", X, Y);
        }
    }

    /// <summary>
    /// 3D point with X, Y, &amp; Z
    /// </summary>
    [Serializable]
    public struct Point3D
    {
        public int X, Y, Z;

        public Point3D(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}, Z={2}}}", X, Y, Z);
        }
    }

    /// <summary>
    /// 3D point with X, Y, &amp; Z as float values
    /// </summary>
    [Serializable]
    public struct Point3DF
    {
        public float X, Y, Z;

        public Point3DF(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}, Z={2}}}", X, Y, Z);
        }
    }

    /// <summary>
    /// Information that an IR point can return.
    /// Position, Raw Position, Size, and in camera view.
    /// </summary>
    [Serializable]
    public struct IRPoint
    {
        public int RawX, RawY, Size;
        public float X, Y;
        public bool InView;
    }

    /// <summary>
    /// Calibration settings for a Joystick.
    /// Minimum, Maximum, and Middle/Center
    /// </summary>
    [Serializable]
    public struct JoyCalibration
    {
        public int Min, Mid, Max;

        public JoyCalibration(int min, int mid, int max) 
        {
            Min = min;
            Mid = mid;
            Max = max;
        }

        public override string ToString()
        {
            return string.Format("{{Min={0}, Mid={1}, Max={2}}}", Min, Mid, Max);
        }
    }

    /// <summary>
    /// Information that a balance board can give.
    /// </summary>
    [Serializable]
    public struct BalanceBoardSensor
    {
        public short TopLeft, TopRight, BottomLeft, BottomRight;

        public BalanceBoardSensor(short topLeft, short topRight, short bottomLeft, short bottomRight)
        {
            this.TopLeft = topLeft;
            this.TopRight = topRight;
            this.BottomLeft = bottomLeft;
            this.BottomRight = bottomRight;
        }
    }
    #endregion

    #region Enumerators
    public enum IRSetting : byte
    {
        Off     = 0x00,
        Basic   = 0x01,     // 10 bytes
        Wide    = 0x03,     // 12 bytes
        Full    = 0x05      // two sets of 16 bytes
    };

    public enum BatteryStatus
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh
    };

    public enum InputReport : byte
    {
        // Status Reports
        Status          = 0x20,
        ReadMem         = 0x21,
        Acknowledge     = 0x22,

        // Data Reports
        BtnsOnly        = 0x30,
        BtnsAcc         = 0x31,
        BtnsExt         = 0x32,
        BtnsAccIR       = 0x33,
        BtnsExtB        = 0x34,
        BtnsAccExt      = 0x35,
        BtnsIRExt       = 0x36,
        BtnsAccIRExt    = 0x37,
        ExtOnly         = 0x3d,
    };

    public enum OutputReport : byte
    {
        LEDs            = 0x11,
        DataReportMode  = 0x12,
        IREnable        = 0x13,
        SpeakerEnable   = 0x14,
        StatusRequest   = 0x15,
        WriteMemory     = 0x16,
        ReadMemory      = 0x17,
        SpeakerData     = 0x18,
        SpeakerMute     = 0x19,
        IREnable2       = 0x1a
    };

    public enum ControllerType : long
    {
        // Wiimote, no extension
        Wiimote             = 0x000000000000,
        // Wii U Pro Controller
        ProController       = 0x0000a4200120,
        // Balance Board
        BalanceBoard        = 0x0000a4200402,
        // Wiimote + Nunchuk
        Nunchuk             = 0x0000a4200000,
        // Wiimote + Nunchuk (type 2)
        NunchukB            = 0xff00a4200000,
        // Wiimote + Classic Controller
        ClassicController   = 0x0000a4200101,
        // Wiimote + Classic Controller Pro
        ClassicControllerPro = 0x0100a4200101,
        // Wiimote + Motion Plus
        MotionPlus          = 0x0000a4200405,
        // Musical
        Guitar              = 0x0000a4200103,
        Drums               = 0x0100a4200103,
        TaikoDrum           = 0x0000a4200111,

        PartiallyInserted   = 0xffffffffffff
    };
    #endregion

    #region Calibration Structs
    /// <summary>
    /// Calibration settings for a Wiimote.
    /// </summary>
    [Serializable]
    public struct WiimoteCalibration
    {
        public Point3D AccelerometerCenter, AccelerometerDeadzone, AccelerometerRange;

        public WiimoteCalibration(Point3D center, Point3D deadzone, Point3D range)
        {
            AccelerometerCenter = center;
            AccelerometerDeadzone = deadzone;
            AccelerometerRange = range;
        }

        public static readonly WiimoteCalibration Empty = new WiimoteCalibration();

        /// <summary>
        /// The Wiimote's default calibration
        /// </summary>
        public static readonly WiimoteCalibration Default = new WiimoteCalibration() 
        {
            AccelerometerCenter = new Point3D(128, 128, 128),
            AccelerometerDeadzone = new Point3D(4, 4, 4),
            AccelerometerRange = new Point3D(48, 48, 48)
        };

        /// <summary>
        /// A modest calibration setting with larger deadzones and lower ranges.
        /// </summary>
        public static readonly WiimoteCalibration Modest = new WiimoteCalibration()
        {
            AccelerometerCenter = new Point3D(128, 128, 128),
            AccelerometerDeadzone = new Point3D(8, 8, 8),
            AccelerometerRange = new Point3D(44, 44, 44)
        };
    }

    /// <summary>
    /// Calibration settings for a Pro Controller
    /// </summary>
    [Serializable]
    public struct ProCalibration
    {
        public JoyCalibration JoystickLX, JoystickLY, JoystickRX, JoystickRY;
        public Point2D JoystickLDeadZone, JoystickRDeadZone;

        public static readonly ProCalibration Empty = new ProCalibration();

        /// <summary>
        /// The Pro Controller's default calibration
        /// </summary>
        public static readonly ProCalibration Default = new ProCalibration()
        {
            JoystickLX = new JoyCalibration(1024, 2048, 3072),
            JoystickLY = new JoyCalibration(1024, 2048, 3072),
            JoystickRX = new JoyCalibration(1024, 2048, 3072),
            JoystickRY = new JoyCalibration(1024, 2048, 3072),
            JoystickLDeadZone = new Point2D(128, 128),
            JoystickRDeadZone = new Point2D(128, 128)
        };

        /// <summary>
        /// A modest calibration setting with smaller deadzones and lower ranges.
        /// </summary>
        public static readonly ProCalibration Modest = new ProCalibration()
        {
            JoystickLX = new JoyCalibration(1152, 2048, 2944),
            JoystickLY = new JoyCalibration(1152, 2048, 2944),
            JoystickRX = new JoyCalibration(1152, 2048, 2944),
            JoystickRY = new JoyCalibration(1152, 2048, 2944),
            JoystickLDeadZone = new Point2D(192, 192),
            JoystickRDeadZone = new Point2D(192, 192)
        };
    }

    /// <summary>
    /// Calibration settings for a Nunchuck
    /// </summary>
    [Serializable]
    public struct NunchuckCalibration
    {
        public JoyCalibration JoystickX, JoystickY;
        public Point2D JoystickDeadzone;
        public Point3D AccelerometerCenter, AccelerometerDeadzone, AccelerometerRange;

        public static readonly NunchuckCalibration Empty = new NunchuckCalibration();

        /// <summary>
        /// The Nunchuck's default calibration
        /// </summary>
        public static readonly NunchuckCalibration Default = new NunchuckCalibration()
        {
            JoystickX = new JoyCalibration(32, 128, 224),
            JoystickY = new JoyCalibration(32, 128, 224),
            JoystickDeadzone = new Point2D(8, 8),
            AccelerometerCenter = new Point3D(128, 128, 128),
            AccelerometerDeadzone = new Point3D(4, 4, 4),
            AccelerometerRange = new Point3D(112, 112, 112)
        };

        /// <summary>
        /// A modest calibration setting with larger deadzones and lower ranges.
        /// </summary>
        public static readonly NunchuckCalibration Modest = new NunchuckCalibration()
        {
            JoystickX = new JoyCalibration(40, 128, 216),
            JoystickY = new JoyCalibration(40, 128, 216),
            JoystickDeadzone = new Point2D(8, 8),
            AccelerometerCenter = new Point3D(128, 128, 128),
            AccelerometerDeadzone = new Point3D(8, 8, 8),
            AccelerometerRange = new Point3D(104, 104, 104)
        };
    }

    /// <summary>
    /// Calibration settings for a Classic Controller
    /// </summary>
    [Serializable]
    public struct ClassicControllerCalibration
    {
        public JoyCalibration JoystickLX, JoystickLY, JoystickRX, JoystickRY;
        public Point2D JoystickLDeadZone, JoystickRDeadZone;
        public int TriggerLMin, TriggerLMax, TriggerRMin, TriggerRMax;

        public static readonly ClassicControllerCalibration Empty = new ClassicControllerCalibration();

        /// <summary>
        /// The Classic Controller's default calibration
        /// </summary>
        public static readonly ClassicControllerCalibration Default = new ClassicControllerCalibration()
        {
            JoystickLX = new JoyCalibration(0, 31, 63),
            JoystickLY = new JoyCalibration(0, 31, 63),
            JoystickRX = new JoyCalibration(0, 15, 31),
            JoystickRY = new JoyCalibration(0, 15, 31),
            JoystickLDeadZone = new Point2D(4, 4),
            JoystickRDeadZone = new Point2D(2, 2),
            TriggerLMin = 0,
            TriggerLMax = 31,
            TriggerRMin = 0,
            TriggerRMax = 31
        };

        /// <summary>
        /// A modest calibration setting with larger deadzones and lower ranges.
        /// </summary>
        public static readonly ClassicControllerCalibration Modest = new ClassicControllerCalibration()
        {
            JoystickLX = new JoyCalibration(8, 31, 55),
            JoystickLY = new JoyCalibration(8, 31, 55),
            JoystickRX = new JoyCalibration(4, 15, 27),
            JoystickRY = new JoyCalibration(4, 15, 27),
            JoystickLDeadZone = new Point2D(8, 8), 
            JoystickRDeadZone = new Point2D(4, 4),
            TriggerLMin = 4,
            TriggerLMax = 27,
            TriggerRMin = 4,
            TriggerRMax = 27
        };
    }

    /// <summary>
    /// Calibration settings for a Classic Controller Pro
    /// </summary>
    [Serializable]
    public struct ClassicControllerProCalibration
    {
        public JoyCalibration JoystickLX, JoystickLY, JoystickRX, JoystickRY;
        public Point2D JoystickLDeadZone, JoystickRDeadZone;

        public static readonly ClassicControllerProCalibration Empty = new ClassicControllerProCalibration();

        /// <summary>
        /// The Classic Controller Pro's default calibraition
        /// </summary>
        public static readonly ClassicControllerProCalibration Default = new ClassicControllerProCalibration()
        {
            JoystickLX = new JoyCalibration(0, 31, 63),
            JoystickLY = new JoyCalibration(0, 31, 63),
            JoystickRX = new JoyCalibration(0, 15, 31),
            JoystickRY = new JoyCalibration(0, 15, 31),
            JoystickLDeadZone = new Point2D(4, 4),
            JoystickRDeadZone = new Point2D(2, 2)
        };

        /// <summary>
        /// A modest calibration setting with larger deadzones and lower ranges.
        /// </summary>
        public static readonly ClassicControllerProCalibration Modest = new ClassicControllerProCalibration()
        {
            JoystickLX = new JoyCalibration(8, 31, 55),
            JoystickLY = new JoyCalibration(8, 31, 55),
            JoystickRX = new JoyCalibration(4, 15, 27),
            JoystickRY = new JoyCalibration(4, 15, 27),
            JoystickLDeadZone = new Point2D(8, 8),
            JoystickRDeadZone = new Point2D(4, 4)
        };
    }

    [Serializable]
    public struct MotionPlusCalibration
    {
        public Point3D GyroscopeStill, GyroscopeDeadzone;

        public static readonly MotionPlusCalibration Empty = new MotionPlusCalibration();

        // TODO: Motion Plus Default Calibration
    }
    #endregion
}
