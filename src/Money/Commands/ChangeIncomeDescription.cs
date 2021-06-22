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
    /// Changes a <see cref="Description"/> of the income with <see cref="IncomeKey"/>.
    /// </summary>
    public class ChangeIncomeDescription : Command
    {
        /// <summary>
        /// Gets a key of the income to modify.
        /// </summary>
        public IKey IncomeKey { get; private set; }

        /// <summary>
        /// Gets a new description of the income.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Changes a <paramref name="description"/> of the income with <paramref name="incomeKey"/>.
        /// </summary>
        /// <param name="incomeKey">A key of the income to modify.</param>
        /// <param name="description">A new description of the income.</param>
        public ChangeIncomeDescription(IKey incomeKey, string description)
        {
            Ensure.Condition.NotEmptyKey(incomeKey);
            Ensure.NotNull(description, "description");
            IncomeKey = incomeKey;
            Description = description;
        }
    }
}
