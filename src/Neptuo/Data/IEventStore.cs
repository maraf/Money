using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    /// <summary>
    /// The underlying store for events.
    /// It is responsible for storing serialized events and loading them back.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Returns enumeration of all events raised on <paramref name="aggregateKey"/>.
        /// </summary>
        /// <param name="aggregateKey">The key to the aggregate to load events of.</param>
        /// <returns>Enumeration of all events raised on <paramref name="aggregateKey"/>.</returns>
        IEnumerable<EventModel> Get(IKey aggregateKey);

        /// <summary>
        /// Returns the enumeration of events raised on <paramref name="aggregateKey"/> with higher version than <paramref name="version"/>.
        /// The event with <paramref name="version"/> is not be included.
        /// </summary>
        /// <param name="aggregateKey">The key to the aggregate to load events of.</param>
        /// <param name="version">The last event version, that is skipped.</param>
        /// <returns>The enumeration of events raised on <paramref name="aggregateKey"/> with higher version than <paramref name="version"/>.</returns>
        IEnumerable<EventModel> Get(IKey aggregateKey, int version);

        /// <summary>
        /// Saves <paramref name="events"/> to the underlying storage.
        /// </summary>
        /// <param name="events">The events to save.</param>
        void Save(IEnumerable<EventModel> events);
    }
}
