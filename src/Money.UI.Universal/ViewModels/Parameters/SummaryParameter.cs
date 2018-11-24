using Money.Models;
using Money.Models.Sorting;
using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    /// <summary>
    /// A navigation parameter for single summary page.
    /// </summary>
    public class SummaryParameter
    {
        private YearModel year;
        private MonthModel month;

        /// <summary>
        /// Gets a period type to display.
        /// </summary>
        public SummaryPeriodType PeriodType { get; private set; }

        /// <summary>
        /// Gets a prefered view type.
        /// </summary>
        public SummaryViewType? ViewType { get; set; }

        /// <summary>
        /// Gets or sets a year to display.
        /// Also switches <see cref="PeriodType"/> to <see cref="SummaryPeriodType.Year"/>.
        /// </summary>
        public YearModel Year
        {
            get { return year; }
            set
            {
                year = value;
                if (value != null)
                {
                    Month = null;
                    PeriodType = SummaryPeriodType.Year;
                }
            }
        }

        /// <summary>
        /// Gets or sets a month to display.
        /// Also switches <see cref="PeriodType"/> to <see cref="SummaryPeriodType.Month"/>.
        /// </summary>
        public MonthModel Month
        {
            get { return month; }
            set
            {
                month = value;
                if(month != null)
                {
                    Year = null;
                    PeriodType = SummaryPeriodType.Month;
                }
            }
        }

        /// <summary>
        /// Gets or sets a default sorting.
        /// </summary>
        public SortDescriptor<SummarySortType> SortDescriptor { get; set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public SummaryParameter()
            : this(SummaryPeriodType.Month)
        { }

        /// <summary>
        /// Creates new instance with prefered <paramref name="periodType" />.
        /// </summary>
        /// <param name="periodType">A period type to display.</param>
        public SummaryParameter(SummaryPeriodType periodType)
        {
            PeriodType = periodType;
        }

        /// <summary>
        /// Creates new instance with prefered <paramref name="viewType"/> and specific <paramref name="year" /> to display.
        /// </summary>
        /// <param name="year">A specific year to display.</param>
        public SummaryParameter(YearModel year)
            : this(SummaryPeriodType.Year)
        {
            Ensure.NotNull(year, "year");
            this.year = year;
        }

        /// <summary>
        /// Creates new instance with prefered <paramref name="viewType"/> and specific <paramref name="month" /> to display.
        /// </summary>
        /// <param name="month">A specific month to display.</param>
        public SummaryParameter(MonthModel month)
            : this(SummaryPeriodType.Month)
        {
            Ensure.NotNull(month, "month");
            this.month = month;
        }

        public override bool Equals(object obj)
        {
            SummaryParameter other = obj as SummaryParameter;
            if (other == null)
                return false;

            if ((ViewType != null && other.ViewType == null) || (ViewType == null && other.ViewType != null) || !ViewType.Equals(other.ViewType))
                return false;

            if (!PeriodType.Equals(other.PeriodType))
                return false;

            if (Year != null && Year.Equals(other.Year))
                return true;
            else if (Year == null && other.Year != null)
                return false;
            else if (Year == null && other.Year == null)
                return true;
            else if (Month != null && Month.Equals(other.Month))
                return true;
            else if (Month == null && other.Month != null)
                return false;
            else if (Month == null && other.Month == null)
                return true;

            return false;
        }
    }
}
