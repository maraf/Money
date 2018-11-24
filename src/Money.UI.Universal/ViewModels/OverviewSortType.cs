using Money.Models.Sorting;
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
        [Description("By Date")]
        ByDate,

        /// <summary>
        /// Sort items by amount.
        /// </summary>
        [Description("By Amount")]
        [DefaultValue(SortDirection.Descending)]
        ByAmount,

        /// <summary>
        /// Sort items by description.
        /// </summary>
        [Description("By Description")]
        ByDescription
    }
}
