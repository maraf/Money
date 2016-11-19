using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// Raised when the version property is not present on type.
    /// </summary>
    public class MissingVersionPropertyException : CompositeMetadataException
    {
        /// <summary>
        /// The type where the version property is missing.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="type">The type where the version property is missing.</param>
        public MissingVersionPropertyException(Type type)
            : base(String.Format("Missing version property in type '{0}'.", type.FullName))
        {
            Type = type;
        }
    }
}
