using Microsoft.AspNetCore.Blazor;
using Money.Models.Api;
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
    internal class BrowserEventDispatcher
    {
        private readonly DefaultEventManager manager = new DefaultEventManager();
        private readonly FormatterContainer formatters;
        private readonly ILog log;

        public BrowserEventDispatcher(FormatterContainer formatters, ILogFactory logFactory)
        {
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(logFactory, "logFactory");
            this.formatters = formatters;
            this.log = logFactory.Scope("BrowserEventDispatcher");
        }

        public IEventHandlerCollection Handlers
        {
            get => manager;
        }

        public void Raise(string rawPayload)
        {
            log.Debug($"Raised: {rawPayload}");

            Response response = JsonUtil.Deserialize<Response>(rawPayload);
            Type type = Type.GetType(response.Type);
            rawPayload = response.Payload;

            object payload = formatters.Event.Deserialize(type, rawPayload);

            MethodInfo methodInfo = manager.GetType().GetMethod(nameof(DefaultEventManager.PublishAsync)).MakeGenericMethod(payload.GetType());
            methodInfo.Invoke(manager, new object[] { payload });
        }
    }
}
