using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Events;
using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries.Handlers;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace Money.Services.Models.Builders
{
    /// <summary>
    /// A builder for querying categories.
    /// </summary>
    public class CategoryBuilder :
        IEventHandler<CategoryCreated>,
        IEventHandler<CategoryRenamed>,
        IEventHandler<CategoryDescriptionChanged>,
        IEventHandler<CategoryColorChanged>,
        IEventHandler<CategoryIconChanged>,
        IEventHandler<CategoryDeleted>,
        IQueryHandler<ListAllCategory, List<CategoryModel>>,
        IQueryHandler<GetCategoryIcon, string>
    {
        public Task HandleAsync(CategoryCreated payload)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                db.Categories.Add(new CategoryEntity(new CategoryModel(
                    payload.AggregateKey,
                    payload.Name,
                    null,
                    payload.Color,
                    null
                )));

                return db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryRenamed payload)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.Name = payload.NewName;
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryDescriptionChanged payload)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.Description = payload.Description;
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryColorChanged payload)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.SetColor(payload.Color);
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryIconChanged payload)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.Icon = payload.Icon;
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CategoryDeleted payload)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                CategoryEntity entity = await db.Categories.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                entity.IsDeleted = true;
                await db.SaveChangesAsync();
            }
        }

        public Task<List<CategoryModel>> HandleAsync(ListAllCategory query)
        {
            List<CategoryModel> result = new List<CategoryModel>();
            using (ReadModelContext db = new ReadModelContext())
            {
                return db.Categories
                    .Where(c => !c.IsDeleted)
                    .OrderBy(c => c.Name)
                    .Select(e => e.ToModel())
                    .ToListAsync();
            }
        }

        public async Task<string> HandleAsync(GetCategoryIcon query)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                CategoryEntity entity = await db.Categories.FindAsync(query.CategoryKey.AsGuidKey().Guid);
                return entity.Icon;
            }
        }
    }
}
