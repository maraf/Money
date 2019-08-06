using Neptuo;
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
    /// A query for getting list of categories with year outcome summary.
    /// </summary>
    public class ListYearCategoryWithOutcome : UserQuery, IQuery<List<CategoryWithAmountModel>>
    {
        public YearModel Year { get; private set; }

        public ListYearCategoryWithOutcome(YearModel year)
        {
            Ensure.NotNull(year, "year");
            Year = year;
        }
    }
}
