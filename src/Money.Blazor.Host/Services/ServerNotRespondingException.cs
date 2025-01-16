using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    [Serializable]
    public class ServerNotRespondingException : Exception
    {
        public ServerNotRespondingException(Exception inner)
            : base("Remote server is not responding.", inner)
        { }
    }
}
