using Neptuo.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Snapshots
{
    /// <summary>
    /// The contract for determing whether to create snapshot from current instance of an aggregate.
    /// </summary>
    public interface ISnapshotProvider
    {
        /// <summary>
        /// Tries to create snapshot from the <paramref name="aggregate"/>.
        /// </summary>
        /// <param name="aggregate">The instance of an aggregate.</param>
        /// <param name="snapshot">The snapshot to save or <c>null</c> to save event stream only.</param>
        /// <returns><c>true</c> if snapshot was created; <c>false</c> otherwise.</returns>
        bool TryCreate(IAggregateRoot aggregate, out ISnapshot snapshot);
    }
}
