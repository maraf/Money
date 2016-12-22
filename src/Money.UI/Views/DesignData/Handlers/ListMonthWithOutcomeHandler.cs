using Money.Services.Models;
using Money.Services.Models.Queries;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.DesignData.Handlers
{
    internal class ListMonthWithOutcomeHandler : IQueryHandler<ListMonthWithOutcome, IEnumerable<MonthModel>>
    {
        public Task<IEnumerable<MonthModel>> HandleAsync(ListMonthWithOutcome query)
        {
            return Task.FromResult<IEnumerable<MonthModel>>(new MonthModel[] 
            {
                new MonthModel(2016, 12),
                new MonthModel(2016, 11),
                new MonthModel(2016, 10)
            });
        }
    }
}
