using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.ViewModels;
using Money.ViewModels.Parameters;
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
    public sealed partial class OutcomeCreate : Page
    {
        public OutcomeViewModel ViewModel
        {
            get { return (OutcomeViewModel)DataContext; }
        }

        public OutcomeCreate()
        {
            this.InitializeComponent();

            dprWhen.MaxWidth = Double.PositiveInfinity;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            OutcomeViewModel viewModel = new OutcomeViewModel(ServiceProvider.Navigator, ServiceProvider.DomainFacade);
            OutcomeParameter parameter = e.Parameter as OutcomeParameter;
            if (parameter != null)
            {
                if (parameter.Amount != null)
                    viewModel.Amount = (float)parameter.Amount.Value;

                if (parameter.Description != null)
                    viewModel.Description = parameter.Description;
            }

            IEnumerable<CategoryModel> categories = await ServiceProvider.QueryDispatcher
                .QueryAsync(new ListAllCategory());

            viewModel.Categories.AddRange(categories);
            DataContext = viewModel;
        }

        private void gvwCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CategoryModel item in e.RemovedItems)
                ViewModel.SelectedCategories.Remove(item.Key);

            foreach (CategoryModel item in e.AddedItems)
                ViewModel.SelectedCategories.Add(item.Key);
        }
    }
}
