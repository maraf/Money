using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Changes a <see cref="When"/> of the expense template with <see cref="ExpenseTemplateKey"/>.
    /// </summary>
    public class ChangeExpenseTemplateWhen : Command
    {
        /// <summary>
        /// Gets a key of the expense template to modify.
        /// </summary>
        public IKey ExpenseTemplateKey { get; private set; }

        /// <summary>
        /// Gets a date when the expense template occured.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Changes a <paramref name="when"/> of the expense template with <paramref name="expenseTemplateKey"/>.
        /// </summary>
        /// <param name="expenseTemplateKey">A key of the expense template to modify.</param>
        /// <param name="when">A date when the expense template occured.</param>
        public ChangeExpenseTemplateWhen(IKey expenseTemplateKey, DateTime when)
        {
            Ensure.Condition.NotEmptyKey(expenseTemplateKey);
            Ensure.NotNull(when, "when");
            ExpenseTemplateKey = expenseTemplateKey;
            When = when;
        }
    }
}
