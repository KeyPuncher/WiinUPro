using ShiftPad.Core.Gamepad.Elements.Calibrations;

namespace ShiftPad.Tests.Core.Calibrations
{
    public class LinearCalibrationTests
    {
        [TestCase(0.0, 0.0)]
        [TestCase(1.0, 1.0)]
        [TestCase(0.3, 0.3)]
        [TestCase(0.8, 0.8)]
        [TestCase(10.0, 1.0)]
        [TestCase(-5.0, 0.0)]
        public void TestEqualityLinearCalibration(double input, double expected)
        {
            var calibration = new LinearCalibration
            {
                ActivationMin = 0.0,
                ActivationMax = 1.0,
                OutputMin = 0.0,
                OutputMax = 1.0
            };

            var result = calibration.Evaluate(input);
            Assert.That(result, Is.EqualTo(expected));
        }

        [TestCase(0.25, 0.0, 0.0)]
        [TestCase(0.25, 0.1, 0.0)]
        [TestCase(0.25, 0.2, 0.0)]
        [TestCase(0.25, 0.24, 0.0)]
        [TestCase(0.25, 0.25, 0.0)]
        [TestCase(0.25, 0.3, 0.05)]
        public void TestLinearCalibrationActivationMin(double min, double input, double expected)
        {
            var calibration = new LinearCalibration
            {
                ActivationMin = min,
                ActivationMax = 1.0,
                OutputMin = 0.0,
                OutputMax = 1.0
            };

            var result = calibration.Evaluate(input);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
