using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views
{
    [NavigationParameter(typeof(OutcomeParameter))]
    public sealed partial class OutcomeCreate : Page
    {
        public OutcomeViewModel ViewModel
        {
            get { return (OutcomeViewModel)DataContext; }
        }

        public OutcomeCreate()
        {
            InitializeComponent();
            Loaded += OnPageLoaded;

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

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            tbxAmount.Focus(FocusState.Keyboard);
        }

        private void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.S)
            {
                CoreVirtualKeyStates controlState = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (controlState.HasFlag(CoreVirtualKeyStates.Down) && ViewModel.Save.CanExecute())
                {
                    ViewModel.Save.Execute();
                    e.Handled = true;
                }
            }
        }

        private void OnInputKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (sender == tbxAmount)
                    tbxDescription.Focus(FocusState.Keyboard);
                else if (sender == tbxDescription)
                    dprWhen.Focus(FocusState.Keyboard);
                else if (sender == dprWhen)
                    gvwCategories.Focus(FocusState.Keyboard);
                else if (sender == gvwCategories && ViewModel.Save.CanExecute())
                    ViewModel.Save.Execute();

                e.Handled = true;
            }
            else if(e.Key == VirtualKey.Escape)
            {
                if (sender == tbxAmount)
                {
                    tbxAmount.Text = "0";
                    tbxAmount.SelectAll();
                    e.Handled = true;
                }
                else if (sender == tbxDescription)
                {
                    tbxDescription.Text = String.Empty;
                    e.Handled = true;
                }
            }
        }

        private void gvwCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (CategoryModel item in e.RemovedItems)
                ViewModel.SelectedCategories.Remove(item.Key);

            foreach (CategoryModel item in e.AddedItems)
                ViewModel.SelectedCategories.Add(item.Key);
        }

        private void tbxAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxAmount.SelectAll();
        }
    }
}
