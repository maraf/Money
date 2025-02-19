﻿using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class ExpenseBag : IDisposable, 
        ExpenseCard.IContext,
        IEventHandler<LocallyStoredExpenseCreated>, 
        IEventHandler<LocallyStoredExpensesPublished>
    {
        [Inject]
        internal IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        internal IQueryDispatcher Queries { get; set; }

        [Inject]
        internal CreateExpenseStorage Storage { get; set; }

        [Inject]
        internal LocalExpenseOnlineRunner Runner { get; set; }

        [Inject]
        internal ILog<ExpenseBag> Log { get; set; }

        [Parameter]
        public RenderFragment BeforeContent { get; set; }

        private List<CreateOutcome> models;
        protected List<OutcomeOverviewModel> Models { get; } = new List<OutcomeOverviewModel>();
        protected LoadingContext Loading { get; } = new LoadingContext();

        protected async override Task OnInitializedAsync()
        {
            EventHandlers.Add<LocallyStoredExpenseCreated>(this);
            EventHandlers.Add<LocallyStoredExpensesPublished>(this);

            await base.OnInitializedAsync();
            await LoadAsync();
        }

        public void Dispose()
        {
            EventHandlers.Remove<LocallyStoredExpenseCreated>(this);
            EventHandlers.Remove<LocallyStoredExpensesPublished>(this);
        }

        private async Task LoadAsync()
        {
            using (Loading.Start())
            {
                Models.Clear();

                models = await Storage.LoadAsync();
                if (models != null)
                    Models.AddRange(models.Select(c => new OutcomeOverviewModel(c.Key, c.Amount, c.When, c.Description, c.CategoryKey)));
            }
        }

        async Task IEventHandler<LocallyStoredExpenseCreated>.HandleAsync(LocallyStoredExpenseCreated payload)
        {
            await LoadAsync();
            StateHasChanged();
        }

        async Task IEventHandler<LocallyStoredExpensesPublished>.HandleAsync(LocallyStoredExpensesPublished payload)
        {
            await LoadAsync();
            StateHasChanged();
        }

        bool ExpenseCard.IContext.HasEdit => true;

        void ExpenseCard.IContext.Duplicate(IExpenseOverviewModel model)
        { }

        void ExpenseCard.IContext.CreateTemplate(IExpenseOverviewModel model)
        { }

        void ExpenseCard.IContext.EditAmount(IExpenseOverviewModel model)
        { }

        void ExpenseCard.IContext.EditDescription(IExpenseOverviewModel model)
        { }

        void ExpenseCard.IContext.EditWhen(IExpenseOverviewModel model)
        { }

        async void ExpenseCard.IContext.Delete(IExpenseOverviewModel model)
        {
            Log.Debug($"Deleting '{model.Key}'.");

            if (models != null)
            {
                var command = models.FirstOrDefault(m => m.Key.Equals(model.Key));
                if (command != null)
                {
                    Log.Debug($"Command found.");

                    var modelToRemove = Models.Single(m => m.Key.Equals(model.Key));
                    Models.Remove(modelToRemove);

                    models.Remove(command);
                    await Storage.SaveAsync(models);


                    Log.Debug($"Rerendering.");
                    StateHasChanged();
                }
            }
        }
    }
}
