using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised after changing a name.
    /// </summary>
    public class CategoryRenamed : UserEvent
    {
        /// <summary>
        /// Get a new name of the category.
        /// </summary>
        public string NewName { get; private set; }

        /// <summary>
        /// Get an original name of the category.
        /// </summary>
        public string OldName { get; private set; }

        internal CategoryRenamed(string newName, string oldName)
        {
            NewName = newName;
            OldName = oldName;
        }
    }
}
