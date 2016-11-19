using Neptuo;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Formatters;
using Neptuo.Internals;
using Neptuo.Models.Keys;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.ReadModels
{
    /// <summary>
    /// Service for rebuilding read-model from existing store.
    /// </summary>
    public class Rebuilder : IEventHandlerCollection
    {
        private readonly PersistentEventDispatcher eventDispatcher;
        private readonly IEventRebuilderStore store;
        private readonly IDeserializer deserializer;
        private readonly Dictionary<IKey, int> aggregateVersionFilter = new Dictionary<IKey, int>();

        /// <summary>
        /// Creates new instance for rebuilding events from <paramref name="store"/> using <paramref name="deserializer"/>
        /// for loading event instances.
        /// </summary>
        /// <param name="store">The store containing already published events.</param>
        /// <param name="deserializer">The deserializer from loading event instances.</param>
        public Rebuilder(IEventRebuilderStore store, IDeserializer deserializer)
        {
            Ensure.NotNull(store, "store");
            Ensure.NotNull(deserializer, "deserializer");
            this.eventDispatcher = new PersistentEventDispatcher(new EmptyEventStore());
            this.store = store;
            this.deserializer = deserializer;
        }

        /// <summary>
        /// Adds filter for events from aggregate of key <paramref name="aggregateKey"/> to be
        /// only of version <paramref name="minimalVersion"/> and higher.
        /// </summary>
        /// <param name="aggregateKey">The key of the owner of events to apply filter on.</param>
        /// <param name="minimalVersion">The required minimal version.</param>
        /// <returns>Self (for fluency).</returns>
        public Rebuilder AddAggregateVersionFilter(IKey aggregateKey, int minimalVersion)
        {
            Ensure.Condition.NotEmptyKey(aggregateKey);
            Ensure.Positive(minimalVersion, "minimalVersion");
            aggregateVersionFilter[aggregateKey] = minimalVersion;
            return this;
        }

        /// <summary>
        /// Removes filter for events from aggregate of key <paramref name="aggregateKey"/>.
        /// </summary>
        /// <param name="aggregateKey">The key of the aggregate to remove filter for.</param>
        /// <returns>Self (for fluency).</returns>
        public Rebuilder RemoveAggregateVersionFilter(IKey aggregateKey)
        {
            Ensure.Condition.NotEmptyKey(aggregateKey);
            if (aggregateVersionFilter.ContainsKey(aggregateKey))
                aggregateVersionFilter.Remove(aggregateKey);

            return this;
        }

        /// <summary>
        /// Registers events of type <typeparamref name="TEvent"/> to be loaded and passed to the <paramref name="handler"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to reload.</typeparam>
        /// <param name="handler">The handler to handle events of type <typeparamref name="TEvent"/>.</param>
        /// <returns>Self (for fluency).</returns>
        public Rebuilder Add<TEvent>(IEventHandler<TEvent> handler)
        {
            eventDispatcher.Handlers.Add(handler);
            return this;
        }

        IEventHandlerCollection IEventHandlerCollection.Add<TEvent>(IEventHandler<TEvent> handler)
        {
            return Add(handler);
        }

        /// <summary>
        /// Unregisters <paramref name="handler"/> to be a handler of events of type <typeparamref name="TEvent"/>.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to remove from reloading process.</typeparam>
        /// <param name="handler">The handler to unsubscribe from events of type <typeparamref name="TEvent"/>.</param>
        /// <returns>Self (for fluency).</returns>
        public Rebuilder Remove<TEvent>(IEventHandler<TEvent> handler)
        {
            eventDispatcher.Handlers.Remove(handler);
            return this;
        }

        IEventHandlerCollection IEventHandlerCollection.Remove<TEvent>(IEventHandler<TEvent> handler)
        {
            return Remove(handler);
        }

        /// <summary>
        /// Pushlishes all events on the registered handlers using <see cref="Add"/>.
        /// </summary>
        /// <returns>The continuation task.</returns>
        public async Task RunAsync()
        {
            // Use list of required event types to load events from store.
            IEnumerable<string> eventTypes = eventDispatcher
                .EnumerateEventTypes()
                .Select(t => t.AssemblyQualifiedName);

            IEnumerable<EventModel> eventData = await store.GetAsync(eventTypes);
            IEnumerable<IEvent> events = eventData
                .Where(IsFilterPassed)
                .Select(e => deserializer.DeserializeEvent(Type.GetType(e.EventKey.Type), e.Payload));

            // Replay events on handler(s).
            foreach (IEvent payload in events)
                await eventDispatcher.PublishAsync(payload);
        }

        private bool IsFilterPassed(EventModel e)
        {
            int minimalVersion;
            if (aggregateVersionFilter.TryGetValue(e.AggregateKey, out minimalVersion))
                return e.Version >= minimalVersion;

            return true;
        }
    }
}
