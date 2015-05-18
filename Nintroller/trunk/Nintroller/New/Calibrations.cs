using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib.New
{
    public static class Calibrations
    {
        public enum CalibrationPreset
        {
            Default = 0,
            Modest
        }

        #region Properties
        public static readonly Defalut Defaults = new Defalut();
        // TODO: New: Add Modest Calibrations
        #endregion

        #region Inner Classes (Calibration Types)
        public class Defalut
        {
            public ProController ProControllerDefault = new ProController()
            {
                LJoy = new Joystick()
                {
                    centerX = 2048,
                    minX    = 1024,
                    maxX    = 3072,
                    deadX   = 128,

                    centerY = 2048,
                    minY    = 1024,
                    maxY    = 3072,
                    deadY   = 128
                },
                RJoy = new Joystick()
                {
                    centerX = 2048,
                    minX    = 1024,
                    maxX    = 3072,
                    deadX   = 128,

                    centerY = 2048,
                    minY    = 1024,
                    maxY    = 3072,
                    deadY   = 128
                }
            };

            public Wiimote WiimoteDefault = new Wiimote()
            {
                accelerometer = new Accelerometer()
                {
                    centerX = 128,
                    minX    = 80,
                    maxX    = 176,
                    deadX   = 4,

                    centerY = 128,
                    minY    = 80,
                    maxY    = 176,
                    deadY   = 4,

                    centerZ = 128,
                    minZ    = 80,
                    maxZ    = 176,
                    deadZ   = 4
                }
            };

            public Nunchuk NunchukDefault = new Nunchuk()
            {
                joystick = new Joystick()
                {
                    centerX = 128,
                    minX    = 32,
                    maxX    = 2224,
                    deadX   = 8,

                    centerY = 128,
                    minY    = 32,
                    maxY    = 2224,
                    deadY   = 8
                },
                accelerometer = new Accelerometer()
                {
                    centerX = 128,
                    minX    = 80,
                    maxX    = 176,
                    deadX   = 4,

                    centerY = 128,
                    minY    = 80,
                    maxY    = 176,
                    deadY   = 4,

                    centerZ = 128,
                    minZ    = 80,
                    maxZ    = 176,
                    deadZ   = 4
                }
            };

            public ClassicController ClassicControllerDefault = new ClassicController()
            {
                LJoy = new Joystick()
                {
                    centerX = 31,
                    minX    = 0,
                    maxX    = 63,
                    deadX   = 4,

                    centerY = 31,
                    minY    = 0,
                    maxY    = 63,
                    deadY   = 4
                },
                RJoy = new Joystick()
                {
                    centerX = 15,
                    minX    = 0,
                    maxX    = 31,
                    deadX   = 2,

                    centerY = 15,
                    minY    = 0,
                    maxY    = 31,
                    deadY   = 2
                },
                L = new Trigger()
                {
                    min = 0,
                    max = 31
                },
                R = new Trigger()
                {
                    min = 0,
                    max = 31
                }
            };

            public ClassicControllerPro ClassicControllerProDefault = new ClassicControllerPro()
            {
                LJoy = new Joystick()
                {
                    centerX = 31,
                    minX    = 0,
                    maxX    = 63,
                    deadX   = 4,

                    centerY = 31,
                    minY    = 0,
                    maxY    = 63,
                    deadY   = 4
                },
                RJoy = new Joystick()
                {
                    centerX = 15,
                    minX    = 0,
                    maxX    = 31,
                    deadX   = 2,

                    centerY = 15,
                    minY    = 0,
                    maxY    = 31,
                    deadY   = 2
                }
            };
        }
        #endregion
    }
}
