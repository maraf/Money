using Money.Events;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.Services.Tiles;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Controls;
using Money.Views.Dialogs;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Events;
using Neptuo.Events.Handlers;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(SummaryParameter))]
    public sealed partial class Summary : Page, INavigatorPage, INavigatorParameterPage,
        IEventHandler<OutcomeCreated>
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly IEventHandlerCollection eventHandlers = ServiceProvider.EventHandlers;
        private readonly TileService tileService = ServiceProvider.TileService;
        private SummaryParameter parameter;

        private bool isAmountSorted;
        private bool isCategorySorted = true;

        public event EventHandler ContentLoaded;

        public object Parameter
        {
            get { return parameter; }
        }

        public GroupViewModel ViewModel
        {
            get { return (GroupViewModel)DataContext; }
            private set { DataContext = value; }
        }

        public Summary()
        {
            InitializeComponent();
            ViewModel = new GroupViewModel(navigator);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            eventHandlers.Add(this);

            parameter = (SummaryParameter)e.Parameter;
            await LoadAsync();
        }

        private Task ReloadAsync()
        {
            ViewModel.Clear();
            return LoadAsync();
        }

        private async Task LoadAsync()
        {
            ViewModel.ViewType = parameter.ViewType;

            switch (parameter.PeriodType)
            {
                case SummaryPeriodType.Month:
                    await LoadMonthViewAsync(ViewModel, parameter.Month);
                    break;
                case SummaryPeriodType.Year:
                    await LoadYearViewAsync(ViewModel, parameter.Year);
                    break;
                default:
                    throw Ensure.Exception.NotSupported(parameter.PeriodType.ToString());
            }

            if (parameter.SortDescriptor != null)
                ViewModel.SortDescriptor = parameter.SortDescriptor;

            ContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            eventHandlers.Remove(this);
        }

        private async Task LoadMonthViewAsync(GroupViewModel viewModel, MonthModel prefered)
        {
            ViewModel.IsLoading = true;

            IEnumerable<MonthModel> months = await queryDispatcher.QueryAsync(new ListMonthWithOutcome());
            int? preferedIndex = null;
            int index = 0;
            foreach (MonthModel month in months)
            {
                viewModel.Add(month.ToString(), month);

                if (prefered == month)
                    preferedIndex = index;

                index++;
            }

            if (preferedIndex != null)
                pvtGroups.SelectedIndex = preferedIndex.Value;

            ViewModel.IsLoading = false;
        }

        private async Task LoadYearViewAsync(GroupViewModel viewModel, YearModel prefered)
        {
            throw new NotImplementedException();
        }

        public void DecorateParameter(SummaryParameter parameter)
        {
            if (parameter.Month == null && parameter.Year == null)
            {
                GroupItemViewModel viewModel = pvtGroups.SelectedItem as GroupItemViewModel;
                if (viewModel != null)
                {
                    MonthModel month = viewModel.Parameter as MonthModel;
                    if (month != null)
                    {
                        parameter.Month = month;
                        return;
                    }

                    YearModel year = viewModel.Parameter as YearModel;
                    if (year != null)
                    {
                        parameter.Year = year;
                        return;
                    }
                }
            }
        }

        private void mfiSortAmount_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SortDescriptor = ViewModel.SortDescriptor.Update(SummarySortType.ByAmount, SortDirection.Descending);
            parameter.SortDescriptor = ViewModel.SortDescriptor;
            isAmountSorted = !isAmountSorted;
            isCategorySorted = false;
        }

        private void mfiSortCategory_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SortDescriptor = ViewModel.SortDescriptor.Update(SummarySortType.ByCategory);
            parameter.SortDescriptor = ViewModel.SortDescriptor;
            isCategorySorted = !isCategorySorted;
            isAmountSorted = false;
        }

        private void pvtGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupItemViewModel viewModel = (GroupItemViewModel)e.AddedItems.FirstOrDefault();
            if (viewModel == null)
                return;

            MonthModel month = viewModel.Parameter as MonthModel;
            if (month != null)
                this.parameter.Month = month;

            YearModel year = viewModel.Parameter as YearModel;
            if (year != null)
                this.parameter.Year = year;
        }

        private async void btnPinOutcomeCreate_Click(object sender, RoutedEventArgs e)
        {
            IKey categoryKey = KeyFactory.Empty(typeof(Category));
            Color? background = null;
            string categoryName = null;

            // Select a category.
            CategoryPicker categoryPicker = new CategoryPicker();
            categoryPicker.SelectedKey = categoryKey;

            ContentDialogResult categoryResult = await categoryPicker.ShowAsync();
            if (categoryResult == ContentDialogResult.None)
                return;

            if (categoryResult == ContentDialogResult.Primary)
                categoryKey = categoryPicker.SelectedKey;
            else if (categoryResult == ContentDialogResult.Secondary)
                categoryKey = KeyFactory.Empty(typeof(Category));

            if (!categoryKey.IsEmpty)
            {
                categoryName = await queryDispatcher.QueryAsync(new GetCategoryName(categoryKey));
                background = await queryDispatcher.QueryAsync(new GetCategoryColor(categoryKey));
            }

            // Select a background color.
            ColorPicker backgroundPicker = new ColorPicker();
            backgroundPicker.Title = "Pick a tile background color";
            backgroundPicker.PrimaryButtonText = "Create";

            if (background != null)
                backgroundPicker.Value = background.Value;

            ContentDialogResult backgroundResult = await backgroundPicker.ShowAsync();
            if (backgroundResult == ContentDialogResult.None)
                return;

            // Create a tile.
            await tileService.PinOutcomeCreate(categoryKey, categoryName, backgroundPicker.Value);

            string message = "Tile created";
            if (categoryName != null)
                message += $" for category '{categoryName}'";

            navigator
                .Message(message)
                .Show();
        }

        async Task IEventHandler<OutcomeCreated>.HandleAsync(OutcomeCreated payload)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await ReloadAsync());
        }
    }
}
