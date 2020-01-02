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
    public abstract class JsonLocalStorage<T>
        where T : class
    {
        private readonly FormatterContainer formatters;
        private readonly IJSRuntime jsRuntime;
        private readonly ILog log;
        private readonly string key;

        public JsonLocalStorage(FormatterContainer formatters, IJSRuntime jsRuntime, ILogFactory logFactory, string key)
        {
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(jsRuntime, "jsRuntime");
            Ensure.NotNull(logFactory, "logFactory");
            Ensure.NotNullOrEmpty(key, "key");
            this.formatters = formatters;
            this.jsRuntime = jsRuntime;
            this.log = logFactory.Scope(GetType().Name);
            this.key = key;
        }

        public async Task<T> LoadAsync()
        {
            try
            {
                string raw = await jsRuntime.InvokeAsync<string>("window.localStorage.getItem", key);
                log.Debug($"Loaded '{raw}'.");
                if (String.IsNullOrEmpty(raw))
                    return null;

                T model = formatters.Query.Deserialize<T>(raw);
                return model;
            }
            catch (Exception e)
            {
                log.Fatal(e);
                return null;
            }
        }

        public async Task SaveAsync(T model)
        {
            string raw = formatters.Query.Serialize(model);
            await jsRuntime.InvokeVoidAsync("window.localStorage.setItem", key, raw);
            log.Debug($"Saved '{raw}'.");
        }

        public async Task DeleteAsync()
        {
            await jsRuntime.InvokeVoidAsync("window.localStorage.removeItem", key);
            log.Debug($"Deleted.");
        }
    }
}
