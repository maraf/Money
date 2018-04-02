using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when after a category description is changed.
    /// </summary>
    public class CategoryDescriptionChanged : UserEvent
    {
        /// <summary>
        /// Gets an description of the category.
        /// </summary>
        public string Description { get; private set; }

        internal CategoryDescriptionChanged(string description)
        {
            Description = description;
        }
    }
}
