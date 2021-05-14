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
    /// A query for getting total month income.
    /// </summary>
    public class GetTotalMonthIncome : UserQuery, IQuery<Price>
    {
        public MonthModel Month { get; private set; }

        public GetTotalMonthIncome(MonthModel month)
        {
            Month = month;
        }
    }
}
