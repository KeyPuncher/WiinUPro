using System.Collections.Generic;

namespace NintrollerLib
{
    public interface INintrollerState : IEnumerable<KeyValuePair<string, float>>
    {
        void Update(byte[] data);
        float GetValue(string input);
        void SetCalibration(Calibrations.CalibrationPreset preset);
        void SetCalibration(INintrollerState from);
        void SetCalibration(string calibrationString);
        string GetCalibrationString();
        bool CalibrationEmpty { get; }
    }

    public interface IWiimoteExtension
    {
        Wiimote wiimote { get; }
    }

    public interface INintrollerParsable
    {
        void Parse(byte[] input, int offset = 0);
    }

    public interface INintrollerNormalizable
    {
        void Normalize();
    }

    public interface INintrollerBounds
    {
        bool InBounds(float x, float y = 0);
    }
}
