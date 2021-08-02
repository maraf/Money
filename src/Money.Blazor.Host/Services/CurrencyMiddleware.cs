using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal class CurrencyMiddleware : HttpQueryDispatcher.IMiddleware,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyDeleted>,
        IEventHandler<UserSignedOut>
    {
        private static readonly ListAllCurrency listAllQuery = new ListAllCurrency();

        private readonly ServerConnectionState serverConnection;
        private readonly CurrencyStorage localStorage;

        private readonly List<CurrencyModel> models = new List<CurrencyModel>();
        private Task listAllTask;

        public CurrencyMiddleware(ServerConnectionState serverConnection, CurrencyStorage localStorage)
        {
            Ensure.NotNull(serverConnection, "serverConnection");
            Ensure.NotNull(localStorage, "localStorage");
            this.serverConnection = serverConnection;
            this.localStorage = localStorage;
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is ListAllCurrency listAll)
            {
                await EnsureListAsync(null, next, listAll);
                return models.Select(c => c.Clone()).ToList();
            }
            else if (query is GetCurrencyDefault currencyDefault)
            {
                await EnsureListAsync(dispatcher, null, listAllQuery);
                CurrencyModel model = models.FirstOrDefault(c => c.IsDefault);
                if (model != null)
                    return model.UniqueCode;
            }
            else if (query is FindCurrencyDefault currencyDefaultNullable)
            {
                await EnsureListAsync(dispatcher, null, listAllQuery);
                CurrencyModel model = models.FirstOrDefault(c => c.IsDefault);
                if (model != null)
                    return model.UniqueCode;
            }
            else if (query is GetCurrencySymbol currencySymbol)
            {
                await EnsureListAsync(dispatcher, null, listAllQuery);
                CurrencyModel model = Find(currencySymbol.UniqueCode);
                if (model != null)
                    return model.Symbol;
            }

            return await next(query);
        }

        private async Task EnsureListAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListAllCurrency listAll)
        {
            if (models.Count == 0)
            {
                if (listAllTask == null)
                    listAllTask = LoadAllAsync(dispatcher, next, listAll);

                try
                {
                    await listAllTask;
                }
                finally
                {
                    listAllTask = null;
                }
            }
        }

        private async Task LoadAllAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListAllCurrency listAll)
        {
            models.Clear();
            if (!serverConnection.IsAvailable)
            {
                var items = await localStorage.LoadAsync();
                if (items != null)
                {
                    models.AddRange(items);
                    return;
                }
            }

            if (dispatcher != null)
            {
                await dispatcher.QueryAsync(listAll);
            }
            else
            {
                models.AddRange((List<CurrencyModel>)await next(listAll));
                await localStorage.SaveAsync(models);
            }
        }

        private CurrencyModel Find(string uniqueCode)
            => models.FirstOrDefault(c => c.UniqueCode.Equals(uniqueCode));

        private async Task Update(string uniqueCode, Action<CurrencyModel> handler)
        {
            CurrencyModel model = Find(uniqueCode);
            if (model != null)
                handler(model);

            await localStorage.SaveAsync(models);
        }

        async Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            models.Add(new CurrencyModel(payload.UniqueCode, payload.Symbol, false));
            await localStorage.SaveAsync(models);
        }

        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            foreach (CurrencyModel model in models)
                model.IsDefault = false;

            return Update(payload.UniqueCode, model => model.IsDefault = true);
        }

        Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload)
            => Update(payload.UniqueCode, model => model.Symbol = payload.Symbol);

        Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload)
            => Update(payload.UniqueCode, model => models.Remove(model));

        async Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            models.Clear();
            await localStorage.DeleteAsync();
        }
    }
}
