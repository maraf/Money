using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a category icon is changed.
    /// </summary>
    public class CategoryIconChanged : UserEvent
    {
        /// <summary>
        /// Gets a category font icon.
        /// </summary>
        public string Icon { get; private set; }

        internal CategoryIconChanged(string icon)
        {
            Icon = icon;
        }
    }
}
