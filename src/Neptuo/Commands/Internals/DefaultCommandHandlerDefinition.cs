using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands.Internals
{
    /// <summary>
    /// Definition of command handler inside <see cref="DefaultCommandDispatcher"/>.
    /// </summary>
    internal class DefaultCommandHandlerDefinition
    {
        /// <summary>
        /// Command handler.
        /// Should never be <c>null</c>.
        /// </summary>
        public object CommandHandler { get; set; }
        
        public Func<object, Task> HandleMethod { get; set; }

        public DefaultCommandHandlerDefinition(object commandHandler, Func<object, Task> handleMethod)
        {
            CommandHandler = commandHandler;
            HandleMethod = handleMethod;
        }

        public Task HandleAsync(object command)
        {
            return HandleMethod(command);
        }
    }
}
