using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Models
{
    /// <summary>
    /// A model for a whole year.
    /// </summary>
    public class YearModel
    {
        /// <summary>
        /// Gets a year.
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="year">A year</param>
        public YearModel(int year)
        {
            Ensure.Positive(year, "year");
            Year = year;
        }

        public override bool Equals(object obj)
        {
            YearModel other = obj as YearModel;
            if (other == null)
                return false;

            return Year == other.Year;
        }

        public override string ToString()
        {
            return Year.ToString();
        }

        public static bool operator ==(YearModel a, YearModel b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(YearModel a, YearModel b)
        {
            return !(a == b);
        }
    }
}
