using Neptuo;
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
    /// Exception raised when server API is not supported by the current client version.
    /// </summary>
    [Serializable]
    public class NotSupportedApiVersionException : Exception
    {
        public Version ApiVersion { get; }

        /// <summary>
        /// Creates a new instance with server api version.
        /// </summary>
        /// <param name="apiVersion">The server API version.</param>
        public NotSupportedApiVersionException(Version apiVersion)
            : base($"Server API version is 'v{apiVersion}' which is not support by current client version.")
        {
            Ensure.NotNull(apiVersion, "apiVersion");
            ApiVersion = apiVersion;
        }

        /// <summary>
        /// Creates new instance for deserialization.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected NotSupportedApiVersionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
