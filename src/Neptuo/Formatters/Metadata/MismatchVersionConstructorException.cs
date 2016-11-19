using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// Raised when there is a problem between version properties and version constructor parameters.
    /// </summary>
    public class MismatchVersionConstructorException : CompositeMetadataException
    {
        /// <summary>
        /// The type where the problem occurs.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The version where the problem occurs.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="type">The type where the problem occurs.</param>
        /// <param name="version">The version where the problem occurs.</param>
        public MismatchVersionConstructorException(Type type, int version)
            : base(String.Format("Missing constructor in type '{0}' for version '{1}'.", type.FullName, version))
        {
            Type = type;
            Version = version;
        }
    }
}
