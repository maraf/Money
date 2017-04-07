using Money.Services.Models.Queries;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [NavigationParameter(typeof(CurrencyParameter))]
    public sealed partial class CurrencyList : Page, INavigatorPage
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        public event EventHandler ContentLoaded;

        public CurrencyListViewModel ViewModel
        {
            get { return (CurrencyListViewModel)DataContext; }
            set { DataContext = value; }
        }

        public CurrencyList()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CurrencyParameter parameter = (CurrencyParameter)e.Parameter;

            IEnumerable<string> models = await queryDispatcher.QueryAsync(new ListAllCurrency());
            string defaultModel = await queryDispatcher.QueryAsync(new GetDefaultCurrency());

            ViewModel = new CurrencyListViewModel(domainFacade);

            foreach (string model in models)
            {
                ViewModel.Items.Add(new CurrencyEditViewModel(domainFacade)
                {
                    Name = model
                });
            }

            // TODO: Set a default one.

            ContentLoaded?.Invoke(this, EventArgs.Empty);
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
    }
}