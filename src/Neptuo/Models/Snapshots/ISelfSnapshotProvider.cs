using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Snapshots
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISelfSnapshotProvider
    {
        /// <summary>
        /// Tries to create snapshot from current self state.
        /// </summary>
        /// <param name="snapshot">The snapshot to save or <c>null</c> to save event stream only.</param>
        /// <returns><c>true</c> if snapshot was created; <c>false</c> otherwise.</returns>
        bool TryCreate(out ISnapshot snapshot);
    }
}
