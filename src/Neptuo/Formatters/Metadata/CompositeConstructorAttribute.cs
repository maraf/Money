using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters.Metadata
{
    /// <summary>
    /// Defines constructor that is used to reconstruct instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class CompositeConstructorAttribute : Attribute
    {
        /// <summary>
        /// The version this settings apply to.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Creates new instance with <see cref="Version"/> set to <c>1</c>.
        /// </summary>
        public CompositeConstructorAttribute()
        {
            Version = 1;
        }
    }
}
