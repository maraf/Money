using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a new category is created.
    /// </summary>
    public class CategoryCreated : Event
    {
        /// <summary>
        /// Gets a name of the category.
        /// </summary>
        public string Name { get; private set; }

        internal CategoryCreated(string name)
        {
            Name = name;
        }
    }
}
