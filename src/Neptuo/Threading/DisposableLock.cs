using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Threading
{
    /// <summary>
    /// Wrapper for <see cref="ReaderWriterLockSlim"/> that can be used in using block.
    /// </summary>
    public class DisposableLock : DisposableBase
    {
        private readonly ReaderWriterLockSlim slimLock;

        /// <summary>
        /// Executed when this object is disposed (after unlocking inner lock).
        /// </summary>
        public event Action Disposed;

        /// <summary>
        /// Creates new instance and uses WriteLock for exclusive execution.
        /// </summary>
        /// <param name="slimLock">Inner lock.</param>
        public DisposableLock(ReaderWriterLockSlim slimLock)
        {
            Ensure.NotNull(slimLock, "slimLock");
            this.slimLock = slimLock;
            slimLock.EnterWriteLock();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            slimLock.ExitWriteLock();

            if (Disposed != null)
            {
                Disposed();
                Disposed = null;
            }
        }
    }
}
