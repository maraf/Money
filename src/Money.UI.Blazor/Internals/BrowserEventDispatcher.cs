using Microsoft.AspNetCore.Blazor;
using Money.Models.Api;
using Money.Services;
using Neptuo;
using Neptuo.Events;
using Neptuo.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Money.Internals
{
    internal class BrowserEventDispatcher
    {
        private readonly DefaultEventManager manager = new DefaultEventManager();
        private readonly FormatterContainer formatters;

        public BrowserEventDispatcher(FormatterContainer formatters)
        {
            Ensure.NotNull(formatters, "formatters");
            this.formatters = formatters;
        }

        public IEventHandlerCollection Handlers
        {
            get => manager;
        }

        public void Raise(string rawPayload)
        {
            Console.WriteLine($"BRED: Raise: {rawPayload}");

            Response response = JsonUtil.Deserialize<Response>(rawPayload);
            Type type = Type.GetType(response.type);
            rawPayload = response.payload;

            object payload = formatters.Event.Deserialize(type, rawPayload);

            MethodInfo methodInfo = manager.GetType().GetMethod(nameof(DefaultEventManager.PublishAsync)).MakeGenericMethod(payload.GetType());
            methodInfo.Invoke(manager, new object[] { payload });
        }
    }
}
