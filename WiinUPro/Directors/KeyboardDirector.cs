using System.Collections.Generic;
using InputManager;

namespace WiinUPro
{
    class KeyboardDirector
    {
        #region Access
        public static KeyboardDirector Access { get; protected set; }

        static KeyboardDirector()
        {
            Access = new KeyboardDirector();
        }
        #endregion

        private List<VirtualKeyCode> _pressedKeys;

        public KeyboardDirector()
        {
            _pressedKeys = new List<VirtualKeyCode>();
        }

        public void KeyDown(VirtualKeyCode code)
        {
            if (!_pressedKeys.Contains(code))
            {
                Keyboard.KeyDown((uint)code);
                _pressedKeys.Add(code);
            }
        }

        public void KeyUp(VirtualKeyCode code)
        {
            if (_pressedKeys.Contains(code))
            {
                Keyboard.KeyUp((uint)code);
                _pressedKeys.Remove(code);
            }
        }

        public void KeyPress(VirtualKeyCode code)
        {
            Keyboard.KeyPress(code);
        }

        public void DetectKey()
        {
            // TODO Director: start key detection
        }

        public void Release()
        {
            foreach (var key in _pressedKeys.ToArray())
            {
                Keyboard.KeyUp((uint)key);
            }

            _pressedKeys.Clear();
        }
    }
}
