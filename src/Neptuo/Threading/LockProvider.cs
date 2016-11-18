using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Threading
{
    /// <summary>
    /// Wrapps <see cref="ReaderWriterLockSlim"/> and works like a factory for disposable objects for using block.
    /// When disposed, disposes inner lock.
    /// </summary>
    public class LockProvider : DisposableBase
    {
        private readonly ReaderWriterLockSlim slimLock;

        /// <summary>
        /// Executed when objekt from <see cref="LockProvider.Lock"/> is disposed (after unlocking inner lock).
        /// </summary>
        public event Action<LockProvider> Disposed;

        /// <summary>
        /// Returns <c>true</c> is anybody is waiting on inner lock.
        /// </summary>
        public bool IsLocked
        {
            get { return slimLock.WaitingWriteCount != 0; }
        }

        /// <summary>
        /// Creates new instance with self managed inner lock.
        /// </summary>
        public LockProvider()
            : this(new ReaderWriterLockSlim())
        { }

        /// <summary>
        /// Creates new instance and uses WriteLock for exclusive execution.
        /// </summary>
        /// <param name="slimLock">Inner lock.</param>
        public LockProvider(ReaderWriterLockSlim slimLock)
        {
            Ensure.NotNull(slimLock, "slimLock");
            this.slimLock = slimLock;
        }

        /// <summary>
        /// Locks inner lock for exclusive access.
        /// </summary>
        /// <returns>Object which is used to exit lock when disposed.</returns>
        public IDisposable Lock()
        {
            DisposableLock disposableLock = new DisposableLock(slimLock);
            disposableLock.Disposed += OnDisposed;

            return disposableLock;
        }

        private void OnDisposed()
        {
            if (Disposed != null)
                Disposed(this);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            slimLock.Dispose();
        }
    }
}
