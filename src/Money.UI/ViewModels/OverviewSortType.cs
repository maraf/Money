using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// An enumeration of suppported overview sorting types.
    /// </summary>
    public enum OverviewSortType
    {
        /// <summary>
        /// Sort items by creation date.
        /// </summary>
        ByDate,

        /// <summary>
        /// Sort items by amount.
        /// </summary>
        ByAmount,

        /// <summary>
        /// Sort items by description.
        /// </summary>
        ByDescription
    }
}
