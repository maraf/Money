using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    internal class ExpenseTemplateMiddleware : HttpQueryDispatcher.IMiddleware,
        IEventHandler<ExpenseTemplateCreated>,
        IEventHandler<ExpenseTemplateDeleted>,
        IEventHandler<UserSignedOut>
    {
        private readonly ServerConnectionState serverConnection;
        private readonly ExpenseTemplateStorage localStorage;

        private readonly List<ExpenseTemplateModel> models = new List<ExpenseTemplateModel>();
        private Task listAllTask;

        public ExpenseTemplateMiddleware(ServerConnectionState serverConnection, ExpenseTemplateStorage localStorage)
        {
            Ensure.NotNull(serverConnection, "serverConnection");
            Ensure.NotNull(localStorage, "localStorage");
            this.serverConnection = serverConnection;
            this.localStorage = localStorage;
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is ListAllExpenseTemplate listAll)
            {
                await EnsureListAsync(null, next, listAll);
                return models.Select(c => c.Clone()).ToList();
            }

            return await next(query);
        }

        private async Task EnsureListAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListAllExpenseTemplate listAll)
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

        private async Task LoadAllAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListAllExpenseTemplate listAll)
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
                models.AddRange((List<ExpenseTemplateModel>)await next(listAll));
                await localStorage.SaveAsync(models);
            }
        }

        async Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            models.Clear();
            await localStorage.DeleteAsync();
        }

        Task IEventHandler<ExpenseTemplateCreated>.HandleAsync(ExpenseTemplateCreated payload)
        {
            models.Add(new ExpenseTemplateModel(payload.AggregateKey, payload.Amount, payload.Description, payload.CategoryKey));
            return Task.CompletedTask;
        }

        Task IEventHandler<ExpenseTemplateDeleted>.HandleAsync(ExpenseTemplateDeleted payload)
        {
            var model = models.Find(m => m.Key.Equals(payload.AggregateKey));
            if (model != null)
                models.Remove(model);

            return Task.CompletedTask;
        }
    }
}
