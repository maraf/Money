using Money.Services;
using Money.Services.Models;
using Money.Services.Models.Queries;
using Money.Services.Tiles;
using Money.ViewModels;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.StartScreen;
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
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;
        private readonly INavigator navigator = ServiceProvider.Navigator;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly TileService tileService = ServiceProvider.TileService;

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

            OutcomeViewModel viewModel = new OutcomeViewModel(navigator, domainFacade);
            OutcomeParameter parameter = e.Parameter as OutcomeParameter;
            if (parameter != null)
            {
                if (parameter.Amount != null)
                    viewModel.Amount = (float)parameter.Amount.Value;

                if (parameter.Description != null)
                    viewModel.Description = parameter.Description;

                if (!parameter.CategoryKey.IsEmpty)
                    viewModel.SelectedCategories.Add(parameter.CategoryKey);
            }

            IEnumerable<CategoryModel> categories = await queryDispatcher.QueryAsync(new ListAllCategory());

            viewModel.Categories.AddRange(categories);
            DataContext = viewModel;

            UpdateSelectedCategoriesView();
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
            else if (e.Key == VirtualKey.Escape)
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

        private bool isCategoriesViewChangedAttached = true;

        private void gvwCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isCategoriesViewChangedAttached)
                return;

            foreach (CategoryModel item in e.RemovedItems)
                ViewModel.SelectedCategories.Remove(item.Key);

            foreach (CategoryModel item in e.AddedItems)
                ViewModel.SelectedCategories.Add(item.Key);
        }

        private void UpdateSelectedCategoriesView()
        {
            isCategoriesViewChangedAttached = false;
            gvwCategories.SelectedItems.Clear();

            if (ViewModel.SelectedCategories.Count > 0)
            {
                foreach (IKey categoryKey in ViewModel.SelectedCategories)
                    gvwCategories.SelectedItems.Add(gvwCategories.Items.OfType<CategoryModel>().First(c => c.Key.Equals(categoryKey)));
            }

            isCategoriesViewChangedAttached = true;
        }

        private void tbxAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxAmount.SelectAll();
        }

        private async void abbPin_Click(object sender, RoutedEventArgs e)
        {
            await tileService.PinOutcomeCreate(
                ViewModel.SelectedCategories.FirstOrDefault() ?? KeyFactory.Empty(typeof(Category))
            );
        }
    }
}
