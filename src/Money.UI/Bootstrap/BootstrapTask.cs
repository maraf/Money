using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Services;
using Money.Services.Models.Builders;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Converters;
using Neptuo.Data;
using Neptuo.Events;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters;
using Neptuo.Formatters.Converters;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using Neptuo.Models.Repositories;
using Neptuo.Models.Snapshots;
using Neptuo.Queries;
using Neptuo.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace Money.Bootstrap
{
    public class BootstrapTask : IExceptionHandler
    {
        private PersistentEventDispatcher eventDispatcher;

        public IFactory<Price, decimal> PriceFactory { get; private set; }
        public IRepository<Outcome, IKey> OutcomeRepository { get; private set; }
        public IRepository<Category, IKey> CategoryRepository { get; private set; }
        public IDomainFacade DomainFacade { get; private set; }

        public EntityEventStore EventStore { get; private set; }

        public IFormatter EventFormatter { get; private set; }

        public IQueryDispatcher QueryDispatcher { get; private set; }

        public void Initialize()
        {
            Domain();
            ReadModels();

            ServiceProvider.QueryDispatcher = QueryDispatcher;
            ServiceProvider.DomainFacade = DomainFacade;
            ServiceProvider.UpgradeService = new UpgradeService(DomainFacade, EventStore, EventFormatter);
        }
        
        private void Domain()
        {
            Converts.Repository
                .AddJsonEnumSearchHandler()
                .AddJsonPrimitivesSearchHandler()
                .AddJsonObjectSearchHandler()
                .AddJsonKey()
                .AddJsonTimeSpan()
                .Add(new ColorConverter());
            
            EventStore = new EntityEventStore(Factory.Default<EventSourcingContext>());
            eventDispatcher = new PersistentEventDispatcher(new EmptyEventStore());
            eventDispatcher.DispatcherExceptionHandlers.Add(this);
            eventDispatcher.EventExceptionHandlers.Add(this);

            IFactory<ICompositeStorage> compositeStorageFactory = Factory.Default<JsonCompositeStorage>();

            ICompositeTypeProvider typeProvider = new ReflectionCompositeTypeProvider(
                new ReflectionCompositeDelegateFactory(),
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            EventFormatter = new CompositeEventFormatter(typeProvider, compositeStorageFactory);

            OutcomeRepository = new AggregateRootRepository<Outcome>(
                EventStore,
                EventFormatter,
                new ReflectionAggregateRootFactory<Outcome>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            CategoryRepository = new AggregateRootRepository<Category>(
                EventStore,
                EventFormatter,
                new ReflectionAggregateRootFactory<Category>(),
                eventDispatcher,
                new NoSnapshotProvider(),
                new EmptySnapshotStore()
            );

            PriceFactory = new PriceFactory("CZK");
            DomainFacade = new DefaultDomainFacade(
                OutcomeRepository,
                CategoryRepository,
                PriceFactory
            );
        }

        private void ReadModels()
        {
            // Should match with RecreateReadModelContext.
            DefaultQueryDispatcher queryDispatcher = new DefaultQueryDispatcher();
            QueryDispatcher = queryDispatcher;

            CategoryBuilder categoryBuilder = new CategoryBuilder();
            queryDispatcher.AddAll(categoryBuilder);
            eventDispatcher.Handlers.AddAll(categoryBuilder);

            OutcomeBuilder outcomeBuilder = new OutcomeBuilder(PriceFactory);
            queryDispatcher.AddAll(outcomeBuilder);
            eventDispatcher.Handlers.AddAll(outcomeBuilder);
        }

        public void Handle(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
