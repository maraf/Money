using Money.Services.Models;
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
using System.ComponentModel;
using Money.Services;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Money.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [NavigationParameter(typeof(CategoryListParameter))]
    public sealed partial class CategoryList : Page
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

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

            CategoryListParameter parameter = (CategoryListParameter)e.Parameter;

            ViewModel = new CategoryListViewModel();

            IEnumerable<CategoryModel> models = await queryDispatcher.QueryAsync(new ListAllCategory());
            foreach (CategoryModel model in models)
            {
                CategoryEditViewModel viewModel = new CategoryEditViewModel(domainFacade, model.Key, model.Name, model.Description, model.Color);
                viewModel.PropertyChanged += OnItemViewModelPropertyChanged;
                if (parameter.Key.Equals(model.Key))
                    viewModel.IsSelected = true;

                ViewModel.Items.Add(viewModel);
            }

            UpdateSelectedItemView();
        }

        private void OnItemViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CategoryEditViewModel.Color))
            {
                //ListViewItem item = (ListViewItem)lvwItems.ContainerFromItem(sender);
                //FrameworkElement fe = (FrameworkElement)item.FindName("s1");
                //Button button = (Button)(StackPanel)(item.Content).Find("btnColor");
                //button.Flyout.Hide();
            }
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
