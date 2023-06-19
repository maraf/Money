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
    /// Changes a <see cref="IsFixed"/> of the expense template with <see cref="ExpenseTemplateKey"/>.
    /// </summary>
    public class ChangeExpenseTemplateFixed : Command
    {
        /// <summary>
        /// Gets a key of the expense template to modify.
        /// </summary>
        public IKey ExpenseTemplateKey { get; private set; }

        /// <summary>
        /// Gets whether the expense template should produce fixed expenses.
        /// </summary>
        public bool IsFixed { get; private set; }

        /// <summary>
        /// Changes a <paramref name="isFixed"/> of the expense template with <paramref name="expenseTemplateKey"/>.
        /// </summary>
        /// <param name="expenseTemplateKey">A key of the expense template to modify.</param>
        /// <param name="isFixed">Whether the expense template should produce fixed expenses.</param>
        public ChangeExpenseTemplateFixed(IKey expenseTemplateKey, bool isFixed)
        {
            Ensure.Condition.NotEmptyKey(expenseTemplateKey);
            ExpenseTemplateKey = expenseTemplateKey;
            IsFixed = isFixed;
        }
    }
}
