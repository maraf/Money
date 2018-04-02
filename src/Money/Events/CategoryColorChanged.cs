using Neptuo.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Events
{
    /// <summary>
    /// An event raised when a category color is changed.
    /// </summary>
    public class CategoryColorChanged : UserEvent
    {
        /// <summary>
        /// Gets a new color of the category
        /// </summary>
        public Color Color { get; private set; }

        internal CategoryColorChanged(Color color)
        {
            Color = color;
        }
    }
}
