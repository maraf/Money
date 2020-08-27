using Neptuo;
using Neptuo.Activators;
using Neptuo.Bootstrap;
using Neptuo.Events;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Builders
{
    public class BootstrapTask : IBootstrapTask
    {
        private readonly IQueryHandlerCollection queryHandlers;
        private readonly IEventHandlerCollection eventHandlers;
        private readonly IFactory<ReadModelContext> contextFactory;
        private readonly IPriceConverter priceConverter;

        public BootstrapTask(IQueryHandlerCollection queryHandlers, IEventHandlerCollection eventHandlers, IFactory<ReadModelContext> contextFactory, IPriceConverter priceConverter)
        {
            Ensure.NotNull(queryHandlers, "queryHandlers");
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(contextFactory, "contextFactory");
            Ensure.NotNull(priceConverter, "priceConverter");
            this.queryHandlers = queryHandlers;
            this.eventHandlers = eventHandlers;
            this.contextFactory = contextFactory;
            this.priceConverter = priceConverter;
        }

        public void Initialize()
        {
            CategoryBuilder categoryBuilder = new CategoryBuilder(contextFactory);
            queryHandlers.AddAll(categoryBuilder);
            eventHandlers.AddAll(categoryBuilder);

            OutcomeBuilder outcomeBuilder = new OutcomeBuilder(contextFactory, priceConverter);
            queryHandlers.AddAll(outcomeBuilder);
            eventHandlers.AddAll(outcomeBuilder);

            CurrencyBuilder currencyBuilder = new CurrencyBuilder(contextFactory);
            queryHandlers.AddAll(currencyBuilder);
            eventHandlers.AddAll(currencyBuilder);
        }
    }
}
