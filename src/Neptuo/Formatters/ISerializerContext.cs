using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// The context information for serializing objects.
    /// </summary>
    public interface ISerializerContext
    {
        /// <summary>
        /// The serialization output.
        /// </summary>
        Stream Output { get; }

        /// <summary>
        /// The type to serialize.
        /// </summary>
        Type InputType { get; }

        /// <summary>
        /// The metadata of the context.
        /// </summary>
        IReadOnlyKeyValueCollection Metadata { get; }
    }
}
