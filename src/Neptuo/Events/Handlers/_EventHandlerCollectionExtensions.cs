using Neptuo.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neptuo.Events.Handlers
{
    /// <summary>
    /// Common extensions for <see cref="IEventHandlerCollection"/>.
    /// </summary>
    public static class _EventHandlerCollectionExtensions
    {
        /// <summary>
        /// Returns continuation task that is completed when <paramref name="eventHandlers"/> gets
        /// notification for event of the type <typeparamref name="T"/> and <paramref name="filter"/> is satisfied.
        /// </summary>
        /// <typeparam name="T">The type of the event (can contain <see cref="Envelope"/> or <see cref="IEventHandlerContext{T}"/>).</typeparam>
        /// <param name="eventHandlers">The collection of an event handlers.</param>
        /// <param name="filter">The event filter, only the event for which returns true is used to complete the task.</param>
        /// <param name="token">The continuation task cancellation token.</param>
        /// <returns>
        /// Continuation task that is completed when <paramref name="eventHandlers"/> gets
        /// notification for event of the type <typeparamref name="T"/> and <paramref name="filter"/> is satisfied.
        /// </returns>
        public static Task<T> Await<T>(this IEventHandlerCollection eventHandlers, Func<T, bool> filter, CancellationToken token)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(filter, "filter");
            Ensure.NotNull(token, "token");

            TaskCompletionSource<T> source = new TaskCompletionSource<T>();
            eventHandlers.Add(new AwaitEventHandler<T>(eventHandlers, source, token, filter));

            return source.Task;
        }

        /// <summary>
        /// Returns continuation task that is completed when <paramref name="eventHandlers"/> gets
        /// notification for event of the type <typeparamref name="T"/> and <paramref name="filter"/> is satisfied.
        /// </summary>
        /// <typeparam name="T">The type of the event (can contain <see cref="Envelope"/> or <see cref="IEventHandlerContext{T}"/>).</typeparam>
        /// <param name="eventHandlers">The collection of an event handlers.</param>
        /// <param name="filter">The event filter, only the event for which returns true is used to complete the task.</param>
        /// <returns>
        /// Continuation task that is completed when <paramref name="eventHandlers"/> gets
        /// notification for event of the type <typeparamref name="T"/> and <paramref name="filter"/> is satisfied.
        /// </returns>
        public static Task<T> Await<T>(this IEventHandlerCollection eventHandlers, Func<T, bool> filter)
        {
            return Await(eventHandlers, filter, CancellationToken.None);
        }

        /// <summary>
        /// Returns continuation task that is completed when <paramref name="eventHandlers"/> gets
        /// notification for event of the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the event (can contain <see cref="Envelope"/> or <see cref="IEventHandlerContext{T}"/>).</typeparam>
        /// <param name="eventHandlers">The collection of an event handlers.</param>
        /// <param name="token">The continuation task cancellation token.</param>
        /// <returns>
        /// Continuation task that is completed when <paramref name="eventHandlers"/> gets
        /// notification for event of the type <typeparamref name="T"/>.
        /// </returns>
        public static Task<T> Await<T>(this IEventHandlerCollection eventHandlers, CancellationToken token)
        {
            return Await<T>(eventHandlers, e => true, token);
        }

        /// <summary>
        /// Returns continuation task that is completed when <paramref name="eventHandlers"/> gets
        /// notification for event of the type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the event (can contain <see cref="Envelope"/> or <see cref="IEventHandlerContext{T}"/>).</typeparam>
        /// <param name="eventHandlers">The collection of an event handlers.</param>
        /// <returns>
        /// Continuation task that is completed when <paramref name="eventHandlers"/> gets
        /// notification for event of the type <typeparamref name="T"/>.
        /// </returns>
        public static Task<T> Await<T>(this IEventHandlerCollection eventHandlers)
        {
            return Await<T>(eventHandlers, CancellationToken.None);
        }


        private class AwaitEventHandler<T> : IEventHandler<T>
        {
            private readonly IEventHandlerCollection collection;
            private readonly TaskCompletionSource<T> source;
            private readonly CancellationToken cancellation;
            private readonly Func<T, bool> filter;

            public AwaitEventHandler(IEventHandlerCollection collection, TaskCompletionSource<T> source, CancellationToken cancellation, Func<T, bool> filter)
            {
                Ensure.NotNull(collection, "collection");
                Ensure.NotNull(source, "source");
                Ensure.NotNull(cancellation, "cancellation");
                Ensure.NotNull(filter, "filter");
                this.collection = collection;
                this.source = source;
                this.cancellation = cancellation;
                this.filter = filter;

                cancellation.Register(OnCancellationRequested);
            }

            private void OnCancellationRequested()
            {
                collection.Remove(this);
                source.TrySetCanceled();
            }

            public Task HandleAsync(T payload)
            {
                if (filter(payload))
                {
                    collection.Remove(this);
                    source.SetResult(payload);
                }

                return Async.CompletedTask;
            }
        }

    }
}
