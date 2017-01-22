using Money.Services.Models;
using Money.Services.Models.Queries;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Money.Views.Dialogs
{
    public sealed partial class CategoryPicker : ContentDialog
    {
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private List<CategoryModel> models;

        public IKey SelectedKey
        {
            get { return (IKey)GetValue(SelectedKeyProperty); }
            set { SetValue(SelectedKeyProperty, value); }
        }

        public static readonly DependencyProperty SelectedKeyProperty = DependencyProperty.Register(
            "SelectedKey",
            typeof(IKey),
            typeof(OutcomeAmount),
            new PropertyMetadata(KeyFactory.Empty(typeof(Category)), OnSelectedKeyChanged)
        );

        private static void OnSelectedKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CategoryPicker control = (CategoryPicker)d;
            control.OnSelectedKeyChanged(e);
        }

        public CategoryPicker()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnSelectedKeyChanged(DependencyPropertyChangedEventArgs e)
        {
            isCategoriesViewChangedAttached = false;

            if (SelectedKey.IsEmpty)
                gvwCategories.SelectedItem = null;
            else
                gvwCategories.SelectedItem = gvwCategories.Items.OfType<CategoryModel>().FirstOrDefault(c => c.Key.Equals(SelectedKey));
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            models = (await queryDispatcher.QueryAsync(new ListAllCategory())).ToList();
            gvwCategories.ItemsSource = models;
        }

        private void tbxSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                if (sender == tbxSearch)
                {
                    if (tbxSearch.Text == String.Empty)
                        Hide();
                    else
                        tbxSearch.Text = String.Empty;

                    e.Handled = true;
                }
            }
        }

        private bool isCategoriesViewChangedAttached = true;

        private void gvwCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isCategoriesViewChangedAttached)
                return;

            if (e.AddedItems.Count > 0)
                SelectedKey = ((CategoryModel)e.AddedItems[0]).Key;
            else
                SelectedKey = KeyFactory.Empty(typeof(Category));
        }
    }
}
