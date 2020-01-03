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
        public CreateExpenseStorage(FormatterContainer formatters, IJSRuntime jsRuntime, ILogFactory logFactory) 
            : base(formatters.Query, jsRuntime, logFactory, "expenses")
        { }
    }
}
