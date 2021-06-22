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
    /// Changes a <see cref="When"/> of the income with <see cref="IncomeKey"/>.
    /// </summary>
    public class ChangeIncomeWhen : Command
    {
        /// <summary>
        /// Gets a key of the income to modify.
        /// </summary>
        public IKey IncomeKey { get; private set; }

        /// <summary>
        /// Gets a date when the income occured.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Changes a <paramref name="when"/> of the income with <paramref name="incomeKey"/>.
        /// </summary>
        /// <param name="incomeKey">A key of the income to modify.</param>
        /// <param name="when">A date when the income occured.</param>
        public ChangeIncomeWhen(IKey incomeKey, DateTime when)
        {
            Ensure.Condition.NotEmptyKey(incomeKey);
            Ensure.NotNull(when, "when");
            IncomeKey = incomeKey;
            When = when;
        }
    }
}
