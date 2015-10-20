using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    static class ShiftDirector
    {
        public const int SHIFT_STATE_COUNT = 4;

        static ShiftDirector()
        {
            _currentState = ShiftState.None;
        }

        public static int CurrentShiftState
        {
            get
            {
                return (int)_currentState;
            }
        }

        private static ShiftState _currentState;

        public static void ChangeState(ShiftState newState)
        {
            if (_currentState != newState)
            {
                KeyboardDirector.Access.Release();
                MouseDirector.Access.Release();
                _currentState = newState;
            }
        }
    }

    public enum ShiftState
    {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 3
    }
}
