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
    /// An event raised when category is added to an existing outcome.
    /// </summary>
    public class OutcomeCategoryAdded : UserEvent
    {
        /// <summary>
        /// Gets a key of the category assigned to the outcome.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        internal OutcomeCategoryAdded(IKey categoryKey)
        {
            CategoryKey = categoryKey;
        }
    }
}
