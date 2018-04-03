using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Queries
{
    /// <summary>
    /// A query for getting a default currency unique code.
    /// </summary>
    public class GetCurrencyDefault : UserQuery, IQuery<string>
    { }
}
