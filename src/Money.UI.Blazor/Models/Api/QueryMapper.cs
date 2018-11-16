using Money.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Api
{
    public class QueryMapper : TypeMapper
    {
        public QueryMapper()
        {
            Add<ListAllCurrency>("currency-list");
            Add<ListAllCategory>("category-list");
            Add<GetTotalMonthOutcome>("outcome-month-total");
        }
    }
}
