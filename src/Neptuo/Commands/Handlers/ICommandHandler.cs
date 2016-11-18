using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands.Handlers
{
    /// <summary>
    /// Command handler contract.
    /// </summary>
    /// <typeparam name="TCommand">Type of command to handle.</typeparam>
    public interface ICommandHandler<TCommand>
    {
        /// <summary>
        /// Handles <paramref name="command"/>.
        /// </summary>
        /// <param name="command">Command to handle.</param>
        Task HandleAsync(TCommand command);
    }
}
