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
        private readonly IFactory<ReadModelContext> dbFactory;
        private readonly IPriceConverter priceConverter;

        public BootstrapTask(IQueryHandlerCollection queryHandlers, IEventHandlerCollection eventHandlers, IFactory<ReadModelContext> dbFactory, IPriceConverter priceConverter)
        {
            Ensure.NotNull(queryHandlers, "queryHandlers");
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(dbFactory, "dbFactory");
            Ensure.NotNull(priceConverter, "priceConverter");
            this.queryHandlers = queryHandlers;
            this.eventHandlers = eventHandlers;
            this.dbFactory = dbFactory;
            this.priceConverter = priceConverter;
        }

        public void Initialize()
        {
            var categoryBuilder = new CategoryBuilder(dbFactory);
            queryHandlers.AddAll(categoryBuilder);
            eventHandlers.AddAll(categoryBuilder);

            var outcomeBuilder = new OutcomeBuilder(dbFactory, priceConverter);
            queryHandlers.AddAll(outcomeBuilder);
            eventHandlers.AddAll(outcomeBuilder);

            var expenseTemplateBuilder = new ExpenseTemplateBuilder(dbFactory);
            queryHandlers.AddAll(expenseTemplateBuilder);
            eventHandlers.AddAll(expenseTemplateBuilder);

            var incomeBuilder = new IncomeBuilder(dbFactory, priceConverter);
            queryHandlers.AddAll(incomeBuilder);
            eventHandlers.AddAll(incomeBuilder);

            var currencyBuilder = new CurrencyBuilder(dbFactory);
            queryHandlers.AddAll(currencyBuilder);
            eventHandlers.AddAll(currencyBuilder);
        }
    }
}
