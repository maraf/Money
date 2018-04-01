using Money.Events;
using Neptuo;
using Neptuo.Events.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models
{
    public class OutcomeEventHandler : IEventHandler<OutcomeCreated>, IEventHandler<OutcomeDeleted>, IEventHandler<OutcomeAmountChanged>, IEventHandler<OutcomeDescriptionChanged>, IEventHandler<OutcomeWhenChanged>
    {
        public Action Handler { get; set; }

        public OutcomeEventHandler(Action handler)
        {
            Ensure.NotNull(handler, "handler");
            Handler = handler;
        }

        private Task RaiseHandler()
        {
            Handler();
            return Task.CompletedTask;
        }

        public Task HandleAsync(OutcomeCreated payload) => RaiseHandler();
        public Task HandleAsync(OutcomeDeleted payload) => RaiseHandler();
        public Task HandleAsync(OutcomeAmountChanged payload) => RaiseHandler();
        public Task HandleAsync(OutcomeDescriptionChanged payload) => RaiseHandler();
        public Task HandleAsync(OutcomeWhenChanged payload) => RaiseHandler();
    }
}
