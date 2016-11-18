using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// Dispatcher for handling commands.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Posts <paramref name="command"/> to command infrastructure for execution.
        /// </summary>
        /// <remarks>
        /// Based on concrete implementation, returning from this may mean:
        /// 1) Command was successfully executed,
        /// 2) command was successfully stored for late (asynchronous) execution.
        /// For more on this beahavior, see concrete implementation.
        /// </remarks>
        /// <param name="command">Instance describing requested operation.</param>
        /// <returns>Continuation task.</returns>
        Task HandleAsync<TCommand>(TCommand command);
    }
}
