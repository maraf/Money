using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// A single version of composite types.
    /// Defines version number, a constructor and set of properties to this version.
    /// </summary>
    public class CompositeVersion
    {
        /// <summary>
        /// The version of the model definition.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// The constructor for this version.
        /// </summary>
        public CompositeConstructor Constructor { get; private set; }

        /// <summary>
        /// The set of properties for this version.
        /// </summary>
        public IEnumerable<CompositeProperty> Properties { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="version">The version of the model definition.</param>
        /// <param name="constructor">The constructor.</param>
        /// <param name="properties">The set of properties.</param>
        public CompositeVersion(int version, CompositeConstructor constructor, IEnumerable<CompositeProperty> properties)
        {
            Ensure.Positive(version, "version");
            Ensure.NotNull(constructor, "constructor");
            Ensure.NotNull(properties, "properties");
            Version = version;
            Constructor = constructor;
            Properties = properties;
        }
    }
}
