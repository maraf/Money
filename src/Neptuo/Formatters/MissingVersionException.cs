using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// Raised when composite type hasn't defined specified version.
    /// </summary>
    public class MissingVersionException : CompositeException
    {
        /// <summary>
        /// The type where the <paramref name="Version"/> is missing.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The missing version of the <paramref name="Type"/>.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="type">The type where the <paramref name="Version"/> is missing.</param>
        /// <param name="version">The missing version of the <paramref name="Type"/>.</param>
        public MissingVersionException(Type type, int version)
            : base(String.Format("Type '{0}' doesn't define version '{1}'.", type.FullName, version))
        {
            Type = type;
            Version = version;
        }
    }
}
