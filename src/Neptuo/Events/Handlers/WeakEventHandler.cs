using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events.Handlers
{
    /// <summary>
    /// Wrapper for <see cref="IEventHandler{TEvent}"/> to
    /// support inner handler garbage collection with auto-unsubscribe.
    /// </summary>
    /// <typeparam name="TEvent">Type of event data.</typeparam>
    public class WeakEventHandler<TEvent> : IEventHandler<IEventHandlerContext<TEvent>>
    {
        private readonly WeakReference<IEventHandler<TEvent>> innerHandler;

        /// <summary>
        /// Creates new instance to wrap <paramref name="innerHandler"/>.
        /// </summary>
        /// <param name="innerHandler">Event handler to handle events of type <typeparamref name="TEvent"/>.</param>
        public WeakEventHandler(IEventHandler<TEvent> innerHandler)
        {
            Ensure.NotNull(innerHandler, "innerHandler");
            this.innerHandler = new WeakReference<IEventHandler<TEvent>>(innerHandler);
        }

        public Task HandleAsync(IEventHandlerContext<TEvent> context)
        {
            IEventHandler<TEvent> target;
            if (innerHandler.TryGetTarget(out target))
                return target.HandleAsync(context.Payload.Body);
            else
                context.Handlers.Remove(this);

            return Async.CompletedTask;
        }
    }
}
