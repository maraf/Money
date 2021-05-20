using Neptuo;
using Neptuo.Commands;
using Neptuo.Commands.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Commands
{
    internal class LocalCommandDispatcher : ICommandDispatcher
    {
        private readonly OfflineCommandDispatcher inner;
        private readonly ICommandHandlerCollection handlers;

        public LocalCommandDispatcher(OfflineCommandDispatcher inner, ICommandHandlerCollection handlers)
        {
            Ensure.NotNull(inner, "inner");
            Ensure.NotNull(handlers, "handlers");
            this.inner = inner;
            this.handlers = handlers;
        }

        public async Task HandleAsync<TCommand>(TCommand command)
        {
            if (handlers.TryGet<TCommand>(out var handler))
            {
                _ = handler.HandleAsync(command);
                return;
            }

            await inner.HandleAsync(command);
        }
    }

    public class LocalCommandHandlerCollection : ICommandHandlerCollection
    {
        private readonly Dictionary<Type, object> storage = new Dictionary<Type, object>();

        public ICommandHandlerCollection Add<TCommand>(ICommandHandler<TCommand> handler)
        {
            Ensure.NotNull(handler, "handler");
            storage[typeof(TCommand)] = handler;
            return this;
        }

        public bool TryGet<TCommand>(out ICommandHandler<TCommand> handler)
        {
            if (storage.TryGetValue(typeof(TCommand), out var baseHandler))
            {
                handler = (ICommandHandler<TCommand>)baseHandler;
                return true;
            }

            handler = null;
            return false;
        }
    }
}
