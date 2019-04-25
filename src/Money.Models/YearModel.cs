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
        public int Year { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="year">A year</param>
        public YearModel(int year)
        {
            Ensure.Positive(year, "year");
            Year = year;
        }

        public override string ToString() => Year.ToString();

        public override bool Equals(object obj) => Equals(obj as YearModel);

        public bool Equals(YearModel other) => other != null && Year == other.Year;

        public override int GetHashCode() => 1642494364 + Year.GetHashCode();

        public static bool operator ==(YearModel model1, YearModel model2) => EqualityComparer<YearModel>.Default.Equals(model1, model2);

        public static bool operator !=(YearModel model1, YearModel model2) => !(model1 == model2);
    }
}
