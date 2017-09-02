using Money.Events;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class PriceCalculator : DisposableBase,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyExchangeRateSet>
    {
        private readonly IEventHandlerCollection eventHandlers;
        private readonly ExchangeRateModelComparer exchangeRateComparer = new ExchangeRateModelComparer();
        private readonly Dictionary<string, List<ExchangeRateModel>> currencies = new Dictionary<string, List<ExchangeRateModel>>();

        public PriceCalculator(IEventHandlerCollection eventHandlers)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            this.eventHandlers = eventHandlers;
        }

        public async Task Initialize(IQueryDispatcher queryDispatcher)
        {
            IEnumerable<string> currencies = await queryDispatcher.QueryAsync(new ListAllCurrency());
            foreach (string currency in currencies)
            {
                List<ExchangeRateModel> rates = await queryDispatcher.QueryAsync(new ListTargetCurrencyExchangeRates(currency));
                rates.Sort(exchangeRateComparer);
                this.currencies[currency] = rates;
            }
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            currencies.Add(payload.UniqueCode, new List<ExchangeRateModel>());
            return Async.CompletedTask;
        }

        Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload)
        {
            List<ExchangeRateModel> exchangeRates = currencies[payload.TargetUniqueCode];
            exchangeRates.Add(new ExchangeRateModel(payload.SourceUniqueCode, payload.Rate, payload.ValidFrom));
            exchangeRates.Sort(exchangeRateComparer);
            return Async.CompletedTask;
        }

        private class ExchangeRateModelComparer : IComparer<ExchangeRateModel>
        {
            public int Compare(ExchangeRateModel x, ExchangeRateModel y)
            {
                return y.ValidFrom.CompareTo(x.ValidFrom);
            }
        }
    }
}
