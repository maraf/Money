using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    /// <summary>
    /// The underlying store for the persistent event delivery.
    /// It is responsible for persisting delivery confirmation, loading missed delivery and clearing unpublished queue.
    /// </summary>
    public interface IEventPublishingStore : IEventStore
    {
        /// <summary>
        /// Returns the enumeration of the unpublished events (<see cref="EventPublishingModel"/>) 
        /// which were published to the handlers (<see cref="EventPublishingModel.PublishedHandlerIdentifiers"/>).
        /// </summary>
        /// <returns>The enumeration of the unpublished events which were published to the handlers.</returns>
        Task<IEnumerable<EventPublishingModel>> GetAsync();

        /// <summary>
        /// Saves the information about publishing <paramref name="eventKey"/> to the <paramref name="handlerIdentifier"/>.
        /// </summary>
        /// <param name="eventKey">The key of the published event.</param>
        /// <param name="handlerIdentifier">The identifier of the handler where the event was published.</param>
        /// <returns>The continuation task.</returns>
        Task PublishedAsync(IKey eventKey, string handlerIdentifier);

        /// <summary>
        /// Clears the queue of the unpublished events.
        /// </summary>
        /// <returns>The continuation task.</returns>
        Task ClearAsync();
    }
}
