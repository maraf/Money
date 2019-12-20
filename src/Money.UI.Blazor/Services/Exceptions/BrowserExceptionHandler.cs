using Money.Models.Api;
using Money.Services;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neptuo.Exceptions
{
    internal class BrowserExceptionHandler
    {
        public ExceptionHandlerBuilder HandlerBuilder { get; private set; } = new ExceptionHandlerBuilder();
        public IExceptionHandler Handler { get => HandlerBuilder; }

        private readonly FormatterContainer formatters;
        private readonly ILog log;

        public BrowserExceptionHandler(FormatterContainer formatters, ILogFactory logFactory)
        {
            Ensure.NotNull(formatters, "formatters");
            this.formatters = formatters;
            this.log = logFactory.Scope("ExceptionHandler");
        }

        internal void Raise(string rawPayload)
        {
            log.Debug($"'{rawPayload}'.");

            Response response = JsonSerializer.Deserialize<Response>(rawPayload);
            Type type = Type.GetType(response.Type);
            rawPayload = response.Payload;

            Exception exception = (Exception)formatters.Exception.Deserialize(type, rawPayload);
            Handler.Handle(exception);

            log.Debug($"Handled exception of type '{exception.GetType().FullName}'.");
        }
    }
}
