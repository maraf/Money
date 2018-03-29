using Money.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    partial class PriceCalculator
    {
        private class ExchangeRateModelComparer : IComparer<ExchangeRateModel>
        {
            public int Compare(ExchangeRateModel x, ExchangeRateModel y)
            {
                return y.ValidFrom.CompareTo(x.ValidFrom);
            }
        }
    }
}
