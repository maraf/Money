using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class TokenStorage
    {
        private const string key = "token";

        private readonly ISessionStorageService session;
        private readonly ILocalStorageService local;

        public TokenStorage(ISessionStorageService session, ILocalStorageService local)
        {
            Ensure.NotNull(session, "session");
            Ensure.NotNull(local, "local");
            this.session = session;
            this.local = local;
        }

        public async Task SetAsync(string token, bool isPersistent)
        {
            if (isPersistent)
                await local.SetItemAsStringAsync(key, token);
            else
                await session.SetItemAsync(key, token);
        }

        public async Task ClearAsync()
        {
            await local.RemoveItemAsync(key);
            await session.RemoveItemAsync(key);
        }

        public async Task<string> FindAsync()
        {
            string token = await local.GetItemAsStringAsync(key);
            if (String.IsNullOrEmpty(token))
                token = await session.GetItemAsync<string>(key);

            return token;
        }
    }
}
