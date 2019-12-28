using Microsoft.JSInterop;
using Money.Models;
using Neptuo;
using Neptuo.Formatters;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class CurrencyStorage : JsonLocalStorage<List<CurrencyModel>>
    {
        public CurrencyStorage(Json json, IJSRuntime jsRuntime, ILogFactory logFactory)
            : base(json, jsRuntime, logFactory, "currencies")
        { }
    }
}
