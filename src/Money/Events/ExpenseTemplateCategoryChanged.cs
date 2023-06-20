using Neptuo.Events;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when an date when the expense template occured has changed.
    /// </summary>
    public class ExpenseTemplateCategoryChanged : UserEvent
    {
        /// <summary>
        /// Gets a category key.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        internal ExpenseTemplateCategoryChanged(IKey categoryKey)
        {
            CategoryKey = categoryKey;
        }
    }
}
