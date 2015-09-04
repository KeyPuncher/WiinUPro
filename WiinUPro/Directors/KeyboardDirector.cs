using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    class KeyboardDirector
    {
        public static KeyboardDirector Access { get; protected set; }

        static KeyboardDirector()
        {
            Access = new KeyboardDirector();
        }

        public KeyboardDirector()
        {
            // TODO Director: initialize keyboard hook
        }

        public void KeyDown(int code)
        {
            // TODO Director: key down
        }

        public void KeyUp(int code)
        {
            // TODO Director: key up
        }

        public void KeyPress(int code)
        {
            // TODO Director: key press
        }

        public void DetectKey()
        {
            // TODO Director: start key detection
        }
    }
}
