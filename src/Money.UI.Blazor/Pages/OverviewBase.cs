using Microsoft.AspNetCore.Blazor.Components;
using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Neptuo;
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
    public class OverviewBase : BlazorComponent,
        System.IDisposable,
        IEventHandler<OutcomeCreated>, 
        IEventHandler<OutcomeDeleted>, 
        IEventHandler<OutcomeAmountChanged>, 
        IEventHandler<OutcomeDescriptionChanged>, 
        IEventHandler<OutcomeWhenChanged>
    {
        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        protected MonthModel MonthModel { get; set; }
        protected List<OutcomeOverviewModel> Models { get; set; }
        protected OutcomeOverviewModel Selected { get; set; }

        protected bool IsCreateVisible { get; set; }
        protected bool IsDescriptionEditVisible { get; set; }

        public string Year { get; set; }
        public string Month { get; set; }

        protected override async Task OnInitAsync()
        {
            BindEvents();
            MonthModel = new MonthModel(Int32.Parse(Year), Int32.Parse(Month));

            await LoadDataAsync();
        }

        protected async void Reload()
        {
            await LoadDataAsync();
            StateHasChanged();
        }

        protected async Task LoadDataAsync()
            => Models = await Queries.QueryAsync(new ListMonthOutcomeFromCategory(KeyFactory.Empty(typeof(Category)), MonthModel));

        protected async void OnDeleteClick(OutcomeOverviewModel model)
            => await Commands.HandleAsync(new DeleteOutcome(model.Key));

        protected OutcomeOverviewModel FindModel(IEvent payload)
            => Models.FirstOrDefault(o => o.Key.Equals(payload.AggregateKey));

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

        Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        {
            MonthModel payloadMonth = payload.When;
            if (MonthModel == payloadMonth)
                Reload();

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
        {
            OutcomeOverviewModel model = FindModel(payload);
            if (model != null)
                Models.Remove(model);

            StateHasChanged();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
        {
            OutcomeOverviewModel model = FindModel(payload);
            if (model != null)
            {
                // TODO: We can do even better.
                Reload();
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDescriptionChanged>.HandleAsync(OutcomeDescriptionChanged payload)
        {
            OutcomeOverviewModel model = FindModel(payload);
            if (model != null)
            {
                // TODO: We can do even better.
                Reload();
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            OutcomeOverviewModel model = FindModel(payload);
            if (model != null)
            {
                // TODO: We can do even better.
                Reload();
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
