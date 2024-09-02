namespace ShiftPad.Core.Gamepad.Elements.Calibrations
{
    /// <summary>
    /// Calibration settings for 3-dimensional gamepad elements like accelerometers.
    /// </summary>
    public class SpacialCalibration : IElementCalibration<(double, double, double)>
    {
        public (double, double, double) Evaluate((double, double, double) input)
        {
            return (0, 0, 0);
        }
    }
}
