using Money.Data;
using Neptuo.Activators;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Formatters;
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
        public void Initialize()
        {
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

            IRepository<Outcome, IKey> outcomRepository = new AggregateRootRepository<Outcome>(
                eventStore,
                new CompositeEventFormatter(typeProvider, compositeStorageFactory),
                new ReflectionAggregateRootFactory<Outcome>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );
        }
    }
}
