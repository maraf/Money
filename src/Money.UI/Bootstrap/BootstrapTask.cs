using Money.Data;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Formatters;
using Neptuo.Formatters.Converters;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using Neptuo.Models.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Money.Bootstrap
{
    public class BootstrapTask
    {
        public IFactory<Price, decimal> PriceFactory { get; private set; }
        public IRepository<Outcome, IKey> OutcomeRepository { get; private set; }

        public void Initialize()
        {
            Converts.Repository
                .AddJsonEnumSearchHandler()
                .AddJsonPrimitivesSearchHandler()
                .AddJsonObjectSearchHandler()
                .AddJsonKey()
                .AddJsonTimeSpan();

            ApplicationDataContainer root = ApplicationData.Current.LocalSettings;
            ApplicationDataContainer eventStoreContainer = root
                .CreateContainer("EventStore", ApplicationDataCreateDisposition.Always);

            ContainerEventStore eventStore = new ContainerEventStore(eventStoreContainer);

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Default<JsonCompositeStorage>();

            ICompositeTypeProvider typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            IEventDispatcher eventDispatcher = new PersistentEventDispatcher(
                new EmptyEventStore()
            );

            OutcomeRepository = new AggregateRootRepository<Outcome>(
                eventStore,
                new CompositeEventFormatter(typeProvider, compositeStorageFactory),
                new ReflectionAggregateRootFactory<Outcome>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            PriceFactory = new PriceFactory("CZK");
        }
    }
}
