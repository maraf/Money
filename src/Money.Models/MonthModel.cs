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
    public class MonthModel : IEquatable<MonthModel>, IComparable<MonthModel>
    {
        /// <summary>
        /// Gets a year.
        /// </summary>
        public virtual int Year { get; }

        /// <summary>
        /// Gets a month number.
        /// </summary>
        public virtual int Month { get; }

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

        public override string ToString()
        {
            string monthName = ToMonthString();
            if (Year == DateTime.Now.Year)
                return monthName;

            return $"{monthName} {Year}";
        }

        public string ToMonthString() 
            => CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[Month - 1];

        public override bool Equals(object obj) => Equals(obj as MonthModel);

        public bool Equals(MonthModel other) => other != null && Year == other.Year && Month == other.Month;

        public override int GetHashCode()
        {
            var hashCode = 173839681;
            hashCode = hashCode * -1521134295 + Year.GetHashCode();
            hashCode = hashCode * -1521134295 + Month.GetHashCode();
            return hashCode;
        }

        public int CompareTo(MonthModel other)
        {
            int compare = Year.CompareTo(other.Year);
            if (compare == 0)
                compare = Month.CompareTo(other.Month);

            return compare;
        }

        /// <summary>
        /// Creates an instance of <see cref="MonthModel"/> from <paramref name="dateTime"/>.
        /// </summary>
        /// <param name="dateTime">A source date and time.</param>
        public static implicit operator MonthModel(DateTime dateTime) => new MonthModel(dateTime.Year, dateTime.Month);

        public static bool operator ==(MonthModel model1, MonthModel model2) 
            => EqualityComparer<MonthModel>.Default.Equals(model1, model2);

        public static bool operator !=(MonthModel model1, MonthModel model2) 
            => !(model1 == model2);
        
        public static bool operator >(MonthModel model1, MonthModel model2) 
            => model1.Year > model2.Year || (model1.Year == model2.Year && model1.Month > model2.Month);

        public static bool operator <(MonthModel model1, MonthModel model2) 
            => model1.Year < model2.Year || (model1.Year == model2.Year && model1.Month < model2.Month);

        public static MonthModel operator -(MonthModel model, int amount)
        {
            int year = model.Year;
            int month = model.Month - amount;
            if (month <= 0)
            {
                month = 12 + month;
                year = Math.Max(year - 1, 0);
            }

            return new MonthModel(year, month);
        }
    }
}
