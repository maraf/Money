using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    /// <summary>
    /// The underlying store for the persistent command delivery.
    /// It is responsible for persisting delivery confirmation, loading missed delivery and clearing unpublished queue.
    /// </summary>
    public interface ICommandPublishingStore : ICommandStore
    {
        /// <summary>
        /// Returns the enumeration of the unpublished commands.
        /// </summary>
        /// <returns>The enumeration of the unpublished commands.</returns>
        Task<IEnumerable<CommandModel>> GetAsync();

        /// <summary>
        /// Saves the information about publishing <paramref name="commandKey"/>.
        /// </summary>
        /// <param name="commandKey">The key of the published command.</param>
        /// <returns>The continuation task.</returns>
        Task PublishedAsync(IKey commandKey);

        /// <summary>
        /// Clears the queue of the unpublished commands.
        /// </summary>
        /// <returns>The continuation task.</returns>
        Task ClearAsync();
    }
}
