using Microsoft.EntityFrameworkCore;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Builders
{
    public class IncomeBuilder : IEventHandler<IncomeCreated>,
        IEventHandler<IncomeAmountChanged>,
        IEventHandler<IncomeDescriptionChanged>,
        IEventHandler<IncomeWhenChanged>,
        IEventHandler<IncomeDeleted>,
        IQueryHandler<GetTotalMonthIncome, Price>,
        IQueryHandler<ListMonthIncome, List<IncomeOverviewModel>>
    {
        const int PageSize = 10;

        private readonly IFactory<ReadModelContext> dbFactory;
        private readonly IPriceConverter priceConverter;

        public IncomeBuilder(IFactory<ReadModelContext> dbFactory, IPriceConverter priceConverter)
        {
            Ensure.NotNull(dbFactory, "dbFactory");
            Ensure.NotNull(priceConverter, "priceConverter");
            this.dbFactory = dbFactory;
            this.priceConverter = priceConverter;
        }

        public async Task HandleAsync(IncomeCreated payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                db.Incomes.Add(new IncomeEntity(payload).SetUserKey(payload.UserKey));
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(IncomeAmountChanged payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                IncomeEntity entity = await db.Incomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    entity.Amount = payload.NewValue.Value;
                    entity.Currency = payload.NewValue.Currency;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(IncomeDescriptionChanged payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                IncomeEntity entity = await db.Incomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    entity.Description = payload.Description;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(IncomeWhenChanged payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                IncomeEntity entity = await db.Incomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    entity.When = payload.When;
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(IncomeDeleted payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                IncomeEntity entity = await db.Incomes.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    db.Incomes.Remove(entity);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<Price> HandleAsync(GetTotalMonthIncome query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                List<PriceFixed> outcomes = await db.Incomes
                    .WhereUserKey(query.UserKey)
                    .Where(o => o.When.Month == query.Month.Month && o.When.Year == query.Month.Year)
                    .Select(o => new PriceFixed(new Price(o.Amount, o.Currency), o.When))
                    .ToListAsync();

                return SumPriceInDefaultCurrency(query.UserKey, outcomes);
            }
        }

        public async Task<List<IncomeOverviewModel>> HandleAsync(ListMonthIncome query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var sql = db.Incomes
                    .WhereUserKey(query)
                    .Where(i => i.When.Month == query.Month.Month && i.When.Year == query.Month.Year);

                sql = ApplySorting(sql, query);
                sql = ApplyPaging(sql, query);

                List<IncomeOverviewModel> models = await sql
                    .Select(i => i.ToOverviewModel(query))
                    .ToListAsync();

                return models;
            }
        }

        private IQueryable<IncomeEntity> ApplySorting(IQueryable<IncomeEntity> sql, ISortableQuery<IncomeOverviewSortType> query)
        {
            var sortDescriptor = query.SortDescriptor;
            if (sortDescriptor == null)
                sortDescriptor = new SortDescriptor<IncomeOverviewSortType>(IncomeOverviewSortType.ByWhen);

            switch (sortDescriptor.Type)
            {
                case IncomeOverviewSortType.ByAmount:
                    sql = sql.OrderBy(sortDescriptor.Direction, o => o.Amount);
                    break;
                case IncomeOverviewSortType.ByDescription:
                    sql = sql.OrderBy(sortDescriptor.Direction, o => o.Description);
                    break;
                case IncomeOverviewSortType.ByWhen:
                    sql = sql.OrderBy(sortDescriptor.Direction, o => o.When);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(sortDescriptor.Type.ToString());
            }

            return sql;
        }

        private IQueryable<IncomeEntity> ApplyPaging(IQueryable<IncomeEntity> sql, IPageableQuery query)
        {
            if (query.PageIndex != null)
                sql = sql.TakePage(query.PageIndex.Value, PageSize);

            return sql;
        }

        private Price SumPriceInDefaultCurrency(IKey userKey, IEnumerable<PriceFixed> outcomes)
        {
            Price price = priceConverter.ZeroDefault(userKey);
            foreach (PriceFixed outcome in outcomes)
                price += priceConverter.ToDefault(userKey, outcome);

            return price;
        }
    }
}
