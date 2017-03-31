using Money.Services;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.ViewModels;
using Money.ViewModels.Commands;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Dialogs;
using Money.Views.Navigation;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(OverviewParameter))]
    public sealed partial class Overview : Page
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;

        private MonthModel month;
        private YearModel year;

        private bool isDateSorted = true;
        private bool isAmountSorted;
        private bool isDescriptionSorted;

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

            object period = null;
            IEnumerable<OutcomeOverviewModel> models = null;
            if (parameter.Month != null)
            {
                month = parameter.Month;
                period = parameter.Month;
                models = await queryDispatcher.QueryAsync(new ListMonthOutcomeFromCategory(parameter.CategoryKey, parameter.Month));
            }

            if (parameter.Year != null)
            {
                year = parameter.Year;
                period = parameter.Year;
                models = await queryDispatcher.QueryAsync(new ListYearOutcomeFromCategory(parameter.CategoryKey, parameter.Year));
            }

            ViewModel = new OverviewViewModel(navigator, parameter.CategoryKey, categoryName, period);
            if (models != null)
            {
                foreach (OutcomeOverviewModel model in models)
                    ViewModel.Items.Add(new OutcomeOverviewViewModel(model));
            }
        }

        private void mfiSortDate_Click(object sender, RoutedEventArgs e)
        {
            if (isDateSorted)
            {
                ViewModel.Items.SortDescending(i => i.When);
                isDateSorted = false;
            }
            else
            {
                ViewModel.Items.Sort(i => i.When);
                isDateSorted = true;
            }

            isAmountSorted = false;
            isDescriptionSorted = false;
        }

        private void mfiSortAmount_Click(object sender, RoutedEventArgs e)
        {
            if (isAmountSorted)
            {
                ViewModel.Items.SortDescending(i => i.Amount.Value);
                isAmountSorted = false;
            }
            else
            {
                ViewModel.Items.Sort(i => i.Amount.Value);
                isAmountSorted = true;
            }

            isDateSorted = false;
            isDescriptionSorted = false;
        }

        private void mfiSortDescription_Click(object sender, RoutedEventArgs e)
        {
            if (isDescriptionSorted)
            {
                ViewModel.Items.SortDescending(i => i.Description);
                isDescriptionSorted = false;
            }
            else
            {
                ViewModel.Items.Sort(i => i.Description);
                isDescriptionSorted = true;
            }

            isAmountSorted = false;
            isDateSorted = false;
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
            dialog.Value = (double)viewModel.Amount.Value;
            dialog.Currency = viewModel.Amount.Currency;

            ContentDialogResult result = await dialog.ShowAsync();
            decimal newValue = (decimal)dialog.Value;
            if (result == ContentDialogResult.Primary && newValue != viewModel.Amount.Value)
            {
                Price newAmount = new Price(newValue, dialog.Currency);
                await domainFacade.ChangeOutcomeAmountAsync(viewModel.Key, newAmount);
                viewModel.Amount = newAmount;
            }
        }

        private async void btnDescription_Click(object sender, RoutedEventArgs e)
        {
            OutcomeOverviewViewModel viewModel = (OutcomeOverviewViewModel)((Button)sender).DataContext;

            OutcomeDescription dialog = new OutcomeDescription();
            dialog.Value = viewModel.Description;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.Value != viewModel.Description)
            {
                await domainFacade.ChangeOutcomeDescriptionAsync(viewModel.Key, dialog.Value);
                viewModel.Description = dialog.Value;
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
                viewModel.When = dialog.Value;

                if (month != null && (dialog.Value.Year != month.Year || dialog.Value.Month != month.Month))
                    ViewModel.Items.Remove(viewModel);

                if (year != null && (dialog.Value.Year != year.Year))
                    ViewModel.Items.Remove(viewModel);
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
