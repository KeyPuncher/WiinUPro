namespace ShiftPad.Core.Gamepad.Elements.Calibrations
{
    public interface IElementCalibration
    {
    }

    public interface IElementCalibration<T> : IElementCalibration
    {
        public T Evaluate(T input);
    }
}
