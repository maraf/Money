using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// Base class for implementing <see cref="IDisposable"/>.
    /// Provides posibility to distinguish between disposiing managed and unmanaged resources.
    /// Provides flag to see if object is already disposed.
    /// Once object is disposed, calling <see cref="IDisposable.Disponse"/> has no effect.
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        protected DisposableBase()
        { }

        ~DisposableBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Ensures this object is not disposed.
        /// If <see cref="IsDisposed"/> is <c>true</c>, throws <see cref="ObjectDisposedException"/>.
        /// </summary>
        protected void ThrowWhenDisposed()
        {
            Ensure.NotDisposed(this, "this");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            else
                IsDisposed = true;

            if (disposing)
                DisposeManagedResources();

            DisposeUnmanagedResources();
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources()
        { }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        { }
    }
}
