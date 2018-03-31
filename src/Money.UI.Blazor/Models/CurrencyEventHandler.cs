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
    public class CurrencyEventHandler : IEventHandler<CurrencyCreated>, IEventHandler<CurrencySymbolChanged>, IEventHandler<CurrencyDefaultChanged>, IEventHandler<CurrencyDeleted>
    {
        public Action Handler { get; set; }

        public CurrencyEventHandler(Action handler)
        {
            Ensure.NotNull(handler, "handler");
            Handler = handler;
        }

        private Task RaiseHandler()
        {
            Handler();
            return Task.CompletedTask;
        }

        public Task HandleAsync(CurrencyCreated payload) => RaiseHandler();
        public Task HandleAsync(CurrencySymbolChanged payload) => RaiseHandler();
        public Task HandleAsync(CurrencyDefaultChanged payload) => RaiseHandler();
        public Task HandleAsync(CurrencyDeleted payload) => RaiseHandler();
    }
}
