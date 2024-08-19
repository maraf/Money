using Money.Services;
using Neptuo.Events;
using Neptuo.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    internal class HttpCommandDispatcher : ICommandDispatcher
    {
        private readonly ApiClient api;
        private readonly FormatterContainer formatters;
        private readonly IEventDispatcher events;

        public HttpCommandDispatcher(ApiClient api, FormatterContainer formatters, IEventDispatcher events)
        {
            Ensure.NotNull(api, "api");
            Ensure.NotNull(formatters, "formatters");
            Ensure.NotNull(events, "events");
            this.api = api;
            this.formatters = formatters;
            this.events = events;
        }

        public async Task HandleAsync<TCommand>(TCommand command)
        {
            string payload = formatters.Command.Serialize(command);
            await events.PublishAsync(new HttpCommandSending(command));
            await api.CommandAsync(command.GetType(), payload);
        }
    }

    public record HttpCommandSending(object Command);
}
