using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events
{
    /// <summary>
    /// Base implementation of <see cref="IEvent"/> with support for automatic fill of parameters from <see cref="AggregateRoot"/>.
    /// </summary>
    public abstract class Event : IEvent
    {
        /// <summary>
        /// The key of this event.
        /// </summary>
        public IKey Key { get; internal set; }

        /// <summary>
        /// The key of the aggregate where originated.
        /// </summary>
        public IKey AggregateKey { get; internal set; }

        /// <summary>
        /// The version of the aggregate.
        /// </summary>
        public int Version { get; internal set; }

        /// <summary>
        /// Creates new empty instance.
        /// </summary>
        protected Event()
        {
            Key = KeyFactory.Create(GetType());
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="key">The key of the event.</param>
        /// <param name="aggregateKey">The key of the aggregate where originated.</param>
        /// <param name="version">The version of the aggregate.</param>
        protected Event(IKey key, IKey aggregateKey, int version)
            : this()
        {
            Ensure.Condition.NotEmptyKey(key, "key");
            Ensure.Condition.NotEmptyKey(aggregateKey, "aggregateKey");
            Key = key;
            AggregateKey = aggregateKey;
            Version = version;
        }
    }
}
