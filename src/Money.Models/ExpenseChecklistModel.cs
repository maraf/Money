using Neptuo;
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
    public class ExpenseChecklistModel
    {
        /// <summary>
        /// Gets an key of expense template.
        /// </summary>
        public IKey ExpenseTemplateKey { get; private set; }

        /// <summary>
        /// Gets an key of already existing expense if such exist.
        /// This is the determinator whether the item is checked.
        /// </summary>
        public IKey ExpenseKey { get; private set; }

        /// <summary>
        /// Gets an amount.
        /// </summary>
        public Price Amount { get; set; }

        /// <summary>
        /// Gets expense creation date or due date for the template.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets a description.
        /// </summary>
        public string Description { get; set; }

        public ExpenseChecklistModel(IKey expenseTemplateKey, IKey expenseKey, Price amount, DateTime when, string description)
        {
            Ensure.Condition.NotEmptyKey(expenseTemplateKey);
            Ensure.NotNull(expenseKey, "expenseKey");
            Ensure.NotNull(amount, "amount");
            ExpenseTemplateKey = expenseTemplateKey;
            ExpenseKey = expenseKey;
            Amount = amount;
            When = when;
            Description = description;
        }
    }
}
