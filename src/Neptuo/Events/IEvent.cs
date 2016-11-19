using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events
{
    /// <summary>
    /// Describes event-sourcing compatible event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The key of this event.
        /// </summary>
        IKey Key { get; }

        /// <summary>
        /// The key of the aggregate where originated.
        /// </summary>
        IKey AggregateKey { get; }

        /// <summary>
        /// The version of the aggregate.
        /// </summary>
        int Version { get; }
    }
}
