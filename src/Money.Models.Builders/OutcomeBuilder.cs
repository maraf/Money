using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Money.Models.Builders
{
    public class OutcomeBuilder : IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeCategoryAdded>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeDescriptionChanged>,
        IEventHandler<OutcomeWhenChanged>,
        IEventHandler<OutcomeDeleted>,
        IQueryHandler<ListMonthWithOutcome, List<MonthModel>>,
        IQueryHandler<ListMonthWithExpenseOrIncome, List<MonthModel>>,
        IQueryHandler<ListYearWithOutcome, List<YearModel>>,
        IQueryHandler<ListYearWithExpenseOrIncome, List<YearModel>>,
        IQueryHandler<ListMonthCategoryWithOutcome, List<CategoryWithAmountModel>>,
        IQueryHandler<ListYearCategoryWithOutcome, List<CategoryWithAmountModel>>,
        IQueryHandler<GetTotalMonthOutcome, Price>,
        IQueryHandler<GetTotalYearOutcome, Price>,
        IQueryHandler<GetCategoryName, string>,
        IQueryHandler<GetCategoryColor, Color>,
        IQueryHandler<ListMonthOutcomeFromCategory, List<OutcomeOverviewModel>>,
        IQueryHandler<ListYearOutcomeFromCategory, List<OutcomeOverviewModel>>,
        IQueryHandler<SearchOutcomes, List<OutcomeOverviewModel>>,
        IQueryHandler<ListMonthOutcomesForCategory, List<MonthWithAmountModel>>,
        IQueryHandler<ListYearOutcomesForCategory, List<YearWithAmountModel>>,
        IQueryHandler<ListMonthBalance, List<MonthBalanceModel>>,
        IQueryHandler<ListMonthExpenseChecklist, List<ExpenseChecklistModel>>,
        IQueryHandler<GetMonthExpectedExpenseTotal, Price>
    {
        const int PageSize = 10;

        private readonly IFactory<ReadModelContext> dbFactory;
        private readonly IPriceConverter priceConverter;

        internal OutcomeBuilder(IFactory<ReadModelContext> dbFactory, IPriceConverter priceConverter)
        {
            Ensure.NotNull(dbFactory, "dbFactory");
            Ensure.NotNull(priceConverter, "priceConverter");
            this.dbFactory = dbFactory;
            this.priceConverter = priceConverter;
        }

        public async Task<List<MonthModel>> HandleAsync(ListMonthWithOutcome query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var entities = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Select(o => new { Year = o.When.Year, Month = o.When.Month })
                    .Distinct()
                    .OrderByDescending(o => o.Year)
                    .ThenByDescending(o => o.Month)
                    .ToListAsync();

                return entities
                    .Select(o => new MonthModel(o.Year, o.Month))
                    .ToList();
            }
        }

        public async Task<List<MonthModel>> HandleAsync(ListMonthWithExpenseOrIncome query)
        {
            var expenses = await HandleAsync(new ListMonthWithOutcome() { UserKey = query.UserKey });
            var incomes = await GetListMonthWithIncomeAsync(query.UserKey);
            var union = expenses
                .Union(incomes)
                .OrderByDescending(o => o.Year)
                .ThenByDescending(o => o.Month)
                .ToList();

            return union;
        }

        private async Task<List<MonthModel>> GetListMonthWithIncomeAsync(IKey userKey)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var entities = await db.Incomes
                    .WhereUserKey(userKey)
                    .Select(o => new { Year = o.When.Year, Month = o.When.Month })
                    .Distinct()
                    .OrderByDescending(o => o.Year)
                    .ThenByDescending(o => o.Month)
                    .ToListAsync();

                return entities
                    .Select(o => new MonthModel(o.Year, o.Month))
                    .ToList();
            }
        }

        public async Task<List<YearModel>> HandleAsync(ListYearWithOutcome query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var entities = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Select(o => o.When.Year)
                    .Distinct()
                    .OrderByDescending(o => o)
                    .ToListAsync();

                return entities
                    .Select(o => new YearModel(o))
                    .ToList();
            }
        }

        public async Task<List<YearModel>> HandleAsync(ListYearWithExpenseOrIncome query)
        {
            var expenses = await HandleAsync(new ListYearWithOutcome() { UserKey = query.UserKey });
            var incomes = await GetListYearWithIncomeAsync(query.UserKey);
            var union = expenses
                .Union(incomes)
                .OrderByDescending(o => o.Year)
                .ToList();

            return union;
        }

        private async Task<List<YearModel>> GetListYearWithIncomeAsync(IKey userKey)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var entities = await db.Incomes
                    .WhereUserKey(userKey)
                    .Select(o => o.When.Year)
                    .Distinct()
                    .OrderByDescending(o => o)
                    .ToListAsync();

                return entities
                    .Select(o => new YearModel(o))
                    .ToList();
            }
        }

        private async Task<List<CategoryWithAmountModel>> GetCategoryWithAmounts(ReadModelContext db, IKey userKey, List<OutcomeEntity> outcomes, int version = 1)
        {
            Dictionary<Guid, Price> totals = new Dictionary<Guid, Price>();
            Dictionary<Guid, Price> fixedTotals = new Dictionary<Guid, Price>();

            foreach (OutcomeEntity outcome in outcomes)
            {
                foreach (OutcomeCategoryEntity category in outcome.Categories)
                {
                    var dictionary = outcome.IsFixed ? fixedTotals : totals;
                    if (dictionary.TryGetValue(category.CategoryId, out var price))
                        price = price + priceConverter.ToDefault(userKey, outcome);
                    else
                        price = priceConverter.ToDefault(userKey, outcome);

                    dictionary[category.CategoryId] = price;
                }
            }

            List<CategoryWithAmountModel> result = new List<CategoryWithAmountModel>();
            foreach (var item in totals.Keys.Union(fixedTotals.Keys))
            {
                CategoryEntity entity = await db.Categories.FirstOrDefaultAsync(c => c.Id == item);
                if (entity == null)
                    throw Ensure.Exception.InvalidOperation($"Missing category with id '{item}'.");

                CategoryModel model = entity.ToModel(false);

                if (!fixedTotals.TryGetValue(item, out var fixedTotal))
                    fixedTotal = priceConverter.ZeroDefault(userKey);

                if (!totals.TryGetValue(item, out var nonFixedTotal))
                    nonFixedTotal = priceConverter.ZeroDefault(userKey);

                CategoryWithAmountModel resultItem = null;
                if (version == 1)
                {
                    resultItem = new CategoryWithAmountModel(
                        model.Key,
                        model.Name,
                        model.Description,
                        model.Color,
                        model.Icon,
                        nonFixedTotal
                    );
                }
                else if (version == 2)
                {
                    resultItem = new CategoryWithAmountModel(
                        model.Key,
                        model.Name,
                        model.Description,
                        model.Color,
                        model.Icon,
                        nonFixedTotal,
                        fixedTotal
                    );
                }
                else
                {
                    throw Ensure.Exception.InvalidOperation($"Unsupported version '{version}' of query.");
                }

                result.Add(resultItem);
            }

            return result.OrderBy(m => m.Name).ToList();
        }

        public async Task<List<CategoryWithAmountModel>> HandleAsync(ListMonthCategoryWithOutcome query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                List<OutcomeEntity> outcomes = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Month == query.Month.Month && o.When.Year == query.Month.Year)
                    .Include(o => o.Categories)
                    .ToListAsync();

                return await GetCategoryWithAmounts(db, query.UserKey, outcomes, query.Version);
            }
        }

        public async Task<List<CategoryWithAmountModel>> HandleAsync(ListYearCategoryWithOutcome query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                List<OutcomeEntity> outcomes = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Year == query.Year.Year)
                    .Include(o => o.Categories)
                    .ToListAsync();

                return await GetCategoryWithAmounts(db, query.UserKey, outcomes, 1);
            }
        }

        public async Task<string> HandleAsync(GetCategoryName query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                CategoryEntity category = await db.Categories.FindAsync(query.CategoryKey.AsGuidKey().Guid);
                if (category != null && category.IsUserKey(query.UserKey))
                    return category.Name;

                throw Ensure.Exception.ArgumentOutOfRange("categoryKey", "No such category with key '{0}'.", query.CategoryKey);
            }
        }

        public async Task<Color> HandleAsync(GetCategoryColor query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                CategoryEntity category = await db.Categories.FindAsync(query.CategoryKey.AsGuidKey().Guid);
                if (category != null && category.IsUserKey(query.UserKey))
                    return category.ToColor();

                throw Ensure.Exception.ArgumentOutOfRange("categoryKey", "No such category with key '{0}'.", query.CategoryKey);
            }
        }

        private Price SumPriceInDefaultCurrency(IKey userKey, IEnumerable<PriceFixed> outcomes)
        {
            Price price = priceConverter.ZeroDefault(userKey);
            foreach (PriceFixed outcome in outcomes)
                price += priceConverter.ToDefault(userKey, outcome);

            return price;
        }

        public async Task<Price> HandleAsync(GetTotalMonthOutcome query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                List<PriceFixed> outcomes = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Month == query.Month.Month && o.When.Year == query.Month.Year)
                    .Select(o => new PriceFixed(new Price(o.Amount, o.Currency), o.When))
                    .ToListAsync();

                return SumPriceInDefaultCurrency(query.UserKey, outcomes);
            }
        }

        public async Task<Price> HandleAsync(GetTotalYearOutcome query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                List<PriceFixed> outcomes = await db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Year == query.Year.Year)
                    .Select(o => new PriceFixed(new Price(o.Amount, o.Currency), o.When))
                    .ToListAsync();

                return SumPriceInDefaultCurrency(query.UserKey, outcomes);
            }
        }

        private IQueryable<OutcomeEntity> ApplyPaging(IQueryable<OutcomeEntity> sql, IPageableQuery query)
        {
            if (query.PageIndex != null)
                sql = sql.TakePage(query.PageIndex.Value, PageSize);

            return sql;
        }

        public async Task<List<OutcomeOverviewModel>> HandleAsync(ListYearOutcomeFromCategory query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var sql = db.Outcomes
                    .Include(o => o.Categories)
                    .WhereUserKey(query)
                    .Where(o => o.When.Year == query.Year.Year);

                sql = ApplyCategoryKey(sql, query.CategoryKey);
                sql = ApplySorting(sql, query);
                sql = ApplyPaging(sql, query);

                List<OutcomeOverviewModel> outcomes = await sql
                    .Select(o => o.ToOverviewModel(query.Version))
                    .ToListAsync();

                return outcomes;
            }
        }

        public async Task<List<OutcomeOverviewModel>> HandleAsync(ListMonthOutcomeFromCategory query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var sql = db.Outcomes
                    .Include(o => o.Categories)
                    .WhereUserKey(query)
                    .Where(o => o.When.Month == query.Month.Month && o.When.Year == query.Month.Year);

                sql = ApplyCategoryKey(sql, query.CategoryKey);
                sql = ApplySorting(sql, query);
                sql = ApplyPaging(sql, query);

                List<OutcomeOverviewModel> models = await sql
                    .Select(o => o.ToOverviewModel(query.Version))
                    .ToListAsync();

                return models;
            }
        }

        private static IQueryable<OutcomeEntity> ApplyCategoryKey(IQueryable<OutcomeEntity> sql, IKey categoryKey)
        {
            if (!categoryKey.IsEmpty)
                sql = sql.Where(o => o.Categories.Any(c => c.CategoryId == categoryKey.AsGuidKey().Guid));

            return sql;
        }

        public async Task HandleAsync(OutcomeCreated payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                db.Outcomes.Add(new OutcomeEntity(
                    new OutcomeModel(
                        payload.AggregateKey,
                        payload.Amount,
                        payload.When,
                        payload.Description,
                        Enumerable.Empty<IKey>(),
                        payload.IsFixed
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
            using (ReadModelContext db = dbFactory.Create())
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
            using (ReadModelContext db = dbFactory.Create())
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
            using (ReadModelContext db = dbFactory.Create())
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
            using (ReadModelContext db = dbFactory.Create())
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
            using (ReadModelContext db = dbFactory.Create())
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
            using (ReadModelContext db = dbFactory.Create())
            {
                var sql = db.Outcomes
                    .Include(o => o.Categories)
                    .WhereUserKey(query.UserKey)
                    .Where(o => EF.Functions.Like(o.Description, $"%{query.Text}%"));

                sql = ApplySorting(sql, query);
                sql = ApplyPaging(sql, query);

                List<OutcomeEntity> entities = await sql.ToListAsync();

                List<OutcomeOverviewModel> models = entities
                    .Select(e => e.ToOverviewModel(query.Version))
                    .ToList();

                return models;
            }
        }

        private IQueryable<OutcomeEntity> ApplySorting(IQueryable<OutcomeEntity> sql, ISortableQuery<OutcomeOverviewSortType> query)
        {
            var sortDescriptor = query.SortDescriptor;
            if (sortDescriptor == null)
                sortDescriptor = new SortDescriptor<OutcomeOverviewSortType>(OutcomeOverviewSortType.ByWhen);

            switch (sortDescriptor.Type)
            {
                case OutcomeOverviewSortType.ByAmount:
                    sql = sql.OrderBy(sortDescriptor.Direction, o => o.Amount);
                    break;
                case OutcomeOverviewSortType.ByCategory:
                    sql = sql.OrderBy(sortDescriptor.Direction, o => o.Categories.FirstOrDefault().Category.Name);
                    break;
                case OutcomeOverviewSortType.ByDescription:
                    sql = sql.OrderBy(sortDescriptor.Direction, o => o.Description);
                    break;
                case OutcomeOverviewSortType.ByWhen:
                    sql = sql.OrderBy(sortDescriptor.Direction, o => o.When);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(sortDescriptor.Type.ToString());
            }

            return sql;
        }

        public async Task<List<MonthWithAmountModel>> HandleAsync(ListMonthOutcomesForCategory query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var sql = db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Year == query.Year.Year);

                sql = ApplyCategoryKey(sql, query.CategoryKey);

                List<OutcomeEntity> entities = await sql.ToListAsync();
                Dictionary<MonthModel, Price> totals = new Dictionary<MonthModel, Price>();
                foreach (OutcomeEntity entity in entities)
                {
                    MonthModel month = entity.When;
                    Price price;
                    if (totals.TryGetValue(month, out price))
                        price = price + priceConverter.ToDefault(query.UserKey, entity);
                    else
                        price = priceConverter.ToDefault(query.UserKey, entity);

                    totals[month] = price;
                }

                return totals
                    .Select(t => new MonthWithAmountModel(t.Key.Year, t.Key.Month, t.Value))
                    .ToList();
            }
        }

        public async Task<List<YearWithAmountModel>> HandleAsync(ListYearOutcomesForCategory query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var sql = db.Outcomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Year >= query.StartYear.Year && o.When.Year < (query.StartYear.Year + PageSize));

                sql = ApplyCategoryKey(sql, query.CategoryKey);

                List<OutcomeEntity> entities = await sql.ToListAsync();
                Dictionary<YearModel, Price> totals = new Dictionary<YearModel, Price>();
                foreach (OutcomeEntity entity in entities)
                {
                    YearModel month = entity.When;
                    Price price;
                    if (totals.TryGetValue(month, out price))
                        price = price + priceConverter.ToDefault(query.UserKey, entity);
                    else
                        price = priceConverter.ToDefault(query.UserKey, entity);

                    totals[month] = price;
                }

                return totals
                    .Select(t => new YearWithAmountModel(t.Key.Year, t.Value))
                    .ToList();
            }
        }

        public async Task<List<MonthBalanceModel>> HandleAsync(ListMonthBalance query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                Dictionary<MonthModel, Price> expenses = await GetMonthBalanceExpensesAsync(db, query);
                Dictionary<MonthModel, Price> incomes = await GetMonthBalanceIncomesAsync(db, query);
                
                // TODO: Do it for every month?
                Dictionary<MonthModel, Price> expectedExpenses = new();
                if (query.IncludeExpectedExpenses)
                {
                    for (int i = 1; i < 13; i ++)
                    {
                        var month = new MonthModel(query.Year.Year, i);
                        expectedExpenses[month] = await HandleAsync(new GetMonthExpectedExpenseTotal(month) { UserKey = query.UserKey });
                    }
                }

                List<MonthBalanceModel> result = new List<MonthBalanceModel>();
                for (int i = 1; i < 13; i ++)
                {
                    var expense = expenses.FirstOrDefault(e => e.Key.Month == i).Value;
                    if (expense == null)
                        expense = priceConverter.ZeroDefault(query.UserKey);

                    var income = incomes.FirstOrDefault(e => e.Key.Month == i).Value;
                    if (income == null)
                        income = priceConverter.ZeroDefault(query.UserKey);

                    if (query.IncludeExpectedExpenses)
                    {
                        var expectedExpense = expectedExpenses.FirstOrDefault(e => e.Key.Month == i).Value;
                        result.Add(new MonthBalanceModel(query.Year.Year, i, expense, income, expectedExpense));
                    }
                    else
                    {
                        result.Add(new MonthBalanceModel(query.Year.Year, i, expense, income));
                    }
                }
                
                return result;
            }
        }

        private async Task<Dictionary<MonthModel, Price>> GetMonthBalanceExpensesAsync(ReadModelContext db, ListMonthBalance query)
        {
            var sql = db.Outcomes
                .WhereUserKey(query.UserKey)
                .Where(o => o.When.Year == query.Year.Year);

            List<OutcomeEntity> entities = await sql.ToListAsync();
            Dictionary<MonthModel, Price> totals = new Dictionary<MonthModel, Price>();
            foreach (OutcomeEntity entity in entities)
            {
                MonthModel month = entity.When;
                Price price;
                if (totals.TryGetValue(month, out price))
                    price = price + priceConverter.ToDefault(query.UserKey, entity);
                else
                    price = priceConverter.ToDefault(query.UserKey, entity);

                totals[month] = price;
            }

            return totals;
        }

        private async Task<Dictionary<MonthModel, Price>> GetMonthBalanceIncomesAsync(ReadModelContext db, ListMonthBalance query)
        {
            var sql = db.Incomes
                .WhereUserKey(query.UserKey)
                .Where(o => o.When.Year == query.Year.Year);

            List<IncomeEntity> entities = await sql.ToListAsync();
            Dictionary<MonthModel, Price> totals = new Dictionary<MonthModel, Price>();
            foreach (IncomeEntity entity in entities)
            {
                MonthModel month = entity.When;
                Price price;
                if (totals.TryGetValue(month, out price))
                    price += priceConverter.ToDefault(query.UserKey, entity);
                else
                    price = priceConverter.ToDefault(query.UserKey, entity);

                totals[month] = price;
            }

            return totals;
        }

        public async Task<Price> HandleAsync(GetMonthExpectedExpenseTotal query)
        {
            var result = priceConverter.ZeroDefault(query.UserKey);
            var checklist = await HandleAsync(new ListMonthExpenseChecklist(query.Month) { UserKey = query.UserKey });
            foreach (var expected in checklist)
            {
                if (!expected.ExpenseKey.IsEmpty)
                    continue;
                
                result += priceConverter.ToDefault(query.UserKey, new PriceFixed(expected.Amount, expected.When));
            }

            return result;
        }

        public async Task<List<ExpenseChecklistModel>> HandleAsync(ListMonthExpenseChecklist query)
        {
            var result = new List<ExpenseChecklistModel>();

            using (ReadModelContext db = dbFactory.Create())
            {
                var templates = await db.ExpenseTemplates
                    .WhereUserKey(query.UserKey)
                    .Where(e => e.CreatedAt == null 
                        || e.CreatedAt.Value.Year < query.Month.Year 
                        || (e.CreatedAt.Value.Year == query.Month.Year && e.CreatedAt.Value.Month <= query.Month.Month)
                    )
                    .Where(e => e.DeletedAt == null 
                        || e.DeletedAt.Value.Year > query.Month.Year 
                        || (e.DeletedAt.Value.Year == query.Month.Year && e.DeletedAt.Value.Month >= query.Month.Month)
                    )
                    .Where(e => e.Period != null)
                    .Where(e => e.Period != RecurrencePeriod.Single || (e.DueDate.Value.Year == query.Month.Year && e.DueDate.Value.Month == query.Month.Month))
                    .ToListAsync();

                foreach (var template in templates)
                {
                    var sql = db.Outcomes
                        .Include(o => o.Categories)
                        .WhereUserKey(query.UserKey)
                        .Where(e => e.Description == template.Description);

                    if (template.Period == RecurrencePeriod.Monthly)
                        sql = sql.Where(e => e.When.Month == query.Month.Month && e.When.Year == query.Month.Year);
                    else if (template.Period == RecurrencePeriod.Single)
                        sql = sql.Where(e => e.When.Month == query.Month.Month && e.When.Year == query.Month.Year);

                    var expense = await sql.FirstOrDefaultAsync();

                    var model = expense != null 
                        ? expense.ToExpenseChecklistModel(template.GetKey())
                        : template.ToExpenseChecklistModel(template.Period == RecurrencePeriod.Monthly
                            ? new DateTime(query.Month.Year, query.Month.Month, template.DayInPeriod.Value)
                            : template.DueDate.Value);

                    result.Add(model);
                }
            }
            
            return result;
        }
    }
}
