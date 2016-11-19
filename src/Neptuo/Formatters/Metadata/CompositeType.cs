using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// A completion versioned definition of type.
    /// </summary>
    public class CompositeType
    {
        /// <summary>
        /// The name of the type.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The underlaying .NET type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The set of versions.
        /// </summary>
        public IEnumerable<CompositeVersion> Versions { get; private set; }

        /// <summary>
        /// The version property to determine version from an instance.
        /// </summary>
        public CompositeProperty VersionProperty { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="name">he name of the type.</param>
        /// <param name="type">The underlaying .NET type.</param>
        /// <param name="versions">The set of versions.</param>
        /// <param name="versionProperty">The version property to determine version from an instance.</param>
        public CompositeType(string name, Type type, IEnumerable<CompositeVersion> versions, CompositeProperty versionProperty)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Ensure.NotNull(type, "type");
            Ensure.NotNull(versions, "versions");
            Ensure.NotNull(versionProperty, "versionProperty");
            Name = name;
            Type = type;
            Versions = versions;
            VersionProperty = versionProperty;
        }
    }
}
