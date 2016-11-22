using Money.ViewModels;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Money.Views.DesignData
{
    internal class SummaryGroupViewModelProvider : SummaryGroupViewModel.IProvider
    {
        public Task<Price> GetTotalAmount()
        {
            return Task.FromResult(new Price(7800, "CZK"));
        }

        public Task ReplaceAsync(IList<SummaryItemViewModel> collection)
        {
            collection.Add(new SummaryItemViewModel()
            {
                Amount = new Price(2500, "CZK"),
                Name = "Food",
                Color = Colors.CadetBlue
            });
            collection.Add(new SummaryItemViewModel()
            {
                Amount = new Price(900, "CZK"),
                Name = "Eating out",
                Color = Colors.Brown
            });
            collection.Add(new SummaryItemViewModel()
            {
                Amount = new Price(4400, "CZK"),
                Name = "Home",
                Color = Colors.Gold
            });

            return Async.CompletedTask;
        }
    }
}
