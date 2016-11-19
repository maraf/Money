using Neptuo.Events;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Domains
{
    /// <summary>
    /// Describes event-sourcing compatible aggregate root.
    /// </summary>
    public interface IAggregateRoot : IDomainModel<IKey>
    {
        /// <summary>
        /// The current version of the aggregate root.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// The enumeration of unpublished events.
        /// </summary>
        IEnumerable<IEvent> Events { get; }
    }
}
