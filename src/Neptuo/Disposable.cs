using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo
{
    /// <summary>
    /// Wrapper for <see cref="System.IDisposable"/> to <see cref="Neptuo.IDisposable"/>.
    /// </summary>
    /// <typeparam name="T">Target disposable type.</typeparam>
    public class Disposable<T> : DisposableBase
        where T : class, System.IDisposable
    {
        private WeakReference<T> reference;

        /// <summary>
        /// Creates new instance for <paramref name="target"/>.
        /// </summary>
        /// <param name="target">Target disposable object.</param>
        public Disposable(T target)
        {
            this.reference = new WeakReference<T>(target);
        }

        /// <summary>
        /// Dispose weak reference.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            T target;
            if (reference.TryGetTarget(out target))
                target.Dispose();
        }
    }
}
