using Neptuo;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services.Models.Queries
{
    public class ListTargetCurrencyExchangeRates : IQuery<List<ExchangeRateModel>>
    { 
        public string TargetCurrency { get; private set; }

        public ListTargetCurrencyExchangeRates(string targetCurrency)
        {
            Ensure.NotNullOrEmpty(targetCurrency, "targetCurrency");
            TargetCurrency = targetCurrency;
        }
    }
}
