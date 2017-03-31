using Microsoft.EntityFrameworkCore;
using Money.Data;
using Money.Events;
using Money.Services.Models.Queries;
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
        IQueryHandler<ListAllCurrency, List<string>>
    {
        public async Task HandleAsync(CurrencyCreated payload)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                await db.Currencies.AddAsync(new CurrencyEntity()
                {
                    Name = payload.Name
                });

                await db.SaveChangesAsync();
            }
        }

        public Task<List<string>> HandleAsync(ListAllCurrency query)
        {
            using (ReadModelContext db = new ReadModelContext())
            {
                return db.Currencies
                    .Select(c => c.Name)
                    .ToListAsync();
            }
        }
    }
}
