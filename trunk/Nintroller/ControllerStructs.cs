using System;
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

        public void Update(byte[] data)
        {
            buttons.Parse(data, 1);
            accelerometer.Parse(data, 3);
            irSensor.Parse(data, 3);

            accelerometer.Normalize();
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    //accelerometer.Calibrate(Calibrations.Defaults.WiimoteDefault.accelerometer);
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
                irSensor.boundingArea = ((Wiimote)from).irSensor.boundingArea;
            }
            else if (from.GetType() == typeof(Nunchuk))
            {
                accelerometer.Calibrate(((Nunchuk)from).wiimote.accelerometer);
                irSensor.boundingArea = ((Nunchuk)from).wiimote.irSensor.boundingArea;
            }
            else if (from.GetType() == typeof(ClassicController))
            {
                accelerometer.Calibrate(((ClassicController)from).wiimote.accelerometer);
                irSensor.boundingArea = ((ClassicController)from).wiimote.irSensor.boundingArea;
            }
            else if (from.GetType() == typeof(ClassicControllerPro))
            {
                accelerometer.Calibrate(((ClassicControllerPro)from).wiimote.accelerometer);
                irSensor.boundingArea = ((ClassicControllerPro)from).wiimote.irSensor.boundingArea;
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

                    irSensor.boundingArea = sBoundry;
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

                    irSensor.boundingArea = sBoundry;
                }
            }
        }

        /// <summary>
        /// Creates a string containing the calibration settings for the Wiimote.
        /// String is in the following format 
        /// -wm:acc|centerX|minX|minY|deadX|centerY|[...]:ir
        /// </summary>
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
                
            if (irSensor.boundingArea != null)
            {
                if (irSensor.boundingArea is SquareBoundry)
                {
                    SquareBoundry sqr = (SquareBoundry)irSensor.boundingArea;
                    sb.Append(":irSqr");
                        sb.Append("|"); sb.Append(sqr.center_x);
                        sb.Append("|"); sb.Append(sqr.center_y);
                        sb.Append("|"); sb.Append(sqr.width);
                        sb.Append("|"); sb.Append(sqr.height);
                }
                else if (irSensor.boundingArea is CircularBoundry)
                {
                    CircularBoundry cir = (CircularBoundry)irSensor.boundingArea;
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
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_X, irSensor.X);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_Y, irSensor.Y);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_UP, irSensor.Y > 0 ? irSensor.Y : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_DOWN, irSensor.Y < 0 ? irSensor.Y : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_LEFT, irSensor.X < 0 ? irSensor.X : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.IR_RIGHT, irSensor.X > 0 ? irSensor.X : 0);

            // Accelerometer
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.ACC_X, accelerometer.X);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.ACC_Y, accelerometer.Y);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.ACC_Z, accelerometer.Z);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TILT_LEFT, accelerometer.X < 0 ? accelerometer.X : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TILT_RIGHT, accelerometer.X > 0 ? accelerometer.X : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TILT_UP, accelerometer.Y > 0 ? accelerometer.Y : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.TILT_DOWN, accelerometer.Y < 0 ? accelerometer.Y : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.FACE_UP, accelerometer.Z > 0 ? accelerometer.Z : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.WIIMOTE.FACE_DOWN, accelerometer.Z < 0 ? accelerometer.Z : 0);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct Nunchuk : INintrollerState
    {
        public Wiimote wiimote;
        public Accelerometer accelerometer;
        public Joystick joystick;
        public bool C, Z;

        public Nunchuk(Wiimote wm)
        {
            this = new Nunchuk();
            wiimote = wm;
        }

        public Nunchuk(byte[] rawData)
        {
            wiimote = new Wiimote(rawData);
            accelerometer = new Accelerometer();
            joystick = new Joystick();

            C = Z = false;
            Update(rawData);
        }

        public void Update(byte[] data)
        {
            int offset = 0;
            switch((InputReport)data[0])
            {
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
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                case InputReport.Status:
                    offset = -1;
                    break;
                default:
                    return;
            }

            if (offset > 0)
            {
                // Buttons
                C = (data[offset + 5] & 0x02) == 0;
                Z = (data[offset + 5] & 0x01) == 0;

                // Joystick
                joystick.rawX = data[offset];
                joystick.rawY = data[offset + 1];

                // Accelerometer
                accelerometer.Parse(data, offset + 2);

                // Normalize
                joystick.Normalize();
                accelerometer.Normalize();
            }

            wiimote.Update(data);
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);

            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    SetCalibration(Calibrations.Defaults.NunchukDefault);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    SetCalibration(Calibrations.Moderate.NunchukModest);
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    SetCalibration(Calibrations.Extras.NunchukExtra);
                    break;

                case Calibrations.CalibrationPreset.Minimum:
                    SetCalibration(Calibrations.Minimum.NunchukMinimal);
                    break;

                case Calibrations.CalibrationPreset.None:
                    SetCalibration(Calibrations.None.NunchukRaw);
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

            if (from.GetType() == typeof(Nunchuk))
            {
                accelerometer.Calibrate(((Nunchuk)from).accelerometer);
                joystick.Calibrate(((Nunchuk)from).joystick);
            }
            else if (from.GetType() == typeof(Wiimote))
            {
                wiimote.SetCalibration(from);
            }
        }

        public void SetCalibration(string calibrationString)
        {
            if (calibrationString.Count(c => c == '0') > 5)
            {
                // don't set empty calibrations
                return;
            }

            string[] components = calibrationString.Split(new char[] { ':' });

            foreach (string component in components)
            {
                if (component.StartsWith("joy"))
                {
                    string[] joyConfig = component.Split(new char[] { '|' });

                    for (int j = 1; j < joyConfig.Length; j++)
                    {
                        int value = 0;
                        if (int.TryParse(joyConfig[j], out value))
                        {
                            switch (j)
                            {
                                case 1: joystick.centerX = value; break;
                                case 2: joystick.minX    = value; break;
                                case 3: joystick.maxX    = value; break;
                                case 4: joystick.deadX   = value; break;
                                case 5: joystick.centerY = value; break;
                                case 6: joystick.minY    = value; break;
                                case 7: joystick.maxY    = value; break;
                                case 8: joystick.deadY   = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("acc"))
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
                                default: break;
                            }
                        }
                    }
                }
            }
        }

        public string GetCalibrationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-nun");
                sb.Append(":joy");
                    sb.Append("|"); sb.Append(joystick.centerX);
                    sb.Append("|"); sb.Append(joystick.minX);
                    sb.Append("|"); sb.Append(joystick.maxX);
                    sb.Append("|"); sb.Append(joystick.deadX);

                    sb.Append("|"); sb.Append(joystick.centerY);
                    sb.Append("|"); sb.Append(joystick.minY);
                    sb.Append("|"); sb.Append(joystick.maxY);
                    sb.Append("|"); sb.Append(joystick.deadY);
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
                else if (joystick.maxX == 0 && joystick.maxY == 0)
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
            foreach (var input in wiimote)
            {
                yield return input;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct ClassicController : INintrollerState
    {
        public Wiimote wiimote;
        public Joystick LJoy, RJoy;
        public Trigger L, R;
        public bool A, B, X, Y;
        public bool Up, Down, Left, Right;
        public bool ZL, ZR, LFull, RFull;
        public bool Plus, Minus, Home;

        public ClassicController(Wiimote wm)
        {
            this = new ClassicController();
            wiimote = wm;
        }

        public bool Start
        {
            get { return Plus; }
            set { Plus = value; }
        }

        public bool Select
        {
            get { return Minus; }
            set { Minus = value; }
        }

        public void Update(byte[] data)
        {
            int offset = 0;
            switch ((InputReport)data[0])
            {
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
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                default:
                    return;
            }

            if (offset > 0)
            {
                // Buttons
                A     = (data[offset + 5] & 0x10) == 0;
                B     = (data[offset + 5] & 0x40) == 0;
                X     = (data[offset + 5] & 0x08) == 0;
                Y     = (data[offset + 5] & 0x20) == 0;
                LFull = (data[offset + 4] & 0x20) == 0;  // Until the Click
                RFull = (data[offset + 4] & 0x02) == 0;  // Until the Click
                ZL    = (data[offset + 5] & 0x80) == 0;
                ZR    = (data[offset + 5] & 0x04) == 0;
                Plus  = (data[offset + 4] & 0x04) == 0;
                Minus = (data[offset + 4] & 0x10) == 0;
                Home  = (data[offset + 4] & 0x08) == 0;

                // Dpad
                Up    = (data[offset + 5] & 0x01) == 0;
                Down  = (data[offset + 4] & 0x40) == 0;
                Left  = (data[offset + 5] & 0x02) == 0;
                Right = (data[offset + 4] & 0x80) == 0;

                // Joysticks
                LJoy.rawX = (byte)(data[offset] & 0x3F);
                LJoy.rawY = (byte)(data[offset + 1] & 0x03F);
                RJoy.rawX = (byte)(data[offset + 2] >> 7 | (data[offset + 1] & 0xC0) >> 5 | (data[offset] & 0xC0) >> 3);
                RJoy.rawY = (byte)(data[offset + 2] & 0x1F);

                // Triggers
                L.rawValue = (byte)(((data[offset + 2] & 0x60) >> 2) | (data[offset + 3] >> 5));
                R.rawValue = (byte)(data[offset + 3] & 0x1F);
                L.full = LFull;
                R.full = RFull;

                // Normalize
                LJoy.Normalize();
                RJoy.Normalize();
                L.Normalize();
                R.Normalize();
            }

            wiimote.Update(data);
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);

            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    //LJoy.Calibrate(Calibrations.Defaults.ClassicControllerDefault.LJoy);
                    //RJoy.Calibrate(Calibrations.Defaults.ClassicControllerDefault.RJoy);
                    //L.Calibrate(Calibrations.Defaults.ClassicControllerDefault.L);
                    //R.Calibrate(Calibrations.Defaults.ClassicControllerDefault.R);
                    SetCalibration(Calibrations.Defaults.ClassicControllerDefault);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    SetCalibration(Calibrations.Moderate.ClassicControllerModest);
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    SetCalibration(Calibrations.Extras.ClassicControllerExtra);
                    break;

                case Calibrations.CalibrationPreset.Minimum:
                    SetCalibration(Calibrations.Minimum.ClassicControllerMinimal);
                    break;

                case Calibrations.CalibrationPreset.None:
                    SetCalibration(Calibrations.None.ClassicControllerRaw);
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

            if (from.GetType() == typeof(ClassicController))
            {
                LJoy.Calibrate(((ClassicController)from).LJoy);
                RJoy.Calibrate(((ClassicController)from).RJoy);
                L.Calibrate(((ClassicController)from).L);
                R.Calibrate(((ClassicController)from).R);
            }
            else if (from.GetType() == typeof(Wiimote))
            {
                wiimote.SetCalibration(from);
            }
        }

        public void SetCalibration(string calibrationString)
        {
            if (calibrationString.Count(c => c == '0') > 5)
            {
                // don't set empty calibrations
                return;
            }

            string[] components = calibrationString.Split(new char[] { ':' });

            foreach (string component in components)
            {
                if (component.StartsWith("joyL"))
                {
                    string[] joyLConfig = component.Split(new char[] { '|' });

                    for (int jL = 1; jL < joyLConfig.Length; jL++)
                    {
                        int value = 0;
                        if (int.TryParse(joyLConfig[jL], out value))
                        {
                            switch (jL)
                            {
                                case 1: LJoy.centerX = value; break;
                                case 2: LJoy.minX = value; break;
                                case 3: LJoy.maxX = value; break;
                                case 4: LJoy.deadX = value; break;
                                case 5: LJoy.centerY = value; break;
                                case 6: LJoy.minY = value; break;
                                case 7: LJoy.maxY = value; break;
                                case 8: LJoy.deadY = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("joyR"))
                {
                    string[] joyRConfig = component.Split(new char[] { '|' });

                    for (int jR = 1; jR < joyRConfig.Length; jR++)
                    {
                        int value = 0;
                        if (int.TryParse(joyRConfig[jR], out value))
                        {
                            switch (jR)
                            {
                                case 1: RJoy.centerX = value; break;
                                case 2: RJoy.minX = value; break;
                                case 3: RJoy.maxX = value; break;
                                case 4: RJoy.deadX = value; break;
                                case 5: RJoy.centerY = value; break;
                                case 6: RJoy.minY = value; break;
                                case 7: RJoy.maxY = value; break;
                                case 8: RJoy.deadY = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("tl"))
                {
                    string[] triggerLConfig = component.Split(new char[] { '|' });

                    for (int tl = 1; tl < triggerLConfig.Length; tl++)
                    {
                        int value = 0;
                        if (int.TryParse(triggerLConfig[tl], out value))
                        {
                            switch (tl)
                            {
                                case 1: L.min = value; break;
                                case 2: L.max = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("tr"))
                {
                    string[] triggerRConfig = component.Split(new char[] { '|' });

                    for (int tr = 1; tr < triggerRConfig.Length; tr++)
                    {
                        int value = 0;
                        if (int.TryParse(triggerRConfig[tr], out value))
                        {
                            switch (tr)
                            {
                                case 1: R.min = value; break;
                                case 2: R.max = value; break;
                                default: break;
                            }
                        }
                    }
                }
            }
        }

        public string GetCalibrationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-cla");
            sb.Append(":joyL");
                sb.Append("|"); sb.Append(LJoy.centerX);
                sb.Append("|"); sb.Append(LJoy.minX);
                sb.Append("|"); sb.Append(LJoy.maxX);
                sb.Append("|"); sb.Append(LJoy.deadX);
                sb.Append("|"); sb.Append(LJoy.centerY);
                sb.Append("|"); sb.Append(LJoy.minY);
                sb.Append("|"); sb.Append(LJoy.maxY);
                sb.Append("|"); sb.Append(LJoy.deadY);
            sb.Append(":joyR");
                sb.Append("|"); sb.Append(RJoy.centerX);
                sb.Append("|"); sb.Append(RJoy.minX);
                sb.Append("|"); sb.Append(RJoy.maxX);
                sb.Append("|"); sb.Append(RJoy.deadX);
                sb.Append("|"); sb.Append(RJoy.centerY);
                sb.Append("|"); sb.Append(RJoy.minY);
                sb.Append("|"); sb.Append(RJoy.maxY);
                sb.Append("|"); sb.Append(RJoy.deadY);
            sb.Append(":tl");
                sb.Append("|"); sb.Append(L.min);
                sb.Append("|"); sb.Append(L.max);
            sb.Append(":tr");
                sb.Append("|"); sb.Append(R.min);
                sb.Append("|"); sb.Append(R.max);

            return sb.ToString();
        }

        public bool CalibrationEmpty
        {
            get
            {
                if (LJoy.maxX == 0 && LJoy.maxY == 0 && RJoy.maxX == 0 && RJoy.maxY == 0)
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
            foreach (var input in wiimote)
            {
                yield return input;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct ClassicControllerPro : INintrollerState
    {
        public Wiimote wiimote;
        public Joystick LJoy, RJoy;
        public bool A, B, X, Y;
        public bool Up, Down, Left, Right;
        public bool L, R, ZL, ZR;
        public bool Plus, Minus, Home;

        public ClassicControllerPro(Wiimote wm)
        {
            this = new ClassicControllerPro();
            wiimote = wm;
        }

        public bool Start
        {
            get { return Plus; }
            set { Plus = value; }
        }

        public bool Select
        {
            get { return Minus; }
            set { Minus = value; }
        }

        public void Update(byte[] data)
        {
            int offset = 0;
            switch ((InputReport)data[0])
            {
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
                case InputReport.ExtOnly:
                    offset = 1;
                    break;
                default:
                    return;
            }

            if (offset > 0)
            {
                // Buttons
                A     = (data[offset + 5] & 0x10) == 0;
                B     = (data[offset + 5] & 0x40) == 0;
                X     = (data[offset + 5] & 0x08) == 0;
                Y     = (data[offset + 5] & 0x20) == 0;
                L     = (data[offset + 4] & 0x20) == 0;
                R     = (data[offset + 4] & 0x02) == 0;
                ZL    = (data[offset + 5] & 0x80) == 0;
                ZR    = (data[offset + 5] & 0x04) == 0;
                Plus  = (data[offset + 4] & 0x04) == 0;
                Minus = (data[offset + 4] & 0x10) == 0;
                Home  = (data[offset + 4] & 0x08) == 0;

                // Dpad
                Up    = (data[offset + 5] & 0x01) == 0;
                Down  = (data[offset + 4] & 0x40) == 0;
                Left  = (data[offset + 5] & 0x02) == 0;
                Right = (data[offset + 4] & 0x80) == 0;

                // Joysticks
                LJoy.rawX = (byte)(data[offset] & 0x3F);
                LJoy.rawY = (byte)(data[offset + 1] & 0x03F);
                RJoy.rawX = (byte)(data[offset + 2] >> 7 | (data[offset + 1] & 0xC0) >> 5 | (data[offset] & 0xC0) >> 3);
                RJoy.rawY = (byte)(data[offset + 2] & 0x1F);

                // Normalize
                LJoy.Normalize();
                RJoy.Normalize();
            }

            wiimote.Update(data);
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);

            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    //LJoy.Calibrate(Calibrations.Defaults.ClassicControllerProDefault.LJoy);
                    //RJoy.Calibrate(Calibrations.Defaults.ClassicControllerProDefault.RJoy);
                    SetCalibration(Calibrations.Defaults.ClassicControllerProDefault);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    SetCalibration(Calibrations.Moderate.ClassicControllerProModest);
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    SetCalibration(Calibrations.Extras.ClassicControllerProExtra);
                    break;

                case Calibrations.CalibrationPreset.Minimum:
                    SetCalibration(Calibrations.Minimum.ClassicControllerProMinimal);
                    break;

                case Calibrations.CalibrationPreset.None:
                    SetCalibration(Calibrations.None.ClassicControllerProRaw);
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

            if (from.GetType() == typeof(ClassicControllerPro))
            {
                LJoy.Calibrate(((ClassicControllerPro)from).LJoy);
                RJoy.Calibrate(((ClassicControllerPro)from).RJoy);
            }
            else if (from.GetType() == typeof(Wiimote))
            {
                wiimote.SetCalibration(from);
            }
        }

        public void SetCalibration(string calibrationString)
        {
            if (calibrationString.Count(c => c == '0') > 5)
            {
                // don't set empty calibrations
                return;
            }

            string[] components = calibrationString.Split(new char[] { ':' });

            foreach (string component in components)
            {
                if (component.StartsWith("joyL"))
                {
                    string[] joyLConfig = component.Split(new char[] { '|' });

                    for (int jL = 1; jL < joyLConfig.Length; jL++)
                    {
                        int value = 0;
                        if (int.TryParse(joyLConfig[jL], out value))
                        {
                            switch (jL)
                            {
                                case 1: LJoy.centerX = value; break;
                                case 2: LJoy.minX = value; break;
                                case 3: LJoy.maxX = value; break;
                                case 4: LJoy.deadX = value; break;
                                case 5: LJoy.centerY = value; break;
                                case 6: LJoy.minY = value; break;
                                case 7: LJoy.maxY = value; break;
                                case 8: LJoy.deadY = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("joyR"))
                {
                    string[] joyRConfig = component.Split(new char[] { '|' });

                    for (int jR = 1; jR < joyRConfig.Length; jR++)
                    {
                        int value = 0;
                        if (int.TryParse(joyRConfig[jR], out value))
                        {
                            switch (jR)
                            {
                                case 1: RJoy.centerX = value; break;
                                case 2: RJoy.minX = value; break;
                                case 3: RJoy.maxX = value; break;
                                case 4: RJoy.deadX = value; break;
                                case 5: RJoy.centerY = value; break;
                                case 6: RJoy.minY = value; break;
                                case 7: RJoy.maxY = value; break;
                                case 8: RJoy.deadY = value; break;
                                default: break;
                            }
                        }
                    }
                }
            }
        }

        public string GetCalibrationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-ccp");
                sb.Append(":joyL");
                    sb.Append("|"); sb.Append(LJoy.centerX);
                    sb.Append("|"); sb.Append(LJoy.minX);
                    sb.Append("|"); sb.Append(LJoy.maxX);
                    sb.Append("|"); sb.Append(LJoy.deadX);
                    sb.Append("|"); sb.Append(LJoy.centerY);
                    sb.Append("|"); sb.Append(LJoy.minY);
                    sb.Append("|"); sb.Append(LJoy.maxY);
                    sb.Append("|"); sb.Append(LJoy.deadY);
                sb.Append(":joyR");
                    sb.Append("|"); sb.Append(RJoy.centerX);
                    sb.Append("|"); sb.Append(RJoy.minX);
                    sb.Append("|"); sb.Append(RJoy.maxX);
                    sb.Append("|"); sb.Append(RJoy.deadX);
                    sb.Append("|"); sb.Append(RJoy.centerY);
                    sb.Append("|"); sb.Append(RJoy.minY);
                    sb.Append("|"); sb.Append(RJoy.maxY);
                    sb.Append("|"); sb.Append(RJoy.deadY);

            return sb.ToString();
        }

        public bool CalibrationEmpty
        {
            get
            {
                if (LJoy.maxX == 0 && LJoy.maxY == 0 && RJoy.maxX == 0 && RJoy.maxY == 0)
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
            foreach (var input in wiimote)
            {
                yield return input;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct ProController : INintrollerState
    {
        public Joystick LJoy, RJoy;
        public bool A, B, X, Y;
        public bool Up, Down, Left, Right;
        public bool L, R, ZL, ZR;
        public bool Plus, Minus, Home;
        public bool LStick, RStick;
        public bool charging, usbConnected;

        public bool Start
        {
            get { return Plus; }
            set { Plus = value; }
        }

        public bool Select
        {
            get { return Minus; }
            set { Minus = value; }
        }

        public void Update(byte[] data)
        {
            int offset = 0;

            switch ((InputReport)data[0])
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
                case InputReport.Status:
                    Plus  = (data[1] & 0x04) == 0;
                    Home  = (data[1] & 0x08) == 0;
                    Minus = (data[1] & 0x10) == 0;
                    Down  = (data[1] & 0x40) == 0;
                    Right = (data[1] & 0x80) == 0;
                    Up    = (data[2] & 0x01) == 0;
                    Left  = (data[2] & 0x02) == 0;
                    A     = (data[2] & 0x10) == 0;
                    B     = (data[2] & 0x40) == 0;
                    return;
                default:
                    return;
            }

            // Buttons
            A      = (data[offset +  9] & 0x10) == 0;
            B      = (data[offset +  9] & 0x40) == 0;
            X      = (data[offset +  9] & 0x08) == 0;
            Y      = (data[offset +  9] & 0x20) == 0;
            L      = (data[offset +  8] & 0x20) == 0;
            R      = (data[offset +  8] & 0x02) == 0;
            ZL     = (data[offset +  9] & 0x80) == 0;
            ZR     = (data[offset +  9] & 0x04) == 0;
            Plus   = (data[offset +  8] & 0x04) == 0;
            Minus  = (data[offset +  8] & 0x10) == 0;
            Home   = (data[offset +  8] & 0x08) == 0;
            LStick = (data[offset + 10] & 0x02) == 0;
            RStick = (data[offset + 10] & 0x01) == 0;

            // DPad
            Up    = (data[offset + 9] & 0x01) == 0;
            Down  = (data[offset + 8] & 0x40) == 0;
            Left  = (data[offset + 9] & 0x02) == 0;
            Right = (data[offset + 8] & 0x80) == 0;

            // Joysticks
            LJoy.rawX = BitConverter.ToInt16(data, offset);
            LJoy.rawY = BitConverter.ToInt16(data, offset + 4);
            RJoy.rawX = BitConverter.ToInt16(data, offset + 2);
            RJoy.rawY = BitConverter.ToInt16(data, offset + 6);

            // Other
            charging     = (data[offset + 10] & 0x04) == 0;
            usbConnected = (data[offset + 10] & 0x08) == 0;

            // Normalize
            LJoy.Normalize();
            RJoy.Normalize();
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    //LJoy.Calibrate(Calibrations.Defaults.ProControllerDefault.LJoy);
                    //RJoy.Calibrate(Calibrations.Defaults.ProControllerDefault.RJoy);
                    SetCalibration(Calibrations.Defaults.ProControllerDefault);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    SetCalibration(Calibrations.Moderate.ProControllerModest);
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    SetCalibration(Calibrations.Extras.ProControllerExtra);
                    break;

                case Calibrations.CalibrationPreset.Minimum:
                    SetCalibration(Calibrations.Minimum.ProControllerMinimal);
                    break;

                case Calibrations.CalibrationPreset.None:
                    SetCalibration(Calibrations.None.ProControllerRaw);
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

            if (from.GetType() == typeof(ProController))
            {
                LJoy.Calibrate(((ProController)from).LJoy);
                RJoy.Calibrate(((ProController)from).RJoy);
            }
        }

        public void SetCalibration(string calibrationString)
        {
            if (calibrationString.Count(c => c == '0') > 5)
            {
                // don't set empty calibrations
                return;
            }

            string[] components = calibrationString.Split(new char[] { ':' });

            foreach (string component in components)
            {
                if (component.StartsWith("joyL"))
                {
                    string[] joyLConfig = component.Split(new char[] { '|' });

                    for (int jL = 1; jL < joyLConfig.Length; jL++)
                    {
                        int value = 0;
                        if (int.TryParse(joyLConfig[jL], out value))
                        {
                            switch (jL)
                            {
                                case 1: LJoy.centerX = value; break;
                                case 2: LJoy.minX    = value; break;
                                case 3: LJoy.maxX    = value; break;
                                case 4: LJoy.deadX   = value; break;
                                case 5: LJoy.centerY = value; break;
                                case 6: LJoy.minY    = value; break;
                                case 7: LJoy.maxY    = value; break;
                                case 8: LJoy.deadY   = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("joyR"))
                {
                    string[] joyRConfig = component.Split(new char[] { '|' });

                    for (int jR = 1; jR < joyRConfig.Length; jR++)
                    {
                        int value = 0;
                        if (int.TryParse(joyRConfig[jR], out value))
                        {
                            switch (jR)
                            {
                                case 1: RJoy.centerX = value; break;
                                case 2: RJoy.minX    = value; break;
                                case 3: RJoy.maxX    = value; break;
                                case 4: RJoy.deadX   = value; break;
                                case 5: RJoy.centerY = value; break;
                                case 6: RJoy.minY    = value; break;
                                case 7: RJoy.maxY    = value; break;
                                case 8: RJoy.deadY   = value; break;
                                default: break;
                            }
                        }
                    }
                }
            }
        }

        public string GetCalibrationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-pro");
            sb.Append(":joyL");
            sb.Append("|"); sb.Append(LJoy.centerX);
            sb.Append("|"); sb.Append(LJoy.minX);
            sb.Append("|"); sb.Append(LJoy.maxX);
            sb.Append("|"); sb.Append(LJoy.deadX);
            sb.Append("|"); sb.Append(LJoy.centerY);
            sb.Append("|"); sb.Append(LJoy.minY);
            sb.Append("|"); sb.Append(LJoy.maxY);
            sb.Append("|"); sb.Append(LJoy.deadY);
            sb.Append(":joyR");
            sb.Append("|"); sb.Append(RJoy.centerX);
            sb.Append("|"); sb.Append(RJoy.minX);
            sb.Append("|"); sb.Append(RJoy.maxX);
            sb.Append("|"); sb.Append(RJoy.deadX);
            sb.Append("|"); sb.Append(RJoy.centerY);
            sb.Append("|"); sb.Append(RJoy.minY);
            sb.Append("|"); sb.Append(RJoy.maxY);
            sb.Append("|"); sb.Append(RJoy.deadY);

            return sb.ToString();
        }

        public bool CalibrationEmpty
        {
            get
            {
                if (LJoy.maxX == 0 && LJoy.maxY == 0 && RJoy.maxX == 0 && RJoy.maxY == 0)
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
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.A, A ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.B, B ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.X, X ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.Y, Y ? 1.0f : 0.0f);

            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.L,  L  ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.R,  R  ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.ZL, ZL ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.ZR, ZR ? 1.0f : 0.0f);

            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.UP,    Up    ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.DOWN,  Down  ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.LEFT,  Left  ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.RIGHT, Right ? 1.0f : 0.0f);

            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.START,  Start  ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.SELECT, Select ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.HOME,   Home   ? 1.0f : 0.0f);

            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.LS, LStick ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.RS, RStick ? 1.0f : 0.0f);

            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.LX, LJoy.X);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.LY, LJoy.Y);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.RX, RJoy.X);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.RY, RJoy.X);

            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.LUP,    LJoy.Y > 0f ? LJoy.Y : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.LDOWN,  LJoy.Y > 0f ? 0.0f : -LJoy.Y); // These are inverted
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.LLEFT,  LJoy.X > 0f ? 0.0f : -LJoy.X); // because they
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.LRIGHT, LJoy.X > 0f ? LJoy.X : 0.0f);

            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.RUP,    RJoy.Y > 0f ? RJoy.Y : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.RDOWN,  RJoy.Y > 0f ? 0.0f : -RJoy.Y); // represents how far the
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.RLEFT,  RJoy.X > 0f ? 0.0f : -RJoy.X); // input is left or down
            yield return new KeyValuePair<string, float>(INPUT_NAMES.PRO_CONTROLLER.RRIGHT, RJoy.X > 0f ? RJoy.X : 0.0f);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct BalanceBoard : INintrollerState
    {

        public void Update(byte[] data)
        {
            throw new NotImplementedException();
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        // TODO: Calibration - Balance Board Calibration
        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    break;

                case Calibrations.CalibrationPreset.Minimum:
                    break;

                case Calibrations.CalibrationPreset.None:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            if (from.GetType() == typeof(BalanceBoard))
            {
                
            }
        }

        public void SetCalibration(string calibrationString)
        {

        }

        public string GetCalibrationString()
        {
            return "";
        }


        public bool CalibrationEmpty
        {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            yield return new KeyValuePair<string, float>("bb", 0);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct WiimotePlus : INintrollerState
    {
        Wiimote wiimote;
        //gyro

        public void Update(byte[] data)
        {
            throw new NotImplementedException();
        }

        public float GetValue(string input)
        {
            throw new NotImplementedException();
        }

        // TODO: Calibration - Balance Board Calibration
        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    break;

                case Calibrations.CalibrationPreset.Minimum:
                    break;

                case Calibrations.CalibrationPreset.None:
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            if (from.GetType() == typeof(WiimotePlus))
            {

            }
        }

        public void SetCalibration(string calibrationString)
        {

        }

        public string GetCalibrationString()
        {
            return "";
        }

        public bool CalibrationEmpty
        {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            foreach (var input in wiimote)
            {
                yield return input;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
