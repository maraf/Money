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
    /// An enumeration of supported sorting of expense templates.
    /// </summary>
    public enum ExpenseTemplateSortType
    {
        /// <summary>
        /// Sort items by amount.
        /// </summary>
        [Description("By Amount")]
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
        ByDescription
    }
}
