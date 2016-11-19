using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using Neptuo.Models.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Activators
{
    /// <summary>
    /// Factory for refreshing aggregate root instance in particular state.
    /// </summary>
    /// <typeparam name="T">The type of the aggreagate root.</typeparam>
    public interface IAggregateRootFactory<T>
        where T : AggregateRoot
    {
        /// <summary>
        /// Creates instance of the aggregate and replays <paramref name="events"/>.
        /// </summary>
        /// <param name="aggregateKey">The key of the aggreage.</param>
        /// <param name="events">The enumeration of the events representing current state.</param>
        /// <returns>The refreshed instance of the aggregate root.</returns>
        T Create(IKey aggregateKey, IEnumerable<object> events);

        /// <summary>
        /// Creates instance of the aggregate and replays <paramref name="events"/>.
        /// </summary>
        /// <param name="aggregateKey">The key of the aggreage.</param>
        /// <param name="snapshot">The snapshot to apply as base state.</param>
        /// <param name="events">The enumeration of the events representing current state.</param>
        /// <returns>The refreshed instance of the aggregate root.</returns>
        T Create(IKey aggregateKey, ISnapshot snapshot, IEnumerable<object> events);
    }
}
