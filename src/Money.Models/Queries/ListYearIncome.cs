using Money.Models.Sorting;
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
    /// A query for getting list of incomes in a year.
    /// </summary>
    public class ListYearIncome : UserQuery, IQuery<List<IncomeOverviewModel>>, ISortableQuery<IncomeOverviewSortType>, IPageableQuery
    {
        public YearModel Year { get; private set; }
        public SortDescriptor<IncomeOverviewSortType> SortDescriptor { get; private set; }
        public int? PageIndex { get; private set; }

        public ListYearIncome(YearModel year, SortDescriptor<IncomeOverviewSortType> sortDescriptor = null, int? pageIndex = null)
        {
            Ensure.NotNull(year, "year");
            Year = year;
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;
        }
    }
}
