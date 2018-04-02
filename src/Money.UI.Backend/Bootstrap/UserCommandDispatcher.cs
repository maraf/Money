using Microsoft.AspNetCore.Http;
using Neptuo;
using Neptuo.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    internal class UserCommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandDispatcher inner;
        private readonly HttpContext httpContext;

        public UserCommandDispatcher(ICommandDispatcher inner, HttpContext httpContext)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(httpContext, "httpContext");
            this.inner = inner;
            this.httpContext = httpContext;
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            Envelope envelope = command as Envelope;
            if (envelope == null)
                envelope = Envelope.Create<TCommand>(command);

            string userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!String.IsNullOrEmpty(userId))
                envelope.Metadata.Add("UserId", userId);

            return inner.HandleAsync(envelope);
        }
    }
}
