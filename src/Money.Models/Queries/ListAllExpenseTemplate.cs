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
    /// A query for getting all expense templates.
    /// </summary>
    public class ListAllExpenseTemplate : UserQuery, IQuery<List<ExpenseTemplateModel>>
    { }
}
