using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Keys;
using Neptuo.Models.Snapshots;

namespace Neptuo.Data
{
    /// <summary>
    /// The empty implementation of <see cref="ISnapshotStore"/>.
    /// </summary>
    public class EmptySnapshotStore : ISnapshotStore
    {
        public ISnapshot Find(IKey aggregateKey)
        {
            return null;
        }

        public void Save(ISnapshot snapshot)
        { }
    }
}
