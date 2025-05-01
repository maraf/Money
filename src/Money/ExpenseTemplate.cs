using Money.Events;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Domains;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    /// <summary>
    /// Model of expense template.
    /// </summary>
    public class ExpenseTemplate : AggregateRoot,
        IEventHandler<ExpenseTemplateCreated>,
        IEventHandler<ExpenseTemplateAmountChanged>,
        IEventHandler<ExpenseTemplateDescriptionChanged>,
        IEventHandler<ExpenseTemplateCategoryChanged>,
        IEventHandler<ExpenseTemplateFixedChanged>,
        IEventHandler<ExpenseTemplateRecurrenceChanged>,
        IEventHandler<ExpenseTemplateRecurrenceCleared>,
        IEventHandler<ExpenseTemplateDeleted>
    {
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Gets an amount of the expense template.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the expense template.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a category of the expense template.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets whether the template should create fixed expenses.
        /// </summary>
        public bool IsFixed { get; private set; }

        public RecurrencePeriod? Period { get; set; }

        public int? DayInPeriod { get; set; }
        public DateTime? DueDate { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param>
        /// <param name="categoryKey">A category of the expense template.</param>
        public ExpenseTemplate(Price amount, string description, IKey categoryKey)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Publish(new ExpenseTemplateCreated(amount, description, categoryKey));
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="amount">An amount of the expense template.</param>
        /// <param name="description">A description of the expense template.</param>
        /// <param name="categoryKey">A category of the expense template.</param>
        public ExpenseTemplate(Price amount, string description, IKey categoryKey, bool isFixed)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Publish(new ExpenseTemplateCreated(amount, description, categoryKey, isFixed, AppDateTime.Today));
        }

        public ExpenseTemplate(IKey key, IEnumerable<IEvent> events)
            : base(key, events)
        { }

        Task IEventHandler<ExpenseTemplateCreated>.HandleAsync(ExpenseTemplateCreated payload) => UpdateState(() =>
        {
            Amount = payload.Amount;
            Description = payload.Description;
            CategoryKey = payload.CategoryKey;
            IsFixed = payload.IsFixed;

            if (payload.CompositeVersion >= 3)
                CreatedAt = payload.CreatedAt;
        });

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
                throw new ExpenseTemplateAlreadyDeletedException();
        }

        public void Delete()
        {
            EnsureNotDeleted();
            Publish(new ExpenseTemplateDeleted(AppDateTime.Today));
        }

        Task IEventHandler<ExpenseTemplateDeleted>.HandleAsync(ExpenseTemplateDeleted payload) => UpdateState(() =>
        {
            IsDeleted = true;

            if (payload.CompositeVersion >= 2)
                DeletedAt = payload.DeletedAt;
        });

        public void ChangeAmount(Price amount)
        {
            EnsureNotDeleted();
            Publish(new ExpenseTemplateAmountChanged(Amount, amount));
        }

        Task IEventHandler<ExpenseTemplateAmountChanged>.HandleAsync(ExpenseTemplateAmountChanged payload) => UpdateState(() =>
        {
            Amount = payload.NewValue;
        });

        public void ChangeDescription(string description)
        {
            EnsureNotDeleted();
            Publish(new ExpenseTemplateDescriptionChanged(description));
        }

        Task IEventHandler<ExpenseTemplateDescriptionChanged>.HandleAsync(ExpenseTemplateDescriptionChanged payload) => UpdateState(() =>
        {
            Description = payload.Description;
        });

        public void ChangeCategory(IKey categoryKey)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            EnsureNotDeleted();
            Publish(new ExpenseTemplateCategoryChanged(categoryKey));
        }

        Task IEventHandler<ExpenseTemplateCategoryChanged>.HandleAsync(ExpenseTemplateCategoryChanged payload) => UpdateState(() =>
        {
            CategoryKey = payload.CategoryKey;
        });

        public void ChangeFixed(bool isFixed)
        {
            EnsureNotDeleted();
            if (isFixed == IsFixed)
                throw new ExpenseTemplateAlreadyFixedException();

            Publish(new ExpenseTemplateFixedChanged(isFixed));
        }

        Task IEventHandler<ExpenseTemplateFixedChanged>.HandleAsync(ExpenseTemplateFixedChanged payload) => UpdateState(() =>
        {
            IsFixed = payload.IsFixed;
        });

        public void SetMonthlyRecurrence(int dayInPeriod)
        {
            EnsureNotDeleted();
            Publish(new ExpenseTemplateRecurrenceChanged(RecurrencePeriod.Monthly, dayInPeriod: dayInPeriod, dueDate: null));
        }

        public void SetXMonthsRecurrence(int everyXPeriods, int dayInPeriod)
        {
            EnsureNotDeleted();
            Publish(new ExpenseTemplateRecurrenceChanged(RecurrencePeriod.XMonths, everyXPeriods: everyXPeriods, dayInPeriod: dayInPeriod));
        }

        public void SetYearlyRecurrence(int monthInPeriod, int dayInPeriod)
        {
            EnsureNotDeleted();
            Publish(new ExpenseTemplateRecurrenceChanged(RecurrencePeriod.Yearly, monthInPeriod: monthInPeriod, dayInPeriod: dayInPeriod));
        }

        public void SetSingleRecurrence(DateTime dueDate)
        {
            EnsureNotDeleted();
            Publish(new ExpenseTemplateRecurrenceChanged(RecurrencePeriod.Single, dayInPeriod: null, dueDate: dueDate));
        }

        Task IEventHandler<ExpenseTemplateRecurrenceChanged>.HandleAsync(ExpenseTemplateRecurrenceChanged payload) => UpdateState(() =>
        {
            Period = payload.Period;
            DayInPeriod = payload.DayInPeriod;
            DueDate = payload.DueDate;
        });

        public void ClearRecurrence()
        {
            EnsureNotDeleted();
            Publish(new ExpenseTemplateRecurrenceCleared());
        }

        Task IEventHandler<ExpenseTemplateRecurrenceCleared>.HandleAsync(ExpenseTemplateRecurrenceCleared payload) => UpdateState(() =>
        {
            Period = null;
            DayInPeriod = null;
            DueDate = null;
        });
    }
}
