using Blazored.LocalStorage;
using Microsoft.JSInterop;
using Money.Services;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    public class CreateExpenseStorage : JsonLocalStorage<List<CreateOutcome>>
    {
        public CreateExpenseStorage(FormatterContainer formatters, ILocalStorageService localStorage, ILogFactory logFactory) 
            : base(formatters.Query, localStorage, logFactory, "expenses")
        { }
    }
}
