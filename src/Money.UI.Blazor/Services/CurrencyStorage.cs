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
    internal class CurrencyStorage
    {
        private readonly Json json;
        private readonly IJSRuntime jsRuntime;
        private readonly ILog<CurrencyStorage> log;

        public CurrencyStorage(Json json, IJSRuntime jsRuntime, ILog<CurrencyStorage> log)
        {
            Ensure.NotNull(json, "json");
            Ensure.NotNull(jsRuntime, "jsRuntime");
            Ensure.NotNull(log, "log");
            this.json = json;
            this.jsRuntime = jsRuntime;
            this.log = log;
        }

        public async Task<List<CurrencyModel>> LoadAsync()
        {
            string raw = await jsRuntime.InvokeAsync<string>("window.localStorage.getItem", "currencies");
            log.Debug($"Load currencies: '{raw}'.");
            if (String.IsNullOrEmpty(raw))
                return null;

            List<CurrencyModel> models = json.Deserialize<List<CurrencyModel>>(raw);
            return models;
        }

        public async Task SaveAsync(IReadOnlyCollection<CurrencyModel> models)
        {
            string raw = json.Serialize(models);
            log.Debug($"Save currencies: '{raw}'.");
            await jsRuntime.InvokeVoidAsync("window.localStorage.setItem", "currencies", raw);
        }
    }
}
