using Neptuo;
using Neptuo.Formatters.Metadata;
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
        [CompositeProperty(1, Version = 1)]
        [CompositeProperty(1, Version = 2)]
        public YearModel Year { get; private set; }

        [CompositeProperty(2, Version = 2)]
        public bool IncludeExpectedExpenses { get; private set; }

        [CompositeVersion]
        public int Version { get; private set; }

        [CompositeConstructor(Version = 1)]
        public ListMonthBalance(YearModel year)
        {
            Ensure.NotNull(year, "year");
            Year = year;
            Version = 1;
        }

        [CompositeConstructor(Version = 2)]
        public ListMonthBalance(YearModel year, bool includeExpectedExpenses)
            : this(year)
        {
            IncludeExpectedExpenses = includeExpectedExpenses;
            Version = 2;
        }
    }
}
