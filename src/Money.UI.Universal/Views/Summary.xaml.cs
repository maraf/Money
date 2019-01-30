using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.Models.Sorting;
using Money.Services.Settings;
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
using Windows.Storage;
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
using ColorPicker = Money.Views.Dialogs.ColorPicker;

namespace Money.Views
{
    [NavigationParameter(typeof(SummaryParameter))]
    public sealed partial class Summary : Page, INavigatorPage, INavigatorParameterPage, IEventHandler<OutcomeCreated>
    {
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly TileService tileService = ServiceProvider.TileService;
        private readonly IUserPreferenceService userPreferences = ServiceProvider.UserPreferences;
        private readonly IEventHandlerCollection eventHandlers = ServiceProvider.EventHandlers;
        private readonly List<UiThreadEventHandler> handlers = new List<UiThreadEventHandler>();

        private SummaryParameter parameter;

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

            parameter = e.GetParameterOrDefault<SummaryParameter>();
            await LoadAsync();

            handlers.Add(eventHandlers.AddUiThread(this, Dispatcher));
        }

        private Task ReloadAsync()
        {
            ViewModel.Clear();
            return LoadAsync();
        }

        private async Task LoadAsync()
        {
            if (parameter.ViewType != null)
                ViewModel.ViewType = parameter.ViewType.Value;
            else if (userPreferences.TryLoad("Summary.ViewTypeDescriptor", out SummaryViewTypeDescriptor viewTypeDescriptor))
                ViewModel.ViewType = viewTypeDescriptor.Type;
            else
                ViewModel.ViewType = SummaryViewType.BarGraph;

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
            else if (userPreferences.TryLoad("Summary.SortDescriptor", out SortDescriptor<SummarySortType> sortDescriptor))
                ViewModel.SortDescriptor = sortDescriptor;
            else
                ViewModel.SortDescriptor = new SortDescriptor<SummarySortType>(SummarySortType.ByCategory, SortDirection.Ascending);

            ViewModel.PropertyChanged += OnViewModelChanged;
            ContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void OnViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.SortDescriptor))
            {
                if (ViewModel.SortDescriptor != null)
                    userPreferences.TrySave("Summary.SortDescriptor", ViewModel.SortDescriptor);
            }
            else if (e.PropertyName == nameof(ViewModel.ViewType))
            {
                userPreferences.TrySave("Summary.ViewTypeDescriptor", new SummaryViewTypeDescriptor(ViewModel.ViewType));
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            foreach (GroupItemViewModel viewModel in ViewModel.Items)
            {
                PivotItem item = (PivotItem)pvtGroups.ContainerFromItem(viewModel);
                if (item.ContentTemplateRoot is System.IDisposable content)
                    content.Dispose();
            }

            foreach (UiThreadEventHandler handler in handlers)
                handler.Remove(eventHandlers);
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

        private void pvtGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupItemViewModel viewModel = (GroupItemViewModel)e.AddedItems.FirstOrDefault();
            if (viewModel == null)
                return;

            MonthModel month = viewModel.Parameter as MonthModel;
            if (month != null)
                parameter.Month = month;

            YearModel year = viewModel.Parameter as YearModel;
            if (year != null)
                parameter.Year = year;
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

        private void PieChart_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ViewType = SummaryViewType.PieChart;
        }

        private void BarGraph_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ViewType = SummaryViewType.BarGraph;
        }

        public Task HandleAsync(OutcomeCreated payload)
        {
            bool isReloadRequired = false;
            if (parameter.Month != null)
            {
                MonthModel other = payload.When;
                if (!ViewModel.Items.Any(g => g.Parameter.Equals(other)))
                    isReloadRequired = true;
            }
            else if (parameter.Year != null)
            {
                YearModel other = new YearModel(payload.When.Year);
                if (!ViewModel.Items.Any(g => g.Parameter.Equals(other)))
                    isReloadRequired = true;
            }
            else
            {
                if (!ViewModel.Items.Any())
                    isReloadRequired = true;
            }

            if (isReloadRequired)
                ReloadAsync();

            return Task.CompletedTask;
        }
    }
}
