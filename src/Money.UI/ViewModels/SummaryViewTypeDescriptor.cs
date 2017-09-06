using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    /// <summary>
    /// A descriptor for summary view type.
    /// </summary>
    public class SummaryViewTypeDescriptor
    {
        /// <summary>
        /// Gets a view type.
        /// </summary>
        public SummaryViewType Type { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="type">A view type.</param>
        public SummaryViewTypeDescriptor(SummaryViewType type)
        {
            Type = type;
        }
    }
}
