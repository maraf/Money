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
    /// Changes an <see cref="Amount"/> of the outcome with <see cref="OutcomeKey"/>.
    /// </summary>
    public class ChangeOutcomeAmount : Command
    {
        /// <summary>
        /// Gets a key of the outcome to modify.
        /// </summary>
        public IKey OutcomeKey { get; private set; }

        /// <summary>
        /// Gets a new outcome value.
        /// </summary>
        public Price Amount { get; private set; }

        public ChangeOutcomeAmount(IKey outcomeKey, Price amount)
        {
            Ensure.Condition.NotEmptyKey(outcomeKey);
            Ensure.NotNull(amount, "amount");
            OutcomeKey = outcomeKey;
            Amount = amount;
        }
    }
}
