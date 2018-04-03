using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for getting total month outcome.
    /// </summary>
    public class GetTotalMonthOutcome : UserQuery, IQuery<Price>
    {
        public MonthModel Month { get; private set; }

        public GetTotalMonthOutcome(MonthModel month)
        {
            Month = month;
        }
    }
}
