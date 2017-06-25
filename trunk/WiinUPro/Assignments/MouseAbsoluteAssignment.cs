using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiinUPro
{
    public class MouseAbsoluteAssignment : IAssignment
    {
        static float xPosition = 0.5f;
        static float yPosition = 0.5f;

        public MousePosition Input { get; set; }

        public MouseAbsoluteAssignment() { }

        public MouseAbsoluteAssignment(MousePosition inputType)
        {
            Input = inputType;
        }

        public void Apply(float value)
        {
            switch (Input)
            {
                case MousePosition.X:
                    xPosition = value;
                    break;

                case MousePosition.Y:
                    yPosition = value;
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
    }

    public enum MousePosition
    {
        X,
        Y,
        Center
    }
}
