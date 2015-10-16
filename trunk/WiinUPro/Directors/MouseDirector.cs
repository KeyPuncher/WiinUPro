using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace WiinUPro
{
    class MouseDirector
    {
        #region Access
        public static MouseDirector Access { get; protected set; }

        static MouseDirector()
        {
            Access = new MouseDirector();
        }
        #endregion  

        private IMouseSimulator _mouse;
        private List<byte> _pressedButtons;

        public MouseDirector()
        {
            _mouse = new MouseSimulator(InputSim.Simulator);
            _pressedButtons = new List<byte>();
        }

        public void MouseButtonDown(byte code)
        {
            if (!_pressedButtons.Contains(code))
            {
            switch (code)
            {
                case 0:
                    _mouse.LeftButtonDown();
                    break;

                case 1:
                    _mouse.RightButtonDown();
                    break;

                default:
                    _mouse.XButtonDown(code);
                    break;
            }

                _pressedButtons.Add(code);
            }
        }

        public void MouseButtonUp(byte code)
        {
            if (_pressedButtons.Contains(code))
            {
            switch (code)
            {
                case 0:
                    _mouse.LeftButtonUp();
                    break;

                case 1:
                    _mouse.RightButtonUp();
                    break;

                default:
                    _mouse.XButtonUp(code);
                    break;
            }

                _pressedButtons.Remove(code);
            }
        }

        public void MouseButtonPress(int code)
        {
            switch (code)
            {
                case 0:
                    _mouse.LeftButtonClick();
                    break;

                case 1:
                    _mouse.RightButtonClick();
                    break;

                default:
                    _mouse.XButtonClick(code);
                    break;
            }
        }

        public void MouseButtonDoubleClick(int code)
        {
            switch (code)
            {
                case 0:
                    _mouse.LeftButtonDoubleClick();
                    break;

                case 1:
                    _mouse.RightButtonDoubleClick();
                    break;

                default:
                    _mouse.XButtonDoubleClick(code);
                    break;
            }
        }

        public void MouseMoveX(int amount)
        {
            _mouse.MoveMouseBy(amount, 0);
        }

        public void MouseMoveY(int amount)
        {
            _mouse.MoveMouseBy(0, amount);
        }

        public void MouseMoveTo(float x, float y)
        {
            var w = /*System.Windows.SystemParameters.PrimaryScreenWidth*/ 65535 * x;
            var h = /*System.Windows.SystemParameters.PrimaryScreenHeight*/ 65535 * y;

            _mouse.MoveMouseTo(w, h);
        }

        public void MouseScrollVertical(int amount)
        {
            _mouse.VerticalScroll(amount);
        }

        public void MouseScrollHorizontal(int amount)
        {
            _mouse.HorizontalScroll(amount);
        }

        public void Release()
        {
            foreach (var btn in _pressedButtons)
            {
                if (btn == 0)
                {
                    _mouse.LeftButtonUp();
                }
                else if (btn == 1)
                {
                    _mouse.RightButtonUp();
                }
                else
                {
                    _mouse.XButtonUp(btn);
                }
            }
        }
    }

    // TODO: Test if XButton ids start from 0, 1, or overlap with left and right
    public enum VirtualMouseButton : byte
    {
        Left    = 0,
        Right   = 1,
        Middle  = 2,
        Button4 = 3
    }
}
