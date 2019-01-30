using Neptuo.Events;
using Money.Events;
using Money.Models;
using Money.Models.Queries;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Neptuo.Commands;

namespace Money.Views
{
    [NavigationParameter(typeof(CategoryListParameter))]
    public sealed partial class CategoryList : Page, INavigatorPage
    {
        private readonly List<UiThreadEventHandler> handlers = new List<UiThreadEventHandler>();

        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly IEventHandlerCollection eventHandlers = ServiceProvider.EventHandlers;
        private readonly INavigator navigator = ServiceProvider.Navigator;

        public event EventHandler ContentLoaded;

        public CategoryListViewModel ViewModel
        {
            get { return (CategoryListViewModel)DataContext; }
            set { DataContext = value; }
        }

        public CategoryList()
        {
            InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            CategoryListParameter parameter = e.GetParameterOrDefault<CategoryListParameter>();

            ViewModel = new CategoryListViewModel(commandDispatcher, navigator);

            // Bind events.
            handlers.Add(eventHandlers.AddUiThread<CategoryCreated>(ViewModel, Dispatcher));
            handlers.Add(eventHandlers.AddUiThread<CategoryRenamed>(ViewModel, Dispatcher));
            handlers.Add(eventHandlers.AddUiThread<CategoryDescriptionChanged>(ViewModel, Dispatcher));
            handlers.Add(eventHandlers.AddUiThread<CategoryColorChanged>(ViewModel, Dispatcher));
            handlers.Add(eventHandlers.AddUiThread<CategoryIconChanged>(ViewModel, Dispatcher));
            handlers.Add(eventHandlers.AddUiThread<CategoryDeleted>(ViewModel, Dispatcher));

            // Just to show the loading wheel.
            await Task.Delay(100);
            
            IEnumerable<CategoryModel> models = await queryDispatcher.QueryAsync(new ListAllCategory());
            foreach (CategoryModel model in models)
            {
                CategoryEditViewModel viewModel = new CategoryEditViewModel(commandDispatcher, navigator, model.Key, model.Name, model.Description, model.Color, model.Icon);
                if (parameter.Key.Equals(model.Key))
                    viewModel.IsSelected = true;

                ViewModel.Items.Add(viewModel);
            }

            UpdateSelectedItemView();
            ContentLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            foreach (UiThreadEventHandler handler in handlers)
                handler.Remove(eventHandlers);
        }

        private void lvwItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CategoryEditViewModel viewModel in ViewModel.Items)
                viewModel.IsSelected = e.AddedItems.FirstOrDefault() == viewModel;
        }

        private void UpdateSelectedItemView()
        {
            lvwItems.SelectedItem = ViewModel.Items.FirstOrDefault(i => i.IsSelected);
        }

        private void UpdateSelectedItemViewModel()
        {
            foreach (CategoryEditViewModel viewModel in ViewModel.Items)
                viewModel.IsSelected = lvwItems.SelectedItem == viewModel;
        }
    }
}
