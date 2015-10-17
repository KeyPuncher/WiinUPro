using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace WiinUPro
{
    public class KeyboardAssignment : IAssignment
    {
        /// <summary>
        /// The Key to be simulated
        /// </summary>
        public VirtualKeyCode KeyCode { get; set; }

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

        public KeyboardAssignment(VirtualKeyCode key)
        {
            KeyCode = key;
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

                if (now > _lastApplied + TurboRate)
                {
                    KeyboardDirector.Access.KeyPress(KeyCode);
                    _lastApplied = now;
                }
            }
            else if (isDown != _lastState)
            {
                if (isDown)
                {
                    KeyboardDirector.Access.KeyDown(KeyCode);
                }
                else
                {
                    KeyboardDirector.Access.KeyUp(KeyCode);
                }

                _lastState = isDown;
            }
        }

        // Tests if the assignment is exactly the same
        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as KeyboardAssignment;

            if (other == null)
            {
                return false;
            }

            bool result = true;

            result &= KeyCode == other.KeyCode;
            result &= InverseInput == other.InverseInput;
            result &= Threshold == other.Threshold;
            result &= TurboEnabled == other.TurboEnabled;
            result &= TurboRate == other.TurboRate;

            return result;
        }

        // Only compaires the Key being simulated
        public override bool Equals(object obj)
        {
            var other = obj as KeyboardAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return KeyCode == other.KeyCode;
            }
        }
    }
}
