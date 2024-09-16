using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries;

/// <summary>
/// A query for getting total year income.
/// </summary>
public class GetTotalYearIncome : UserQuery, IQuery<Price>
{
    public YearModel Year { get; private set; }

    public GetTotalYearIncome(YearModel year)
    {
        Year = year;
    }
}

