using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.ViewModels.Parameters
{
    public class CurrencyAddExchangeRateParameter
    {
        public string TargetCurrency { get; private set; }

        public CurrencyAddExchangeRateParameter(string targetCurrency)
        {
            Ensure.NotNull(targetCurrency, "targetCurrency");
            TargetCurrency = targetCurrency;
        }
    }
}
