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
    public class CurrenciesBase : BlazorComponent,
        IDisposable,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencySymbolChanged>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencyDeleted>
    {
        private bool isListExchangeRateReopened;
        private bool isCreateExchangeRateVisible;

        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        protected bool IsCreateVisible { get; set; }
        protected bool IsEditVisible { get; set; }
        protected bool IsListExchangeRateVisible { get; set; }
        protected bool IsCreateExchangeRateVisible
        {
            get => isCreateExchangeRateVisible;
            set
            {
                if (isCreateExchangeRateVisible != value)
                {
                    isCreateExchangeRateVisible = value;
                    if (!isCreateExchangeRateVisible && isListExchangeRateReopened)
                    {
                        isListExchangeRateReopened = false;
                        IsListExchangeRateVisible = true;
                        StateHasChanged();
                    }
                }
            }
        }

        public List<CurrencyModel> Models { get; private set; } = new List<CurrencyModel>();
        public CurrencyModel Selected { get; protected set; }

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
            => Models = await Queries.QueryAsync(new ListAllCurrency());

        protected async void OnDeleteClick(CurrencyModel model)
           => await Commands.HandleAsync(new DeleteCurrency(model.UniqueCode));

        protected async void OnChangeDefaultClick(CurrencyModel model)
            => await Commands.HandleAsync(new SetCurrencyAsDefault(model.UniqueCode));

        protected void OnAddExchangeRateClick()
        {
            isListExchangeRateReopened = true;
            IsCreateExchangeRateVisible = true;
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
