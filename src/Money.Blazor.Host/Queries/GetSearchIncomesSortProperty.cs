using Money.Models;
using Money.Models.Sorting;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Queries
{
    /// <summary>
    /// A query for getting a sort descriptor for income search.
    /// </summary>
    public class GetSearchIncomesSortProperty : IQuery<SortDescriptor<IncomeOverviewSortType>>
    { }
}
