using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    /// <summary>
    /// A navigation model for group page.
    /// </summary>
    public class GroupParameter
    {
        /// <summary>
        /// Gets a type of the grouping.
        /// </summary>
        public SummaryPeriodType Type { get; private set; }

        /// <summary>
        /// Gets a parameter for inner page.
        /// </summary>
        public object Inner { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="type">A type of the grouping.</param>
        /// <param name="inner">A parameter for inner page.</param>
        public GroupParameter(SummaryPeriodType type, object inner)
        {
            Ensure.NotNull(inner, "inner");
            Type = type;
            Inner = inner;
        }
    }
}
