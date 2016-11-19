using Neptuo.Models.Keys;
using Neptuo.Models.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    /// <summary>
    /// The underlying store for snapshots.
    /// </summary>
    public interface ISnapshotStore
    {
        /// <summary>
        /// Tries to find latest snapshot for aggregate identified by <paramref name="aggregateKey"/>.
        /// </summary>
        /// <param name="aggregateKey">The key of the aggregate to find latest snapshot for.</param>
        /// <returns>The latest aggregate root snapshot or <c>null</c>.</returns>
        ISnapshot Find(IKey aggregateKey);

        /// <summary>
        /// Saves the <paramref name="snapshot"/> to the underlying storage.
        /// </summary>
        /// <param name="snapshot">The snapshot to save.</param>
        void Save(ISnapshot snapshot);
    }
}
