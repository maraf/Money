using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events.Handlers
{
    /// <summary>
    /// Default implmentation of <see cref="IEventHandlerContext{TEvent}"/>.
    /// </summary>
    /// <typeparam name="TEvent">Type of event data.</typeparam>
    public class DefaultEventHandlerContext<TEvent> : IEventHandlerContext<TEvent>
    {
        public Envelope<TEvent> Payload { get; private set; }
        public IEventHandlerCollection Handlers { get; private set; }
        public IEventDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Creates context from event data and wraps it into <see cref="Envelope{TEvent}"/>.
        /// </summary>
        /// <param name="payload">Event data.</param>
        /// <param name="handlers">Current collection of event subscriptions.</param>
        /// <param name="dispatcher">Current event dispatcher.</param>
        public DefaultEventHandlerContext(TEvent payload, IEventHandlerCollection handlers, IEventDispatcher dispatcher)
            : this(Envelope.Create(payload), handlers, dispatcher)
        { }

        /// <summary>
        /// Creates context from event data envelope.
        /// </summary>
        /// <param name="payload">Event data wrapped in envelope.</param>
        /// <param name="handlers">Current collection of event subscriptions.</param>
        /// <param name="dispatcher">Current event dispatcher.</param>
        public DefaultEventHandlerContext(Envelope<TEvent> payload, IEventHandlerCollection handlers, IEventDispatcher dispatcher)
        {
            Ensure.NotNull(payload, "payload");
            Ensure.NotNull(handlers, "eventHandlers");
            Ensure.NotNull(dispatcher, "dispatcher");
            Payload = payload;
            Handlers = handlers;
            Dispatcher = dispatcher;
        }
    }
}
