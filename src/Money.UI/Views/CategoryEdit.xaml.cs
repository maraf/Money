using Money.ViewModels;
using Money.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.Views
{
    public sealed partial class CategoryEdit : UserControl
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;

        public CategoryEditViewModel ViewModel
        {
            get { return (CategoryEditViewModel)DataContext; }
        }

        public CategoryEdit()
        {
            InitializeComponent();
        }

        private async void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            if (e.NewValue is CategoryEditViewModel viewModel && viewModel.Key.IsEmpty)
                await RenameCategoryAsync();
        }

        private async void btnColor_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker dialog = new ColorPicker();
            dialog.Value = ViewModel.Color;

            dialog.Title = "Background color";
            dialog.PrimaryButtonText = "Select";
            dialog.SecondaryButtonText = "Cancel";

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && dialog.Value != ViewModel.Color)
            {
                await domainFacade.ChangeCategoryColorAsync(ViewModel.Key, dialog.Value);
                ViewModel.Color = dialog.Value;
            }
        }

        private async void btnRename_Click(object sender, RoutedEventArgs e)
        {
            await RenameCategoryAsync();
        }

        private async Task RenameCategoryAsync()
        {
            CategoryName dialog = new CategoryName();
            dialog.Name = ViewModel.Name;
            dialog.Description = ViewModel.Description;

            ContentDialogResult result = await dialog.ShowAsync();
            if ((result == ContentDialogResult.Primary || dialog.IsEnterPressed) && (dialog.Name != ViewModel.Name || dialog.Description != ViewModel.Description))
            {
                if (ViewModel.Key.IsEmpty)
                {
                    if (String.IsNullOrEmpty(dialog.Name))
                        return;

                    Color color = Colors.Black;
                    ViewModel.Key = await domainFacade.CreateCategoryAsync(dialog.Name, color);
                    ViewModel.Name = dialog.Name;
                    ViewModel.Color = color;
                }
                else if (ViewModel.Name != dialog.Name)
                {
                    await domainFacade.RenameCategoryAsync(ViewModel.Key, dialog.Name);
                    ViewModel.Name = Name;
                }

                if (ViewModel.Description != dialog.Description)
                {
                    await domainFacade.ChangeCategoryDescriptionAsync(ViewModel.Key, dialog.Description);
                    ViewModel.Description = dialog.Description;
                }
            }
        }
    }
}
