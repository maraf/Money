using Money.Models;
using Money.Models.Queries;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Views.DesignData
{
    internal class MockQueryDispatcher : IQueryDispatcher
    {
        public Task<TOutput> QueryAsync<TOutput>(IQuery<TOutput> query)
        {
            if (query.GetType() == typeof(ListMonthWithOutcome))
            {
                IEnumerable<MonthModel> models = new MonthModel[]
                {
                    new MonthModel(2016, 12),
                    new MonthModel(2016, 11),
                    new MonthModel(2016, 10)
                };
                return Task.FromResult((TOutput)models);
            }

            return Task.FromResult(default(TOutput));
        }
    }
}
