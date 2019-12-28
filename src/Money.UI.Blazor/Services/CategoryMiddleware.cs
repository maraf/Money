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
        private readonly NetworkState network;
        private readonly CategoryStorage localStorage;

        private readonly List<CategoryModel> models = new List<CategoryModel>();
        private Task listAllTask;

        public CategoryMiddleware(NetworkState network, CategoryStorage localStorage)
        {
            Ensure.NotNull(network, "network");
            Ensure.NotNull(localStorage, "localStorage");
            this.network = network;
            this.localStorage = localStorage;
        }

        public async Task<object> ExecuteAsync(object query, HttpQueryDispatcher dispatcher, HttpQueryDispatcher.Next next)
        {
            if (query is ListAllCategory listAll)
            {
                if (models.Count == 0)
                {
                    if (listAllTask == null)
                        listAllTask = LoadAllAsync(listAll, next).ContinueWith(t => listAllTask = null);

                    await listAllTask;
                }

                return models.Select(c => c.Clone()).ToList();
            }
            else if (query is GetCategoryName categoryName)
            {
                CategoryModel model = Find(categoryName.CategoryKey);
                if (model != null)
                    return model.Name;
            }
            else if (query is GetCategoryIcon categoryIcon)
            {
                CategoryModel model = Find(categoryIcon.CategoryKey);
                if (model != null)
                    return model.Icon;
            }
            else if (query is GetCategoryColor categoryColor)
            {
                CategoryModel model = Find(categoryColor.CategoryKey);
                if (model == null)
                {
                    var models = await dispatcher.QueryAsync(new ListAllCategory());
                    model = models.FirstOrDefault(c => c.Key.Equals(categoryColor.CategoryKey));
                }

                model = Find(categoryColor.CategoryKey);
                if (model == null)
                    return Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

                return model.Color;
            }
            else if (query is GetCategoryNameDescription categoryNameDescription)
            {
                CategoryModel model = Find(categoryNameDescription.CategoryKey);
                if (model != null)
                    return new CategoryNameDescriptionModel(model.Name, model.Description);
            }

            return await next(query);
        }

        private async Task LoadAllAsync(ListAllCategory listAll, HttpQueryDispatcher.Next next)
        {
            models.Clear();
            if (!network.IsOnline)
            {
                var items = await localStorage.LoadAsync();
                if (items != null)
                {
                    models.AddRange(items);
                    return;
                }
            }

            models.AddRange((List<CategoryModel>)await next(listAll));
            await localStorage.SaveAsync(models);
        }

        private CategoryModel Find(IKey key)
            => models.FirstOrDefault(c => c.Key.Equals(key));

        private Task Update(IKey key, Action<CategoryModel> handler)
        {
            CategoryModel model = Find(key);
            if (model != null)
                handler(model);

            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryCreated>.HandleAsync(CategoryCreated payload)
        {
            models.Add(new CategoryModel(payload.AggregateKey, payload.Name, null, payload.Color, null));
            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryDescriptionChanged>.HandleAsync(CategoryDescriptionChanged payload)
            => Update(payload.AggregateKey, model => model.Description = payload.Description);

        Task IEventHandler<CategoryIconChanged>.HandleAsync(CategoryIconChanged payload)
            => Update(payload.AggregateKey, model => model.Icon = payload.Icon);

        Task IEventHandler<CategoryColorChanged>.HandleAsync(CategoryColorChanged payload)
            => Update(payload.AggregateKey, model => model.Color = payload.Color);

        Task IEventHandler<CategoryDeleted>.HandleAsync(CategoryDeleted payload)
            => Update(payload.AggregateKey, model => models.Remove(model));

        Task IEventHandler<UserSignedOut>.HandleAsync(UserSignedOut payload)
        {
            models.Clear();
            return Task.CompletedTask;
        }
    }
}
