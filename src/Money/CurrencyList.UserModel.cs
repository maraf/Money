using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money
{
    partial class CurrencyList
    {
        private class UserModel
        {
            public HashSet<string> UniqueCodes { get; } = new HashSet<string>();
            public HashSet<string> DeletedUniqueCodes { get; } = new HashSet<string>();
            public HashSet<int> ExchangeRateHashCodes { get; } = new HashSet<int>();
            public string DefaultName { get; set; }
        }
    }
}
