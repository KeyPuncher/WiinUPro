using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NintrollerLib
{
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
            int offset = Utils.GetExtensionOffset((InputReport)data[0]);

            if (offset > 0)
            {
                // Buttons
                A = (data[offset + 9] & 0x10) == 0;
                B = (data[offset + 9] & 0x40) == 0;
                X = (data[offset + 9] & 0x08) == 0;
                Y = (data[offset + 9] & 0x20) == 0;
                L = (data[offset + 8] & 0x20) == 0;
                R = (data[offset + 8] & 0x02) == 0;
                ZL = (data[offset + 9] & 0x80) == 0;
                ZR = (data[offset + 9] & 0x04) == 0;
                Plus = (data[offset + 8] & 0x04) == 0;
                Minus = (data[offset + 8] & 0x10) == 0;
                Home = (data[offset + 8] & 0x08) == 0;
                LStick = (data[offset + 10] & 0x02) == 0;
                RStick = (data[offset + 10] & 0x01) == 0;

                // DPad
                Up = (data[offset + 9] & 0x01) == 0;
                Down = (data[offset + 8] & 0x40) == 0;
                Left = (data[offset + 9] & 0x02) == 0;
                Right = (data[offset + 8] & 0x80) == 0;

                // Joysticks
                LJoy.rawX = BitConverter.ToInt16(data, offset);
                LJoy.rawY = BitConverter.ToInt16(data, offset + 4);
                RJoy.rawX = BitConverter.ToInt16(data, offset + 2);
                RJoy.rawY = BitConverter.ToInt16(data, offset + 6);

                // Other
                charging = (data[offset + 10] & 0x04) == 0;
                usbConnected = (data[offset + 10] & 0x08) == 0;

                // Normalize
                LJoy.Normalize();
                RJoy.Normalize();
            }
            else if (Utils.ReportContainsCoreButtons((InputReport)data[0]))
            {
                Plus = (data[1] & 0x04) == 0;
                Home = (data[1] & 0x08) == 0;
                Minus = (data[1] & 0x10) == 0;
                Down = (data[1] & 0x40) == 0;
                Right = (data[1] & 0x80) == 0;
                Up = (data[2] & 0x01) == 0;
                Left = (data[2] & 0x02) == 0;
                A = (data[2] & 0x10) == 0;
                B = (data[2] & 0x40) == 0;
            }
        }

        public float GetValue(string input)
        {
            switch (input)
            {
                case INPUT_NAMES.PRO_CONTROLLER.A: return A ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.B: return B ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.X: return X ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.Y: return Y ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.UP: return Up ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.DOWN: return Down ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.LEFT: return Left ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.RIGHT: return Right ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.L: return L ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.R: return R ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.ZL: return ZL ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.ZR: return ZR ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.LX: return LJoy.X;
                case INPUT_NAMES.PRO_CONTROLLER.LY: return LJoy.Y;
                case INPUT_NAMES.PRO_CONTROLLER.RX: return RJoy.X;
                case INPUT_NAMES.PRO_CONTROLLER.RY: return RJoy.Y;
                case INPUT_NAMES.PRO_CONTROLLER.LUP: return LJoy.Y > 0 ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.LDOWN: return LJoy.Y < 0 ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.LLEFT: return LJoy.X < 0 ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.LRIGHT: return LJoy.X > 0 ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.RUP: return RJoy.Y > 0 ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.RDOWN: return RJoy.Y < 0 ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.RLEFT: return RJoy.X < 0 ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.RRIGHT: return RJoy.X > 0 ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.LS: return LStick ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.RS: return RStick ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.SELECT: return Select ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.START: return Start ? 1 : 0;
                case INPUT_NAMES.PRO_CONTROLLER.HOME: return Home ? 1 : 0;
            }

            return 0;
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

            LJoy.Normalize();
            RJoy.Normalize();
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
}
