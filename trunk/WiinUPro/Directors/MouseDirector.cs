using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    class MouseDirector
    {
        public static MouseDirector Access { get; protected set; }

        static MouseDirector()
        {
            Access = new MouseDirector();
        }

        public MouseDirector()
        {
            // TODO Director: Initilize mouse hook
        }

        public void MouseButtonDown(int code)
        {
            // TODO Director: mouse btn down
        }

        public void MouseButtonUp(int code)
        {
            // TODO Director: mouse btn up
        }

        public void MouseButtonPress(int code)
        {
            // TODO Director: mouse btn press
        }

        public void MouseMoveX(float amount)
        {
            // TODO Director: mouse movement along X plane
        }

        public void MouseMoveY(float amount)
        {
            // TODO Director: mouse movement along Y plane
        }

        public void MouseMoveTo(float x, float y)
        {
            // TODO Director: absolute mouse movement
            //System.Windows.SystemParameters.PrimaryScreenHeight;
            //System.Windows.SystemParameters.PrimaryScreenWidth;
        }
    }
}
