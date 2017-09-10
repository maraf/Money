using Money.Events;
using Money.Services;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.Services.Settings;
using Money.ViewModels;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Dialogs;
using Money.Views.Navigation;
using Neptuo.Events;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(OverviewParameter))]
    public sealed partial class Overview : Page, INavigatorPage
    {
        private readonly List<UiThreadEventHandler> handlers = new List<UiThreadEventHandler>();

        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly IEventHandlerCollection eventHandlers = ServiceProvider.EventHandlers;
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;
        private readonly IUserPreferenceService userPreferences = ServiceProvider.UserPreferences;

        private IKey categoryKey;
        private MonthModel month;
        private YearModel year;

        private SortDescriptor<OverviewSortType> sortDescriptor = new SortDescriptor<OverviewSortType>(OverviewSortType.ByDate, SortDirection.Ascending);

        public event EventHandler ContentLoaded;

        public OverviewViewModel ViewModel
        {
            get { return (OverviewViewModel)DataContext; }
            set { DataContext = value; }
        }

        public Overview()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OverviewParameter parameter = (OverviewParameter)e.Parameter;

            string categoryName = parameter.CategoryKey.IsEmpty
                ? "All"
                : await queryDispatcher.QueryAsync(new GetCategoryName(parameter.CategoryKey));

            categoryKey = parameter.CategoryKey;
            object period = null;
            if (parameter.Month != null)
            {
                month = parameter.Month;
                period = parameter.Month;
            }

            if (parameter.Year != null)
            {
                year = parameter.Year;
                period = parameter.Year;

            }

            ViewModel = new OverviewViewModel(navigator, parameter.CategoryKey, categoryName, period);
            ViewModel.Reload += OnViewModelReload;

            handlers.Add(eventHandlers.AddUiThread<OutcomeCreated>(ViewModel, Dispatcher));
            handlers.Add(eventHandlers.AddUiThread<OutcomeAmountChanged>(ViewModel, Dispatcher));
            handlers.Add(eventHandlers.AddUiThread<OutcomeDescriptionChanged>(ViewModel, Dispatcher));
            handlers.Add(eventHandlers.AddUiThread<OutcomeWhenChanged>(ViewModel, Dispatcher));

            if (userPreferences.TryLoad("Overview.SortDescriptor", out SortDescriptor<OverviewSortType> sortDescriptor))
                this.sortDescriptor = sortDescriptor;

            await ReloadAsync();
            ContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        private async void OnViewModelReload()
        {
            await ReloadAsync();
        }

        private async Task ReloadAsync()
        {

            ViewModel.Items.Clear();
            IEnumerable<OutcomeOverviewModel> models = null;
            if (month != null)
                models = await queryDispatcher.QueryAsync(new ListMonthOutcomeFromCategory(categoryKey, month));

            if (year != null)
                models = await queryDispatcher.QueryAsync(new ListYearOutcomeFromCategory(categoryKey, year));

            if (models != null)
            {
                foreach (OutcomeOverviewModel model in models)
                    ViewModel.Items.Add(new OutcomeOverviewViewModel(queryDispatcher, model));
            }

            ApplySortDescriptor(false);
        }

        private void ApplySortDescriptor(bool isSaveRequired)
        {
            if (sortDescriptor != null)
            {
                switch (sortDescriptor.Type)
                {
                    case OverviewSortType.ByDate:
                        if (sortDescriptor.Direction == SortDirection.Ascending)
                            ViewModel.Items.Sort(i => i.When);
                        else
                            ViewModel.Items.SortDescending(i => i.When);
                        break;

                    case OverviewSortType.ByAmount:
                        if (sortDescriptor.Direction == SortDirection.Ascending)
                            ViewModel.Items.Sort(i => i.Amount.Value);
                        else
                            ViewModel.Items.SortDescending(i => i.Amount.Value);
                        break;

                    case OverviewSortType.ByDescription:
                        if (sortDescriptor.Direction == SortDirection.Ascending)
                            ViewModel.Items.Sort(i => i.Description);
                        else
                            ViewModel.Items.SortDescending(i => i.Description);
                        break;
                }
            }

            if (isSaveRequired)
                userPreferences.TrySave("Overview.SortDescriptor", sortDescriptor);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            foreach (UiThreadEventHandler handler in handlers)
                handler.Remove(eventHandlers);
        }

        private void mfiSortDate_Click(object sender, RoutedEventArgs e)
        {
            sortDescriptor = sortDescriptor.Update(OverviewSortType.ByDate);
            ApplySortDescriptor(true);
        }

        private void mfiSortAmount_Click(object sender, RoutedEventArgs e)
        {
            sortDescriptor = sortDescriptor.Update(OverviewSortType.ByAmount, SortDirection.Descending);
            ApplySortDescriptor(true);
        }

        private void mfiSortDescription_Click(object sender, RoutedEventArgs e)
        {
            sortDescriptor = sortDescriptor.Update(OverviewSortType.ByDescription);
            ApplySortDescriptor(true);
        }

        private void lvwItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OutcomeOverviewViewModel selected = (OutcomeOverviewViewModel)e.AddedItems.FirstOrDefault();
            foreach (OutcomeOverviewViewModel viewModel in ViewModel.Items)
                viewModel.IsSelected = selected == viewModel;
        }

        private async void btnAmount_Click(object sender, RoutedEventArgs e)
        {
            OutcomeOverviewViewModel viewModel = (OutcomeOverviewViewModel)((Button)sender).DataContext;

            OutcomeAmount dialog = new OutcomeAmount(queryDispatcher);
            dialog.Value = viewModel.Amount.Value;
            dialog.Currency = viewModel.Amount.Currency;

            ContentDialogResult result = await dialog.ShowAsync();
            decimal newValue = (decimal)dialog.Value;
            if (result == ContentDialogResult.None && dialog.Result != null)
                result = dialog.Result.Value;

            if (result == ContentDialogResult.Primary && newValue != viewModel.Amount.Value)
            {
                Price newAmount = new Price(newValue, dialog.Currency);
                await domainFacade.ChangeOutcomeAmountAsync(viewModel.Key, newAmount);
                //viewModel.Amount = newAmount;
            }
        }

        private async void btnDescription_Click(object sender, RoutedEventArgs e)
        {
            OutcomeOverviewViewModel viewModel = (OutcomeOverviewViewModel)((Button)sender).DataContext;

            OutcomeDescription dialog = new OutcomeDescription();
            dialog.Value = viewModel.Description;

            ContentDialogResult result = await dialog.ShowAsync();
            if ((result == ContentDialogResult.Primary || dialog.IsEnterPressed) && dialog.Value != viewModel.Description)
            {
                await domainFacade.ChangeOutcomeDescriptionAsync(viewModel.Key, dialog.Value);
                //viewModel.Description = dialog.Value;
            }
        }

        private async void btnWhen_Click(object sender, RoutedEventArgs e)
        {
            OutcomeOverviewViewModel viewModel = (OutcomeOverviewViewModel)((Button)sender).DataContext;

            OutcomeWhen dialog = new OutcomeWhen();
            dialog.Value = viewModel.When;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.Value != viewModel.When)
            {
                await domainFacade.ChangeOutcomeWhenAsync(viewModel.Key, dialog.Value);
                //viewModel.When = dialog.Value;

                //if (month != null && (dialog.Value.Year != month.Year || dialog.Value.Month != month.Month))
                //    ViewModel.Items.Remove(viewModel);

                //if (year != null && (dialog.Value.Year != year.Year))
                //    ViewModel.Items.Remove(viewModel);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OutcomeOverviewViewModel viewModel = (OutcomeOverviewViewModel)((Button)sender).DataContext;

            navigator
                .Message(String.Format("Do you realy want to delete an outcome for '{0}' from '{1}'?", viewModel.Amount, viewModel.When))
                .Button("Yes", new DeleteOutcomeCommand(domainFacade, viewModel.Key).AddExecuted(() => ViewModel.Items.Remove(viewModel)))
                .ButtonClose("No")
                .Show();
        }
    }
}
