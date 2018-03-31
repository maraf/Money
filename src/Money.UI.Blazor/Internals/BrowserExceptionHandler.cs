using Microsoft.AspNetCore.Blazor;
using Money.Models.Api;
using Money.Services;
using Neptuo;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Internals
{
    internal class BrowserExceptionHandler
    {
        public ExceptionHandlerBuilder HandlerBuilder { get; private set; } = new ExceptionHandlerBuilder();
        public IExceptionHandler Handler { get => HandlerBuilder; }

        private readonly FormatterContainer formatters;

        public BrowserExceptionHandler(FormatterContainer formatters)
        {
            Ensure.NotNull(formatters, "formatters");
            this.formatters = formatters;
        }

        internal void Raise(string rawPayload)
        {
            Console.WriteLine($"Program: Exception: {rawPayload}");

            Response response = JsonUtil.Deserialize<Response>(rawPayload);
            Type type = Type.GetType(response.type);
            rawPayload = response.payload;

            Exception exception = (Exception)formatters.Exception.Deserialize(type, rawPayload);
            Handler.Handle(exception);
        }
    }
}
