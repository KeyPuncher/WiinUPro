namespace ShiftPad.Core.Battery
{
    public interface IBateryState
    {
        public bool IsPresent { get; }
        public bool IsCharging { get; }
        public BateryStatus Status { get; }
        public float Percentage { get; }
    }
}
