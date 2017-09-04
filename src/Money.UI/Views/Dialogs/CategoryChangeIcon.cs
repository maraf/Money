using Money.Services.Models.Queries;
using Money.ViewModels.Parameters;
using Money.Views.Navigation;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Dialogs
{
    [NavigationParameter(typeof(CategoryChangeIconParameter))]
    public class CategoryChangeIcon : IWizard
    {
        private readonly IDomainFacade domainFacade = ServiceProvider.DomainFacade;
        private readonly IQueryDispatcher queryDispatcher = ServiceProvider.QueryDispatcher;

        public async Task ShowAsync(object rawParameter)
        {
            CategoryChangeIconParameter parameter = (CategoryChangeIconParameter)rawParameter;

            IconPicker dialog = new IconPicker();

            string icon = await queryDispatcher.QueryAsync(new GetCategoryIcon(parameter.CategoryKey));
            dialog.Value = icon;

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
                await domainFacade.ChangeCategoryIconAsync(parameter.CategoryKey, dialog.Value);
        }
    }
}
