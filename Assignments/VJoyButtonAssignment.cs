using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiinUPro
{
    public class VJoyButtonAssignment : IAssignment
    {
        /// <summary>
        /// VJoy Device ID to apply changes to
        /// </summary>
        public uint DeviceId { get; set; }

        /// <summary>
        /// VJoy Button Number to change
        /// </summary>
        public int Button { get; set; }

        /// <summary>
        /// Set to use Turbo feature
        /// </summary>
        public bool TurboEnabled { get; set; }

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

        private int _turboRate = 200;
        private float _threashold = 0.1f;
        private bool _lastState = false;
        private int _lastApplied = 0;

        public VJoyButtonAssignment() { }

        public VJoyButtonAssignment(int button, uint id = 1)
        {
            Button = button;
            DeviceId = id;
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
                    return;
                }

                int now = DateTime.Now.Millisecond;

                if (_lastApplied > now)
                {
                    _lastApplied = _lastApplied + TurboRate - 1000;
                }
                
                VJoyDirector.Access.SetButton(Button, now > _lastApplied + TurboRate, DeviceId);
            }
            else if (isDown != _lastState)
            {
                VJoyDirector.Access.SetButton(Button, isDown, DeviceId);
                _lastState = isDown;
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as VJoyButtonAssignment;

            if (other == null)
            {
                return false;
            }

            bool result = true;

            result &= Button == other.Button;
            result &= DeviceId == other.DeviceId;
            result &= InverseInput == other.InverseInput;
            result &= Threshold == other.Threshold;
            result &= TurboEnabled == other.TurboEnabled;
            result &= TurboRate == other.TurboRate;

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as VJoyButtonAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return Button == other.Button && DeviceId == other.DeviceId;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int)Button + (int)DeviceId;
            return hash;
        }

        public override string ToString()
        {
            return Button.ToString();
        }
    }
}
