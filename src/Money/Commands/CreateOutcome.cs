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
    /// Creates an outcome.
    /// </summary>
    public class CreateOutcome : Command
    {
        /// <summary>
        /// Gets an amount of the outcome.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a description of the outcome.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a date and time when the outcome occured.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Gets a category key when outcome will be created.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Creates an outcome.
        /// </summary>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="description">A description of the outcome.</param>
        /// <param name="when">A date and time when the outcome occured.</param>
        public CreateOutcome(Price amount, string description, DateTime when, IKey categoryKey)
        {
            Ensure.NotNull(amount, "amount");
            Ensure.NotNull(description, "description");
            Ensure.NotNull(when, "when");
            Ensure.Condition.NotEmptyKey(categoryKey);
            Amount = amount;
            Description = description;
            When = when;
            CategoryKey = categoryKey;
        }
    }
}
