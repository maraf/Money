using Microsoft.AspNetCore.SignalR;
using Money.Events;
using Money.Models.Api;
using Money.Services;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Exceptions.Handlers;
using Neptuo.Formatters;
using Neptuo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Hubs
{
    public class ApiHub : Hub, 
        IExceptionHandler<AggregateRootException>,
        IEventHandler<CategoryCreated>, IEventHandler<CategoryDeleted>, IEventHandler<CategoryRenamed>, IEventHandler<CategoryDescriptionChanged>, IEventHandler<CategoryIconChanged>, IEventHandler<CategoryColorChanged>,
        IEventHandler<CurrencyCreated>, IEventHandler<CurrencyDeleted>, IEventHandler<CurrencyDefaultChanged>, IEventHandler<CurrencySymbolChanged>
    {
        private readonly FormatterContainer formatters;

        public ApiHub(IEventHandlerCollection eventHandlers, FormatterContainer formatters, ExceptionHandlerBuilder exceptionHandlerBuilder)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(formatters, "formatters");
            this.formatters = formatters;
            eventHandlers.AddAll(this);
            exceptionHandlerBuilder.Filter<AggregateRootException>().Handler(this);
        }

        private Task RaiseEvent<T>(T payload)
        {
            string type = typeof(T).AssemblyQualifiedName;
            string rawPayload = formatters.Event.Serialize(payload);

            // TODO: Per-user.
            Clients.All.SendAsync("RaiseEvent", JsonConvert.SerializeObject(new Response()
            {
                type = type,
                payload = rawPayload
            }));

            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryCreated>.HandleAsync(CategoryCreated payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryDeleted>.HandleAsync(CategoryDeleted payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryRenamed>.HandleAsync(CategoryRenamed payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryDescriptionChanged>.HandleAsync(CategoryDescriptionChanged payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryIconChanged>.HandleAsync(CategoryIconChanged payload) => RaiseEvent(payload);
        Task IEventHandler<CategoryColorChanged>.HandleAsync(CategoryColorChanged payload) => RaiseEvent(payload);

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload) => RaiseEvent(payload);
        Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload) => RaiseEvent(payload);
        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload) => RaiseEvent(payload);
        Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload) => RaiseEvent(payload);

        public void Handle(AggregateRootException exception)
        {
            string type = exception.GetType().AssemblyQualifiedName;
            string rawPayload = formatters.Exception.Serialize(exception);

            // TODO: Per-user.
            Clients.All.SendAsync("RaiseException", JsonConvert.SerializeObject(new Response()
            {
                type = type,
                payload = rawPayload
            }));
        }
    }
}
