using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    class ScpDirector
    {
        public static ScpDirector Access { get; protected set; }

        static ScpDirector()
        {
            // TODO Director: Initialize SCPControl
        }

        public bool Available { get; protected set; }
    }
}
