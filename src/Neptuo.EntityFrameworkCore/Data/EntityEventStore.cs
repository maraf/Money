using Microsoft.EntityFrameworkCore;
using Neptuo.Activators;
using Neptuo.Data;
using Neptuo.Data.Entity;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Data
{
    public class EntityEventStore : IEventStore, IEventPublishingStore, IEventRebuilderStore
    {
        private readonly Func<IEventContext> contextFactory;

        public EntityEventStore(IEventContext context)
        {
            Ensure.NotNull(context, "context");
            this.contextFactory = () => context;
        }

        public EntityEventStore(IFactory<IEventContext> contextFactory)
        {
            Ensure.NotNull(contextFactory, "contextFactory");
            this.contextFactory = contextFactory.Create;
        }

        public IEnumerable<EventModel> Get(IKey aggregateKey)
        {
            return Get(aggregateKey, 0);
        }

        public IEnumerable<EventModel> Get(IKey aggregateKey, int version)
        {
            Ensure.Condition.NotEmptyKey(aggregateKey);

            GuidKey key = aggregateKey as GuidKey;
            if (key == null)
                throw Ensure.Exception.NotGuidKey(aggregateKey.GetType(), "aggregateKey");

            IEnumerable<EventEntity> entities = contextFactory().Events
                .Where(e => e.AggregateType == key.Type && e.AggregateID == key.Guid && e.Version > version)
                .OrderBy(e => e.Version);

            return entities.Select(e => e.ToModel());
        }

        public void Save(IEnumerable<EventModel> events)
        {
            Ensure.NotNull(events, "events");

            IEventContext context = contextFactory();
            foreach (EventEntity entity in events.Select(EventEntity.FromModel))
            {
                context.Events.Add(entity);
                context.UnPublishedEvents.Add(new UnPublishedEventEntity(entity));
            }

            context.Save();
        }

        public async Task<IEnumerable<EventPublishingModel>> GetAsync()
        {
            return await contextFactory().UnPublishedEvents
                .Select(e => new EventPublishingModel(e.Event.ToModel(), e.PublishedToHandlers.Select(h => h.HandlerIdentifier)))
                .ToListAsync();
        }

        public Task PublishedAsync(IKey key, string handlerIdentifier)
        {
            GuidKey eventKey = key as GuidKey;
            if (eventKey == null)
                throw Ensure.Exception.NotGuidKey(eventKey.GetType(), "key");

            IEventContext context = contextFactory();
            UnPublishedEventEntity entity = context.UnPublishedEvents.FirstOrDefault(e => e.Event.EventType == eventKey.Type && e.Event.EventID == eventKey.Guid);
            if (entity == null)
                return Task.FromResult(true);

            entity.PublishedToHandlers.Add(new EventPublishedToHandlerEntity(handlerIdentifier));
            return context.SaveAsync();
        }

        public Task ClearAsync()
        {
            IEventContext context = contextFactory();
            foreach (UnPublishedEventEntity entity in context.UnPublishedEvents)
                context.UnPublishedEvents.Remove(entity);

            return context.SaveAsync();
        }

        public async Task<IEnumerable<EventModel>> GetAsync(IEnumerable<string> eventTypes)
        {
            return await contextFactory().Events
                .Where(e => eventTypes.Contains(e.EventType))
                .OrderBy(e => e.ID)
                .Select(e => e.ToModel())
                .ToListAsync();
        }
    }
}
