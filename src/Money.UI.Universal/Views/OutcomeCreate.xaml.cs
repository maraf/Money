using Money.Services;
using Money.Models;
using Money.Models.Queries;
using Money.Services.Tiles;
using Money.ViewModels;
using Money.ViewModels.Navigation;
using Money.ViewModels.Parameters;
using Money.Views.Dialogs;
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
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ColorPicker = Money.Views.Dialogs.ColorPicker;
using Neptuo.Commands;

namespace Money.Views
{
    //[NavigationParameter(typeof(OutcomeParameter))]
    public sealed partial class OutcomeCreate : Page
    {
        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;
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

            OutcomeViewModel viewModel = new OutcomeViewModel(navigator, commandDispatcher);
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
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            tbxAmount.Focus(FocusState.Keyboard);
            UpdateSelectedCategoriesView();
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
                if (sender == tbxAmount && tbxAmount.Text != "0")
                {
                    tbxAmount.Text = "0";
                    tbxAmount.SelectAll();
                    e.Handled = true;
                }
                else if (sender == tbxDescription && tbxDescription.Text != String.Empty)
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
                    gvwCategories.SelectedItems.Add(ViewModel.Categories.First(c => c.Key.Equals(categoryKey)));
            }

            isCategoriesViewChangedAttached = true;
        }

        private void tbxAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            tbxAmount.SelectAll();
        }

        private async void abbPin_Click(object sender, RoutedEventArgs e)
        {
            IKey categoryKey = ViewModel.SelectedCategories.FirstOrDefault() ?? KeyFactory.Empty(typeof(Category));
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
                CategoryModel category = ViewModel.Categories.FirstOrDefault(c => c.Key.Equals(categoryKey));
                if (category != null)
                {
                    categoryName = category.Name;
                    background = category.Color;
                }
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
    }
}
