using Microsoft.AspNetCore.Blazor.Components;
using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Models.Confirmation;
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
        protected bool IsNameEditVisible;
        protected bool IsIconEditVisible;
        protected bool IsColorEditVisible;

        protected List<CategoryModel> Models { get; private set; } = new List<CategoryModel>();
        protected CategoryModel Selected { get; set; }
        protected DeleteContext<CategoryModel> Delete { get; } = new DeleteContext<CategoryModel>();

        protected override async Task OnInitAsync()
        {
            BindEvents();
            Delete.Confirmed += async model => await Commands.HandleAsync(new DeleteCategory(model.Key));
            Delete.MessageFormatter = model => $"Do you really want to delete category '{model.Name}'?";
            await LoadDataAsync();
        }

        protected async void Reload()
        {
            await LoadDataAsync();
            StateHasChanged();
        }

        protected async Task LoadDataAsync()
            => Models = await Queries.QueryAsync(new ListAllCategory());

        protected void OnActionClick(CategoryModel model, ref bool isVisible)
        {
            Selected = model;
            isVisible = true;
            StateHasChanged();
        }

        protected void OnDeleteClick(CategoryModel model)
        {
            Delete.Model = model;
            StateHasChanged();
        }

        private CategoryModel FindModel(IEvent payload)
            => Models.FirstOrDefault(c => c.Key.Equals(payload.AggregateKey));

        private void SortModels()
            => Models.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));

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

        private Task UpdateModel(IEvent payload, Action<CategoryModel> handler)
        {
            CategoryModel model = FindModel(payload);
            if (model != null)
            {
                handler(model);
                StateHasChanged();
            }
            else
            {
                Reload();
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryCreated>.HandleAsync(CategoryCreated payload)
        {
            Models.Add(new CategoryModel(payload.AggregateKey, payload.Name, null, payload.Color, null));
            SortModels();
            return Task.CompletedTask;
        }

        Task IEventHandler<CategoryRenamed>.HandleAsync(CategoryRenamed payload)
            => UpdateModel(payload, model =>
            {
                model.Name = payload.NewName;
                SortModels();
            });

        Task IEventHandler<CategoryDescriptionChanged>.HandleAsync(CategoryDescriptionChanged payload)
            => UpdateModel(payload, model => model.Description = payload.Description);

        Task IEventHandler<CategoryIconChanged>.HandleAsync(CategoryIconChanged payload)
            => UpdateModel(payload, model => model.Icon = payload.Icon);

        Task IEventHandler<CategoryColorChanged>.HandleAsync(CategoryColorChanged payload)
            => UpdateModel(payload, model => model.Color = payload.Color);

        Task IEventHandler<CategoryDeleted>.HandleAsync(CategoryDeleted payload)
            => UpdateModel(payload, model => Models.Remove(model));

        #endregion
    }
}
