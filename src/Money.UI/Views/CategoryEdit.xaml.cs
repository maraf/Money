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
    }
}
