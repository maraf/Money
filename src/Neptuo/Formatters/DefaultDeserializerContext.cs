using Neptuo.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Formatters
{
    /// <summary>
    /// Default implementation of <see cref="IDeserializerContext"/>.
    /// </summary>
    public class DefaultDeserializerContext : IDeserializerContext
    {
        public object Output { get; set; }
        public Type OutputType { get; private set; }

        private KeyValueCollection metadata;

        public IKeyValueCollection Metadata
        {
            get
            {
                if (metadata == null)
                    metadata = new KeyValueCollection();

                return metadata;
            }
        }

        IReadOnlyKeyValueCollection IDeserializerContext.Metadata
        {
            get { return Metadata; }
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="outputType">The type that is required to deserialize.</param>
        public DefaultDeserializerContext(Type outputType)
        {
            Ensure.NotNull(outputType, "outputType");
            OutputType = outputType;
        }
    }
}
