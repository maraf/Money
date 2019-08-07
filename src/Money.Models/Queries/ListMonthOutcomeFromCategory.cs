using Money.Models.Sorting;
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
    /// A query for getting all out comes from a category within a month.
    /// </summary>
    public class ListMonthOutcomeFromCategory : UserQuery, IQuery<List<OutcomeOverviewModel>>, ISortableQuery<OutcomeOverviewSortType>, IPageableQuery
    {
        /// <summary>
        /// Gets a key of the category.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a month to find outcomes from.
        /// </summary>
        public MonthModel Month { get; private set; }

        /// <summary>
        /// Gets a sorting descriptor.
        /// </summary>
        public SortDescriptor<OutcomeOverviewSortType> SortDescriptor { get; private set; }

        /// <summary>
        /// Gets a page index to load.
        /// If <c>null</c>, load all results.
        /// </summary>
        public int? PageIndex { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="month">A month to find outcomes from.</param>
        /// <param name="sortDescriptor">A sorting descriptor.</param>
        /// <param name="pageIndex">A page index to load. If <c>null</c>, load all results.</param>
        public ListMonthOutcomeFromCategory(IKey categoryKey, MonthModel month, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null, int? pageIndex = null)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Ensure.NotNull(month, "month");
            CategoryKey = categoryKey;
            Month = month;
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;
        }
    }
}
