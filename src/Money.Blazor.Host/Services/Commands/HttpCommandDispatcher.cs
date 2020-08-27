using Money.Services;
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

        public HttpCommandDispatcher(ApiClient api, FormatterContainer formatters)
        {
            Ensure.NotNull(api, "api");
            Ensure.NotNull(formatters, "formatters");
            this.api = api;
            this.formatters = formatters;
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            string payload = formatters.Command.Serialize(command);
            return api.CommandAsync(command.GetType(), payload);
        }
    }
}
