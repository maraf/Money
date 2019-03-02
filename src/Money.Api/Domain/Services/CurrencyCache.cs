using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal partial class CurrencyCache : IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyDeleted>,
        IQueryHandler<GetCurrencySymbol, string>
    {
        private readonly Dictionary<IKey, Dictionary<string, Model>> storage = new Dictionary<IKey, Dictionary<string, Model>>();
        private readonly IQueryDispatcher queryDispatcher;

        public CurrencyCache(IEventHandlerCollection eventHandlers, IQueryHandlerCollection queryHandlers, IQueryDispatcher queryDispatcher)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(queryHandlers, "queryHandlers");
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            eventHandlers.AddAll(this);
            queryHandlers.AddAll(this);
            this.queryDispatcher = queryDispatcher;
        }

        private async Task EnsureUserStorageAsync(IKey userKey)
        {
            if (!this.storage.TryGetValue(userKey, out Dictionary<string, Model> storage))
            {
                this.storage[userKey] = storage = new Dictionary<string, Model>();

                List<CurrencyModel> models = await queryDispatcher.QueryAsync(new ListAllCurrency() { UserKey = userKey });
                foreach (CurrencyModel model in models)
                {
                    storage[model.UniqueCode] = new Model()
                    {
                        UniqueCode = model.UniqueCode,
                        Symbol = model.Symbol
                    };
                }
            }
        }

        async Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            await EnsureUserStorageAsync(payload.UserKey);
            storage[payload.UserKey][payload.UniqueCode] = new Model()
            {
                UniqueCode = payload.UniqueCode,
                Symbol = payload.Symbol
            };
        }

        async Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload)
        {
            await EnsureUserStorageAsync(payload.UserKey);
            if (storage[payload.UserKey].TryGetValue(payload.UniqueCode, out Model model))
                model.Symbol = payload.Symbol;
        }

        async Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload)
        {
            await EnsureUserStorageAsync(payload.UserKey);
            if (storage[payload.UserKey].ContainsKey(payload.UniqueCode))
                storage[payload.UserKey].Remove(payload.UniqueCode);
        }

        async Task<string> IQueryHandler<GetCurrencySymbol, string>.HandleAsync(GetCurrencySymbol query)
        {
            await EnsureUserStorageAsync(query.UserKey);
            if (storage[query.UserKey].TryGetValue(query.UniqueCode, out Model model))
                return model.Symbol;

            throw new CurrencyDoesNotExistException();
        }
    }
}
