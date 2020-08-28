using Money.Api.Models;
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
    public class BrowserExceptionHandler
    {
        public ExceptionHandlerBuilder HandlerBuilder { get; private set; } = new ExceptionHandlerBuilder();
        public IExceptionHandler Handler { get => HandlerBuilder; }

        private readonly FormatterContainer formatters;
        private readonly ILog log;
        private readonly Json json;

        public BrowserExceptionHandler(FormatterContainer formatters, ILogFactory logFactory, Json json)
        {
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(logFactory, "logFactory");
            Ensure.NotNull(json, "json");
            this.formatters = formatters;
            this.log = logFactory.Scope("ExceptionHandler");
            this.json = json;
        }

        internal void Raise(string rawPayload)
        {
            log.Debug($"'{rawPayload}'.");

            Response response = json.Deserialize<Response>(rawPayload);
            Type type = Type.GetType(response.Type);
            rawPayload = response.Payload;

            Exception exception = (Exception)formatters.Exception.Deserialize(type, rawPayload);
            Handler.Handle(exception);

            log.Debug($"Handled exception of type '{exception.GetType().FullName}'.");
        }
    }
}
