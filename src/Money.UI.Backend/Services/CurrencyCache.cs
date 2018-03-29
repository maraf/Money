using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
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
        private readonly Dictionary<string, Model> storage = new Dictionary<string, Model>();

        public CurrencyCache(IEventHandlerCollection eventHandlers, IQueryHandlerCollection queryHandlers)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(queryHandlers, "queryHandlers");
            eventHandlers.AddAll(this);
            queryHandlers.AddAll(this);
        }

        public async Task InitializeAsync(IQueryDispatcher queryDispatcher)
        {
            List<CurrencyModel> models = await queryDispatcher.QueryAsync(new ListAllCurrency());
            foreach (CurrencyModel model in models)
            {
                storage[model.UniqueCode] = new Model()
                {
                    UniqueCode = model.UniqueCode,
                    Symbol = model.Symbol
                };
            }
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            storage[payload.UniqueCode] = new Model()
            {
                UniqueCode = payload.UniqueCode,
                Symbol = payload.Symbol
            };

            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload)
        {
            if (storage.TryGetValue(payload.UniqueCode, out Model model))
                model.Symbol = payload.Symbol;

            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload)
        {
            if (storage.ContainsKey(payload.UniqueCode))
                storage.Remove(payload.UniqueCode);

            return Task.CompletedTask;
        }

        Task<string> IQueryHandler<GetCurrencySymbol, string>.HandleAsync(GetCurrencySymbol query)
        {
            if (storage.TryGetValue(query.UniqueCode, out Model model))
                return Task.FromResult(model.Symbol);

            throw new CurrencyDoesNotExistException();
        }
    }
}
