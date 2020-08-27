using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    /// <summary>
    /// API 500.
    /// </summary>
    [Serializable]
    public class InternalServerException : Exception
    {
        public InternalServerException()
            : base("Internal API error.")
        { }

        /// <summary>
        /// Creates a new instance for deserialization.
        /// </summary>
        /// <param name="info">A serialization info.</param>
        /// <param name="context">A streaming context.</param>
        protected InternalServerException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        { }
    }
}
