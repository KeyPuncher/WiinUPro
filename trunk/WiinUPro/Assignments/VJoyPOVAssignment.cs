using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiinUPro
{
    public class VJoyPOVAssignment : IAssignment
    {
        public uint DeviceId { get; set; }
        public int POVNum { get; set; }
        public VJoyDirector.POVDirection Direction { get; set; }

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
        
        private int _turboRate = 200;
        private float _threashold = 0.1f;
        private bool _lastState = false;
        private int _lastApplied = 0;

        public void Apply(float value)
        {
            bool isDown = value >= Threshold;
            
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

                VJoyDirector.Access.SetPOV(POVNum, Direction, now > _lastApplied + TurboRate, DeviceId);
            }
            else if (isDown != _lastState)
            {
                VJoyDirector.Access.SetPOV(POVNum, Direction, isDown, DeviceId);
                _lastState = isDown;
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            var other = assignment as VJoyPOVAssignment;

            if (other == null)
            {
                return false;
            }

            bool result = true;

            result &= DeviceId == other.DeviceId;
            result &= POVNum == other.POVNum;
            result &= Direction == other.Direction;
            result &= Threshold == other.Threshold;
            result &= TurboEnabled == other.TurboEnabled;
            result &= TurboRate == other.TurboRate;

            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as VJoyPOVAssignment;

            if (other == null)
            {
                return false;
            }
            else
            {
                return Direction == other.Direction && DeviceId == other.DeviceId;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int)Direction + (int)DeviceId;
            return hash;
        }

        public override string ToString()
        {
            return Direction.ToString();
        }
    }
}
