using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for getting all out comes from a category within an year.
    /// </summary>
    public class ListYearOutcomeFromCategory : UserQuery, IQuery<IEnumerable<OutcomeOverviewModel>>
    {
        /// <summary>
        /// Gets a key of the category.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a year to find outcomes from.
        /// </summary>
        public YearModel Year { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="year">A year to find outcomes from.</param>
        public ListYearOutcomeFromCategory(IKey categoryKey, YearModel year)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Ensure.NotNull(year, "year");
            CategoryKey = categoryKey;
            Year = year;
        }
    }
}
