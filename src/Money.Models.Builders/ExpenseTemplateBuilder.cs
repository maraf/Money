using Microsoft.EntityFrameworkCore;
using Money.Events;
using Money.Models.Queries;
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
    public class ExpenseTemplateBuilder : IEventHandler<ExpenseTemplateCreated>,
        IEventHandler<ExpenseTemplateDeleted>,
        IQueryHandler<ListAllExpenseTemplate, List<ExpenseTemplateModel>>
    {
        private readonly IFactory<ReadModelContext> dbFactory;

        public ExpenseTemplateBuilder(IFactory<ReadModelContext> dbFactory)
        {
            Ensure.NotNull(dbFactory, "dbFactory");
            this.dbFactory = dbFactory;
        }

        public async Task HandleAsync(ExpenseTemplateCreated payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                db.ExpenseTemplates.Add(new ExpenseTemplateEntity(payload).SetUserKey(payload.UserKey));
                await db.SaveChangesAsync();
            }
        }

        public async Task HandleAsync(ExpenseTemplateDeleted payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var entity = await db.ExpenseTemplates.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    db.ExpenseTemplates.Remove(entity);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task<List<ExpenseTemplateModel>> HandleAsync(ListAllExpenseTemplate query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var sql = db.ExpenseTemplates
                    .WhereUserKey(query)
                    .OrderBy(e => e.Description)
                    .ThenBy(e => e.CategoryId);

                List<ExpenseTemplateModel> models = await sql
                    .Select(e => e.ToModel())
                    .ToListAsync();

                return models;
            }
        }
    }
}
