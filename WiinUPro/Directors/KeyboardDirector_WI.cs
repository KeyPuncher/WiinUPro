using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace WiinUPro
{
    class KeyboardDirector_WI
    {
        #region Access
        public static KeyboardDirector_WI Access { get; protected set; }

        static KeyboardDirector_WI()
        {
            Access = new KeyboardDirector_WI();
        }

        public static VirtualKeyCode TranslateVirtualKeyCode(InputManager.VirtualKeyCode code)
        {
            return (VirtualKeyCode)code;
        }
        #endregion

        private IKeyboardSimulator _keyboard;
        private List<VirtualKeyCode> _pressedKeys;

        public KeyboardDirector_WI()
        {
            _keyboard = new KeyboardSimulator(InputSim.Simulator);
            _pressedKeys = new List<VirtualKeyCode>();
        }

        public void KeyDown(VirtualKeyCode code)
        {
            if (!_pressedKeys.Contains(code))
            {
                _keyboard.KeyDown(code);
                _pressedKeys.Add(code);
            }
        }

        public void KeyUp(VirtualKeyCode code)
        {
            if (_pressedKeys.Contains(code))
            {
                _keyboard.KeyUp(code);
                _pressedKeys.Remove(code);
            }
        }

        public void KeyPress(VirtualKeyCode code)
        {
            _keyboard.KeyPress(code);
        }

        public void DetectKey()
        {
            // TODO Director: start key detection
        }

        public void Release()
        {
            foreach (var key in _pressedKeys.ToArray())
            {
                _keyboard.KeyUp(key);
            }

            _pressedKeys.Clear();
        }
    }

    static class InputSim
    {
        static IInputSimulator _simulator;
        public static IInputSimulator Simulator
        {
            get
            {
                return _simulator;
            }
        }

        static InputSim()
        {
            _simulator = new InputSimulator();
        }
    }
}
