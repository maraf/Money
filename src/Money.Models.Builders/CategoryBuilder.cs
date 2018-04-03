using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Events;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Builders
{
    /// <summary>
    /// A builder for querying categories.
    /// </summary>
    internal class CategoryBuilder :
        IEventHandler<CategoryCreated>,
        IEventHandler<CategoryRenamed>,
        IEventHandler<CategoryDescriptionChanged>,
        IEventHandler<CategoryColorChanged>,
        IEventHandler<CategoryIconChanged>,
        IEventHandler<CategoryDeleted>,
        IQueryHandler<ListAllCategory, List<CategoryModel>>,
        IQueryHandler<GetCategoryIcon, string>,
        IQueryHandler<GetCategoryNameDescription, CategoryNameDescriptionModel>
    {
        private readonly IFactory<ReadModelContext> readModelContextFactory;

        public CategoryBuilder(IFactory<ReadModelContext> readModelContextFactory)
        {
            Ensure.NotNull(readModelContextFactory, "readModelContextFactory");
            this.readModelContextFactory = readModelContextFactory;
        }

        public Task HandleAsync(CategoryCreated payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                db.Categories.Add(
                    new CategoryEntity(
                        new CategoryModel(
                            payload.AggregateKey,
                            payload.Name,
                            null,
                            payload.Color,
                            null
                        )
                    ).SetUserKey(payload.UserKey)
                );

                return db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryRenamed payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.Name = payload.NewName;
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryDescriptionChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.Description = payload.Description;
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryColorChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.SetColor(payload.Color);
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryIconChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.Icon = payload.Icon;
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryDeleted payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.IsDeleted = true;
                await db.SaveChangesAsync();
            }
        }

        public Task<List<CategoryModel>> HandleAsync(ListAllCategory query)
        {
            List<CategoryModel> result = new List<CategoryModel>();
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                return db.Categories
                    .WhereUserKey(query.UserKey)
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.Name)
                    .Select(e => e.ToModel())
                    .ToListAsync();
            }
        }

        public async Task<string> HandleAsync(GetCategoryIcon query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity entity = await db.Categories.FindAsync(query.CategoryKey.AsGuidKey().Guid);
                if (entity.IsUserKey(query.UserKey))
                    return entity.Icon;

                throw MissingCategory(query.CategoryKey);
            }
        }

        public async Task<CategoryNameDescriptionModel> HandleAsync(GetCategoryNameDescription query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity entity = await db.Categories.FindAsync(query.CategoryKey.AsGuidKey().Guid);
                if (entity.IsUserKey(query.UserKey))
                    return new CategoryNameDescriptionModel(entity.Name, entity.Description);

                throw MissingCategory(query.CategoryKey);
            }
        }

        private InvalidOperationException MissingCategory(IKey categoryKey)
            => Ensure.Exception.InvalidOperation("Missing category with key '{0}'.", categoryKey);
    }
}
