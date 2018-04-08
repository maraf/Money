using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Bootstrap
{
    internal class UserCommandDispatcher : ICommandDispatcher
    {
        private readonly ICommandDispatcher inner;
        private readonly Func<IKey> userKeyGetter;

        public UserCommandDispatcher(ICommandDispatcher inner, Func<IKey> userKeyGetter)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(userKeyGetter, "userKeyGetter");
            this.inner = inner;
            this.userKeyGetter = userKeyGetter;
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            Envelope envelope = command as Envelope;
            if (envelope == null)
                envelope = Envelope.Create<TCommand>(command);

            envelope.Metadata.Add("UserKey", userKeyGetter());
            return inner.HandleAsync(envelope);
        }
    }
}
