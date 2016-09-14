using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputManager;

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

        private List<Mouse.MouseKeys> _pressedButtons;

        public MouseDirector()
        {
            _pressedButtons = new List<Mouse.MouseKeys>();
        }

        public void MouseButtonDown(Mouse.MouseKeys code)
        {
            if (!_pressedButtons.Contains(code))
            {
                Mouse.ButtonDown(code);
                _pressedButtons.Add(code);
            }
        }

        public void MouseButtonUp(Mouse.MouseKeys code)
        {
            if (_pressedButtons.Contains(code))
            {
                Mouse.ButtonUp(code);
                _pressedButtons.Remove(code);
            }
        }

        public void MouseButtonPress(Mouse.MouseKeys code)
        {
            Mouse.PressButton(code);
        }

        public void MouseMoveX(int amount)
        {
            Mouse.MoveRelative(amount, 0);
        }

        public void MouseMoveY(int amount)
        {
            Mouse.MoveRelative(0, amount);
        }

        public void MouseMoveTo(float x, float y)
        {
            Mouse.Move((int)Math.Floor(x * 100), (int)Math.Floor(y * 100));
        }

        // Will need to change and test how the scrolling works
        public void MouseScroll(Mouse.ScrollDirection scrollDirection)
        {
            Mouse.Scroll(scrollDirection);
        }
        
        public void Release()
        {
            foreach (var btn in _pressedButtons)
            {
                Mouse.ButtonUp(btn);
            }
        }
    }
}
