using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("By Date")]
        ByDate,

        /// <summary>
        /// Sort items by amount.
        /// </summary>
        [DisplayName("By Amount")]
        [DefaultValue(SortDirection.Descending)]
        ByAmount,

        /// <summary>
        /// Sort items by description.
        /// </summary>
        [DisplayName("By Description")]
        ByDescription
    }
}
