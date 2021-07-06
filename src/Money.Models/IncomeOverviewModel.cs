using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of a single income for overview.
    /// </summary>
    public class IncomeOverviewModel
    {
        /// <summary>
        /// Gets a key of the income.
        /// </summary>
        public IKey Key { get; private set; }

        /// <summary>
        /// Gets an amount of the income.
        /// </summary>
        public Price Amount { get; set; }

        /// <summary>
        /// Gets a date when the income ocured.
        /// </summary>
        public DateTime When { get; set; }

        /// <summary>
        /// Gets a description of the income.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="key">A key of the income.</param>
        /// <param name="amount">An amount of the income.</param>
        /// <param name="when">A date when the income ocured.</param>
        /// <param name="description">A description of the income.</param> 
        public IncomeOverviewModel(IKey key, Price amount, DateTime when, string description)
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
