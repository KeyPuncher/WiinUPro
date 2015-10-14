using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace WiinUPro
{
    class KeyboardDirector
    {
        public static KeyboardDirector Access { get; protected set; }

        static KeyboardDirector()
        {
            Access = new KeyboardDirector();
        }

        private IKeyboardSimulator _keyboard;

        public KeyboardDirector()
        {
            _keyboard = new KeyboardSimulator(InputSim.Simulator);
        }

        public void KeyDown(VirtualKeyCode code)
        {
            _keyboard.KeyDown(code);
        }

        public void KeyUp(VirtualKeyCode code)
        {
            _keyboard.KeyUp(code);
        }

        public void KeyPress(VirtualKeyCode code)
        {
            _keyboard.KeyPress(code);
        }

        public void DetectKey()
        {
            // TODO Director: start key detection
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
