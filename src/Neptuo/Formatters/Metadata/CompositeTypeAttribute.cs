using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// Defines type metadata.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CompositeTypeAttribute : Attribute
    {
        /// <summary>
        /// The name of the type (refactoring friendly) to replace .NET type name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates new instance with <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the type (refactoring friendly) to replace .NET type name.</param>
        public CompositeTypeAttribute(string name)
        {
            Ensure.NotNullOrEmpty(name, "name");
            Name = name;
        }
    }
}
