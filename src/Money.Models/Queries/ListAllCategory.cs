using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for getting all categories.
    /// </summary>
    public class ListAllCategory : UserQuery, IQuery<List<CategoryModel>>
    {
        /// <summary>
        /// Gets or sets whether to include deleted categories.
        /// </summary>
        /// <value>
        /// <c>true</c> = only deleted;
        /// <c>false</c> (default) = only not deleted;
        /// <c>null</c> = all;
        /// </value>
        public bool? IsDeleted { get; private set; }

        public ListAllCategory(bool? isDeleted = false)
        {
            IsDeleted = isDeleted;
        }
    }
}
