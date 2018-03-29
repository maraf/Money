using Money.Commands;
using Money.ViewModels;
using Money.Views.Dialogs;
using Neptuo.Commands;
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

namespace Money.Views
{
    public sealed partial class CurrencyEdit : UserControl
    {
        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;

        public CurrencyEditViewModel ViewModel
        {
            get { return (CurrencyEditViewModel)DataContext; }
        }

        public CurrencyEdit()
        {
            InitializeComponent();
        }

        private async void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            CurrencyName dialog = new CurrencyName();
            dialog.UniqueCode = ViewModel.UniqueCode;
            dialog.IsUniqueCodeEnabled = false;
            dialog.Symbol = ViewModel.Symbol;

            dialog.PrimaryButtonText = "Update";

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary || dialog.IsEnterPressed && ViewModel.Symbol != dialog.Symbol)
                await commandDispatcher.HandleAsync(new ChangeCurrencySymbol(ViewModel.UniqueCode, dialog.Symbol));
        }
    }
}
