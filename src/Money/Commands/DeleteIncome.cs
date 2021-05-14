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
    /// Deletes an income.
    /// </summary>
    public class DeleteIncome : Command
    {
        /// <summary>
        /// Gets a key of the income to delete.
        /// </summary>
        public IKey IncomeKey { get; private set; }

        /// <summary>
        /// Deletes an outcome with <paramref name="incomeKey"/>.
        /// </summary>
        /// <param name="incomeKey">A key of the income to delete.</param>
        public DeleteIncome(IKey incomeKey)
        {
            Ensure.Condition.NotEmptyKey(incomeKey);
            IncomeKey = incomeKey;
        }
    }
}
