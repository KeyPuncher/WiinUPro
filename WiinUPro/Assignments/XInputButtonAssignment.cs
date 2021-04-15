using System;
using ScpControl;

namespace WiinUPro
{
    public class XInputButtonAssignment : IAssignment
    {
        /// <summary>
        /// The XInput Device to use
        /// </summary>
        public ScpDirector.XInput_Device Device { get; set; }

        /// <summary>
        /// The Xbox 360 Button to be simulated
        /// </summary>
        public X360Button Button { get; set; }

        /// <summary>
        /// Set to use Turbo feature
        /// </summary>
        public bool TurboEnabled
        {
            get { return _turboEnabled; }
            set
            {
                if (_turboEnabled == value) return;

                if (value)
                {
                    _stopWatch = new System.Diagnostics.Stopwatch();
                    _stopWatch.Start();
                }
                else
                {
                    _stopWatch = null;
                }

                _turboEnabled = value;
            }
        }

        /// <summary>
        /// Turbo rate in milliseconds (0ms to 1000ms)
        /// </summary>
        public int TurboRate
        {
            get { return _turboRate; }
            set { _turboRate = Math.Min(Math.Max(0, value), 1000); }
        }

        /// <summary>
        /// What the applied value must be greater than to apply
        /// </summary>
        public float Threshold
        {
            get { return _threashold; }
            set { _threashold = value; }
        }

        /// <summary>
        /// Set to apply key simulation when input is not being applied
        /// </summary>
        public bool InverseInput { get; set; }

        private bool _turboEnabled = false;
        private int _turboRate = 200;
        private float _threashold = 0.1f;
        private bool _lastState = false;
        private double _lastApplied = 0;
        private System.Diagnostics.Stopwatch _stopWatch;

        public XInputButtonAssignment() { }

        public XInputButtonAssignment(X360Button button, ScpDirector.XInput_Device device = ScpDirector.XInput_Device.Device_A)
        {
            Button = button;
            Device = device;
        }

        public void Apply(float value)
        {
            bool isDown = value >= Threshold;

            if (InverseInput)
            {
                isDown = !isDown;
            }

            if (TurboEnabled)
            {
                if (!isDown)
                {
                    if (_lastState)
                    {
                        ScpDirector.Access.SetButton(Button, false, Device);
                        _lastState = false;
                    }

                    return;
                }

                double tick = Math.Floor(_stopWatch.ElapsedMilliseconds / (double)TurboRate);

                if (tick > _lastApplied)
                {
                    if (_lastState)
                    {
                        ScpDirector.Access.SetButton(Button, false, Device);
                        _lastState = false;
                    }
                    else
                    {
                        ScpDirector.Access.SetButton(Button, true, Device);
                        _lastState = true;
                    }

                    _lastApplied = tick;
                }
            }
            else if (isDown != _lastState)
            {
                ScpDirector.Access.SetButton(Button, isDown, Device);
                _lastState = isDown;
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as XInputButtonAssignment;

            if (other == null)
            {
                return false;
            }

            bool result = true;

            result &= Button == other.Button;
            result &= Device == other.Device;
            result &= InverseInput == other.InverseInput;
            result &= Threshold == other.Threshold;
            result &= TurboEnabled == other.TurboEnabled;
            result &= TurboRate == other.TurboRate;

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as XInputButtonAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return Button == other.Button && Device == other.Device;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int)Button + (int)Device;
            return hash;
        }

        public override string ToString()
        {
            return Button.ToString();
        }

        public string GetDisplayName()
        {
            return $"X{Device.ToString().Replace("Device_","")}.{ToString()}";
        }
    }
}
