using Money.Models;
using Neptuo;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    /// <summary>
    /// A navigation parameters for <see cref="OverviewViewModel"/>.
    /// </summary>
    public class OverviewParameter
    {
        /// <summary>
        /// Gets a category key or empty key.
        /// </summary>
        public IKey CategoryKey { get; private set; }

        /// <summary>
        /// Gets a month to display.
        /// </summary>
        public MonthModel Month { get; private set; }

        /// <summary>
        /// Gets a whole year to display.
        /// </summary>
        public YearModel Year { get; private set; }

        /// <summary>
        /// Creates a new instance for diplaying a month.
        /// </summary>
        /// <param name="categoryKey">A category key or empty key.</param>
        public OverviewParameter(IKey categoryKey, MonthModel month)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Ensure.NotNull(month, "month");
            CategoryKey = categoryKey;
            Month = month;
        }

        /// <summary>
        /// Creates a new isntance for displaying a year.
        /// </summary>
        /// <param name="categoryKey">A category key or empty key.</param>
        /// <param name="year">A year to display.</param>
        public OverviewParameter(IKey categoryKey, YearModel year)
        {
            Ensure.NotNull(categoryKey, "categoryKey");
            Ensure.NotNull(year, "year");
            CategoryKey = categoryKey;
            Year = year;
        }
    }
}
