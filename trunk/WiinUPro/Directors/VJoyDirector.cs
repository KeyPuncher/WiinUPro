using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    class VJoyDirector
    {
        public static VJoyDirector Access { get; protected set; }

        static VJoyDirector()
        {
            Access = new VJoyDirector();
        }

        public VJoyDirector()
        {
            // TODO Director: Initilize vJoy
        }

        public bool Available { get; protected set; }

        public int ControllerCount { get; protected set; }
    }
}
