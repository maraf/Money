using Neptuo;
using Neptuo.Activators;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Internals;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using Neptuo.Models.Snapshots;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Models.Repositories
{
    /// <summary>
    /// The implementation of EventSourcing AggregateRoot repository.
    /// </summary>
    /// <typeparam name="T">The type of the aggregate root.</typeparam>
    public class AggregateRootRepository<T> : IRepository<T, IKey>
        where T : AggregateRoot
    {
        private readonly IEventStore store;
        private readonly IFormatter formatter;
        private readonly IAggregateRootFactory<T> factory;
        private readonly IEventDispatcher eventDispatcher;
        private readonly ISnapshotProvider snapshotProvider;
        private readonly ISnapshotStore snapshotStore;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="store">The underlaying event store.</param>
        /// <param name="formatter">The formatter for serializing and deserializing event payloads.</param>
        /// <param name="factory">The aggregate root factory.</param>
        /// <param name="eventDispatcher">The dispatcher for newly created events in the aggregates.</param>
        /// <param name="snapshotProvider">The snapshot provider.</param>
        /// <param name="snapshotStore">The store for snapshots.</param>
        public AggregateRootRepository(IEventStore store, IFormatter formatter, IAggregateRootFactory<T> factory, IEventDispatcher eventDispatcher, 
            ISnapshotProvider snapshotProvider, ISnapshotStore snapshotStore)
        {
            Ensure.NotNull(store, "store");
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(factory, "factory");
            Ensure.NotNull(eventDispatcher, "eventDispatcher");
            Ensure.NotNull(snapshotProvider, "snapshotProvider");
            Ensure.NotNull(snapshotStore, "snapshotStore");
            this.store = store;
            this.formatter = formatter;
            this.factory = factory;
            this.eventDispatcher = eventDispatcher;
            this.snapshotProvider = snapshotProvider;
            this.snapshotStore = snapshotStore;
        }

        public virtual void Save(T model)
        {
            Ensure.NotNull(model, "model");

            IEnumerable<IEvent> events = model.Events;
            if (events.Any())
            {
                // Serialize and save all new events.
                IEnumerable<EventModel> eventModels = events.Select(e => new EventModel(e.AggregateKey, e.Key, formatter.SerializeEvent(e), e.Version));
                store.Save(eventModels);

                // Try to create snapshot.
                ISnapshot snapshot;
                if (snapshotProvider.TryCreate(model, out snapshot))
                    snapshotStore.Save(snapshot);

                // Publish new events.
                foreach (IEvent e in events)
                    eventDispatcher.PublishAsync(e).Wait();
            }
        }

        public T Find(IKey key)
        {
            Ensure.Condition.NotEmptyKey(key);

            // Try to find snapshot.
            ISnapshot snapshot = snapshotStore.Find(key);

            // If snapshot exists, load only newer events; otherwise load all of them.
            IEnumerable<EventModel> eventModels = null;
            if (snapshot == null)
                eventModels = store.Get(key);
            else
                eventModels = store.Get(key, snapshot.Version);

            IEnumerable<object> events = eventModels.Select(e => formatter.DeserializeEvent(Type.GetType(e.EventKey.Type), e.Payload));

            // If snapshot exists, create instance with it and newer events; otherwise create instance using all events.
            T instance = null;
            if (snapshot == null)
                instance = factory.Create(key, events);
            else
                instance = factory.Create(key, snapshot, events);

            // Return the aggregate.
            return instance;
        }
    }
}
