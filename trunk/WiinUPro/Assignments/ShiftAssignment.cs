using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private NintyControl _control;
        private ShiftState _previousState;
        private float _threashold = 0.1f;
        private bool _isEnabled = false;

        public ShiftAssignment(NintyControl control)
        {
            _control = control;
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
                            
                            if (ToggleStates.Count > index)
                            {
                                _control.ChangeState(ToggleStates[index]);
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

            bool isSame = true;
            isSame &= Toggles == obj.Toggles;
            if (Toggles)
            {
                isSame &= ToggleStates == obj.ToggleStates;
            }
            else
            {
                isSame &= TargetState == obj.TargetState;
            }

            return isSame;
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
