namespace ShiftPad.Core.Gamepad.Elements.Calibrations
{
    /// <summary>
    /// Calibration settings for two-dimensional gamepad elements like joysticks.
    /// </summary>
    public class PlanarCalibration : IElementCalibration<(double x, double y)>
    {
        public virtual (double x, double y) Evaluate((double x, double y) input)
        {
            return (0, 0);
        }
    }
}
