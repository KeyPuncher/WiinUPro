using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputManager;

namespace WiinUPro
{
    public class MouseScrollAssignment : IAssignment
    {
        /// <summary>
        /// The Scroll direction to be simulated
        /// </summary>
        public Mouse.ScrollDirection ScrollDirection { get; set; }

        /// <summary>
        /// Set to constantly scroll while Applying
        /// </summary>
        public bool Continuous { get; set; }

        /// <summary>
        /// What the applied value must be greater than to apply
        /// </summary>
        public float Threshold
        {
            get { return _threashold; }
            set { _threashold = value; }
        }

        // TODO: Scroll Rate

        private float _threashold = 0.1f;
        private bool _lastState = false;
        
        public MouseScrollAssignment(Mouse.ScrollDirection dir)
        {
            ScrollDirection = dir;
        }

        public void Apply(float value)
        {
            bool isDown = value >= _threashold;

            if (isDown != _lastState)
            {
                if (isDown)
                {
                    MouseDirector.Access.MouseScroll(ScrollDirection);
                }

                _lastState = isDown;
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as MouseScrollAssignment;

            if (other == null)
            {
                return false;
            }

            bool result = true;

            result &= ScrollDirection == other.ScrollDirection;
            result &= Continuous == other.Continuous;
            result &= Threshold == other.Threshold;

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as MouseScrollAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return ScrollDirection == other.ScrollDirection;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int)ScrollDirection + 51;
            return hash;
        }

        public override string ToString()
        {
            return ScrollDirection.ToString();
        }
    }
}
