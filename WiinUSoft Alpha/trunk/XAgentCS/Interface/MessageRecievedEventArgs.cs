using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XAgentCS.Interface
{
    [Serializable]
    public class MessageReceivedEventArgs : MarshalByRefObject
    {
        public string Message { get; set; }

        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
