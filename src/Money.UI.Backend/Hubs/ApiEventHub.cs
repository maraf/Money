using Microsoft.AspNetCore.SignalR;
using Money.Events;
using Money.Models.Api;
using Money.Services;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Formatters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Hubs
{
    public class ApiEventHub : Hub, IEventHandler<CategoryCreated>, IEventHandler<CategoryDeleted>, IEventHandler<CategoryRenamed>, IEventHandler<CategoryDescriptionChanged>
    {
        private readonly Formatters formatters;

        public ApiEventHub(IEventHandlerCollection eventHandlers, Formatters formatters)
        {
            Ensure.NotNull(eventHandlers, "eventHandlers");
            Ensure.NotNull(formatters, "formatters");
            this.formatters = formatters;
            eventHandlers.AddAll(this);
        }

        private Task RaiseEvent<T>(T payload)
        {
            string type = typeof(T).AssemblyQualifiedName;
            string rawPayload = formatters.Event.Serialize(payload);

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
    }
}
