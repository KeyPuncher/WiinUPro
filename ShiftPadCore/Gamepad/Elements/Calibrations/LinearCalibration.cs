namespace ShiftPad.Core.Gamepad.Elements.Calibrations
{
    /// <summary>
    /// Calibration settings for 1-dimensional gamepad elements like triggers.
    /// </summary>
    public struct LinearCalibration : IElementCalibration<double>
    {
        /// <summary>
        /// The minimum raw value must reach to be considered above 0% output.
        /// </summary>
        public double ActivationMin { get; set; }
        /// <summary>
        /// The raw value that once reached will consider the output to be 100%.
        /// </summary>
        public double ActivationMax { get; set; }
        /// <summary>
        /// The value to start the output at.
        /// </summary>
        public double OutputMin { get; set; }
        /// <summary>
        /// The maximum value of the output when at 100%.
        /// </summary>
        public double OutputMax { get; set; }

        public LinearCalibration(double activationMin, double activationMax)
        {
            ActivationMin = activationMin;
            ActivationMax = activationMax;
            OutputMin = 0.0;
            OutputMax = 1.0;
        }

        public LinearCalibration(
            double activationMin,
            double activationMax,
            double outputMin,
            double outputMax)
        {
            ActivationMin = activationMin;
            ActivationMax = activationMax;
            OutputMin = outputMin;
            OutputMax = outputMax;
        }

        public double Evaluate(double input)
        {
            if (input < ActivationMin)
            {
                return 0.0;
            }

            if (input >= ActivationMax)
            {
                return OutputMax;
            }

            var a = input - ActivationMin;
            var b = ActivationMax - ActivationMin;
            if (b != 0.0)
            {
                a /= b;
            }

            return OutputMin + (a * OutputMax);
        }
    }

    /// <summary>
    /// Used for handling asymetric linear valeus (-1,1) such as a slider.
    /// </summary>
    public struct BiLinearCalibration : IElementCalibration<double>
    {
        public LinearCalibration PositiveCalibration { get; private set; }
        public LinearCalibration NegativeCalibration { get; private set; }

        public BiLinearCalibration(LinearCalibration positive, LinearCalibration negative)
        {
            PositiveCalibration = positive;
            NegativeCalibration = negative;
        }

        public double Evaluate(double input)
        {
            var positive = PositiveCalibration.Evaluate(input);
            if (positive != 0.0)
            {
                return positive;
            }

            return NegativeCalibration.Evaluate(input);
        }
    }
}
