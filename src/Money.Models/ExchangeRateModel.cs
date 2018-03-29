using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class ExchangeRateModel
    {
        public string SourceCurrency { get; private set; }
        public double Rate { get; private set; }
        public DateTime ValidFrom { get; private set; }

        public string ValidFromToString
        {
            get { return ValidFrom.ToString("dd.MM.yyyy"); }
        }

        public ExchangeRateModel(string sourceCurrency, double rate, DateTime validFrom)
        {
            SourceCurrency = sourceCurrency;
            Rate = rate;
            ValidFrom = validFrom;
        }
    }
}
