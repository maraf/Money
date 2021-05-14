using Money.Events;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Builders
{
    public class IncomeBuilder : IEventHandler<IncomeCreated>,
        IEventHandler<IncomeDeleted>
    {
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
                db.Incomes.Add(new IncomeEntity(payload));

                await db.SaveChangesAsync();
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
    }
}
