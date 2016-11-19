using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    /// <summary>
    /// TODO: Fill description.
    /// </summary>
    public interface IEventRebuilderStore
    {
        /// <summary>
        /// Returns enumeration of all events of type <paramref name="eventTypes" />.
        /// </summary>
        /// <param name="eventTypes">The enumeration of event types to load.</param>
        /// <returns>Enumeration of all events of type <paramref name="eventTypes" />.</returns>
        Task<IEnumerable<EventModel>> GetAsync(IEnumerable<string> eventTypes);
    }
}
