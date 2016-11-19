using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// Defines property, that should be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CompositePropertyAttribute : Attribute
    {
        /// <summary>
        /// The order of the property in composite constructor.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The version this settings apply to.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Creates new instance with <paramref name="index"/> definition and <see cref="Version"/> set to <c>1</c>.
        /// </summary>
        /// <param name="index">The order of the property in composite constructor.</param>
        public CompositePropertyAttribute(int index)
        {
            Ensure.PositiveOrZero(index, "order");
            Index = index;
            Version = 1;
        }
    }
}
