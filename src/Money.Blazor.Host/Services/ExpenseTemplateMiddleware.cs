using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Queries;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Models.Keys;
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
        IEventHandler<ExpenseTemplateAmountChanged>,
        IEventHandler<ExpenseTemplateDescriptionChanged>,
        IEventHandler<ExpenseTemplateCategoryChanged>,
        IEventHandler<ExpenseTemplateRecurrenceChanged>,
        IEventHandler<ExpenseTemplateRecurrenceCleared>,
        IEventHandler<ExpenseTemplateDeleted>,
        IEventHandler<UserSignedOut>
    {
        private readonly ServerConnectionState serverConnection;
        private readonly ExpenseTemplateStorage localStorage;
        private readonly ILog log;

        private bool areModelsLoadedAtLeastOnce;
        private readonly List<ExpenseTemplateModel> models = new List<ExpenseTemplateModel>();
        private Task listAllTask;

        public ExpenseTemplateMiddleware(ServerConnectionState serverConnection, ExpenseTemplateStorage localStorage, ILogFactory logFactory)
        {
            Ensure.NotNull(serverConnection, "serverConnection");
            Ensure.NotNull(localStorage, "localStorage");
            Ensure.NotNull(logFactory, "logFactory");
            this.serverConnection = serverConnection;
            this.localStorage = localStorage;
            this.log = logFactory.Scope("ExpenseTemplateMiddleware");
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is ListAllExpenseTemplate listAll)
            {
                await EnsureListAsync(null, next, listAll);
                var result = models.Select(c => c.Clone()).ToList();
                await EnsureSortedAsync(dispatcher, result, listAll.SortDescriptor);
                return result;
            }

            return await next(query);
        }

        private async Task EnsureListAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListAllExpenseTemplate listAll)
        {
            if (models.Count == 0 || !areModelsLoadedAtLeastOnce)
            {
                if (listAllTask == null)
                    listAllTask = LoadAllAsync(dispatcher, next, listAll);

                try
                {
                    await listAllTask;
                    areModelsLoadedAtLeastOnce = true;
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

        private async Task EnsureSortedAsync(HttpQueryDispatcher dispatcher, List<ExpenseTemplateModel> models, SortDescriptor<ExpenseTemplateSortType> sortDescriptor)
        {
            log.Debug("Ensure sorted");

            if (sortDescriptor == null)
            {
                sortDescriptor = await dispatcher.QueryAsync(new GetExpenseTemplateSortProperty());
                log.Debug($"The provided sort descriptor is empty, using '{sortDescriptor.Type}' '{sortDescriptor.Direction}'");
            }

            int Compare<T>(SortDirection direction, T a, T b, Func<T, T, int> comparer) => direction switch
            {
                SortDirection.Ascending => comparer(a, b),
                SortDirection.Descending => comparer(b, a),
                _ => throw new NotSupportedException($"Missing case for {direction}")
            };

            switch (sortDescriptor.Type)
            {
                case ExpenseTemplateSortType.ByAmount:
                    models.Sort((a, b) => Compare(sortDescriptor.Direction, a.Amount?.Value ?? 0, b.Amount?.Value ?? 0, Decimal.Compare));
                    break;
                case ExpenseTemplateSortType.ByDescription:
                    models.Sort((a, b) => Compare(sortDescriptor.Direction, a.Description, b.Description, String.Compare));
                    break;
                case ExpenseTemplateSortType.ByCategory:
                    var categoryNames = (await dispatcher.QueryAsync(ListAllCategory.WithDeleted)).ToDictionary(c => c.Key, c => c.Name);
                    categoryNames[KeyFactory.Empty(typeof(Category))] = String.Empty;
                    models.Sort((a, b) => 
                    {
                        var result = Compare(sortDescriptor.Direction, categoryNames[a.CategoryKey], categoryNames[b.CategoryKey], String.Compare);
                        if (result == 0)
                            return Compare(sortDescriptor.Direction, a.Description, b.Description, String.Compare);

                        return result;
                    });
                    break;
            }
        }

        async Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            models.Clear();
            await localStorage.DeleteAsync();
        }

        async Task IEventHandler<ExpenseTemplateCreated>.HandleAsync(ExpenseTemplateCreated payload)
        {
            log.Debug($"Got ExpenseTemplateCreated with key '{payload.AggregateKey}'");

            models.Add(new ExpenseTemplateModel(payload.AggregateKey, payload.Amount, payload.Description, payload.CategoryKey, payload.IsFixed, null, null, null));
            models.Sort((a, b) => StringComparer.InvariantCultureIgnoreCase.Compare(a.Description, b.Description));
            await localStorage.SaveAsync(models);
        }

        private async Task UpdateAsync(IKey aggregateKey, Action<ExpenseTemplateModel> handler)
        {
            var model = models.Find(m => m.Key.Equals(aggregateKey));
            if (model != null)
            {
                handler(model);
                log.Debug($"Updated model with key '{model.Key}'");
            }

            await localStorage.SaveAsync(models);
        }

        Task IEventHandler<ExpenseTemplateAmountChanged>.HandleAsync(ExpenseTemplateAmountChanged payload)
            => UpdateAsync(payload.AggregateKey, model => model.Amount = payload.NewValue);

        Task IEventHandler<ExpenseTemplateDescriptionChanged>.HandleAsync(ExpenseTemplateDescriptionChanged payload)
            => UpdateAsync(payload.AggregateKey, model => model.Description = payload.Description);

        Task IEventHandler<ExpenseTemplateCategoryChanged>.HandleAsync(ExpenseTemplateCategoryChanged payload)
            => UpdateAsync(payload.AggregateKey, model => model.CategoryKey = payload.CategoryKey);

        Task IEventHandler<ExpenseTemplateRecurrenceChanged>.HandleAsync(ExpenseTemplateRecurrenceChanged payload)
            => UpdateAsync(payload.AggregateKey, model => 
            {
                model.Period = payload.Period;
                model.DayInPeriod = payload.DayInPeriod;
                model.DueDate = payload.DueDate;
            });

        Task IEventHandler<ExpenseTemplateRecurrenceCleared>.HandleAsync(ExpenseTemplateRecurrenceCleared payload)
            => UpdateAsync(payload.AggregateKey, model => 
            {
                model.Period = null;
                model.DayInPeriod = null;
                model.DueDate = null;
            });

        Task IEventHandler<ExpenseTemplateDeleted>.HandleAsync(ExpenseTemplateDeleted payload)
            => UpdateAsync(payload.AggregateKey, model => models.Remove(model));
    }
}
