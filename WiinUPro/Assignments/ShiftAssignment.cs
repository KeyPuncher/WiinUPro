using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    public class ShiftAssignment : IAssignment
    {
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

        private ShiftState _previousState;
        private float _threashold = 0.1f;
        private bool _isEnabled = false;

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
                        if (ToggleStates.Contains((ShiftState)ShiftDirector.CurrentShiftState))
                        {
                            int index = ToggleStates.IndexOf((ShiftState)ShiftDirector.CurrentShiftState);
                            
                            if (ToggleStates.Count > index)
                            {
                                ShiftDirector.ChangeState(ToggleStates[index]);
                            }
                            else
                            {
                                ShiftDirector.ChangeState(ToggleStates[0]);
                            }
                        }
                        else
                        {
                            ShiftDirector.ChangeState(ToggleStates[0]);
                        }
                    }
                }
                else if (isDown)
                {
                    _previousState = (ShiftState)ShiftDirector.CurrentShiftState;
                    ShiftDirector.ChangeState(TargetState);
                }
                else
                {
                    ShiftDirector.ChangeState(_previousState);
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
}
