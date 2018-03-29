using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Data
{
    public class CurrencyExchangeRateEntity
    {
        public int Id { get; set; }
        public string TargetCurrency { get; set; }
        public string SourceCurrency { get; set; }
        public double Rate { get; set; }
        public DateTime ValidFrom { get; set; }
    }
}
