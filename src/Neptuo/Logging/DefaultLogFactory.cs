using Neptuo.Logging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neptuo.Logging
{
    /// <summary>
    /// Default implementation of <see cref="ILogFactory"/>.
    /// </summary>
    public class DefaultLogFactory : ILogFactory
    {
        private readonly List<ILogSerializer> serializers;

        public string ScopeName { get; private set; }

        /// <summary>
        /// Creates new root log factory.
        /// No scope name will be used.
        /// </summary>
        public DefaultLogFactory()
            : this(String.Empty, Enumerable.Empty<ILogSerializer>())
        { }

        /// <summary>
        /// Creates new log factory with <paramref name="scopeName"/>.
        /// </summary>
        /// <param name="scopeName">Scope name.</param>
        public DefaultLogFactory(string scopeName)
            : this(scopeName, Enumerable.Empty<ILogSerializer>())
        { }

        /// <summary>
        /// Cretes new log factory with <paramref name="scopeName"/> and <paramref name="initialSerializers"/>.
        /// </summary>
        /// <param name="scopeName">Scop name.</param>
        /// <param name="initialSerializers">Enumeration of initial serializers (this enumeration is copies).</param>
        public DefaultLogFactory(string scopeName, IEnumerable<ILogSerializer> initialSerializers)
        {
            Ensure.NotNull(initialSerializers, "initialSerializers");
            ScopeName = scopeName;
            this.serializers = new List<ILogSerializer>(initialSerializers);
        }

        public ILog Scope(string scopeName)
        {
            Ensure.NotNullOrEmpty(scopeName, "scopeName");
            return new DefaultLog(ScopeName + "." + scopeName, serializers);
        }

        public ILogFactory AddSerializer(ILogSerializer serializer)
        {
            Ensure.NotNull(serializer, "serializer");
            serializers.Add(serializer);
            return this;
        }
    }
}
