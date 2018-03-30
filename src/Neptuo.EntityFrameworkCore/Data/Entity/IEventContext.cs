using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data.Entity
{
    /// <summary>
    /// Db context contract for storing event stream.
    /// </summary>
    public interface IEventContext
    {
        /// <summary>
        /// The stream of events.
        /// </summary>
        DbSet<EventEntity> Events { get; }

        /// <summary>
        /// The stream of events that needs to be published on the dispatcher.
        /// </summary>
        DbSet<UnPublishedEventEntity> UnPublishedEvents { get; }

        /// <summary>
        /// Saves changes to the storage.
        /// </summary>
        void Save();

        /// <summary>
        /// Saves changes to the storage asynchronously.
        /// </summary>
        Task SaveAsync();
    }
}
