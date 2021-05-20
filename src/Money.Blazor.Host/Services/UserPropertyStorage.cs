using Blazored.LocalStorage;
using Money.Models;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class UserPropertyStorage : JsonLocalStorage<List<UserPropertyModel>>
    {
        public UserPropertyStorage(FormatterContainer formatters, ILocalStorageService localStorage, ILogFactory logFactory)
            : base(formatters.Query, localStorage, logFactory, "userProperties")
        { }
    }
}
