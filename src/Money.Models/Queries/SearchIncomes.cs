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
    /// A query for searching in incomes.
    /// </summary>
    public class SearchIncomes : UserQuery, IQuery<List<IncomeOverviewModel>>, ISortableQuery<IncomeOverviewSortType>, IPageableQuery
    {
        /// <summary>
        /// Gets a phrase to search.
        /// </summary>
        [CompositeProperty(0, Version = 1)]
        public string Text { get; private set; }

        /// <summary>
        /// Gets a sorting descriptor.
        /// </summary>
        [CompositeProperty(1, Version = 1)]
        public SortDescriptor<IncomeOverviewSortType> SortDescriptor { get; private set; }

        /// <summary>
        /// Gets a page index to load.
        /// </summary>
        [CompositeProperty(2, Version = 1)]
        public int PageIndex { get; private set; }

        [CompositeVersion]
        [CompositeProperty(3, Version = 1)]
        public int Version { get; private set; }

        int? IPageableQuery.PageIndex => PageIndex;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="text">A phrase to search.</param>
        /// <param name="sortDescriptor">A sorting descriptor.</param>
        /// <param name="pageIndex">A page index to load.</param>
        [CompositeConstructor(Version = 1)]
        public SearchIncomes(string text, SortDescriptor<IncomeOverviewSortType> sortDescriptor, int pageIndex, int version = 1)
        {
            Ensure.NotNullOrEmpty(text, "text");
            Ensure.NotNull(sortDescriptor, "sortDescriptor");
            Ensure.PositiveOrZero(pageIndex, "pageIndex");
            Text = text.Trim();
            SortDescriptor = sortDescriptor;
            PageIndex = pageIndex;

            Version = version;
        }

        public static SearchIncomes Version1(string text, SortDescriptor<IncomeOverviewSortType> sortDescriptor, int pageIndex)
        {
            return new SearchIncomes(text, sortDescriptor, pageIndex, 1);
        }
    }
}
