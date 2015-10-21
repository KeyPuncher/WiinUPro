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
        public MouseInput Input { get; set; }

        public float Rate { get; set; }

        public bool Absolute { get; set; }

        public void Apply(float value)
        {
            throw new NotImplementedException();
        }

        public bool SameAs(IAssignment assignment)
        {
            throw new NotImplementedException();
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
            return base.GetHashCode();
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
        MoveRight,
        ScrollUp,
        ScrollDown
    }
}
