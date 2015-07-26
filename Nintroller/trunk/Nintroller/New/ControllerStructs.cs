using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib.New
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
            if (from.GetType() == typeof(Wiimote))
            {
                accelerometer.Calibrate(((Wiimote)from).accelerometer);
            }
            else if (from.GetType() == typeof(Nunchuk))
            {
                accelerometer.Calibrate(((Nunchuk)from).wiimote.accelerometer);
            }
            else if (from.GetType() == typeof(ClassicController))
            {
                accelerometer.Calibrate(((ClassicController)from).wiimote.accelerometer);
            }
            else if (from.GetType() == typeof(ClassicControllerPro))
            {
                accelerometer.Calibrate(((ClassicControllerPro)from).wiimote.accelerometer);
            }
        }

        public void SetCalibration(string calibrationString)
        {
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

            // there is no IR settings yet

            return sb.ToString();
        }
    }

    public struct Nunchuk : INintrollerState
    {
        public Wiimote wiimote;
        public Accelerometer accelerometer;
        public Joystick joystick;
        public bool C, Z;

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
                default:
                    return;
            }

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
            if (from.GetType() == typeof(Nunchuk))
            {
                accelerometer.Calibrate(((Nunchuk)from).accelerometer);
                joystick.Calibrate(((Nunchuk)from).joystick);
            }
        }

        public void SetCalibration(string calibrationString)
        {
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
            LJoy.rawX = (byte)(data[offset    ] & 0x3F);
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
            if (from.GetType() == typeof(ClassicController))
            {
                LJoy.Calibrate(((ClassicController)from).LJoy);
                RJoy.Calibrate(((ClassicController)from).RJoy);
                L.Calibrate(((ClassicController)from).L);
                R.Calibrate(((ClassicController)from).R);
            }
        }

        public void SetCalibration(string calibrationString)
        {
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
    }

    public struct ClassicControllerPro : INintrollerState
    {
        public Wiimote wiimote;
        public Joystick LJoy, RJoy;
        public bool A, B, X, Y;
        public bool Up, Down, Left, Right;
        public bool L, R, ZL, ZR;
        public bool Plus, Minus, Home;

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
            if (from.GetType() == typeof(ClassicControllerPro))
            {
                LJoy.Calibrate(((ClassicControllerPro)from).LJoy);
                RJoy.Calibrate(((ClassicControllerPro)from).RJoy);
            }
        }

        public void SetCalibration(string calibrationString)
        {
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

            return sb.ToString();
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
                default:
                    break;
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
            if (from.GetType() == typeof(ProController))
            {
                LJoy.Calibrate(((ProController)from).LJoy);
                RJoy.Calibrate(((ProController)from).RJoy);
            }
        }

        public void SetCalibration(string calibrationString)
        {
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

            return sb.ToString();
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
    }
}
