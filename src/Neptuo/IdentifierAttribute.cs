using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// Defines unique type identifier.
    /// This attribute used by the persistent dispatchers to identify handlers that are must have persistent delivery.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class IdentifierAttribute : Attribute
    {
        /// <summary>
        /// The value of the identifier.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Creates new instance with identifier as <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value of the identifier.</param>
        public IdentifierAttribute(string value)
        {
            Ensure.NotNullOrEmpty(value, "value");
            Value = value;
        }
    }
}
