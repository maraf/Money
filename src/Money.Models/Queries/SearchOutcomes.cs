using Money.Models.Sorting;
using Neptuo;
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
    public class SearchOutcomes : UserQuery, IQuery<List<OutcomeOverviewModel>>, ISortableQuery<OutcomeOverviewSortType>
    {
        /// <summary>
        /// Gets a phrase to search.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets a sorting descriptor.
        /// </summary>
        public SortDescriptor<OutcomeOverviewSortType> SortDescriptor { get; private set; }

        /// <summary>
        /// Gets a page index to load.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="text">A phrase to search.</param>
        /// <param name="sortDescriptor">A sorting descriptor.</param>
        /// <param name="pageIndex">A page index to load.</param>
        public SearchOutcomes(string text, SortDescriptor<OutcomeOverviewSortType> sortDescriptor, int pageIndex)
        {
            Ensure.NotNullOrEmpty(text, "text");
            Ensure.NotNull(sortDescriptor, "sortDescriptor");
            Ensure.PositiveOrZero(pageIndex, "pageIndex");
            Text = text;
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;
        }
    }
}
