using Money.Events;
using Money.Services.Models.Queries;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Neptuo.Threading.Tasks;
using Windows.UI.Core;
using Money.Services.Models;

namespace Money.Views
{
    [NavigationParameter(typeof(CurrencyParameter))]
    public sealed partial class CurrencyList : Page, INavigatorPage,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyDefaultChanged>,
        IEventHandler<CurrencyExchangeRateSet>
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly IEventHandlerCollection eventHandlers = ServiceProvider.EventHandlers;

        public event EventHandler ContentLoaded;

        public CurrencyListViewModel ViewModel
        {
            get { return (CurrencyListViewModel)DataContext; }
            set { DataContext = value; }
        }

        public CurrencyList()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            eventHandlers
                .Add<CurrencyCreated>(this)
                .Add<CurrencyDefaultChanged>(this)
                .Add<CurrencyExchangeRateSet>(this);

            CurrencyParameter parameter = (CurrencyParameter)e.Parameter;

            IEnumerable<CurrencyModel> models = await queryDispatcher.QueryAsync(new ListAllCurrency());

            ViewModel = new CurrencyListViewModel(domainFacade, navigator);

            foreach (CurrencyModel model in models)
                ViewModel.Items.Add(new CurrencyEditViewModel(navigator, domainFacade, queryDispatcher, model.UniqueCode, model.Symbol));

            UpdateStandalone();

            CurrencyModel defaultModel = models.FirstOrDefault(c => c.IsDefault);
            if (defaultModel != null)
                UpdateDefaultCurrency(defaultModel.UniqueCode);

            ContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            eventHandlers
                .Remove<CurrencyCreated>(this)
                .Remove<CurrencyDefaultChanged>(this)
                .Remove<CurrencyExchangeRateSet>(this);
        }

        private void UpdateDefaultCurrency(string name)
        {
            foreach (CurrencyEditViewModel viewModel in ViewModel.Items)
                viewModel.IsDefault = viewModel.UniqueCode == name;
        }

        private void UpdateStandalone()
        {
            bool isStandalone = ViewModel.Items.Count < 2;
            foreach (CurrencyEditViewModel viewModel in ViewModel.Items)
                viewModel.IsStandalone = isStandalone;
        }

        private void lvwItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CurrencyEditViewModel viewModel in ViewModel.Items)
                viewModel.IsSelected = e.AddedItems.FirstOrDefault() == viewModel;
        }

        private void UpdateSelectedItemView()
        {
            lvwItems.SelectedItem = ViewModel.Items.FirstOrDefault(i => i.IsSelected);
        }

        private void UpdateSelectedItemViewModel()
        {
            foreach (CurrencyEditViewModel viewModel in ViewModel.Items)
                viewModel.IsSelected = lvwItems.SelectedItem == viewModel;
        }

        async Task IEventHandler<CurrencyCreated>.HandleAsync(CurrencyCreated payload)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ViewModel.Items.Add(new CurrencyEditViewModel(navigator, domainFacade, queryDispatcher, payload.UniqueCode, payload.Symbol));
                UpdateStandalone();
            });
        }

        async Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateDefaultCurrency(payload.UniqueCode));
        }

        async Task IEventHandler<CurrencyExchangeRateSet>.HandleAsync(CurrencyExchangeRateSet payload)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                CurrencyEditViewModel viewModel = ViewModel.Items.FirstOrDefault(c => c.UniqueCode == payload.TargetUniqueCode);
                if (viewModel != null)
                {
                    viewModel.ExchangeRates.Add(new ExchangeRateModel(
                        payload.SourceUniqueCode,
                        payload.Rate,
                        payload.ValidFrom
                    ));
                    viewModel.ExchangeRates.SortDescending(r => r.ValidFrom);
                }
            });
        }
    }
}