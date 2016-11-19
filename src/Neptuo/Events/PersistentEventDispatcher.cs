using Neptuo.Data;
using Neptuo.Events.Handlers;
using Neptuo.Exceptions;
using Neptuo.Formatters;
using Neptuo.Internals;
using Neptuo.Linq.Expressions;
using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Events
{
    /// <summary>
    /// The implementation of <see cref="IEventDispatcher"/> and <see cref="IEventHandlerCollection"/> with persistent delivery.
    /// </summary>
    public partial class PersistentEventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, HashSet<HandlerDescriptor>> storage = new Dictionary<Type, HashSet<HandlerDescriptor>>();
        private readonly IEventPublishingStore store;
        private readonly ISchedulingProvider schedulingProvider;
        private readonly HandlerDescriptorProvider descriptorProvider;

        /// <summary>
        /// The collection of event handlers.
        /// </summary>
        public IEventHandlerCollection Handlers { get; private set; }

        /// <summary>
        /// The collection of exception handlers for exceptions from the event processing.
        /// </summary>
        public IExceptionHandlerCollection EventExceptionHandlers { get; private set; }

        /// <summary>
        /// The collection of exception handlers for exceptions from the infrastructure.
        /// </summary>
        public IExceptionHandlerCollection DispatcherExceptionHandlers { get; private set; }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="store">The publishing store for command persistent delivery.</param>
        public PersistentEventDispatcher(IEventPublishingStore store)
            : this(store, new TimerSchedulingProvider(new TimerSchedulingProvider.DateTimeNowProvider()))
        { }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="store">The publishing store for command persistent delivery.</param>
        /// <param name="schedulingProvider">The provider of a delay computation for delayed events.</param>
        public PersistentEventDispatcher(IEventPublishingStore store, ISchedulingProvider schedulingProvider)
        {
            Ensure.NotNull(store, "store");
            Ensure.NotNull(schedulingProvider, "schedulingProvider");
            this.store = store;
            this.schedulingProvider = schedulingProvider;

            EventExceptionHandlers = new DefaultExceptionHandlerCollection();
            DispatcherExceptionHandlers = new DefaultExceptionHandlerCollection();

            this.descriptorProvider = new HandlerDescriptorProvider(
                typeof(IEventHandler<>),
                typeof(IEventHandlerContext<>),
                TypeHelper.MethodName<IEventHandler<object>, object, Task>(h => h.HandleAsync),
                EventExceptionHandlers,
                DispatcherExceptionHandlers
            );

            Handlers = new HandlerCollection(storage, descriptorProvider);
        }

        public Task PublishAsync<TEvent>(TEvent payload)
        {
            Ensure.NotNull(payload, "payload");

            ArgumentDescriptor argument = descriptorProvider.Get(payload);
            HashSet<HandlerDescriptor> handlers;
            if (storage.TryGetValue(argument.ArgumentType, out handlers))
                return PublishAsync(handlers, argument, payload, true);

            return Async.CompletedTask;
        }

        private Task PublishAsync(IEnumerable<HandlerDescriptor> handlers, ArgumentDescriptor argument, object eventPayload, bool isEnvelopeDelayUsed)
        {
            try
            {
                bool hasContextHandler = handlers.Any(d => d.IsContext);
                bool hasEnvelopeHandler = hasContextHandler || handlers.Any(d => d.IsEnvelope);

                object payload = eventPayload;
                object context = null;
                Envelope envelope = null;

                IEvent eventWithKey = null;
                if (argument.IsContext)
                {
                    // If passed argument is context, throw.
                    throw Ensure.Exception.NotSupported("PersistentEventDispatcher doesn't support passing in event handler context.");
                }
                else
                {
                    // If passed argument is not context, try to create it if needed.
                    if (argument.IsEnvelope)
                    {
                        // If passed argument is envelope, extract payload.
                        envelope = (Envelope)payload;
                        payload = envelope.Body;
                    }
                    else
                    {
                        eventWithKey = payload as IEvent;
                        hasEnvelopeHandler = hasEnvelopeHandler || eventWithKey != null;

                        // If passed argument is not envelope, try to create it if needed.
                        if (hasEnvelopeHandler)
                            envelope = EnvelopeFactory.Create(payload, argument.ArgumentType);
                    }

                    if (hasContextHandler)
                    {
                        Type contextType = typeof(DefaultEventHandlerContext<>).MakeGenericType(argument.ArgumentType);
                        context = Activator.CreateInstance(contextType, envelope, this, this);
                    }
                }

                if (eventWithKey == null)
                    eventWithKey = payload as IEvent;

                // If isEnvelopeDelayUsed, try to schedule the execution.
                // If succeeded, return.
                if (isEnvelopeDelayUsed && TrySchedule(envelope, context, handlers, argument))
                    return Async.CompletedTask;

                // Distribute the execution.
                return DistributeExecution(payload, context, envelope, eventWithKey, handlers);
            }
            catch (Exception e)
            {
                DispatcherExceptionHandlers.Handle(e);
                return Async.CompletedTask;
            }
        }

        private bool TrySchedule(Envelope envelope, object handlerContext, IEnumerable<HandlerDescriptor> handlers, ArgumentDescriptor argument)
        {
            if (schedulingProvider.IsLaterExecutionRequired(envelope))
            {
                ScheduleEventContext context = new ScheduleEventContext(handlers, argument, envelope, handlerContext, OnScheduledEvent);
                schedulingProvider.Schedule(context);
                return true;
            }

            return false;
        }

        private Task DistributeExecution(object payload, object context, Envelope envelope, IEvent eventWithKey, IEnumerable<HandlerDescriptor> handlers)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (HandlerDescriptor handler in handlers.ToList())
                {
                    try
                    {
                        if (handler.IsContext)
                            handler.Execute(context, null).Wait();
                        else if (handler.IsEnvelope)
                            handler.Execute(envelope, null).Wait();
                        else if (handler.IsPlain)
                            handler.Execute(payload, null).Wait();
                        else
                            throw Ensure.Exception.UndefinedHandlerType(handler);

                        if (eventWithKey != null && handler.HandlerIdentifier != null)
                            store.PublishedAsync(eventWithKey.Key, handler.HandlerIdentifier).Wait();
                    }
                    catch (Exception e)
                    {
                        DispatcherExceptionHandlers.Handle(e);
                    }
                }
            });
        }

        /// <summary>
        /// Raised from the <see cref="ScheduleEventContext.Execute"/> when scheduling provider deems.
        /// </summary>
        /// <param name="context">The context publish.</param>
        private void OnScheduledEvent(ScheduleEventContext context)
        {
            DistributeExecution(
                context.Envelope.Body,
                context.HandlerContext,
                context.Envelope,
                context.Envelope.Body as IEvent,
                context.Handlers
            ).Wait();
        }

        /// <summary>
        /// Re-publishes events from unpublished queue.
        /// Uses <paramref name="formatter"/> to deserialize events from store.
        /// </summary>
        /// <param name="formatter">The event deserializer.</param>
        /// <returns>The continuation task.</returns>
        public async Task RecoverAsync(IDeserializer formatter)
        {
            IEnumerable<EventPublishingModel> models = await store.GetAsync();
            foreach (EventPublishingModel model in models)
            {
                IEvent eventModel = formatter.DeserializeEvent(Type.GetType(model.Event.EventKey.Type), model.Event.Payload);
                await RecoverEventAsync(eventModel, model.PublishedHandlerIdentifiers);
            }

            await store.ClearAsync();
        }

        private async Task RecoverEventAsync(IEvent model, IEnumerable<string> handlerIdentifiers)
        {
            ArgumentDescriptor argument = descriptorProvider.Get(model.GetType());

            HashSet<HandlerDescriptor> handlers;
            if (storage.TryGetValue(argument.ArgumentType, out handlers))
            {
                IEnumerable<HandlerDescriptor> unPublishedHandlers = handlers.Where(h => !handlerIdentifiers.Contains(h.HandlerIdentifier));
                if (unPublishedHandlers.Any())
                    await PublishAsync(unPublishedHandlers, argument, model, true);
            }
        }

        #region Usefull methods from rebuilding read models

        internal IEnumerable<Type> EnumerateEventTypes()
        {
            return storage.Keys;
        }

        #endregion
    }
}
