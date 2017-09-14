using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Events;
using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Events.Handlers;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Models.Builders
{
    /// <summary>
    /// A builder for querying currencies.
    /// </summary>
    public class CurrencyBuilder :
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencyExchangeRateSet>,
        IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyDeleted>,
        IQueryHandler<ListAllCurrency, List<CurrencyModel>>,
        IQueryHandler<ListTargetCurrencyExchangeRates, List<ExchangeRateModel>>,
        IQueryHandler<GetCurrencyDefault, string>
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
                await db.Currencies.AddAsync(new CurrencyEntity()
                {
                    UniqueCode = payload.UniqueCode,
                    Symbol = payload.Symbol
                });

                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CurrencyDefaultChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity entity = db.Currencies.FirstOrDefault(c => c.IsDefault);
                if (entity != null)
                    entity.IsDefault = false;

                entity = db.Currencies.FirstOrDefault(c => c.UniqueCode == payload.UniqueCode);
                if (entity != null)
                    entity.IsDefault = true;

                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CurrencySymbolChanged payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity entity = db.Currencies.First(c => c.UniqueCode == payload.UniqueCode);
                entity.Symbol = payload.Symbol;
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(CurrencyDeleted payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity entity = db.Currencies.First(c => c.UniqueCode == payload.UniqueCode);
                entity.IsDeleted = true;
                await db.SaveChangesAsync();
            }
        }

        public Task<List<CurrencyModel>> HandleAsync(ListAllCurrency query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                return db.Currencies
                    .Where(e => !e.IsDeleted)
                    .Select(e => new CurrencyModel(e.UniqueCode, e.Symbol, e.IsDefault))
                    .ToListAsync();
            }
        }
        
        public async Task HandleAsync(CurrencyExchangeRateSet payload)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                await db.ExchangeRates.AddAsync(new CurrencyExchangeRateEntity()
                {
                    TargetCurrency = payload.TargetUniqueCode,
                    SourceCurrency = payload.SourceUniqueCode,
                    Rate = payload.Rate,
                    ValidFrom = payload.ValidFrom
                });

                await db.SaveChangesAsync();
            }
        }

        public Task<List<ExchangeRateModel>> HandleAsync(ListTargetCurrencyExchangeRates query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                return db.ExchangeRates
                    .Where(e => e.TargetCurrency == query.TargetCurrency)
                    .OrderByDescending(e => e.ValidFrom)
                    .Select(e => new ExchangeRateModel(e.SourceCurrency, e.Rate, e.ValidFrom))
                    .ToListAsync();
            }
        }

        public async Task<string> HandleAsync(GetCurrencyDefault query)
        {
            using (ReadModelContext db = readModelContextFactory.Create())
            {
                CurrencyEntity currency = await db.Currencies.FirstAsync(c => c.IsDefault);
                return currency.UniqueCode;
            }
        }
    }
}
