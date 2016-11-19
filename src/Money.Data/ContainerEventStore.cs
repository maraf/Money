using Money;
using Neptuo;
using Neptuo.Data;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money.Data
{
    /// <summary>
    /// An implementation of <see cref="IEventStore"/> using <see cref="ApplicationDataContainer"/>.
    /// </summary>
    public class ContainerEventStore : IEventStore, IEventRebuilderStore
    {
        private readonly ApplicationDataContainer container;

        /// <summary>
        /// Creates new instance with <paramref name="container"/> as a root for storing events.
        /// </summary>
        /// <param name="container">The container where aggregate events are stored.</param>
        public ContainerEventStore(ApplicationDataContainer container)
        {
            Ensure.NotNull(container, "container");
            this.container = container;
        }

        public IEnumerable<EventModel> Get(IKey aggregateKey)
        {
            return Get(aggregateKey, 0);
        }

        public IEnumerable<EventModel> Get(IKey aggregateKey, int minVersion)
        {
            string rawKey = aggregateKey.AsGuidKey().Guid.ToString();

            ApplicationDataContainer aggregateContainer;
            if (container.Containers.TryGetValue(rawKey, out aggregateContainer))
            {
                List<EventModel> result = new List<EventModel>();
                foreach (ApplicationDataContainer eventContainer in aggregateContainer.Containers.Values)
                {
                    int version = (int)eventContainer.Values["Version"];
                    if (version > minVersion)
                    {
                        GuidKey eventKey = GuidKey.Create(Guid.Parse((string)eventContainer.Values["Guid"]), (string)eventContainer.Values["Type"]);
                        string payload = (string)eventContainer.Values["Payload"];

                        result.Add(new EventModel(
                            aggregateKey,
                            eventKey,
                            payload,
                            version
                        ));
                    }
                }

                return result;
            }

            return Enumerable.Empty<EventModel>();
        }

        public Task<IEnumerable<EventModel>> GetAsync(IEnumerable<string> eventTypes)
        {
            List<EventModel> result = new List<EventModel>();
            foreach (ApplicationDataContainer aggregateContainer in container.Containers.Values)
            {
                foreach (ApplicationDataContainer eventContainer in aggregateContainer.Containers.Values)
                {
                    int version = (int)eventContainer.Values["Version"];
                    string eventType = (string)eventContainer.Values["Type"];

                    if (eventTypes.Contains(eventType))
                    {
                        GuidKey aggregateKey = GuidKey.Create(
                            Guid.Parse(aggregateContainer.Name),
                            (string)eventContainer.Values["AggregateType"] ?? typeof(Outcome).FullName
                        );

                        GuidKey eventKey = GuidKey.Create(
                            Guid.Parse((string)eventContainer.Values["Guid"]),
                            eventType
                        );

                        string payload = (string)eventContainer.Values["Payload"];

                        result.Add(new EventModel(
                            aggregateKey,
                            eventKey,
                            payload,
                            version
                        ));
                    }
                }
            }

            return Task.FromResult<IEnumerable<EventModel>>(result);
        }

        public void Save(IEnumerable<EventModel> events)
        {
            foreach (EventModel model in events)
            {
                string rawAggregateKey = model.AggregateKey.AsGuidKey().Guid.ToString();

                ApplicationDataContainer aggregateContainer;
                if (!container.Containers.TryGetValue(rawAggregateKey, out aggregateContainer))
                    aggregateContainer = container.CreateContainer(rawAggregateKey, ApplicationDataCreateDisposition.Always);

                string rawEventKey = model.EventKey.AsGuidKey().Guid.ToString();

                ApplicationDataContainer eventContainer;
                eventContainer = aggregateContainer.CreateContainer(rawEventKey, ApplicationDataCreateDisposition.Always);
                eventContainer.Values["Guid"] = rawEventKey;
                eventContainer.Values["AggregateType"] = model.AggregateKey.Type;
                eventContainer.Values["Type"] = model.EventKey.Type;
                eventContainer.Values["Payload"] = model.Payload;
                eventContainer.Values["Version"] = model.Version;
            }
        }
    }
}
