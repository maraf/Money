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
    /// A query for getting a expense/income balance for year.
    /// </summary>
    public class ListMonthBalance : UserQuery, IQuery<List<MonthBalanceModel>>
    {
        public YearModel Year { get; private set; }

        public ListMonthBalance(YearModel year)
        {
            Ensure.NotNull(year, "year");
            Year = year;
        }
    }
}
