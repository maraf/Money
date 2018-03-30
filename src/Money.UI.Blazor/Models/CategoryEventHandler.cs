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
    public class CategoryEventHandler : IEventHandler<CategoryCreated>, IEventHandler<CategoryDeleted>
    {
        public Action Handler { get; set; }

        public CategoryEventHandler(Action handler)
        {
            Ensure.NotNull(handler, "handler");
            Handler = handler;
        }

        public Task HandleAsync(CategoryCreated payload)
        {
            Handler();
            return Task.CompletedTask;
        }

        public Task HandleAsync(CategoryDeleted payload)
        {
            Handler();
            return Task.CompletedTask;
        }
    }
}
