using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Commands
{
    /// <summary>
    /// Raised when command handler is not found 
    /// </summary>
    public class MissingCommandHandlerException : Exception
    {
        /// <summary>
        /// The type of the command.
        /// </summary>
        public Type CommandType { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="commandType">The type of the command.</param>
        public MissingCommandHandlerException(Type commandType)
            : base(String.Format("Missing a command handler for a command of the type '{0}'.", commandType.FullName))
        {
            CommandType = commandType;
        }
    }
}
