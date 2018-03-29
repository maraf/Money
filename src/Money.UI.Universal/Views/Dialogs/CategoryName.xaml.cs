using Money.Commands;
using Money.Models;
using Money.Models.Queries;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo;
using Neptuo.Commands;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    [NavigationParameter(typeof(CategoryCreateParameter))]
    [NavigationParameter(typeof(CategoryRenameParameter))]
    public sealed partial class CategoryName : ContentDialog, IWizard
    {
        public new string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public new static readonly DependencyProperty NameProperty = DependencyProperty.Register(
            "Name",
            typeof(string),
            typeof(CategoryName),
            new PropertyMetadata(null)
        );

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description",
            typeof(string),
            typeof(CategoryName),
            new PropertyMetadata(null)
        );

        public bool IsEnterPressed { get; private set; }

        public CategoryName()
        {
            InitializeComponent();
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
                IsEnterPressed = true;
                e.Handled = true;
                Hide();
            }
        }

        #region IWizard

        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;
        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;
        private IKey key;
        private CategoryNameDescriptionModel model;

        public async Task ShowAsync(object parameter)
        {
            if (parameter is CategoryCreateParameter create)
            {
                key = KeyFactory.Empty(typeof(Category));
                model = new CategoryNameDescriptionModel(null, null);
            }
            else if (parameter is CategoryRenameParameter rename)
            {
                key = rename.Key;
                model = await queryDispatcher.QueryAsync(new GetCategoryNameDescription(key));
                Name = model.Name;
                Description = model.Description;
            }

            ContentDialogResult result = await ShowAsync();
            if ((result == ContentDialogResult.Primary || IsEnterPressed) && (Name != model.Name || Description != model.Description))
            {
                if (key.IsEmpty)
                {
                    if (String.IsNullOrEmpty(Name))
                        return;

                    Color color = ColorConverter.Map(Colors.Black);
                    await commandDispatcher.HandleAsync(new CreateCategory(Name, Description, color));
                    Name = Name;
                }
                else
                {
                    if (Name != model.Name)
                    {
                        await commandDispatcher.HandleAsync(new RenameCategory(key, Name));
                        Name = Name;
                    }

                    if (Description != model.Description)
                    {
                        await commandDispatcher.HandleAsync(new ChangeCategoryDescription(key, Description));
                        Description = Description;
                    }
                }
            }
        }

        #endregion
    }
}
