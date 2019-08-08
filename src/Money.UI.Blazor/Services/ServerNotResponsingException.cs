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
    public class ServerNotResponsingException : Exception
    {
        public ServerNotResponsingException(Exception inner)
            : base("Remote server is not responding.", inner)
        { }

        /// <summary>
        /// Creates a new instance for deserialization.
        /// </summary>
        /// <param name="info">A serialization info.</param>
        /// <param name="context">A streaming context.</param>
        protected ServerNotResponsingException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { }
    }
}
