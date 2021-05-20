using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    /// <summary>
    /// A model for a whole year.
    /// </summary>
    public class YearModel : IEquatable<YearModel>
    {
        /// <summary>
        /// Gets a year.
        /// </summary>
        public virtual int Year { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="year">A year</param>
        public YearModel(int year)
        {
            Ensure.Positive(year, "year");
            Year = year;
        }

        public override string ToString() 
            => Year.ToString();

        public string ToShortString()
            => Year.ToString().Substring(2);

        public override bool Equals(object obj) => Equals(obj as YearModel);

        public bool Equals(YearModel other) => other != null && Year == other.Year;

        public override int GetHashCode() => 1642494364 + Year.GetHashCode();

        /// <summary>
        /// Creates an instance of <see cref="YearModel"/> from <paramref name="dateTime"/>.
        /// </summary>
        /// <param name="dateTime">A source date and time.</param>
        public static implicit operator YearModel(DateTime dateTime) => new YearModel(dateTime.Year);

        public static bool operator ==(YearModel model1, YearModel model2) 
            => EqualityComparer<YearModel>.Default.Equals(model1, model2);

        public static bool operator !=(YearModel model1, YearModel model2) 
            => !(model1 == model2);

        public static bool operator >(YearModel model1, YearModel model2)
            => model1.Year > model2.Year;

        public static bool operator <(YearModel model1, YearModel model2)
            => model1.Year < model2.Year;

        public static YearModel operator -(YearModel model, int amount) 
            => new YearModel(Math.Max(model.Year - amount, 0));
    }
}
