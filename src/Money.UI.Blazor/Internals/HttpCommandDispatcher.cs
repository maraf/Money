using Money.Models.Api;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Internals
{
    internal class HttpCommandDispatcher : ICommandDispatcher
    {
        private readonly ApiClient api;
        private readonly Formatters formatters;

        public HttpCommandDispatcher(ApiClient api, Formatters formatters)
        {
            Ensure.NotNull(api, "api");
            Ensure.NotNull(formatters, "formatters");
            this.api = api;
            this.formatters = formatters;
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            string payload = formatters.Command.Serialize(command);
            string type = command.GetType().AssemblyQualifiedName;

            return api.CommandAsync(new Request()
            {
                Payload = payload,
                Type = type
            });
        }
    }
}
