using System;
using System.Runtime.Serialization;

namespace Neptuo.Activators
{
    /// <summary>
    /// The exception raised when the aggregate root type doesn't support constructor with snapshot.
    /// </summary>
    public class SnapshotConstructorNotSupportedException : Exception
    {
        /// <summary>
        /// Gets the type of the aggregate root that doesn't support constructor with snapshot.
        /// </summary>
        public Type AggregateRootType { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public SnapshotConstructorNotSupportedException(Type aggregateRootType)
        {
            AggregateRootType = aggregateRootType;
        }
    }
}