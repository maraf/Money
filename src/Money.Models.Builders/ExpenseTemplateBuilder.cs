using Microsoft.EntityFrameworkCore;
using Money.Events;
using Money.Models.Queries;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Events;
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
        IEventHandler<ExpenseTemplateAmountChanged>,
        IEventHandler<ExpenseTemplateDescriptionChanged>,
        IEventHandler<ExpenseTemplateCategoryChanged>,
        IEventHandler<ExpenseTemplateFixedChanged>,
        IEventHandler<ExpenseTemplateDeleted>,
        IEventHandler<ExpenseTemplateRecurrenceChanged>,
        IEventHandler<ExpenseTemplateRecurrenceCleared>,
        IQueryHandler<ListAllExpenseTemplate, List<ExpenseTemplateModel>>
    {
        private readonly IFactory<ReadModelContext> dbFactory;

        public ExpenseTemplateBuilder(IFactory<ReadModelContext> dbFactory)
        {
            Ensure.NotNull(dbFactory, "dbFactory");
            this.dbFactory = dbFactory;
        }

        private Task UpdateAsync(IEvent payload, Action<ExpenseTemplateEntity> handler) => UpdateAsync(payload, (db, entity) => handler(entity));

        private async Task UpdateAsync(IEvent payload, Action<ReadModelContext, ExpenseTemplateEntity> handler)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var entity = await db.ExpenseTemplates.FindAsync(payload.AggregateKey.AsGuidKey().Guid);
                if (entity != null)
                {
                    handler(db, entity);
                    await db.SaveChangesAsync();
                }
            }
        }

        public async Task HandleAsync(ExpenseTemplateCreated payload)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                db.ExpenseTemplates.Add(new ExpenseTemplateEntity(payload).SetUserKey(payload.UserKey));
                await db.SaveChangesAsync();
            }
        }

        public Task HandleAsync(ExpenseTemplateAmountChanged payload) => UpdateAsync(payload, e =>
        {
            if (payload.NewValue != null)
            {
                e.Amount = payload.NewValue.Value;
                e.Currency = payload.NewValue.Currency;
            }
            else
            {
                e.Amount = null;
                e.Currency = null;
            }
        });

        public Task HandleAsync(ExpenseTemplateDescriptionChanged payload) => UpdateAsync(payload, e => e.Description = payload.Description);

        public Task HandleAsync(ExpenseTemplateCategoryChanged payload) => UpdateAsync(payload, e =>
        {
            if (payload.CategoryKey.IsEmpty)
                e.CategoryId = null;
            else
                e.CategoryId = payload.CategoryKey.AsGuidKey().Guid;
        });

        public Task HandleAsync(ExpenseTemplateFixedChanged payload) => UpdateAsync(payload, e => e.IsFixed = payload.IsFixed);

        public Task HandleAsync(ExpenseTemplateDeleted payload) => UpdateAsync(payload, (db, e) => 
        {
            e.IsDeleted = true;
            if (payload.Version >= 2)
                e.DeletedAt = payload.DeletedAt;
        });

        public Task HandleAsync(ExpenseTemplateRecurrenceChanged payload) => UpdateAsync(payload, e =>
        {
            e.Period = payload.Period;
            e.DayInPeriod = payload.DayInPeriod;
            e.DueDate = payload.DueDate;
        });

        public Task HandleAsync(ExpenseTemplateRecurrenceCleared payload) => UpdateAsync(payload, e =>
        {
            e.Period = null;
            e.DayInPeriod = null;
            e.DueDate = null;
        });

        public async Task<List<ExpenseTemplateModel>> HandleAsync(ListAllExpenseTemplate query)
        {
            using (ReadModelContext db = dbFactory.Create())
            {
                var sql = db.ExpenseTemplates
                    .WhereUserKey(query)
                    .Where(e => e.IsDeleted == false)
                    .OrderBy(e => e.Description)
                    .ThenBy(e => e.CategoryId);

                List<ExpenseTemplateModel> models = await sql
                    .Select(e => e.ToModel(query.Version))
                    .ToListAsync();

                return models;
            }
        }
    }
}
