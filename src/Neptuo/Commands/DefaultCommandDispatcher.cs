using Neptuo.Commands.Handlers;
using Neptuo.Commands.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// Default implementation of <see cref="ICommandDispatcher"/> and <see cref="ICommandHandlerCollection"/>.
    /// When handling command and command handler is missing, exception is thrown.
    /// </summary>
    public class DefaultCommandDispatcher : ICommandHandlerCollection, ICommandDispatcher
    {
        private readonly Dictionary<Type, DefaultCommandHandlerDefinition> storage = new Dictionary<Type, DefaultCommandHandlerDefinition>();

        public ICommandHandlerCollection Add<TCommand>(ICommandHandler<TCommand> handler)
        {
            Ensure.NotNull(handler, "handler");
            Type commandType = typeof(TCommand);
            storage[commandType] = new DefaultCommandHandlerDefinition(handler, command => handler.HandleAsync((TCommand)command));

            return this;
        }

        public bool TryGet<TCommand>(out ICommandHandler<TCommand> handler)
        {
            Type commandType = typeof(TCommand);
            DefaultCommandHandlerDefinition definition;
            if (storage.TryGetValue(commandType, out definition))
            {
                handler = (ICommandHandler<TCommand>)definition.CommandHandler;
                return true;
            }

            handler = null;
            return false;
        }

        public Task HandleAsync<TCommand>(TCommand command)
        {
            Ensure.NotNull(command, "command");
            Type commandType = command.GetType();

            DefaultCommandHandlerDefinition definition;
            if (storage.TryGetValue(commandType, out definition))
                return definition.HandleAsync(command);

            throw Ensure.Exception.ArgumentOutOfRange("command", "There isn't command handler for command of type '{0}'.", commandType.FullName);
        }
    }
}
