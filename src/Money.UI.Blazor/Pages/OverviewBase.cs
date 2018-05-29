using Microsoft.AspNetCore.Blazor.Components;
using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Models.Confirmation;
using Money.Models.Queries;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public class OverviewBase : BlazorComponent,
        System.IDisposable,
        IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeDeleted>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeDescriptionChanged>,
        IEventHandler<OutcomeWhenChanged>
    {
        private CurrencyFormatter formatter;

        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        protected MonthModel MonthModel { get; set; }
        protected IKey CategoryKey { get; set; }
        protected string CategoryName { get; set; }
        protected List<OutcomeOverviewModel> Models { get; set; }
        protected OutcomeOverviewModel Selected { get; set; }

        protected bool IsCreateVisible { get; set; }
        protected bool IsAmountEditVisible { get; set; }
        protected bool IsDescriptionEditVisible { get; set; }
        protected bool IsWhenEditVisible { get; set; }

        protected DeleteContext<OutcomeOverviewModel> Delete { get; } = new DeleteContext<OutcomeOverviewModel>();

        [Parameter]
        protected string Year { get; set; }

        [Parameter]
        protected string Month { get; set; }

        [Parameter]
        protected string CategoryGuid { get; set; }

        protected override async Task OnInitAsync()
        {
            BindEvents();
            Delete.Confirmed += async model => await Commands.HandleAsync(new DeleteOutcome(model.Key));
            Delete.MessageFormatter = model => $"Do you really want to delete outcome '{model.Description}'?";

            MonthModel = new MonthModel(Int32.Parse(Year), Int32.Parse(Month));
            CategoryKey = Guid.TryParse(CategoryGuid, out var categoryGuid) ? GuidKey.Create(categoryGuid, KeyFactory.Empty(typeof(Category)).Type) : KeyFactory.Empty(typeof(Category));
            formatter = new CurrencyFormatter(await Queries.QueryAsync(new ListAllCurrency()));
            await LoadDataAsync();
        }

        protected async void Reload()
        {
            await LoadDataAsync();
            StateHasChanged();
        }

        protected async Task LoadDataAsync()
        {
            Models = await Queries.QueryAsync(new ListMonthOutcomeFromCategory(CategoryKey, MonthModel));
            if (!CategoryKey.IsEmpty)
                CategoryName = await Queries.QueryAsync(new GetCategoryName(CategoryKey));
        }

        protected void OnDeleteClick(OutcomeOverviewModel model)
            => Delete.Model = model;

        protected OutcomeOverviewModel FindModel(IEvent payload)
            => Models.FirstOrDefault(o => o.Key.Equals(payload.AggregateKey));

        private void SortModels()
            => Models.Sort((c1, c2) => c1.When.CompareTo(c2.When));

        protected string FormatPrice(Price price)
            => formatter.Format(price);

        public void Dispose()
            => UnBindEvents();

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<OutcomeCreated>(this)
                .Add<OutcomeDeleted>(this)
                .Add<OutcomeAmountChanged>(this)
                .Add<OutcomeDescriptionChanged>(this)
                .Add<OutcomeWhenChanged>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<OutcomeCreated>(this)
                .Remove<OutcomeDeleted>(this)
                .Remove<OutcomeAmountChanged>(this)
                .Remove<OutcomeDescriptionChanged>(this)
                .Remove<OutcomeWhenChanged>(this);
        }

        private Task UpdateModel(IEvent payload, Action<OutcomeOverviewModel> handler)
        {
            OutcomeOverviewModel model = FindModel(payload);
            if (model != null)
            {
                handler(model);
                StateHasChanged();
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        {
            MonthModel payloadMonth = payload.When;
            if (MonthModel == payloadMonth)
                Reload();

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
            => UpdateModel(payload, model => Models.Remove(model));

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
            => UpdateModel(payload, model => model.Amount = payload.NewValue);

        Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload)
            => UpdateModel(payload, model => model.Description = payload.Description);

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            OutcomeOverviewModel model = FindModel(payload);
            if (model != null && model.When.Year == payload.When.Year && model.When.Month == payload.When.Month)
            {
                model.When = payload.When;
                SortModels();
                StateHasChanged();
            }
            else
            {
                Reload();
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
