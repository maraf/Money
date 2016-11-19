using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// The context information for deserializing objects.
    /// </summary>
    public interface IDeserializerContext
    {
        /// <summary>
        /// The deserialized object.
        /// </summary>
        object Output { get; set; }

        /// <summary>
        /// The type that is required to deserialize.
        /// </summary>
        Type OutputType { get; }

        /// <summary>
        /// The metadata of the context.
        /// </summary>
        IReadOnlyKeyValueCollection Metadata { get; }
    }
}
