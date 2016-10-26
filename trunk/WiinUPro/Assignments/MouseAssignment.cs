using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputManager;

namespace WiinUPro
{
    public class MouseAssignment : IAssignment
    {
        public const int PIXEL_RATE = 4;

        public MouseInput Input { get; set; }

        public float Rate { get; set; }

        //public bool Absolute { get; set; }

        public MouseAssignment(MouseInput inputType, float rate = 1.0f)
        {
            Input = inputType;
            Rate = rate;
        }

        public void Apply(float value)
        {
            int pixels = (int)Math.Round(PIXEL_RATE * Rate * value);

            switch (Input)
            {
                case MouseInput.MoveUp:
                    MouseDirector.Access.MouseMoveY(pixels * -1);
                    break;

                case MouseInput.MoveDown:
                    MouseDirector.Access.MouseMoveY(pixels);
                    break;

                case MouseInput.MoveLeft:
                    MouseDirector.Access.MouseMoveX(pixels * -1);
                    break;

                case MouseInput.MoveRight:
                    MouseDirector.Access.MouseMoveX(pixels);
                    break;
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as MouseAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                bool result = true;

                result &= Input == other.Input;
                result &= Rate == other.Rate;

                return result;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as MouseAssignment;

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
            hash += (int)Math.Round(10 * Rate);
            return hash;
        }

        public override string ToString()
        {
            return Input.ToString();
        }
    }

    public enum MouseInput
    {
        /// <summary>Positive movement along the Y-axis</summary>
        MoveUp,
        /// <summary>Negative movement along the Y-axis</summary>
        MoveDown,
        /// <summary>Negative movement along the X-axis</summary>
        MoveLeft,
        /// <summary>Positive movement along the X-axis</summary>
        MoveRight
    }
}
