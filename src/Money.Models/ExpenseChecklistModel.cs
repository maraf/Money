using Neptuo;
using Neptuo.Formatters.Metadata;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model for a single item in monthly expense checklist
    /// </summary>
    public class ExpenseChecklistModel : IExpenseOverviewModel
    {
        [CompositeVersion]
        public int Version { get; private set; }

        /// <summary>
        /// Gets an key of expense template.
        /// </summary>
        [CompositeProperty(1)]
        [CompositeProperty(1, Version = 2)]
        public IKey ExpenseTemplateKey { get; private set; }

        /// <summary>
        /// Gets an key of already existing expense if such exist.
        /// This is the determinator whether the item is checked.
        /// </summary>
        [CompositeProperty(2)]
        [CompositeProperty(2, Version = 2)]
        public IKey ExpenseKey { get; private set; }
        IKey IExpenseOverviewModel.Key => ExpenseKey;

        /// <summary>
        /// Gets an amount.
        /// </summary>
        [CompositeProperty(3)]
        [CompositeProperty(3, Version = 2)]
        public Price Amount { get; set; }

        /// <summary>
        /// Gets expense creation date or due date for the template.
        /// </summary>
        [CompositeProperty(4)]
        [CompositeProperty(4, Version = 2)]
        public DateTime When { get; set; }

        /// <summary>
        /// Gets the expected date for the expense, if different from the actual date.
        /// </summary>
        [CompositeProperty(5, Version = 2)]
        public DateTime? ExpectedWhen { get; set; }

        DateTime? IExpenseOverviewModel.ExpectedWhen => ExpectedWhen;

        /// <summary>
        /// Gets expense category.
        /// </summary>
        [CompositeProperty(5)]
        [CompositeProperty(6, Version = 2)]
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a description.
        /// </summary>
        [CompositeProperty(6)]
        [CompositeProperty(7, Version = 2)]
        public string Description { get; set; }

        /// <summary>
        /// Gets whether the expense is fixed.
        /// </summary>
        [CompositeProperty(7)]
        [CompositeProperty(8, Version = 2)]
        public bool IsFixed { get; set; }

        [CompositeConstructor]
        public ExpenseChecklistModel(IKey expenseTemplateKey, IKey expenseKey, Price amount, DateTime when, IKey categoryKey, string description, bool isFixed)
        {
            Ensure.Condition.NotEmptyKey(expenseTemplateKey);
            Ensure.NotNull(expenseKey, "expenseKey");
            Ensure.NotNull(categoryKey, "categoryKey");
            ExpenseTemplateKey = expenseTemplateKey;
            ExpenseKey = expenseKey;
            Amount = amount;
            When = when;
            CategoryKey = categoryKey;
            Description = description;
            IsFixed = isFixed;
            Version = 1;
        }

        [CompositeConstructor(Version = 2)]
        public ExpenseChecklistModel(IKey expenseTemplateKey, IKey expenseKey, Price amount, DateTime when, DateTime? expectedWhen, IKey categoryKey, string description, bool isFixed)
            : this(expenseTemplateKey, expenseKey, amount, when, categoryKey, description, isFixed)
        {
            ExpectedWhen = expectedWhen;
            Version = 2;
        }
    }
}
