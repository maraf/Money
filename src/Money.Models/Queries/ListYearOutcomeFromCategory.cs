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
    /// A query for getting all out comes from a category within an year.
    /// </summary>
    public class ListYearOutcomeFromCategory : UserQuery, IQuery<List<OutcomeOverviewModel>>, ISortableQuery<OutcomeOverviewSortType>, IPageableQuery
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
        /// <param name="year">A year to find outcomes from.</param>
        public ListYearOutcomeFromCategory(IKey categoryKey, YearModel year, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null, int? pageIndex = null)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Ensure.NotNull(year, "year");
            CategoryKey = categoryKey;
            Year = year;
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;
        }
    }
}
