using Money.Events;
using Money.Models;
using Money.Models.Queries;
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
        IEventHandler<CurrencyDeleted>
    {
        private readonly List<CurrencyModel> models = new List<CurrencyModel>();
        private Task listAllTask;

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher.Next next)
        {
            if (query is ListAllCurrency listAll)
            {
                if (models.Count == 0)
                {
                    if (listAllTask == null)
                        listAllTask = LoadAllAsync(listAll, next);

                    await listAllTask;
                    listAllTask = null;
                }

                return models.Select(c => c.Clone()).ToList();
            }

            return await next(query);
        }

        private async Task LoadAllAsync(ListAllCurrency listAll, HttpQueryDispatcher.Next next)
        {
            models.Clear();
            models.AddRange((List<CurrencyModel>)await next(listAll));
        }

        private CurrencyModel Find(string uniqueCode)
            => models.FirstOrDefault(c => c.UniqueCode.Equals(uniqueCode));

        private Task Update(string uniqueCode, Action<CurrencyModel> handler)
        {
            CurrencyModel model = Find(uniqueCode);
            if (model != null)
                handler(model);

            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            models.Add(new CurrencyModel(payload.UniqueCode, payload.Symbol, false));
            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            foreach (CurrencyModel model in models)
                model.IsDefault = false;

            return Update(payload.UniqueCode, model => model.IsDefault = false);
        }

        Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload)
            => Update(payload.UniqueCode, model => model.Symbol = payload.Symbol);

        Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload)
            => Update(payload.UniqueCode, model => models.Remove(model));
    }
}
