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
        public static MouseDirector Access { get; protected set; }

        static MouseDirector()
        {
            Access = new MouseDirector();
        }

        private IMouseSimulator _mouse;

        public MouseDirector()
        {
            _mouse = new MouseSimulator(InputSim.Simulator);
        }

        public void MouseButtonDown(byte code)
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
        }

        public void MouseButtonUp(byte code)
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
            var w = System.Windows.SystemParameters.PrimaryScreenWidth * x;
            var h = System.Windows.SystemParameters.PrimaryScreenHeight * y;

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
    }

    public enum VirtualMouseButton : byte
    {
        Left    = 0,
        Right   = 1,
        Middle  = 2,
        Button4 = 3
    }
}
