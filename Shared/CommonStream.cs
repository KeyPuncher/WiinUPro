using System.IO;

namespace Shared
{
    public abstract class CommonStream : Stream
    {
        public abstract bool OpenConnection();
    }
}
