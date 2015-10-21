using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputManager;

namespace WiinUPro.Assignments
{
    public class MouseButtonAssignment : IAssignment
    {
        /// <summary>
        /// The Mouse Button to be simulated
        /// </summary>
        public Mouse.MouseKeys MouseButton { get; set; }

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

        public MouseButtonAssignment(Mouse.MouseKeys btn)
        {
            MouseButton = btn;
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
                    MouseDirector.Access.MouseButtonPress(MouseButton);
                    _lastApplied = now;
                }
            }
            else if (isDown != _lastState)
            {
                if (isDown)
                {
                    MouseDirector.Access.MouseButtonDown(MouseButton);
                }
                else
                {
                    MouseDirector.Access.MouseButtonUp(MouseButton);
                }

                _lastState = isDown;
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as MouseButtonAssignment;

            if (other == null)
            {
                return false;
            }

            bool result = true;

            result &= MouseButton == other.MouseButton;
            result &= InverseInput == other.InverseInput;
            result &= Threshold == other.Threshold;
            result &= TurboEnabled == other.TurboEnabled;
            result &= TurboRate == other.TurboRate;

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as MouseButtonAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return MouseButton == other.MouseButton;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int) MouseButton + 5;
            return hash;
        }

        public override string ToString()
        {
            return MouseButton.ToString();
        }
    }
}
