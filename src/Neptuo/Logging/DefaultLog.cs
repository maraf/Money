using Neptuo.Logging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Logging
{
    /// <summary>
    /// Default implementation of <see cref="ILog"/>.
    /// </summary>
    public class DefaultLog : ILog
    {
        private readonly IEnumerable<ILogSerializer> serializers;
        private readonly string scopeName;
        private ILogFactory factory;

        public ILogFactory Factory
        {
            get
            {
                if (factory == null)
                    factory = new DefaultLogFactory(scopeName, serializers);

                return factory;
            }
        }

        /// <summary>
        /// Creates empty root log.
        /// </summary>
        public DefaultLog()
            : this("Root", Enumerable.Empty<ILogSerializer>())
        { }

        /// <summary>
        /// Creates new instance with <paramref name="serializers"/> in scope <paramref name="scopeName"/>.
        /// </summary>
        /// <param name="scopeName">Current log scope name.</param>
        /// <param name="serializers">Serializers for current log.</param>
        public DefaultLog(string scopeName, IEnumerable<ILogSerializer> serializers)
        {
            Ensure.NotNull(serializers, "serializers");
            this.serializers = serializers;
            this.scopeName = scopeName;
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            return serializers.Any(w => w.IsEnabled(scopeName, level));
        }

        public void Log(LogLevel level, object model)
        {
            foreach (ILogSerializer serializer in serializers)
                serializer.Append(scopeName, level, model);
        }
    }
}
