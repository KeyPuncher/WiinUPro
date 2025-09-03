using System.Collections;
using System.Collections.Generic;

namespace NintrollerLib
{
    public struct TaikoDrum : INintrollerState, IWiimoteExtension
    {
        public Wiimote wiimote { get; set; }
        public bool centerLeft, centerRight, rimLeft, rimRight;

        public void Update(byte[] data)
        {
            int offset = Utils.GetExtensionOffset((InputReport)data[0]);

            if (offset > 0)
            {
                // other bits must be set like so:
                if ((data[offset] & 0b_1000_0111) == 0b_1000_0111)
                {
                    rimRight = (data[offset] & 0x08) == 0;
                    centerRight = (data[offset] & 0x10) == 0;
                    rimLeft = (data[offset] & 0x20) == 0;
                    centerLeft = (data[offset] & 0x40) == 0;
                }
            }

            wiimote = new Wiimote(data, wiimote);
        }

        public float GetValue(string input)
        {
            switch (input)
            {
                case INPUT_NAMES.TAIKO_DRUM.CENTER_LEFT: return centerLeft ? 1 : 0;
                case INPUT_NAMES.TAIKO_DRUM.CENTER_RIGHT: return centerRight ? 1 : 0;
                case INPUT_NAMES.TAIKO_DRUM.RIM_LEFT: return rimLeft ? 1 : 0;
                case INPUT_NAMES.TAIKO_DRUM.RIM_RIGHT: return rimRight ? 1 : 0;
            }

            return wiimote.GetValue(input);
        }

        public void SetCalibration(Calibrations.CalibrationPreset preset)
        {
            wiimote.SetCalibration(preset);
        }

        public void SetCalibration(INintrollerState from)
        {
            // no calibration needed
        }

        public void SetCalibration(string calibrationString)
        {
            // no calibration needed
        }

        public string GetCalibrationString()
        {
            // none needed
            return string.Empty;
        }

        public bool CalibrationEmpty
        {
            get { return wiimote.CalibrationEmpty; }
        }

        public IEnumerator<KeyValuePair<string, float>> GetEnumerator()
        {
            foreach (var input in wiimote)
            {
                yield return input;
            }

            yield return new KeyValuePair<string, float>(INPUT_NAMES.TAIKO_DRUM.CENTER_LEFT, centerLeft ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.TAIKO_DRUM.CENTER_RIGHT, centerRight ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.TAIKO_DRUM.RIM_LEFT, rimLeft ? 1.0f : 0.0f);
            yield return new KeyValuePair<string, float>(INPUT_NAMES.TAIKO_DRUM.RIM_RIGHT, rimRight ? 1.0f : 0.0f);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
