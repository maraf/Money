using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Internals
{
    /// <summary>
    /// Provides description of (command or event) handler.
    /// </summary>
    internal class HandlerDescriptor : ArgumentDescriptor
    {
        /// <summary>
        /// The unique handler identifier.
        /// </summary>
        public string HandlerIdentifier { get; private set; }

        /// <summary>
        /// The instance itself.
        /// </summary>
        public object Handler { get; private set; }

        /// <summary>
        /// The method that executes handler with parameter.
        /// </summary>
        protected Func<object, object, Action<Exception>, Task> ExecuteMethod { get; private set; }


        /// <summary>
        /// Creates new instance.
        /// </summary>
        public HandlerDescriptor(string handlerIdentifier, object handler, Type argumentType, Func<object, object, Action<Exception>, Task> execute, bool isPlain, bool isEnvelope, bool isContext)
            : base(argumentType, isPlain, isEnvelope, isContext)
        {
            Ensure.NotNull(handler, "handler");
            Ensure.NotNull(execute, "execute");
            HandlerIdentifier = handlerIdentifier;
            Handler = handler;
            ExecuteMethod = execute;
        }

        /// <summary>
        /// Executes handler method with <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The argument to the handle method.</param>
        /// <param name="additionalExceptionDecorator">The delegate for decorating raised exceptions.</param>
        public Task Execute(object parameter, Action<Exception> additionalExceptionDecorator)
        {
            return ExecuteMethod(Handler, parameter, additionalExceptionDecorator);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 7;
                hash = 13 * hash * Handler.GetHashCode();
                hash = 13 * hash * ArgumentType.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            HandlerDescriptor other = obj as HandlerDescriptor;
            if (other == null)
                return false;

            return Handler == other.Handler && ArgumentType == other.ArgumentType;
        }
    }
}
