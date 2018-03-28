using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    partial class CurrencyCache
    {
        private class Model
        {
            public string UniqueCode { get; set; }
            public string Symbol { get; set; }
        }
    }
}
