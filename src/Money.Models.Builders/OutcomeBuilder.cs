using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Events;
using Money.Models.Queries;
using Money.Models.Sorting;
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
    public class OutcomeBuilder : IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeCategoryAdded>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeDescriptionChanged>,
        IEventHandler<OutcomeWhenChanged>,
        IEventHandler<OutcomeDeleted>,
        IQueryHandler<ListMonthWithOutcome, List<MonthModel>>,
        IQueryHandler<ListMonthCategoryWithOutcome, List<CategoryWithAmountModel>>,
        IQueryHandler<GetTotalMonthOutcome, Price>,
        IQueryHandler<GetCategoryName, string>,
        IQueryHandler<GetCategoryColor, Color>,
        IQueryHandler<ListMonthOutcomeFromCategory, List<OutcomeOverviewModel>>,
        IQueryHandler<ListYearOutcomeFromCategory, IEnumerable<OutcomeOverviewModel>>,
        IQueryHandler<SearchOutcomes, List<OutcomeOverviewModel>>
    {
        const int PageSize = 10;

        private readonly IFactory<ReadModelContext> readModelContextFactory;
        private readonly IPriceConverter priceConverter;

        internal OutcomeBuilder(IFactory<ReadModelContext> readModelContextFactory, IPriceConverter priceConverter)
        {
            Ensure.NotNull(readModelContextFactory, "readModelContextFactory");
            Ensure.NotNull(priceConverter, "priceConverter");
            this.readModelContextFactory = readModelContextFactory;
            this.priceConverter = priceConverter;
        }

        public async Task<List<MonthModel>> HandleAsync(ListMonthWithOutcome query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                var entities = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Select(o => new { Year = o.When.Year, Month = o.When.Month })
                    .OrderByDescending(o => o.Year)
                    .ThenByDescending(o => o.Month)
                    .Distinct()
                    .ToListAsync();

                return entities
                    .Select(o => new MonthModel(o.Year, o.Month))
                    .ToList();
            }
        }

        public async Task<List<CategoryWithAmountModel>> HandleAsync(ListMonthCategoryWithOutcome query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                Dictionary<Guid, Price> totals = new Dictionary<Guid, Price>();

                List<OutcomeEntity> outcomes = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Month == query.Month.Month && o.When.Year == query.Month.Year)
                    .Include(o => o.Categories)
                    .ToListAsync();

                foreach (OutcomeEntity outcome in outcomes)
                {
                    foreach (OutcomeCategoryEntity category in db.OutcomeCategories.Where(oc => oc.OutcomeId == outcome.Id))
                    {
                        Price price;
                        if (totals.TryGetValue(category.CategoryId, out price))
                            price = price + priceConverter.ToDefault(query.UserKey, outcome);
                        else
                            price = priceConverter.ToDefault(query.UserKey, outcome);

                        totals[category.CategoryId] = price;
                    }
                }

                List<CategoryWithAmountModel> result = new List<CategoryWithAmountModel>();
                foreach (var item in totals)
                {
                    CategoryModel model = (await db.Categories.FindAsync(item.Key)).ToModel();
                    result.Add(new CategoryWithAmountModel(
                        model.Key,
                        model.Name,
                        model.Description,
                        model.Color,
                        model.Icon,
                        item.Value
                    ));
                }

                return result.OrderBy(m => m.Name).ToList();
            }
        }

        public async Task<string> HandleAsync(GetCategoryName query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity category = await db.Categories.FindAsync(query.CategoryKey.AsGuidKey().Guid);
                if (category != null && category.IsUserKey(query.UserKey))
                    return category.Name;

                throw Ensure.Exception.ArgumentOutOfRange("categoryKey", "No such category with key '{0}'.", query.CategoryKey);
            }
        }

        public async Task<Color> HandleAsync(GetCategoryColor query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CategoryEntity category = await db.Categories.FindAsync(query.CategoryKey.AsGuidKey().Guid);
                if (category != null && category.IsUserKey(query.UserKey))
                    return category.ToColor();

                throw Ensure.Exception.ArgumentOutOfRange("categoryKey", "No such category with key '{0}'.", query.CategoryKey);
            }
        }

        public async Task<Price> HandleAsync(GetTotalMonthOutcome query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                List<PriceFixed> outcomes = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Month == query.Month.Month && o.When.Year == query.Month.Year)
                    .Select(o => new PriceFixed(new Price(o.Amount, o.Currency), o.When))
                    .ToListAsync();

                Price price = priceConverter.ZeroDefault(query.UserKey);
                foreach (PriceFixed outcome in outcomes)
                    price += priceConverter.ToDefault(query.UserKey, outcome);

                return price;
            }
        }

        public async Task<IEnumerable<OutcomeOverviewModel>> HandleAsync(ListYearOutcomeFromCategory query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                IQueryable<OutcomeEntity> entities = db.Outcomes;
                if (!query.CategoryKey.IsEmpty)
                    entities = entities.Where(o => o.Categories.Select(c => c.CategoryId).Contains(query.CategoryKey.AsGuidKey().Guid));

                List<OutcomeOverviewModel> outcomes = await entities
                    .Include(o => o.Categories)
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Year == query.Year.Year)
                    .OrderBy(o => o.When)
                    .Select(o => o.ToOverviewModel())
                    .ToListAsync();

                return outcomes;
            }
        }

        public async Task<List<OutcomeOverviewModel>> HandleAsync(ListMonthOutcomeFromCategory query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                IQueryable<OutcomeEntity> entities = db.Outcomes.WhereUserKey(query.UserKey);
                if (!query.CategoryKey.IsEmpty)
                    entities = entities.Where(o => o.Categories.Select(c => c.CategoryId).Contains(query.CategoryKey.AsGuidKey().Guid));

                var sql = entities
                    .Include(o => o.Categories)
                    .Where(o => o.When.Month == query.Month.Month && o.When.Year == query.Month.Year);

                sql = SortOverviewOutcomes(sql, query.SortDescriptor);

                if (query.PageIndex != null)
                    sql = sql.Skip(query.PageIndex.Value * PageSize).Take(PageSize);

                List<OutcomeOverviewModel> models = await sql
                    .Select(o => o.ToOverviewModel())
                    .ToListAsync();

                return models;
            }
        }

        public async Task HandleAsync(OutcomeCreated payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                db.Outcomes.Add(new OutcomeEntity(
                    new OutcomeModel(
                        payload.AggregateKey,
                        payload.Amount,
                        payload.When,
                        payload.Description,
                        Enumerable.Empty<IKey>()
                    )
                ).SetUserKey(payload.UserKey));
                
                await db.SaveChangesAsync();

                OutcomeEntity entity = await db.Outcomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    db.OutcomeCategories.Add(new OutcomeCategoryEntity()
                    {
                        OutcomeId = payload.AggregateKey.AsGuidKey().Guid,
                        CategoryId = payload.CategoryKey.AsGuidKey().Guid
                    });
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(OutcomeCategoryAdded payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                OutcomeEntity entity = await db.Outcomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    db.OutcomeCategories.Add(new OutcomeCategoryEntity()
                    {
                        OutcomeId = payload.AggregateKey.AsGuidKey().Guid,
                        CategoryId = payload.CategoryKey.AsGuidKey().Guid
                    });
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(OutcomeAmountChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                OutcomeEntity entity = await db.Outcomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    entity.Amount = payload.NewValue.Value;
                    entity.Currency = payload.NewValue.Currency;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(OutcomeDescriptionChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                OutcomeEntity entity = await db.Outcomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    entity.Description = payload.Description;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(OutcomeWhenChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                OutcomeEntity entity = await db.Outcomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    entity.When = payload.When;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(OutcomeDeleted payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                OutcomeEntity entity = await db.Outcomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    db.Outcomes.Remove(entity);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<List<OutcomeOverviewModel>> HandleAsync(SearchOutcomes query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                List<OutcomeEntity> entities = await db.Outcomes
                    .Include(o => o.Categories)
                    .WhereUserKey(query.UserKey)
                    .Where(o => EF.Functions.Like(o.Description, $"%{query.Text}%"))
                    .TakePage(query.PageIndex, PageSize)
                    .ToListAsync();

                List<OutcomeOverviewModel> models = entities
                    .Select(e => e.ToOverviewModel())
                    .ToList();

                return models;
            }
        }

        private IQueryable<OutcomeEntity> SortOverviewOutcomes(IQueryable<OutcomeEntity> entities, SortDescriptor<OutcomeOverviewSortType> sortDescriptor)
        {
            if (sortDescriptor == null)
                sortDescriptor = new SortDescriptor<OutcomeOverviewSortType>(OutcomeOverviewSortType.ByWhen);

            switch (sortDescriptor.Type)
            {
                case OutcomeOverviewSortType.ByAmount:
                    entities = entities.OrderBy(sortDescriptor.Direction, o => o.Amount);
                    break;
                case OutcomeOverviewSortType.ByCategory:
                    entities = entities.OrderBy(sortDescriptor.Direction, o => o.Categories.FirstOrDefault().Category.Name);
                    break;
                case OutcomeOverviewSortType.ByDescription:
                    entities = entities.OrderBy(sortDescriptor.Direction, o => o.Description);
                    break;
                case OutcomeOverviewSortType.ByWhen:
                    entities = entities.OrderBy(sortDescriptor.Direction, o => o.When);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(sortDescriptor.Type.ToString());
            }

            return entities;
        }
    }
}
