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
    /// A query for getting all categories.
    /// </summary>
    public class ListAllCategory : UserQuery, IQuery<List<CategoryModel>>
    {
        [CompositeVersion]
        public int Version { get; set; }

        [CompositeProperty(1, Version = 2)]
        public bool IncludeDeleted { get; set; }

        [CompositeConstructor(Version = 1)]
        public ListAllCategory()
        {
            Version = 1;
        }

        [CompositeConstructor(Version = 2)]
        public ListAllCategory(bool includeDeleted)
        {
            IncludeDeleted = includeDeleted;
            Version = 2;
        }
    }
}
