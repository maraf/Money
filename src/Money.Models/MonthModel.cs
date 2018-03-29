using Neptuo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model for a month of a year.
    /// </summary>
    public class MonthModel
    {
        /// <summary>
        /// Gets a month number.
        /// </summary>
        public int Month { get; private set; }

        /// <summary>
        /// Gets a year.
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="year">A year</param>
        /// <param name="month">A month number</param>
        public MonthModel(int year, int month)
        {
            Ensure.Positive(year, "year");
            Ensure.Positive(month, "month");

            if (month > 12)
                throw Ensure.Exception.ArgumentOutOfRange("month", "A month must be between 1 and 12.");

            Year = year;
            Month = month;
        }

        public override bool Equals(object obj)
        {
            MonthModel other = obj as MonthModel;
            if (other == null)
                return false;

            return Year == other.Year && Month == other.Month;
        }

        public override string ToString()
        {
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[Month - 1];
            if (Year == DateTime.Now.Year)
                return monthName;

            return $"{monthName} {Year}";
        }

        /// <summary>
        /// Creates an instalce of <see cref="MonthModel"/> from <paramref name="dateTime"/>.
        /// </summary>
        /// <param name="dateTime">A source date and time.</param>
        public static implicit operator MonthModel(DateTime dateTime)
        {
            return new MonthModel(dateTime.Year, dateTime.Month);
        }

        public static bool operator ==(MonthModel a, MonthModel b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(MonthModel a, MonthModel b)
        {
            return !(a == b);
        }
    }
}
