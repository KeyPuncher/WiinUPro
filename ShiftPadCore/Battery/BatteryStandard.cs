namespace ShiftPad.Core.Battery
{
    public class BatteryStandard : IBateryState
    {
        public bool IsPresent { get; set; }
        public bool IsCharging { get; set; }
        public BateryStatus Status { get; private set; }
        public float Percentage { get; private set; }

        private float _percentageRatio;

        public BatteryStandard(float percentageRatio)
        {
            _percentageRatio = percentageRatio;
        }

        public void Update(uint batteryValue)
        {
            Percentage = (float)batteryValue * _percentageRatio;

            if (Percentage > 85f)
                Status = BateryStatus.Full;
            else if (Percentage > 65f)
                Status = BateryStatus.High;
            else if (Percentage > 40f)
                Status = BateryStatus.Medium;
            else if (Percentage > 25f)
                Status = BateryStatus.Low;
            else
                Status = BateryStatus.VeryLow;
        }
    }
}
