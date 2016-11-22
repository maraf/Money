using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels
{
    partial class SummaryGroupViewModel
    {
        public interface IProvider
        {
            Task ReplaceAsync(IList<SummaryItemViewModel> collection);

            Task<Price> GetTotalAmount();
        }
    }
}
