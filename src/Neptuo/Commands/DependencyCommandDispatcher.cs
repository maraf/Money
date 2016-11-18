using Neptuo.Activators;
using Neptuo.Commands.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// Command dispatcher based on registration from <see cref="IDependencyProvider"/>.
    /// </summary>
    public class DependencyCommandDispatcher : ICommandDispatcher
    {
        private IDependencyProvider dependencyProvider;

        /// <summary>
        /// Creates new instance with <paramref name="dependencyProvider"/>.
        /// </summary>
        /// <param name="dependencyProvider">Source of registrations.</param>
        public DependencyCommandDispatcher(IDependencyProvider dependencyProvider)
        {
            Ensure.NotNull(dependencyProvider, "dependencyProvider");
            this.dependencyProvider = dependencyProvider;
        }

        /// <summary>
        /// Handles <paramref name="command"/>.
        /// </summary>
        /// <param name="command">Command to handle.</param>
        public Task HandleAsync<TCommand>(TCommand command)
        {
            Ensure.NotNull(command, "command");
            ICommandHandler<TCommand> commandHandler = dependencyProvider.Resolve<ICommandHandler<TCommand>>();
            return commandHandler.HandleAsync(command);
        }
    }
}
