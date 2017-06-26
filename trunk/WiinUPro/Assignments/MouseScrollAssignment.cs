using System;
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

        public int ScrollRate
        {
            get { return _scrollRate; }
            set { _scrollRate = Math.Min(Math.Max(0, value), 1000); }
        }

        /// <summary>
        /// What the applied value must be greater than to apply
        /// </summary>
        public float Threshold
        {
            get { return _threashold; }
            set { _threashold = value; }
        }

        private int _scrollRate = 200;
        private float _threashold = 0.1f;
        private bool _lastState = false;
        private int _lastApplied = 0;

        public MouseScrollAssignment() { }
        
        public MouseScrollAssignment(Mouse.ScrollDirection dir)
        {
            ScrollDirection = dir;
        }

        public void Apply(float value)
        {
            bool isDown = value >= _threashold;

            if (Continuous)
            {
                if (!isDown)
                {
                    return;
                }

                int now = DateTime.Now.Millisecond;

                if (_lastApplied > now)
                {
                    _lastApplied = _lastApplied + ScrollRate - 1000;
                }

                if (now > _lastApplied + ScrollRate)
                {
                    MouseDirector.Access.MouseScroll(ScrollDirection);
                    _lastApplied = now;
                }
            }
            else if (isDown != _lastState)
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
            result &= ScrollRate == other.ScrollRate;
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
