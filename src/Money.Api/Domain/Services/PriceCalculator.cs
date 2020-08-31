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
using System.Collections.Concurrent;
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
        private readonly IQueryDispatcher queryDispatcher;
        private readonly ConcurrentDictionary<IKey, UserModel> storage = new ConcurrentDictionary<IKey, UserModel>();

        public PriceCalculator(IEventHandlerCollection eventHandlers, IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            eventHandlers.AddAll(this);
            this.queryDispatcher = queryDispatcher;
        }

        private async Task EnsureUserStorageAsync(IKey userKey)
        {
            if (!storage.TryGetValue(userKey, out UserModel model))
            {
                storage[userKey] = model = new UserModel();

                model.DefaultCurrencyUniqueCode = await queryDispatcher.QueryAsync(new FindCurrencyDefault() { UserKey = userKey });

                IEnumerable<CurrencyModel> currencies = await queryDispatcher.QueryAsync(new ListAllCurrency() { UserKey = userKey });
                foreach (CurrencyModel currency in currencies)
                {
                    List<ExchangeRateModel> rates = await queryDispatcher.QueryAsync(new ListTargetCurrencyExchangeRates(currency.UniqueCode) { UserKey = userKey });
                    rates.Sort(exchangeRateComparer);
                    model.Currencies[currency.UniqueCode] = rates;
                }
            }
        }

        async Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            await EnsureUserStorageAsync(payload.UserKey);
            storage[payload.UserKey].Currencies.TryAdd(payload.UniqueCode, new List<ExchangeRateModel>());
        }

        async Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload)
        {
            await EnsureUserStorageAsync(payload.UserKey);
            List<ExchangeRateModel> exchangeRates = storage[payload.UserKey].Currencies[payload.TargetUniqueCode];
            exchangeRates.Add(new ExchangeRateModel(payload.SourceUniqueCode, payload.Rate, payload.ValidFrom));
            exchangeRates.Sort(exchangeRateComparer);
        }

        async Task IEventHandler<CurrencyExchangeRateRemoved>.HandleAsync(CurrencyExchangeRateRemoved payload)
        {
            await EnsureUserStorageAsync(payload.UserKey);
            List<ExchangeRateModel> exchangeRates = storage[payload.UserKey].Currencies[payload.TargetUniqueCode];
            ExchangeRateModel exchangeRate = exchangeRates.FirstOrDefault(r => r.SourceCurrency == payload.SourceUniqueCode && r.Rate == payload.Rate && r.ValidFrom == payload.ValidFrom);
            if (exchangeRate != null)
            {
                exchangeRates.Remove(exchangeRate);
                exchangeRates.Sort(exchangeRateComparer);
            }
        }

        async Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            await EnsureUserStorageAsync(payload.UserKey);
            storage[payload.UserKey].DefaultCurrencyUniqueCode = payload.UniqueCode;
        }

        public Price ZeroDefault(IKey userKey)
        {
            EnsureUserStorageAsync(userKey).Wait();
            string defaultCurrenyUniqueCode = storage[userKey].DefaultCurrencyUniqueCode;
            if (defaultCurrenyUniqueCode == null)
                throw new MissingDefaultCurrentException();

            return Price.Zero(defaultCurrenyUniqueCode);
        }

        public Price ToDefault(IKey userKey, IPriceFixed price)
        {
            Ensure.NotNull(price, "price");

            EnsureUserStorageAsync(userKey).Wait();

            string defaultCurrencyUniqueCode = storage[userKey].DefaultCurrencyUniqueCode;

            if (price.Amount.Currency == defaultCurrencyUniqueCode)
                return price.Amount;

            if (storage[userKey].Currencies.TryGetValue(defaultCurrencyUniqueCode, out List<ExchangeRateModel> rates))
            {
                ExchangeRateModel rate = rates.FirstOrDefault(e => e.SourceCurrency == price.Amount.Currency && e.ValidFrom <= price.When);
                if (rate != null)
                    return new Price(price.Amount.Value * (decimal)rate.Rate, defaultCurrencyUniqueCode);
            }

            return new Price(price.Amount.Value, defaultCurrencyUniqueCode);
        }
    }
}
