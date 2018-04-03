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
    /// A query for getting category name and description.
    /// </summary>
    public class GetCategoryNameDescription : UserQuery, IQuery<CategoryNameDescriptionModel>
    {
        /// <summary>
        /// Gets a category key.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="categoryKey">A category key.</param>
        public GetCategoryNameDescription(IKey categoryKey)
        {
            Ensure.Condition.NotEmptyKey(categoryKey);
            CategoryKey = categoryKey;
        }
    }
}
