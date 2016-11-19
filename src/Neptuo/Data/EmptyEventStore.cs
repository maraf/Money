using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo.Models.Keys;
using Neptuo.Threading.Tasks;
using Neptuo;

namespace Neptuo.Data
{
    /// <summary>
    /// The empty implementation of <see cref="IEventStore"/> and <see cref="IEventPublishingStore"/>.
    /// </summary>
    public class EmptyEventStore : IEventStore, IEventPublishingStore, IEventRebuilderStore
    {
        private readonly Method supportedMethod;
        private readonly IEventStore store;
        private readonly IEventPublishingStore publishingStore;
        private readonly IEventRebuilderStore rebuilderStore;

        /// <summary>
        /// Creates new instance where all methods are empty.
        /// </summary>
        public EmptyEventStore()
        { }

        /// <summary>
        /// Creates new instance with enumeration of supported methods in <paramref name="supportedMethod"/>. Other methods are empty.
        /// If one of stores required by <paramref name="supportedMethod"/> is <c>null</c>, this method is automatically not supported.
        /// </summary>
        /// <param name="supportedMethod">The enumeration of supported methods.</param>
        /// <param name="store">The store for loading and saving events.</param>
        /// <param name="publishingStore">The store for saving delivery information.</param>
        /// <param name="rebuilderStore">The store for rebuilding read-models.</param>
        public EmptyEventStore(Method supportedMethod, IEventStore store, IEventPublishingStore publishingStore, IEventRebuilderStore rebuilderStore)
        {
            this.supportedMethod = supportedMethod;
            this.store = store;
            this.publishingStore = publishingStore;
            this.rebuilderStore = rebuilderStore;

            if (store == null)
            {
                supportedMethod &= Method.Get;
                supportedMethod &= Method.GetWithVersion;
                supportedMethod &= Method.Save;
            }

            if (publishingStore == null)
            {
                supportedMethod &= Method.GetUnpublished;
                supportedMethod &= Method.Clear;
                supportedMethod &= Method.Publish;
            }

            if (rebuilderStore == null)
                supportedMethod &= Method.GetOfTypes;
        }

        public IEnumerable<EventModel> Get(IKey aggregateKey)
        {
            if ((supportedMethod & Method.Get) == Method.Get)
                return store.Get(aggregateKey);

            return Enumerable.Empty<EventModel>();
        }

        public IEnumerable<EventModel> Get(IKey aggregateKey, int version)
        {
            if ((supportedMethod & Method.GetWithVersion) == Method.GetWithVersion)
                return store.Get(aggregateKey, version);

            return Enumerable.Empty<EventModel>();
        }

        public void Save(IEnumerable<EventModel> events)
        {
            if ((supportedMethod & Method.GetWithVersion) == Method.GetWithVersion)
                store.Save(events);
        }

        public Task ClearAsync()
        {
            if ((supportedMethod & Method.Clear) == Method.Clear)
                return publishingStore.ClearAsync();

            return Async.CompletedTask;
        }

        public Task<IEnumerable<EventPublishingModel>> GetAsync()
        {
            if ((supportedMethod & Method.GetUnpublished) == Method.GetUnpublished)
                return publishingStore.GetAsync();

            return Task.FromResult(Enumerable.Empty<EventPublishingModel>());
        }

        public Task PublishedAsync(IKey eventKey, string handlerIdentifier)
        {
            if ((supportedMethod & Method.Publish) == Method.Publish)
                return publishingStore.PublishedAsync(eventKey, handlerIdentifier);

            return Async.CompletedTask;
        }

        public Task<IEnumerable<EventModel>> GetAsync(IEnumerable<string> eventTypes)
        {
            if ((supportedMethod & Method.GetOfTypes) == Method.GetOfTypes)
                return rebuilderStore.GetAsync(eventTypes);

            return Task.FromResult(Enumerable.Empty<EventModel>());
        }

        /// <summary>
        /// The enumeration of supported methods.
        /// </summary>
        [Flags]
        public enum Method
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,

            /// <summary>
            /// <see cref="IEventStore.Get(IKey)"/>.
            /// </summary>
            Get = 1,

            /// <summary>
            /// <see cref="IEventStore.Get(IKey, int)"/>.
            /// </summary>
            GetWithVersion,

            /// <summary>
            /// <see cref="IEventStore.Save(IEnumerable{EventModel})"/>
            /// </summary>
            Save = 2,

            /// <summary>
            /// <see cref="IEventPublishingStore.GetAsync"/>.
            /// </summary>
            GetUnpublished = 4,

            /// <summary>
            /// <see cref="IEventPublishingStore.ClearAsync"/>.
            /// </summary>
            Clear = 8,

            /// <summary>
            /// <see cref="IEventPublishingStore.PublishedAsync(IKey, string)"/>.
            /// </summary>
            Publish = 16,

            /// <summary>
            /// <see cref="IEventRebuilderStore.GetAsync(IEnumerable{string})"/>.
            /// </summary>
            GetOfTypes = 32
        }
    }
}
