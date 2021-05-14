using Money.Models.Sorting;
using Neptuo;
using Neptuo.Formatters.Metadata;
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
        [CompositeProperty(0, Version = 1)]
        [CompositeProperty(0, Version = 2)]
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a month to find outcomes from.
        /// </summary>
        [CompositeProperty(1, Version = 1)]
        [CompositeProperty(1, Version = 2)]
        public MonthModel Month { get; private set; }

        /// <summary>
        /// Gets a sorting descriptor.
        /// </summary>
        [CompositeProperty(2, Version = 1)]
        [CompositeProperty(2, Version = 2)]
        public SortDescriptor<OutcomeOverviewSortType> SortDescriptor { get; private set; }

        /// <summary>
        /// Gets a page index to load.
        /// If <c>null</c>, load all results.
        /// </summary>
        [CompositeProperty(3, Version = 1)]
        [CompositeProperty(3, Version = 2)]
        public int? PageIndex { get; private set; }

        [CompositeVersion]
        [CompositeProperty(4, Version = 2)]
        public int Version { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="month">A month to find outcomes from.</param>
        /// <param name="sortDescriptor">A sorting descriptor.</param>
        /// <param name="pageIndex">A page index to load. If <c>null</c>, load all results.</param>
        [CompositeConstructor(Version = 1)]
        public ListMonthOutcomeFromCategory(IKey categoryKey, MonthModel month, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null, int? pageIndex = null)
            : this(categoryKey, month, sortDescriptor, pageIndex, 1)
        { }

        [CompositeConstructor(Version = 2)]
        public ListMonthOutcomeFromCategory(IKey categoryKey, MonthModel month, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null, int? pageIndex = null, int version = 2)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Ensure.NotNull(month, "month");
            CategoryKey = categoryKey;
            Month = month;
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;
            Version = version;
        }

        /// <summary>
        /// Creates a new instance which returns object in version 2.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        /// <param name="month">A month to find outcomes from.</param>
        /// <param name="sortDescriptor">A sorting descriptor.</param>
        /// <param name="pageIndex">A page index to load. If <c>null</c>, load all results.</param>
        public static ListMonthOutcomeFromCategory Version2(IKey categoryKey, MonthModel month, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null, int? pageIndex = null)
        {
            return new ListMonthOutcomeFromCategory(categoryKey, month, sortDescriptor, pageIndex, 2);
        }
    }
}
