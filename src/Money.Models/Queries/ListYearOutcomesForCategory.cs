using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for getting list of yearly grouped expenses in a single category.
    /// It returns StartYear + 9 upcoming years.
    /// </summary>
    public class ListYearOutcomesForCategory : UserQuery, IQuery<List<YearWithAmountModel>>
    {
        public IKey CategoryKey { get; private set; }
        public YearModel StartYear { get; private set; }

        public ListYearOutcomesForCategory(IKey categoryKey, YearModel startYear)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            Ensure.NotNull(startYear, "startYear");
            CategoryKey = categoryKey;
            StartYear = startYear;
        }
    }
}
