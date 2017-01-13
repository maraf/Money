using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// An enumeration of supported sorting of summary items.
    /// </summary>
    public enum SummarySortType
    {
        /// <summary>
        /// Sort items by amount.
        /// </summary>
        ByAmount,

        /// <summary>
        /// Sort items by category name.
        /// </summary>
        ByCategory
    }
}
