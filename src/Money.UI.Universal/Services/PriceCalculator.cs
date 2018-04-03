using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public partial class PriceCalculator : DisposableBase,
        IPriceConverter,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencyExchangeRateSet>,
        IEventHandler<CurrencyExchangeRateRemoved>
    {
        private readonly ExchangeRateModelComparer exchangeRateComparer = new ExchangeRateModelComparer();
        private readonly Dictionary<string, List<ExchangeRateModel>> currencies = new Dictionary<string, List<ExchangeRateModel>>();
        private string defaultCurrencyUniqueCode;

        public PriceCalculator(IEventHandlerCollection eventHandlers)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            eventHandlers.AddAll(this);
        }

        public async Task InitializeAsync(IQueryDispatcher queryDispatcher)
        {
            defaultCurrencyUniqueCode = await queryDispatcher.QueryAsync(new GetCurrencyDefault());

            IEnumerable<CurrencyModel> currencies = await queryDispatcher.QueryAsync(new ListAllCurrency());
            foreach (CurrencyModel currency in currencies)
            {
                List<ExchangeRateModel> rates = await queryDispatcher.QueryAsync(new ListTargetCurrencyExchangeRates(currency.UniqueCode));
                rates.Sort(exchangeRateComparer);
                this.currencies[currency.UniqueCode] = rates;
            }
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            currencies.Add(payload.UniqueCode, new List<ExchangeRateModel>());
            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload)
        {
            List<ExchangeRateModel> exchangeRates = currencies[payload.TargetUniqueCode];
            exchangeRates.Add(new ExchangeRateModel(payload.SourceUniqueCode, payload.Rate, payload.ValidFrom));
            exchangeRates.Sort(exchangeRateComparer);
            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyExchangeRateRemoved>.HandleAsync(CurrencyExchangeRateRemoved payload)
        {
            List<ExchangeRateModel> exchangeRates = currencies[payload.TargetUniqueCode];
            ExchangeRateModel exchangeRate = exchangeRates.FirstOrDefault(r => r.SourceCurrency == payload.SourceUniqueCode && r.Rate == payload.Rate && r.ValidFrom == payload.ValidFrom);
            if (exchangeRate != null)
            {
                exchangeRates.Remove(exchangeRate);
                exchangeRates.Sort(exchangeRateComparer);
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            defaultCurrencyUniqueCode = payload.UniqueCode;
            return Task.CompletedTask;
        }

        // TODO: Single user mode for now.
        public Price ZeroDefault(IKey userKey)
        {
            return Price.Zero(defaultCurrencyUniqueCode);
        }

        // TODO: Single user mode for now.
        public Price ToDefault(IKey userKey, IPriceFixed price)
        {
            Ensure.NotNull(price, "price");

            if (price.Amount.Currency == defaultCurrencyUniqueCode)
                return price.Amount;

            if (currencies.TryGetValue(defaultCurrencyUniqueCode, out List<ExchangeRateModel> rates))
            {
                ExchangeRateModel rate = rates.FirstOrDefault(e => e.SourceCurrency == price.Amount.Currency && e.ValidFrom <= price.When);
                if (rate != null)
                    return new Price(price.Amount.Value * (decimal)rate.Rate, defaultCurrencyUniqueCode);
            }

            return new Price(price.Amount.Value, defaultCurrencyUniqueCode);
        }
    }
}
