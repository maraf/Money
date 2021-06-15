using Money.Models.Sorting;
using Neptuo;
using Neptuo.Formatters.Metadata;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for searching in outcomes.
    /// </summary>
    public class SearchOutcomes : UserQuery, IQuery<List<OutcomeOverviewModel>>, ISortableQuery<OutcomeOverviewSortType>, IPageableQuery
    {
        /// <summary>
        /// Gets a phrase to search.
        /// </summary>
        [CompositeProperty(0, Version = 1)]
        [CompositeProperty(0, Version = 2)]
        public string Text { get; private set; }

        /// <summary>
        /// Gets a sorting descriptor.
        /// </summary>
        [CompositeProperty(1, Version = 1)]
        [CompositeProperty(1, Version = 2)]
        public SortDescriptor<OutcomeOverviewSortType> SortDescriptor { get; private set; }

        /// <summary>
        /// Gets a page index to load.
        /// </summary>
        [CompositeProperty(2, Version = 1)]
        [CompositeProperty(2, Version = 2)]
        public int PageIndex { get; private set; }

        [CompositeVersion]
        [CompositeProperty(3, Version = 2)]
        public int Version { get; private set; }

        int? IPageableQuery.PageIndex => PageIndex;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="text">A phrase to search.</param>
        /// <param name="sortDescriptor">A sorting descriptor.</param>
        /// <param name="pageIndex">A page index to load.</param>
        [CompositeConstructor(Version = 1)]
        public SearchOutcomes(string text, SortDescriptor<OutcomeOverviewSortType> sortDescriptor, int pageIndex)
            : this(text, sortDescriptor, pageIndex, 1)
        { }

        [CompositeConstructor(Version = 2)]
        public SearchOutcomes(string text, SortDescriptor<OutcomeOverviewSortType> sortDescriptor, int pageIndex, int version = 2)
        {
            Ensure.NotNullOrEmpty(text, "text");
            Ensure.NotNull(sortDescriptor, "sortDescriptor");
            Ensure.PositiveOrZero(pageIndex, "pageIndex");
            Text = text;
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;

            Version = version;
        }

        public static SearchOutcomes Version2(string text, SortDescriptor<OutcomeOverviewSortType> sortDescriptor, int pageIndex)
        {
            return new SearchOutcomes(text, sortDescriptor, pageIndex, 2);
        }
    }
}
