using System;
using System.Collections;
using System.Collections.Generic;

namespace NintrollerLib
{
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
}
