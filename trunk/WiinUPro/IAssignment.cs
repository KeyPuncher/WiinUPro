using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    public interface IAssignment
    {
        void Apply(float value);
    }

    public class TestAssignment : IAssignment
    {
        private WindowsInput.Native.VirtualKeyCode _key;

        public TestAssignment(WindowsInput.Native.VirtualKeyCode key)
        {
            _key = key;
        }

        public void Apply(float value)
        {
            if (value > 0.1f)
            {
                KeyboardDirector.Access.KeyPress(_key);
            }
        }
    }

    public class TestMouseAssignment : IAssignment
    {
        private bool xAxis;

        private static float x, x2;
        private static float y, y2;

        public TestMouseAssignment(bool isX)
        {
            xAxis = isX;
        }

        public void Apply(float value)
        {
            value = (float)Math.Round(value, 2);

            if (xAxis)
            {
                //MouseDirector.Access.MouseMoveX((int)Math.Round(10 * value));
                //MouseDirector.Access.MouseMoveTo(value, 0);
                x = value;
            }
            else
            {
                //MouseDirector.Access.MouseMoveY((int)Math.Round(10 * value));
                // MouseDirector.Access.MouseMoveTo(0, value);
                y = value;
            }

            if (x != x2 || y != y2)
            {
                MouseDirector.Access.MouseMoveTo(x, y);
                x2 = x;
                y2 = y;
            }
        }
    }
}
