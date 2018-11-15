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
            Add("currency-list", typeof(ListAllCurrency));
            Add("category-list", typeof(ListAllCategory));
            Add("outcome-month-total", typeof(GetTotalMonthOutcome));
        }
    }
}
