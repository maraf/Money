using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Internals
{
    internal class TreeQueueThreadPool : DisposableBase
    {
        private class Item
        {
            public TreeQueue.Queue Queue { get; private set; }
            public bool IsRunning { get; set; }

            public Item(TreeQueue.Queue queue)
            {
                Queue = queue;
            }
        }

        private readonly TreeQueue root;

        private readonly object itemsLock = new object();
        private readonly List<Item> items = new List<Item>();

        public TreeQueueThreadPool(TreeQueue root)
        {
            Ensure.NotNull(root, "queue");
            this.root = root;
            this.root.QueueAdded += OnQueueAdded;
        }

        private void OnQueueAdded(TreeQueue.Queue queue)
        {
            Item item = new Item(queue);
            queue.ItemAdded += OnQueueItemAdded;

            lock (itemsLock)
                items.Add(item);

            TryExecuteNext(item);
        }

        private void OnQueueItemAdded(TreeQueue.Queue queue, Func<Task> execute)
        {
            Item item = null;
            lock (itemsLock)
                item = items.FirstOrDefault(t => t.Queue == queue);

            if (item == null)
            {
                // TODO: This is weird.
                return;
            }

            TryExecuteNext(item);
        }

        // Thread has completed it's work.
        private void OnQueueItemCompleted(Task t, object state)
        {
            Item item = (Item)state;

            // Free the queue for next work.
            lock (item.Queue.Lock)
                item.IsRunning = false;

            TryExecuteNext(item);
        }

        private void TryExecuteNext(Item item)
        {
            Func<Task> execute = null;

            // Lock the queue to query status or find oldest item.
            lock (item.Queue.Lock)
            {
                // If thread is bussy, do nothing.
                if (item.IsRunning || item.Queue.Count == 0)
                    return;

                execute = item.Queue.Dequeue();

                if (execute != null)
                    item.IsRunning = true;
            }

            if (execute != null)
                Task.Run(execute).ContinueWith(OnQueueItemCompleted, item);
        }
        
        protected override void DisposeUnmanagedResources()
        {
            base.DisposeUnmanagedResources();
            // TODO: Cancel all currently running tasks.
        }
    }
}
