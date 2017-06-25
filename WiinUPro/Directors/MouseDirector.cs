using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InputManager;
using System.Windows.Forms;
using System.Drawing;

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
        private Rectangle _screenResolution;

        public MouseDirector()
        {
            _pressedButtons = new List<Mouse.MouseKeys>();

            // Listen for Screen size changes
            _screenResolution = Screen.PrimaryScreen.Bounds;
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
        }

        private void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            _screenResolution = Screen.PrimaryScreen.Bounds;
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
            if (x > 1f) x = 1f;
            else if (x < 0) x = 0;
            if (y > 1f) y = 1f;
            else if (y < 0) y = 0;

            Mouse.Move((int)Math.Floor(x * _screenResolution.Width), (int)Math.Floor(_screenResolution.Height - y * _screenResolution.Height));
        }

        // Will need to change and test how the scrolling works
        public void MouseScroll(Mouse.ScrollDirection scrollDirection)
        {
            Mouse.Scroll(scrollDirection);
        }
        
        public void Release()
        {
            foreach (var btn in _pressedButtons.ToArray())
            {
                Mouse.ButtonUp(btn);
            }
        }
    }
}
