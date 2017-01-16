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
    internal class SummaryGroupViewModelProvider : SummaryViewModel.IProvider
    {
        public Task<Price> GetTotalAmount()
        {
            return Task.FromResult(new Price(7800, "CZK"));
        }

        public Task ReplaceAsync(IList<SummaryCategoryViewModel> collection)
        {
            collection.Add(new SummaryCategoryViewModel()
            {
                Amount = new Price(2500, "CZK"),
                Name = "Food",
                Color = Colors.CadetBlue
            });
            collection.Add(new SummaryCategoryViewModel()
            {
                Amount = new Price(900, "CZK"),
                Name = "Eating out",
                Color = Colors.Brown
            });
            collection.Add(new SummaryCategoryViewModel()
            {
                Amount = new Price(4400, "CZK"),
                Name = "Home",
                Color = Colors.Gold
            });

            return Async.CompletedTask;
        }
    }
}
