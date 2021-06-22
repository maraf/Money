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
    /// Changes an <see cref="Amount"/> of the income with <see cref="IncomeKey"/>.
    /// </summary>
    public class ChangeIncomeAmount : Command
    {
        /// <summary>
        /// Gets a key of the income to modify.
        /// </summary>
        public IKey IncomeKey { get; private set; }

        /// <summary>
        /// Gets a new income value.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Changes an <paramref name="amount"/> of the income with <paramref name="key"/>.
        /// </summary>
        /// <param name="incomeKey">A key of the income to modify.</param>
        /// <param name="amount">A new income value.</param>
        public ChangeIncomeAmount(IKey incomeKey, Price amount)
        {
            Ensure.Condition.NotEmptyKey(incomeKey);
            Ensure.NotNull(amount, "amount");
            IncomeKey = incomeKey;
            Amount = amount;
        }
    }
}
