using Microsoft.JSInterop;
using Money.Models;
using Neptuo;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Services
{
    public class CategoryStorage : JsonLocalStorage<List<CategoryModel>>
    {
        public CategoryStorage(FormatterContainer formatters, IJSRuntime jsRuntime, ILogFactory logFactory)
            : base(formatters, jsRuntime, logFactory, "categories")
        { }
    }
}
