using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for getting total year outcome.
    /// </summary>
    public class GetTotalYearOutcome : UserQuery, IQuery<Price>
    {
        public YearModel Year { get; private set; }

        public GetTotalYearOutcome(YearModel year)
        {
            Ensure.NotNull(year, "year");
            Year = year;
        }
    }
}
