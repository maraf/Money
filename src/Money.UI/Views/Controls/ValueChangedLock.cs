using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.Controls
{
    /// <summary>
    /// A helper for checking wheter some operation is already executing.
    /// </summary>
    internal class ValueChangedLock
    {
        public bool IsLocked { get; private set; }

        public System.IDisposable Lock()
        {
            if (IsLocked)
                throw Ensure.Exception.InvalidOperation("Already locked");

            return new LockDisposable(this);
        }

        private class LockDisposable : System.IDisposable
        {
            private readonly ValueChangedLock listener;

            public LockDisposable(ValueChangedLock listener)
            {
                Ensure.NotNull(listener, "listener");
                this.listener = listener;
                this.listener.IsLocked = true;
            }

            public void Dispose()
            {
                listener.IsLocked = false;
            }
        }
    }
}
