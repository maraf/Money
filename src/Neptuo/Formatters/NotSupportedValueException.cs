using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// Raised when the serialization/deserialization for passed value is not supported.
    /// </summary>
    public class NotSupportedValueException : CompositeException
    {
        /// <summary>
        /// The type of value unnable to serialize/deserialize.
        /// </summary>
        public Type ValueType { get; private set; }

        /// <summary>
        /// Create new instance.
        /// </summary>
        /// <param name="valueType">The type of value unnable to serialize/deserialize.</param>
        public NotSupportedValueException(Type valueType, string propertyName = null)
            : base(String.Format("Unnable to serialize or deserialize value of type '{0}' for property '{1}'.", valueType, propertyName))
        {
            ValueType = valueType;
        }
    }
}
