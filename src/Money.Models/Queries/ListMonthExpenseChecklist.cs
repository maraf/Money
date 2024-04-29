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
    /// A query for getting a monthly expense checklist.
    /// </summary>
    public class ListMonthExpenseChecklist : UserQuery, IQuery<List<ExpenseChecklistModel>>
    {
        public MonthModel Month { get; private set; }

        public ListMonthExpenseChecklist(MonthModel month)
        {
            Ensure.NotNull(month, "month");
            Month = month;
        }
    }
}
