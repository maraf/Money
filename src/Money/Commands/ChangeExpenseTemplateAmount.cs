using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    /// <summary>
    /// Changes an <see cref="Amount"/> of the expense template with <see cref="ExpenseTemplateAmountKey"/>.
    /// </summary>
    public class ChangeExpenseTemplateAmount : Command, IExpenseTemplateCommand, IAggregateRootCommand
    {
        /// <summary>
        /// Gets a key of the expense template to modify.
        /// </summary>
        public IKey ExpenseTemplateKey { get; private set; }

        IKey IAggregateRootCommand.AggregateKey => ExpenseTemplateKey;

        /// <summary>
        /// Gets a new expense template amount value.
        /// A <c>null</c> value clears the template amount (no amount, nor currency).
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Changes an <paramref name="amount"/> of the expense template with <paramref name="key"/>.
        /// </summary>
        /// <param name="expenseTemplateKey">A key of the expense template to modify.</param>
        /// <param name="amount">A new expense template amount value, or <c>null</c> to clear the amount.</param>
        public ChangeExpenseTemplateAmount(IKey expenseTemplateKey, Price amount)
        {
            Ensure.Condition.NotEmptyKey(expenseTemplateKey);
            ExpenseTemplateKey = expenseTemplateKey;
            Amount = amount;
        }
    }
}
