using System;

namespace WiinUPro
{
    public class RumbleAssignment : IAssignment
    {
        private Action<bool> _callback;
        private bool lastState = false;

        public RumbleAssignment(Action<bool> callback)
        {
            _callback = callback;
        }

        public void SetCallback(Action<bool> callback)
        {
            _callback = callback;
        }

        public void Apply(float value)
        {
            bool newState = Math.Abs(value) > 0.1f;
            
            if (newState != lastState)
            {
                lastState = newState;
                _callback(newState);
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            if (assignment is RumbleAssignment)
            {
                return _callback == (assignment as RumbleAssignment)._callback;
            }
            else
            {
                return false;
            }
        }

        public string GetDisplayName()
        {
            return "~";
        }
    }
}
