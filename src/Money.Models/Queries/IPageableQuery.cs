using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query that can page.
    /// </summary>
    public interface IPageableQuery
    {
        /// <summary>
        /// Gets a page index.
        /// </summary>
        int? PageIndex { get; }
    }
}
