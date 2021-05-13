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
    /// A query for getting all out comes from a category within an year.
    /// </summary>
    public class ListYearOutcomeFromCategory : UserQuery, IQuery<List<OutcomeOverviewModel>>, ISortableQuery<OutcomeOverviewSortType>, IPageableQuery
    {
        /// <summary>
        /// Gets a key of the category.
        /// </summary>
        [CompositeProperty(0, Version = 1)]
        [CompositeProperty(0, Version = 2)]
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a year to find outcomes from.
        /// </summary>
        [CompositeProperty(1, Version = 1)]
        [CompositeProperty(1, Version = 2)]
        public YearModel Year { get; private set; }

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
        /// <param name="year">A year to find outcomes from.</param>
        [CompositeConstructor(Version = 1)]
        public ListYearOutcomeFromCategory(IKey categoryKey, YearModel year, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null, int? pageIndex = null)
            : this(categoryKey, year, sortDescriptor, pageIndex, 1)
        { }

        [CompositeConstructor(Version = 2)]
        public ListYearOutcomeFromCategory(IKey categoryKey, YearModel year, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null, int? pageIndex = null, int version = 2)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Ensure.NotNull(year, "year");
            CategoryKey = categoryKey;
            Year = year;
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;

            Version = version;
        }

        public static ListYearOutcomeFromCategory Version2(IKey categoryKey, YearModel year, SortDescriptor<OutcomeOverviewSortType> sortDescriptor = null, int? pageIndex = null)
        {
            return new ListYearOutcomeFromCategory(categoryKey, year, sortDescriptor, pageIndex, 2);
        }
    }
}
