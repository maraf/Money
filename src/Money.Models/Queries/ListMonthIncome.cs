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
    /// A query for getting list of incomes in a month.
    /// </summary>
    public class ListMonthIncome : UserQuery, IQuery<List<IncomeOverviewModel>>, ISortableQuery<IncomeOverviewSortType>, IPageableQuery
    {
        public MonthModel Month { get; private set; }
        public SortDescriptor<IncomeOverviewSortType> SortDescriptor { get; private set; }
        public int? PageIndex { get; private set; }

        public ListMonthIncome(MonthModel month, SortDescriptor<IncomeOverviewSortType> sortDescriptor = null, int? pageIndex = null)
        {
            Ensure.NotNull(month, "month");
            Month = month;
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;
        }
    }
}
