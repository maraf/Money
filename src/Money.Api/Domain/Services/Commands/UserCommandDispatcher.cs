using Microsoft.AspNetCore.Http;
using Money.Hubs;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    internal class UserCommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandDispatcher inner;
        private readonly HttpContext httpContext;
        private readonly ApiHub hub;

        public UserCommandDispatcher(ICommandDispatcher inner, HttpContext httpContext, ApiHub hub)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(httpContext, "httpContext");
            Ensure.NotNull(hub, "hub");
            this.inner = inner;
            this.httpContext = httpContext;
            this.hub = hub;
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            Envelope envelope = command as Envelope;
            if (envelope == null)
                envelope = Envelope.Create<TCommand>(command);

            string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!String.IsNullOrEmpty(userId))
            {
                IKey userKey = StringKey.Create(userId, "User");
                envelope.Metadata.Add("UserKey", userKey);

                if (command is ICommand esCommand)
                    hub.AddCommand(esCommand.Key, userKey);
            }

            return inner.HandleAsync(envelope);
        }
    }
}
