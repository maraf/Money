using Money.Commands;
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
    internal class CategoryMiddleware : HttpQueryDispatcher.IMiddleware,
        IEventHandler<CategoryCreated>,
        IEventHandler<CategoryDescriptionChanged>,
        IEventHandler<CategoryIconChanged>,
        IEventHandler<CategoryColorChanged>,
        IEventHandler<CategoryDeleted>,
        IEventHandler<UserSignedOut>
    {
        private static readonly ListAllCategory listAllQuery = new ListAllCategory();

        private readonly ServerConnectionState serverConnection;
        private readonly CategoryStorage localStorage;

        private readonly List<CategoryModel> models = new List<CategoryModel>();
        private Task listAllTask;

        public CategoryMiddleware(ServerConnectionState serverConnection, CategoryStorage localStorage)
        {
            Ensure.NotNull(serverConnection, "serverConnection");
            Ensure.NotNull(localStorage, "localStorage");
            this.serverConnection = serverConnection;
            this.localStorage = localStorage;
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is ListAllCategory listAll)
            {
                await EnsureListAsync(null, next, listAll);
                return models.Select(c => c.Clone()).ToList();
            }
            else if (query is GetCategoryName categoryName)
            {
                await EnsureListAsync(dispatcher, null, listAllQuery);
                CategoryModel model = Find(categoryName.CategoryKey);
                if (model != null)
                    return model.Name;
            }
            else if (query is GetCategoryIcon categoryIcon)
            {
                await EnsureListAsync(dispatcher, null, listAllQuery);
                CategoryModel model = Find(categoryIcon.CategoryKey);
                if (model != null)
                    return model.Icon;
            }
            else if (query is GetCategoryColor categoryColor)
            {
                await EnsureListAsync(dispatcher, null, listAllQuery);
                CategoryModel model = Find(categoryColor.CategoryKey);
                if (model == null)
                    return Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

                return model.Color;
            }
            else if (query is GetCategoryNameDescription categoryNameDescription)
            {
                await EnsureListAsync(dispatcher, null, listAllQuery);
                CategoryModel model = Find(categoryNameDescription.CategoryKey);
                if (model != null)
                    return new CategoryNameDescriptionModel(model.Name, model.Description);
            }

            return await next(query);
        }

        private async Task EnsureListAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListAllCategory listAll)
        {
            if (models.Count == 0)
            {
                if (listAllTask == null)
                    listAllTask = LoadAllAsync(dispatcher, next, listAll);

                await listAllTask;
                listAllTask = null;
            }
        }

        private async Task LoadAllAsync(HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next, ListAllCategory listAll)
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
                models.AddRange(await dispatcher.QueryAsync(listAll));
            else
                models.AddRange((List<CategoryModel>)await next(listAll));

            await localStorage.SaveAsync(models);
        }

        private CategoryModel Find(IKey key)
            => models.FirstOrDefault(c => c.Key.Equals(key));

        private async Task Update(IKey key, Action<CategoryModel> handler)
        {
            CategoryModel model = Find(key);
            if (model != null)
                handler(model);

            await localStorage.SaveAsync(models);
        }

        async Task IEventHandler<CategoryCreated>.HandleAsync(CategoryCreated payload)
        {
            models.Add(new CategoryModel(payload.AggregateKey, payload.Name, null, payload.Color, null));
            await localStorage.SaveAsync(models);
        }

        Task IEventHandler<CategoryDescriptionChanged>.HandleAsync(CategoryDescriptionChanged payload)
            => Update(payload.AggregateKey, model => model.Description = payload.Description);

        Task IEventHandler<CategoryIconChanged>.HandleAsync(CategoryIconChanged payload)
            => Update(payload.AggregateKey, model => model.Icon = payload.Icon);

        Task IEventHandler<CategoryColorChanged>.HandleAsync(CategoryColorChanged payload)
            => Update(payload.AggregateKey, model => model.Color = payload.Color);

        Task IEventHandler<CategoryDeleted>.HandleAsync(CategoryDeleted payload)
            => Update(payload.AggregateKey, model => models.Remove(model));

        async Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            models.Clear();
            await localStorage.DeleteAsync();
        }
    }
}
