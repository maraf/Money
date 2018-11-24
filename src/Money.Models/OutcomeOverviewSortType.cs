using Money.Models.Sorting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// An enumeration of supported sorting of overview items.
    /// </summary>
    public enum OutcomeOverviewSortType
    {
        /// <summary>
        /// Sort items by amount.
        /// </summary>
        [Description("By Amount")]
        [DefaultValue(SortDirection.Descending)]
        ByAmount,

        /// <summary>
        /// Sort items by category name.
        /// </summary>
        [Description("By Category")]
        ByCategory,

        /// <summary>
        /// Sort items by description.
        /// </summary>
        [Description("By Description")]
        ByDescription,

        /// <summary>
        /// Sort items by creation date.
        /// </summary>
        [Description("By Creation Date")]
        ByWhen
    }
}
