using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// Raised when serialization of the input failed.
    /// </summary>
    public class SerializationFailedException : FormatterException
    {
        /// <summary>
        /// The type of the input.
        /// </summary>
        public Type InputType { get; private set; }

        /// <summary>
        /// Creates new instance for failed serialization of <paramref name="inputType"/>.
        /// </summary>
        /// <param name="inputType">The type of the input.</param>
        public SerializationFailedException(Type inputType)
            : base(String.Format("Serialization of the '{0}' failed.", inputType.AssemblyQualifiedName))
        {
            InputType = inputType;
        }
    }
}
