using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Threading
{
    /// <summary>
    /// Works like <see cref="LockProvider"/>, but supports multiple paralel processes.
    /// These processes are distinguished by key parameter to the method <see cref="MultiLockProvider.Lock"/> 
    /// (= call with the same keys are executed one by one; but calls with different keys are executed paralelly).
    /// </summary>
    public class MultiLockProvider
    {
        private readonly Func<object, object> keyMapper;
        private readonly LockProvider storageLock = new LockProvider();
        private readonly Dictionary<object, LockProvider> storage = new Dictionary<object, LockProvider>();

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public MultiLockProvider()
        { }

        /// <summary>
        /// Creates new instance with transformation function for input keys.
        /// </summary>
        /// <param name="keyMapper">Function which transforms input key to keys, which are used internally.</param>
        public MultiLockProvider(Func<object, object> keyMapper)
        {
            Ensure.NotNull(keyMapper, "keyMapper");
            this.keyMapper = keyMapper;
        }

        /// <summary>
        /// Locks inner lock for <paramref name="key"/> for exclusive access.
        /// </summary>
        /// <param name="key">Key to distinguish different locks/processes.</param>
        /// <returns>Object which is used to exit lock when disposed.</returns>
        public IDisposable Lock(object key)
        {
            Ensure.NotNull(key, "key");

            if (keyMapper != null)
                key = keyMapper(key);

            LockProvider provider;
            using (storageLock.Lock())
            {
                if (!storage.TryGetValue(key, out provider))
                {
                    provider = new LockProvider();
                    provider.Disposed += OnProviderDisposed;
                    storage[key] = provider;
                }
            }

            return provider.Lock();
        }

        private void OnProviderDisposed(LockProvider provider)
        {
            if (!provider.IsLocked)
            {
                using (storageLock.Lock())
                {
                    if (!provider.IsLocked)
                    {
                        object key = storage.FirstOrDefault(x => x.Value == provider);
                        if (key != null)
                            storage.Remove(key);
                    }
                }
            }
        }
    }
}
