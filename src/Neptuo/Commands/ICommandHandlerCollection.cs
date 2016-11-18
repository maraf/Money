using Neptuo.Commands.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// Collection of registered command handlers.
    /// </summary>
    public interface ICommandHandlerCollection
    {
        /// <summary>
        /// Registers <paramref name="handler"/> to handle commands of type <typeparamref name="TCommand"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of command.</typeparam>
        /// <param name="handler">Handler for commands of type <typeparamref name="TCommand"/>.</param>
        /// <returns>Self (for fluency).</returns>
        ICommandHandlerCollection Add<TCommand>(ICommandHandler<TCommand> handler);

        /// <summary>
        /// Tries to find query handler for query of type <typeparamref name="TQuery"/>.
        /// </summary>
        /// <typeparam name="TCommand">Type of command.</typeparam>
        /// <param name="handler">Handler for commands of type <typeparamref name="TCommand"/>.</param>
        /// <returns><c>true</c> if such a handler is registered; <c>false</c> otherwise.</returns>
        bool TryGet<TCommand>(out ICommandHandler<TCommand> handler);
    }
}
