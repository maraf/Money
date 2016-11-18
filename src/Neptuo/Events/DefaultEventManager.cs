using Neptuo.Events.Handlers;
using Neptuo.Events.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events
{
    /// <summary>
    /// Default implementation of <see cref="IEventDispatcher"/> and <see cref="IEventHandlerCollection"/>.
    /// </summary>
    public class DefaultEventManager : IEventDispatcher, IEventHandlerCollection
    {
        private readonly TypeResolver eventTypeResolver;
        private readonly ThreeBranchStorage registry;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public DefaultEventManager()
        {
            eventTypeResolver = new TypeResolver(typeof(IEventHandlerContext<>));
            registry = new ThreeBranchStorage();
        }

        public Task PublishAsync<TEvent>(TEvent payload)
        {
            Ensure.NotNull(payload, "payload");

            Type eventType = typeof(TEvent);
            TypeResolverResult eventTypeDescriptor = eventTypeResolver.Resolve(eventType);

            // Throw, because passing in context is not supported.
            if (eventTypeDescriptor.IsContext)
                throw Ensure.Exception.NotSupported("Event manager can publish event context.");

            // Unwrap envelope, create context and invoke.
            if (eventTypeDescriptor.IsEnvelope)
            {
                Type contextType = typeof(DefaultEventHandlerContext<>).MakeGenericType(eventTypeDescriptor.DataType);
                object context = Activator.CreateInstance(contextType, payload, this, this);

                // Get publishing method and make it generic for current event.
                MethodInfo publishInternalMethod = typeof(DefaultEventManager).GetMethod("PublishInternalAsyc", BindingFlags.NonPublic | BindingFlags.Instance);
                if (publishInternalMethod == null)
                    throw Ensure.Exception.NotImplemented("Bug in implementation of DefaultEventManager. Unnable to find publishing method.");

                return (Task)publishInternalMethod.MakeGenericMethod(eventTypeDescriptor.DataType).Invoke(this, new object[] { context });
            }

            // Create context and invoke.
            return PublishInternalAsyc<TEvent>(new DefaultEventHandlerContext<TEvent>(payload, this, this));
        }

        private Task PublishInternalAsyc<TEvent>(IEventHandlerContext<TEvent> context)
        {
            Type eventType = typeof(TEvent);

            object[] contextHandlers = registry.GetContextHandlers(eventType);
            object[] envelopeHandlers = registry.GetEnvelopeHandlers(eventType);
            object[] directHandlers = registry.GetDirectHandlers(eventType);

            Task[] tasks = new Task[contextHandlers.Length + envelopeHandlers.Length + directHandlers.Length];

            if (tasks.Length == 0)
                return Task.FromResult(true);

            // Execute context handlers and store tasks.
            for (int i = 0; i < contextHandlers.Length; i++)
                tasks[i] = ((IEventHandler<IEventHandlerContext<TEvent>>)contextHandlers[i]).HandleAsync(context);

            // Execute envelope handlers and store tasks.
            for (int i = 0; i < envelopeHandlers.Length; i++)
                tasks[contextHandlers.Length + i] = ((IEventHandler<Envelope<TEvent>>)envelopeHandlers[i]).HandleAsync(context.Payload);

            // Execute direct handlers and store tasks.
            for (int i = 0; i < directHandlers.Length; i++)
                tasks[contextHandlers.Length + envelopeHandlers.Length + i] = ((IEventHandler<TEvent>)directHandlers[i]).HandleAsync(context.Payload.Body);

            // Return joined task.
            return Task.Factory.ContinueWhenAll(tasks, (items) => Task.FromResult(true));
        }

        public IEventHandlerCollection Add<TEvent>(IEventHandler<TEvent> handler)
        {
            Ensure.NotNull(handler, "handler");

            Type eventType = typeof(TEvent);
            TypeResolverResult eventTypeDescriptor = eventTypeResolver.Resolve(eventType);

            if (eventTypeDescriptor.IsContext)
                registry.AddContextHandler(eventTypeDescriptor.DataType, handler);
            else if (eventTypeDescriptor.IsEnvelope)
                registry.AddEnvelopeHandler(eventTypeDescriptor.DataType, handler);
            else
                registry.AddDirectHandler(eventTypeDescriptor.DataType, handler);

            return this;
        }

        public IEventHandlerCollection Remove<TEvent>(IEventHandler<TEvent> handler)
        {
            Ensure.NotNull(handler, "handler");

            Type eventType = typeof(TEvent);
            TypeResolverResult eventTypeDescriptor = eventTypeResolver.Resolve(eventType);

            if (eventTypeDescriptor.IsContext)
                registry.RemoveContextHandler(eventTypeDescriptor.DataType, handler);
            else if (eventTypeDescriptor.IsEnvelope)
                registry.RemoveEnvelopeHandler(eventTypeDescriptor.DataType, handler);
            else
                registry.RemoveDirectHandler(eventTypeDescriptor.DataType, handler);

            return this;
        }
    }
}
