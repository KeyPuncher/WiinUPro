using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiinUPro
{
    interface IControllerMap
    {
        List<IAssignment>[][] Assignments { get; set; }
    }
}
