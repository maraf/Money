using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Internals
{
    /// <summary>
    /// Distributor-based queue for scheduling async operations.
    /// For each distributor object, there exists queue where operations are executed in serie.
    /// </summary>
    internal class TreeQueue
    {
        private readonly object storageLock = new object();
        private readonly Dictionary<object, Queue> storage = new Dictionary<object, Queue>();

        public event Action<Queue> QueueAdded;

        public void Enqueue(object distributor, Func<Task> execute)
        {
            Ensure.NotNull(distributor, "distributor");
            Ensure.NotNull(execute, "execute");

            Queue queue;
            if (!storage.TryGetValue(distributor, out queue))
            {
                lock (storageLock)
                {
                    if (!storage.TryGetValue(distributor, out queue))
                    {
                        storage[distributor] = queue = new Queue();
                        RaiseQueueAdded(queue);
                    }
                }
            }

            lock (queue.Lock)
            {
                queue.Enqueue(execute);
                queue.RaiseItemAdded(execute);
            }
        }

        private void RaiseQueueAdded(Queue queue)
        {
            if (QueueAdded != null)
                QueueAdded(queue);
        }

        /// <summary>
        /// The single queue where each <see cref="Enqueue"/> or <see cref="Dequeue"/> must be locked using <see cref="Lock"/>.
        /// </summary>
        public class Queue : Queue<Func<Task>>
        {
            /// <summary>
            /// Gets the lock object for modifying queue state.
            /// </summary>
            public object Lock { get; private set; }

            /// <summary>
            /// The event that should be raised when item is added.
            /// </summary>
            public event Action<Queue, Func<Task>> ItemAdded;

            /// <summary>
            /// Creates new empty instance.
            /// </summary>
            public Queue()
            {
                Lock = new object();
            }

            /// <summary>
            /// Raises <see cref="ItemAdded"/>.
            /// </summary>
            /// <param name="item">The newly added item.</param>
            public void RaiseItemAdded(Func<Task> item)
            {
                if(ItemAdded != null)
                    ItemAdded(this, item);
            }
        }
    }
}
