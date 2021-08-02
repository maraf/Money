using Microsoft.AspNetCore.Components;
using Money.Components;
using Money.Components.Bootstrap;
using Money.Events;
using Money.Models;
using Money.Models.Loading;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Logging;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class Summary<T> :
        System.IDisposable,
        IEventHandler<OutcomeCreated>,
        IEventHandler<OutcomeDeleted>,
        IEventHandler<OutcomeAmountChanged>,
        IEventHandler<OutcomeWhenChanged>,
        IEventHandler<PulledToRefresh>,
        IEventHandler<IncomeCreated>,
        IEventHandler<IncomeDeleted>
    {
        private CurrencyFormatter formatter;

        [Inject]
        public ICommandDispatcher Commands { get; set; }

        [Inject]
        public IEventHandlerCollection EventHandlers { get; set; }

        [Inject]
        public IQueryDispatcher Queries { get; set; }

        [Inject]
        internal ILog<Summary<T>> Log { get; set; }

        [Inject]
        internal Navigator Navigator { get; set; }

        [Inject]
        protected CurrencyFormatterFactory CurrencyFormatterFactory { get; set; }

        protected string SubTitle { get; set; }

        protected LoadingContext PeriodsLoading { get; } = new LoadingContext();
        protected List<T> Periods { get; private set; }
        protected T SelectedPeriod { get; set; }
        protected IReadOnlyCollection<T> PeriodGuesses { get; set; }

        protected LoadingContext CategoriesLoading { get; } = new LoadingContext();
        protected Price IncomeTotal { get; private set; }
        protected Price ExpenseTotal { get; private set; }
        protected List<CategoryWithAmountModel> Categories { get; private set; }

        protected SortDescriptor<SummarySortType> SortDescriptor { get; set; }

        protected Modal PeriodSelectorModal { get; set; }
        protected IncomeCreate IncomeCreate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Log.Debug("Summary.OnInitializedAsync");
            BindEvents();
            SortDescriptor = new SortDescriptor<SummarySortType>(SummarySortType.ByCategory, SortDirection.Ascending);

            await base.OnInitializedAsync();

            formatter = await CurrencyFormatterFactory.CreateAsync();
        }

        protected virtual void ClearPreviousParameters()
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(ClearPreviousParameters)}'.");

        protected virtual (T, IReadOnlyCollection<T>) CreateSelectedPeriodFromParameters()
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateSelectedPeriodFromParameters)}'.");

        public override Task SetParametersAsync(ParameterView parameters)
        {
            ClearPreviousParameters();
            return base.SetParametersAsync(parameters);
        }

        protected async override Task OnParametersSetAsync()
        {
            (SelectedPeriod, PeriodGuesses) = CreateSelectedPeriodFromParameters();
            await LoadSelectedPeriodAsync();
        }

        protected virtual IQuery<List<T>> CreatePeriodsQuery()
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreatePeriodsQuery)}'.");

        protected async Task LoadPeriodsAsync(bool isReload = true)
        {
            using (PeriodsLoading.Start())
            {
                if (isReload || Periods == null)
                    Periods = await Queries.QueryAsync(CreatePeriodsQuery());
            }
        }

        protected async Task LoadSelectedPeriodAsync()
        {
            if (SelectedPeriod != null)
            {
                using (CategoriesLoading.Start())
                {
                    try
                    {
                        Categories = await Queries.QueryAsync(CreateCategoriesQuery(SelectedPeriod));

                        await LoadIncomeTotalAsync();

                        ExpenseTotal = await Queries.QueryAsync(CreateExpenseTotalQuery(SelectedPeriod));
                    }
                    catch (MissingDefaultCurrentException)
                    {
                        Navigator.OpenCurrencies();
                    }
                }

                Sort();
            }
        }

        private async Task LoadIncomeTotalAsync()
        {
            var incomeQuery = CreateIncomeTotalQuery(SelectedPeriod);
            if (incomeQuery != null)
                IncomeTotal = await Queries.QueryAsync(CreateIncomeTotalQuery(SelectedPeriod));
            else
                IncomeTotal = null;
        }

        protected virtual IQuery<Price> CreateIncomeTotalQuery(T item)
            => null;

        protected virtual IQuery<Price> CreateExpenseTotalQuery(T item)
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateExpenseTotalQuery)}'.");

        protected virtual IQuery<List<CategoryWithAmountModel>> CreateCategoriesQuery(T item)
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(CreateCategoriesQuery)}'.");

        protected void OnSorted()
        {
            Sort();
            StateHasChanged();
        }

        private void Sort()
        {
            Log.Debug($"Sorting: Type='{SortDescriptor.Type}', Direction='{SortDescriptor.Direction}'.");

            switch (SortDescriptor.Type)
            {
                case SummarySortType.ByAmount:
                    Categories.Sort(SortDescriptor.Direction, x => x.TotalAmount.Value);
                    break;
                case SummarySortType.ByCategory:
                    Categories.Sort(SortDescriptor.Direction, x => x.Name);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(SortDescriptor.Type.ToString());
            }
        }

        protected string GetPercentualValue(CategoryWithAmountModel category)
        {
            decimal percentage = 0;
            decimal total = Categories.Sum(c => c.TotalAmount.Value);
            if (total != 0)
                percentage = 100 / total * category.TotalAmount.Value;

            return percentage.ToString("0.##", CultureInfo.InvariantCulture);
        }

        protected string FormatPrice(Price price)
            => formatter.Format(price);

        public void Dispose()
            => UnBindEvents();

        #region Navigations

        protected virtual string UrlSummary(T item) => null;

        protected virtual void OpenOverview(T item)
        { }

        protected virtual void OpenOverviewIncomes(T item)
        { }

        protected virtual void OpenOverview(T item, IKey categorykey)
        { }

        #endregion

        #region Events

        private void BindEvents()
        {
            EventHandlers
                .Add<OutcomeCreated>(this)
                .Add<OutcomeDeleted>(this)
                .Add<OutcomeAmountChanged>(this)
                .Add<OutcomeWhenChanged>(this)
                .Add<PulledToRefresh>(this)
                .Add<IncomeCreated>(this)
                .Add<IncomeDeleted>(this);
        }

        private void UnBindEvents()
        {
            EventHandlers
                .Remove<OutcomeCreated>(this)
                .Remove<OutcomeDeleted>(this)
                .Remove<OutcomeAmountChanged>(this)
                .Remove<OutcomeWhenChanged>(this)
                .Remove<PulledToRefresh>(this)
                .Remove<IncomeCreated>(this)
                .Remove<IncomeDeleted>(this);
        }

        Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        {
            OnMonthUpdatedEvent(payload.When);
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeDeleted>.HandleAsync(OutcomeDeleted payload)
        {
            OnOutcomeDeletedEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeAmountChanged>.HandleAsync(OutcomeAmountChanged payload)
        {
            OnOutcomeAmountChangedEvent();
            return Task.CompletedTask;
        }

        Task IEventHandler<OutcomeWhenChanged>.HandleAsync(OutcomeWhenChanged payload)
        {
            OnMonthUpdatedEvent(payload.When);
            return Task.CompletedTask;
        }

        async Task IEventHandler<PulledToRefresh>.HandleAsync(PulledToRefresh payload)
        {
            payload.IsHandled = true;
            await LoadSelectedPeriodAsync();
            StateHasChanged();
        }

        async Task IEventHandler<IncomeCreated>.HandleAsync(IncomeCreated payload)
        {
            await LoadIncomeTotalAsync();
            StateHasChanged();
        }

        async Task IEventHandler<IncomeDeleted>.HandleAsync(IncomeDeleted payload)
        {
            await LoadIncomeTotalAsync();
            StateHasChanged();
        }

        private async void OnMonthUpdatedEvent(DateTime changed)
        {
            if (Periods != null && !IsContained(changed))
                await LoadPeriodsAsync();

            await LoadSelectedPeriodAsync();
            StateHasChanged();
        }

        protected virtual bool IsContained(DateTime changed)
            => throw Ensure.Exception.NotImplemented($"Missing override for method '{nameof(IsContained)}'.");

        private async void OnOutcomeAmountChangedEvent()
        {
            await LoadSelectedPeriodAsync();
            StateHasChanged();
        }

        private async void OnOutcomeDeletedEvent()
        {
            if (Periods != null)
                await LoadPeriodsAsync();

            await LoadSelectedPeriodAsync();
            StateHasChanged();
        }

        protected async Task OpenPeriodSelectorAsync()
        {
            PeriodSelectorModal.Show();
            await LoadPeriodsAsync(isReload: false);
        }

        #endregion
    }
}
