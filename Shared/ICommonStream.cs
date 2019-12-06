using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    interface ICommonStream
    {
        bool OpenConnection();
        void Close();
    }
}
