using Money.Events;
using Money.Services.Models.Queries;
using Neptuo.Events.Handlers;
using Neptuo.Queries.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Models.Builders
{
    public class OutcomeBuilder : IEventHandler<OutcomeCreated>, IEventHandler<OutcomeCategoryAdded>, IQueryHandler<ListMonthWithOutcome, IEnumerable<MonthModel>>
    {
        public Task<IEnumerable<MonthModel>> HandleAsync(ListMonthWithOutcome query)
        {
            return Task.FromResult<IEnumerable<MonthModel>>(new List<MonthModel>()
            {
                new MonthModel(2016, 10),
                new MonthModel(2016, 11),
                new MonthModel(2016, 12)
            });
        }

        public Task HandleAsync(OutcomeCategoryAdded payload)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(OutcomeCreated payload)
        {
            throw new NotImplementedException();
        }
    }
}
