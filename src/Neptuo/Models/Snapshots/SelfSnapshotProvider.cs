using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Domains;

namespace Neptuo.Models.Snapshots
{
    /// <summary>
    /// The implementation of <see cref="ISnapshotProvider"/> that tries to cast aggregate to <see cref="ISelfSnapshotProvider"/> and
    /// use it to create snapshot.
    /// </summary>
    public class SelfSnapshotProvider : ISnapshotProvider
    {
        public bool TryCreate(IAggregateRoot aggregate, out ISnapshot snapshot)
        {
            ISelfSnapshotProvider provider = aggregate as ISelfSnapshotProvider;
            if (provider == null)
            {
                snapshot = null;
                return false;
            }

            return provider.TryCreate(out snapshot);
        }
    }
}
