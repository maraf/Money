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
    public class CategoryEventHandler : IEventHandler<CategoryCreated>, IEventHandler<CategoryDeleted>, IEventHandler<CategoryRenamed>, IEventHandler<CategoryDescriptionChanged>
    {
        public Action Handler { get; set; }

        public CategoryEventHandler(Action handler)
        {
            Ensure.NotNull(handler, "handler");
            Handler = handler;
        }

        private Task RaiseHandler()
        {
            Handler();
            return Task.CompletedTask;
        }

        public Task HandleAsync(CategoryCreated payload) => RaiseHandler();
        public Task HandleAsync(CategoryDeleted payload) => RaiseHandler();
        public Task HandleAsync(CategoryRenamed payload) => RaiseHandler();
        public Task HandleAsync(CategoryDescriptionChanged payload) => RaiseHandler();
    }
}
