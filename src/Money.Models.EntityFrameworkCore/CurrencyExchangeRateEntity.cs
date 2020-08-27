using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class CurrencyExchangeRateEntity : IUserEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string TargetCurrency { get; set; }
        public string SourceCurrency { get; set; }
        public double Rate { get; set; }
        public DateTime ValidFrom { get; set; }
    }
}
