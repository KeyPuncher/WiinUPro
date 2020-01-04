using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NintrollerLib
{
    public struct Guitar : INintrollerState, IWiimoteExtension
    {
        public Wiimote wiimote { get; set; }
        public bool Green, Red, Yellow, Blue, Orange;
        public bool StrumUp, StrumDown;
        public bool Minus, Plus;
        public Joystick joystick;
        public Trigger whammyBar;
        public byte touchBar;
        public bool T1, T2, T3, T4, T5; // Touch Frets

        public void Update(byte[] data)
        {
            // http://wiibrew.org/wiki/Wiimote/Extension_Controllers/Guitar_Hero_(Wii)_Guitars
            // Byte 0: bits 0-5 = joy X
            // Byte 1: bits 0-5 = joy Y
            // Byte 2: bits 0-4 = touch
            // Byte 3: bits 0-4 = whammy
            // Byte 4: bit 2 = Plus, bit 4 = Minus, bit 6 = Strum Down
            // Byte 5: bit 0 = Strum Up, bit 3 = yellow
            //         bit 4 = green, bit 5 = blue
            //         bit 6 = red, bit 7 = orange

            int offset = Utils.GetExtensionOffset((InputReport)data[0]);

            if (offset > 0)
            {
                joystick.rawX =    data[offset    ] & 0x3F;
                joystick.rawY =    data[offset + 1] & 0x3F;
                touchBar  = (byte)(data[offset + 2] & 0x1F);
                whammyBar.rawValue = (byte)(data[offset + 3] & 0x1F);

                Plus      = (data[offset + 4] & 0x04) == 0;
                Minus     = (data[offset + 4] & 0x10) == 0;
                StrumDown = (data[offset + 4] & 0x40) == 0;
                StrumUp   = (data[offset + 5] & 0x01) == 0;
                Yellow    = (data[offset + 5] & 0x08) == 0;
                Green     = (data[offset + 5] & 0x10) == 0;
                Blue      = (data[offset + 5] & 0x20) == 0;
                Red       = (data[offset + 5] & 0x40) == 0;
                Orange    = (data[offset + 5] & 0x80) == 0;

                switch (touchBar)
                {
                    default:
                    case 0x0F:
                        T1 = T2 = T3 = T4 = T5 = false;
                        break;

                    case 0x04:
                        T1 = true;
                        T2 = T3 = T4 = T5 = false;
                        break;

                    case 0x07:
                        T1 = T2 = true;
                        T3 = T4 = T5 = false;
                        break;

                    case 0x0A:
                        T2 = true;
                        T1 = T3 = T4 = T5 = false;
                        break;

                    case 0x0C:
                    case 0x0D:
                        T2 = T3 = true;
                        T1 = T4 = T5 = false;
                        break;

                    case 0x12:
                    case 0x13:
                        T3 = true;
                        T1 = T2 = T4 = T5 = false;
                        break;

                    case 0x14:
                    case 0x15:
                        T3 = T4 = true;
                        T1 = T2 = T5 = false;
                        break;

                    case 0x17:
                    case 0x18:
                        T4 = true;
                        T1 = T2 = T3 = T5 = false;
                        break;

                    case 0x1A:
                        T4 = T5 = true;
                        T1 = T2 = T3 = false;
                        break;

                    case 0x1F:
                        T5 = true;
                        T1 = T2 = T3 = T4 = false;
                        break;
                }

                joystick.Normalize();
                whammyBar.Normalize();
                whammyBar.full = whammyBar.rawValue >= whammyBar.max;
            }

            wiimote = new Wiimote(data, wiimote);
        }

        public float GetValue(string input)
        {
            switch (input)
            {
                case INPUT_NAMES.GUITAR.GREEN: return Green ? 1 : 0;
                case INPUT_NAMES.GUITAR.RED: return Red ? 1 : 0;
                case INPUT_NAMES.GUITAR.YELLOW: return Yellow ? 1 : 0;
                case INPUT_NAMES.GUITAR.BLUE: return Blue ? 1 : 0;
                case INPUT_NAMES.GUITAR.ORANGE: return Orange ? 1 : 0;
                case INPUT_NAMES.GUITAR.MINUS: return Minus ? 1 : 0;
                case INPUT_NAMES.GUITAR.PLUS: return Plus ? 1 : 0;
                case INPUT_NAMES.GUITAR.TOUCH_1: return T1 ? 1 : 0;
                case INPUT_NAMES.GUITAR.TOUCH_2: return T2 ? 1 : 0;
                case INPUT_NAMES.GUITAR.TOUCH_3: return T3 ? 1 : 0;
                case INPUT_NAMES.GUITAR.TOUCH_4: return T4 ? 1 : 0;
                case INPUT_NAMES.GUITAR.TOUCH_5: return T5 ? 1 : 0;
                case INPUT_NAMES.GUITAR.STRUM_UP: return StrumUp ? 1 : 0;
                case INPUT_NAMES.GUITAR.STRUM_DOWN: return StrumDown ? 1 : 0;
                case INPUT_NAMES.GUITAR.JOY_X: return joystick.X;
                case INPUT_NAMES.GUITAR.JOY_Y: return joystick.Y;
                case INPUT_NAMES.GUITAR.UP: return joystick.Y > 0 ? 1 : 0;
                case INPUT_NAMES.GUITAR.DOWN: return joystick.Y < 0 ? 1 : 0;
                case INPUT_NAMES.GUITAR.LEFT: return joystick.X < 0 ? 1 : 0;
                case INPUT_NAMES.GUITAR.RIGHT: return joystick.X > 0 ? 1 : 0;
                case INPUT_NAMES.GUITAR.WHAMMY: return whammyBar.value > 0 ? 1 : 0;
                case INPUT_NAMES.GUITAR.WHAMMY_BAR: return whammyBar.value;
                case INPUT_NAMES.GUITAR.WHAMMY_FULL: return whammyBar.value >= whammyBar.max ? 1 : 0;
            }

            return wiimote.GetValue(input);
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);

            switch (preset)
            {
                case Calibrations.CalibrationPreset.Default:
                    SetCalibration(Calibrations.Defaults.GuitarDefault);
                    break;
                    
                case Calibrations.CalibrationPreset.Minimum:
                    SetCalibration(Calibrations.Minimum.GuitarMinimal);
                    break;

                case Calibrations.CalibrationPreset.Modest:
                    SetCalibration(Calibrations.Moderate.GuitarModest);
                    break;

                case Calibrations.CalibrationPreset.Extra:
                    SetCalibration(Calibrations.Extras.GuitarExtra);
                    break;

                case Calibrations.CalibrationPreset.None:
                    SetCalibration(Calibrations.None.GuitarRaw);
                    break;
            }
        }

        public void SetCalibration(INintrollerState from)
        {
            if (from.CalibrationEmpty)
            {
                return;
            }

            if (from.GetType() == typeof(Guitar))
            {
                joystick.Calibrate(((Guitar)from).joystick);
                whammyBar.Calibrate(((Guitar)from).whammyBar);
                wiimote.SetCalibration(((Guitar)from).wiimote);
            }
            else if (from.GetType() == typeof(Wiimote))
            {
                wiimote.SetCalibration(from);
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
                                case 2: joystick.minX = value; break;
                                case 3: joystick.maxX = value; break;
                                case 4: joystick.deadX = value; break;
                                case 5: joystick.centerY = value; break;
                                case 6: joystick.minY = value; break;
                                case 7: joystick.maxY = value; break;
                                case 8: joystick.deadY = value; break;
                                default: break;
                            }
                        }
                    }
                }
                else if (component.StartsWith("t"))
                {
                    string[] triggerConfig = component.Split(new char[] { '|' });

                    for (int t = 1; t < triggerConfig.Length; t++)
                    {
                        int value = 0;
                        if (int.TryParse(triggerConfig[t], out value))
                        {
                            switch (t)
                            {
                                case 1: whammyBar.min = value; break;
                                case 2: whammyBar.max = value; break;
                            }
                        }
                    }
                }
            }
        }

        public string GetCalibrationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("-gut");
                sb.Append(":joy");
                    sb.Append("|"); sb.Append(joystick.centerX);
                    sb.Append("|"); sb.Append(joystick.minX);
                    sb.Append("|"); sb.Append(joystick.maxX);
                    sb.Append("|"); sb.Append(joystick.deadX);

                    sb.Append("|"); sb.Append(joystick.centerY);
                    sb.Append("|"); sb.Append(joystick.minY);
                    sb.Append("|"); sb.Append(joystick.maxY);
                    sb.Append("|"); sb.Append(joystick.deadY);

                sb.Append(":t");
                    sb.Append("|"); sb.Append(whammyBar.min);
                    sb.Append("|"); sb.Append(whammyBar.max);

            return sb.ToString();
        }

        public bool CalibrationEmpty
        {
            get
            {
                if (joystick.maxX == 0 && joystick.maxY == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            foreach (var input in wiimote)
            {
                yield return input;
            }

            // Frets & Buttons
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.GREEN, Green ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.RED, Red ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.YELLOW, Yellow ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.BLUE, Blue ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.ORANGE, Orange ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.STRUM_UP, StrumUp ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.STRUM_DOWN, StrumDown ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.PLUS, Plus ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.MINUS, Minus ? 1.0f : 0.0f);

            // Joystick
            joystick.Normalize();
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.JOY_X, joystick.X);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.JOY_Y, joystick.Y);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.UP, joystick.Y > 0 ? joystick.Y : 0);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.DOWN, joystick.Y > 0 ? 0 : -joystick.Y);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.LEFT, joystick.X > 0 ? 0 : -joystick.X);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.RIGHT, joystick.X > 0 ? joystick.X : 0);

            // Whammy
            whammyBar.Normalize();
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.WHAMMY, whammyBar.value > 0 ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.WHAMMY_FULL, whammyBar.rawValue >= whammyBar.max ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.WHAMMY_BAR, whammyBar.value);

            // Touch Frets
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.TOUCH_1, T1 ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.TOUCH_2, T2 ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.TOUCH_3, T3 ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.TOUCH_4, T4 ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.GUITAR.TOUCH_5, T5 ? 1.0f : 0.0f);

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
