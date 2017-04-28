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

namespace Money.Views
{
    [NavigationParameter(typeof(CurrencyParameter))]
    public sealed partial class CurrencyList : Page, INavigatorPage,
        IEventHandler<CurrencyCreated>,
        IEventHandler<CurrencyDefaultChanged>
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
                .Add<CurrencyDefaultChanged>(this);

            CurrencyParameter parameter = (CurrencyParameter)e.Parameter;

            IEnumerable<string> models = await queryDispatcher.QueryAsync(new ListAllCurrency());

            ViewModel = new CurrencyListViewModel(domainFacade, navigator);

            foreach (string model in models)
                ViewModel.Items.Add(new CurrencyEditViewModel(navigator, domainFacade, model));

            string defaultModel = await queryDispatcher.QueryAsync(new GetDefaultCurrency());
            UpdateDefaultCurrency(defaultModel);

            ContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            eventHandlers
                .Remove<CurrencyCreated>(this)
                .Remove<CurrencyDefaultChanged>(this);
        }

        private void UpdateDefaultCurrency(string name)
        {
            foreach (CurrencyEditViewModel viewModel in ViewModel.Items)
                viewModel.IsDefault = viewModel.Name == name;
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
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ViewModel.Items.Add(new CurrencyEditViewModel(navigator, domainFacade, payload.Name)));
        }

        async Task IEventHandler<CurrencyDefaultChanged>.HandleAsync(CurrencyDefaultChanged payload)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateDefaultCurrency(payload.Name));
        }
    }
}