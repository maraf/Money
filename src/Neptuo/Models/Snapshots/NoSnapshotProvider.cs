using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Domains;

namespace Neptuo.Models.Snapshots
{
    /// <summary>
    /// The implementation of <see cref="ISnapshot"/> that never creates snapshot.
    /// </summary>
    public class NoSnapshotProvider : ISnapshotProvider
    {
        public bool TryCreate(IAggregateRoot aggregate, out ISnapshot snapshot)
        {
            snapshot = null;
            return false;
        }
    }
}
