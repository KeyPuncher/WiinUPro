using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib
{
    public struct Wiimote : INintrollerState
    {
        public CoreButtons buttons;
        public Accelerometer accelerometer;
        public IR irSensor;
        //INintrollerState extension;

        public Wiimote(byte[] rawData)
        {
            buttons = new CoreButtons();
            accelerometer = new Accelerometer();
            irSensor = new IR();
            //extension = null;

            Update(rawData);
        }

        // TODO: Address the issue that made this change necessary.
        // Most likely will be in a performance & memory optimization refactor.
        public Wiimote(byte[] rawData, Wiimote calibration)
        {
            buttons = new CoreButtons();
            accelerometer = new Accelerometer();
            irSensor = new IR();

            SetCalibration(calibration);
            Update(rawData);
        }

        public void Update(byte[] data)
        {
            InputReport reportType = (InputReport)data[0];

            if (Utils.ReportContainsCoreButtons(reportType))
                buttons.Parse(data, 1);

            if (Utils.ReportContainsAccelerometer(reportType))
                accelerometer.Parse(data, 3);

            if (reportType == InputReport.BtnsAccIR) // 12 bytes
                irSensor.Parse(data, 6, false);
            else if (reportType == InputReport.BtnsIRExt) // 10 bytes
                irSensor.Parse(data, 3, true);
            else if (reportType == InputReport.BtnsAccIRExt) // 10 bytes
                irSensor.Parse(data, 6, true);

            accelerometer.Normalize();
        }

        public float GetValue(string input)
        {
            switch (input)
            {
                case INPUT_NAMES.WIIMOTE.A: return buttons.A ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.B: return buttons.B ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.ONE: return buttons.One ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.TWO: return buttons.Two ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.UP: return buttons.Up ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.DOWN: return buttons.Down ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.LEFT: return buttons.Left ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.RIGHT: return buttons.Right ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.MINUS: return buttons.Minus ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.PLUS: return buttons.Plus ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.HOME: return buttons.Home ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.ACC_X: return accelerometer.X;
                case INPUT_NAMES.WIIMOTE.ACC_Y: return accelerometer.Y;
                case INPUT_NAMES.WIIMOTE.ACC_Z: return accelerometer.Z;
                case INPUT_NAMES.WIIMOTE.TILT_RIGHT: return accelerometer.X > 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.TILT_LEFT: return accelerometer.X < 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.TILT_UP: return accelerometer.Y > 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.TILT_DOWN: return accelerometer.Y < 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.FACE_UP: return accelerometer.Z > 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.FACE_DOWN: return accelerometer.Z < 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.IR_X: return irSensor.X;
                case INPUT_NAMES.WIIMOTE.IR_Y: return irSensor.Y;
                case INPUT_NAMES.WIIMOTE.IR_UP: return irSensor.Y > 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.IR_DOWN: return irSensor.Y < 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.IR_LEFT: return irSensor.X < 0 ? 1 : 0;
                case INPUT_NAMES.WIIMOTE.IR_RIGHT: return irSensor.X > 0 ? 1 : 0;
            }

            return 0;
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    SetCalibration(Calibrations.Defaults.WiimoteDefault);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    SetCalibration(Calibrations.Moderate.WiimoteModest);
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    SetCalibration(Calibrations.Extras.WiimoteExtra);
                    break;

                case Calibrations.CalibrationPreset.Minimum:
                    SetCalibration(Calibrations.Minimum.WiimoteMinimal);
                    break;

                case Calibrations.CalibrationPreset.None:
                    SetCalibration(Calibrations.None.WiimoteRaw);
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            if (from.CalibrationEmpty)
            {
                // don't apply empty calibrations
                return;
            }

            if (from.GetType() == typeof(Wiimote))
            {
                accelerometer.Calibrate(((Wiimote)from).accelerometer);
                irSensor.deadArea = ((Wiimote)from).irSensor.deadArea;
                irSensor.leftBounds = ((Wiimote)from).irSensor.leftBounds;
                irSensor.rightBounds = ((Wiimote)from).irSensor.rightBounds;
                irSensor.topBounds = ((Wiimote)from).irSensor.topBounds;
                irSensor.bottomBounds = ((Wiimote)from).irSensor.bottomBounds;
                irSensor.offscreenBehavior = ((Wiimote)from).irSensor.offscreenBehavior;
            }
            else if (from.GetType() == typeof(Nunchuk))
            {
                accelerometer.Calibrate(((Nunchuk)from).wiimote.accelerometer);
                irSensor.deadArea = ((Nunchuk)from).wiimote.irSensor.deadArea;
                irSensor.leftBounds = ((Nunchuk)from).wiimote.irSensor.leftBounds;
                irSensor.rightBounds = ((Nunchuk)from).wiimote.irSensor.rightBounds;
                irSensor.topBounds = ((Nunchuk)from).wiimote.irSensor.topBounds;
                irSensor.bottomBounds = ((Nunchuk)from).wiimote.irSensor.bottomBounds;
                irSensor.offscreenBehavior = ((Nunchuk)from).wiimote.irSensor.offscreenBehavior;
            }
            else if (from.GetType() == typeof(ClassicController))
            {
                accelerometer.Calibrate(((ClassicController)from).wiimote.accelerometer);
                irSensor.deadArea = ((ClassicController)from).wiimote.irSensor.deadArea;
                irSensor.leftBounds = ((ClassicController)from).wiimote.irSensor.leftBounds;
                irSensor.rightBounds = ((ClassicController)from).wiimote.irSensor.rightBounds;
                irSensor.topBounds = ((ClassicController)from).wiimote.irSensor.topBounds;
                irSensor.bottomBounds = ((ClassicController)from).wiimote.irSensor.bottomBounds;
                irSensor.offscreenBehavior = ((ClassicController)from).wiimote.irSensor.offscreenBehavior;
            }
            else if (from.GetType() == typeof(ClassicControllerPro))
            {
                accelerometer.Calibrate(((ClassicControllerPro)from).wiimote.accelerometer);
                irSensor.deadArea = ((ClassicControllerPro)from).wiimote.irSensor.deadArea;
                irSensor.leftBounds = ((ClassicControllerPro)from).wiimote.irSensor.leftBounds;
                irSensor.rightBounds = ((ClassicControllerPro)from).wiimote.irSensor.rightBounds;
                irSensor.topBounds = ((ClassicControllerPro)from).wiimote.irSensor.topBounds;
                irSensor.bottomBounds = ((ClassicControllerPro)from).wiimote.irSensor.bottomBounds;
                irSensor.offscreenBehavior = ((ClassicControllerPro)from).wiimote.irSensor.offscreenBehavior;
            }
        }

        public void SetCalibration(string calibrationString)
        {
            if (calibrationString.Count(c => c == '0') > 5)
            {
                // don't set empty calibrations
                return;
            }

            string[] components = calibrationString.Split(new char[] {':'});

            foreach (string component in components)
            {
                if (component.StartsWith("acc"))
                {
                    string[] accConfig = component.Split(new char[] { '|' });

                    for (int a = 1; a < accConfig.Length; a++)
                    {
                        int value = 0;
                        if (int.TryParse(accConfig[a], out value))
                        {
                            switch (a)
                            {
                                case 1:  accelerometer.centerX = value; break;
                                case 2:  accelerometer.minX    = value; break;
                                case 3:  accelerometer.maxX    = value; break;
                                case 4:  accelerometer.deadX   = value; break;
                                case 5:  accelerometer.centerY = value; break;
                                case 6:  accelerometer.minY    = value; break;
                                case 7:  accelerometer.maxY    = value; break;
                                case 8:  accelerometer.deadY   = value; break;
                                case 9:  accelerometer.centerZ = value; break;
                                case 10: accelerometer.minZ    = value; break;
                                case 11: accelerometer.maxZ    = value; break;
                                case 12: accelerometer.deadZ   = value; break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("ir"))
                {
                    string[] irConfig = component.Split(new char[] { '|' });

                    for (int r = 0; r < irConfig.Length; r++)
                    {
                        if (int.TryParse(irConfig[r], out int value))
                        {
                            switch (r)
                            {
                                case 1: irSensor.offscreenBehavior = (IRCamOffscreenBehavior)value; break;
                                case 2: irSensor.minimumVisiblePoints = (IRCamMinimumVisiblePoints)value; break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("irArea"))
                {
                    string[] irArea = component.Split(new char[] { '|' });

                    for (int r = 0; r < irArea.Length; r++)
                    {
                        if (int.TryParse(irArea[r], out int value))
                        {
                            switch (r)
                            {
                                case 1: irSensor.leftBounds = value; break;
                                case 2: irSensor.rightBounds = value; break;
                                case 3: irSensor.topBounds = value; break;
                                case 4: irSensor.bottomBounds = value; break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("irSqr"))
                {
                    SquareBoundry sBoundry = new SquareBoundry();
                    string[] sqrConfig = component.Split(new char[] { '|' });

                    for (int s = 1; s < sqrConfig.Length; s++)
                    {
                        int value = 0;
                        if (int.TryParse(sqrConfig[s], out value))
                        {
                            switch (s)
                            {
                                case 1: sBoundry.center_x = value; break;
                                case 2: sBoundry.center_y = value; break;
                                case 3: sBoundry.width = value; break;
                                case 4: sBoundry.height = value; break;
                            }
                        }
                    }

                    irSensor.deadArea = sBoundry;
                }
                else if (component.StartsWith("irCir"))
                {
                    CircularBoundry sBoundry = new CircularBoundry();
                    string[] cirConfig = component.Split(new char[] { '|' });

                    for (int c = 1; c < cirConfig.Length; c++)
                    {
                        int value = 0;
                        if (int.TryParse(cirConfig[c], out value))
                        {
                            switch (c)
                            {
                                case 1: sBoundry.center_x = value; break;
                                case 2: sBoundry.center_y = value; break;
                                case 3: sBoundry.radius = value; break;
                            }
                        }
                    }

                    irSensor.deadArea = sBoundry;
                }
            }
        }

        /// <summary>
        /// Creates a string containing the calibration settings for the Wiimote.
        /// String is in the following format 
        /// -wm:acc|centerX|minX|minY|deadX|centerY|[...]:ir
        /// </summary>
        /// <remarks>I guess I didn't want to use JSON ??</remarks>
        /// <returns>String representing the Wiimote's calibration settings.</returns>
        public string GetCalibrationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-wm");
                sb.Append(":acc");
                    sb.Append("|"); sb.Append(accelerometer.centerX);
                    sb.Append("|"); sb.Append(accelerometer.minX);
                    sb.Append("|"); sb.Append(accelerometer.maxX);
                    sb.Append("|"); sb.Append(accelerometer.deadX);

                    sb.Append("|"); sb.Append(accelerometer.centerY);
                    sb.Append("|"); sb.Append(accelerometer.minY);
                    sb.Append("|"); sb.Append(accelerometer.maxY);
                    sb.Append("|"); sb.Append(accelerometer.deadY);

                    sb.Append("|"); sb.Append(accelerometer.centerZ);
                    sb.Append("|"); sb.Append(accelerometer.minZ);
                    sb.Append("|"); sb.Append(accelerometer.maxZ);
                    sb.Append("|"); sb.Append(accelerometer.deadZ);

                sb.Append(":ir");
                    sb.Append("|"); sb.Append((int)irSensor.offscreenBehavior);
                    sb.Append("|"); sb.Append((int)irSensor.minimumVisiblePoints);

                sb.Append(":irArea");
                    sb.Append("|"); sb.Append(irSensor.leftBounds);
                    sb.Append("|"); sb.Append(irSensor.rightBounds);
                    sb.Append("|"); sb.Append(irSensor.topBounds);
                    sb.Append("|"); sb.Append(irSensor.bottomBounds);

            if (irSensor.deadArea != null)
            {
                if (irSensor.deadArea is SquareBoundry)
                {
                    SquareBoundry sqr = (SquareBoundry)irSensor.deadArea;
                    sb.Append(":irSqr");
                        sb.Append("|"); sb.Append(sqr.center_x);
                        sb.Append("|"); sb.Append(sqr.center_y);
                        sb.Append("|"); sb.Append(sqr.width);
                        sb.Append("|"); sb.Append(sqr.height);
                }
                else if (irSensor.deadArea is CircularBoundry)
                {
                    CircularBoundry cir = (CircularBoundry)irSensor.deadArea;
                    sb.Append(":irCir");
                        sb.Append("|"); sb.Append(cir.center_x);
                        sb.Append("|"); sb.Append(cir.center_y);
                        sb.Append("|"); sb.Append(cir.radius);
                }
            }

            return sb.ToString();
        }

        public bool CalibrationEmpty
        {
            get 
            { 
                if (accelerometer.maxX == 0 && accelerometer.maxY == 0 && accelerometer.maxZ == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            // Buttons
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.PLUS, buttons.Plus ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.MINUS, buttons.Minus ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.HOME, buttons.Home ? 1.0f : 0.0f);

            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.A, buttons.A ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.B, buttons.B ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.ONE, buttons.One ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TWO, buttons.Two ? 1.0f : 0.0f);

            // D-Pad
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.UP, buttons.Up ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.DOWN, buttons.Down ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.LEFT, buttons.Left ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.RIGHT, buttons.Right ? 1.0f : 0.0f);

            // IR Sensor
            irSensor.Normalize();
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_X, irSensor.X);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_Y, irSensor.Y);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_UP, irSensor.Y > 0 ? irSensor.Y : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_DOWN, irSensor.Y < 0 ? -irSensor.Y : 0); // Fixed line for IR Sensor movement.
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_LEFT, irSensor.X < 0 ? -irSensor.X : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_RIGHT, irSensor.X > 0 ? irSensor.X : 0);

            // Accelerometer
            accelerometer.Normalize();
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.ACC_X, accelerometer.X);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.ACC_Y, accelerometer.Y);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.ACC_Z, accelerometer.Z);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TILT_LEFT, accelerometer.X < 0 ? -accelerometer.X : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TILT_RIGHT, accelerometer.X > 0 ? accelerometer.X : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TILT_UP, accelerometer.Y > 0 ? accelerometer.Y : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TILT_DOWN, accelerometer.Y < 0 ? -accelerometer.Y : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.FACE_UP, accelerometer.Z > 0 ? accelerometer.Z : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.FACE_DOWN, accelerometer.Z < 0 ? -accelerometer.Z : 0);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
