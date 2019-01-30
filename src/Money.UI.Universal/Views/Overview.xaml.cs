using Money.Commands;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services.Settings;
using Money.ViewModels;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Dialogs;
using Money.Views.Navigation;
using Neptuo.Commands;
using Neptuo.Events;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;
        private readonly IUserPreferenceService userPreferences = ServiceProvider.UserPreferences;

        private IKey categoryKey;
        private MonthModel month;
        private YearModel year;

        public event EventHandler ContentLoaded;

        public OverviewViewModel ViewModel
        {
            get { return (OverviewViewModel)DataContext; }
            set { DataContext = value; }
        }

        public SortDescriptor<OverviewSortType> SortDescriptor
        {
            get { return (SortDescriptor<OverviewSortType>)GetValue(SortDescriptorProperty); }
            set { SetValue(SortDescriptorProperty, value); }
        }

        public static readonly DependencyProperty SortDescriptorProperty = DependencyProperty.Register(
            "SortDescriptor",
            typeof(SortDescriptor<OverviewSortType>),
            typeof(Overview),
            new PropertyMetadata(null, OnSortDescriptorChanged)
        );

        private static void OnSortDescriptorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Overview page = (Overview)d;
            page.ApplySortDescriptor(true);
        }

        public Overview()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OverviewParameter parameter = e.GetParameter<OverviewParameter>();

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
                SortDescriptor = sortDescriptor;
            else
                SortDescriptor = new SortDescriptor<OverviewSortType>(OverviewSortType.ByDate, SortDirection.Ascending);

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
                    ViewModel.Items.Add(new OutcomeOverviewViewModel(model));
            }

            ApplySortDescriptor(false);
        }

        private void ApplySortDescriptor(bool isSaveRequired)
        {
            ViewModel.Sort(SortDescriptor);

            if (isSaveRequired)
                userPreferences.TrySave("Overview.SortDescriptor", SortDescriptor);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            foreach (UiThreadEventHandler handler in handlers)
                handler.Remove(eventHandlers);
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

            ContentDialogResult result = await dialog.ShowAsync(true);
            decimal newValue = dialog.Value;

            if (result == ContentDialogResult.Primary && newValue != viewModel.Amount.Value)
            {
                Price newAmount = new Price(newValue, dialog.Currency);
                await commandDispatcher.HandleAsync(new ChangeOutcomeAmount(viewModel.Key, newAmount));
            }
        }

        private async void btnDescription_Click(object sender, RoutedEventArgs e)
        {
            OutcomeOverviewViewModel viewModel = (OutcomeOverviewViewModel)((Button)sender).DataContext;

            OutcomeDescription dialog = new OutcomeDescription();
            dialog.Value = viewModel.Description;

            ContentDialogResult result = await dialog.ShowAsync();
            if ((result == ContentDialogResult.Primary || dialog.IsEnterPressed) && dialog.Value != viewModel.Description)
                await commandDispatcher.HandleAsync(new ChangeOutcomeDescription(viewModel.Key, dialog.Value));
        }

        private async void btnWhen_Click(object sender, RoutedEventArgs e)
        {
            OutcomeOverviewViewModel viewModel = (OutcomeOverviewViewModel)((Button)sender).DataContext;

            OutcomeWhen dialog = new OutcomeWhen();
            dialog.Value = viewModel.When;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.Value != viewModel.When)
                await commandDispatcher.HandleAsync(new ChangeOutcomeWhen(viewModel.Key, dialog.Value));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            OutcomeOverviewViewModel viewModel = (OutcomeOverviewViewModel)((Button)sender).DataContext;

            navigator
                .Message(String.Format("Do you realy want to delete an outcome for '{0}' from '{1}'?", viewModel.Amount, viewModel.When))
                .Button("Yes", new DeleteOutcomeCommand(commandDispatcher, viewModel.Key).AddExecuted(() => ViewModel.Items.Remove(viewModel)))
                .ButtonClose("No")
                .Show();
        }
    }
}
