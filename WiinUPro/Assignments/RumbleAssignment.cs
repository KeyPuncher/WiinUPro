using System;

namespace WiinUPro
{
    public class RumbleAssignment : IAssignment
    {
        public Action<bool> Callback { get; protected set; }

        private bool lastState = false;

        public RumbleAssignment(Action<bool> callback)
        {
            Callback = callback;
        }

        public void Apply(float value)
        {
            bool newState = Math.Abs(value) > 0.1f;
            
            if (newState != lastState)
            {
                lastState = newState;
                Callback(newState);
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            if (assignment is RumbleAssignment)
            {
                return Callback == (assignment as RumbleAssignment).Callback;
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
