using Money.Services.Models;
using Money.Services.Models.Queries;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    public class NotEmptyMonthCategoryGroupProvider : SummaryViewModel.IProvider
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly IFactory<Price, decimal> priceFactory;
        private readonly MonthModel month;

        public NotEmptyMonthCategoryGroupProvider(IQueryDispatcher queryDispatcher, IFactory<Price, decimal> priceFactory, MonthModel month)
        {
            Ensure.NotNull(queryDispatcher, "queryDispatcher");
            Ensure.NotNull(priceFactory, "priceFactory");
            Ensure.NotNull(month, "month");
            this.queryDispatcher = queryDispatcher;
            this.priceFactory = priceFactory;
            this.month = month;
        }

        public async Task ReplaceAsync(IList<SummaryItemViewModel> collection)
        {
            IEnumerable<CategoryWithAmountModel> categories = await queryDispatcher.QueryAsync(new ListMonthCategoryWithOutcome(month));
            foreach (CategoryWithAmountModel category in categories)
            {
                collection.Add(new SummaryItemViewModel()
                {
                    CategoryKey = category.Key,
                    Name = category.Name,
                    Color = category.Color,
                    Amount = category.TotalAmount
                });
            }
        }

        public Task<Price> GetTotalAmount()
        {
            return queryDispatcher.QueryAsync(new GetTotalMonthOutcome(month));
        }
    }
}
