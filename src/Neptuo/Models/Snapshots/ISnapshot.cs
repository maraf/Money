using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Snapshots
{
    /// <summary>
    /// The marker interface for aggregate root snapshot class.
    /// </summary>
    public interface ISnapshot
    {
        /// <summary>
        /// Gets the key of the aggregate where the snapshot belongs.
        /// </summary>
        IKey AggregateKey { get; }

        /// <summary>
        /// Gets the version of the aggregate when the snaphot was created.
        /// </summary>
        int Version { get; }
    }
}
