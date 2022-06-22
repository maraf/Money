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
    public class ExpenseTemplateStorage : JsonLocalStorage<List<ExpenseTemplateModel>>
    {
        public ExpenseTemplateStorage(FormatterContainer formatters, ILocalStorageService localStorage, ILogFactory logFactory)
            : base(formatters.Query, localStorage, logFactory, "expenseTemplates")
        { }
    }
}
