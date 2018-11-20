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
    public class SearchOutcomes : UserQuery, IQuery<List<OutcomeSearchModel>>
    {
        /// <summary>
        /// Gets a phrase to search.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets a page index to load.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="text">A phrase to search.</param>
        /// <param name="pageIndex">A page index to load.</param>
        public SearchOutcomes(string text, int pageIndex)
        {
            Ensure.NotNullOrEmpty(text, "text");
            Text = text;
            PageIndex = pageIndex;
        }
    }
}
