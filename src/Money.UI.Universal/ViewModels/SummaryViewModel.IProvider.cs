using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    partial class SummaryViewModel
    {
        public interface IProvider
        {
            Task ReplaceAsync(IList<SummaryCategoryViewModel> collection);

            Task<Price> GetTotalAmount();
        }
    }
}
