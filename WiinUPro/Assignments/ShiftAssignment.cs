using System.Collections.Generic;

namespace WiinUPro
{
    public class ShiftAssignment : IAssignment
    {
        public const int SHIFT_STATE_COUNT = 4;

        /// <summary>
        /// The state to enter when used.
        /// </summary>
        public ShiftState TargetState { get; set; }

        /// <summary>
        /// True if this shift assignment toggles between states.
        /// </summary>
        public bool Toggles { get; set; }

        /// <summary>
        /// The shift states to be toggle between
        /// </summary>
        public List<ShiftState> ToggleStates { get; set; }

        /// <summary>
        /// What the applied value must be greater than to apply
        /// </summary>
        public float Threshold
        {
            get { return _threashold; }
            set { _threashold = value; }
        }

        private IDeviceControl _control;
        private ShiftState _previousState;
        private float _threashold = 0.1f;
        private bool _isEnabled = false;

        public ShiftAssignment() { }

        public ShiftAssignment(IDeviceControl control)
        {
            _control = control;
            ToggleStates = new List<ShiftState>();
        }

        public void Apply(float value)
        {
            bool isDown = value > Threshold;

            if (_isEnabled != isDown)
            {
                _isEnabled = isDown;

                if (Toggles)
                {
                    if (isDown)
                    {
                        if (ToggleStates.Contains(_control.CurrentShiftState))
                        {
                            int index = ToggleStates.IndexOf(_control.CurrentShiftState);
                            
                            if (ToggleStates.Count > index + 1)
                            {
                                _control.ChangeState(ToggleStates[index + 1]);
                            }
                            else
                            {
                                _control.ChangeState(ToggleStates[0]);
                            }
                        }
                        else
                        {
                            _control.ChangeState(ToggleStates[0]);
                        }
                    }
                }
                else if (isDown)
                {
                    _previousState = _control.CurrentShiftState;
                    _control.ChangeState(TargetState);
                }
                else
                {
                    _control.ChangeState(_previousState);
                }
            }
        }

        public bool SameAs(IAssignment assignment)
        {
            var obj = assignment as ShiftAssignment;

            if (assignment == null)
            {
                return false;
            }

            bool result = true;
            result &= Toggles == obj.Toggles;
            if (Toggles)
            {
                result &= ToggleStates == obj.ToggleStates;
            }
            else
            {
                result &= TargetState == obj.TargetState;
            }

            return result;
        }

        public void SetControl(IDeviceControl control)
        {
            _control = control;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ShiftAssignment;

            if (other == null)
            {
                return false;
            }

            if (Toggles != other.Toggles)
            {
                return false;
            }

            if (Toggles)
            {
                return TargetState == other.TargetState;
            }
            else
            {
                return ToggleStates == other.ToggleStates;
            }
        }

        public override int GetHashCode()
        {
            int hash = (int)TargetState + 1;

            if (Toggles && ToggleStates != null)
            {
                hash = (hash * 17) + ToggleStates.Count;
                foreach (var state in ToggleStates)
                {
                    hash = (hash * 17) + (int)state;
                }
            }

            return hash;
        }

        public override string ToString()
        {
            string result = "";

            if (Toggles)
            {
                foreach (var state in ToggleStates)
                {
                    result += state;
                }
            }
            else
            {
                result = TargetState.ToString();
            }

            return result;
        }

        public string GetDisplayName()
        {
            if (Toggles)
            {
                string s = "Toggle";
                foreach (var shift in ToggleStates)
                    s += $".{shift}";

                return s;
            }
            else
            {
                return $"Shift.{TargetState}";
            }
        }
    }

    public enum ShiftState
    {
        None    = 0,
        Red     = 1,
        Blue    = 2,
        Green   = 3
    }
}
