using Microsoft.AspNetCore.Blazor.Components;
using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public class CategoriesBase : BlazorComponent,
        IDisposable,
        IEventHandler<CategoryCreated>,
        IEventHandler<CategoryRenamed>,
        IEventHandler<CategoryDescriptionChanged>,
        IEventHandler<CategoryIconChanged>,
        IEventHandler<CategoryColorChanged>,
        IEventHandler<CategoryDeleted>
    {
        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        protected bool IsCreateVisible { get; set; }
        protected bool IsNameEditVisible { get; set; }
        protected bool IsIconEditVisible { get; set; }

        protected List<CategoryModel> Models { get; private set; } = new List<CategoryModel>();
        protected CategoryModel Selected { get; set; }

        protected override async Task OnInitAsync()
        {
            BindEvents();
            await LoadDataAsync();
        }

        protected async void OnEvent()
        {
            await LoadDataAsync();
            StateHasChanged();
        }

        protected async Task LoadDataAsync()
            => Models = await Queries.QueryAsync(new ListAllCategory());

        protected async void OnDeleteClick(CategoryModel model)
            => await Commands.HandleAsync(new DeleteCategory(model.Key));

        public void Dispose()
            => UnBindEvents();

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<CategoryCreated>(this)
                .Add<CategoryRenamed>(this)
                .Add<CategoryDescriptionChanged>(this)
                .Add<CategoryIconChanged>(this)
                .Add<CategoryColorChanged>(this)
                .Add<CategoryDeleted>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<CategoryCreated>(this)
                .Remove<CategoryRenamed>(this)
                .Remove<CategoryDescriptionChanged>(this)
                .Remove<CategoryIconChanged>(this)
                .Remove<CategoryColorChanged>(this)
                .Remove<CategoryDeleted>(this);
        }

        Task IEventHandler<CategoryCreated>.HandleAsync(CategoryCreated payload)
        {
            // TODO: We can do even better.
            OnEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryRenamed>.HandleAsync(CategoryRenamed payload)
        {
            // TODO: We can do even better.
            OnEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryDescriptionChanged>.HandleAsync(CategoryDescriptionChanged payload)
        {
            // TODO: We can do even better.
            OnEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryIconChanged>.HandleAsync(CategoryIconChanged payload)
        {
            // TODO: We can do even better.
            OnEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryColorChanged>.HandleAsync(CategoryColorChanged payload)
        {
            // TODO: We can do even better.
            OnEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryDeleted>.HandleAsync(CategoryDeleted payload)
        {
            CategoryModel model = Models.FirstOrDefault(c => c.Key.Equals(payload.AggregateKey));
            if (model != null)
                Models.Remove(model);

            StateHasChanged();
            return Task.CompletedTask;
        }

        #endregion
    }
}
