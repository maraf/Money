using Microsoft.AspNetCore.Components;
using Money.Commands;
using Money.Components.Bootstrap;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components
{
    public partial class ExchangeRateList : IDisposable,
        IEventHandler<CurrencyExchangeRateSet>,
        IEventHandler<CurrencyExchangeRateRemoved>
    {
        [Inject]
        internal ILog<ExchangeRateList> Log { get; set; }

        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        [Parameter]
        public string TargetCurrency { get; set; }

        [Parameter]
        public Action AddClick { get; set; }

        protected string Title { get; set; }
        protected List<ExchangeRateModel> Models { get; set; }
        protected CurrencyFormatter CurrencyFormatter { get; set; }

        private bool isShown;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            BindEvents();
        }

        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            Title = $"List of Exchange Rates for {TargetCurrency}.";
            if (isShown)
            {
                Models = await Queries.QueryAsync(new ListTargetCurrencyExchangeRates(TargetCurrency));
                CurrencyFormatter = await CurrencyFormatterFactory.CreateAsync();
                isShown = false;
            }
        }

        public override void Show()
        {
            base.Show();
            isShown = true;
        }

        protected void OnAddClick()
        {
            AddClick?.Invoke();
            Modal.Hide();
        }

        protected async Task OnDeleteClickAsync(ExchangeRateModel model)
            => await Commands.HandleAsync(new RemoveExchangeRate(model.SourceCurrency, TargetCurrency, model.ValidFrom, model.Rate));

        public void Dispose()
            => UnBindEvents();

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<CurrencyExchangeRateSet>(this)
                .Add<CurrencyExchangeRateRemoved>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<CurrencyExchangeRateSet>(this)
                .Remove<CurrencyExchangeRateRemoved>(this);
        }

        Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload)
        {
            if (payload.TargetUniqueCode == TargetCurrency)
            {
                Models.Add(new ExchangeRateModel(payload.SourceUniqueCode, payload.Rate, payload.ValidFrom));
                Models.Sort((r1, r2) => r2.ValidFrom.CompareTo(r1.ValidFrom));
                StateHasChanged();
            }

            return Task.CompletedTask;
        }

        Task IEventHandler<CurrencyExchangeRateRemoved>.HandleAsync(CurrencyExchangeRateRemoved payload)
        {
            Log.Debug($"Got '{nameof(CurrencyExchangeRateRemoved)}' with TargetCurrency: '{payload.TargetUniqueCode}'.");
            if (payload.TargetUniqueCode == TargetCurrency)
            {
                ExchangeRateModel model = Models.FirstOrDefault(m => m.SourceCurrency == payload.SourceUniqueCode && m.Rate == payload.Rate && m.ValidFrom == payload.ValidFrom);
                if (model != null)
                {
                    Log.Debug($"Found model.");
                    Models.Remove(model);
                    StateHasChanged();
                }
                else
                {
                    Log.Debug($"Model not found, SourceCurreny: '{payload.SourceUniqueCode}', Rate: '{payload.Rate}', ValidFrom: '{payload.ValidFrom}'.");
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
