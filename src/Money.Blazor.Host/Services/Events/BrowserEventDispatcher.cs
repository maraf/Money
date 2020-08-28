using Money.Api.Models;
using Money.Services;
using Neptuo.Formatters;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events
{
    public class BrowserEventDispatcher
    {
        private readonly DefaultEventManager manager = new DefaultEventManager();
        private readonly FormatterContainer formatters;
        private readonly ILog log;
        private readonly Json json;

        public BrowserEventDispatcher(FormatterContainer formatters, ILogFactory logFactory, Json json)
        {
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(logFactory, "logFactory");
            Ensure.NotNull(json, "json");
            this.formatters = formatters;
            this.log = logFactory.Scope("BrowserEventDispatcher");
            this.json = json;
        }

        public IEventHandlerCollection Handlers => manager;
        public IEventDispatcher Dispatcher => manager;

        public void Raise(string rawPayload)
        {
            log.Debug($"Raised: {rawPayload}");

            Response response = json.Deserialize<Response>(rawPayload);

            log.Debug($"Response: '{response.ResponseType}', '{response.Payload}'.");

            Type type = Type.GetType(response.Type);
            rawPayload = response.Payload;

            object payload = formatters.Event.Deserialize(type, rawPayload);

            MethodInfo methodInfo = manager.GetType().GetMethod(nameof(DefaultEventManager.PublishAsync)).MakeGenericMethod(payload.GetType());
            methodInfo.Invoke(manager, new object[] { payload });
        }
    }
}
