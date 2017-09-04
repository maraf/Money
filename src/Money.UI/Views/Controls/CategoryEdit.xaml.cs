using Money.ViewModels;
using Money.ViewModels.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;

namespace Money.Views.Controls
{
    public sealed partial class CategoryEdit : UserControl
    {
        public CategoryEdit()
        {
            InitializeComponent();
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            CategoryEditViewModel viewModel = e.NewValue as CategoryEditViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged += OnViewModelPropertyChanged;

                if (viewModel.Key.IsEmpty)
                    flyRename.ShowAt(btnRename);
            }
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CategoryEditViewModel.Color))
                flyColor.Hide();
            else if (e.PropertyName == nameof(CategoryEditViewModel.Name) || e.PropertyName == nameof(CategoryEditViewModel.Description))
                flyRename.Hide();
        }

        private void flyRename_Opened(object sender, object e)
        {
            tbxName.Focus(FocusState.Keyboard);
        }

        private void tbxName_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                tbxDescription.Focus(FocusState.Keyboard);
                e.Handled = true;
            }
        }

        private void tbxDescription_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (btnSave.Command.CanExecute(null))
                    btnSave.Command.Execute(null);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            flyRename.Hide();
        }
    }
}
