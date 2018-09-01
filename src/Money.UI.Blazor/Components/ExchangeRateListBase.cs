using Microsoft.AspNetCore.Blazor.Components;
using Money.Commands;
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
    public class ExchangeRateListBase : DialogBase, IDisposable,
        IEventHandler<CurrencyExchangeRateSet>,
        IEventHandler<CurrencyExchangeRateRemoved>
    {
        private ILog log;

        protected ILog Log
        {
            get
            {
                if (log == null)
                    log = LogFactory.Scope("ExchangeRateList");

                return log;
            }
        }

        [Inject]
        public ILogFactory LogFactory { get; set; }

        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        [Parameter]
        protected string TargetCurrency { get; set; }

        [Parameter]
        protected Action AddClick { get; set; }

        protected string Title { get; set; }
        protected List<ExchangeRateModel> Models { get; set; }
        protected CurrencyFormatter CurrencyFormatter { get; set; }

        protected override void OnInit()
        {
            base.OnInit();
            BindEvents();
        }

        protected async override Task OnParametersSetAsync()
        {
            if (IsVisible)
            {
                Title = $"List of Exchange Rates for {TargetCurrency}.";
                await ReloadAsync();
            }
        }

        protected async Task ReloadAsync()
        { 
            Models = await Queries.QueryAsync(new ListTargetCurrencyExchangeRates(TargetCurrency));
            CurrencyFormatter = new CurrencyFormatter(await Queries.QueryAsync(new ListAllCurrency()));
        }

        protected bool OnAddClick()
        {
            AddClick?.Invoke();
            return true;
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
