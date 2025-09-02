using System;

namespace WiinUPro
{
    public class MouseAbsoluteAssignment : IAssignment
    {
        static float xPosition = 0.5f;
        static float yPosition = 0.5f;

        public MousePosition Input { get; set; }

        private float _lastValue = float.MinValue;

        public MouseAbsoluteAssignment() { }

        public MouseAbsoluteAssignment(MousePosition inputType)
        {
            Input = inputType;
        }

        public void Apply(float value)
        {
            if (_lastValue == value)
            {
                // Note, this will allow the pointer to be moved when the wiimote is set aside.
                return;
            }

            _lastValue = value;
            float output = (Math.Min(1f, Math.Max(-1f, value)) + 1f) / 2f;

            switch (Input)
            {
                case MousePosition.X:
                    xPosition = output;
                    break;

                case MousePosition.Y:
                        yPosition = output;
                    break;

                case MousePosition.Center:
                    xPosition = 0.5f;
                    yPosition = 0.5f;
                    break;
            }

            MouseDirector.Access.MouseMoveTo(xPosition, yPosition);
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as MouseAbsoluteAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return Input == other.Input;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as MouseAbsoluteAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return Input == other.Input;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int)Input + 1;
            return hash;
        }

        public override string ToString()
        {
            return "Mouse " + Input.ToString() + " Position";
        }

        public string GetDisplayName()
        {
            return $"M.Abs_{Input}";
        }
    }

    public enum MousePosition
    {
        X,
        Y,
        Center
    }
}
