using Money.Commands;
using Money.Models.Queries;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Commands;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Dialogs
{
    [NavigationParameter(typeof(CategoryChangeColorParameter))]
    public class CategoryChangeColor : IWizard
    {
        private readonly ICommandDispatcher commandDispatcher = ServiceProvider.CommandDispatcher;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        public async Task ShowAsync(object rawParameter)
        {
            CategoryChangeColorParameter parameter = (CategoryChangeColorParameter)rawParameter;

            ColorPicker dialog = new ColorPicker();
            dialog.Title = "Change a color";
            dialog.PrimaryButtonText = "Select";
            dialog.SecondaryButtonText = "Cancel";

            Color color = await queryDispatcher.QueryAsync(new GetCategoryColor(parameter.CategoryKey));
            dialog.Value = color;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary && color != dialog.Value)
                await commandDispatcher.HandleAsync(new ChangeCategoryColor(parameter.CategoryKey, dialog.Value));
        }
    }
}
