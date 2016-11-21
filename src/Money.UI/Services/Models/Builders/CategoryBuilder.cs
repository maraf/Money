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
    public class CategoryBuilder : IEventHandler<CategoryCreated>, IQueryHandler<ListAllCategory, List<CategoryModel>>
    {
        public Task<List<CategoryModel>> HandleAsync(ListAllCategory query)
        {
            List<CategoryModel> result = new List<CategoryModel>();
            using (ReadModelContext db = new ReadModelContext())
            {
                return db.Categories
                    .Select(e => e.ToModel())
                    .ToListAsync();
            }
        }

        public Task HandleAsync(CategoryCreated payload)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                db.Categories.Add(new CategoryEntity(new CategoryModel(
                    payload.AggregateKey,
                    payload.Name,
                    payload.Color
                )));

                return db.SaveChangesAsync();
            }
        }
    }
}
