using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries;

/// <summary>
/// A query for getting total month expenses expected to come.
/// </summary>
public class GetMonthExpectedExpenseTotal : UserQuery, IQuery<Price>
{
    public MonthModel Month { get; private set; }

    public GetMonthExpectedExpenseTotal(MonthModel month)
    {
        Month = month;
    }
}
