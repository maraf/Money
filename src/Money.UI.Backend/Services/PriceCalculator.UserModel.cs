using Money.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    partial class PriceCalculator
    {
        private class UserModel
        {
            public ConcurrentDictionary<string, List<ExchangeRateModel>> Currencies { get; } = new ConcurrentDictionary<string, List<ExchangeRateModel>>();
            public string DefaultCurrencyUniqueCode { get; set; }
        }
    }
}
