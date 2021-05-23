using Blazored.LocalStorage;
using Microsoft.JSInterop;
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
        private readonly IFormatter formatter;
        private readonly ILocalStorageService localStorage;
        private readonly ILog log;
        private readonly string key;

        public JsonLocalStorage(IFormatter formatter, ILocalStorageService localStorage, ILogFactory logFactory, string key)
        {
            Ensure.NotNull(formatter, "formatter");
            Ensure.NotNull(localStorage, "localStorage");
            Ensure.NotNull(logFactory, "logFactory");
            Ensure.NotNullOrEmpty(key, "key");
            this.formatter = formatter;
            this.localStorage = localStorage;
            this.log = logFactory.Scope(GetType().Name);
            this.key = key;
        }

        public async Task<T> LoadAsync()
        {
            try
            {
                string raw = await localStorage.GetItemAsStringAsync(key);
                log.Debug($"Loaded '{raw}', T is '{typeof(T).FullName}'.");
                if (String.IsNullOrEmpty(raw))
                    return null;

                T model = formatter.Deserialize<T>(raw);
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
            string raw = formatter.Serialize(model);
            await localStorage.SetItemAsStringAsync(key, raw);
            log.Debug($"Saved '{raw}'.");
        }

        public async Task DeleteAsync()
        {
            await localStorage.RemoveItemAsync(key);
            log.Debug($"Deleted.");
        }
    }
}
