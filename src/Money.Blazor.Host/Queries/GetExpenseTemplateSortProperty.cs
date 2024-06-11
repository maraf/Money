using Money.Models;
using Money.Models.Sorting;
using Money.Pages;
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
    /// A query for getting a sort descriptor for expense templates.
    /// </summary>
    public class GetExpenseTemplateSortProperty : IQuery<SortDescriptor<ExpenseTemplateSortType>>
    { }
}
