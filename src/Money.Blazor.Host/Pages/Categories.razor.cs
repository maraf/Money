using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
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
    public partial class Categories :
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

        protected CategoryName CreateModal { get; set; }
        protected CategoryName NameModal { get; set; }
        protected CategoryIcon IconModal { get; set; }
        protected CategoryColor ColorModal { get; set; }

        protected List<CategoryModel> Models { get; private set; } = new List<CategoryModel>();
        protected CategoryModel Selected { get; set; }
        protected LoadingContext Loading { get; } = new LoadingContext();

        protected string DeleteMessage { get; set; }
        protected Confirm DeleteConfirm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            BindEvents();

            using (Loading.Start())
                await LoadDataAsync();
        }

        protected async void Reload()
        {
            await LoadDataAsync();
            StateHasChanged();
        }

        protected async Task LoadDataAsync()
            => Models = await Queries.QueryAsync(new ListAllCategory());

        protected void OnActionClick(CategoryModel model, ModalDialog modal)
        {
            Selected = model;
            modal.Show();
            StateHasChanged();
        }

        protected void OnDeleteClick(CategoryModel model)
        {
            Selected = model;
            DeleteMessage = $"Do you really want to delete category '{model.Name}'?";
            DeleteConfirm.Show();
            StateHasChanged();
        }

        protected async void OnDeleteConfirmed()
        {
            await Commands.HandleAsync(new DeleteCategory(Selected.Key));
            StateHasChanged();
        }

        protected string GetItemCssClass(CategoryModel item, string length10, string length30, string length50, string length100, string lengthX)
        {
            int GetLength(string text) => text?.Length ?? 0;

            int length = GetLength(item.Name) + GetLength(item.Description);
            if (length < 10)
                return length10;
            else if (length < 30)
                return length30;
            else if (length < 50)
                return length50;
            else if (length < 100)
                return length100;
            else
                return lengthX;
        }

        protected string GetItemFlexCssClass(CategoryModel item)
            => GetItemCssClass(item, "flex-row", "flex-column flex-sm-row", "flex-column flex-md-row", "flex-column flex-lg-row", "flex-column");

        protected string GetItemButtonsMarginCssClass(CategoryModel item)
            => GetItemCssClass(item, "mt-0", "mt-2 mt-sm-0", "mt-2 mt-md-0", "mt-2 mt-lg-0", "mt-2");

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
