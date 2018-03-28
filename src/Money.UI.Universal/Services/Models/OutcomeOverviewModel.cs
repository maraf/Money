using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Models
{
    /// <summary>
    /// A model of a single outcome for overview.
    /// </summary>
    public class OutcomeOverviewModel
    {
        /// <summary>
        /// Gets a key of the outcome.
        /// </summary>
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets an amount of the outcome.
        /// </summary>
        public Price Amount { get; private set; }

        /// <summary>
        /// Gets a date when the outcome ocured.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets a description of the outcome.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="key">A key of the outcome.</param>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="when">A date when the outcome ocured.</param>
        /// <param name="description">A description of the outcome.</param>
        public OutcomeOverviewModel(IKey key, Price amount, DateTime when, string description)
        {
            Ensure.Condition.NotEmptyKey(key);
            Ensure.NotNull(amount, "amount");
            Key = key;
            Amount = amount;
            When = when;
            Description = description;
        }
    }
}
