using Microsoft.EntityFrameworkCore;
using Money.Events;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Events.Handlers;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Builders
{
    /// <summary>
    /// A builder for querying currencies.
    /// </summary>
    public class CurrencyBuilder :
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencyExchangeRateSet>,
        IEventHandler<CurrencyExchangeRateRemoved>,
        IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyDeleted>,
        IQueryHandler<ListAllCurrency, List<CurrencyModel>>,
        IQueryHandler<ListTargetCurrencyExchangeRates, List<ExchangeRateModel>>,
        IQueryHandler<GetCurrencyDefault, string>,
        IQueryHandler<FindCurrencyDefault, string>
    {
        private readonly IFactory<ReadModelContext> readModelContextFactory;

        internal CurrencyBuilder(IFactory<ReadModelContext> readModelContextFactory)
        {
            Ensure.NotNull(readModelContextFactory, "readModelContextFactory");
            this.readModelContextFactory = readModelContextFactory;
        }

        public async Task HandleAsync(CurrencyCreated payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                await db.Currencies.AddAsync(
                    new CurrencyEntity()
                    {
                        UniqueCode = payload.UniqueCode,
                        Symbol = payload.Symbol
                    }.SetUserKey(payload.UserKey)
                );

                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CurrencyDefaultChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity entity = db.Currencies.WhereUserKey(payload.UserKey).FirstOrDefault(c => c.IsDefault);
                if (entity != null)
                    entity.IsDefault = false;

                entity = db.Currencies.WhereUserKey(payload.UserKey).FirstOrDefault(c => c.UniqueCode == payload.UniqueCode);
                if (entity != null)
                    entity.IsDefault = true;

                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CurrencySymbolChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity entity = db.Currencies.WhereUserKey(payload.UserKey).First(c => c.UniqueCode == payload.UniqueCode);
                entity.Symbol = payload.Symbol;
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CurrencyDeleted payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity entity = db.Currencies.WhereUserKey(payload.UserKey).First(c => c.UniqueCode == payload.UniqueCode);
                entity.IsDeleted = true;
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<CurrencyModel>> HandleAsync(ListAllCurrency query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                return await db.Currencies
                    .WhereUserKey(query.UserKey)
                    .Where(c => !c.IsDeleted)
                    .Select(c => new CurrencyModel(c.UniqueCode, c.Symbol, c.IsDefault))
                    .ToListAsync();
            }
        }
        
        public async Task HandleAsync(CurrencyExchangeRateSet payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                await db.ExchangeRates.AddAsync(
                    new CurrencyExchangeRateEntity()
                    {
                        TargetCurrency = payload.TargetUniqueCode,
                        SourceCurrency = payload.SourceUniqueCode,
                        Rate = payload.Rate,
                        ValidFrom = payload.ValidFrom
                    }.SetUserKey(payload.UserKey)
                );

                await db.SaveChangesAsync();
            }
        }
        
        public async Task HandleAsync(CurrencyExchangeRateRemoved payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyExchangeRateEntity entity = await db.ExchangeRates
                    .WhereUserKey(payload.UserKey)
                    .FirstOrDefaultAsync(r => r.TargetCurrency == payload.TargetUniqueCode && r.SourceCurrency == payload.SourceUniqueCode && r.Rate == payload.Rate && r.ValidFrom == payload.ValidFrom);

                if (entity != null)
                    db.ExchangeRates.Remove(entity);

                await db.SaveChangesAsync();
            }
        }

        public async Task<List<ExchangeRateModel>> HandleAsync(ListTargetCurrencyExchangeRates query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                return await db.ExchangeRates
                    .WhereUserKey(query.UserKey)
                    .Where(r => r.TargetCurrency == query.TargetCurrency)
                    .OrderByDescending(r => r.ValidFrom)
                    .Select(r => new ExchangeRateModel(r.SourceCurrency, r.Rate, r.ValidFrom))
                    .ToListAsync();
            }
        }

        public async Task<string> HandleAsync(GetCurrencyDefault query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity currency = await db.Currencies.WhereUserKey(query.UserKey).FirstAsync(c => c.IsDefault);
                return currency.UniqueCode;
            }
        }

        public async Task<string> HandleAsync(FindCurrencyDefault query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity currency = await db.Currencies.WhereUserKey(query.UserKey).FirstOrDefaultAsync(c => c.IsDefault);
                return currency?.UniqueCode;
            }
        }
    }
}
