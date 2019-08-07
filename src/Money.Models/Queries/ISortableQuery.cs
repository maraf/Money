using Money.Models.Sorting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query that can sort.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISortableQuery<T>
    {
        /// <summary>
        /// Gets a sort description.
        /// </summary>
        SortDescriptor<T> SortDescriptor { get; }
    }
}
