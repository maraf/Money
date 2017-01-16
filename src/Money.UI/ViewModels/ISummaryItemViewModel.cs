using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// A contract for summary item.
    /// </summary>
    public interface ISummaryItemViewModel
    {
        /// <summary>
        /// Gets a category key of the item.
        /// </summary>
        IKey CategoryKey { get; }

        /// <summary>
        /// Gets a total amount of the item.
        /// </summary>
        Price Amount { get; }
    }
}
