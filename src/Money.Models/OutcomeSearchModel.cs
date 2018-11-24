using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model of a single outcome for search.
    /// </summary>
    public class OutcomeSearchModel : OutcomeOverviewModel
    {
        /// <summary>
        /// Gets a key of a category.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Create a new instance.
        /// </summary>
        /// <param name="key">A key of the outcome.</param>
        /// <param name="amount">An amount of the outcome.</param>
        /// <param name="when">A date when the outcome ocured.</param>
        /// <param name="description">A description of the outcome.</param>
        /// <param name="categoryKey">A key of a category.</param>
        public OutcomeSearchModel(IKey key, Price amount, DateTime when, string description, IKey categoryKey)
            : base(key, amount, when, description)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
        }
    }
}
