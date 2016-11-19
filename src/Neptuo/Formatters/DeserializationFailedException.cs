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
    public class DeserializationFailedException : FormatterException
    {
        /// <summary>
        /// The type of the input.
        /// </summary>
        public Type OutputType { get; private set; }

        /// <summary>
        /// The serialized input.
        /// </summary>
        public string Input { get; private set; }

        /// <summary>
        /// Creates new instance for failed serialization of <paramref name="outputType"/>.
        /// </summary>
        /// <param name="outputType">The type of the input.</param>
        public DeserializationFailedException(Type outputType, string input)
            : base(String.Format("Deserialization of the '{0}' from '{1}' failed.", outputType.AssemblyQualifiedName, input))
        {
            OutputType = outputType;
            Input = input;
        }
    }
}
