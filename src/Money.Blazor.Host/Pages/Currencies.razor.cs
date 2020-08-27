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
    public partial class Currencies : 
        IDisposable,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencyDeleted>
    {
        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        protected ModalDialog CreateModal { get; set; }
        protected ModalDialog EditModal { get; set; }
        protected ModalDialog ListExchangeRateModal { get; set; }
        protected ModalDialog CreateExchangeRateModal { get; set; }

        public List<CurrencyModel> Models { get; private set; } = new List<CurrencyModel>();
        public CurrencyModel Selected { get; protected set; }
        protected LoadingContext Loading { get; } = new LoadingContext();

        protected string DeleteMessage { get; set; }
        protected Confirm DeleteConfirm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            BindEvents();

            using (Loading.Start())
                await LoadDataAsync();
        }

        protected async void OnEvent()
        {
            await LoadDataAsync();
            StateHasChanged();
        }

        protected async Task LoadDataAsync()
            => Models = await Queries.QueryAsync(new ListAllCurrency());

        protected void OnActionClick(CurrencyModel model, ModalDialog modal)
        {
            Selected = model;
            modal.Show();
            StateHasChanged();
        }

        protected void OnDeleteClick(CurrencyModel model)
        {
            Selected = model;
            DeleteMessage = $"Do you really want to delete currency '{model.Symbol}'?";
            DeleteConfirm.Show();
            StateHasChanged();
        }

        protected async void OnDeleteConfirmed()
        {
            await Commands.HandleAsync(new DeleteCurrency(Selected.UniqueCode));
            StateHasChanged();
        }

        protected async Task OnChangeDefaultClickAsync(CurrencyModel model)
           => await Commands.HandleAsync(new SetCurrencyAsDefault(model.UniqueCode));

        protected void OnAddExchangeRateClick()
        {
            CreateExchangeRateModal.Show();
            StateHasChanged();
        }

        public void Dispose()
            => UnBindEvents();

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<CurrencyCreated>(this)
                .Add<CurrencySymbolChanged>(this)
                .Add<CurrencyDefaultChanged>(this)
                .Add<CurrencyDeleted>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<CurrencyCreated>(this)
                .Remove<CurrencySymbolChanged>(this)
                .Remove<CurrencyDefaultChanged>(this)
                .Remove<CurrencyDeleted>(this);
        }

        Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            Models.Add(new CurrencyModel(payload.UniqueCode, payload.Symbol, false));
            Models.Sort((a, b) => a.Symbol.CompareTo(b.Symbol));
            StateHasChanged();
            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencySymbolChanged>.HandleAsync(CurrencySymbolChanged payload)
        {
            CurrencyModel model = Models.FirstOrDefault(c => c.UniqueCode == payload.UniqueCode);
            if (model != null)
            {
                model.Symbol = payload.Symbol;
                StateHasChanged();
            }
            else
            {
                OnEvent();
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            CurrencyModel model = Models.FirstOrDefault(c => c.IsDefault);
            if (model != null)
            {
                model.IsDefault = false;

                model = Models.FirstOrDefault(c => c.UniqueCode == payload.UniqueCode);
                if (model != null)
                {
                    model.IsDefault = true;
                    StateHasChanged();
                    return Task.CompletedTask;
                }
            }

            OnEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyDeleted>.HandleAsync(CurrencyDeleted payload)
        {
            CurrencyModel model = Models.FirstOrDefault(c => c.UniqueCode == payload.UniqueCode);
            if (model != null)
                Models.Remove(model);

            StateHasChanged();
            return Task.CompletedTask;
        }

        #endregion
    }
}
