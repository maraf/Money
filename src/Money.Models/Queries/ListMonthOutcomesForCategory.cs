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
    public class ListMonthOutcomesForCategory : UserQuery, IQuery<List<MonthWithAmountModel>>
    {
        public IKey CategoryKey { get; private set; }
        public YearModel Year { get; private set; }

        public ListMonthOutcomesForCategory(IKey categoryKey, YearModel year)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            Ensure.NotNull(year, "year");
            CategoryKey = categoryKey;
            Year = year;
        }
    }
}
