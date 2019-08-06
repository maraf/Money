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
    /// A query for getting list of years with outcome.
    /// </summary>
    public class ListYearWithOutcome : UserQuery, IQuery<List<YearModel>>
    { }
}
