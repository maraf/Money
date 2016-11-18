using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Events.Handlers
{
    /// <summary>
    /// Factory class for event handlers from actions and functions.
    /// </summary>
    public static class DelegateEventHandler
    {
        /// <summary>
        /// Creates new instance using <paramref name="action"/>.
        /// </summary>
        /// <param name="action">Degate for handling events.</param>
        public static IEventHandler<TEvent> FromAction<TEvent>(Action<TEvent> action)
        {
            Ensure.NotNull(action, "action");
            return new EventHandler<TEvent>((payload) =>
            {
                action(payload);
                return Task.FromResult(true);
            });
        }

        /// <summary>
        /// Creates new instance using <paramref name="func"/>.
        /// </summary>
        /// <param name="func">Delegate for handling (possibly asynchronously) events.</param>
        public static IEventHandler<TEvent> FromFunc<TEvent>(Func<TEvent, Task> func)
        {
            Ensure.NotNull(func, "func");
            return new EventHandler<TEvent>(func);
        }

        private class EventHandler<TEvent> : IEventHandler<TEvent>
        {
            private readonly Func<TEvent, Task> func;

            public EventHandler(Func<TEvent, Task> func)
            {
                this.func = func;
            }

            public Task HandleAsync(TEvent payload)
            {
                return func(payload);
            }
        }
    }
}
