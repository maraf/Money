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
    /// A query for getting category name.
    /// </summary>
    public class GetCategoryName : UserQuery, IQuery<string>
    {
        /// <summary>
        /// Gets a key of the category.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="categoryKey">A key of the category.</param>
        public GetCategoryName(IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
        }
    }
}
